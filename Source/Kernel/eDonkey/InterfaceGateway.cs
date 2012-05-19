#region Copyright (c)2003 Juanjo < http://lphant.sourceforge.net >
/*
* This file is part of eLePhant
* Copyright (C)2003 Juanjo < j_u_a_n_j_o@users.sourceforge.net / http://lphant.sourceforge.net >
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
using System.Net;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
//using System.Runtime.Remoting.Lifetime;

using eLePhant.Types;
using eLePhant.Classes;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for edonkeyInterfazPasarela.
	/// </summary>
	// Define the class that contains the information for a Submission event.
	public delegate void FileEvent(InterfaceFile file);
	public delegate void IPEvent(uint ip, ushort port);
	public delegate void SourceEvent(InterfaceClient source,string strFileHashDescarga);
	public delegate void ClientEvent(InterfaceClient client);
	public delegate void SearchEvent(InterfaceSearchedFile file, int SearchIndex);
	public delegate void EndSearchEvent(int SearchIndex);
	public delegate void LogEvent(Constants.Log importance,string strMsg);
	public delegate void ServerEvent(InterfaceServer server);
	public delegate void SearchTabEvent(int searchID,string tabName);
	public delegate void SearchProgressEvent(int progress,int searchID);
	
	#region Interface classes
	[Serializable]
	public class InterfaceFile
	{
		public string Name;
		public string CompleteName;
		public string strHash;
		public ushort nSources;
		public ushort nValidSources;
		public ushort nTransferringSources;
		public float DownSpeed;
		public Constants.Priority DownloadPriority;
		public Constants.Priority UploadPriority;
		public uint Size;
		public uint BytesDownloaded;
		public byte Status;
		public uint RemainingBytes;
		public byte[] ChunksStatus;
		public ushort[] ChunksAvaibility;
		public ushort[] UploadChunksAvaibility;
		public SortedList Gaps; 
		public SortedList RequestingBlocks;
		public decimal PercentCompleted;
		public uint CategoryID;
		public string Category;
		public uint MaxSources;
		//public sFileInfo FileInfo;
	}

	[Serializable]
	public class InterfaceClient
	{
		public string Name;
		public string DownFileName;
		public string UpFileName;
		public uint ID;
		public ushort Port;
		public uint ServerIP;
		public string Software;
		public uint Version;
		public uint UploadRequests;
		public uint UploadedBytes;
		public uint DownloadedBytes;
		public byte DownloadState;
		public byte UploadState;
		public DateTime LastUploadRequest;
		public DateTime QueueTime;		
		public float DownloadSpeed;
		public float UploadSpeed;
		public uint DownQR;
		public byte[] DownFileChunks;
		public byte[] UpFileChunks;
		public byte[] UserHash;
	}
	[Serializable]
	public class InterfaceServer
	{
		public string Name;
		public string IP;
		public ushort Port;
		public DateTime LastConnection;
		public byte FailedConnections;
		public uint Users;
		public uint Files;
		public Constants.ServerPriority Priority;
	}
	[Serializable]
	public class InterfaceSearchedFile
	{
		public string Name;
		public string strFileHash;
		public uint Size;
		public uint Avaibility;
		public string Codec;
		public uint BitRate;
		public string Length;
		public Types.Constants.SearchResultState ResultState;
		public bool Completed;
	}
	[Serializable]
	public class GlobalStatus
	{
		public string ServerName;
		public ushort ActiveConnections;
		public uint UserID;
		public bool IsHighID;
		public uint ServerFiles;
		public uint ServerUsers;
		public float DowloadSpeed;
		public float UploadSpeed;
		public float AvgUpSpeed;
		public float AvgDownSpeed;
		public ulong SessionUpload;
		public ulong SessionDownload;
	}
	[Serializable]
	public class InterfaceComment
	{
		public string ClientName;
		public string Comment;
		public string Rating;
	}
	[Serializable]
	public class InterfaceFileNames
	{
		public string FileName;
		public ushort nClients;
	}
	[Serializable]
	public class InterfacePreferences
	{
		public ushort TCPPort;
		public ushort UDPPort;
		public bool UseeMuleProtocol;
		public short maxServerFails;
		public bool Autoreconect;
		public float maxUploadRate;
		public float maxDownloadRate;
		public int maxConnexions;
		public int maxSources;
		public bool autoextendSearch;
		public int queueLength;
		public bool ICHEnabled;
		public bool RemoteControlEnabled;
		public bool DownloadServerMet;
		public string ServerMetUri;
		public string Language;
		public Constants.AllowViewShared AllowViewShared;
		public string UserName;
		public uint UserID;
		public bool IsHighID;
		public string[] sharedFolders;
		public string[] TempFolders;
		public int RemoteControlPort;
		public string RemoteControlPassword;
		public bool OSigEnabled;
		public bool IPFilterEnabled;
		public CompressionType CompressionMethod;
		public string[] AllowedIP;
		public bool StartNextStoppedFile;
		//public bool UpdateServerListFromClient;
		//public bool UpdateServerListFromServer;
	}

	[Serializable]
	public class FileStatistics
	{
		public uint SessionRequests;
		public ulong SessionUpload;
		public ulong SessionDownload;
		public float AvgUpSpeed;
		public float AvgDownSpeed;
		public DateTime CompletedTime;
	}
	#endregion

	
	[Serializable]
	public class CInterfaceGateway : MarshalByRefObject
	{
		protected internal bool apw;
		private ListDictionary m_LogList;

		public CInterfaceGateway()
		{
			bool maxReached=false;
			for (int i=0;i<=CKernel.InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]==null && !maxReached)
				{
					CKernel.InterfaceGateway[i]=this;
					maxReached=true;
				}
			}
			if (!maxReached)
				//no crear la clase
				return ;
		}

		~CInterfaceGateway()
		{				
			for (int i=0;i<=CKernel.InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]==this)				
					CKernel.InterfaceGateway[i]=null;				
			}
		}

		public event FileEvent OnNewFile;
		public event FileEvent OnNewSharedFile;
		public event FileEvent OnRemovedSharedFile;
		public event SourceEvent OnNewSource;
		public event SearchEvent OnNewSearched;
		public event SearchEvent OnModifiedSearched;
		public event ClientEvent OnNewUpload;
		public event LogEvent OnLogMessage;	
		public event ServerEvent OnNewServer;
		public event EndSearchEvent OnEndSearch;
		public event IPEvent OnDeleteServer;
		public event SearchTabEvent OnNewSearchTab;
		public event SearchProgressEvent OnNewSearchProgress;
		public event SourceEvent OnNewChatMessage;

		public  override Object InitializeLifetimeService()
		{
			return null;
		}

		public virtual void SubmitOnNewFile(InterfaceFile file)
		{
			if (OnNewFile!=null) OnNewFile(file);				
		}

		public virtual void SubmitOnNewSharedFile(InterfaceFile file)
		{
			if (OnNewSharedFile!=null) OnNewSharedFile(file);				
		}

		public virtual void SubmitOnRemovedSharedFile(InterfaceFile file)
		{
			if (OnRemovedSharedFile!=null) OnRemovedSharedFile(file);				
		}

		public virtual void SubmitOnNewSource(InterfaceClient source,string strFileHashDescarga)
		{
			if (OnNewSource!=null) OnNewSource(source,strFileHashDescarga);
		}

		public virtual void SubmitOnNewSearched(InterfaceSearchedFile file, int SearchIndex)
		{
			if (OnNewSearched!=null) OnNewSearched(file,SearchIndex);
		}

		public virtual void SubmitOnEndSearch(int SearchIndex)
		{
			if (OnEndSearch!=null) OnEndSearch(SearchIndex);
		}

		public virtual void SubmitOnModifiedSearched(InterfaceSearchedFile file, int SearchIndex)
		{
			if (OnModifiedSearched!=null) OnModifiedSearched(file,SearchIndex);
		}

		public virtual void SubmitOnNewUpload(InterfaceClient client)
		{
			if (OnNewUpload!=null) OnNewUpload(client);
		}

		public virtual void SubmitOnLogMessage(Constants.Log importance, string strMsg)
		{
			if (OnLogMessage!=null)	
			{
				if ((m_LogList!=null)&&(m_LogList.Count>0))
				{
					lock (m_LogList.SyncRoot)
					{
						foreach ( DictionaryEntry de in m_LogList )
						{
							OnLogMessage((Constants.Log)de.Value,(string)de.Key);
						}
						m_LogList.Clear();
					}
				}
				OnLogMessage(importance,strMsg);
			}
			else
			{
				if (m_LogList==null) m_LogList=new ListDictionary();
				if (!m_LogList.Contains(strMsg)) m_LogList.Add(strMsg,importance);
			}
		}

		internal virtual void SubmitOnNewServer(CServer server)
		{
			if (OnNewServer!=null) OnNewServer(ServerToInterfaceServer(server));
		}

		internal virtual void SubmitOnDeleteServer(uint ip, ushort port)
		{
			if (OnDeleteServer!=null) OnDeleteServer(ip,port);
		}

		internal virtual void SubmitOnNewSearchTab(int searchID,string tabName)
		{
			if (OnNewSearchTab!=null) OnNewSearchTab(searchID,tabName);
		}

		internal virtual void SubmitOnNewSearchProgress(int progressPerCent,int searchID)
		{
			if (OnNewSearchProgress!=null) OnNewSearchProgress(progressPerCent,searchID);
		}

		internal virtual void SubmitOnNewChatMessage(CClient client, string message)
		{
			if (OnNewChatMessage!=null) OnNewChatMessage(ClientToInterfaceClient(client),message);
		}

		private int ByteCompare(byte [] b1, byte [] b2)
		{
			//devuelve 0 si son iguales
			//1 si es mayor b1
			//-1 si es menor b1
			for (int i=0; i<b1.Length;i++)
			{
				if (b1[i]<b2[i]) return -1;
				if (b1[i]>b2[i]) return 1;
			}
			return 0;
		}

		public bool CheckPw(string cpw)
		{
			System.Security.Cryptography.MD5 crypto=System.Security.Cryptography.MD5.Create();
			byte [] c=null;
			string clearPass=(string)cpw;
			c=new byte[clearPass.Length];
			for (int i=0; i < clearPass.Length; i++)
			{
				c[i]=System.Convert.ToByte(clearPass[i]);
			}
			byte[] res=crypto.ComputeHash(c);
			cpw=CKernel.HashToString(res);
			if (cpw==CKernel.Preferences.GetString("RemotePassword"))
			{
				this.apw=true;
				return true;
			}
			else 
				return false;
		}

		internal void NewFile(InterfaceFile file)
		{			
			if (apw)
				if (file!=null) SubmitOnNewFile(file);
		}

		internal void NewSharedFile(InterfaceFile file)
		{			
			if (apw)
				if (file!=null) SubmitOnNewSharedFile(file);
		}

		internal void RemovedSharedFile(InterfaceFile file)
		{			
			if (apw)
				if (file!=null) SubmitOnRemovedSharedFile(file);
		}

		public void NewLogMessage(Constants.Log importance, string strMsg)
		{
			if (apw)
				SubmitOnLogMessage(importance, strMsg);						
		}

		internal void NewSource(InterfaceClient client,string strDownFileHash)
		{
			if (apw)
				if (client!=null) SubmitOnNewSource(client,strDownFileHash);
		}

		internal void NewUpload(InterfaceClient client)
		{
			if (apw)
				if (client!=null) SubmitOnNewUpload(client);
		}

		internal void NewFileFound(InterfaceSearchedFile filefound, int searchID)
		{
			if (apw)
				if (filefound!=null) SubmitOnNewSearched(filefound,searchID);
		}

		internal void SearchEnded(int searchID)
		{
			if (apw)
				SubmitOnEndSearch(searchID);
		}

		internal void SearchFileModified(InterfaceSearchedFile filefound, int searchID)
		{
			if (apw)
				if (filefound!=null) SubmitOnModifiedSearched(filefound,searchID);
		}

		public InterfaceFile GetDownloadFile(string Hash)
		{
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(Hash)];
				return FileToInterfaceFile(Element);
			} 
			else return null;
		}

		private InterfaceComment CommentToInterfaceComment(CedonkeyComment Comment)
		{
			InterfaceComment ifaceComment=new InterfaceComment();
			ifaceComment.ClientName=Comment.userName;
			ifaceComment.Comment=Comment.strComment;
			ifaceComment.Rating=Comment.rating.ToString();
			return ifaceComment;
		}

		public InterfaceComment[] GetComments(string strFileHash)
		{
			CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
			if (Element==null) return null;
			
			InterfaceComment mycomment=null;
			int nComments=0;

			if ((Element.File!=null)&&(Element.File.Comment!=null)&&(Element.File.Comment.Length>0))
			{
				mycomment=new InterfaceComment();
				mycomment.ClientName=CKernel.Preferences.GetString("UserName");
				mycomment.Rating="";
				mycomment.Comment=Element.File.Comment;
				nComments=1;
			}

			if ((Element.Comments!=null)&&(Element.Comments.Count>0))
				nComments+=(int)Element.Comments.Count;
			
			if (nComments==0) return null;

			InterfaceComment[] listaComments=new InterfaceComment[nComments];
			
			int i=0;
			if (mycomment!=null) 
			{
				listaComments[0]=mycomment;
				i++;
			}
			
			if ((Element.Comments!=null)&&(Element.Comments.Count>0))
			{
				foreach (CedonkeyComment Comment in Element.Comments)
				{
					listaComments[i]=CommentToInterfaceComment(Comment);
					i++;
				}
			}
			return listaComments;
		}

		internal InterfaceFile FileToInterfaceFile(CElement Element)
		{
			if (apw)
			{
				if (Element==null) return null;
				InterfaceFile response=new InterfaceFile();
				response.Name=Element.File.FileName;
				response.CompleteName=Element.File.CompleteName;
				response.Size=Element.File.FileSize;
				response.BytesDownloaded=Element.File.Transferred;
				response.RemainingBytes=Element.File.GetRemainingBytes();
				response.Status=(byte)Element.File.FileStatus;
				response.ChunksStatus=Element.File.ChunksStatus;
				response.UploadPriority=Element.File.UpPriority;
				response.DownloadPriority=Element.File.DownPriority;
				response.Gaps=Element.File.Gaps; 
				response.RequestingBlocks=Element.File.RequestingBlocks;
				response.UploadChunksAvaibility=Element.Statistics.UploadAvaibility;
				if (response.ChunksStatus.Length==0) 
				{
					response.ChunksStatus=new byte[CHash.GetChunksCount(Element.File.FileSize)];
					for (int i=0; i < response.ChunksStatus.Length; i++)
					{
						response.ChunksStatus[i]=(byte)Protocol.ChunkState.Empty;
					}
				}
				response.MaxSources=Element.File.MaxSourcesInt;
				if (Element.SourcesList!=null)
				{
					response.nSources=(ushort)Element.SourcesList.Count();
					response.DownSpeed=Element.SourcesList.GetDownloadSpeed();
					response.ChunksAvaibility=Element.SourcesList.GetChunksAvaibility();
					response.nValidSources=Element.SourcesList.GetUsableClients();
					response.nTransferringSources=Element.SourcesList.GetDownloadingClients();
				}
				else
				{
					response.DownSpeed=0;
					response.nSources=0;
				}
				response.strHash=CKernel.HashToString(Element.File.FileHash);
				
				if(response.Size!=0)
					response.PercentCompleted=(decimal)(response.Size-response.RemainingBytes)/response.Size;
				else
					response.PercentCompleted=0;
				
				response.CategoryID=Element.File.CategoryID;
				if (Element.File.FileStatus!=Protocol.FileState.Complete)
					response.Category=CKernel.CategoriesList[Element.File.CategoryID];
				else
					response.Category="";

				return response;
			}
			else return null;
		}

		public InterfaceFile[] GetDownloadFileList()
		{
			if (apw)
			{
				InterfaceFile[] response=new InterfaceFile[CKernel.FilesList.Count];
				int i=0;
				foreach (CElement Element in CKernel.FilesList.Values)
				{
					if (!Element.File.Completed)
					{
						response[i]=new InterfaceFile();
						response[i]=FileToInterfaceFile(Element);
						i++;
					}
				}
				return response;
			} 
			else return null;
		}

		public SFriend[] GetFriends()
		{
			if (apw)
			{
				SFriend[] response= new SFriend[CKernel.FriendsList.List.Count];
				for (int i=0;i!=CKernel.FriendsList.List.Count;i++)
				{
					//response[i] = new SFriend();
					response[i] = (SFriend)CKernel.FriendsList.List[i];
				}
				return response;
			}
			else return null;
		}

		public sFileDetails GetFileDetails(string strFileHash)
		{
			sFileDetails response=new sFileDetails();			
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if (Element==null) return response;
				response=(sFileDetails)Element.File.Details;
				return response;			
			} 
			else return response;
		}

		public FileStatistics GetFileStatistics(string strFileHash)
		{
			FileStatistics response=new FileStatistics();
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if (Element==null) return response;
				response.SessionRequests=Element.Statistics.SessionRequests;
				response.SessionDownload=Element.Statistics.SessionDownload;
				response.SessionUpload=Element.Statistics.SessionUpload;
				response.AvgDownSpeed=Element.Statistics.GetAvgDown();
				response.AvgUpSpeed=Element.Statistics.GetAvgUp();
				response.CompletedTime=Element.Statistics.TimeCompleted;
			}
			return response;
		}

//		public sFileInfo GetFileInfo(string strFileHash)
//		{
//			sFileInfo response=new sFileInfo();			
//			if (apw)
//			{
//				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
//				if (Element==null) return response;
//				response=Element.File.GetInfo();
//				return response;			
//			} 
//			else return response;
//		}

		public InterfaceFile[] GetSharedFileList()
		{
			if (apw)
			{
				InterfaceFile[] response=new InterfaceFile[CKernel.FilesList.Count];
				int i=0;
				foreach (CElement Element in CKernel.FilesList.Values)
				{
					if (!Element.File.Empty)
					{
						response[i]=new InterfaceFile();
						response[i]=FileToInterfaceFile(Element);
						i++;
					}
				}
				return response;
			} 
			else return null;
		}

		public InterfaceClient[] GetFileSources(string strFileHash)
		{
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if ((Element==null)||(Element.SourcesList==null)||(Element.SourcesList.Count()==0)) return null;
				InterfaceClient[] response=new InterfaceClient[Element.SourcesList.Count()];
				int i=0;
				lock(Element.SourcesList.SyncRoot())
				{
					foreach (CClient Client in Element.SourcesList)
					{
						response[i]=ClientToInterfaceClient(Client);
						i++;
					}
				}
				return response;
			
			} 
			else return null;
		}

		public InterfaceClient[] GetQueuedSourcesList(byte[] fileHashFilter)
		{
			if (apw)
			{
				ArrayList sourceslist=new ArrayList();
				lock (CKernel.Queue.List.SyncRoot)
				{
					foreach (CClient Client in CKernel.Queue.List)
					{
						if ((fileHashFilter==null)||(CKernel.SameHash(ref fileHashFilter, ref Client.UploadElement.File.FileHash)))
						{
							sourceslist.Add(ClientToInterfaceClient(Client));
						}
					}
				}
				InterfaceClient[] response=new InterfaceClient[sourceslist.Count];
				for (int i=0;i<sourceslist.Count;i++)
				{
					response[i]=(InterfaceClient)sourceslist[i];
				}
				return response;
			} 
			else return null;
		}

		public InterfaceClient[] GetUploadingSources()
		{
			if (apw)
			{
				InterfaceClient[] response=new InterfaceClient[CKernel.Queue.UploadList.Count];
				int i=0;
				foreach (CClient Client in CKernel.Queue.UploadList)
				{
					response[i]=ClientToInterfaceClient(Client);
					i++;
				}
				return response;
			} 
			else return null;
		}

		public InterfaceFileNames[] GetFileNamesList(string strFileHash)
		{
			if (!apw) return null;
			Hashtable listaNombres=new Hashtable();
			CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
			if ((Element==null)||(Element.SourcesList==null)||(Element.SourcesList.Count()==0)) return null;
			ushort i=0;
			lock(Element.SourcesList.SyncRoot())
			{
				foreach (CClient Client in Element.SourcesList)
				{
					if (Client.ClientFileName.Length > 0)
						if (listaNombres.ContainsKey(Client.ClientFileName))
						{
							i=(ushort)(listaNombres[Client.ClientFileName]);
							listaNombres[Client.ClientFileName]=++i;
						}
						else
							listaNombres[Client.ClientFileName]=(ushort)1;
				}
			}
			InterfaceFileNames[] response=new InterfaceFileNames[listaNombres.Count];
			i=0;
			foreach (string nombre in listaNombres.Keys)
			{
				response[i]=new InterfaceFileNames();
				response[i].FileName=nombre;
				response[i].nClients=(ushort)(listaNombres[nombre]);
				i++;
			}
			return response;
		}

		internal InterfaceServer ServerToInterfaceServer(CServer server)
		{
			InterfaceServer response=new InterfaceServer();
			response.Name=server.Name;
			response.IP=new IPAddress(server.IP).ToString();
			response.Files=server.Files;
			response.Port=server.Port;
			response.LastConnection=new DateTime(server.LastConnection);
			response.Users=server.Users;
			response.FailedConnections=(byte)server.Fails;
			response.Priority=server.Priority;
			return response;
		}

		public InterfaceServer[] GetServerList()
		{
			if (apw)
			{
				InterfaceServer[] response=new InterfaceServer[CKernel.ServersList.Count];
				int i=0;
				foreach (CServer server in CKernel.ServersList)
				{
					response[i]=ServerToInterfaceServer(server);
					i++;
				}
				return response;
			} 
			else return null;
		}

		internal InterfaceClient ClientToInterfaceClient(CClient Client)
		{
			if (apw)
			{
				InterfaceClient response=new InterfaceClient();
				response.Name=Client.UserName;
#if DEBUG
				if (Client.IsPartner) response.Name="(partner) "+response.Name;
#endif
				response.DownFileName=Client.ClientFileName;
				if ((Client.UploadElement!=null)&&(Client.UploadElement.File!=null))
					response.UpFileName=Client.UploadElement.File.FileName;
				else
					response.UpFileName="";
				response.ID=Client.UserID;
				response.Port=Client.Port;
				response.ServerIP=Client.ServerIP;
				response.Software=Client.Software;
				response.Version=Client.Version;
				response.DownloadState=(byte)Client.DownloadState;
				response.UploadState=(byte)Client.UploadState;
				response.UploadRequests=Client.UploadRequests;
				response.UploadedBytes=Client.UploadedBytes;
				response.DownloadedBytes=Client.DownloadedBytes;
				response.LastUploadRequest=Client.LastUploadRequest;
				response.QueueTime=Client.QueueInTime;		
				response.DownloadSpeed=(float)Math.Round((Client.DownloadSpeed/1024F),1);
				response.UploadSpeed=Client.UploadSpeed;
				response.DownQR=Client.QRDownload;
				response.DownFileChunks=Client.DownFileChunks;
				response.UpFileChunks=Client.UpFileChunks;
				response.UserHash=Client.UserHash;

				return response;
			} 
			else return null;
		}

		public InterfaceClient GetClient(uint ID, ushort Port,uint ServerIP, byte[] ClientHash)
		{
			if (apw)
			{
				CClient Client=CKernel.ClientsList.GetClient(ID,Port,ServerIP,ClientHash);
				if (Client!=null)
				{
					InterfaceClient response=new InterfaceClient();
					response=ClientToInterfaceClient(Client);
					return response;
				} 
				else return null;
			} 
			else return null;
		}

		public InterfaceClient GetUploadClient(uint ID, ushort Port,uint ServerIP, byte[] ClientHash)
		{
			if (apw)
			{
				CClient Client=CKernel.ClientsList.GetClient(ID,Port,ServerIP,ClientHash);
				if (Client!=null)
				{			
					if (CKernel.Queue.UploadList.Contains(Client))
					{
						InterfaceClient response=new InterfaceClient();
						response=ClientToInterfaceClient(Client);
						return response;
					}
				}
				return null;
			} 
			else return null;
		}

		public InterfaceClient GetDownloadClient(uint ID, ushort Port,uint ServerIP, byte[] ClientHash,string strDownFileHash)
		{
			if (apw)
			{
				CElement Element=CKernel.FilesList[CKernel.StringToHash(strDownFileHash)];
				if ((Element==null)||(Element.SourcesList==null)) return null;
				CClient client=Element.SourcesList.GetSource(ID,Port,ServerIP,ClientHash);
				if (client==null) return null;
				InterfaceClient response=new InterfaceClient();
				response=ClientToInterfaceClient(client);
				return response;
				
//				CClient Client=CKernel.ClientsList.GetClient(ID,Port,ServerIP,ClientHash);
//				if ((Client!=null)&&(Client.DownFileHash!=null)&&(CKernel.HashToString(Client.DownFileHash)==strDownFileHash))
//				{
//					InterfaceClient response=new InterfaceClient();
//					response=ClientToInterfaceClient(Client);
//					return response;
//				} 
//				else return null;
			} 
			else return null;
		}

		public void RequestSharedList(uint ID, ushort Port,uint ServerIP, byte[] ClientHash)
		{
			if (apw)
			{
				CClient Client=CKernel.ClientsList.GetClient(ID,Port,ServerIP,ClientHash);
				if (Client!=null)
				{
					Client.RequestSharedList();
				}
			}
		}

		public void SendChatMessage(uint ID, ushort Port,uint ServerIP, byte[] ClientHash, string message)
		{
			if (apw)
			{
				CClient Client=CKernel.ClientsList.GetClient(ID,Port,ServerIP,ClientHash);
				if (Client!=null)
				{
					Client.SendChatMessage(message);
				}
			}
		}

		public void AddFriend (uint ID, ushort Port,uint ServerIP, byte[] ClientHash, ushort ServerPort, string OurName, string Name, string Software, uint Version, bool friendSlot)
		{
			if (apw)
				CKernel.FriendsList.Add(ID, Port, ServerIP, ClientHash, ServerPort, OurName, Name, Software, Version);
		}
		
		public void ChangeFriendSlotAssigned(uint ID, byte[] ClientHash, ushort Port, bool assigned)
		{
			if (apw)
				CKernel.FriendsList.ChangeFriendSlotAssigned(ID,ClientHash,Port,assigned);
		}
		
		public void DeleteFriend (byte[] ClientHash,uint ID, ushort Port)
		{
			if (apw)
				CKernel.FriendsList.Delete(ClientHash, ID, Port);
		}

		public void DeleteFile (string strFileHash)
		{
			if (apw)
				CKernel.FilesList.DeleteFile(strFileHash);
		}

		public GlobalStatus GetGlobalStatus()
		{
			if (apw)
			{
				GlobalStatus status=new GlobalStatus();
				if (CKernel.ServersList.ActiveServer!=null)
				{
					status.ServerName=CKernel.ServersList.ActiveServer.Name;
					status.ServerFiles=CKernel.ServersList.ActiveServer.Files;
					status.ServerUsers=CKernel.ServersList.ActiveServer.Users;
				}
				else
				{
					status.ServerName=CKernel.Globalization["SRV_DISCONNECTED"];
					status.ServerFiles=0;
					status.ServerUsers=0;
				}
				if (CKernel.ServersList.ActiveServer!=null)
				{
					status.UserID=CKernel.ServersList.ActiveServer.UserID;
					status.IsHighID=CKernel.ServersList.ActiveServer.IsHighID;
				}
				else
					status.UserID=0;
				status.ActiveConnections=(ushort)CKernel.ConnectionsList.Count();
				status.DowloadSpeed=CKernel.FilesList.GlobalDownSpeed;
				status.UploadSpeed=CKernel.Queue.GetUploadSpeed(); 
				status.AvgDownSpeed=(float)Math.Round(CKernel.GlobalStatistics.GetAvgDown(),1);
				status.AvgUpSpeed=(float)Math.Round(CKernel.GlobalStatistics.GetAvgUp(),1);
				status.SessionDownload=CKernel.GlobalStatistics.GetSesionDown();
				status.SessionUpload=CKernel.GlobalStatistics.GetSessionUp();
				return status;
			} 
			else 
			{
				return null;
			}
		}

		public void ConnectToaServer()
		{
			if (apw) 
				CKernel.ServersList.ConnectToAnyServer();
		}

		public bool ConnectToServer(uint ip, ushort port)
		{
			if (apw) 
				return CKernel.ServersList.ConnectToServer(ip,port);
			return false;
		}

		public bool DeleteServer(uint ip, ushort port)
		{
			if (apw) 
				return CKernel.ServersList.Remove(ip,port);
			return false;
		}

		public void SetServerPriority(uint ip, ushort port,Constants.ServerPriority priority)
		{
			if (apw) 
				CKernel.ServersList.SetServerPriority(ip,port,priority);
		}

		public void DownloadFile(InterfaceSearchedFile file, int SearchIndex)
		{
			if (apw)
			{
				DownloadElink("ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strFileHash+"|/",false);
				//try to get soruces from the search results
				//lookup the result in the search result list
				if ((CKernel.Searchs.Count==0)||(CKernel.Searchs[SearchIndex]==null))
				{
					foreach (CSearcher searcher in CKernel.Searchs.Values)
					{
						foreach (CFileFound fileFound in ((Hashtable)searcher.Clone()).Values)
						{
							if (fileFound.Hash==file.strFileHash)
							{
								fileFound.AddSourcesToFile();
							}
						}
					}
				}
				else
				{
					CSearcher searcher=(CSearcher)CKernel.Searchs[SearchIndex];
					foreach (CFileFound fileFound in ((Hashtable)searcher.Clone()).Values)
					{
						if (fileFound.Hash==file.strFileHash)
						{
							if (fileFound.ResultState==Types.Constants.SearchResultState.New)
								fileFound.ResultState=Types.Constants.SearchResultState.AlreadyDownloading;
							fileFound.AddSourcesToFile();
							CKernel.SearchFileModified(fileFound,SearchIndex);
						}
					}
				}
			}
		}

		public void DownloadElink(string elink, bool stopped)
		{
			if (apw)
			{
				CLinks link=new CLinks(elink);
				if (link.IsFileLink)
				{
					CKernel.FilesList.AddFile(link.FileName,link.FileSize,link.FileHash,link.Clients);
					if(stopped)
					{
						CKernel.FilesList.StopFile(link.FileHash);
					}
				} 
				else if (link.IsServerLink)
				{
					CKernel.ServersList.Add(link.ServerIP,link.ServerPort);
				}
				else CLog.Log(Constants.Log.Notify,"LNK_INVALID",elink);
			}
		}

		public void DownloadServerList(string uri)
		{
			if (apw)
				CKernel.httpDownload.DownloadFile(uri,"","server.met",true);
		}

		public string Version
		{
			get { return CKernel.Version; }
		}

		public void StartKernel()
		{
			CKernel.StartKernel();
		}

		public void CloseKernel()
		{
			if (apw)
				CKernel.CloseKernel();
		}

		public void Search(int searchID, string condition, bool matchAnyWords,string type,uint maxSize, uint minSize,uint avaibility, string exclude)
		{
			if (apw)
				CKernel.NewSearch(searchID,condition, matchAnyWords,type,maxSize, minSize,avaibility,exclude);
		}

		public void ExtendSearch(int SearchIndex)
		{
			if (apw)
				CKernel.ExtendSearch(SearchIndex);
		}

		internal InterfaceSearchedFile FileFountToInterfaceFileFound(CFileFound file)
		{
			if (apw)
			{	
				InterfaceSearchedFile found= new InterfaceSearchedFile();
				found=new InterfaceSearchedFile();
				found.Name=file.Name;
				found.Size=file.Size;
				found.strFileHash=file.Hash;
				found.Avaibility=file.Avaibility;
				found.ResultState=file.ResultState;
				found.Codec=file.Codec;
				found.Length=file.Length;
				found.BitRate=file.BitRate;
				found.Completed=file.Complete;
				return found;
			} 
			else return null;
		}

		public void CancelSearch(int SearchIndex)
		{
			if (apw)
			{
				if (SearchIndex<0) SearchIndex=CKernel.Searchs.Count-1;
				if ((CKernel.Searchs.Count==0)||(CKernel.Searchs[SearchIndex]==null)) return;
				CSearcher searcher=(CSearcher)CKernel.Searchs[SearchIndex];
				if (searcher!=null) searcher.CancelSearch();
			}
		}

		public void DeleteSearch(int searchID)
		{
			if (apw)
			{
				CKernel.DeleteSearch(searchID);
			}
		}

		public InterfaceSearchedFile[] GetSearch(int SearchIndex)
		{
			if (apw)
			{
				if (SearchIndex<0) SearchIndex=CKernel.Searchs.Count-1;
				if ((CKernel.Searchs.Count==0)||(CKernel.Searchs.Count<=SearchIndex)) return null;
				CSearcher searcher=(CSearcher)CKernel.Searchs[SearchIndex];
				InterfaceSearchedFile[] filesFound=new InterfaceSearchedFile[searcher.Count];
			
				int i=0;
				lock (CKernel.Searchs[SearchIndex])
				{
					foreach (CFileFound fileFound in searcher.Values)
					{
						filesFound[i]=FileFountToInterfaceFileFound(fileFound);
						i++;
					}
				}
				return filesFound;
			} 
			else return null;
		}

		public InterfacePreferences GetConfig()
		{
			InterfacePreferences config=new InterfacePreferences();
			
			config.autoextendSearch=CKernel.Preferences.GetBool("AutoExtendSearch");
			config.Autoreconect=CKernel.Preferences.GetBool("AutoReconnect");
			config.ICHEnabled=CKernel.Preferences.GetBool("ICHEnabled");
			config.maxConnexions=CKernel.Preferences.GetInt("MaxConnections");
			config.maxDownloadRate=CKernel.Preferences.GetFloat("MaxDownloadRate");
			config.maxServerFails=CKernel.Preferences.GetShort("MaxServerFails");
			config.maxSources=CKernel.Preferences.GetInt("MaxSourcesPerFile");
			config.maxUploadRate=CKernel.Preferences.GetFloat("MaxUploadRate");
			config.queueLength=CKernel.Preferences.GetInt("QueueLength");
			config.sharedFolders=CKernel.Preferences.GetStringArray("SharedFolders");
			config.TempFolders=CKernel.Preferences.GetStringArray("TempFolders");
			config.UDPPort=CKernel.Preferences.GetUShort("UDPPort");
			config.TCPPort=CKernel.Preferences.GetUShort("TCPPort");
			config.UseeMuleProtocol=CKernel.Preferences.GetBool("UseEmuleProtocol");
			config.UserName=CKernel.Preferences.GetString("UserName");
			config.UserID=CKernel.Preferences.GetUInt("ID");
			config.RemoteControlEnabled=CKernel.Preferences.GetBool("RemoteControlEnabled");
			config.DownloadServerMet=CKernel.Preferences.GetBool("DownloadServerMet");
			config.ServerMetUri=CKernel.Preferences.GetString("ServerMetUri");
			config.Language=CKernel.Preferences.GetString("Language");
			config.AllowViewShared=(Constants.AllowViewShared)CKernel.Preferences.GetEnum("AllowViewShared", Constants.AllowViewShared.Nobody);
			config.RemoteControlPort=CKernel.Preferences.GetInt("RemoteControlPort");
			config.RemoteControlPassword="*****";//CKernel.Preferences.GetString("RemotePassword");
			config.OSigEnabled=CKernel.Preferences.GetBool("OSigEnabled"); 
			config.IPFilterEnabled=CKernel.Preferences.GetBool("IPFilterEnabled");
			config.StartNextStoppedFile=CKernel.Preferences.GetBool("StartNextStoppedFile");
			//config.UpdateServerListFromClient = CKernel.Preferences.GetBool("UpdateServerListFromClient");
			//config.UpdateServerListFromServer = CKernel.Preferences.GetBool("UpdateServerListFromServer");
			
			if(config.UserID>Protocol.LowIDLimit) 
				config.IsHighID = true;
			else
				config.IsHighID = false;
			config.AllowedIP=CKernel.Preferences.GetStringArray("AllowedIP");
			config.CompressionMethod=(CompressionType)CKernel.Preferences.GetEnum("CompressionMethod",CompressionType.Zip);
			return config;
		}

		public void SetConfig(InterfacePreferences config)
		{
			CKernel.Preferences.SetProperty("AutoExtendSearch", config.autoextendSearch);
			CKernel.Preferences.SetProperty("AutoReconnect", config.Autoreconect);
			CKernel.Preferences.SetProperty("ICHEnabled", config.ICHEnabled);
			CKernel.Preferences.SetProperty("MaxConnections", config.maxConnexions);
			CKernel.Preferences.SetProperty("MaxServerFails", config.maxServerFails);
			CKernel.Preferences.SetProperty("MaxSourcesPerFile", config.maxSources);
			CKernel.Preferences.SetProperty("MaxUploadRate", config.maxUploadRate);
			CKernel.Preferences.SetProperty("MaxDownloadRate", config.maxDownloadRate);
			//CKernel.Preferences.SetProperty("QueueLength", config.queueLength);
			CKernel.Preferences.SetProperty("SharedFolders", config.sharedFolders);
			CKernel.Preferences.SetProperty("TempFolders", config.TempFolders);
			CKernel.Preferences.SetProperty("UDPPort", config.UDPPort);
			CKernel.Preferences.SetProperty("TCPPort", config.TCPPort);
			CKernel.Preferences.SetProperty("UseEmuleProtocol", config.UseeMuleProtocol);
			CKernel.Preferences.SetProperty("UserName", (config.UserName.Length==0)?"http://www.lphant.com":config.UserName);
			CKernel.Preferences.SetProperty("RemoteControlEnabled", config.RemoteControlEnabled);
			CKernel.Preferences.SetProperty("DownloadServerMet", config.DownloadServerMet);
			CKernel.Preferences.SetProperty("ServerMetUri", config.ServerMetUri);
			CKernel.Preferences.SetProperty("Language", config.Language);
			CKernel.Preferences.SetProperty("AllowViewShared", config.AllowViewShared);
			CKernel.Preferences.SetProperty("UseEmuleProtocol", true);
			//CKernel.Preferences.SetProperty("QueueLength", 200);
			CKernel.Preferences.SetProperty("AutoExtendSearch", true);
			CKernel.Preferences.SetProperty("RemoteControlPort",config.RemoteControlPort);
			if (config.RemoteControlPassword!="*****")
				CKernel.Preferences.SetProperty("RemotePassword",config.RemoteControlPassword);
			CKernel.Preferences.SetProperty("OSigEnabled", config.OSigEnabled);
			CKernel.Preferences.SetProperty("IPFilterEnabled", config.IPFilterEnabled);
			//CKernel.Preferences.SetProperty("UpdateServerListFromClient", false);
			//CKernel.Preferences.SetProperty("UpdateServerListFromServer", true);
			CKernel.Preferences.SetProperty("CompressionMethod", config.CompressionMethod);
			CKernel.Preferences.SetProperty("AllowedIP",config.AllowedIP);
			CKernel.Preferences.SetProperty("StartNextStoppedFile",config.StartNextStoppedFile);
		}

		public void StopFile(string strFileHash)
		{
			if (apw)
				CKernel.FilesList.StopFile(CKernel.StringToHash(strFileHash));
		}

		public void CancelFile(string strFileHash)
		{
			if (apw)
				CKernel.FilesList.CancelFile(strFileHash);
		}

		public void ResumeFile(string strFileHash)
		{
			if (apw)
				CKernel.FilesList.ResumeFile(strFileHash);
		}

		public void SetFileUploadPrioriry(string strFileHash, Constants.Priority priority)
		{
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if (Element!=null)
					Element.File.UpPriority=(Constants.Priority)priority;
			}
		}

		public void SetFileDownloadPrioriry(string strFileHash, Constants.Priority priority)
		{
			if (apw)
			{
				CKernel.FilesList.SetDownloadPriority(strFileHash,priority);
			}
		}

		public void SetFileName(string strFileHash, string newName)
		{
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if (Element!=null)
				{
					Element.File.FileName=newName;
				}
			}
		}

		public void SetFileComment(string strFileHash, string strComment, byte rating)
		{
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if (Element!=null)
				{
					Element.File.Comment=strComment;
					Element.File.Rating=rating;
				}
			}
		}

		public void GetFileComment(string strFileHash, ref string strComment, ref byte rating)
		{
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if (Element!=null)
				{
					strComment=Element.File.Comment;
					rating=Element.File.Rating;
				}
			}
		}

		public void RefreshSharedList()
		{
			if (apw)
				CKernel.FilesList.RefreshSharedList();
		}

		public int GetConnectionsNumber()
		{
			if (apw) 
			{
				return CKernel.ConnectionsList.Count();
			} 
			else return 0;
		}

		public ArrayList GetCategories()
		{
			if (apw)
				return CKernel.CategoriesList.List;
			else return null;
		}

		public bool AddorUpdateCategory(uint ID, string name, string auto, uint color)
		{
			if (apw)
			{
				bool res=CKernel.CategoriesList.AddorUpdateCategory(ID,name,auto,color);
				CKernel.FilesList.CheckAllFilesForAllCategories();
				return res;
			}
			else return false;
		}

		public void SetFileCategory(string strFileHash,string category)
		{
			if (apw)
				CKernel.FilesList.SetFileCategory(strFileHash,category);
		}
		
		public void SetFileMaxSources(string strFileHash, uint maxSources)
		{
			if (apw)
			{
				CElement Element=(CElement)CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if (Element!=null)
				{
					Element.File.MaxSources=maxSources;
				}
			}
		}

		public CCategory GetCategoryByName(string name)
		{
			if (apw)
				return CKernel.CategoriesList.GetCategoryByName(name);
			else
				return null;
		}

		public bool RemoveCategory(uint ID)
		{
			if (apw)
			{
				CKernel.FilesList.UnMarkFilesWithCategory(ID);		
				return CKernel.CategoriesList.RemoveCategory(ID);
			}
			else
				return false;
		}
	}
}
