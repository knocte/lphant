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
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for CUDPListener.
	/// </summary>
	public class CUDPListener
	{
		private Socket m_UDPListenerSocket;
		private byte[] m_UDPSendPacket;		
		private byte[] m_UDPReceivePacket;
		private IPEndPoint m_RemoteIPEndPoint;
		private EndPoint m_RemoteEndPoint;
		private bool m_PortIsAccesible;

		public bool PortIsAccesible
		{
			get
			{
				return m_PortIsAccesible;
			}
		}

		public CUDPListener()
		{
		}

		public void Start()
		{
			try
			{
				m_PortIsAccesible=false;
				m_UDPSendPacket=new byte[5000];
				m_UDPReceivePacket=new byte[5000];
				m_UDPListenerSocket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
				IPEndPoint ep=new IPEndPoint(IPAddress.Any,CKernel.Preferences.GetUShort("UDPPort"));
				m_UDPListenerSocket.Bind(ep);
				m_ReceivePacket();
			}
			catch
			{
				CLog.Log(Types.Constants.Log.Verbose,"PORT_INUSE",CKernel.Preferences.GetUShort("UDPPort"));
			}		
		}

		public void Close()
		{
			m_UDPListenerSocket.Shutdown(SocketShutdown.Both);
			m_UDPListenerSocket.Close();
			m_UDPListenerSocket=null;
		}

		private void m_ReceivePacket()
		{
			m_RemoteIPEndPoint=new IPEndPoint(IPAddress.Any, 0);
			m_RemoteEndPoint=(EndPoint)m_RemoteIPEndPoint;
			
			try
			{
				m_UDPListenerSocket.BeginReceiveFrom(m_UDPReceivePacket,0,5000,SocketFlags.None,ref m_RemoteEndPoint,new AsyncCallback(m_OnUDPPacketReceived),null);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
				if (m_UDPReceivePacket==null) m_UDPReceivePacket=new byte[5000];
				if (m_UDPListenerSocket!=null) m_ReceivePacket();
			}
		}

		private void m_OnUDPPacketReceived(IAsyncResult ar)
		{
			int nbytesRecibidos=0;
			try
			{
				nbytesRecibidos=m_UDPListenerSocket.EndReceiveFrom(ar,ref m_RemoteEndPoint);
			}
			catch 
			{}
			if (nbytesRecibidos>0)
			{
				Protocol.ProtocolType protocol=(Protocol.ProtocolType)m_UDPReceivePacket[0];
				if (protocol==Protocol.ProtocolType.eMule)
				{
					byte Comando=m_UDPReceivePacket[1];
					MemoryStream buffer=new MemoryStream(m_UDPReceivePacket,2,nbytesRecibidos-2);
					m_ProcessUDPPacket(Comando, ref buffer);
				}
				else if (protocol==Protocol.ProtocolType.eDonkey)
				{
					byte Comando=m_UDPReceivePacket[1];
					MemoryStream buffer=new MemoryStream(m_UDPReceivePacket,2,nbytesRecibidos-2);
					m_ProcessServerUDPPacket(Comando, ref buffer);
				}
			}
			m_ReceivePacket();
		}

		private void m_ProcessUDPPacket(byte command, ref MemoryStream packet)
		{
			try
			{
				switch( (Protocol.ClientCommandExtUDP)command )
				{
					case Protocol.ClientCommandExtUDP.ReaskFilePing:
						if (!m_PortIsAccesible)
							CLog.Log(Types.Constants.Log.Info,"UDP_PORT_IS_OPEN");
						m_PortIsAccesible=true;
						MemoryStream responseBuffer;
//						if (packet.Length!=16) break;
//						byte[] FilaHash=new byte[16];
//						packet.Read(FilaHash,0,16);
						CFileReaskUDP reaskMessage=new CFileReaskUDP(packet);
						byte[] FilaHash=reaskMessage.FileHash;
						//byte[] chunksAvaibility=reaskMessage.partes;
						//TODO update chunksAvaibility in shared file

						CElement requestedElement=CKernel.FilesList[FilaHash];
						if (requestedElement==null) 
						{
							responseBuffer=new MemoryStream(2);
							CFileNotFoundUDP FicheroNoEncontradoUDP=new CFileNotFoundUDP(responseBuffer);
						}
						else
						{
							m_RemoteIPEndPoint=(IPEndPoint)m_RemoteEndPoint;
							CClient foundClient=null;
							ushort queuePosition=CKernel.Queue.RefreshClientUDP(BitConverter.ToUInt32(m_RemoteIPEndPoint.Address.GetAddressBytes(),0),(ushort)m_RemoteIPEndPoint.Port,ref foundClient);
							if (queuePosition<=0)
							{
								responseBuffer=new MemoryStream(2);
								CQueueFullUDP PaqueteColaLlenaUDP=new CQueueFullUDP(responseBuffer);
							}
							else
							{
								responseBuffer=new MemoryStream(4);
								CQueuePositionUDP PaquetePosicionColaUDP=new CQueuePositionUDP(responseBuffer,queuePosition,requestedElement.File.ChunksStatus,foundClient.VersionUDP);
							}
						}
						SendPacketUDP(responseBuffer.GetBuffer(),m_RemoteIPEndPoint);
						break;
					case Protocol.ClientCommandExtUDP.ReaskAck:
						m_RemoteIPEndPoint=(IPEndPoint)m_RemoteEndPoint;
						CClient client=CKernel.ClientsList.GetClient(BitConverter.ToUInt32(m_RemoteIPEndPoint.Address.GetAddressBytes(),0),0,0,null);
						if (client!=null)
						{
							ushort queuePosition;


							if ((packet.Length>2)&&(client.VersionUDP>3))
							{
								packet.Seek(0,SeekOrigin.Begin);
								client.ProcessChunksStatus(packet,true);
							}
							BinaryReader reader=new BinaryReader(packet);
							queuePosition=reader.ReadUInt16();
							client.ProcessUDPReask(queuePosition);
							packet.Close();
							packet=null;
						}
						break;
					case Protocol.ClientCommandExtUDP.QueueFull:
						client=CKernel.ClientsList.GetClient(BitConverter.ToUInt32(m_RemoteIPEndPoint.Address.GetAddressBytes(),0),0,0,null);
						if (client!=null)
						{
							client.ProcessUDPReask(0);
						}
						break;
					case Protocol.ClientCommandExtUDP.FileNotFound:
						client=CKernel.ClientsList.GetClient(BitConverter.ToUInt32(m_RemoteIPEndPoint.Address.GetAddressBytes(),0),0,0,null);
						if ((client!=null)&&(client.DownloadElement!=null)&&(client.DownloadElement.SourcesList!=null))
							client.DownloadElement.SourcesList.RemoveSource(client);
						break;
					default:
						CLog.Log(Constants.Log.Verbose,"CLI_UNK_UDP_PACKET",command);
						break;
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
				CLog.Log(Constants.Log.Verbose,"CLI_PAQ_UDP_ERROR",command);
			}
		}

		private void m_ProcessServerUDPPacket(byte command, ref MemoryStream packet)
		{
			//bool receiveMore=false;

			try
			{
				CServer m_Server;
				m_RemoteIPEndPoint=(IPEndPoint)m_RemoteEndPoint;
				m_Server=CKernel.ServersList[BitConverter.ToUInt32(m_RemoteIPEndPoint.Address.GetAddressBytes(),0),(ushort)(m_RemoteIPEndPoint.Port-4)];
				switch((Protocol.ServerCommandUDP)command)
				{
					case Protocol.ServerCommandUDP.GlobalFoundSources:
						Debug.WriteLine(DateTime.Now.ToLongTimeString()+" : Found UDP Sources");
						if (m_Server!=null) m_Server.ProcessSources(packet);
						//receiveMore=true;
						break;
					case Protocol.ServerCommandUDP.GlobalSearchResult:
						if (m_Server!=null) m_Server.ProcessSearchResultsUDP(packet);
						//receiveMore=true;
						break;
					case Protocol.ServerCommandUDP.GlobalStateResult:
						if (m_Server!=null) m_Server.ProcessPingUDP(packet);
						//receiveMore=false;
						break;
					case Protocol.ServerCommandUDP.DescriptionResult:
						//CServer m_Server;
						m_Server=CKernel.ServersList[BitConverter.ToUInt32(m_RemoteIPEndPoint.Address.GetAddressBytes(),0),(ushort)m_RemoteIPEndPoint.Port];
						if (m_Server!=null) m_Server.ProcessDescriptionUDP(packet);
						//receiveMore=false;
						break;
				}
			}
			catch
			{
				CLog.Log(Constants.Log.Verbose,"SRV_PAQ_UDP_ERROR",command);
			}
			packet.Close();
			packet=null;
		}

		public void SendPacketUDP(byte[] packet,IPEndPoint remoteHost)
		{
			try
			{
				m_UDPListenerSocket.SendTo(packet,remoteHost);
			}
			catch
			{}
		}
	}
}