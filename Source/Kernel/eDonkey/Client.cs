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
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;

using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for CClient.
	/// </summary>
	internal class CClient
	{
		public byte[] DownFileHash;
		public CConnection connection;

		#region Instance fields
		private bool m_EmuleProtocol;
		private uint m_Software;
		private byte m_SourceExchangeVersion;
		private byte m_VersionUDP;
		private byte m_VersionComments;
		private byte m_VersionCompression;
		private byte m_VersionExtendedRequest;
		private bool m_AllowViewSharedFiles;

		private byte[] m_UpFileHash;
		private DateTime m_LastUploadRequest;
		private DateTime m_QueueInTime;
		private uint m_ID;
		private string m_strIP;
		private uint m_ServerIP;
		private ushort m_ServerPort;
		private ushort m_UDPPort;
		private byte[] m_UserHash;
		private uint m_UploadRequests;

		private uint m_VersionMajor;
		private uint m_VersionMinor;
		private uint m_VersionRevision;

		private ushort m_Port;
		private string m_UserName;

		private uint m_QRDownload; 
		private uint m_UploadedBytes;
		private uint m_UploadedBytesAcumul;
		private uint m_DownloadedBytes;
		private uint m_DownloadedBytesAcumul;
		private byte m_ConexionTries;
		private int m_DownloadTries;

		private Protocol.DownloadState m_DownloadState;
		private Protocol.UploadState m_UploadState;
		private float m_DownloadSpeed;
		private ArrayList m_DownloadSpeeds;
		private float m_UploadSpeed;
		private ArrayList m_UploadSpeeds;

		private DateTime m_TimeStampDownload;
		private uint m_DownloadBytesLastSecond;
		private ArrayList m_UploadBlocks;
		private ArrayList m_DownloadBlocks;
		private ArrayList m_UploadDataPackets;
		private string m_ClientFileName;
		private CElement m_UploadElement;
		private CElement m_DownloadElement;
		private DateTime m_StartUploadTransferTime;
		private DateTime m_LastDownloadRequest;
		private DateTime m_LastSourcesRequest;
		private DateTime m_CallBackTime;
		private byte[] m_DownFileChunks;
		private byte[] m_UpFileChunks;
		private int m_LastChunkRequested;
		private string m_Comment;
		private byte m_Rating;
		private int m_BytesSentAcumul;
		private int m_LastSentBytes;
		private int m_AverageBytesSum;
		private bool m_SentComment;
		private StringCollection m_OtherDownFiles;
		private bool m_AskingUDP;
		private bool m_SharedFilesRequested;
		private StringCollection m_ChatMessages;
		private bool m_FirstMessage;
		private bool m_IsSpammer;
		private Constants.FriendLevel m_FriendLevel;
		private bool m_IsPartner;
		private bool m_PartnerAllowed;
		private bool m_SupportsPartner;
		private DateTime m_BadPartnerTime;
		private DateTime m_PartnerStartedTime;
		private int m_TimesPartnerSlow;
		private bool m_supportsHorde;
		
		#endregion

		#region Propertys
		public int DownloadTries
		{
			get
			{
				return m_DownloadTries;
			}
		}
		
		public uint Version
		{
			get
			{
				return m_VersionMajor*100+m_VersionMinor;
			}
		}

		public ushort Port
		{
			get
			{
				return m_Port;
			}
		}

		public string UserName
		{
			get
			{
				return m_UserName;
			}
		}

		public string Software
		{
			get
			{
				return CKernel.GetSoftwarebyID(m_Software);
			}
		}

		public uint SoftwareID
		{
			get
			{
				return m_Software;
			}
		}
		public bool EmuleProtocol
		{
			get
			{
				return m_EmuleProtocol;
			}
		}

		public ushort UDPPort
		{
			get
			{
				return m_UDPPort;
			}
		}

		public string Comment
		{
			get
			{
				return m_Comment;
			}
		}

		public byte Rating
		{
			get
			{
				return m_Rating;
			}
		}

		public bool AskingUDP
		{
			get
			{
				return m_AskingUDP;
			}
		}

		public bool DownloadVariousFiles
		{
			get
			{
				return ((m_OtherDownFiles!=null)&&(m_OtherDownFiles.Count>0));
			}
		}

		public DateTime LastUploadRequest
		{
			get
			{
				return m_LastUploadRequest;
			}
			set
			{
				if (DateTime.Now-value<new TimeSpan(0,Protocol.MinClientBan,0))
				{
					// we do not ban clients,instead they go to the end of the queue
					// as if they were a new client
					m_LastUploadRequest=DateTime.Now;
				}
				else
				{
					m_LastUploadRequest=value;
				}
			}
		}

		public DateTime QueueInTime
		{
			get
			{
				return m_QueueInTime;
			}
			set
			{
				m_QueueInTime=value;
			}
		}

		public uint UserID
		{
			get
			{
				return m_ID;
			}
		}
		
		public uint ServerIP
		{
			get
			{
				return m_ServerIP;
			}
		}
		
		public ushort ServerPort
		{
			get
			{
				return m_ServerPort;
			}
		}
		
		public byte[] UserHash
		{
			get
			{
				return m_UserHash;
			}
		}

		public DateTime LastDownloadRequest
		{
			get
			{
				return m_LastDownloadRequest;
			}
			set
			{
				m_LastDownloadRequest=value;
			}
		}

		public uint UploadRequests
		{
			get
			{
				return m_UploadRequests;
			}
		}

		public uint UploadedBytes
		{
			get
			{
				return m_UploadedBytes+m_UploadedBytesAcumul;
			}
		}

		public uint DownloadedBytes
		{
			get
			{
				return m_DownloadedBytes+m_DownloadedBytesAcumul;
			}
		}

		public string ClientFileName
		{
			get
			{
				return m_ClientFileName;
			}
		}

		public byte[] DownFileChunks
		{
			get
			{
				return m_DownFileChunks;
			}
			set
			{
				m_DownFileChunks=value;
			}
		}

		public byte[] UpFileChunks
		{
			get
			{
				return m_UpFileChunks;
			}
		}

		public CElement UploadElement
		{
			get
			{
				return m_UploadElement;
			}
		}

		public CElement DownloadElement
		{
			get
			{
				return m_DownloadElement;
			}
		}

		public uint QRDownload
		{
			get
			{
				return m_QRDownload;
			}
		}

		public bool CallBackTimedOut
		{
			get
			{
				return (((m_DownloadState==Protocol.DownloadState.WaitCallBack)
					|| (m_UploadState==Protocol.UploadState.WaitCallBack))
					&& (DateTime.Now-m_CallBackTime>new TimeSpan(0,0,35)));
			}
		}

		public Protocol.DownloadState DownloadState
		{
			get
			{
				return m_DownloadState;
			}
		}

		public Protocol.UploadState UploadState
		{
			get
			{
				return m_UploadState;
			}
			set
			{
				m_UploadState=value;
			}
		}

		public float DownloadSpeed
		{
			get
			{
				return (float)Math.Round(m_DownloadSpeed,1);
			}
		}

		public float UploadSpeed
		{
			get
			{
				return (float)Math.Round((m_UploadSpeed / 1024F),1);
			}
		}

		public bool IsFriend
		{
			get
			{
				return (byte)m_FriendLevel>0;
			}
		}

		public bool HasFriendSlot
		{
			get
			{
				return (byte)m_FriendLevel>1;
			}
		}

		public Types.Constants.FriendLevel FriendStatus
		{
			get
			{
				return m_FriendLevel;
			}
			set
			{
				m_FriendLevel=value;
			}
		}
		
		public byte[] UpFileHash
		{
			get
			{
				return m_UpFileHash;
			}
		}

		public byte VersionUDP
		{
			get
			{
				return m_VersionUDP;
			}
		}

		public bool IsPartner
		{
			get
			{
				return m_IsPartner;
			}
			set
			{
				m_IsPartner=value;
			}
		}

		#endregion

		#region Constructors & Initialization

		public CClient(CConnection in_connection)
		{
			connection=in_connection;
			m_UploadBlocks=new ArrayList();
			m_UploadDataPackets=new ArrayList();
			m_DownloadBlocks=new ArrayList();
			m_DownloadState=Protocol.DownloadState.None;
			m_UploadState=Protocol.UploadState.None;
			m_UploadElement=null;
			m_LastSourcesRequest=DateTime.MinValue;
			m_DownloadTries=0;
			m_ClientFileName="";
			m_Software=(byte)Protocol.Client.Unknown;
			m_DownloadSpeeds=new ArrayList();
			m_UploadSpeeds=new ArrayList();
			m_SentComment=false;
			m_LastChunkRequested=-1;
			m_AskingUDP=true;
			m_SharedFilesRequested=false;
			m_CallBackTime=DateTime.MinValue;
			m_FriendLevel=Types.Constants.FriendLevel.NoFriend;
			m_FirstMessage=true;
			m_IsSpammer=false;
			m_IsPartner=false;
			m_PartnerAllowed=false;
			m_SupportsPartner=false;
			m_BadPartnerTime=DateTime.MinValue;
			m_PartnerStartedTime=DateTime.MinValue;
			m_supportsHorde=false;
			m_TimesPartnerSlow=0;
		}

		public CClient(ushort in_Port,uint in_IP,uint in_ServerIP,ushort in_ServerPort,byte[] in_DownFileHash)
		{
			InitializeClient(in_Port,in_IP,in_ServerIP,in_ServerPort,in_DownFileHash);
		}

		public CClient(ushort in_Port,string in_strIP,uint in_ServerIP,ushort in_ServerPort,byte[] in_DownFileHash)
		{
			//Port=in_Port;
			IPAddress DirectionIP=IPAddress.Parse(in_strIP);
			m_ID=BitConverter.ToUInt32(DirectionIP.GetAddressBytes(),0);
			//strIP=in_strIP;

			InitializeClient(in_Port,m_ID,in_ServerIP,in_ServerPort,in_DownFileHash);		 
		}

		~CClient()
		{
			//Debug.Write("Client destroyed\n");
			if (connection!=null)
			{
				connection.CloseConnection();
				connection=null;
			}
		}

		private void InitializeClient(ushort in_Port,uint in_IP,uint in_ServerIP,ushort in_ServerPort,byte[] in_DownFileHash)
		{
			m_Port=in_Port;
			m_strIP=in_IP.ToString();
			m_ID=in_IP;
			m_ServerIP=in_ServerIP;
			m_ServerPort=in_ServerPort;
			m_ClientFileName="";
			m_LastDownloadRequest=DateTime.MinValue;
			m_LastSourcesRequest=DateTime.MinValue;
			m_SourceExchangeVersion=1;
			m_DownloadTries=0;
			m_Software=(byte)Protocol.Client.Unknown;
			if (in_DownFileHash!=null)
			{
				DownFileHash=in_DownFileHash;
			}
			else
			{
				DownFileHash=null;
			}
			m_DownloadState=Protocol.DownloadState.None;
			m_UploadState=Protocol.UploadState.None;
			m_UploadBlocks=new ArrayList();
			m_UploadDataPackets=new ArrayList();
			m_DownloadBlocks=new ArrayList();
			m_UploadElement=null;
			m_ConexionTries=0;
#if DEBUG
			IPAddress DirectionIP=new IPAddress(in_IP);
			m_UserName=DirectionIP.ToString()+":"+Convert.ToString(m_Port);
#else
			m_UserName="????";
#endif
			m_DownloadSpeeds=new ArrayList();
			m_UploadSpeeds=new ArrayList();
			m_LastChunkRequested=-1;
			m_SentComment=false;
			m_AskingUDP=true;
			m_SharedFilesRequested=false;
			m_CallBackTime=DateTime.MinValue;
			m_FriendLevel=Types.Constants.FriendLevel.NoFriend;
			m_FirstMessage=true;
			m_IsSpammer=false;
			m_IsPartner=false;
			m_PartnerAllowed=false;
			m_SupportsPartner=false;
			m_BadPartnerTime=DateTime.MinValue;
			m_PartnerStartedTime=DateTime.MinValue;
			m_supportsHorde=false;
			m_TimesPartnerSlow=0;
		}
		#endregion

		#region Public Methods
		public void ResetSentComment()
		{
			m_SentComment=false;
		}

		public uint IncUploadRequests()
		{
			return ++m_UploadRequests;
		}

		public void TryDownload()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("Intentar Descarga",m_ID);
#endif
			DateTime ant_ultimapeticion=m_LastDownloadRequest;
			m_LastDownloadRequest=DateTime.Now;
			m_AskingUDP=false;

			if ((connection!=null)&&(connection.Connected))
			{
				if (m_DownloadState!=Protocol.DownloadState.Downloading)
				{
					m_DownloadState=Protocol.DownloadState.Connected;
					m_SendRequestFile();
				}
			}
			else
			{
				if (CKernel.ConnectionsList.Count()>CKernel.Preferences.GetInt("MaxConnections"))
				{
					m_LastDownloadRequest=ant_ultimapeticion+new TimeSpan(0,0,30);
					return;
				}

				if (m_ID<Protocol.LowIDLimit)
				{
					if (CKernel.Preferences.GetUInt("ID")>Protocol.LowIDLimit)
					{
						m_LastDownloadRequest=DateTime.Now;
						m_UserName=m_UserName+"(LowID)";
						m_DownloadState=Protocol.DownloadState.Connecting;
						Connect();
					}
					else
					{
						m_UserName="LowIDtoLowID";
					}
				}
				else
				{
#if VERBOSE
					CKernel.LogClient.AddLog("Try Download,connect",m_ID);
#endif
					m_DownloadState=Protocol.DownloadState.Connecting;
					Connect();
				}
			}
		}

		public void TryUDPDownload()
		{
			m_AskingUDP=true;

			if ((m_EmuleProtocol)&&
				(m_VersionUDP>=1)&&
				(m_ID>Protocol.LowIDLimit)&&
				(m_UDPPort>0)&&
				(DownFileHash!=null)&&
				((m_QRDownload>50)||(m_QRDownload==0))&&
				((!m_SupportsPartner)||(!CKernel.Queue.AllowNewParner()))&&
				!m_AllowSourceExchangeRequest())
			{
#if VERBOSE
				CKernel.LogClient.AddLog("Intentar Descarga UDP");
#endif
				MemoryStream buffer=new MemoryStream();
				if (m_DownloadElement==null) m_DownloadElement=CKernel.FilesList[DownFileHash];
				if (m_DownloadElement==null) return;
				CFileReaskUDP PeticionUDP=new CFileReaskUDP(buffer,DownFileHash,m_VersionUDP,m_DownloadElement.File.ChunksStatus);

				IPEndPoint hostRemote=new IPEndPoint(m_ID,m_UDPPort);
				CKernel.UDPListener.SendPacketUDP(buffer.GetBuffer(),hostRemote);
			}
		}

		public bool ProcessHello(bool response,byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessHello, Response:"+response.ToString(),m_ID);
#endif
			CReceiveHello helloPacket=new CReceiveHello(response,new MemoryStream(packet));

			packet=null;
			m_UserHash=helloPacket.Hash;

			if (m_Software==(byte)Protocol.Client.Unknown) 
			{
				m_Software=helloPacket.software;
				m_VersionMinor=helloPacket.Version;
			}
			if (helloPacket.software==(byte)Protocol.Client.eMule)
				m_EmuleProtocol=true; 

#if VERBOSE
			if ((m_ID!=0)&&(m_ID!=helloPacket.UserID))
				CKernel.LogClient.AddLog("Older ID:"+Convert.ToString(m_ID)+",new:"+Convert.ToString(helloPacket.UserID),m_ID);
#endif
			if ((helloPacket.UserID==0)||(helloPacket.UserID>Protocol.LowIDLimit))
			{
				// probablemente no está conectado a un servidor y no sabe su ip pública
				// o es un idalto su id será la de su conexión
				if (connection!=null)
					m_ID=connection.IP;
				else
					m_ID=helloPacket.UserID;
			}
			else
			{
				//is a low id
				m_ID=helloPacket.UserID;
			}

			if ((m_ID!=0)&&(m_ID!=helloPacket.UserID))
				Debug.WriteLine("OldUserID:"+Convert.ToString(m_ID)+",new:"+Convert.ToString(helloPacket.UserID));

			m_Port=helloPacket.UserPort;
			m_UserName=helloPacket.UserName;
			
			m_supportsHorde=helloPacket.SupportsHorde;

			if (helloPacket.ExtendedInfoComplete)
			{
				//all the extended info is here we dont need a emule info packet emule v0.42+
				m_Software=helloPacket.IDClientCompatible;
				m_UDPPort=helloPacket.PortUDP;
				m_SourceExchangeVersion=helloPacket.VersionSourceExchange;
				m_VersionMinor=(uint)helloPacket.VersioneMuleMinor;
				m_VersionMajor=(uint)helloPacket.VersioneMuleMajor;
				m_VersionRevision=(uint)helloPacket.VersioneMuleRevision;
				m_EmuleProtocol=true;
				m_VersionUDP=helloPacket.VersionUDP;
				m_VersionComments=helloPacket.VersionComments;
				m_VersionCompression=helloPacket.VersionCompression;
				m_VersionExtendedRequest=helloPacket.VersionExtendedRequests;
				m_AllowViewSharedFiles=helloPacket.AllowViewSharedFiles;
				m_PartnerAllowed=helloPacket.PartnerAllowed;
				m_SupportsPartner=helloPacket.SupportsPartner;
			}

#if VERBOSE
			CKernel.LogClient.AddLog("User:"+m_UserName,m_ID);
#endif
			m_ServerIP=helloPacket.ServerIP;
			m_ServerPort=helloPacket.ServerPort;
			if ((CKernel.ServersList.Count<100)&&(CKernel.Preferences.GetBool("UpdateServerListFromClient"))) CKernel.ServersList.Add(m_ServerIP,m_ServerPort);
#if DEBUG
			if (m_DownloadState==Protocol.DownloadState.WaitCallBack)
			{
	#if VERBOSE
				CKernel.LogClient.AddLog("ProcessHello-Responsed callback",m_ID);
	#endif
				m_UserName="(LowID)"+m_UserName;
			}
#endif
			if (response)
			{
				m_OnConnetionReady();
			}
			return helloPacket.ExtendedInfoComplete;
		}

		public void ResponseHello()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ResponseHello",m_ID);
