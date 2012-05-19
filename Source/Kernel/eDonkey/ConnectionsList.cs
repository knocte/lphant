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

namespace eLePhant.eDonkey
{
	/// <summary>
	/// CConnectionsList handles all opened connections.
	/// </summary>
	internal class CConnectionsList
	{
		private bool m_Closing;
		private ArrayList m_ArrayList;

		public CConnectionsList():base()
		{
			m_ArrayList=ArrayList.Synchronized(new ArrayList());
			m_Closing=false;
		}

		public int Count()
		{
			return m_ArrayList.Count;
		}

		public void CloseAll()
		{
			m_Closing=true;
			lock(m_ArrayList.SyncRoot)
			{
				foreach (CConnection cn in m_ArrayList)
				{
					cn.CloseSocket();
				}
			}
			m_ArrayList.Clear();
		}

		public void Add(CConnection cn)
		{
			if (m_Closing) 
			{
				cn.CloseSocket();
				return;
			}
			m_ArrayList.Add(cn);
		}

		public void Remove(CConnection cn)
		{
			if (m_Closing) 
			{
				cn.CloseSocket();
				return;
			}
			m_ArrayList.Remove(cn);
		}
		public void CheckTimeOuts()
		{
			CConnection cn;
			TimeSpan timeout=new TimeSpan(0,0,Protocol.ConnectionTimeOut);
			for (int i=0;i<m_ArrayList.Count;i++)
			{
				cn=(CConnection)m_ArrayList[i];
				if ((cn.Connected)&&((DateTime.Now-cn.LastPacket)>=timeout))
					cn.OnTimeOut();
			}
		}
	}
}