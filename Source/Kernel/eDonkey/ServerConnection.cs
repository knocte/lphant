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
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// CedonkeyServerConexion handle the connection to an eDonkey network server.
	/// </summary>
	internal class CedonkeyServerConexion : CConnection
	{
		#region Instance fields
		/// <summary>Store for the connected server.</summary>
		private CServer m_Server;
		#endregion

		#region Constructors
		/// <summary>
		/// Create a new server connection.
		/// </summary>
		/// <param name="in_server">Specify the server where to connect.</param>
		public CedonkeyServerConexion(CServer in_server) : base(in_server.IP, in_server.Port, null)
		{
			m_Server=in_server;
		//	m_TimerTimeout.Change(Timeout.Infinite, Timeout.Infinite);
		}
		#endregion

		#region Overriden Methods
		/// <summary>
		/// Connection to server established.
		/// </summary>
		/// <param name="ar">State of connection.</param>
		protected override void OnConnected(IAsyncResult ar)
		{
			base.OnConnected(ar);
			if (m_socket.Connected)
			{
				m_Server.LastConnection=DateTime.Now.Ticks;
				m_Server.ResetFails();
				m_Server.Connected=true;
				m_Server.SendHello();
			}
		}

		/// <summary>
		/// Connection timed out.
		/// </summary>
		/// <param name="state">state</param>
		//protected override void OnTimeOut(Object state)
		public override void OnTimeOut()
		{
			//Debug.WriteLine("Server timeOUT !!!!!!!!!!!!");
			//m_TimerTimeout.Change(Timeout.Infinite, Timeout.Infinite);
		}

		/// <summary>
		/// Connection to server failed.
		/// </summary>
		/// <param name="reason">Reason</param>
		protected override void OnConnectionFail(byte reason)
		{
			if (m_Server!=null) 
			{
				m_Server.Connected=false;
				if (!m_Server.DisconectedByUser)
				{
					switch( (Protocol.ConnectionReason)reason)
					{
						case Protocol.ConnectionReason.CannotConnect:
							m_Server.IncFails((byte)Protocol.ConnectionReason.CannotConnect);
							break;
						case Protocol.ConnectionReason.ClosedConnection:
							m_Server.IncFails((byte)Protocol.ConnectionReason.ClosedConnection);
							break;
						case Protocol.ConnectionReason.NullPacket:
							m_Server.IncFails((byte)Protocol.ConnectionReason.NullPacket);
							break;
						case Protocol.ConnectionReason.NullID:
							m_Server.IncFails((byte)Protocol.ConnectionReason.NullID);
							break;
						case Protocol.ConnectionReason.InvalidHeader:
							m_Server.IncFails((byte)Protocol.ConnectionReason.InvalidHeader);
							break;
					}
				}
				else
				{
					string strreason="";
					switch( (Protocol.ConnectionReason)reason)
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
							strreason=CKernel.Globalization["SRV_FULL"];
							break;
						case Protocol.ConnectionReason.InvalidHeader:
							strreason=CKernel.Globalization["SRV_FULL"];
							break;
					}
					CLog.Log(Constants.Log.Info,"SRV_NOTCONNECT", m_Server.Name, strreason);
				}
				if (m_Server!=null) m_Server.DisconectedByUser=false; //check !=null again, the server may be deleted
			}
			CloseConnection();
			m_Server=null;
		}

		/// <summary>
		/// Process incomming eDonkey packet.
		/// </summary>
		/// <param name="packet">Packet which need to be processed.</param>
		protected override void ProcessDonkeyPacket(ref byte[] packet)
		{
			if (m_Server == null) return;
			try
			{
				switch ( (Protocol.ServerCommand)m_DonkeyHeader.Command )
				{
					case Protocol.ServerCommand.ServerMessage:
						m_Server.SetMessage(packet);
						break;
					case Protocol.ServerCommand.IDChange:
						uint m_ID;
						string LowHigh_ID="low ID";
						bool IsHighID = false;

						BinaryReader reader=new BinaryReader(new MemoryStream(packet));
						m_ID=reader.ReadUInt32();
						
						if (reader.PeekChar()>0)
						{
							m_Server.TCPFlags=reader.ReadUInt32();
							if (reader.PeekChar()>0) 
								m_Server.MainPort=(ushort)reader.ReadUInt32();
						}
						else
							m_Server.TCPFlags=0;

						if (m_ID>0) 
						{
							if (m_Server.ConnectionReady())
							{
								if(m_ID>Protocol.LowIDLimit) 
								{
									LowHigh_ID="high ID"; 
									IsHighID = true;
									CKernel.ServersList.LowIDRetryHelper.GotHighID();
								}
								else 
								{
									//Check if is advisable to decline the low id and retry
									if (CKernel.ServersList.LowIDRetryHelper.ShouldRetry()) 
									{									    
										CLog.Log(Constants.Log.Info, CKernel.Globalization["LOW_ID_RETRY"], 
											m_Server.Description);										
										CKernel.ServersList.ActiveServer=null;
										CloseConnection();
										//OnConnectionFail((byte)Protocol.ConnectionReason.ClosedConnection);
										break;
									}
								}

								CLog.Log(Constants.Log.Notify,"SRV_NEW_ID",LowHigh_ID,m_ID);
								CKernel.ServersList.ActiveServer.IsHighID = IsHighID;
								CKernel.ServersList.ActiveServer.UserID = m_ID;
								CKernel.Preferences.SetProperty("ID", m_ID);
							}
						}
						else 
						{
							OnConnectionFail((byte)Protocol.ConnectionReason.NullID);
						}
						break;
					case Protocol.ServerCommand.SearchResult:
						m_Server.ProcessSearchResults(packet);
						break;
					case Protocol.ServerCommand.FoundSources:
						Debug.WriteLine(DateTime.Now.ToLongTimeString()+" : Found TCP Sources");
						m_Server.ProcessSources(new MemoryStream(packet));
						break;
					case Protocol.ServerCommand.ServerState:
						m_Server.UpdateState(packet);
						break;
					case Protocol.ServerCommand.ServerIdent:
						m_Server.ProcessDescription(packet);
						break;
					case Protocol.ServerCommand.ServerList:
						m_Server.ProcessServerList(packet);
						break;
					case Protocol.ServerCommand.CallBackRequested:
						m_Server.ProcessCallBackResponse(packet);
						break;
					default:
						break;
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
				CLog.Log(Constants.Log.Verbose, "SRV_PAQ_TCP_ERROR", m_DonkeyHeader.Command);
			}
		}

		/// <summary>
		/// Process incomming eMule packet.
		/// </summary>
		/// <param name="packet">Packet which need to be processed.</param>
		protected override void ProcessMulePacket(ref byte[] packet)
		{
			ProcessDonkeyPacket(ref packet);
		}
		#endregion
	}

}