#endif
			MemoryStream packet=new MemoryStream();
			
			bool allowPartner=((m_SupportsPartner)&&
					(this.m_DownloadState!=Protocol.DownloadState.NoNeededParts)&&
					(CKernel.Queue.AllowNewParner()));
			CSendHello helloResponse=new CSendHello(true,packet,CKernel.ServersList.ActiveServer,m_SupportsPartner,allowPartner);
			if (connection!=null)
			{
				connection.SendPacket(packet);
			}
			m_OnConnetionReady();
		}

		public void ProcessFileRequest(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessFileRequest",m_ID);
#endif
			CFileRequest fileRequest=new CFileRequest(new MemoryStream(packet));

			if (fileRequest.FileHash==null) 
				return;

			m_UpFileHash=fileRequest.FileHash;
			byte[] tmp_UploadFileHash=fileRequest.FileHash;
			m_UploadElement=(CElement)CKernel.FilesList[m_UpFileHash];
			CElement tmpUploadElement=m_UploadElement;

			if ((m_UploadElement==null)||(m_UploadElement.File.Empty))
			{
				/*	Dont need to send no file found,we have sent it already in ProcessRequestFileID
				*	MemoryStream buffer=new MemoryStream();
				*	CNoFile noFile=new CNoFile(buffer,m_UpFileHash);
				*	m_UpFileHash=null;
				*	m_UploadElement=null;
				*	if (connection!=null) connection.SendPacket(buffer);
				*/
			}
			else
			{
#if VERBOSE
				CKernel.LogClient.AddLog("ProcessFileRequest-send file info",m_ID);
#endif
				m_UploadElement.Statistics.IncSessionRequests();
				if ((fileRequest.Partes!=null)
					&&(fileRequest.Partes.Length==m_UploadElement.File.NumChunks))
				{
					m_UpFileChunks=fileRequest.Partes;
				}

				MemoryStream fileInfoPacket=new MemoryStream();
				CFileInfo fileInfo=new CFileInfo(m_UpFileHash,m_UploadElement.File.FileName,fileInfoPacket);
				if (connection!=null)
				{
					connection.SendPacket(fileInfoPacket);
				}
				m_SendComment();
			}

			// it can be a new source for downloads
			if ((tmpUploadElement!=null)
				&&(tmpUploadElement.SourcesList!=null)
				&&(!tmpUploadElement.File.Completed)
				&&(tmpUploadElement.File.FileStatus==Protocol.FileState.Ready))
			{
				if (DownFileHash==null)
				{
#if VERBOSE
					CKernel.LogClient.AddLog("ProcessFileRequest-add client to downloads",m_ID);
#endif
					DownFileHash=new byte[16];
					Buffer.BlockCopy(tmpUploadElement.File.FileHash,0,DownFileHash,0,16);
					m_DownloadElement=tmpUploadElement;
					m_DownloadState=Protocol.DownloadState.Connected;
					if (tmpUploadElement.SourcesList.AddSource(this))
					{
						TryDownload();
					}
					else
					{
						m_DownloadState=Protocol.DownloadState.None;
					}
				}
				else
				{
					if (!CKernel.SameHash(ref DownFileHash,ref tmpUploadElement.File.FileHash))
						AddOtherDownloadFile(CKernel.HashToString(tmp_UploadFileHash));
				}
			}

		}


		public void AddOtherDownloadFile(string strDownloadFileHash)
		{
			if (m_OtherDownFiles==null)
			{
				m_OtherDownFiles=new StringCollection();
			}

			if (!m_OtherDownFiles.Contains(strDownloadFileHash))
			{
				m_OtherDownFiles.Add(strDownloadFileHash);
			}
			
			//if the alternative file has higher priority then swap
			CElement newElement=CKernel.FilesList[CKernel.StringToHash(strDownloadFileHash)];
			if (m_DownloadElement==null) m_DownloadElement=CKernel.FilesList[DownFileHash];
			if ((newElement!=null)&&
				(m_DownloadElement!=null)&&
				((byte)m_DownloadElement.File.DownPriority<(byte)newElement.File.DownPriority)&&
				((m_DownloadState==Protocol.DownloadState.OnQueue)||(m_DownloadState==Protocol.DownloadState.None)))
			{
				string previousDownloadHash=CKernel.HashToString(m_DownloadElement.File.FileHash);
				m_DownloadElement.SourcesList.RemoveSource(this); //this will swap the source to the highest priority alternative file
				//add the previous file to the alternative files
				m_OtherDownFiles.Add(previousDownloadHash);
			}
		}

		public void SwapToHigherPriorityFile()
		{
			if ((m_OtherDownFiles==null)||
				(m_OtherDownFiles.Count==0)||
				(DownFileHash==null)||
				(m_DownloadState!=Protocol.DownloadState.OnQueue)&&(m_DownloadState!=Protocol.DownloadState.None)) 
			{
				return;
			}

			if (m_DownloadElement==null) m_DownloadElement=CKernel.FilesList[DownFileHash];
			if (m_DownloadElement==null) return;
			
			bool swapNeeded=false;
			foreach (string strFileHash in m_OtherDownFiles)
			{
				CElement newElement=CKernel.FilesList[CKernel.StringToHash(strFileHash)];
				if ((newElement!=null)&&
					((byte)m_DownloadElement.File.DownPriority<(byte)newElement.File.DownPriority))
				{
					swapNeeded=true;
					break;
				}
			}
			if (!swapNeeded) return;
			//Debug.Write("Swapping "+this.UserName+m_DownloadElement.File.FileName);
			string previousDownloadHash=CKernel.HashToString(DownFileHash);
			m_DownloadElement.SourcesList.RemoveSource(this); //this will swap the source to the highest priority alternative file
			//add the previous file to the alternative files
			m_OtherDownFiles.Add(previousDownloadHash);
			//Debug.WriteLine(" to "+ m_DownloadElement.File.FileName);
		}

		public void SwapToOtherFile()
		{
			if ((m_OtherDownFiles==null)||(m_OtherDownFiles.Count==0)) 
			{
				return;
			}

			byte highestPriority=0;
			CElement tmpElement=null;
			CElement Element=null;
			int i=0;
			do
			{
				tmpElement=(CElement)CKernel.FilesList[CKernel.StringToHash(m_OtherDownFiles[i])];

				if ((tmpElement!=null)
					&&(tmpElement.SourcesList!=null)
					&&(!tmpElement.File.Completed)
					&&(tmpElement.File.FileStatus==Protocol.FileState.Ready))
				{
					if((byte)tmpElement.File.DownPriority>=highestPriority)
					{
						Element=tmpElement;
						highestPriority=(byte)tmpElement.File.DownPriority;
					}
					i++;
				}
				else
				{
					m_OtherDownFiles.RemoveAt(i);
				}
			}
			while ((i<m_OtherDownFiles.Count)&&(m_OtherDownFiles.Count>0));

			// element selected to download
			if (Element!=null)
			{
				m_OtherDownFiles.Remove(CKernel.HashToString(Element.File.FileHash));
				DownFileHash=new byte[16];
				Buffer.BlockCopy(Element.File.FileHash,0,DownFileHash,0,16);
				m_DownloadElement=Element;
				m_ClientFileName="";
				m_DownloadElement.SourcesList.AddSource(this);
				if ((connection!=null)&&connection.Connected)
				{
					m_DownloadState=Protocol.DownloadState.Connected;
					TryDownload();
				}
				else
				{
					m_DownloadState=Protocol.DownloadState.None;
				}
			}
		}

		public void ProcessStartUploadRequest(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessStartUploadRequest",m_ID);
#endif
			//si está en estado uploading quitarlo de la lista de descargas
			if (m_UploadState==Protocol.UploadState.Uploading)
			{
				if (m_SupportsPartner)
					CLog.Log(Constants.Log.Verbose,"Requested upload while uploading");
				CancelUploadTransfer(false);
			}
			m_UploadState=Protocol.UploadState.OnQueue;
			
			ushort QR;
			if (m_SupportsPartner)
				CLog.Log(Constants.Log.Verbose,"Process upload request partner");
			if ((m_SupportsPartner)&&
				(m_PartnerAllowed)&&
				(!m_IsPartner)&&
				(m_UpFileHash!=null)&&
				(m_UploadElement!=null)&&
				(m_UploadElement.SourcesList!=null)&&
				(DownloadState!=Protocol.DownloadState.NoNeededParts)&&
				(DownloadState!=Protocol.DownloadState.None)&&
				(DateTime.Now-this.m_BadPartnerTime>new TimeSpan(0,0,Protocol.BadPartnerTime,0))&&
				(CKernel.Queue.AllowNewParner()))
			{
				m_PartnerStartedTime=DateTime.Now;
				m_TimesPartnerSlow=0;
				CKernel.Queue.AddPartner(this);
				QR=1;
				CLog.Log(Constants.Log.Verbose,"Added Partner "+ this.UserName);
				m_SendQREmule(QR);
				if (m_IsPartner) TryDownload();
			}
			else
			{
				QR=CKernel.Queue.AddClient(this);

				if (m_UseExtendedProtocol()&&(QR>0))
					m_SendQREmule(QR);
				if (m_SupportsPartner)
					CLog.Log(Constants.Log.Verbose,"Partner not stablished");

			}
		}

		public void ProcessRequestFileID(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessRequestFileID",m_ID);
#endif
			CIDFileChange IDFileChange=new CIDFileChange(new MemoryStream(packet));

			if (!CKernel.SameHash(ref m_UpFileHash,ref IDFileChange.FileHash))
			{
				m_UpFileHash=IDFileChange.FileHash;
				m_UploadElement=(CElement)CKernel.FilesList[m_UpFileHash];
			}

			if ((m_UploadElement==null)||(m_UploadElement.File.Empty)) 
			{
				//Debug.Write("No se encontro fichero (Cambio ID) \n");
				MemoryStream buffer=new MemoryStream();
				CNoFile noFile=new CNoFile(buffer,m_UpFileHash);
				m_UpFileHash=null;
				if (connection!=null)
				{
					connection.SendPacket(buffer);
				}
#if VERBOSE
				CKernel.LogClient.AddLog("ProcessRequestFileID-No found",m_ID);
#endif
			}
			else
			{
#if VERBOSE
				CKernel.LogClient.AddLog("ProcessRequestFileID-send file status",m_ID);
#endif
				MemoryStream packetState=new MemoryStream();
				CFileStatus fileState;

				if (m_UploadElement.File.Completed)
				{
					fileState=new CFileStatus(m_UpFileHash,null,packetState);
				}
				else
				{
					fileState=new CFileStatus(m_UpFileHash,m_UploadElement.File.ChunksStatus,packetState);
				}

				if (connection!=null)
				{
					connection.SendPacket(packetState);
				}
			}
		}

		public void ProcessFileInfo(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessFileInfo",m_ID);
#endif
			if (DownFileHash==null) return;

			CFileInfo fileInformation=new CFileInfo(new MemoryStream(packet));

			if (!CKernel.SameHash(ref fileInformation.FileHash,ref DownFileHash))
			{
#if VERBOSE
				CKernel.LogClient.AddLog("ProcessFileInfo-no match hashes",m_ID);
#endif			
				// raise exception
				// m_DownloadElement.SourcesList.RemoveSource(this);
				Exception e=new Exception("File info hash!=DownFileHash");
				throw(e);
			}
			m_ClientFileName=fileInformation.ClientFileName;
		}

		public void ProcessChunksStatus(byte[] packet, bool udpConnection)
		{
			ProcessChunksStatus(new MemoryStream(packet),udpConnection);
		}

		public void ProcessChunksStatus(MemoryStream packet, bool udpConnection)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessChunksStatus",m_ID);
