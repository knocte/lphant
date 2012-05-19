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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using eLePhant.Types;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// CConnection is the base class for eDonkey network connections.
	/// </summary>
	internal class CConnection
	{
		#region Instance fields
		/// <summary>Store for the socket</summary>
		protected Socket m_socket;
		/// <summary>Store for the IPEndPoint</summary>
		protected IPEndPoint m_EPremote;
		/// <summary>Store for the timeout</summary>
//		protected Timer m_TimerTimeout;
		/// <summary>Store for the donkey header</summary>
		protected DonkeyHeader m_DonkeyHeader;

		/// <summary>Specify the internal buffer size</summary>
		private const int m_BuffSize=3000;

		/// <summary>Store for the assigned client</summary>
		private CClient m_Client;
		/// <summary>Store for the send packet</summary>
		private byte[] m_SendPacket;
		private int m_SendBytesPending;
		private int m_TotalSent;
		/// <summary>Store for the download limit</summary>
		private int m_DownLimit;
		/// <summary>Store for the outgoing packets</summary>
		private ArrayList m_SendPacketList;
		/// <summary>Store for the incomming reception stream</summary>
		private MemoryStream m_ReceptionStream;
		private bool m_HeaderPending;
		/// <summary>Store for the current stream position</summary>
		private long m_StreamPosition;
		/// <summary>Store for the current received packet</summary>
		private byte[] m_ReceptionPacket;
		private int m_Acumul;
		private int m_Limit;
		private byte m_ntimeOuts;
		private DateTime m_LastPacket;
		#endregion

		private class stSendingPacket
		{
			public byte[] Packet;
			public bool Freed;
		}

		#region Propertys
		
		public DateTime LastPacket
		{
			get
			{
				return m_LastPacket;
			}
		}
		/// <summary>
		/// IP of the client.
		/// </summary>
		public uint IP
		{
			get
			{
				return BitConverter.ToUInt32(m_EPremote.Address.GetAddressBytes(),0);
			}
		}

		/// <summary>
		/// Set the download limit of the connection.
		/// </summary>
		public int DownloadLimit
		{
			get
			{
				return m_DownLimit;
			}
			set
			{
				this.m_DownLimit=value;
			}
		}

		/// <summary>
		/// Returns true if client sending data.
		/// </summary>
		public bool Sending
		{
			get
			{
				return ( (m_socket.Connected)&&
						 ( (m_SendPacket!=null)||
						   (
							(m_SendPacketList.Count>0)&&(((stSendingPacket)m_SendPacketList[0]).Freed)
						   )	
						 )
					   );
			}
		}

		/// <summary>
		/// Returns true if connection established.
		/// </summary>
		public bool Connected
		{
			get
			{
				if (m_socket!=null)
					return m_socket.Connected;
				else
					return false;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Create a new connection.
		/// </summary>
		/// <param name="socket">socket</param>
		public CConnection(Socket socket)
		{
			m_SendPacketList=ArrayList.Synchronized(new ArrayList());
			m_socket=socket;
			m_EPremote=(IPEndPoint)socket.RemoteEndPoint;
			CKernel.ConnectionsList.Add(this);
			m_ReceptionStream=  new MemoryStream();
			m_HeaderPending=true;
			m_StreamPosition=0;
			m_ntimeOuts=0;
			m_ResetTimeOut();
			m_DownLimit=(int)(CKernel.Preferences.GetFloat("MaxDownloadRate")*1024F*0.20F);
		//	m_TimerTimeout=new Timer(new TimerCallback(OnTimeOut),null,Protocol.ConnectionTimeOut,Timeout.Infinite);
	
			//Debug.WriteLine(m_socket.GetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendBuffer));
			//m_socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendBuffer,1500);

			ReceivePacket();
		}

		/// <summary>
		/// Create a new connection.
		/// </summary>
		/// <param name="strIP">IP</param>
		/// <param name="Port">Port</param>
		/// <param name="in_client">Client</param>
		internal CConnection(string strIP,int Port,CClient in_client)
		{

			IPAddress DirectionIP=IPAddress.Parse(strIP);
			CreateConnection(BitConverter.ToUInt32(DirectionIP.GetAddressBytes(),0),Port,in_client);
		}

		/// <summary>
		/// Create a new connection.
		/// </summary>
		/// <param name="nIP">IP</param>
		/// <param name="Port">Port</param>
		/// <param name="in_client">Client</param>
		internal CConnection(uint nIP,int Port,CClient in_client)
		{
			CreateConnection(nIP,Port,in_client);
		}

		/// <summary>
		/// Create a new connection.
		/// </summary>
		/// <param name="nIP">IP</param>
		/// <param name="Port">Port</param>
		/// <param name="in_client">Client</param>
		private void CreateConnection(uint nIP,int Port,CClient in_client)
		{
			m_ReceptionStream=new MemoryStream();
			m_StreamPosition=0;
			m_ntimeOuts=0;
			m_HeaderPending=true;
			m_DownLimit=(int)(CKernel.Preferences.GetFloat("MaxDownloadRate")*1024F*0.20F);
			m_SendPacketList=new ArrayList();
//			m_TimerTimeout=new Timer(new TimerCallback(OnTimeOut),null,Timeout.Infinite,Timeout.Infinite);

			m_socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			m_socket.Blocking=false;
			//m_socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendBuffer,1500);

			IPAddress DireccionIP=new IPAddress(nIP);
			m_EPremote=new IPEndPoint(DireccionIP,Port);
			if (in_client!=null)
			{
				m_Client=in_client;
			}
		}

		/// <summary>
		/// Destroy connection.
		/// </summary>
		~CConnection()
		{
			if ((m_socket!=null)&&(m_socket.Connected))
			{
				m_socket.Shutdown(SocketShutdown.Both);
				if (m_socket!=null) m_socket.Close();
			}
//			if (m_TimerTimeout!=null)
//			{
//				m_TimerTimeout.Dispose();
//				m_TimerTimeout=null;
//			}
		}
		#endregion

		/// <summary>
		/// Reset the time out.
		/// </summary>
		private void m_ResetTimeOut()
		{
//			if (m_TimerTimeout!=null)
//				m_TimerTimeout.Change(Protocol.ConnectionTimeOut,Timeout.Infinite);
			m_LastPacket=DateTime.Now;
		}

		/// <summary>
		/// ReOpen the connection.
		/// </summary>
		public void ReOpenConnection()
		{
			m_socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		}

		/// <summary>
		/// Connect to client.
		/// </summary>
		public void Connect()
		{
			try
			{
				if (!m_socket.Connected)
					m_socket.BeginConnect(m_EPremote,new AsyncCallback(OnConnected),null);
			}
			catch
			{
				OnConnectionFail((byte)Protocol.ConnectionReason.CannotConnect);
			}          
		}

		/// <summary>
		/// Connection to client established.
		/// </summary>
		/// <param name="ar">State of connection.</param>
		protected virtual void OnConnected(IAsyncResult ar)
		{
			try
			{
				m_socket.EndConnect(ar);
				CKernel.ConnectionsList.Add(this);
				if (m_Client!=null) m_Client.OnConnected();
				m_ResetTimeOut();
				ReceivePacket();
				if (m_SendPacketList.Count>0) 
				{
					stSendingPacket stPaquete=(stSendingPacket)m_SendPacketList[0];
					byte[] paqueteaEnviar=stPaquete.Packet;
					m_SendPacketList.RemoveAt(0);
					SendPacket(paqueteaEnviar,paqueteaEnviar.Length);
				}
			}
			catch
			{
				OnConnectionFail((byte)Protocol.ConnectionReason.CannotConnect);
			}
		}

		/// <summary>
		/// Connection timed out.
		/// </summary>
		/// <param name="state">state</param>
		//protected virtual void OnTimeOut(Object state)
		public virtual void OnTimeOut()
		{
			if ((m_Client!=null)&&(m_Client.DownloadState==Protocol.DownloadState.Downloading)&&(m_ntimeOuts<=Protocol.TransferConnectionTimeOutFactor))
			{
				m_ntimeOuts++;
				m_ResetTimeOut();
//				if (this.m_Client!=null) Debug.Write(m_Client.UserName);
//				Debug.WriteLine(" Time Out");
				return;
			}
#if VERBOSE
			if (m_Client!=null) CKernel.LogClient.AddLog("timeOut",m_Client.UserID);
#endif				
			OnConnectionFail((byte)Protocol.ConnectionReason.TimeOut);
		}

		/// <summary>
		/// Connection failed.
		/// </summary>
		/// <param name="reason">reason</param>
		protected virtual void OnConnectionFail(byte reason)
		{
			if (m_Client!=null) m_Client.OnDisconnect(reason);
			CloseConnection();
		}

		/// <summary>
		/// Close connection.
		/// </summary>
		public void CloseConnection()
		{
			m_ntimeOuts=0;
//			if (m_TimerTimeout!=null)
//			{
//				m_TimerTimeout.Dispose();
//				m_TimerTimeout=null;
//			}
			m_SendPacketList.Clear();
			m_SendPacket=null;
			m_ReceptionPacket=null;
			CloseSocket();

			CKernel.ConnectionsList.Remove(this);
			m_Client=null;
#if VERBOSE
			if (m_Client!=null) CKernel.LogClient.AddLog("closedConnection",m_Client.UserID);
#endif	
		}

		/// <summary>
		/// Close the socket.
		/// </summary>
		public void CloseSocket()
		{
			try
			{
				if ((m_socket!=null)&&(m_socket.Connected))
				{
					m_socket.Shutdown(SocketShutdown.Both);
					if (m_socket!=null) m_socket.Close();
				}
			}
			catch //in very rare cases the socket Shutdown produces SocketException (The socket is not connected)
			{}
		}

		/// <summary>
		/// Receive a packet.
		/// </summary>
		protected virtual void ReceivePacket()
		{
			if ((m_DownLimit>0)&&(m_DownLimit < 2000000)) 
			{
				if (m_DownLimit>CKernel.Preferences.GetFloat("MaxDownloadRate")*1024F*0.2F)
					m_Limit=(int)(CKernel.Preferences.GetFloat("MaxDownloadRate")*1024F*0.2F);
				else
					m_Limit=m_DownLimit;
			}
			else m_Limit=2000000;

			int m_Limitreal=m_Limit-m_Acumul;
			if (m_ReceptionPacket==null) m_ReceptionPacket=new byte[m_BuffSize];

			int minlim=(m_Limitreal>m_BuffSize) ? m_BuffSize : m_Limitreal;
			IAsyncResult ar=null;
			try
			{
				ar=m_socket.BeginReceive(m_ReceptionPacket,0,minlim,SocketFlags.None,new AsyncCallback(OnPacketReceived),minlim);
			}
			catch
			{
				OnConnectionFail((byte)Protocol.ConnectionReason.ClosedConnection); 
			}
		}

		/// <summary>
		/// OnPacketReceived
		/// </summary>
		/// <param name="ar">State of connection.</param>
		private void OnPacketReceived(IAsyncResult ar)
		{
			int receivedBytes=0;
			try
			{
				receivedBytes=m_socket.EndReceive(ar);
				if ((receivedBytes==0)||(m_ReceptionPacket==null)) //m_ReceptionPacket==null is rare, maybe timeout just when receiving
				{
					OnConnectionFail((byte)Protocol.ConnectionReason.NullPacket);
					return;
				}
			}
			catch
			{	
				OnConnectionFail((byte)Protocol.ConnectionReason.ClosedConnection);
				return;
			}
			
			m_ReceptionStream.Write(m_ReceptionPacket,0,receivedBytes);

			m_ResetTimeOut();
			m_Acumul+=receivedBytes;
			if ((receivedBytes==(int)(ar.AsyncState))&&(m_Acumul<m_Limit)&&(m_socket.Poll(100,SelectMode.SelectRead)))
			{
				ReceivePacket();
				return;
			}
			
			while (m_ProcessReceptionStream()) {};

			if (m_Acumul>=m_Limit)
			{
				//Debug.Write(m_Acumul.ToString()+" Delaying\n");
				Thread.Sleep(200);
			}
			m_Acumul=0;
			ReceivePacket();
		}

		private bool m_ProcessReceptionStream()
		{
			if (m_HeaderPending)
			{
				if (m_ReceptionStream.Length-m_StreamPosition>=6) 
				{
					m_ReceptionStream.Seek(m_StreamPosition,SeekOrigin.Begin);
					m_DonkeyHeader=new DonkeyHeader(m_ReceptionStream);
					if ((m_DonkeyHeader.Packetlength>200000)||((m_DonkeyHeader.eDonkeyID!=Protocol.ProtocolType.eDonkey)
						&&(m_DonkeyHeader.eDonkeyID!=Protocol.ProtocolType.eMule)
						&&(m_DonkeyHeader.eDonkeyID!=Protocol.ProtocolType.Packet)))
					{
						OnConnectionFail((byte)Protocol.ConnectionReason.InvalidHeader);
						return false;
					}
					m_HeaderPending=false;
					m_ReceptionStream.Seek(0,SeekOrigin.End);
				}
			}
			if ((!m_HeaderPending)&&
				((m_ReceptionStream.Length-m_StreamPosition)>=m_DonkeyHeader.Packetlength+5))
			{
				m_ReceptionStream.Seek(m_StreamPosition+6,SeekOrigin.Begin);
				byte[] paquete=new byte[m_DonkeyHeader.Packetlength-1];
				m_ReceptionStream.Read(paquete,0,(int)m_DonkeyHeader.Packetlength-1);
				m_HeaderPending=true;
				if (m_DonkeyHeader.eDonkeyID==Protocol.ProtocolType.eDonkey) 
					ProcessDonkeyPacket(ref paquete);
				else if (m_DonkeyHeader.eDonkeyID==Protocol.ProtocolType.eMule) ProcessMulePacket(ref paquete);
				else if (m_DonkeyHeader.eDonkeyID==Protocol.ProtocolType.Packet)
				{
					CReceivedCompressedBlock.Uncompress(ref paquete);
					ProcessMulePacket(ref paquete);
				}
				m_DonkeyHeader=null;
				if (m_ReceptionStream.Position==m_ReceptionStream.Length)
				{
					//m_ReceptionStream=new MemoryStream(buffSize);
					m_ReceptionStream.SetLength(0);
					m_StreamPosition=0;
				}
				else
				{
					if (m_ReceptionStream.Length>10240)
					{
						byte[] tmpbuffer=new byte[m_ReceptionStream.Length-m_ReceptionStream.Position];
						m_ReceptionStream.Read(tmpbuffer,0,tmpbuffer.Length);
						//						m_ReceptionStream.Close();
						//						MemoryStream nuevoStream=new MemoryStream();
						//						nuevoStream.Write(tmpbuffer,0,tmpbuffer.Length);
						//						m_ReceptionStream=nuevoStream;
						m_ReceptionStream.SetLength(0);
						m_ReceptionStream.Write(tmpbuffer,0,tmpbuffer.Length);
						m_StreamPosition=0;
						return true;
					}
					else
					{
						m_StreamPosition=m_ReceptionStream.Position;
						//m_ReceptionStream.Seek(0,SeekOrigin.End);
						return true;
					}
				}
			}
			if (m_ReceptionStream.Position!=m_ReceptionStream.Length)
			{
				m_ReceptionStream.Seek(0,SeekOrigin.End);
			}
			return false;
		}

		public int GetNextFreedPacketSize()
		{
			int i=0;
			int size=0;
			lock(m_SendPacketList.SyncRoot)
			{
				stSendingPacket stPaquete;
				while (i < m_SendPacketList.Count)
				{
					stPaquete=(stSendingPacket)m_SendPacketList[i];
					if (!stPaquete.Freed)
					{
						size=stPaquete.Packet.Length;
						break;
					}
					i++;
				}
			}
			return size;
		}

		public int FreeDataBlock()
		{
			if (m_SendPacketList.Count==0) return 0;

			stSendingPacket stPaquete=(stSendingPacket)m_SendPacketList[0];
			if ((!stPaquete.Freed)&&(m_socket.Connected)&&(m_SendPacket==null))
			{
				stPaquete.Freed=true;
				m_SendPacketList.RemoveAt(0);
				SendPacket(stPaquete.Packet,stPaquete.Packet.Length);
				return stPaquete.Packet.Length;
			}
			else
			{
				int i=0;
				lock(m_SendPacketList.SyncRoot)
				{
					while (i < m_SendPacketList.Count)
					{
						stPaquete=(stSendingPacket)m_SendPacketList[i];
						if (!stPaquete.Freed)
						{
							stPaquete.Freed=true;
							return stPaquete.Packet.Length;
						}
						i++;
					}
				}
			}
			return 0;
		}

		public int SendDataPacket(MemoryStream buffer)
		{
			int end=0;
			int start=0;
			byte[] packet;
			while (end<buffer.Length-1)
			{
				end=start+1300;
				if (end>=buffer.Length) end=(int)buffer.Length-1;
				packet=new Byte[end-start+1];
				Buffer.BlockCopy(buffer.GetBuffer(),start,packet,0,end-start+1);
				start=end+1;
				stSendingPacket stPacket=new stSendingPacket();
				stPacket.Packet=packet;
				stPacket.Freed=false;
				m_SendPacketList.Add(stPacket);
			}
			buffer.Close();
			buffer=null;
			return 0;
		}

		public void SendPacket(MemoryStream buffer)
		{
			if ((m_Client!=null)&&(buffer.GetBuffer()[0]== (byte)Protocol.ProtocolType.eMule)&&(m_Client.EmuleProtocol)&&(buffer.Length>1400))
			{
				CCompressedPacket CompressedPacket=new CCompressedPacket(ref buffer);
			}
			byte[] packet=new byte[buffer.Length];
			Buffer.BlockCopy(buffer.GetBuffer(),0,packet,0,packet.Length);
			stSendingPacket stPacket=new stSendingPacket();
			stPacket.Packet=packet;
			stPacket.Freed=true;
			if ((m_SendPacketList.Count==0)&&(m_socket.Connected)&&(m_SendPacket==null)) 
				SendPacket(packet,(int)buffer.Length);
			else
			{
				m_SendPacketList.Add(stPacket);
			}
			buffer.Close();
			buffer=null;
		}

		private void SendPacket(byte[] packet,int length)
		{
	     
			if ((!m_socket.Connected)||(m_SendPacket!=null))
			{
				//nunca tendría que entrar aquí,tal vez cerrando lphant
				return;
			}
			m_SendPacket=packet;
			m_SendBytesPending=length;
			m_TotalSent=0;

			try
			{
				//Debug.Write("Enviando...");
				m_socket.BeginSend(m_SendPacket,0,m_SendBytesPending,SocketFlags.None,new AsyncCallback(m_OnSentPacket),null);
			}
			catch 
			{
				OnConnectionFail((byte)Protocol.ConnectionReason.ClosedConnection);
			}
		}

		private void m_OnSentPacket(IAsyncResult ar)
		{
			try
			{
				//Debug.WriteLine("->Enviado");

				int sentBytes;
				sentBytes=m_socket.EndSend(ar);
				m_TotalSent+=sentBytes;
				m_SendBytesPending -= sentBytes;
				m_ResetTimeOut();
				if (m_SendBytesPending>0) 
					m_socket.BeginSend(m_SendPacket,sentBytes,m_SendBytesPending,SocketFlags.None,new AsyncCallback(m_OnSentPacket),null);
				else
				{
					//this packet has finalized,look for the next packet in the list
					m_SendPacket=null;
					if ((m_SendPacketList.Count>0)&&(((stSendingPacket)m_SendPacketList[0]).Freed))
					{
						stSendingPacket stPacket=(stSendingPacket)m_SendPacketList[0];
						byte[] packetToSend=stPacket.Packet;
						m_SendPacketList.RemoveAt(0);
						SendPacket(packetToSend,packetToSend.Length);
					}	                
				}
			}
			catch 
			{
				OnConnectionFail((byte)Protocol.ConnectionReason.ClosedConnection);
			}
		}

		/// <summary>
		/// Process incomming eDonkey packet.
		/// </summary>
		/// <param name="packet">Packet which need to be processed.</param>
		protected virtual void ProcessDonkeyPacket(ref byte[] packet)
		{	     
			try
			{	
				switch ((Protocol.ClientCommand)m_DonkeyHeader.Command)
				{
					case Protocol.ClientCommand.Hello:
						if (m_Client==null)
						{
							CReceiveHello HelloPacket=new CReceiveHello(false,new MemoryStream(packet));
							m_Client=CKernel.ClientsList.GetClient(HelloPacket.UserID,HelloPacket.UserPort,HelloPacket.ServerIP,HelloPacket.Hash);
							if (m_Client==null)
							{
								m_Client=new CClient(this);
								CKernel.ClientsList.AddClient(m_Client);
							}
							else
							{
								if ((m_Client.connection!=null)&&(m_Client.connection.Connected)&&(m_Client.connection!=this))
								{
									m_Client.connection.CloseConnection();
									Debug.WriteLine("Client conexion!=null");
								}
								m_Client.connection=this;
							}
						}
						CClient cant=m_Client;
						CConnection conant=m_Client.connection;
						bool NeedExtendRequest=!m_Client.ProcessHello(false,packet);
						if ((NeedExtendRequest)&&(m_Client.EmuleProtocol)&&(CKernel.Preferences.GetBool("UseEmuleProtocol"))) m_Client.SendEmuleHello(false);
						m_Client.ResponseHello();
						break;
					case Protocol.ClientCommand.FileRequest:
						m_Client.ProcessFileRequest(packet);
						break;
					case Protocol.ClientCommand.HelloAnswer:
						m_Client.ProcessHello(true,packet);
						break;
					case Protocol.ClientCommand.SetRequestFileID:
						m_Client.ProcessRequestFileID(packet);
						break; 
					case Protocol.ClientCommand.FileRequestAnswer:
						m_Client.ProcessFileInfo(packet);
						break;
					case Protocol.ClientCommand.FileState:
						m_Client.ProcessChunksStatus(packet,false/*tcp*/);
						break;
					case Protocol.ClientCommand.HashSetAnswer:
						m_Client.ProcessHashSet(packet);
						break;	        
					case Protocol.ClientCommand.HashSetRequest:
						m_Client.SendHashSet();
						break;
					case Protocol.ClientCommand.AcceptUploadRequest:
						if (m_Client.DownloadState==Protocol.DownloadState.OnQueue)
						{
							m_Client.RequestBlocks();
						}
						else
						{
							if (m_Client.DownFileHash!=null)
							{
//								CLog.Log(Constants.Log.Verbose,"Trying to resume download {2}: {0},hashdescarga=null? {1}",m_Client.DownloadState,(m_Client.DownFileHash==null),m_Client.UserName);
								m_Client.RequestBlocks();
							}
							else 
							{
								if (m_Client.UpFileHash!=null)
								{
#if DEBUG
									CLog.Log(Constants.Log.Verbose,"Recovering source to download "+m_Client.UserName);
#endif
									m_Client.DownFileHash=m_Client.UpFileHash;
									m_Client.RequestBlocks();
								}

								else
								{
									CSourceOld sourceRecovered=CKernel.SourcesOld.GetSourceOld(m_Client.UserHash);
									if (sourceRecovered!=null)
									{
										m_Client.DownFileHash=sourceRecovered.FileHash;
										m_Client.DownFileChunks=sourceRecovered.FileChunks;
										m_Client.RequestBlocks();
#if DEBUG
										CLog.Log(Constants.Log.Verbose,"RECOVERED SOURCE from sources cache "+m_Client.UserName);
#endif

									}
#if DEBUG						
									else
										CLog.Log(Constants.Log.Verbose,"download lost: {0}",m_Client.UserName);
#endif
								}
							}
						}
						break;
					case Protocol.ClientCommand.OutOfPartRequest:
						m_Client.ProcessOutOfParts();
						break;
					case Protocol.ClientCommand.StartUploadRequest:
						m_Client.ProcessStartUploadRequest(packet);
						break;
					case Protocol.ClientCommand.RequestParts:
						m_Client.ProcessRequestParts(packet);
						break;
					case Protocol.ClientCommand.EndOfDownload:
					case Protocol.ClientCommand.CancelTransfer:
						m_Client.CancelUploadTransfer(false);
						break;
					case Protocol.ClientCommand.SendingPart:
						m_Client.ReceiveBlock(packet);
						break;
					case Protocol.ClientCommand.QueueRank:
						m_Client.ProcessQR(packet);
						break;
					case Protocol.ClientCommand.FileRequestAnswerNoFile:
						m_Client.FileNotFound(packet);
						break;
					case Protocol.ClientCommand.ChangeClientID:
						m_Client.ProcessIDChange(packet);
						break;
					case Protocol.ClientCommand.ChangeSlot:
						break;
					case Protocol.ClientCommand.AskSharedFiles:
						Constants.AllowViewShared allow=(Constants.AllowViewShared)CKernel.Preferences.GetEnum("AllowViewShared",Constants.AllowViewShared.Nobody);
						if (allow==Constants.AllowViewShared.All)
						{
							m_Client.ProcessSharedListRequest();
						}
						break;
					case Protocol.ClientCommand.AskSharedFilesAnswer:
						m_Client.ProcessSharedListResponse(packet);
						break;
					case Protocol.ClientCommand.Message:
						m_Client.ProcessChatMessage(packet);
						break;
					case Protocol.ClientCommand.CrumbSetResponse:
						m_Client.ProcessCrumbSetResponse(packet);
						break;

					default:
						CLog.Log(Constants.Log.Verbose,"CLI_UNK_DONK_TCP_PACKET",m_DonkeyHeader.Command);
						break;
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
#if DEBUG
				if (m_DonkeyHeader!=null) CLog.Log(Constants.Log.Verbose,"CLI_PAQ_DONK_ERROR",m_DonkeyHeader.Command);
#endif
			}
			packet=null;
			return;
		}

		/// <summary>
		/// Process incomming eMule packet.
		/// </summary>
		/// <param name="packet">Packet which need to be processed.</param>
		protected virtual void ProcessMulePacket(ref byte[] packet)
		{
			try
			{
				switch ((Protocol.ClientCommandExt)m_DonkeyHeader.Command)
				{
					case Protocol.ClientCommandExt.eMuleInfo:
						m_Client.ProcessEmuleHello(packet,false);
						break;
					case Protocol.ClientCommandExt.eMuleInfoAnswer:
						m_Client.ProcessEmuleHello(packet,true);
						break;
					case Protocol.ClientCommandExt.QueueRanking:
						m_Client.ProcessEmuleQR(packet);
						break;
					case Protocol.ClientCommandExt.SourcesResult:
						m_Client.ProcessSources(packet);
						break;
					case Protocol.ClientCommandExt.CompressedPart:
						m_Client.ReceiveCompressedBlock(packet);
						break;
					case Protocol.ClientCommandExt.SourcesRequest:
						m_Client.ProcessSourceRequest(packet);
						break;
					case Protocol.ClientCommandExt.FileDescription:
						m_Client.ProcessComment(packet);
						break;
					default:
						CLog.Log(Constants.Log.Verbose,"CLI_UNK_EMULE_TCP_PACKET",m_DonkeyHeader.Command);
						break;
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
#if DEBUG
				if (m_DonkeyHeader!=null) CLog.Log(Constants.Log.Verbose,"CLI_PAQ_EMULE_ERROR",m_DonkeyHeader.Command);
#endif
			}
			packet=null;
		}
 	}
}