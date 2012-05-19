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

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for FileStatistics.
	/// </summary>
	public class CFileStatistics
	{
		#region Instance fields
		
		private ushort[] m_UploadAvaibility;
		private uint m_SessionRequests;
		private ulong m_SessionUpload;
		private ulong m_SessionDownload;
		private DateTime m_StartTime;
		private DateTime m_TimeCompleted;
		private int m_QueuedClients;
		
		#endregion

		#region Propertys

		public ushort[] UploadAvaibility
		{
			get
			{
				return m_UploadAvaibility;
			}
		}

		public uint SessionRequests
		{
			get
			{
				return m_SessionRequests;
			}
		}
		public ulong SessionUpload
		{
			get
			{
				return m_SessionUpload;
			}
		}
		
		public ulong SessionDownload
		{
			get
			{
				return m_SessionDownload;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return m_StartTime;
			}
		}

		public DateTime TimeCompleted
		{
			get
			{
				return m_TimeCompleted;
			}
			set
			{
				m_TimeCompleted=value;
			}
		}

		public int QueuedClients
		{
			get
			{
				return m_QueuedClients;
			}
		}

		#endregion

		#region Constructors & Initialization
		
		public CFileStatistics()
		{
			m_StartTime = DateTime.Now;
			m_TimeCompleted=DateTime.MinValue;
		}
		
		#endregion
		
		public float GetAvgDown()
		{
			TimeSpan dif;
			if (m_TimeCompleted==DateTime.MinValue)
				dif=DateTime.Now-m_StartTime;
			else
				dif=m_TimeCompleted-m_StartTime;
			return (float)Math.Round((float)(m_SessionDownload/1024F)/((float)dif.TotalSeconds+1),1);
		}

		public float GetAvgUp()
		{
			TimeSpan dif;
			if (m_TimeCompleted==DateTime.MinValue)
				dif=DateTime.Now-m_StartTime;
			else
				dif=m_TimeCompleted-m_StartTime;
			return (float)Math.Round((float)(m_SessionUpload/1024F)/((float)dif.TotalSeconds+1),1);
		}
		
		public void ResetStartTime()
		{
			if (m_SessionDownload==0)
				m_StartTime = DateTime.Now;
			//do not allow reset if file is stopped and started again
		}

		public void AddUploadChunksAvaibility(byte[] chunkStatus)
		{
			m_QueuedClients++;
			if (chunkStatus==null) return;
			if (m_UploadAvaibility==null) m_UploadAvaibility=new ushort[chunkStatus.Length];
			else if (chunkStatus.Length!=m_UploadAvaibility.Length) return;
			for (int i=0; i!=chunkStatus.Length;i++)
			{
				if ((Protocol.ChunkState)chunkStatus[i]==Protocol.ChunkState.Complete)
					m_UploadAvaibility[i]++;
			}			
		}
		public void RemoveUploadChunksAvaibility(byte[] chunkStatus)
		{
			if (m_QueuedClients>0) m_QueuedClients--;
			if (chunkStatus==null) return;
			if (m_UploadAvaibility==null) m_UploadAvaibility=new ushort[chunkStatus.Length];
			else if (chunkStatus.Length!=m_UploadAvaibility.Length) return;
			for (int i=0; i!=chunkStatus.Length;i++)
			{
				if (((Protocol.ChunkState)chunkStatus[i]==Protocol.ChunkState.Complete)
					&&(m_UploadAvaibility[i]>0))
					m_UploadAvaibility[i]--;
			}	
		}
		public void IncSessionRequests()
		{
			m_SessionRequests++;
		}
		
		public void IncSessionUpload(uint uploaded)
		{
			m_SessionUpload+=uploaded;
		}
		public void IncSessionDownload(uint downloaded)
		{
			m_SessionDownload+=downloaded;
		}
	}
}