#endif
			if ((DownFileHash==null)||(m_DownloadElement==null)) return;

			CFileStatus fileStatus=new CFileStatus(packet,!udpConnection);

			if ((!udpConnection)&&(!CKernel.SameHash(ref DownFileHash,ref fileStatus.FileHash)))
			{
				Exception e=new Exception("Chunks status: invalid fileHash");
				throw(e);					
			}

			if (!CKernel.SameHash(ref DownFileHash,ref m_DownloadElement.File.FileHash))
			{
				m_DownloadElement=CKernel.FilesList[DownFileHash];
			}

			if ((CHash.GetChunksCount(m_DownloadElement.File.FileSize)!=fileStatus.nChunks)
				&& (fileStatus.nChunks!=0))
			{
				m_DownFileChunks=null;
				Exception e=new Exception("Received chunks numer does not match with file chunks number\n");
				throw(e);
			}
			if (m_DownloadElement.SourcesList!=null) m_DownloadElement.SourcesList.RemoveChunksAvaibility(m_DownFileChunks);
			uint nChunks=CHash.GetChunksCount(m_DownloadElement.File.FileSize);
			if (fileStatus.Chunks==null)
			{
				if ((m_DownFileChunks==null)||(m_DownFileChunks.Length!=nChunks))
				{
					m_DownFileChunks=new byte[nChunks];
				}

				for(int i=0; i!=m_DownFileChunks.Length; i++)
				{
					m_DownFileChunks[i]=(byte)Protocol.ChunkState.Complete;
				}
				//TODO hay alguna función para hacer esto en .NET equivalente a memset?
			}
			else
			{
				if ((m_DownFileChunks==null)||(m_DownFileChunks.Length!=nChunks))
				{
					m_DownFileChunks=new byte[nChunks];
				}
				Buffer.BlockCopy(fileStatus.Chunks,0,m_DownFileChunks,0,fileStatus.Chunks.Length);
			}

			if (fileStatus.nChunks>0)
			{
				m_DownloadState=Protocol.DownloadState.NoNeededParts;

				for (int i=0; i!=m_DownFileChunks.Length; i++)
				{
					if (((Protocol.ChunkState)m_DownFileChunks[i]==Protocol.ChunkState.Complete)
						&&((m_DownloadElement.File.ChunksStatus.Length==0)
						||((Protocol.ChunkState)m_DownloadElement.File.ChunksStatus[i]!=Protocol.ChunkState.Complete)))
					{
						m_DownloadState=Protocol.DownloadState.OnQueue;
						break;
					}
				}
			}
			else
			{
				m_DownloadState=Protocol.DownloadState.OnQueue;
			}

			//m_DownloadElement.SourcesList.UpdateChunksAvaibility();
			if (m_DownloadElement.SourcesList!=null) m_DownloadElement.SourcesList.AddChunksAvaibility(m_DownFileChunks);
			
			if (udpConnection) return;
			
			packet.Close();
			packet=null;
			// ver si necesitamos el hashset del fichero(si no lo tenemos aun lo pedimos)
			// CElement Element=(CElement)CKernel.FilesList[CKernel.HashToString(DownFileHash)];
			// TODO no es necesario pedir hashSets de ficheros de una única parte
			if ((m_DownloadElement.File.HashSetNeeded())&&(m_DownloadState!=Protocol.DownloadState.NoNeededParts))
			{
#if VERBOSE
				CKernel.LogClient.AddLog("ProcesarInformacionPartesFichero-Pide HashSet",m_ID);
#endif
				m_DownloadState=Protocol.DownloadState.ReqestHashSet;

				MemoryStream responsePacket=new MemoryStream();
				CHashSetRequest hashSet=new CHashSetRequest(DownFileHash,responsePacket);

				if (connection!=null)
				{
					connection.SendPacket(responsePacket);
				}
			}
			else
			{
				if (m_DownloadState!=Protocol.DownloadState.NoNeededParts)
				{
#if VERBOSE
					CKernel.LogClient.AddLog("ProcesarInformacionPartesFichero-Envía IniciarDescarga",m_ID);
#endif
					MemoryStream responsePacket=new MemoryStream();
					CStartDownload startDonwload=new CStartDownload(responsePacket,DownFileHash);

					if (connection!=null)
					{
						connection.SendPacket(responsePacket);
					}
				}
			}
		}

		public void ProcessHashSet(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessHashSet",m_ID);
#endif		 
			if (DownFileHash==null) return;

			Debug.WriteLine("HashSet received,file "+m_DownloadElement.File.FileName+" client "+m_UserName);
			
			CHashSetResponse hashSetResponse;

			if (m_DownloadElement.File.HashSetNeeded())
			{
				hashSetResponse=new CHashSetResponse(new MemoryStream(packet),ref m_DownloadElement.File);
			}
			else
			{
				Debug.WriteLine("File already has hash");
			}

			m_DownloadState=Protocol.DownloadState.OnQueue;

			if (!m_DownloadElement.File.HashSetNeeded())
			{
#if VERBOSE
				CKernel.LogClient.AddLog("ProcessHashSet-start download",m_ID);
#endif	
				MemoryStream responsePacket=new MemoryStream();
				CStartDownload startDownload=new CStartDownload(responsePacket,DownFileHash);
				if (connection!=null)
				{
					connection.SendPacket(responsePacket);
				}
			}
		}

		public void FileNotFound(byte[] packet)
		{
			// delete source
#if VERBOSE
			CKernel.LogClient.AddLog("FileNotFound",m_ID);
#endif	
			m_ClientFileName=CKernel.Globalization["FILE_NOT_FOUND"];
			Protocol.DownloadState prevState=m_DownloadState;
			m_DownloadState=Protocol.DownloadState.None;

			if (packet.Length!=16) 
			{
				//raise exception
				Exception e=new Exception("Invalid packet size");
				throw(e);
			}

			if (!CKernel.SameHash(ref DownFileHash,ref packet))
			{
				DownFileHash=packet;
			}

			if ((DownFileHash!=null)&&(m_DownloadElement==null))
			{
				m_DownloadElement=(CElement)CKernel.FilesList[DownFileHash];
			}

			if ((m_DownloadElement!=null)&&(m_DownloadElement.SourcesList!=null))
			{
				if (prevState==Protocol.DownloadState.Downloading)
				{
					m_CleanDownloadBlocks();
					m_LastDownloadRequest=DateTime.Now;
					m_DownloadSpeeds.Clear(); 
					m_DownloadSpeed=0;
					m_QRDownload=0;
				}
				m_DownloadElement.SourcesList.RemoveSource(this);
			}
		}

		public void ProcessIDChange(byte[] packet)
		{
			CIDClientChange CambioID=new CIDClientChange(new MemoryStream(packet));

			if (CambioID.NewID<Protocol.LowIDLimit)
			{
				CServer servidor=CKernel.ServersList[CambioID.ServerID,0];

				if (servidor!=null)
				{
					m_ID=CambioID.NewID;
					m_ServerIP=CambioID.ServerID;
					m_ServerPort=servidor.Port;
				}
			}
			else
			{
				if (connection!=null) 
				{
					m_ID=connection.IP;
				}
				CServer servidor=CKernel.ServersList[CambioID.ServerID,0];

				if (servidor!=null)
				{
					m_ServerIP=CambioID.ServerID;
					m_ServerPort=servidor.Port;
				}
			}
		}

		public void ProcessComment(byte[] packet)
		{
			CComment commentCMD=new CComment(new MemoryStream(packet));

			m_Comment=commentCMD.comment;
			m_Rating=commentCMD.rating;

			if ((m_DownloadElement==null)&&(DownFileHash!=null))
			{
				m_DownloadElement=CKernel.FilesList[DownFileHash];
			}

			if ((m_DownloadElement!=null)&&(m_DownloadElement.Comments!=null))
			{
				m_DownloadElement.Comments.NewComment(m_UserName,m_Rating,m_Comment);
			}
		}
		
		public void ProcessChatMessage(byte[] packet)
		{
			if(m_FriendLevel==Constants.FriendLevel.NoFriend && CKernel.Preferences.GetBool("ReceiveMessageFromFriendOnly")) return;

			CClientMessage ClientMessage=new CClientMessage(new MemoryStream(packet));
			
			//Do not diplay spam
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("http://")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("ed2k://")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("www")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("sex")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("porn")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("crack")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("virus")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("troyen")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("no up")>=0) m_IsSpammer=true;
			if (m_FirstMessage && ClientMessage.Message.ToLower().IndexOf("noup")>=0) m_IsSpammer=true;
			if (m_IsSpammer) return;

			//filter automated messages
			bool filtered=false;
			if (ClientMessage.Message.ToLower().IndexOf("automated message")>=0) filtered=true;
			if (ClientMessage.Message.ToLower().IndexOf("automated warning")>=0) filtered=true;
			if (ClientMessage.Message.ToLower().IndexOf("your client has an infinite queue")>=0) filtered=true;
			if (ClientMessage.Message.ToLower().IndexOf("automessage")>=0) filtered=true;
			if (ClientMessage.Message.ToLower().IndexOf("banreason")>=0) filtered=true;
			if (ClientMessage.Message.ToLower().IndexOf("auto message")>=0) filtered=true;
			if (filtered)
			{
				CKernel.Queue.RemoveClient(this);
				return;
			}
			CKernel.NewChatMessage(this,ClientMessage.Message);
			m_FirstMessage=false;
			Debug.WriteLine(ClientMessage.Message);
		}
		
		public void SendChatMessage(string message)
		{
			if (connection!=null)
			{
				CClientMessage ClientMessage;
				MemoryStream buffer;
				if ((m_ChatMessages!=null))
				{
					foreach (string msg in m_ChatMessages)
					{
						buffer=new MemoryStream();
						ClientMessage=new CClientMessage(buffer,msg);
						connection.SendPacket(buffer);
					}
					m_ChatMessages.Clear();
					m_ChatMessages=null;
				}
				buffer=new MemoryStream();
				if (message.Length>0)
				{
					ClientMessage=new CClientMessage(buffer,message);
					connection.SendPacket(buffer);
				}
			}
			else
			{
				if (m_ChatMessages==null) m_ChatMessages=new StringCollection();
				m_ChatMessages.Add(message);
				Connect();
			}

		}
		public void SendHashSet()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("SendHashSet",m_ID);
