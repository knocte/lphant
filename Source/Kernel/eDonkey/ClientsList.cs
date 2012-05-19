#region Copyright (c)2003 Juanjo<http://lphant.sourceforge.net >
/*
* This file is part of eLePhant
* Copyright (C)2003 Juanjo<j_u_a_n_j_o@users.sourceforge.net / http://lphant.sourceforge.net >
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
using System.Diagnostics;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for CClientsList.
	/// </summary>
	internal class CClientsList
	{
		#region Instance fields
		private DateTime m_LastConnection;
		private ArrayList m_List;
		#endregion

		#region Constructors & Initialization
		public CClientsList()
		{
			m_List=ArrayList.Synchronized(new ArrayList());
			m_LastConnection=DateTime.MinValue;
		}
		#endregion

		#region Public Methods
		public CClient GetClient(uint ID,ushort port,uint serverIP,byte[] ClientHash)
		{
			return CKernel.GetClient(m_List,ID,port,serverIP,ClientHash);
		}
		public CClient GetClient(uint IP, ushort UDPPort)
		{
			lock(m_List.SyncRoot)
			{
				foreach (CClient client in m_List)
				{
					if ((client.UserID==IP)&&(client.UDPPort==UDPPort))
						return client;
				}
			}
			return null;
		}
/*		public CClient GetClient(uint ID,ushort port,uint serverIP,byte[] ClientHash)
		{
			CClient resClient=null;
			byte[] HashComp;
			bool isSame;

			lock (m_List.SyncRoot)
			{
				foreach (CClient Client in m_List)
				{
					HashComp=Client.UserHash;

					if ((ClientHash!=null)&&(HashComp!=null))
					{
						isSame=true;

						for (int i=0;i!=16;i++)
						{
							if (HashComp[i]!=ClientHash[i]) 
							{
								isSame=false;
								break;
							}
						}

						if (isSame)
						{
							resClient=Client;
							break;
						}
						else
						{
							continue;
						}
					}

					if (ID>Protocol.LowIDLimit)
					{
						if (port>0)
						{
							if ((Client.UserID==ID)&&(Client.Port==port))
							{
								resClient=Client;
								break;
							}
						}
						else
						{
							if (Client.UserID==ID)
							{
								resClient=Client;
								break;
							}
						}
					}
					else
					{
						if ((Client.UserID==ID)&&(Client.Port==port)&&(Client.ServerIP==serverIP))
						{
							resClient=Client;
							break;
						}
					}
				}
			}
			return resClient;
		}
*/
		public bool AddClient(CClient Client)
		{
			CClient ClientBuscado=GetClient(Client.UserID,Client.Port,Client.ServerIP,Client.UserHash);

			if (ClientBuscado==null)
			{
				m_List.Add(Client);
				return true;
			}
			else
			{
				return false;
			}
		}

		public CClient AddClient(uint IP,ushort port,uint ServerIP,byte[] HashClient,ushort ServerPort,byte[] FileHashDescarga)
		{
			CClient client=AddClient(IP,port,ServerIP,HashClient,ServerPort,FileHashDescarga,Constants.FriendLevel.NoFriend);
			return client;
		}

		public CClient AddClient(uint IP,ushort port,uint ServerIP,byte[] HashClient,ushort ServerPort,byte[] FileHashDescarga, Constants.FriendLevel friendLevel)
		{
			CClient client=GetClient(IP,port,ServerIP,HashClient);

			if (client==null)
			{
				client=new CClient(port,IP,ServerIP,ServerPort,FileHashDescarga);
				client.FriendStatus=friendLevel;

				if (m_LastConnection<DateTime.Now-new TimeSpan(0,Protocol.MinClientReask,0))
				{
					m_LastConnection=DateTime.Now-new TimeSpan(0,0,Protocol.MinClientReask-1,59,700);
					client.LastDownloadRequest=DateTime.Now-new TimeSpan(0,Protocol.MinClientReask,0);
				}
				else
				{
					m_LastConnection += new TimeSpan(0,0,0,0,300);
					client.LastDownloadRequest=m_LastConnection;
				}
				m_List.Add(client);
			}
			else 
			{
				if (!client.IsFriend) client.FriendStatus=friendLevel;
				else 
				{
					CKernel.FriendsList.Add(IP,port,ServerIP,HashClient,ServerPort,"",client.UserName,client.Software,client.Version);
				}
			};

			return client;
		}

		public void AddClientToFile(CClient client,byte[] FileHash)
		{
			if (client.DownFileHash==null) 
			{
				client.DownFileHash=FileHash;
				CKernel.FilesList.AddSource(FileHash,ref client);
			}
			else
			{
				if (CKernel.SameHash(ref FileHash,ref client.DownFileHash))
				{
					CKernel.FilesList.AddSource(FileHash,ref client);
				}
				else
				{
					client.AddOtherDownloadFile(CKernel.HashToString(FileHash));
				}
			}
		}

		public CClient AddClientToFile(uint IP,ushort port,uint serverIP,ushort serverPort,byte[] FileHash)
		{
			//filter client if needed
			if ((CKernel.IPFilter!=null)&&(CKernel.IPFilter.BlockIP(IP)))
			{
				CLog.Log(Types.Constants.Log.Verbose,"IP_BLOCKED",(new System.Net.IPAddress(IP)).ToString(),CKernel.IPFilter.BlockedIPDescription);
				return null;
			}
			//
			
			CClient client=AddClient(IP,port,serverIP,null,serverPort,FileHash);
			AddClientToFile(client,FileHash);

			return client;
		}

		public void AddClientsToFile(stDatosFuente[] sources,byte[] FileHash)
		{
			foreach (stDatosFuente source in sources)
			{
				AddClientToFile(source.IP,source.Port,source.ServerIP,source.ServerPort,FileHash);
			}
		}

		public void IsClientNeeded(CClient client)
		{
			
			if ((client.DownFileHash==null)
				||((client.DownFileHash!=null)&&((CKernel.FilesList[client.DownFileHash]==null))
				||((client.DownFileHash!=null)&&(CKernel.FilesList[client.DownFileHash].SourcesList==null))
				||((client.DownFileHash!=null)&&(CKernel.FilesList[client.DownFileHash].SourcesList!=null)
					&&(!CKernel.FilesList[client.DownFileHash].SourcesList.Contains(client)))))
			{
				if ((!client.DownloadVariousFiles)
					&&(!CKernel.Queue.ContainsClient(client))
					&&((client.connection==null)||(!client.connection.Connected)))
				{
#if VERBOSE
					CKernel.LogClient.AddLog("Client not needed",client.UserID);
#endif
					if (!client.IsFriend) 
					{
						//Debug.WriteLine(m_List.Count.ToString()+" Client not needed");
						m_List.Remove(client);
					}
				}
			}
		}

		public void SwapClientsToHigherPriorityFile()
		{
			lock (m_List.SyncRoot)
			{
				CClient Client;
				int i=0;
				while (i<m_List.Count)
				{
					try
					{
						Client=(CClient)m_List[i];
						Client.SwapToHigherPriorityFile();
					}
					catch{}
					i++;
				}
			}
		}

		#endregion
	}
}