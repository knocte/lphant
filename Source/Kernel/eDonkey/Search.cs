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
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for CFileFound.
	/// </summary>
	internal class CFileFound
	{
		public string Hash;
		public string Name;
		public uint Size;
		public uint Avaibility;
		public string Codec;
		public uint BitRate;
		public string Length;
		public bool Complete;
		public Hashtable Sources;

		public Types.Constants.SearchResultState ResultState;
		public StringCollection OtherNames;

		public CFileFound(string in_Hash, string in_Name, uint in_Size, uint in_Avaibility, string in_codec,string in_length,uint in_bitrate, bool in_complete, uint in_ip, ushort in_port)
		{
			this.Hash=in_Hash;
			this.Name=in_Name;
			this.Size=in_Size;
			this.Avaibility=in_Avaibility;
			Codec=in_codec;
			BitRate=in_bitrate;
			Length=in_length;
			Complete=in_complete;
			this.OtherNames=new StringCollection();
			this.OtherNames.Add(Name);
			CElement element=CKernel.FilesList[CKernel.StringToHash(in_Hash)];
			if (element==null) 
				ResultState=Types.Constants.SearchResultState.New;
			else if (element.File.FileStatus==Protocol.FileState.Complete)
				ResultState=Types.Constants.SearchResultState.AlreadyDownloaded;
			else
				ResultState=Types.Constants.SearchResultState.AlreadyDownloading;
			if ((in_ip>Protocol.LowIDLimit)&&(in_port>0)&&(in_port<ushort.MaxValue))
			{
				Sources=new Hashtable();
				Sources.Add(in_ip,in_port);
				//Debug.WriteLine(in_ip.ToString()+":"+in_port.ToString());
				if ((element!=null)&&(element.File.FileStatus==Protocol.FileState.Ready))
					CKernel.ClientsList.AddClientToFile(in_ip,in_port,0,0,element.File.FileHash);
			}
		}

		public void UpdateFile(uint in_Avaibility,string in_Name, string in_codec,string in_length,uint in_bitrate, bool in_complete, uint in_ip, ushort in_port)
		{
			this.Avaibility+=in_Avaibility;
			Complete=Complete||in_complete;
			if (!this.OtherNames.Contains(in_Name))
			{
				this.OtherNames.Add(in_Name);
			}
			if (((Length==null)||(Length.Length==0))&&(in_length.Length>0))
				Length=in_length;
			if ((Codec.Length==0)&&(in_codec.Length>0))
				Codec=in_codec;
			if ((BitRate==0)&&(in_bitrate>0))
				BitRate=in_bitrate;
			if ((in_ip>Protocol.LowIDLimit)&&(in_port>0)&&(in_port<ushort.MaxValue))
			{
				if (Sources==null) Sources=new Hashtable();
				if (!Sources.Contains(in_ip)) Sources.Add(in_ip,in_port);
				//Debug.WriteLine(in_ip.ToString()+":"+in_port.ToString());
				CElement element=CKernel.FilesList[CKernel.StringToHash(Hash)];
				if ((element!=null)&&(element.File.FileStatus==Protocol.FileState.Ready))
				{
					CKernel.ClientsList.AddClientToFile(in_ip,in_port,0,0,element.File.FileHash);
				}
			}
		}
		
		public void AddSourcesToFile()
		{
			if ((Sources==null)||(Sources.Count==0)) return;
			CElement element=CKernel.FilesList[CKernel.StringToHash(this.Hash)];
			int i;
			if ((element!=null)&&(element.File.FileStatus==Protocol.FileState.Ready))
			{
				stDatosFuente[] sourcesList=new stDatosFuente[Sources.Count];
				i=0;
				foreach (uint ip in Sources.Keys)
				{
					sourcesList[i].IP=ip;
					sourcesList[i].Port=(ushort)Sources[ip];
					i++;
				}
				CKernel.ClientsList.AddClientsToFile(sourcesList,CKernel.StringToHash(this.Hash));
			}
		}

		public string ToLink()
		{
			return "ed2k://|file|"+this.Name+"|"+this.Size.ToString()+"|"+this.Hash+"|/";
		}
	}

	internal class CSearcher:Hashtable
	{
		private int m_ServerIndex;
		private MemoryStream m_TCPPacket;
		private MemoryStream m_UDPPacket;
		private uint m_sources;
		private bool m_SearchCanceled;
		private bool m_IsClientSearch;
		private bool m_Searching;

		public void CancelSearch()
		{
			m_SearchCanceled=true;
		}

		/// <summary>
		/// Constructor used in common client server searchs
		/// </summary>
		/// <param name="searchString">searchString</param>
		/// <param name="matchAnyWords">matchAnyWords</param>
		/// <param name="type">type</param>
		/// <param name="maxSize">maxSize</param>
		/// <param name="minSize">minSize</param>
		/// <param name="avaibility">avaibility</param>
		public CSearcher(string searchString, bool matchAnyWords, string type, uint maxSize, uint minSize, uint avaibility, string exclude) : base()
		{
			// buscamos primero en el servidor en el que estamos
			m_TCPPacket=new MemoryStream();
			m_UDPPacket=new MemoryStream();
			m_sources=0;
			m_IsClientSearch=false;
			m_SearchCanceled=false;
			m_Searching=true;
			CServerRequestSearch ServerRequestSearchTCP=new CServerRequestSearch(m_TCPPacket, searchString, matchAnyWords, type, maxSize, minSize, avaibility, exclude, false);
			CServerRequestSearch ServerRequestSearchUDP=new CServerRequestSearch(m_UDPPacket, searchString, matchAnyWords, type, maxSize, minSize, avaibility, exclude, true);
			CKernel.ServersList.ActiveServer.SendTCPSearch(m_TCPPacket,this);
		}

		/// <summary>
		/// Constructor used when the search is a shard file list request
		/// </summary>
		public CSearcher()
		{
			m_IsClientSearch=true;
			m_Searching=false;
		}

		public void ExtendSearch()
		{
			if ((CKernel.ServersList.Count > m_ServerIndex)
				&& (CServer)(CKernel.ServersList[m_ServerIndex])!=CKernel.ServersList.ActiveServer)
			{
				CServer nextServer;
				nextServer=(CServer)CKernel.ServersList[m_ServerIndex];
				nextServer.SendUDPSearch(m_UDPPacket,this);
			}
			m_ServerIndex++;

			if (m_ServerIndex>=CKernel.ServersList.Count)
			{
				m_ServerIndex=0;
			}
		}

		public void AddFileFound(byte[] Hash, string name, uint size, uint avaibility, string codec, string length, uint bitrate, bool complete, uint ip, ushort port)
		{
			try
			{
				string strHash=CKernel.HashToString(Hash);

				m_sources += avaibility;
				if (ContainsKey(strHash))
				{
					CFileFound fileFound=(CFileFound)this[strHash];
					fileFound.UpdateFile(avaibility,name,codec,length,bitrate,complete,ip,port);
					CKernel.SearchFileModified(fileFound,(int)CKernel.Searchs.GetKey(CKernel.Searchs.IndexOfValue(this)));
				}
				else
				{
					CFileFound fileFound=new CFileFound(strHash, name, size, avaibility, codec,length,bitrate,complete,ip,port);
					Add(strHash,fileFound);
					CKernel.NewFileFound(fileFound,(int)CKernel.Searchs.GetKey(CKernel.Searchs.IndexOfValue(this)));
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine("Search error");
				Debug.WriteLine(e.ToString());
				CKernel.SearchEnded((int)CKernel.Searchs.GetKey(CKernel.Searchs.IndexOfValue(this)));
			}
		}

		public void OnTCPSearchEnded()
		{
			if ((CKernel.Preferences.GetBool("AutoExtendSearch"))
				&&(m_sources < Protocol.MaxSearchResults)
				&&(!m_IsClientSearch)&&(m_Searching))
			{
				Debug.Write("Autoextending search\n");

				m_ServerIndex=0;
				for (uint i=0;i<CKernel.ServersList.Count;i++)
				{
					if ((m_sources>Protocol.MaxSearchResults)||(m_SearchCanceled)) break;

					ExtendSearch();
					Thread.Sleep(250);
					CKernel.NewSearchProgress((int)(((float)i/(float)CKernel.ServersList.Count)*100.0F),(int)CKernel.Searchs.GetKey(CKernel.Searchs.IndexOfValue(this)));
				}
				//m_UDPPacket.Close();
				//m_UDPPacket=null;
				if (m_ServerIndex>=CKernel.ServersList.Count)
				{
					m_ServerIndex=0;
				}
			}
//			else
//			{
//				if ((!m_IsClientSearch)&&(m_Searching)&&(CKernel.ServersList.ActiveServer!=null))
//					CKernel.ServersList.ActiveServer.QueryMoreResults();
//
//			}
			m_Searching=false;
			if (CKernel.Searchs.IndexOfValue(this)>=0) 
				CKernel.SearchEnded((int)CKernel.Searchs.GetKey(CKernel.Searchs.IndexOfValue(this)));
		}
	}
}