#endif	
			CElement element=(CElement)CKernel.FilesList[m_UpFileHash];
			if (element==null) return;
			MemoryStream responsePacket=new MemoryStream();
			CHashSetResponse hashSetResponse=new CHashSetResponse(responsePacket,element.File);

			if (connection!=null)
			{
				connection.SendPacket(responsePacket);
			}
		}

		public void RequestBlocks()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("RequestBlocks,download blocks.count:"+Convert.ToString(m_DownloadBlocks.Count),m_ID);
#endif
			if (m_DownloadElement==null)
			{
				if (DownFileHash!=null) 
				{
					m_DownloadElement=(CElement)CKernel.FilesList[DownFileHash];

					if ((m_DownloadElement!=null)&&(m_DownloadElement.SourcesList!=null))
					{
						m_DownloadElement.SourcesList.AddSource(this);
					}
					else
					{
						// probablemente el fichero ya se ha completado
						return;
					}
				}
				else
				{
					return;
				}
			}
			m_DownloadState=Protocol.DownloadState.Downloading;

			//CElement Element=(CElement)CKernel.FilesList[CKernel.HashToString(DownFileHash)];
			uint start=0;
			uint end=0;
			int chunk=0;
			CFileBlock[] blocks=new CFileBlock[3];

			for (uint i=0;i!=3;i++)
			{
				blocks[i]=new CFileBlock();
				//Debug.Write("LastChunk:"+m_LastChunkRequested.ToString());
				chunk=m_DownloadElement.SourcesList.SelectChunk(m_DownFileChunks,m_LastChunkRequested);
				m_LastChunkRequested=chunk;
				//Debug.WriteLine("NewChunk:"+m_LastChunkRequested.ToString());
				if (chunk>=0)
				{
					if (m_DownloadElement.File.GetNewBlockInChunk((uint)chunk,ref start,ref end))
					{
						blocks[i].start=start;
						blocks[i].end=end;
						blocks[i].FileHash=DownFileHash;
						blocks[i].position=start;
						//blocks[i].buffer=new Byte[fin+1-inicio];
						m_DownloadBlocks.Add(blocks[i]);

						if ((end==0)&&(start==0))
						{
							Debug.WriteLine("no more blocks to request "+Convert.ToString(chunk));
						}
						else
						{
							//Debug.WriteLine(m_UserName+", ChunksStatus = "+Convert.ToString(m_DownloadElement.File.ChunksStatus[0])/*+Convert.ToString(m_DownloadElement.File.GetChunksStatus()[1])*/);
#if VERBOSE
							CKernel.LogClient.AddLog("RequestBlocks-chunk requested "+Convert.ToString(chunk)+" block: "+Convert.ToString(start)+"-"+Convert.ToString(end)+"\n",m_ID);
#endif
						}
					}
				}
			}

			if ((blocks[0].start==0)&&(blocks[0].end==0))
			{
#if VERBOSE
				CKernel.LogClient.AddLog("RequestBlocks-first block is null,no request blocks",m_ID);
#endif
#if DEBUG
				Debug.WriteLine(m_UserName+" Parte: "+Convert.ToString(chunk)+" first block is null,no request blocks");

				for (int j=0;j<m_DownloadElement.File.ChunksStatus.Length;j++)
				{
					if (((m_DownloadElement.File.ChunksStatus[j]==(byte)Protocol.ChunkState.Empty)||
						(m_DownloadElement.File.ChunksStatus[j]==(byte)Protocol.ChunkState.InProgress))&&
						(m_DownFileChunks[j]==(byte)Protocol.ChunkState.Complete))
					Debug.WriteLine("Chunk not selected when available");
				}
#endif
				if (m_DownloadBlocks.Count==0)
				{
					//no needed new blocks to download and nothing pending for receiving
					bool willBeChangedToOtherFile=((m_OtherDownFiles!=null)&&(m_OtherDownFiles.Count>0));
					CancelDownloadTransfer(false);
					if (!willBeChangedToOtherFile)
					{
						m_DownloadState=Protocol.DownloadState.NoNeededParts;
					}
				}
			}
			else
			{
				MemoryStream packet=new MemoryStream();
				CFileBlockRequest fileBlockRequest=new CFileBlockRequest(packet,DownFileHash,ref blocks[0],ref blocks[1],ref blocks[2]);

				if (connection!=null)
				{
					connection.SendPacket(packet);
				}
			}
		}

		public void ReceiveBlock(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ReceiveBlock",m_ID);
#endif	
			MemoryStream streamPacket=new MemoryStream(packet);
			CReceivedBlock receivedBlock=new CReceivedBlock(streamPacket);
			streamPacket=null;

			m_DownloadedBytes+=receivedBlock.End-receivedBlock.Start;

			if (m_DownloadBlocks.Count<=0) return;

			if (!CKernel.SameHash(ref DownFileHash,ref receivedBlock.FileHash)) return;

			m_ProcessReceivedBlock(receivedBlock);
		}

		public void ReceiveCompressedBlock(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("RecibirBloqueComprimido",m_ID);
#endif	
			MemoryStream streamPacket=new MemoryStream(packet);
			CReceivedCompressedBlock compressedBlock=new CReceivedCompressedBlock(ref streamPacket);

			if (!CKernel.SameHash(ref DownFileHash,ref compressedBlock.FileHash))
			{
				return;
			}

			m_ProcessCompressedBlock(compressedBlock);
		}

		public float UpdateDownloadSpeed()
		{
			//TimeSpan difference=DateTime.Now-m_TimeStampDownload;
			//m_DownloadSpeed=(float)(m_DownloadedBytes-m_DownloadBytesLastSecond) / ((float)diferencia.TotalMilliseconds / 1000F);

//			if (difference>new TimeSpan(0,0,1))
//			{
			if (m_DownloadSpeeds.Count>=30)
			{
				m_DownloadSpeeds.RemoveAt(0);
			}

			m_DownloadSpeeds.Add(m_DownloadedBytes-m_DownloadBytesLastSecond);
			//_DownloadedBytes=0;
			m_DownloadBytesLastSecond=m_DownloadedBytes;
			m_TimeStampDownload=DateTime.Now;

			uint totalDownload=0;

			foreach (uint down_i in m_DownloadSpeeds)
			{
				totalDownload+=down_i;
			}
			m_DownloadSpeed=totalDownload/m_DownloadSpeeds.Count;
			//}
			return m_DownloadSpeed;
		}

		public void ProcessRequestParts(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessRequestParts",m_ID);
#endif
			if (m_UploadState!=Protocol.UploadState.Uploading) return;

			CFileBlockRequest BlockRequest=new CFileBlockRequest(new MemoryStream(packet));

			foreach (CFileBlock block in BlockRequest.RequestedBlocks)
			{
				m_UploadBlocks.Add(block);
			}
		}

		public void ProcessOutOfParts()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessOutOfParts",m_ID);
