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
using System.Threading;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for CListener.
	/// </summary>
	internal class CListener:TcpListener
	{
		private Thread m_ServerThread;

		public new bool Start()
		{
			bool res;
			try
			{
				m_ServerThread=new Thread(new ThreadStart(m_ListenerProc));	
				m_ServerThread.Name="TCP listener";
				base.Start();
				res=true;
				m_ServerThread.Start();	
			}
			catch 
			{
				res=false;
				CLog.Log(Types.Constants.Log.Verbose,"PORT_INUSE",CKernel.Preferences.GetUShort("TCPPort"));
			}
		 
			return res;
		}

		public new void Stop()
		{
			try
			{
				base.Stop();
				if ((m_ServerThread!=null)&&(m_ServerThread.IsAlive)) m_ServerThread.Abort();
			}
			catch{}
		}

		public CListener(ushort port):base(IPAddress.Any, port)
		{
			
		}

		public CListener():base(IPAddress.Any, CKernel.Preferences.GetUShort("TCPPort"))
		{
			base.Server.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReuseAddress,1);
		}

		private void m_ListenerProc()
		{
			Socket edonkeySocket;
		 
			while ((m_ServerThread.IsAlive)&&(base.Active))
			{ 
				edonkeySocket=null;
				try
				{
					edonkeySocket=base.AcceptSocket();
					edonkeySocket.Blocking=false;
				}
				catch
				{
				}
				if (edonkeySocket!=null) 
				{ 
					CConnection connection=null;
					if ((CKernel.IPFilter==null)||(!CKernel.IPFilter.BlockIP(BitConverter.ToUInt32(((IPEndPoint)edonkeySocket.RemoteEndPoint).Address.GetAddressBytes(),0))))
						connection=new CConnection(edonkeySocket);
					else 
					{
						CLog.Log(Types.Constants.Log.Verbose,"IP_BLOCKED",((IPEndPoint)edonkeySocket.RemoteEndPoint).Address.ToString(),CKernel.IPFilter.BlockedIPDescription);
						edonkeySocket.Shutdown(SocketShutdown.Both);
						edonkeySocket.Close();
					}
				}
			}
		}

		~CListener()
		{
			base.Stop();
			try
			{
				if ((m_ServerThread!=null)&&(m_ServerThread.IsAlive)) m_ServerThread.Abort();
			}
			catch{};
		}
	}
}