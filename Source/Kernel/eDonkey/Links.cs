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
using System.Text.RegularExpressions;
using System.Collections;
using System.Net;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for CLinks.
	/// </summary>
	public class CLinks
	{
		#region Instance fields
		private bool m_isFileLink;
		private bool m_isServerLink;
		private string m_filename;
		private string m_filesize;
		private string m_filehash;
		private string m_serverIP;
		private string m_serverPort;
		private stDatosFuente[] m_clients;
		private bool m_ErrorFound;
		#endregion

		#region Propertys
		public bool IsFileLink
		{
			get
			{
				return ((m_isFileLink)&&(!m_ErrorFound));
			}
		}

		public bool IsServerLink
		{
			get
			{
				return ((m_isServerLink)&&(!m_ErrorFound));
			}
		}

		public string FileName
		{
			get
			{
				return m_filename;
			}
		}

		public uint FileSize
		{
			get
			{
				return Convert.ToUInt32(m_filesize);
			}
		}

		public byte[] FileHash
		{
			get
			{
				return CKernel.StringToHash(m_filehash);
			}
		}

		public string ServerIP
		{
			get
			{
				return m_serverIP;
			}
		}

		public ushort ServerPort
		{
			get
			{
				return Convert.ToUInt16(m_serverPort);
			}
		}

		public stDatosFuente[] Clients
		{
			get
			{
				return m_clients;
			}
		}
		#endregion

		#region Constructors & Initialization

		public CLinks()
		{
		}

		public CLinks(string elink)
		{
			Parse(elink);
		}
		#endregion

		#region Public Methods

		public bool Parse(string elink)
		{
			m_ErrorFound=false;
			elink= m_ExtractType(elink);
			if (m_ErrorFound) return m_ErrorFound;
			if (m_isFileLink)
			{
				elink= m_ExtractFileInfo(elink);
				if ((elink.Length>1)&&(!m_ErrorFound)) elink=m_ExtractSourcesInfo(elink);
			}
			else
				m_ErrorFound=m_ExtractServersInfo(elink);
			return m_ErrorFound;
		}
		#endregion

		#region Internal Methods

		private bool m_ExtractServersInfo(string elink)
		{
			Regex regex = 
				new Regex(@"(?<serverIP>[0-9.]+\.[0-9.]+\.[0-9.]+\.[0-9.]+)\|(?<serverPort>[0-9]+)\|(?<rest>.*)",
					RegexOptions.IgnoreCase
					| RegexOptions.Multiline
					| RegexOptions.IgnorePatternWhitespace
					| RegexOptions.Compiled
				);
			Match m=regex.Match(elink);
			if (m.Success)
			{
				m_serverIP=m.Result("${serverIP}");
				m_serverPort=m.Result("${serverPort}");
				if (uint.Parse(m_serverPort)>ushort.MaxValue)
				{
					m_ErrorFound=true;
					return true;
				}
				else
					return false;
			}
			else 
			{
				m_ErrorFound=true;
				return true;
			}
		}

		private string m_ExtractSourcesInfo(string elink)
		{
			Regex regex = 
				new Regex(@"\/\|sources(?<sources>.*)\|(?<rest>.*)",
					RegexOptions.IgnoreCase
					| RegexOptions.Multiline
					| RegexOptions.IgnorePatternWhitespace
					| RegexOptions.Compiled
				);
			Match m=regex.Match(elink);
			if (m.Success)
			{
				string sources=m.Result("${sources}");
				ArrayList sourcesList=new ArrayList();
				while (sources.Length>0)
				{
					regex=new Regex(@",(?<ip>[0-9.]+\.[0-9.]+\.[0-9.]+\.[0-9.]+):(?<port>[0-9]+)(?<sources>.*)",
					RegexOptions.IgnoreCase
					| RegexOptions.Multiline
					| RegexOptions.IgnorePatternWhitespace
					| RegexOptions.Compiled
						);
					m=regex.Match(sources);
					if (m.Success)
					{
						stDatosFuente sourcefound=new stDatosFuente();
						IPAddress ip = System.Net.IPAddress.Parse(m.Result("${ip}"));
						sourcefound.IP=BitConverter.ToUInt32(ip.GetAddressBytes(),0);
						sourcefound.Port=Convert.ToUInt16(m.Result("${port}"));
						sourcesList.Add(sourcefound);
						sources=m.Result("${sources}");
					}else break;
				}
				if (sourcesList.Count>0) 
				{
					m_clients=new stDatosFuente[sourcesList.Count];
					int i=0;
					foreach (stDatosFuente src in sourcesList)
					{		
						m_clients[i]=src;
						i++;
					}

				}
				return sources;
			}
			else return "";
		}

		private string m_ExtractFileInfo(string elink)
		{
			Regex regex = 
				new Regex(@"(?<name>.+)\|(?<size>\d+)\|(?<hash>[0-9a-fA-F]{32})\|(?<rest>.*)",
					RegexOptions.IgnoreCase
					| RegexOptions.Multiline
					| RegexOptions.IgnorePatternWhitespace
					| RegexOptions.Compiled
				);
			Match m=regex.Match(elink);
			if (m.Success)
			{
				m_filename=m.Result("${name}");
				m_filesize=m.Result("${size}");
				m_filehash=m.Result("${hash}");
				if(m_filesize=="0")
				{
					m_ErrorFound=true;
					return "";}
				else
                    return m.Result("${rest}");
			}
			else 
			{
				m_ErrorFound=true;
				return "";
			}
		}

		private string m_ExtractType(string elink)
		{
			if (elink==null)
			{
				m_ErrorFound=true;
				return "";
			}
			Regex regex = 
				new Regex(@"ed2k:\/\/\|(?<class>file|server)\|(?<rest>.*)",
					RegexOptions.IgnoreCase
					| RegexOptions.Multiline
					| RegexOptions.IgnorePatternWhitespace
					| RegexOptions.Compiled
				);
			Match m=regex.Match(elink);
			if (m.Success)
			{
				m_isServerLink=(m.Result("${class}")=="server");
				m_isFileLink=!m_isServerLink;
				return m.Result("${rest}");
			}
			{
				m_ErrorFound=true;
				return "";
			}
		}
		#endregion
	}
}