#endif
			//CLog.Log(Constants.Log.Verbose,"Transfer interrupted: out of parts");
			m_CleanDownloadBlocks();
			
			m_DownloadSpeeds.Clear();
			m_DownloadSpeed=0;
			m_DownloadState=Protocol.DownloadState.OnQueue;

			if (DownFileHash!=null)
			{
				MemoryStream responsePacket=new MemoryStream();
				CStartDownload StartDownload=new CStartDownload(responsePacket,DownFileHash);
			
				if (connection!=null)
				{
					connection.SendPacket(responsePacket);
				}
			}
		}

		public void ProcessSharedListRequest()
		{
			MemoryStream buffer=new MemoryStream();
			CSharedFiles SharedResponse=new CSharedFiles(buffer,CKernel.FilesList);

			if (connection!=null)
			{
				connection.SendPacket(buffer);
			}
		}

		public void ProcessSharedListResponse(byte[] packet)
		{
			m_SharedFilesRequested=false;
			CSearcher search=new CSearcher();
			int searchID=++CKernel.LastSearch;
//			if (CKernel.Searchs.Count==0)
//				searchID=1;
//			else
//				searchID=(int)CKernel.Searchs.GetKey(CKernel.Searchs.Count-1)+1;
			CKernel.Searchs.Add(searchID,search);
			CKernel.NewSearchTab(searchID,m_UserName);
			CServerSearchResults ServerSearchResults=new CServerSearchResults(new MemoryStream(packet),search,false);
		}

		public void RequestSharedList()
		{
			if (connection!=null)
			{
				m_SharedFilesRequested=false;
				MemoryStream packet=new MemoryStream();
				CAskSharedFiles AskSharedFiles=new CAskSharedFiles(packet);
				connection.SendPacket(packet);
			}
			else
			{
				m_SharedFilesRequested=true;
				Connect();
			}
		}

		public void SendEmuleHello(bool isResponse)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("SendEmuleHello response:"+Convert.ToString(isResponse),m_ID);
#endif
			MemoryStream responsePacket=new MemoryStream();
			CMuleHello HolaeMule=new CMuleHello(responsePacket,isResponse);

			if (connection!=null)
			{
				connection.SendPacket(responsePacket);
			}
		}

		public void ProcessEmuleHello(byte[] packet,bool isResponse)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessEmuleHello",m_ID);
#endif
			MemoryStream buffer=new MemoryStream(packet);
			CMuleHello MuleHello=new CMuleHello(buffer);
			m_UDPPort=MuleHello.PortUDP;
			m_SourceExchangeVersion=MuleHello.VersionSourceExchange;
			//m_VersionMinor=(uint)MuleHello.VersioneMule;
			m_VersionMinor=Convert.ToUInt32(MuleHello.VersioneMule.ToString("X"));
			m_VersionMajor=0;
			m_VersionRevision=0;
			m_EmuleProtocol=true;
			m_Software=MuleHello.IDClientCompatible;
			if (m_Software==(byte)Protocol.Client.eLePhant)
			{
				m_VersionMinor=MuleHello.VersionLphant;
			}
			m_VersionUDP=MuleHello.VersionUDP;
			m_VersionComments=MuleHello.VersionComments;
			m_VersionCompression=MuleHello.VersionCompression;
			m_VersionExtendedRequest=MuleHello.VersionExtendedRequests;

			if (!isResponse) SendEmuleHello(true);
		}

		public void ProcessEmuleQR(byte[] packet)
		{
			m_QRDownload=BitConverter.ToUInt16(packet,0);
			packet=null;
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessEmuleQR-Pos:"+Convert.ToString(m_QRDownload),m_ID);
#endif
		}

		public void ProcessQR(byte[] packet)
		{
			BinaryReader reader=new BinaryReader(new MemoryStream(packet));
			m_QRDownload=reader.ReadUInt32();
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessQR-Pos:"+Convert.ToString(m_QRDownload),m_ID);
#endif
		}

		public void ProcessUDPReask(ushort QR)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessUDPReask",m_ID);
