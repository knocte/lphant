#region Copyright (c)2003 Juanjo<http://lphant.sourceforge.net >
/*
* This file is part of eLePhant
* Copyright (C)2003 Juanjo<j_u_a_n_j_o@users.sourceforge.net / http://lphant.sourceforge.net >
* 
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either
* version 2 of the License, or (at your option) any later version.
* 
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Collections;
using System.Diagnostics;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for CSourcesList.
	/// </summary>
	internal class CSourcesList
	{
		private ArrayList m_ArrayList;
		private CElement m_Element;
		private ushort[] m_Avaibility;
		private DateTime m_LastTCPSearch;
		private DateTime m_LastSourceExchangeRequest;
		private float m_DownloadSpeed;
		private float m_PrevDownloadSpeed;
		private ushort m_nUsableClients;
		private ushort m_nDownloadingClients;

		public DateTime GetLastSourceExchangeRequest() {return m_LastSourceExchangeRequest;}
		public void SetLastSourceExchangeRequest() {m_LastSourceExchangeRequest=DateTime.Now;}
		public float GetDownloadSpeed() {return (float)Math.Round(m_DownloadSpeed/1024F,1);}

		public ushort GetDownloadingClients() {return m_nDownloadingClients;}
		public ushort GetUsableClients() {return m_nUsableClients;}
		public ushort[] GetChunksAvaibility() {return m_Avaibility;}

		public CSourcesList(CElement in_Element)
		{
			m_ArrayList=ArrayList.Synchronized(new ArrayList());
			m_Element=in_Element;
			m_Avaibility=null;
			Random rnd=new Random();
			m_LastTCPSearch=DateTime.MinValue; //-TimeSpan.FromSeconds(rnd.Next(60));
			m_LastSourceExchangeRequest=DateTime.MinValue;
			
			m_DownloadSpeed=0;
		}

		public int Count()
		{
			return m_ArrayList.Count;
		}

		public IEnumerator GetEnumerator()
		{
			return m_ArrayList.GetEnumerator();
		}

		public virtual object SyncRoot()
		{
			return m_ArrayList.SyncRoot;
		}

		public object Item(int index)
		{
			return m_ArrayList[index];
		}

		public CClient GetSource(uint ID,ushort port,uint serverIP,byte[] ClientHash)
		{
			return CKernel.GetClient(m_ArrayList,ID,port,serverIP,ClientHash);
		}

		public bool RemoveNoNeededSource()
		{
			CClient sourceToRemove=null;
			CClient sourceToRemove2=null;
			lock(m_ArrayList.SyncRoot)
			{
				foreach (CClient source in m_ArrayList)
				{
					//try to remove no need source
					if (source.DownloadState==Protocol.DownloadState.NoNeededParts)
					{
						sourceToRemove=source;
						break;
					}
					//if not possible try to remove queue full source
					if ((source.DownloadState==Protocol.DownloadState.OnQueue)&&
						((source.connection==null)||(!source.connection.Connected))&&
						(source.QRDownload==0))
					{
						sourceToRemove2=source;
					}
				}
			}
			if (sourceToRemove!=null) 
			{
	//			Debug.WriteLine("REMOVED: " + sourceToRemove.UserName);
				RemoveSource(sourceToRemove);
				return true;
			}
			if (sourceToRemove2!=null) 
			{
	//			Debug.WriteLine("REMOVED2: " + sourceToRemove2.UserName);
				RemoveSource(sourceToRemove2);
				return true;
			}
			return false;
		}
		
		public bool AddSource(CClient source)
		{
			bool notMaxSources;
			notMaxSources=(((m_Element.File!=null)&&(m_ArrayList.Count<m_Element.File.MaxSources))||
				(RemoveNoNeededSource()));
			if ((notMaxSources)
				&&(!m_ArrayList.Contains(source))
				&&((source.UserID!=CKernel.Preferences.GetUInt("ID"))
				||(source.Port!=CKernel.Preferences.GetUShort("TCPPort"))))
			{
				//this is an additional check to avoid duplicated sources
				//is it really needed, does it solve the problem at 100% ?
				CClient existingSource=CKernel.GetClient(m_ArrayList,source.UserID,source.Port,source.ServerIP,source.UserHash);
				if (existingSource!=null)
				{
#if DEBUG					
					//CLog.Log(Types.Constants.Log.Verbose,"Duplicated source detected: "+ source.UserName + "-" + source.UserID.ToString() + "-" + m_ArrayList.Contains(source).ToString());
#endif
					//attach the client to the existing one
					source=existingSource;
					return false;
				}
				//

				m_ArrayList.Add(source);

				CKernel.NewSource(source, CKernel.HashToString(source.DownFileHash));
				return true;
			}
			else CKernel.ClientsList.IsClientNeeded(source);
			return false;
		}

		public void UpdateChunksAvaibility()
		{
			byte[] DownFileChunks;
			if (m_Avaibility==null) m_Avaibility=new ushort[CHash.GetChunksCount(m_Element.File.FileSize)];
			ushort[] avaibilitytmp=new ushort[m_Avaibility.Length];
			if (m_Element==null) return;
			int nChunks=m_Element.File.NumChunks;
			for (int i=0;i!=avaibilitytmp.Length;i++) avaibilitytmp[i]=0;
			lock(m_ArrayList.SyncRoot)
			{
				foreach (CClient source in m_ArrayList)
				{
					if (m_Element==null) return;
					DownFileChunks=source.DownFileChunks;
					if (DownFileChunks!=null)
						for (int i=0; i!=nChunks;i++)
						{
							if ((Protocol.ChunkState)DownFileChunks[i]==Protocol.ChunkState.Complete)
								avaibilitytmp[i]++;
						}
				}
			}
			m_Avaibility=avaibilitytmp;
		}
		public void AddChunksAvaibility(byte[] chunkStatus)
		{
			if (m_Avaibility==null) m_Avaibility=new ushort[CHash.GetChunksCount(m_Element.File.FileSize)];
			if ((chunkStatus==null)||(chunkStatus.Length!=m_Avaibility.Length)) return;
			for (int i=0; i!=chunkStatus.Length;i++)
			{
				if ((Protocol.ChunkState)chunkStatus[i]==Protocol.ChunkState.Complete)
					m_Avaibility[i]++;
			}			
		}
		public void RemoveChunksAvaibility(byte[] chunkStatus)
		{
			if (m_Element==null) return; //possible when stopping the file
			if (m_Avaibility==null) m_Avaibility=new ushort[CHash.GetChunksCount(m_Element.File.FileSize)];
			if ((chunkStatus==null)||(chunkStatus.Length!=m_Avaibility.Length)) return;
			for (int i=0; i!=chunkStatus.Length;i++)
			{
				if (((Protocol.ChunkState)chunkStatus[i]==Protocol.ChunkState.Complete)
					&&(m_Avaibility[i]>0))
					m_Avaibility[i]--;
			}	
		}
		public int SelectChunk(byte[] clientPartStatus,int lastRequestedPart)
		{
			ArrayList ActiveRareChunks=new ArrayList();	//prioridad 1
			ArrayList ParcialRareChunks=new ArrayList();	//prioridad 2
			ArrayList RareChunks=new ArrayList();			//prioridad 3
			ArrayList ParcialChunks=new ArrayList();			//prioridad 4
			ArrayList EmptyChunks=new ArrayList();			//prioridad 5
			ArrayList ActiveChunks=new ArrayList();		//prioridad 6
			uint minRareChunks=10;
			uint maxRareChunks=1;
			if (m_Element==null) return -1;
			if ((lastRequestedPart>-1)&&(m_Element.File.RemainingToAskInChunk(lastRequestedPart)))
				return lastRequestedPart;
			if (m_Avaibility.Length==1) //si el fichero solo tiene una parte
				return 0;
			byte[] chunkStatus=m_Element.File.ChunksStatus;

			//seleccionar primero la primera y última partes para poder hacer preview
			if ((((clientPartStatus==null))||((Protocol.ChunkState)clientPartStatus[0]==Protocol.ChunkState.Complete))&&
				(m_Element.File.IsEmptyChunk(0)))
			{
				CLog.Log(Types.Constants.Log.Verbose,"selected 1st chunk");
				return 0;
			}
			if ((((clientPartStatus==null))||((Protocol.ChunkState)clientPartStatus[m_Element.File.NumChunks-1]==Protocol.ChunkState.Complete))&&
				(m_Element.File.IsEmptyChunk(m_Element.File.NumChunks-1)))
			{
				CLog.Log(Types.Constants.Log.Verbose, "selected last chunk");
				return m_Element.File.NumChunks-1;
			}

			//calcular la ventana de partes Raras
			for (int i=0;i!=m_Avaibility.Length;i++)
			{
				if (((clientPartStatus==null)||((Protocol.ChunkState)clientPartStatus[i]==Protocol.ChunkState.Complete))&&(m_Element.File.RemainingToAskInChunk(i)))
				{
					if (m_Avaibility[i]<minRareChunks)
						minRareChunks=m_Avaibility[i];
					if (m_Avaibility[i]>maxRareChunks)
						maxRareChunks=m_Avaibility[i];
				}
			}
			//Clasificar las partes por tipos
			for (int i=0;i!=m_Avaibility.Length;i++)
			{
				if (((Protocol.ChunkState)chunkStatus[i]==Protocol.ChunkState.InProgress)||((Protocol.ChunkState)chunkStatus[i]==Protocol.ChunkState.Empty) )
				{
					if ((clientPartStatus==null)||((Protocol.ChunkState)clientPartStatus[i]==Protocol.ChunkState.Complete))
					{
						//partes raras
						if (m_Avaibility[i]==minRareChunks&&minRareChunks<maxRareChunks) 
						{
							if (m_Element.File.RequestingBlocksInChunk(i))
								ActiveRareChunks.Add(i);
							else if (!m_Element.File.IsEmptyChunk(i))
								ParcialRareChunks.Add(i);
							else
								RareChunks.Add(i);
						}
						else if (m_Element.File.RequestingBlocksInChunk(i))
							ActiveChunks.Add(i);
						else if (m_Element.File.IsEmptyChunk(i))
							EmptyChunks.Add(i);
						else
							ParcialChunks.Add(i);
					}
				}
			}
			//Seleccionar la mejor parte posible
			int num_chunk;
			Random rnd=new Random();
			while(true)
			{
				if (ActiveRareChunks.Count>0)
				{
					num_chunk=(int)ActiveRareChunks[rnd.Next(ActiveRareChunks.Count-1)];
					ActiveRareChunks.Remove(num_chunk);
				}
				else if (RareChunks.Count>0)
				{
					num_chunk=(int)RareChunks[rnd.Next(RareChunks.Count-1)];
					RareChunks.Remove(num_chunk);
				}
				else if (ParcialChunks.Count>0)
				{
					num_chunk=(int)ParcialChunks[rnd.Next(ParcialChunks.Count-1)];
					ParcialChunks.Remove(num_chunk);
				}
				else if (EmptyChunks.Count>0)
				{
					num_chunk=(int)EmptyChunks[rnd.Next(EmptyChunks.Count-1)];
					EmptyChunks.Remove(num_chunk);
				}
				else if (ActiveChunks.Count>0)
				{
					num_chunk=(int)ActiveChunks[rnd.Next(ActiveChunks.Count-1)];
					ActiveChunks.Remove(num_chunk);
				}
				else
				{
					num_chunk=-1;
					break;
				}
				if (m_Element.File.RemainingToAskInChunk(num_chunk))
					break;
			}
			return num_chunk;
		}

		public void RemoveSource(CClient Client)
		{
			if (m_ArrayList.Contains(Client))
			{
				RemoveChunksAvaibility(Client.DownFileChunks);
				m_ArrayList.Remove(Client);
				Client.RemoveDownload();
				Client.SwapToOtherFile();
			}
			CKernel.ClientsList.IsClientNeeded(Client);
		}

		public bool Contains(CClient client)
		{
			return m_ArrayList.Contains(client);
		}

		public void Pause()
		{
			//pause currently not supported
		}

		public void Clean()
		{
			m_Element=null;
			while (m_ArrayList.Count>0)
			{
				CClient Client=(CClient)m_ArrayList[0];
				RemoveSource(Client);
			}
		}

		public void Process()
		{
			//Search more sources in the connected server
//			if (((DateTime.Now-m_LastTCPSearch)>new TimeSpan(0,Protocol.ReaskClient,0))&&
//				(CKernel.Preferences.GetInt("MaxSourcesPerFile")>m_ArrayList.Count-5))
//			{
//				if ((CKernel.ServersList.ActiveServer!=null)&&(m_Element!=null)&&(m_Element.File!=null))
//				{
//					CKernel.ServersList.ActiveServer.RequestSources(m_Element.File.FileHash);
//					m_LastTCPSearch=DateTime.Now;
//					Debug.Write("Asking for file "+ m_Element.File.GetFileName() +"\n");
//				}
//			}
			float aux_VelocidadDescarga=0;
			ushort aux_nDownloadingClients=0;
			ushort aux_nUsableClients=0;
			float factor;
			ArrayList timedOutCalledBack=null;
	
			lock(m_ArrayList.SyncRoot)
			{
				try
				{
					foreach (CClient Client in m_ArrayList)
					{
						factor=1.0F;
						if (Client.DownloadState==Protocol.DownloadState.NoNeededParts)
							factor=2.0F;
						else if ((Client.SoftwareID==(uint)Protocol.Client.mlDonkey)&&(Client.DownloadTries>0))
							factor=0.55F;
						else if ((Client.QRDownload==0)&&(Client.DownloadTries>0))
							factor=0.75F;
						
						if ((DateTime.Now-Client.LastDownloadRequest>new TimeSpan(0, 0, (int)((float)Protocol.MinClientReask * factor)-1, 0))
							&&(Client.DownloadState!=Protocol.DownloadState.Connecting)
							&&(Client.DownloadState!=Protocol.DownloadState.Downloading)
							&&(Client.DownloadState!=Protocol.DownloadState.Connected)
							&&(Client.DownloadState!=Protocol.DownloadState.WaitCallBack) )
						{
							if (!Client.AskingUDP)
							{
								Client.TryUDPDownload();
							}
							else
							{
								if (DateTime.Now-Client.LastDownloadRequest>new TimeSpan(0, 0, (int)((float)Protocol.MinClientReask * factor), 0))
								{
									Client.TryDownload();
								}
							}
						}
											
						if ((Client.DownloadState==Protocol.DownloadState.Downloading))
						{
							aux_nDownloadingClients++;
							Client.UpdateDownloadSpeed();
							aux_VelocidadDescarga+=Client.DownloadSpeed;
						}

						if ((Client.DownloadState==Protocol.DownloadState.WaitCallBack)
							//||(Client.UploadState==Protocol.UploadState.WaitCallBack))
							&&(Client.CallBackTimedOut))
						{
							if (timedOutCalledBack==null)
							{
								timedOutCalledBack=new ArrayList();
							}
							timedOutCalledBack.Add(Client);
						}

						if ((Client.DownloadState!=Protocol.DownloadState.None)
							&&(Client.DownloadState!=Protocol.DownloadState.NoNeededParts))
						{
							aux_nUsableClients++;
						}
					}
				}
				catch(InvalidOperationException e)
				{
					//in some vary rare case the enumerating the sources collection can fail
					//possible errors a can not create a socket or similar
					//we catch this execption and continue since we will process sources again in 1 second
					//if entering here is not rare then we must delete the sources with this error outside the loop
#if DEBUG
					CLog.Log(Types.Constants.Log.Verbose,"Error enumerating sources collection in SourcesList.Process "+e.ToString());
#endif
					Debug.WriteLine(e.ToString());
				}
			}

			// Disconnect may modify the sources list collection, so we disconnect outside the loop
			if (timedOutCalledBack!=null) 
			{
				foreach (CClient CallBackClient in timedOutCalledBack)
				{
					CallBackClient.OnDisconnect((byte)Protocol.ConnectionReason.CannotConnect);
				}
			}

			m_PrevDownloadSpeed=m_DownloadSpeed;
			m_DownloadSpeed=aux_VelocidadDescarga;
			m_nDownloadingClients=aux_nDownloadingClients;
			m_nUsableClients=aux_nUsableClients;
		}
	}
}