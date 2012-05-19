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
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Threading;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// CServersList handles a list of eDonkey network servers.
	/// </summary>
	internal class CServersList : CollectionBase
	{
		#region Instance fields
		/// <summary>Store for the current connection index.</summary>
		private int m_ConnectionIndex;
		/// <summary>Store for the failed connection count.</summary>
		private int m_FailedConnetions;
		/// <summary>Store for the last send UDP source time.</summary>
		private DateTime m_LastUDPSourceSearch;
		/// <summary>Store for the last send UDP ping time.</summary>
		private DateTime m_LastPingUDP;
		/// <summary>Store for the file UDP search index.</summary>
		private int m_UDPSourceSearchFileIndex;
		/// <summary>Store for the server search index.</summary>
		private int m_SourceSearchServerIndex;
		/// <summary>Store for the UDP search ping index.</summary>
		private int m_ServerPingUDPIndex;
		/// <summary>Store for the active server.</summary>
		private CServer m_ActiveServer;
		/// <summary>Store for the file TCP search index.</summary>
		private int m_TCPSourceSearchFileIndex;
		/// <summary>Store for the last send TCP source search time.</summary>
		private DateTime m_NextTCPSourcesSearch;
		/// <summary>helper class tro control reconnect on low id.</summary>
		private CLowIDRetryHelper m_lowIDRetryHelper;
		/// <summary>UDP port not open message times displayed. </sumary>
		private int m_TimesshownUDPClosedMessage;
		#endregion

		#region Propertys
		/// <summary>
		/// Specify the active server.
		/// </summary>
		public CServer ActiveServer
		{
			get
			{
				return m_ActiveServer;
			}
			set
			{
				m_ActiveServer=value;
			}
		}

		public DateTime NextTCPSourcesSearch
		{
			set
			{
				m_NextTCPSourcesSearch=value;
			}
		}

		public CLowIDRetryHelper LowIDRetryHelper
		{
			get
			{
				return m_lowIDRetryHelper;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new ServerList.
		/// </summary>
		public CServersList()
		{
			m_ConnectionIndex=0;
			m_ActiveServer=null;
			m_LastUDPSourceSearch=DateTime.MinValue;
			m_LastPingUDP=DateTime.MinValue;
			m_NextTCPSourcesSearch=DateTime.MaxValue;
			m_UDPSourceSearchFileIndex=0;
			m_TCPSourceSearchFileIndex=0;
			m_SourceSearchServerIndex=0;
			m_ServerPingUDPIndex=0;
			m_TimesshownUDPClosedMessage=0;
			m_lowIDRetryHelper=new CLowIDRetryHelper();
		}

		/// <summary>
		/// Destroy the ServerList.
		/// </summary>
		~CServersList()
		{
			// disconnect from server if connected
			if (m_ActiveServer!=null) m_ActiveServer.Disconnect();
		}
		#endregion

		#region Internal Methods
		/// <summary>
		/// Connect to the next server.
		/// </summary>
		private void m_ConnectToNextServers()
		{
			if(this.Count==0) return;
			if (m_ActiveServer!=null)
			{
				//we have a good connection
				m_DeleteFailedServers();
				return;
			}
			if (m_FailedConnetions<=0) m_FailedConnetions=Protocol.MaxParalelConnections;
			if (m_ConnectionIndex>=this.Count)
			{
				//can not connect with any server, probably my connection is down, sleep some mintes and try again
				//and discount an error for all servers
				lock(InnerList.SyncRoot)
				{
					foreach (CServer server in InnerList)
					{
						server.ResetFails();
					}
				}
				Thread.Sleep(Protocol.RetryServerListConnection*1000);
				m_ConnectionIndex=0;
				if (ActiveServer!=null) return;
			}

			while ((m_FailedConnetions>0)&&(m_ConnectionIndex<this.Count))
			{
				CServer server=(CServer)this[m_ConnectionIndex];
				m_ConnectionIndex++;
				m_FailedConnetions--;
				server.Connect();
			}
		}

		/// <summary>
		/// Delete all servers where connection failed too often.
		/// </summary>
		private void m_DeleteFailedServers()
		{
			CServer server;
			int deleted=0;
			for (int i=0;i!=this.Count;i++)
			{
				server=(CServer)this[i];
				if (server.Fails>=CKernel.Preferences.GetShort("MaxServerFails"))
				{
					if (server!=ActiveServer)
					{
						this.RemoveAt(i);
						CKernel.DeleteServer(server.IP,server.Port);
						deleted++;
					}
				}
			}
			CLog.Log(Constants.Log.Info,"SRV_DELETED",deleted);
		}
		#endregion

		public CServer this[int index]
		{
			get
			{
				return (CServer)InnerList[index];
			}
			set
			{
				InnerList[index] = value;
			}
		}

		public CServer this[uint ip, ushort port]
		{
			get
			{
				lock(this)
				{
					foreach (CServer server in this)
					{
						if ((server.IP==ip)&&((port==0)||(server.Port==port)))
							return server;
					}
				}
				return null;
			}
		}

		public int Add(CServer value)
		{
			return InnerList.Add(value);
		}

		/// <summary>
		/// Add a server to the list.
		/// </summary>
		/// <param name="ip">IP of the server.</param>
		/// <param name="port">Port of the server.</param>
		/// <returns>The new server.</returns>
		public CServer Add(uint ip, ushort port)
		{
			if ((ip<Protocol.LowIDLimit)||(port==0)||(port>=IPEndPoint.MaxPort-4)) return null;

			if (this[ip,0]==null)
			{
				CServer server=new CServer(ip,port);
				this.Add(server);
				CKernel.NewServer(server);
				return server;
			}
			else
				return null;
		}

		public CServer Add(string strIP, ushort port)
		{
			IPAddress DirectionIP=IPAddress.Parse(strIP);
			uint ip=BitConverter.ToUInt32(DirectionIP.GetAddressBytes(),0);
			return Add(ip,port);
		}

		public int IndexOf(CServer value)
		{
			return InnerList.IndexOf(value);
		}

		public void Insert(int index, CServer value)
		{
			InnerList.Insert(index, value);
		}

		public void Remove(CServer value)
		{
			InnerList.Remove(value);
		}

		public bool Remove(uint ip, ushort port)
		{
			CServer server=this[ip,port];
			if (server!=null)
			{
				InnerList.Remove(server);
				CKernel.DeleteServer(ip,port);
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Remove all servers from the list.
		/// </summary>
		public new void Clear()
		{
			if (m_ActiveServer!=null)
			{
				m_ActiveServer.Disconnect();
				m_ActiveServer=null;
			}
			InnerList.Clear();
		}

		public bool Contains(CServer value)
		{
			return InnerList.Contains(value);
		}

		/// <summary>
		/// Connect to any server, if a server is already connected, disconnect from it and no connect again
		/// </summary>
		public void ConnectToAnyServer()
		{
			m_FailedConnetions=0;
			if (m_ConnectionIndex>=Count) m_ConnectionIndex=0;
			if (ActiveServer!=null)
			{
				ActiveServer.Disconnect();
				ActiveServer=null;
				//if (!CKernel.Preferences.GetBool("AutoReconnect")) m_ConnectToNextServers();
				return;
			}
			m_ConnectToNextServers();
		}

		/// <summary>
		/// Connect to an specified server.
		/// </summary>
		/// <param name="ip">IP of the server.</param>
		/// <param name="port">Port of the server</param>
		/// <returns>Returns true if connection could be established.</returns>
		public bool ConnectToServer(uint ip, ushort port)
		{
			CServer server=this[ip,port];
			if (server!=null)
			{
				if (m_ActiveServer!=null)
				{
					m_ActiveServer.Disconnect();
					m_ActiveServer=null;
				}
				server.DisconectedByUser=true; //set this flag to true to avoid reconnect to other servers if it fails
				server.Connect();
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Check if the given server is the active server.
		/// </summary>
		/// <param name="IP">IP of the server</param>
		/// <param name="port">Port of the server</param>
		/// <returns>Returns true if the given server data is the active server.</returns>
		public bool IsTheActiveServer(uint IP, ushort port)
		{
			if ((ActiveServer!=null)&&(ActiveServer.IP==IP)&&(ActiveServer.Port==port))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Set a server priority.
		/// </summary>
		/// <param name="ip">IP of the server.</param>
		/// <param name="port">Port of the server.</param>
		/// <param name="priority">New priority of the server.</param>
		public void SetServerPriority(uint ip, ushort port, Constants.ServerPriority priority)
		{
			lock(this)
			{
				foreach (CServer server in InnerList)
				{
					if ((server.IP==ip)&&((port==0)||(server.Port==port)))
					{
						server.Priority=priority;
						break;
					}
				}
			}
		}

		/// <summary>
		/// The connection to server failed.
		/// </summary>
		/// <param name="server">server</param>
		/// <param name="reason">reason</param>
		/// <returns></returns>
		public bool ConnectionFailed(CServer server, byte reason)
		{
			lock(this)
			{
				if (server==ActiveServer)
				{
					ActiveServer=null;
					m_FailedConnetions=0;
					m_ConnectionIndex=0;
					CLog.Log(Constants.Log.Info, "SRV_CNN_LOST");
					if (!CKernel.Preferences.GetBool("AutoReconnect"))
						return false;
					else
					{
						m_ConnectToNextServers();
						return false;
					}
				}
				else
					if (ActiveServer!=null) return false;

				if (m_ConnectionIndex>0)
				{
					m_FailedConnetions++;
					//if (m_FailedConnetions==Protocol.MAX_CONEXIONESPARALELAS)
					m_ConnectToNextServers();

				}
			}
			return true;
		}

		/// <summary>
		/// Request CallBack via UDP from specified server.
		/// </summary>
		/// <param name="ServerIP">IP of the server.</param>
		/// <param name="ServerPort">Port of the server.</param>
		/// <param name="IDClient">ID of the client.</param>
		public void RequestCallBackUDP(uint ServerIP, ushort ServerPort, uint IDClient)
		{
			CServer server=this[ServerIP,ServerPort];
			if (server==null) server=Add(ServerIP,ServerPort);
			if (server!=null) server.RequestCallBackUDP(IDClient);
		}

		/// <summary>
		/// Search sources for the 15 next files using TCP
		/// </summary>
		public void SearchNextSourcesTCP()
		{
			if ((ActiveServer!=null)&&
				(!ActiveServer.AllFilesPublished)&&
				(DateTime.Now-ActiveServer.LastPublishedTime>new TimeSpan(0,1,0)))
				ActiveServer.PublishSharedFiles();
			if ((m_NextTCPSourcesSearch>DateTime.Now)||(ActiveServer==null)||(CKernel.FilesList.Count==0)) return;

			if (m_TimesshownUDPClosedMessage<8) m_TimesshownUDPClosedMessage++;
			if ((!CKernel.UDPListener.PortIsAccesible)&&
				(m_TimesshownUDPClosedMessage<8)&&
				(m_TimesshownUDPClosedMessage>2)&& //ignore first 2 pass
				(CKernel.Preferences.GetUInt("ID")>Protocol.LowIDLimit))
			{
				if (m_TimesshownUDPClosedMessage<7)
					CLog.Log(Types.Constants.Log.Info,"UDP_PORT_NOT_OPEN");
				else
					CLog.Log(Types.Constants.Log.Notify,"UDP_PORT_NOT_OPEN");
			}
			ArrayList hashes=new ArrayList();
			CElement Element;
			do
			{
				if (CKernel.FilesList.Count<=m_TCPSourceSearchFileIndex)
				{
					m_TCPSourceSearchFileIndex=0;
					break;
				}
				else
				{
					Element=CKernel.FilesList[m_TCPSourceSearchFileIndex];
					m_TCPSourceSearchFileIndex++;
				}
				if ((Element.SourcesList!=null)&&(Element.File.FileStatus!=Protocol.FileState.Stopped)&&(Element.File.MaxSources>Element.SourcesList.Count()-5))
				{
					hashes.Add(Element.File.FileHash);
				}
			}
			while (hashes.Count<Protocol.SourcesPerTCPFrame);
			if ((m_TCPSourceSearchFileIndex==0)||(hashes.Count==0))
				m_NextTCPSourcesSearch=DateTime.Now+new TimeSpan(0,Protocol.ReaskServerTCP,0);
			else
				m_NextTCPSourcesSearch=DateTime.Now+new TimeSpan(0,0,Protocol.ReaskNextTCPFile*hashes.Count);
			if (hashes.Count>0) ActiveServer.RequestSources(hashes);
		}
		/// <summary>
		/// Search sources on the next server via UDP.
		/// </summary>
		public void SearchNextSourcesUDP()
		{
			if ((this.Count<=0)||(CKernel.FilesList.Count<=0)) return;
			if ((DateTime.Now.Ticks-m_LastUDPSourceSearch.Ticks) > Protocol.ReaskServerUDP)
			{
				m_LastUDPSourceSearch=DateTime.Now;
				if (this.Count<=m_SourceSearchServerIndex) m_SourceSearchServerIndex=0;
				CServer server=(CServer)this[m_SourceSearchServerIndex];
				ArrayList hashes=new ArrayList();
				CElement Element;
				uint allComplete=0;
				do
				{
					m_UDPSourceSearchFileIndex++;
					if (CKernel.FilesList.Count<=m_UDPSourceSearchFileIndex) m_UDPSourceSearchFileIndex=0;
					Element=CKernel.FilesList[m_UDPSourceSearchFileIndex];
					if ((Element.SourcesList!=null)&&(Element.File.FileStatus!=Protocol.FileState.Stopped)&&(Element.File.MaxSources>Element.SourcesList.Count()-5))
					{
						hashes.Add(Element.File.FileHash);
						if (hashes.Count>=Protocol.MaxRequestsPerServer) break;
					}
					allComplete++;
				}
				while (allComplete<CKernel.FilesList.Count);
				//	if (allComplete<CKernel.FilesList.Count)
				//	{
				//server.RequestSourcesUDP(Element.File.FileHash);
				server.RequestSourcesUDP(hashes);
				m_SourceSearchServerIndex++;
				//	}
			}
		}

		/// <summary>
		/// Ping next server via UDP.
		/// </summary>
		public void NextPingUDP()
		{
			if (this.Count<=0) return;
			if ((DateTime.Now-m_LastPingUDP)>new TimeSpan(0,Protocol.ReaskPingUDP,0))
			{
				m_LastPingUDP=DateTime.Now;
				if (this.Count<=m_ServerPingUDPIndex) m_ServerPingUDPIndex=0;
				CServer server=(CServer)this[m_ServerPingUDPIndex];
				m_ServerPingUDPIndex++;
				server.RequestUDPPing();
			}
		}

		public void Sort()
		{
			IComparer myServerListSorter = new ServerListSorter();
			InnerList.Sort(myServerListSorter);
		}

		#region Load & Save Methods
		/// <summary>
		/// Load a list of servers from binary file (server.met).
		/// </summary>
		/// <param name="fullpathfile">The full Path to the file.</param>
		/// <param name="clear">Set clear to true if the list should be cleared before read.</param>
		/// <returns>Count of servers which could be loaded.</returns>
		public int Load(string fullpathfile, bool clear)
		{
			if (clear) Clear();
			FileStream servermet;
			if (fullpathfile.Length==0)
				//fullpathfile=Application.StartupPath + Path.DirectorySeparatorChar + "server.met";
				fullpathfile=Path.Combine(CKernel.DllDirectory, "server.met");

			try
			{
				servermet=File.OpenRead(fullpathfile);
			}
			catch
			{
				return 0;
			}

			byte version;
			BinaryReader reader=new BinaryReader(servermet);
			int added=0;
			try
			{
				version=reader.ReadByte();

				if (!Enum.IsDefined(typeof(Protocol.ServerMet), version))
				{
					servermet.Close();
					CLog.Log(Constants.Log.Notify,"SRVMET_INVALID");
					return 0;
				}
				uint nServers=reader.ReadUInt32();
				uint ip;
				ushort port;
				CServer server;
				while (nServers>0)
				{
					ip=reader.ReadUInt32();
					port=reader.ReadUInt16();
					server=Add(ip,port);
					if (server!=null)
						added++;
					else
						server=this[ip,0];
					server.Load(reader);
					nServers--;
				}
				Sort();
			}
			catch(EndOfStreamException eose)
			{
				Debug.WriteLine(eose.ToString());
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
				CLog.Log(Constants.Log.Notify,"SRVMET_INVALID");
			}
			servermet.Close();
			CLog.Log(Constants.Log.Info,"SRV_ADDED",added);
			return added;
		}

		/// <summary>
		/// Save list of servers to binary file (server.met).
		/// </summary>
		/// <param name="fullpathfile">Full path to the file.</param>
		public void Save(string fullpathfile)
		{
			int nServers=this.Count;
			if(nServers==0) return;

			FileStream servermet;
			if (fullpathfile.Length==0)
				fullpathfile=Path.Combine(CKernel.DllDirectory, "server.met");
				//fullpathfile=Application.StartupPath + Path.DirectorySeparatorChar + "server.met";

			try
			{
				servermet=File.OpenWrite(fullpathfile);
			}
			catch
			{
				return;
			}

			try
			{
				byte version;
				BinaryWriter writer=new BinaryWriter(servermet);

				version=(byte)Protocol.ServerMet.eDonkey;
				writer.Write(version);
				writer.Write(nServers);

				for (int i=0;i!=nServers;i++)
				{
					if (((CServer)this[i]).Fails<CKernel.Preferences.GetShort("MaxServerFails"))
					{
						writer.Write(((CServer)this[i]).IP);
						writer.Write(((CServer)this[i]).Port);
						((CServer)this[i]).Save(writer);
					}
				}
				writer.Close();
				servermet.Close();
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}
		#endregion

		#region Comparer Class
		/// <summary>
		/// Class compares two servers and order them by files.
		/// </summary>
		public class ServerListSorter : IComparer
		{
			int IComparer.Compare(Object x,Object y)
			{
				if ((x==null)||(y==null)) return 0;

				CServer serverA = (CServer)x;
				CServer serverB = (CServer)y;

				byte prioA=(byte)((byte)serverA.Priority+1);
				byte prioB=(byte)((byte)serverB.Priority+1);
				if (prioA==2) prioA=0;
				if (prioB==2) prioB=0;

				if (prioA<prioB)
					return -1;
				else if (prioA>prioB)
					return 1;

				if(serverA.Files>serverB.Files)
					return -1;
				else
					return 1;
			}
		}
		#endregion
	}

	/// <summary>
	/// CLowIDRetry  helps the server connection deciding when it should retry
	///  in order to get a high id
	/// </summary>
	internal class CLowIDRetryHelper
	{
		private byte m_numRetriesLowID;    //The counter of retries left
		private ushort m_tcpPort;		   //Last tcpPort we got a HighID
		private ushort m_udpPort;		   //Last udpPort we got a HighID

		public CLowIDRetryHelper()
		{
			uint lastId = (uint)CKernel.Preferences.GetUInt("ID");
			//Check if the program finished last run with a High id
			if (lastId > Protocol.LowIDLimit)
			{
				m_numRetriesLowID = Protocol.MaxRetriesLowID;
				m_tcpPort = (ushort)CKernel.Preferences.GetProperty("TCPPort");
				m_udpPort = (ushort)CKernel.Preferences.GetProperty("UDPPort");
			}
			else
			{
				m_numRetriesLowID = 0;
				m_tcpPort = 0;
				m_tcpPort = 0;
			}
		}

		/// <summary>
		/// This method is called to get advice about retrying the connection
		/// </summary>
		public bool ShouldRetry()
		{
			bool result = false;

			if (m_numRetriesLowID > 0)
			{
				lock (this)
				{
					//Check if the port configuration has been changed
					if (m_tcpPort != (ushort)CKernel.Preferences.GetProperty("TCPPort") ||
						m_udpPort != (ushort)CKernel.Preferences.GetProperty("UDPPort"))
					{
						m_numRetriesLowID = 0;
						result = false;
					}
					else
					{
						m_numRetriesLowID--;
						result = true;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// This method should be called when a high ID connection is got
		/// </summary>
		public void GotHighID()
		{
			lock (this)
			{
				m_numRetriesLowID = Protocol.MaxRetriesLowID;
				m_tcpPort = (ushort)CKernel.Preferences.GetProperty("TCPPort");
				m_udpPort = (ushort)CKernel.Preferences.GetProperty("UDPPort");
			}
		}
	}
}