#endif
			m_QRDownload=(uint)QR;
			m_LastDownloadRequest=DateTime.Now;
			m_AskingUDP=false;
		}

		public void ProcessSourceRequest(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessSourceRequest",m_ID);
#endif				
			MemoryStream requestBuffer=new MemoryStream(packet);
			CRequestSourceExchange requestSourceExchange=new CRequestSourceExchange(requestBuffer);

			requestBuffer.Close();
			requestBuffer=null;

			MemoryStream responsePacket=new MemoryStream();
			CSourceExchangeResponse sourceExchangeResponse=new CSourceExchangeResponse(responsePacket,requestSourceExchange.FileHash,m_ID,m_Port,m_SourceExchangeVersion);

			if (sourceExchangeResponse.nSources>0)
			{
#if VERBOSE
				CKernel.LogClient.AddLog("Sent "+Convert.ToString(sourceExchangeResponse.nSources)+" sources\n",m_ID);
#endif
				if (connection!=null)
				{
					connection.SendPacket(responsePacket);
				}
			}
		}

		public void ProcessSources(byte[] packet)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessSources",m_ID);
#endif	
			CSourceExchangeResponse SourceExchangeResponse;

			try
			{
				SourceExchangeResponse=new CSourceExchangeResponse(new MemoryStream(packet),m_SourceExchangeVersion);
				
				Debug.WriteLine(DateTime.Now.ToLongTimeString() + " : Received SourceExchange = "+SourceExchangeResponse.nSources.ToString()+" from "+m_UserName+" | FileHash = "+ CKernel.HashToString(SourceExchangeResponse.FileHash));

				CElement Element=(CElement)CKernel.FilesList[SourceExchangeResponse.FileHash];

				if ((Element!=null)&&(Element.File!=null)&&(!Element.File.Completed))
				{
					CKernel.ClientsList.AddClientsToFile(SourceExchangeResponse.Sources,SourceExchangeResponse.FileHash);
				}
			}
			catch
			{
				Debug.WriteLine("Invalid source exchange packet"+m_Software+" "+m_SourceExchangeVersion);
			}
		}

		public void CancelUploadTransfer(bool enqueue)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("CancelUploadTransfer. Encolar: "+Convert.ToString(enqueue),m_ID);
#endif
			CKernel.Queue.RemoveUpload(this,enqueue);

			if (m_SupportsPartner)
				CLog.Log(Constants.Log.Verbose,"Cancelled upload with partener enabled client");
			
			m_IsPartner=false;
			m_UploadedBytesAcumul+=m_UploadedBytes;
			m_UploadedBytes=0;
			m_LastSentBytes=0;
			m_AverageBytesSum=0;
			m_UploadSpeeds.Clear();
			m_UploadSpeed=0;
			m_UploadBlocks.Clear();
			m_UploadDataPackets.Clear();
		}

		public void StartUpload()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("StartUpload",m_ID);
#endif
			m_StartUploadTransferTime=DateTime.Now;

			if ((connection!=null)&&connection.Connected)
			{
				m_UploadState=Protocol.UploadState.Uploading;
				m_UploadedBytes=0;

				MemoryStream paquete=new MemoryStream();
				CStartUpload IniciarUpload=new CStartUpload(paquete);

				if (connection!=null)
				{
					connection.SendPacket(paquete);
				}
			}
			else
			{
				m_UploadState=Protocol.UploadState.Connecting;
				Connect();
			}
		}

		public void RemoveDownload()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("RemoveDownload",m_ID);
#endif	
			
			if (m_DownloadState==Protocol.DownloadState.Downloading)
			{
				CancelDownloadTransfer(true);
			}
			CKernel.SourcesOld.AddClient(this);
//			DownFileHash=null;
//			m_DownFileChunks=null;
			m_DownloadElement=null;
			m_DownloadState=Protocol.DownloadState.None;
		}

		public void OnDisconnect(byte reason)
		{
			connection=null;
			if (m_SharedFilesRequested) CLog.Log(Constants.Log.Notify,"CLI_DONT_SEND_SHARED",m_UserName);
			m_SharedFilesRequested=false;
#if VERBOSE
			CKernel.LogClient.AddLog("OnDisconnect-Client",m_ID);
#endif
#if DEBUG
			string strReason=CKernel.Globalization["UNK_REASON"];

			switch((Protocol.ConnectionReason)reason)
			{
				case Protocol.ConnectionReason.CannotConnect:
					strReason=CKernel.Globalization["CANT_CONNECT"];
					break;
				case Protocol.ConnectionReason.ClosedConnection:				
					strReason=CKernel.Globalization["CONNECTION_CLOSED"];
					break;
				case Protocol.ConnectionReason.NullPacket:				
					strReason=CKernel.Globalization["NULL_PACKET"];
					break;
				case Protocol.ConnectionReason.NullID:				
					strReason=CKernel.Globalization["NULL_ID"];
					break;
				case Protocol.ConnectionReason.InvalidHeader:
					strReason=CKernel.Globalization["INVALID_HEADER"];
					break;
				case Protocol.ConnectionReason.TimeOut:
					strReason=CKernel.Globalization["TIMEOUT"];
					break;
			}
#endif
#if VERBOSE
			CKernel.LogClient.AddLog("OnDisconnect-"+strReason,m_ID);
#endif
			if (m_ChatMessages!=null)
			{
				m_ChatMessages.Clear();
				m_ChatMessages=null;
			}

			if (m_UploadState==Protocol.UploadState.Uploading)
			{
				CancelUploadTransfer(true);
			}

			if ((m_UploadState==Protocol.UploadState.Connecting)
				|| (m_UploadState==Protocol.UploadState.WaitCallBack))
			{
				CancelUploadTransfer(false);
			}

			if ((m_DownloadState==Protocol.DownloadState.Connecting)
				||(m_DownloadState==Protocol.DownloadState.WaitCallBack))
			{
				if ((m_ConexionTries<Protocol.ConnectionRetries)
					&&(reason==(byte)Protocol.ConnectionReason.CannotConnect))
				{
#if DEBUG
					m_UserName=strReason;
#endif
					Connect();
				}
				else
				{
					//quitar la fuente
					m_ConexionTries=0;
					m_DownloadState=Protocol.DownloadState.None;

					if (DownFileHash!=null)
					{
						if (m_OtherDownFiles!=null)
						{
							m_OtherDownFiles.Clear();
						}

						m_OtherDownFiles=null;

						if (m_DownloadElement==null)
						{
							m_DownloadElement=(CElement)CKernel.FilesList[DownFileHash];
						}
						if ((m_DownloadElement!=null)&&(m_DownloadElement.SourcesList!=null)) m_DownloadElement.SourcesList.RemoveSource(this);
					}
					else
					{
						Debug.WriteLine(m_UserName+"Never should enter here-> null DownFileHash");
					}
#if DEBUG
					m_UserName=strReason;
#endif
				}
			}

			if (m_DownloadState==Protocol.DownloadState.Connected)
			{
				m_DownloadState=Protocol.DownloadState.None;

				if (m_DownloadElement==null)
				{
					m_DownloadElement=(CElement)CKernel.FilesList[DownFileHash];
				}

				m_DownloadElement.SourcesList.RemoveSource(this);
			}

			//free all pending blocks
			if (m_DownloadState==Protocol.DownloadState.Downloading)
			{
				//CLog.Log(Constants.Log.Verbose,"Transfer interrrupted,reason: {0}",strReason);
				m_CleanDownloadBlocks();

				m_LastDownloadRequest=DateTime.Now;
				m_DownloadSpeeds.Clear(); 
				m_DownloadSpeed=0;
				m_QRDownload=0;
				m_DownloadState=Protocol.DownloadState.OnQueue;
			}
			//for eny reason some clients (edonkeys mainly) have pending blocks but their downloadstate!=Downloading
			if (m_DownloadBlocks.Count>0)
			{
				m_CleanDownloadBlocks();
//					Debug.WriteLine(m_DownloadState.ToString());
//					Debug.WriteLine("Disconnected leaving pending blocks!!!");
			}
			CKernel.ClientsList.IsClientNeeded(this);
		}

		public void Connect()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("Connect",m_ID);
#endif
			m_ConexionTries++;

			if (m_ID>Protocol.LowIDLimit)
			{
				if (connection==null)
				{
					connection=new CConnection(m_ID,m_Port,this);
				}
				else
				{
					connection.ReOpenConnection();
				}

				MemoryStream packet=new MemoryStream();
#if VERBOSE
				CKernel.LogClient.AddLog("Connect-send hello",m_ID);
#endif
				bool allowPartner=((this.m_DownloadState!=Protocol.DownloadState.NoNeededParts)&&
					(CKernel.Queue.AllowNewParner()));
				CSendHello sendHello=new CSendHello(false,packet,CKernel.ServersList.ActiveServer,true,allowPartner);

				if (connection!=null)
				{
					connection.SendPacket(packet);
					connection.Connect();
				}
			}
			else
			{
				m_CallBackTime=DateTime.Now;

				if (m_DownloadState==Protocol.DownloadState.Connecting)
				{
					m_DownloadState=Protocol.DownloadState.WaitCallBack;
				}

				if (m_UploadState==Protocol.UploadState.Connecting)
				{
					m_UploadState=Protocol.UploadState.WaitCallBack;
				}

				if (CKernel.ServersList.IsTheActiveServer(m_ServerIP,m_ServerPort))
				{
					CKernel.ServersList.ActiveServer.RequestCallBack(m_ID);
#if VERBOSE
					CKernel.LogClient.AddLog("Connect-RequestCallBack",m_ID);
#endif
				}
				else
				{
#if VERBOSE
					CKernel.LogClient.AddLog("LowID other server-Connect-RequestCallBack",m_ID);
#endif
					//don't request udp callbacks to save servers bandwidth
					//this source will be deleted later
					//CKernel.ServersList.RequestCallBackUDP(m_ServerIP,m_ServerPort,m_ID);
				}
			}
#if VERBOSE
			CKernel.LogClient.AddLog("Connect-connected ",m_ID);
