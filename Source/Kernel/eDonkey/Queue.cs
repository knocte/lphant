#region Copyright (c)2003 Juanjo < http://lphant.sourceforge.net >
/*
* This file is part of eLePhant
* Copyright (C)2003 Juanjo < j_u_a_n_j_o@users.sourceforge.net / http://lphant.sourceforge.net >
*
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either
* version 2 of the License,or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; if not,write to the Free Software
* Foundation,Inc.,675 Mass Ave,Cambridge,MA 02139,USA.
*/
#endregion

using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// eDonkey Queue for upload
	/// </summary>
	internal class CQueue
	{
		public ArrayList UploadList;

		private ArrayList m_QueueList;
		private Thread m_QueueThread;
		private DateTime m_LastSend;
		private double m_UploadSpeed;

		private ArrayList m_UploadAverages;
		private int m_nSent;
		private int m_EstimatedRate;
		private int m_SumForAverages;
		private DateTime m_LastAcceptedClientTime;
		private int m_Partners;
		private bool m_AddingPartner;

		public float GetUploadSpeed()
		{
			return (float)Math.Round(m_UploadSpeed/1024F,1);
		}

		public int Count
		{
			get
			{
				return m_QueueList.Count;
			}
		}

		public CClient this[int index]
		{
			get
			{
				return (CClient)m_QueueList[index];
			}
		}

		public ArrayList List
		{
			get
			{
				return m_QueueList;
			}
		}

		public CQueue()
		{
			m_QueueList=ArrayList.Synchronized(new ArrayList());
			UploadList=ArrayList.Synchronized(new ArrayList());
			m_UploadAverages=new ArrayList();

			for (int i=0;i!=100;i++)
			{
				m_UploadAverages.Add(0);
			}

			m_QueueThread=new Thread(new ThreadStart(m_Process));
			m_QueueThread.Name="QueueThread";
			m_QueueThread.Start();

			m_LastSend=DateTime.Now;
			m_LastAcceptedClientTime=DateTime.MinValue;
			m_Partners=0;
			m_AddingPartner=false;
		}

		public ushort RefreshClientUDP(uint IP,ushort UDPPort, ref CClient foundClient)
		{
			foundClient=null;

			lock(m_QueueList.SyncRoot)
			{
				foreach (CClient client in m_QueueList)
				{
					if ((client.UserID==IP)&&(client.UDPPort==UDPPort))
					{
						foundClient=client;
						break;
					}
				}
			}

			if (foundClient!=null)
			{
				foundClient.IncUploadRequests();
				foundClient.LastUploadRequest=DateTime.Now;
				return m_GetQueuePosition(foundClient);
			}
			else 
			{
				foundClient=CKernel.ClientsList.GetClient(IP,UDPPort);
				if (foundClient!=null)
				{
					if (foundClient.UpFileHash!=null)
						return AddClient(foundClient);
					else 
						return 0;
				}
				else
					return 0;
			}
		}

		public ushort AddClient(CClient Client)
		{
			Client.IncUploadRequests();

			if (m_QueueList.Contains(Client))
			{
				TimeSpan diff=DateTime.Now-Client.LastUploadRequest;
				if ((diff<new TimeSpan(0,Protocol.MinClientReask-11,0))&&
					(diff>new TimeSpan(0,3,0)))
				{
					RemoveClient(Client);
					
					Debug.WriteLine("Client kicked from queue, fast reask "+Client.UserName+" "+Client.Software+" "+diff.TotalMinutes);
					CKernel.ClientsList.IsClientNeeded(Client);
					return 0;
				}
				else
				{
					Client.LastUploadRequest=DateTime.Now;
					return m_GetQueuePosition(Client);
				}
			}

			Client.LastUploadRequest=DateTime.Now;
			if (UploadList.Contains(Client))
				return 1;
			//is a new client
			int queueLength=CKernel.Preferences.GetInt("QueueLength");
			int queuedForRequestedFile=0;
			if (Client.UploadElement!=null) 
				queuedForRequestedFile=Client.UploadElement.Statistics.QueuedClients;

			if ((m_QueueList.Count>=queueLength)&&
				(!Client.IsFriend)&&
				((queuedForRequestedFile>0)||((queuedForRequestedFile==0)&&(m_QueueList.Count>=queueLength+15))))
			{
				if ((Client.UploadElement==null)||
					(Client.UploadElement.File==null)||
					(Client.UploadElement.File.UpPriority!=Constants.Priority.VeryHigh)||
					(m_QueueList.Count>=queueLength+50))

				if (Client.SoftwareID==(uint)Protocol.Client.eLePhant) 
					return (ushort)m_QueueList.Count;
				else
					return 0;
			}
			//there is room in queue or is a very high priority file and we can use additional 50 queue positions
			//or is the first client requesting a file
//#if DEBUG
//			if (queuedForRequestedFile==0)
//				CLog.Log(Constants.Log.Verbose,"Added to queue "+ m_QueueList.Count.ToString()+" "+Client.UserName +" for rare file "+Client.UploadElement.File.FileName);
//#endif
			m_QueueList.Add(Client);
			if (Client.UploadElement!=null)  Client.UploadElement.Statistics.AddUploadChunksAvaibility(Client.UpFileChunks);
			Client.QueueInTime=DateTime.Now;
			return m_GetQueuePosition(Client);
		}

		private ushort m_GetQueuePosition(CClient Client)
		{
			if (!m_QueueList.Contains(Client)) return 0;

			ushort position=1;
			double rating=GetClientRating(Client);

			foreach (CClient compareClient in m_QueueList)
			{
				if (compareClient==Client) continue;

				if (GetClientRating(compareClient)<rating)
				{
					position++;
				}
			}
			return position;
		}

		private void m_RemoveTimedOutCallBacks()
		{	
			try
			{
				ArrayList timedOutCalledBack=null;
				lock(UploadList.SyncRoot)
				{
					try
					{
						foreach(CClient Client in UploadList)
						{
							if ((Client.UploadState==Protocol.UploadState.WaitCallBack)
								&&(Client.CallBackTimedOut))
							{
								if (timedOutCalledBack==null)
								{
									timedOutCalledBack=new ArrayList();
								}
								timedOutCalledBack.Add(Client);
							}
						}
					}
					catch(Exception e)
					{
						Debug.WriteLine(e.ToString());
					}
				}
				if (timedOutCalledBack!=null) 
				{
					foreach (CClient CallBackClient in timedOutCalledBack)
					{
						CallBackClient.OnDisconnect((byte)Protocol.ConnectionReason.CannotConnect);
					}
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}
		private int m_GetNumberOfSlots()
		{
			int SlotNumber;
			float SlotSpeed;
			
			float maxSpeed=CKernel.Preferences.GetFloat("MaxUploadRate");
			
			if (maxSpeed<=9) SlotSpeed=2;
			else if (maxSpeed<=15) SlotSpeed=3;
			else if (maxSpeed<=22) SlotSpeed=4;
			else SlotSpeed=5;
		
			SlotNumber=(int)(maxSpeed/SlotSpeed)+(int)((maxSpeed-GetUploadSpeed())/SlotSpeed);
			return SlotNumber;
		}
		
		private bool m_AllowNewUpload()
		{
			//add new uploads
			if ((m_AddingPartner)||(DateTime.Now-m_LastAcceptedClientTime<new TimeSpan(0,0,10)))
				return false;
			if ((m_QueueList.Count < 1)
				||((float)UploadList.Count>=m_GetNumberOfSlots()))
			{
				return false;
			}
//			if (((m_UploadSpeed/UploadList.Count)>2000F)&&((m_UploadSpeed/UploadList.Count)<3000F)) return false;
			return true;
		}
		private void m_AcceptNewUpload()
		{
			if (!m_AllowNewUpload()) return;

			CClient client=SelectNextClient();

			if (client!=null)
			{
				client.StartUpload();
				m_LastAcceptedClientTime=DateTime.Now;
				UploadList.Add(client);
				m_QueueList.Remove(client);
				CKernel.NewUpload(client);
			}
		}

		private void m_Process()
		{
			DateTime inTime;
			int timeSleep;
			TimeSpan lostTime;
			while (m_QueueThread.IsAlive)
			{
				inTime=DateTime.Now;

				m_RemoveTimedOutCallBacks();

				CKernel.ServersList.SearchNextSourcesUDP();
				CKernel.ServersList.SearchNextSourcesTCP();
				CKernel.ServersList.NextPingUDP();

				//this is from emule ;)
				m_SumForAverages-=(int)(m_UploadAverages[0]);
				m_UploadAverages.RemoveAt(0);
				m_UploadAverages.Add(m_nSent);
				m_SumForAverages+=m_nSent;
				m_UploadSpeed=10*m_SumForAverages/m_UploadAverages.Count;
				CKernel.GlobalStatistics.IncSessionUp(m_nSent);

				m_AcceptNewUpload();
				m_nSent=0;
				if (UploadList.Count==0)
				{
					timeSleep=100;
				}
				else
				{
					int readyClients=0;

					lock(UploadList.SyncRoot)
					{
						foreach (CClient client in UploadList)
						{
							if ((client.connection!=null)&&(!client.connection.Sending)&&(client.connection.Connected))
							{
								readyClients++;
							}
						}
					}
					if (readyClients==0)
					{
						m_EstimatedRate-=200;
						if (m_EstimatedRate<100)
						{
							m_EstimatedRate=100;
						}
						readyClients++;
					}
					else
					{
						m_EstimatedRate+=200;
						if (m_EstimatedRate>(int)(CKernel.Preferences.GetFloat("MaxUploadRate")*102))
						{
							m_EstimatedRate=(int)(CKernel.Preferences.GetFloat("MaxUploadRate")*102);
						}
					}
					
					CClient Client;

					int sendByClient=m_EstimatedRate/readyClients;
					int index=0;

					while (index < UploadList.Count)
					{
						Client=(CClient)(UploadList[index]);
						m_nSent+=Client.SendBlockData(sendByClient);
						index++;
					}
					lostTime=(DateTime.Now-inTime);
					if (lostTime.TotalMilliseconds>=100)
					{
						timeSleep=1;
					}
					else
					{
						timeSleep=100-(int)lostTime.TotalMilliseconds;
					}
				}
				Thread.Sleep(timeSleep);
			}
		}

		private double GetClientRating(CClient client)
		{
			double priority=1F;
			double credit=1F;
			TimeSpan timeInQueue=DateTime.Now-client.QueueInTime;

			if ((client.UploadElement!=null)&&(client.UploadElement.File!=null))
			{
				switch(client.UploadElement.File.UpPriority)
				{
					case Constants.Priority.VeryLow:
						priority=1;
						break;
					case Constants.Priority.Low:
						priority=3;
						break;
					case Constants.Priority.Normal:
						priority=8;
						break;
					case Constants.Priority.High:
						priority=18;
						break;
					case Constants.Priority.VeryHigh:
						priority=37;
						break;
				}
			}
			//instant credit
			if ((client.DownloadState==Protocol.DownloadState.Downloading))
			{
				credit=1F+client.DownloadSpeed*20F;
			}
			// friend bonus
			if (client.IsFriend)
			{
				credit=5.0F;
			}
			// emule credits systems benefit sources to us by probabilities due to the credit system,we emulate it with a littel benefit to get similar downloads speeds
			if ((credit==1F)&&(client.DownFileHash!=null)&&(client.DownloadState!=Protocol.DownloadState.NoNeededParts))
			{
				if ((client.QRDownload<100)&&(client.QRDownload>0))
				{
					credit=3.5F;
				}
				else
				{
					credit=2.5F;
				}
			}
			return timeInQueue.TotalSeconds*priority*credit;
		}

		private CClient SelectNextClient()
		{
			CClient maxClient=null;
			ArrayList deleteList=new ArrayList();
			double maxRating=0;
			double rating=0;
			DateTime nowTime=DateTime.Now;

			lock (m_QueueList.SyncRoot)
			{
				foreach (CClient client in m_QueueList)
				{
					if (nowTime-client.LastUploadRequest>=new TimeSpan(0,Protocol.MinPurgeQueueTime,0))
					{
						deleteList.Add(client);
					}
					else
					{
						if (client.HasFriendSlot)
						{
							maxClient=client;
							break;
						}
						rating=GetClientRating(client);
						if (rating > maxRating)
						{
							maxRating=rating;
							maxClient=client;
						}
					}
				}
			}

			foreach (CClient client in deleteList)
			{
				m_QueueList.Remove(client);
				if (client.UploadElement!=null) client.UploadElement.Statistics.RemoveUploadChunksAvaibility(client.UpFileChunks);
				CKernel.ClientsList.IsClientNeeded(client);
			}
			return maxClient;
		}

		public bool ContainsClient(CClient cliente)
		{
			return ((m_QueueList.Contains(cliente))||(UploadList.Contains(cliente)));
		}

		public void Clean(bool stop)
		{
			if (stop) m_QueueThread.Abort();
			m_QueueList.Clear();
			UploadList.Clear();
		}

		public void RemoveClient(CClient client)
		{
			if (client.UploadElement!=null) client.UploadElement.Statistics.RemoveUploadChunksAvaibility(client.UpFileChunks);
			m_QueueList.Remove(client);
		}

		public void RemoveUpload(CClient Client, bool enqueue)
		{
			if (!UploadList.Contains(Client)) return;
			if ((enqueue)&&((float)CKernel.Queue.UploadList.Count-1>=m_GetNumberOfSlots()))
			{
				// si no hay nadie en la cola yo no lo he tirado así que se ha salido de transferencias por que 
				// ha desconectado,si no hay más gente en la cola que slots no lo metemos para prevenir un bucle
				Client.QueueInTime=DateTime.Now;
				UploadList.Remove(Client);
				AddClient(Client);
				Client.UploadState=Protocol.UploadState.OnQueue;
				if ((Client.IsPartner)&&(m_Partners>0)) m_Partners--;
				Debug.WriteLine("Client "+Client.UserName+" moved to queue");

			}
			else
			{
				UploadList.Remove(Client);
				if (Client.UploadElement!=null) Client.UploadElement.Statistics.RemoveUploadChunksAvaibility(Client.UpFileChunks);
				if ((Client.IsPartner)&&(m_Partners>0)) m_Partners--;
				Client.UploadState=Protocol.UploadState.None;
				Debug.WriteLine("Client "+Client.UserName+" removed from transfer");
			}
		}

		public bool AllowNewParner()
		{
			bool allowed=((m_Partners==0)||((float)m_Partners<(float)m_GetNumberOfSlots()*Protocol.MaxPartners));
//			if (!allowed) CLog.Log(Constants.Log.Verbose,"Partner requested but not allowed");
			return allowed;
		}

		public void AddPartner(CClient client)
		{
			if (!AllowNewParner()) return;

			//select a client and kick to allow slot for a partner
			m_AddingPartner=true;
			CClient kickClient;
			if ((UploadList.Count>=this.m_GetNumberOfSlots()))
			{
				for (int i=0;i<UploadList.Count;i++)
				{
					kickClient=(CClient)UploadList[i];
					if (!kickClient.IsPartner)
					{
						kickClient.CancelUploadTransfer(true);
						CLog.Log(Constants.Log.Verbose,"kicked "+kickClient.UserName+" to allow new partner");
						break;
					}
				}
			}
			
			if (client!=null)
			{
				client.IsPartner=true;
				m_Partners++;
				client.StartUpload();
				m_LastAcceptedClientTime=DateTime.Now;
				UploadList.Add(client);
				m_QueueList.Remove(client);
				CKernel.NewUpload(client);
			}
			m_AddingPartner=false;
		}
	}
}