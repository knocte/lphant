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
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Collections;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// CServer handles the data for eDonkey network servers.
	/// </summary>
	internal class CServer
	{
		#region Instance fields
		/// <summary>Store for the server IP.</summary>
		private uint m_IP;
		/// <summary>Store for the server port.</summary>
		private ushort m_Port;
		/// <summary>Store for the name property.</summary>
		private string m_Name;
		/// <summary>Store for the description.</summary>
		private string m_Description;
		/// <summary>Store for the preference.</summary>
		private Constants.ServerPriority m_Preference;
		/// <summary>Store the last ping time.</summary>
		private uint m_LastPingTime;
		/// <summary>Store for the dynamic IP.</summary>
		private string m_DynamicIP;
		/// <summary>Store for the fails count.</summary>
		private uint m_nFails;
		/// <summary>Store for the maximum users on this server.</summary>
		private uint m_MaxUsers;
		/// <summary>Store for the users which use this server.</summary>
		private uint m_Users;
		/// <summary>Store for the files which are available on the server.</summary>
		private uint m_Files;
		/// <summary>Store for the last searcher.</summary>
		private CSearcher m_LastSearcher;
		/// <summary>Store for the server message.</summary>
		private string m_Message;
		/// <summary>Store for the ping time.</summary>
		private DateTime m_PingTime;
		/// <summary>Store for the random ping value.</summary>
		private uint m_RndPingValue;
		/// <summary>Store for the IPEndPoint.</summary>
		private IPEndPoint m_EPRemote;
		/// <summary>Store for the UDP IPEndPoint.</summary>
		private IPEndPoint m_EPUDPRemote;
		/// <summary>Store for the TCPFlags.</summary>
		private uint m_TCPflags;
		/// <summary>Store for the server connection.</summary>
		private CedonkeyServerConexion m_ServerConnection;
		/// <summary>Store for the UDP server connection.</summary>
		//private CedonkeyServerConexionUDP m_ServerConnectionUDP;
		/// <summary>Store for the last connection time</summary>
		private long m_LastConnection;
		/// <summary>Store for the current connection state.</summary>
		private bool m_Connected;
		/// <summary>Store the ID of the current user.</summary>
		private uint m_UserID;		
		/// <summary>Store the information : Low or High ID.</summary>
		private bool m_IsHighID;
		/// <summary>Flag to know if the server is disconnected by the user</summary>
		private bool m_DisconectedByUser;		
		/// <summary>This is the main server port, alternative port can be used un m_Port
		private ushort m_MainPort;
		/// <sumary>flag to indicate all files has been published.</sumary>
		private bool m_AllFilesPublished;
		/// <summary>Last published time.</summary>
		private DateTime m_LastPublishedTime;
		/// <summary>Last file published Index</summary>
		private uint m_LastFilePublishedIndex;

		#endregion

		#region Propertys
		/// <summary>
		/// Specify the Name of the server.
		/// </summary>
		public string Name
		{
			get
			{
				if (m_Name!="")
					return m_Name;
				else 
					return new IPAddress(IP).ToString();
			}
		}

		/// <summary>
		/// Specify the server IP.
		/// </summary>
		public uint IP
		{
			get
			{
				return m_IP;
			}
		}

		/// <summary>
		/// Specify the current used server port.
		/// </summary>
		public ushort Port
		{
			get
			{
				return m_Port;
			}
		}

		/// <summary>
		/// Specify the server main port.
		/// </summary>
		public ushort MainPort
		{
			get
			{
				if (m_MainPort>0)
					return m_MainPort;
				else
					return m_Port;
			}
			set
			{
				m_MainPort=value;
			}
		}
		/// <summary>
		/// TCPFlags
		/// </summary>
		public uint TCPFlags
		{
			get
			{
				return m_TCPflags;
			}
			set
			{
				m_TCPflags=value;
			}
		}

		/// <summary>
		/// Specify the RemoteEndPoint
		/// </summary>
		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return m_EPRemote;
			}
		}

		/// <summary>
		/// Specify how often the connection to server fails.
		/// </summary>
		public uint Fails
		{
			get
			{
				return m_nFails;
			}
		}

		/// <summary>
		/// Specify the server description.
		/// </summary>
		public string Description
		{
			get
			{
				return m_Description;
			}
		}

		/// <summary>
		/// Specify the users which use the server.
		/// </summary>
		public uint Users
		{
			get
			{
				return m_Users;
			}
		}

		/// <summary>
		/// Specify the files which are available on the server.
		/// </summary>
		public uint Files
		{
			get
			{
				return m_Files;
			}
		}

		/// <summary>
		/// server support compression.
		/// </summary>
		public bool SupportCompression
		{
			get
			{
				return (m_TCPflags & (uint)Protocol.ServerFlagTCP.Compression)>0;
			}
		}

		/// <summary>
		/// Specify the server message.
		/// </summary>
		public string Message
		{
			get
			{
				return m_Message;
			}
		}

		/// <summary>
		/// Specify the last connection time.
		/// </summary>
		public long LastConnection
		{
			get
			{
				return m_LastConnection;
			}
			set
			{
				m_LastConnection=value;
			}
		}

		/// <summary>
		/// Specify the connection state.
		/// </summary>
		public bool Connected
		{
			get
			{
				return m_Connected;
			}
			set
			{
				m_Connected=value;
			}
		}

		/// <summary>
		/// Specify the priority.
		/// </summary>
		public Constants.ServerPriority Priority
		{
			get
			{
				return m_Preference;
			}
			set
			{
				CKernel.ServersList.Sort();
				m_Preference=value;
			}
		}

		/// <summary>
		/// Specify UserID.
		/// </summary>
		public uint UserID
		{
			get
			{
				return m_UserID;
			}
			set
			{
				m_UserID=value;
			}
		}
		
		/// <summary>
		/// Specify the ID state.
		/// </summary>
		public bool IsHighID
		{
			get
			{
				return m_IsHighID;
			}
			set
			{
				m_IsHighID=value;
			}
		}

		/// <summary>
		/// Flag to know if the server is disconnected by the user.
		/// </summary>
		public bool DisconectedByUser
		{
			get
			{
				return m_DisconectedByUser;
			}
			set
			{
				m_DisconectedByUser=value;
			}
		}

		/// <summary>
		/// flag to indicate all files has been published.
		/// </summary>
		public bool AllFilesPublished
		{
			get
			{
				return m_AllFilesPublished;
			}
			set
			{
				if (!value) m_LastFilePublishedIndex=0;
				m_AllFilesPublished=value;
			}
		}
		
		/// <summary>
		/// Last published time.
		/// </summary>
		public DateTime LastPublishedTime
		{
			get
			{
				return m_LastPublishedTime;
			}
		}
		
		
		#endregion

		#region Constructors & Initialization
		/// <summary>
		/// Create a new server.
		/// </summary>
		/// <param name="strIP">IP of the server.</param>
		/// <param name="in_Port">Port of the server.</param>
		public CServer(uint in_IP, ushort in_Port)
		{
			IPAddress DireccionIP=new IPAddress(in_IP);
			m_Name="";
			Initialize(in_IP, in_Port);
		}

		/// <summary>
		/// Create a new server.
		/// </summary>
		/// <param name="strIP">IP of the server.</param>
		/// <param name="in_Port">Port of the server.</param>
		public CServer(string strIP, ushort in_Port)
		{
			IPAddress DirectionIP=IPAddress.Parse(strIP);
			m_Name=strIP;
			Initialize(BitConverter.ToUInt32(DirectionIP.GetAddressBytes(),0), in_Port);
		}

		/// <summary>
		/// Initialize the server with default values.
		/// </summary>
		/// <param name="in_IP">IP of the server.</param>
		/// <param name="in_Port">Port of the server.</param>
		private void Initialize(uint in_IP, ushort in_Port)
		{
			m_IP=in_IP;
			m_Port=in_Port;
			m_Description="";
			m_DynamicIP="";
			m_Files=0;
			m_Users=0;
			m_MaxUsers=0;
			m_LastPingTime=0;
			m_nFails=0;
			m_MainPort=0;
			m_Connected=false;
			m_EPRemote=new IPEndPoint(m_IP, m_Port);
			m_EPUDPRemote=new IPEndPoint(m_IP,m_Port+4);
			m_DisconectedByUser=false;
			m_AllFilesPublished=false;
			m_LastPublishedTime=DateTime.MinValue;
			m_LastFilePublishedIndex=0;
			//m_ServerConnection=new CedonkeyServerConexion(this);
			//m_ServerConnectionUDP=new CedonkeyServerConexionUDP(this);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Increase connection fails by reason.
		/// </summary>
		/// <param name="reason">Reason why connection fails.</param>
		/// <returns>New fails count.</returns>
		public uint IncFails(byte reason)
		{
			if (CKernel.ServersList.ConnectionFailed(this, reason))
			{
				string strreason="";
				switch((Protocol.ConnectionReason)reason)
				{
					case Protocol.ConnectionReason.CannotConnect:
						strreason=CKernel.Globalization["SRV_DOWN"];
						break;
					case Protocol.ConnectionReason.ClosedConnection:				
						strreason=CKernel.Globalization["SRV_DISCONNECTED"];
						break;
					case Protocol.ConnectionReason.NullPacket:				
						strreason=CKernel.Globalization["SRV_FULL"];
						break;
					case Protocol.ConnectionReason.NullID:				
						strreason=CKernel.Globalization["SRV_FULL"]; //strreason="ID 0";
						break;
					case Protocol.ConnectionReason.InvalidHeader:
						strreason=CKernel.Globalization["SRV_FULL"]; //strreason="Invalid header";
						break;
				}
				CLog.Log(Constants.Log.Info,"SRV_NOTCONNECT", this.Name, strreason);
				if (reason==(byte)Protocol.ConnectionReason.CannotConnect)
					m_nFails++; 
			}
			m_ServerConnection=null;
			m_AllFilesPublished=false;
			m_LastPublishedTime=DateTime.MinValue;
			m_LastFilePublishedIndex=0;
			if (m_LastSearcher!=null) m_LastSearcher.OnTCPSearchEnded();
			return m_nFails;
		}

		/// <summary>
		/// Reset the fails count of the server.
		/// </summary>
		public void ResetFails()
		{
			m_nFails=0;
		}

		/// <summary>
		/// Set the server message.
		/// </summary>
		/// <param name="buffer">Incomming buffer with the message.</param>
		public void SetMessage(byte[] buffer) 
		{
			ushort length;
			BinaryReader reader=new BinaryReader(new MemoryStream(buffer));
			length=reader.ReadUInt16();
			m_Message=new string(reader.ReadChars(length));
			if(m_UserID!=0) CLog.Log(Constants.Log.Info,m_Message);
		}
		#endregion

		#region Connection Methods
		/// <summary>
		/// Connect to the server.
		/// </summary>
		public void Connect()
		{
			CLog.Log(Constants.Log.Info,"SRV_CONNECTING",this.Name);
			if (m_ServerConnection!=null) m_ServerConnection.CloseConnection();
			m_ServerConnection=new CedonkeyServerConexion(this);
			m_ServerConnection.Connect();
		}

		/// <summary>
		/// Send the hello message to the server.
		/// </summary>
		public void SendHello()
		{
			if (m_ServerConnection==null) return;
			MemoryStream buffer=new MemoryStream();
			CServerHello DonkeyServidorEnvioHola=new CServerHello(buffer);
			m_ServerConnection.SendPacket(buffer);
		}

		/// <summary>
		/// Check the connection state of the server
		/// </summary>
		/// <returns>ConnectionReady returns true if the connection is established.</returns>
		public bool ConnectionReady()
		{
			m_DisconectedByUser=false;  //reset the flag
			if (CKernel.ServersList.ActiveServer!=null) 
			{
				if (m_ServerConnection!=null)
				{	
					m_UserID=0;
					m_ServerConnection.CloseConnection();
				}
				return false;
			}
			else 
			{
				CLog.Log(Constants.Log.Info, "SRV_CONNECTED", this.Name);
				CKernel.ServersList.ActiveServer=this;
				m_LastFilePublishedIndex=0;
				PublishSharedFiles();
			
				MemoryStream buffer=new MemoryStream();
				CServerServerList ListaServidores=new CServerServerList(buffer);
				if (m_ServerConnection!=null) m_ServerConnection.SendPacket(buffer);
				
				CKernel.ServersList.NextTCPSourcesSearch=DateTime.Now+new TimeSpan(0,0,5);
				return true;
			}
		}

		/// <summary>
		/// Close connection to the server.
		/// </summary>
		public void Disconnect()
		{
			if (m_ServerConnection!=null)
			{
				m_UserID=0;
				m_DisconectedByUser=true;
				m_ServerConnection.CloseConnection();
			}
		}
		#endregion
	
		#region Incomming & Outgoing Packet Methods
		/// <summary>
		/// Read the server state from incomming packet.
		/// </summary>
		/// <param name="packet">Incomming packet.</param>
		public void UpdateState(byte[] packet)
		{
			if (packet.Length!=8) return;
			CServerStatus DonkeyServidorEstado=new CServerStatus(new MemoryStream(packet));
			m_Users=DonkeyServidorEstado.Users;
			m_Files=DonkeyServidorEstado.Files;
//			Debug.Write(m_Name+" Users:"+Convert.ToString(m_Users)+" Files:"+Convert.ToString(m_Files)+"\n");
		}

		/// <summary>
		/// Read the server description from incomming packet.
		/// </summary>
		/// <param name="packet">Incomming packet.</param>
		public void ProcessDescription(byte[] packet)
		{
			if (packet.Length<38) return;
			CServerDescription DonkeyServidorDescripcion=new CServerDescription(new MemoryStream(packet));
			m_Name=DonkeyServidorDescripcion.Name;
			m_Description=DonkeyServidorDescripcion.Description;
		}

		/// <summary>
		/// Read a list of servers from an incomming packet.
		/// </summary>
		/// <param name="packet">Incomming packet.</param>
		public void ProcessServerList(byte[] packet)
		{
			if(!CKernel.Preferences.GetBool("UpdateServerListFromServer")) return;
			CServerServerList DonkeyServidorListaServidores=new CServerServerList(new MemoryStream(packet),CKernel.ServersList);
			CLog.Log(Constants.Log.Info,"SRV_RECEIVED",DonkeyServidorListaServidores.NewServers);
		}

		/// <summary>
		/// Request a CallBack.
		/// </summary>
		/// <param name="ID">ID</param>
		public void RequestCallBack(uint ID)
		{
			if (Connected)
			{	
				MemoryStream buffer=new MemoryStream();
				CServerRequestCallback DonkeyServidorPedirCallback=new CServerRequestCallback(ID,buffer);
				if (m_ServerConnection!=null) m_ServerConnection.SendPacket(buffer);
			}
		}

		/// <summary>
		/// Process an incomming CallBack Response.
		/// </summary>
		/// <param name="packet">Incomming packet.</param>
		public void ProcessCallBackResponse(byte[] packet)
		{
			MemoryStream buffer=new MemoryStream(packet);
			CServerCallbackResponse CallbackResponse=new CServerCallbackResponse(buffer);
			buffer.Close();
			CClient Client;
			Client=CKernel.ClientsList.AddClient(CallbackResponse.IP,CallbackResponse.Port,0,null,0,null);
			//si el cliente ya existía tendremos su referencia
			if (Client!=null) Client.Connect();
		}

		/// <summary>
		/// Send a list of shared files to the server.
		/// </summary>
		public void PublishSharedFiles()
		{
			m_LastPublishedTime=DateTime.Now;
			MemoryStream buffer=new MemoryStream();
			CServerPublishFiles ServerPublishFiles=new CServerPublishFiles(buffer, CKernel.FilesList, m_LastFilePublishedIndex);
			m_LastFilePublishedIndex=ServerPublishFiles.LastFilePublished+1;
			if (m_LastFilePublishedIndex>=CKernel.FilesList.Count-1) m_AllFilesPublished=true;
			//if server supports compression we send it compressed
			if (SupportCompression)
			{
				CCompressedPacket CompressedPacket=new CCompressedPacket(ref buffer);
			}
			if (m_ServerConnection!=null) m_ServerConnection.SendPacket(buffer);
		}

		/// <summary>
		/// Publish one new shared file (only one)
		/// </summary>
		public void PublishSharedFile(CElement Element)
		{
			MemoryStream buffer=new MemoryStream();
			CServerPublishFiles ServerPublishFiles=new CServerPublishFiles(buffer, Element);
			//if server supports compression we send it compressed
			if (SupportCompression)
			{
				CCompressedPacket CompressedPacket=new CCompressedPacket(ref buffer);
			}
			if (m_ServerConnection!=null) m_ServerConnection.SendPacket(buffer);
		}

		/// <summary>
		/// Request file sources from server.
		/// </summary>
		/// <param name="FileHash">Hash of the file.</param>
		public void RequestSources(byte[] FileHash)
		{
			MemoryStream buffer=new MemoryStream();
			CServerAskSources RequestSources=new CServerAskSources(FileHash,buffer);
			if (m_ServerConnection!=null) m_ServerConnection.SendPacket(buffer);
		}
		
		/// <summary>
		/// Request file several sources in one TCP frame from server.
		/// </summary>
		/// <param name="FileHashes">Array with the file hashes.</param>
		public void RequestSources(ArrayList FileHashes)
		{
			MemoryStream buffer=new MemoryStream();
			CServerAskSources RequestSources;
			foreach (byte[] FileHash in FileHashes)
			{
				RequestSources=new CServerAskSources(FileHash,buffer);
				Debug.WriteLine("Asking file "+CKernel.HashToString(FileHash));
			}
			if (m_ServerConnection!=null) m_ServerConnection.SendPacket(buffer);
		}

		/// <summary>
		/// Request file sources via UDP from server.
		/// </summary>
		/// <param name="FileHash">Hash of the file.</param>
		public void RequestSourcesUDP(byte[] FileHash)
		{
			MemoryStream buffer=new MemoryStream();
			CServerAskSourcesUDP PedirFuentesUDP=new CServerAskSourcesUDP(FileHash,buffer);
			m_SendPacketUDP(buffer);
		}

		/// <summary>
		/// Request file sources via UDP from server.
		/// </summary>
		/// <param name="hashes">Array of file hashes.</param>
		public void RequestSourcesUDP(ArrayList hashes)
		{
			MemoryStream buffer=new MemoryStream();
			CServerAskSourcesUDP PedirFuentesUDP=new CServerAskSourcesUDP(hashes,buffer);
			m_SendPacketUDP(buffer);
		}

		/// <summary>
		/// Request a CallBack via UDP.
		/// </summary>
		/// <param name="ID">ID</param>
		public void RequestCallBackUDP(uint ID)
		{
			MemoryStream buffer=new MemoryStream();
			CServerRequestCallBackUDP PedirCallBackUDP=new CServerRequestCallBackUDP(ID,CKernel.Preferences.GetUInt("ID"),CKernel.Preferences.GetUShort("TCPPort"),buffer);
			m_SendPacketUDP(buffer);
		}

		/// <summary>
		/// Read a list of sources from incomming packet.
		/// </summary>
		/// <param name="packet">Incomming packet.</param>
		public void ProcessSources(MemoryStream packet)
		{
			CServerProcessSources ProcessSources=new CServerProcessSources(packet, this.m_IP, this.m_Port);
		}

		/// <summary>
		/// Send a search packet via TCP to server.
		/// </summary>
		/// <param name="packet">packet</param>
		/// <param name="searcher">searcher</param>
		public void SendTCPSearch(MemoryStream packet, CSearcher searcher)		
		{
			m_LastSearcher=searcher;
			if (m_ServerConnection!=null) m_ServerConnection.SendPacket(packet);
		}

		/// <summary>
		/// Send a search packet via UDP to server.
		/// </summary>
		/// <param name="packet">packet</param>
		/// <param name="searcher">searcher</param>
		public void SendUDPSearch(MemoryStream packet, CSearcher searcher)				
		{
			m_LastSearcher=searcher;
			m_SendPacketUDP(packet);
		}

		/// <summary>
		/// Request ping time via UDP from server.
		/// </summary>
		public void RequestUDPPing()
		{
			MemoryStream packet=new MemoryStream();
			CServerRequestPingUDP PeticionPingUDP=new CServerRequestPingUDP(packet);
			m_PingTime=DateTime.Now;
			m_nFails++;
			m_RndPingValue=PeticionPingUDP.rndvalping;
			m_SendPacketUDP(packet);
		}
		
		/// <summary>
		/// Request description via UDP from server.
		/// </summary>
		public void RequestUDPDescription()
		{
			MemoryStream packet=new MemoryStream();
			CServerRequestStatusUDP PeticionEstadoUDP=new CServerRequestStatusUDP(packet);
			m_SendPacketUDP(packet);
		}

		/// <summary>
		/// Process the server ping reply from incomming UDP buffer.
		/// </summary>
		/// <param name="buffer">Incomming buffer.</param>
		public void ProcessPingUDP(MemoryStream buffer)
		{
			CServerPingResponseUDP RespuestaPingUDP=new CServerPingResponseUDP(buffer);
			if (m_RndPingValue==RespuestaPingUDP.rndretval)
			{
				TimeSpan tdif=DateTime.Now-m_PingTime;
				m_LastPingTime=(uint)tdif.TotalMilliseconds;
			}
			m_nFails=0;
			m_Users=RespuestaPingUDP.nUsers;
			m_Files=RespuestaPingUDP.nfiles;
			m_MaxUsers=RespuestaPingUDP.nMaxUsers;
			RequestUDPDescription();
		}

		/// <summary>
		/// Process the server description from incomming UDP buffer.
		/// </summary>
		/// <param name="buffer">Incomming buffer.</param>
		public void ProcessDescriptionUDP(MemoryStream buffer)
		{
			m_nFails=0;
			CServerResponseStatusUDP ResponseStatusUDP=new CServerResponseStatusUDP(buffer);
			m_Name=ResponseStatusUDP.name;
			m_Description=ResponseStatusUDP.description;
		}

		/// <summary>
		/// Process a list of search results from incomming packet.
		/// </summary>
		/// <param name="packet">Incomming packet.</param>
		public void ProcessSearchResults(byte[] packet)
		{
			CServerSearchResults ServerSearchResults=new CServerSearchResults(new MemoryStream(packet),m_LastSearcher,false);
		}


		public void QueryMoreResults()
		{
			MemoryStream buffer=new MemoryStream();
			CServerQueryMoreResults ServerQueryMoreResults=new CServerQueryMoreResults(buffer);
			if (m_ServerConnection!=null) m_ServerConnection.SendPacket(buffer);
		}
		/// <summary>
		/// Process a list of search results from incomming UDP packet.
		/// </summary>
		/// <param name="packet">Incomming packet.</param>
		public void ProcessSearchResultsUDP(MemoryStream packet)
		{
			m_nFails=0;
			CServerSearchResults ServerSearchResults=new CServerSearchResults(packet,m_LastSearcher,true);
		}

		private void m_SendPacketUDP(MemoryStream buffer)
		{
			if (m_EPRemote.Port<=IPEndPoint.MinPort) return;
			if (m_EPRemote.Port>=IPEndPoint.MaxPort) return;
			if (BitConverter.ToUInt32(m_EPRemote.Address.GetAddressBytes(),0)<=Protocol.LowIDLimit) return;
			
			
			//Debug.WriteLine("Asking to "+m_EPUDPRemote.ToString());
			
			buffer.Capacity=(int)buffer.Length;
			CKernel.UDPListener.SendPacketUDP(buffer.GetBuffer(),m_EPUDPRemote);
		}
		#endregion

		#region Load & Save Methods
		/// <summary>
		/// Save the server to an BinaryWriter.
		/// </summary>
		/// <param name="writer">Specify the BinaryWriter to save the server data.</param>
		public void Save(BinaryWriter writer)
		{
			uint parameters=7;
			if (m_Name.Length>0) parameters++;
			if (m_DynamicIP.Length>0) parameters++;
			if (m_Description.Length>0) parameters++;

			writer.Write(parameters);

			if (m_Name.Length>0)
				new ParameterWriter((byte)Protocol.ServerTag.Name, m_Name, writer);

			if (m_DynamicIP.Length>0)
				new ParameterWriter((byte)Protocol.ServerTag.DynIP, m_DynamicIP, writer);

			if (m_Description.Length>0)
				new ParameterWriter((byte)Protocol.ServerTag.Description, m_Description, writer);

			new ParameterWriter((byte)Protocol.ServerTag.Fails, m_nFails, writer);
			new ParameterWriter((byte)Protocol.ServerTag.Preference, (uint)m_Preference, writer);
			new ParameterWriter("users", m_Users, writer);
			new ParameterWriter("files", m_Files, writer);
			new ParameterWriter((byte)Protocol.ServerTag.Ping, m_LastPingTime, writer);
			new ParameterWriter((byte)Protocol.ServerTag.LastPing, m_LastPingTime, writer);
			new ParameterWriter((byte)Protocol.ServerTag.MaxUsers, m_MaxUsers, writer);
		}

		/// <summary>
		/// Reads the server data from an BinaryReader.
		/// </summary>
		/// <param name="reader">Specify the BinaryReader to load the server data.</param>
		public void Load(BinaryReader reader)
		{
			uint parameters;
			CParameterReader parameterReader; 
			parameters=reader.ReadUInt32();
			for (uint i=0;i!=parameters;i++)
			{
				parameterReader=new CParameterReader(reader);
				switch((Protocol.ServerTag)parameterReader.id)
				{
					case Protocol.ServerTag.Name:
						m_Name=parameterReader.valorString;
						break;
					case Protocol.ServerTag.Description:
						m_Description=parameterReader.valorString;
						break;
					case Protocol.ServerTag.Preference:
						m_Preference=(Constants.ServerPriority)parameterReader.valorNum;
						break;
					case Protocol.ServerTag.Ping:
						m_LastPingTime=parameterReader.valorNum;
						break;
					case Protocol.ServerTag.DynIP:
						m_DynamicIP=parameterReader.valorString;
						break;
					case Protocol.ServerTag.Fails:
						m_nFails=parameterReader.valorNum;
						break;
					case Protocol.ServerTag.MaxUsers:
						m_MaxUsers=parameterReader.valorNum;
						break;
					default:
						if (parameterReader.nombreParam==null) break;
						if (parameterReader.nombreParam.Equals("users"))
							m_Users=parameterReader.valorNum;
						else if (parameterReader.nombreParam.Equals("files"))
							m_Files=parameterReader.valorNum;
						break;
				}
			}
		}
		#endregion
	}
}