#endif
		}

		public void OnConnected()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("OnConnected",m_ID);
#endif
		}

		public int SendBlockData(int maxSend)
		{
#if VERBOSE
			//CKernel.LogClient.AddLog("SendBlockData",m_ID);
#endif
			try
			{
				m_AverageBytesSum+=m_LastSentBytes;
				m_UploadSpeeds.Add(m_LastSentBytes);

				if (m_UploadSpeeds.Count>500)
				{
					m_AverageBytesSum -= (int)(m_UploadSpeeds[0]);
					m_UploadSpeeds.RemoveAt(0);
				}

				if (m_UploadSpeeds.Count>10)
				{
					m_UploadSpeed=10 * m_AverageBytesSum / m_UploadSpeeds.Count;
				}
				else
				{
					m_UploadSpeed=0;
				}

				m_LastSentBytes=0;

				if (m_ShouldCancelUpload()) return 0;

				if ((connection==null)||(connection.Sending)||(!connection.Connected))
				{
					return 0;
				}
			
				if ((m_UploadDataPackets.Count==0)&&(m_UploadBlocks.Count!=0))
				{
					if (!m_CreateDataPackets()) return 0;
				}

				if (m_UploadDataPackets.Count==0) return 0;

				int bytesSent;

				m_BytesSentAcumul+=maxSend;
				
				int nextPacketSize=connection.GetNextFreedPacketSize();
				
				while ((m_BytesSentAcumul>=nextPacketSize)
					&&((nextPacketSize>0)
					||(m_UploadDataPackets.Count>0)
					||(m_UploadBlocks.Count>0)))
				{
					bytesSent=connection.FreeDataBlock();
					m_BytesSentAcumul-=bytesSent;
					m_UploadedBytes+=(uint)bytesSent;
					if (bytesSent==0)
					{
						if (m_UploadDataPackets.Count>0)
						{
							connection.SendDataPacket((MemoryStream)m_UploadDataPackets[0]);
							m_UploadDataPackets.RemoveAt(0);
							bytesSent=connection.FreeDataBlock();
							m_BytesSentAcumul -= bytesSent;
							m_UploadedBytes+=(uint)bytesSent;
						}
					}
					if ((m_UploadDataPackets.Count<=0)&&(m_UploadBlocks.Count>0))
					{
						m_CreateDataPackets();
					}
					nextPacketSize=connection.GetNextFreedPacketSize();
				}
				m_LastSentBytes=maxSend; //- m_BytesSentAcumul;
				if (m_UploadElement!=null) m_UploadElement.Statistics.IncSessionUpload((uint)m_LastSentBytes);
				return maxSend;
			}
			catch
			{
				CancelUploadTransfer(false);
				//maybe the exception is because the file has been deleted
				//try to remove the file to avoid more exceptions
				CKernel.FilesList.RemoveSharedFileIfNotExists(m_UpFileHash);
				return 0;
			}
		}
		#endregion

		#region Internal Methods
		private bool m_UseExtendedProtocol()
		{
			return (m_EmuleProtocol)&&(CKernel.Preferences.GetBool("UseEmuleProtocol"));
		}

		private void m_SendComment()
		{
			if ((m_EmuleProtocol)
				&&(!m_SentComment)
				&&(m_VersionComments>0)
				&&(m_UploadElement.File.HasComment))
			{
				MemoryStream paqueteComment=new MemoryStream();
				CComment Comment=new CComment(paqueteComment,m_UploadElement.File.Rating,m_UploadElement.File.Comment);
				if (connection!=null)
				{
					connection.SendPacket(paqueteComment);
				}
				m_SentComment=true;
			}
		}

		private void m_SendRequestFile()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("SendRequestFile",m_ID);
#endif
			m_LastDownloadRequest=DateTime.Now;
			m_DownloadTries++;

			if (DownFileHash==null) return;

			m_DownloadElement=(CElement)CKernel.FilesList[DownFileHash];

			if (m_DownloadElement==null) return;

			MemoryStream packet=new MemoryStream();
			CFileRequest fileRequest;

			if ((m_EmuleProtocol)&&(m_VersionExtendedRequest>0))
			{
				fileRequest=new CFileRequest(DownFileHash,m_DownloadElement.File.ChunksStatus,packet);
			}
			else
			{
				fileRequest=new CFileRequest(DownFileHash,null,packet);
			}

//			if (connection!=null)
//			{
//				connection.SendPacket(packet);
//			}

			//packet=new MemoryStream();
			packet.Seek(0,SeekOrigin.End);
			CIDFileChange IDFileChange=new CIDFileChange(DownFileHash,packet);

//			if (connection!=null)
//			{
//				connection.SendPacket(packet);
//			}
			
			if ((m_supportsHorde)&&(m_DownloadElement.File.CrumbsHashSetNeeded()))
			{
				//packet.Seek(0,SeekOrigin.End);
				//CHordeSlotRequest HordeSlotRequest=new CHordeSlotRequest(DownFileHash,packet);
				CLog.Log(Constants.Log.Verbose,"Requesting crumhashset to "+m_UserName+" for "+m_DownloadElement.File.FileName);
				packet.Seek(0,SeekOrigin.End);
				CCrumbSetRequest CrumbSetRequest=new CCrumbSetRequest(DownFileHash,packet);
			}
			
			// Request sources if allowed
			if (m_AllowSourceExchangeRequest())
			{
#if VERBOSE
				CKernel.LogClient.AddLog("m_SendRequestFile-RequestSources",m_ID);
#endif
				Debug.WriteLine(DateTime.Now.ToLongTimeString()+" : Requesting SourceExchange "+m_DownloadElement.File.FileName+" to "+ m_UserName+" | FileHash = "+ CKernel.HashToString(DownFileHash));
				
				//MemoryStream sourcesPacket=new MemoryStream();
				packet.Seek(0,SeekOrigin.End);
				CRequestSourceExchange requestSourceExchange=new CRequestSourceExchange(packet,DownFileHash);
				
				if (connection!=null)
				{
					m_LastSourcesRequest=DateTime.Now;
					m_DownloadElement.SourcesList.SetLastSourceExchangeRequest();
					//connection.SendPacket(sourcesPacket);
				}
			}
			if (connection!=null)
			{
				connection.SendPacket(packet);
			}
		}
		private bool m_AllowSourceExchangeRequest()
		{
			long nextReask;
			if ((!m_EmuleProtocol)||(DownFileHash==null)) return false;
			if (m_DownloadElement==null) m_DownloadElement=CKernel.FilesList[DownFileHash];
			if ((m_DownloadElement==null)||(m_DownloadElement.SourcesList==null)) return false;
			if (DateTime.Now.Ticks-m_DownloadElement.Statistics.StartTime.Ticks<Protocol.StartUpPeriod)
				nextReask = Protocol.ReaskSourceAtStartUp;
			else
				nextReask = Protocol.ReaskSource;			
			return ((DateTime.Now.Ticks-m_LastSourcesRequest.Ticks>Protocol.ReaskSameSource)
				&& (DateTime.Now.Ticks-m_DownloadElement.SourcesList.GetLastSourceExchangeRequest().Ticks>nextReask)
				&& (m_SourceExchangeVersion>=1)
				&& (CKernel.Preferences.GetBool("UseEmuleProtocol"))
				&& (m_DownloadElement.File.MaxSources>m_DownloadElement.SourcesList.Count()-5));

		}
		private void m_ProcessReceivedBlock(CReceivedBlock receivedBlock)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessReceivedBlock",m_ID);
#endif
			int blockIndex=0;
			CFileBlock block;
			//CElement Element=(CElement)CKernel.FilesList[CKernel.HashToString(DownFileHash)];

			CKernel.GlobalStatistics.IncSessionDown(receivedBlock.Data.Length);

			lock (m_DownloadBlocks)
			{
				while (blockIndex<m_DownloadBlocks.Count)
				{
					block=(CFileBlock)m_DownloadBlocks[blockIndex];

					if ((block.start<=receivedBlock.Start)&&(block.end+1>=receivedBlock.End))
					{
						if (block.buffer==null)
						{
							block.buffer=new Byte[block.end+1-block.start];
						}

						block.position=receivedBlock.End;
						Buffer.BlockCopy(receivedBlock.Data,0,block.buffer,(int)(receivedBlock.Start-block.start),(int)(receivedBlock.End-receivedBlock.Start));

						if (block.end+1==receivedBlock.End) 
						{
							receivedBlock.Data=null;
							receivedBlock=null;

							if (!m_DownloadElement.File.WriteBlock(block.start,block.end,block.buffer))
							{
								m_DownloadElement.File.RemoveRequestedBlock(block.start,block.end);
								CKernel.FilesList.StopFile(m_DownloadElement.File.FileHash);
							}
							m_DownloadElement.Statistics.IncSessionDownload(block.end-block.start+1);

							block.buffer=null;
							block=null;

							if (m_DownloadBlocks.Count>0)
							{
								m_DownloadBlocks.RemoveAt(blockIndex);
							}
							//Debug.WriteLine(GC.GetTotalMemory(true).ToString());
							//GC.Collect();

							if ((m_DownloadBlocks.Count<4)
								&& (m_DownloadState==Protocol.DownloadState.Downloading)
								&& (m_DownloadElement.File.FileStatus==Protocol.FileState.Ready))
							{
								RequestBlocks();
							}
						}
						break;
					}
					blockIndex++;
				}
//				if (blockIndex==m_DownloadBlocks.Count)
//				{
//					CLog.Log(Constants.Log.Verbose,"Requested part not found");
//				}
			}
		}

		private void m_ProcessCompressedBlock(CReceivedCompressedBlock receivedBlock)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("ProcessCompressedBlock",m_ID);
#endif	
			int blockIndex=0;
			CFileBlock block;

			CKernel.GlobalStatistics.IncSessionDown(receivedBlock.Data.Length);
			
			//CElement Element=(CElement)CKernel.FilesList[CKernel.HashToString(DownFileHash)];
			
			lock (m_DownloadBlocks)
			{
				while (blockIndex<m_DownloadBlocks.Count)
				{
					block=(CFileBlock)m_DownloadBlocks[blockIndex];

					if (block.start==receivedBlock.Start) //&&(block.end+1>=RecepcionBloque.fin))
					{
						if (block.buffer==null)
						{
							block.buffer=new Byte[block.end+1-block.start];
						}
						block.compressed=true;
						Buffer.BlockCopy(receivedBlock.Data,0,block.buffer,(int)(block.position-block.start),receivedBlock.Data.Length);
						m_DownloadedBytes+=(uint)receivedBlock.Data.Length;
						
						//ActualizarVelocidadDescarga();
						block.position+=(uint)receivedBlock.Data.Length;

						if (block.position==receivedBlock.End) 
						{
							receivedBlock.Data=null;
							receivedBlock=null;
							int uncompressedSize=CReceivedCompressedBlock.Uncompress(ref block.buffer);

							if (uncompressedSize==block.end-block.start+1)
							{
								if (!m_DownloadElement.File.WriteBlock(block.start,block.start+(uint)block.buffer.Length-1,block.buffer))
								{
									m_DownloadElement.File.RemoveRequestedBlock(block.start,block.end);
									CKernel.FilesList.StopFile(m_DownloadElement.File.FileHash);
								}
								m_DownloadElement.Statistics.IncSessionDownload(block.end-block.start+1);
							}
							else
							{
								m_DownloadElement.File.RemoveRequestedBlock(block.start,block.end);
								Debug.Write("Error uncompressing block,size: "+Convert.ToString(uncompressedSize));
							}

							block.buffer=null;
							block=null;
							
							if (m_DownloadBlocks.Count>0)
							{
								m_DownloadBlocks.RemoveAt(blockIndex);
							}
							//Debug.WriteLine(GC.GetTotalMemory(true).ToString());
							//GC.Collect();

							if ((m_DownloadBlocks.Count<4)
								&& (m_DownloadState==Protocol.DownloadState.Downloading))
							{
								RequestBlocks();
							}
						}
						break;
					}
					blockIndex++;
				}
//				if (blockIndex==m_DownloadBlocks.Count)
//				{
//					CLog.Log(Constants.Log.Verbose,"Requested part not found");
//				}
			}
		}

		private void m_SendQREmule(ushort QR)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("SendQREmule",m_ID);
#endif			
			MemoryStream packet=new MemoryStream();
			CMuleQR MuleQR=new CMuleQR(packet,QR);

			if (connection!=null)
			{
				connection.SendPacket(packet);
			}
		}

		public void ProcessCrumbSetResponse(byte[] packet)
		{
			if ((m_DownloadElement.File.HashSetNeeded())||(m_DownloadElement.File.CrumbsHashSetNeeded()))
			{
				CLog.Log(Constants.Log.Verbose,m_UserName+" Len:"+packet.Length+" CrumbSetResponse: "+m_DownloadElement.File.FileName);
				CCrumbSetResponse crumbSetResponse=new CCrumbSetResponse(new MemoryStream(packet),ref m_DownloadElement.File);
			}
			if (!m_DownloadElement.File.CrumbsHashSetNeeded()) TryDownload();
		}

		private void CancelDownloadTransfer(bool cleaningFromsourcesList)
		{
#if VERBOSE
			CKernel.LogClient.AddLog("CancelDownloadTransfer",m_ID);
#endif
			Debug.Write("Cancelling transfer with "+m_UserName+"\n");

			//CLog.Log(Constants.Log.Verbose,"Cancelling transfer no need parts");

			MemoryStream packet=new MemoryStream();
			CCancelTransfer cancelarCmd=new CCancelTransfer(packet);

			if (connection!=null)
			{
				connection.SendPacket(packet);
			}

			m_CleanDownloadBlocks();
			m_DownloadSpeeds.Clear();
			m_DownloadSpeed=0;
			m_DownloadState=Protocol.DownloadState.None;

			//try to change to other file to continue downloading if the client has other file we need
			if ((m_OtherDownFiles!=null)
				&& (m_OtherDownFiles.Count>0)
				&& (!cleaningFromsourcesList))
			{
				m_DownloadElement.SourcesList.RemoveSource(this);
			}
		}

		private void m_CleanDownloadBlocks()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("CleanDownloadBlocks",m_ID);
#endif
			CFileBlock block;
			CElement elementOfBlock;
			m_DownloadedBytesAcumul+=m_DownloadedBytes;
			m_DownloadBytesLastSecond=0;	
			m_DownloadedBytes=0;

			while (m_DownloadBlocks.Count>0)
			{
				block=(CFileBlock)m_DownloadBlocks[0];
				m_DownloadBlocks.RemoveAt(0);
				elementOfBlock=CKernel.FilesList[block.FileHash];
				if (elementOfBlock==null) continue;
				if (block.position!=block.start)
				{
					if (!block.compressed)
					{
						if (!elementOfBlock.File.WriteBlock(block.start,block.position-1,block.buffer))
						{
							elementOfBlock.File.RemoveRequestedBlock(block.start,block.end);
							CKernel.FilesList.StopFile(elementOfBlock.File.FileHash);
						}
						Debug.WriteLine(m_UserName+" write parcial block: "+Convert.ToString(block.start));
#if VERBOSE
						CKernel.LogClient.AddLog("CleanDownloadBlocks-write parcial block",m_ID);
#endif
					}
					else
					{
						//byte[] bufferIncompleto=new Byte[block.end-block.posicion-1];
						//Array.Copy(block.buffer,0,bufferIncompleto,0,(int)block.end-(int)block.posicion-1);
						//int uncompressedSize=CRecepcionBloqueComprimido.descomprimirBuffer(ref bufferIncompleto);
						//m_DownloadElement.File.EscribirBloque(block.start,block.start+(uint)uncompressedSize-1,bufferIncompleto);
						//Debug.Write(UserName+" escribe block comprimido parcial: "+Convert.ToString(block.start)+"\n");
						elementOfBlock.File.RemoveRequestedBlock(block.start,block.end);
						//Debug.Write(m_UserName+" libera block: "+Convert.ToString(block.start)+"\n");
					}
				}
				else
				{
					elementOfBlock.File.RemoveRequestedBlock(block.start,block.end);
					//Debug.Write(m_UserName+" libera block: "+Convert.ToString(block.start)+"\n");
				}
				block=null;
			}
		}

		private void m_OnConnetionReady()
		{
#if VERBOSE
			CKernel.LogClient.AddLog("m_OnConnetionReady",m_ID);
#endif
			m_ConexionTries=0;

			if ((m_UploadState==Protocol.UploadState.Connecting)
				|| (m_UploadState==Protocol.UploadState.WaitCallBack))
			{
				m_UploadState=Protocol.UploadState.Uploading;
				StartUpload();
			}

			if (m_DownloadState==Protocol.DownloadState.Connecting)
			{
				m_DownloadState=Protocol.DownloadState.Connected;
				TryDownload();
			}

			if (m_DownloadState==Protocol.DownloadState.WaitCallBack)
			{
				m_DownloadState=Protocol.DownloadState.Connected;
				TryDownload();
			}
			if (m_SharedFilesRequested) RequestSharedList();
			if ((m_ChatMessages!=null)&&(m_ChatMessages.Count>0)) SendChatMessage("");
		}

		private bool m_ShouldCancelUpload()
		{
			TimeSpan diff=DateTime.Now-m_StartUploadTransferTime;

			if ((!m_IsPartner)&&
				((m_UploadedBytes>Protocol.PartSize)
				|| (diff.Duration()>new TimeSpan(0,45,0)))
				&& (CKernel.Queue.Count>0))
			{
				CancelUploadTransfer(true);
				return true;
			}

			if (m_IsPartner)
			{
				if (DateTime.Now-m_PartnerStartedTime>new TimeSpan(0,1,0))
				{
					if (DownloadSpeed/1024F<UploadSpeed/3.0F)
					{
						if ((m_TimesPartnerSlow<=3)&&(m_DownloadState==Protocol.DownloadState.Downloading))
						{
							m_PartnerStartedTime=DateTime.Now; //1 minute more
							m_TimesPartnerSlow++;
						}
						else
						{
							m_BadPartnerTime=DateTime.Now;
							m_TimesPartnerSlow=0;
							CLog.Log(Constants.Log.Verbose,"Removed Bad partner"+ this.UserName+ " Do: "+DownloadSpeed.ToString()+" UP: "+UploadSpeed.ToString());
							CancelUploadTransfer(true);
							return true;
						}
					}
					else
					{
						if ((DownloadSpeed/1024F>UploadSpeed)&&(m_TimesPartnerSlow>0)) 
							m_TimesPartnerSlow--; //give more credit
						m_PartnerStartedTime=DateTime.Now+new TimeSpan(0,2,0); //2 minutes more
					}
				}
			}
			return false;
		}

		private bool m_CreateDataPackets()
		{
			CFileBlock sendBlock;	
			sendBlock=(CFileBlock)m_UploadBlocks[0];
			byte[] Datos=null;
			CElement element=(CElement)CKernel.FilesList[sendBlock.FileHash];

			m_UploadElement=element;

			if (CKernel.SameHash(ref m_UpFileHash,ref sendBlock.FileHash))
			{
				Buffer.BlockCopy(sendBlock.FileHash,0,m_UpFileHash,0,16);
			}

			if ((element!=null)&&(sendBlock.end-sendBlock.start<185000))
			{
				Datos=element.File.ReadBlock(sendBlock.start,sendBlock.end-1,false);
			}

			if ((Datos==null)||(element==null))
			{
				// cancel upload
				Debug.Write("Part not found in file\n");
				CancelUploadTransfer(false);
				return false;
			}

			MemoryStream DatosStream=new MemoryStream(Datos);

			//if you uncoment this upload compression will be enabled (not so tested)
			//if (bEsMula)
			//{
			//	CCompressedBlockSend EnvioBloque=new CCompressedBlockSend(Datos,sendBlock.inicio,sendBlock.fin,sendBlock.FileHash,ref m_UploadDataPackets);
			//}
			//else
			//{
			CSendBlock EnvioBloque=new CSendBlock(DatosStream,sendBlock.start,sendBlock.end,sendBlock.FileHash,ref m_UploadDataPackets);
			//}
			Datos=null;
			m_UploadBlocks.Remove(sendBlock);
			//GC.Collect();
			return true;
		}
		#endregion
	}
}