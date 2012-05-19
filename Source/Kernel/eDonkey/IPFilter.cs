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
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Collections;
using eLePhant.Types;


/*CIPFilter IPFilter = new CIPFilter();
IPFilter.LoadIPFilter(Application.StartupPath,"ipfilter.dat",128);

- 192.168.100.3
Debug.WriteLine(IPFilter.BlockIP("192.168.100.3",128).ToString());
Debug.WriteLine(IPFilter.BlockedIPDescription);

- 239.168.100.3
Debug.WriteLine(IPFilter.BlockIP(40151279,128).ToString());
*/

namespace eLePhant.eDonkey
{
	public class CIPFilter
	{	
		#region Instance fields
		private struct IPRange
		{
			private uint m_IPfrom;
			private uint m_IPto;
			private byte m_Filter;
			private string m_Description;

			public IPRange(uint IPfrom, uint IPto, byte Filter, string Description) 
			{
				m_IPfrom = IPfrom;
				m_IPto = IPto;
				m_Filter = Filter;
				m_Description = Description;
			}
			
			public uint IPfrom
			{
				get	{return m_IPfrom;}
			}
			
			public uint IPto
			{
				get	{return m_IPto;}
			}
			
			public byte Filter
			{
				get	{return m_Filter;}
			}
			
			public string Description
			{
				get	{return m_Description;}
			}
		}

		private ArrayList m_IPFilterList = new ArrayList();
		private string m_BlockedIPDescription;
		private bool m_IsFilterDatExists;
		private byte m_maxFilter;
		private DateTime m_LastDisplayTotal;
		private uint m_TotalHits;
		#endregion
		
		#region Propertys
		public string BlockedIPDescription
		{
			get	{return m_BlockedIPDescription;}
		}
		public bool IsFilterDatExists
		{
			get	{return m_IsFilterDatExists;}
		}

		#endregion
		
		#region Constructors & Methods
		public CIPFilter()
		{
			m_LastDisplayTotal=DateTime.Now;
		}
		
		/// <summary>
		/// Loads a IPfilter.dat.
		/// </summary>
		/// <returns>
		/// false = no file or the number of ranges
		/// </returns>
		/// <param name="path">
		/// The path of the ipfilter.dat.
		/// </param>
		/// <param name="fileName">
		/// The filename of the ipfilter.dat.
		/// </param>
		/// <param name="maxFilter">
		/// The filter at loading, 255 = no filter.
		/// </param>
		public bool LoadIPFilter(string path,string fileName,byte maxFilter)
		{	
			uint IPfrom;
			uint IPto;
			byte Filter;
			int count=0;
						
			m_maxFilter = maxFilter;
			string fullpath = Path.Combine(path, fileName);

			if (File.Exists(fullpath)) 
			{
				using(StreamReader sr = File.OpenText(fullpath))
				{
					string s = "";
					while ((s = sr.ReadLine()) != null) 
					{
						//000.000.000.000 - 000.255.255.255 , 100 , LOW_ID (invalid)
						//ignore comments and short string
						
						try
						{
							if(s.Length<18 || s[0]=='#' || s[0]=='/') continue;
							string [] splitString = s.Split(new Char [] {'-', ',', ','});
							
							Filter = Convert.ToByte(splitString[2]);
							if(Filter>m_maxFilter) continue;

							IPfrom = IPConvert(splitString[0]);
							IPto = IPConvert(splitString[1]);
							
							IPRange myIPRange = new IPRange(IPfrom,IPto,Filter,splitString[3]);
							m_IPFilterList.Add(myIPRange);
							count++;
						}
						catch(Exception e)
						{
							Debug.WriteLine(s+" : "+e.Message);
						}
					}
				}
				m_IPFilterList.Sort(new IPRangeSorter());
				CLog.Log(Constants.Log.Info,"IP_FILTER_LOADED",count.ToString(),fileName);
				m_IsFilterDatExists = true;
				return m_IsFilterDatExists;
			}
			else
			{	m_IsFilterDatExists = false;
				return m_IsFilterDatExists;
			}
		}
		
		/// <summary>
		/// The IP must be blocked.
		/// </summary>
		/// <param name="in_IPtoTest">
		/// The IP to test, string or uint
		/// </param>
		/// <param name="in_filter">
		/// The max filter.
		/// </param>
		/// 
		
		public bool BlockIP(uint in_IPtoTest)
		{
			return BlockIP(in_IPtoTest, m_maxFilter);
		}

		public bool BlockIP(uint in_IPtoTest, byte in_filter)
		{
			try
			{	
				byte filter = in_filter;
				uint IPtoTest = IPConvert(in_IPtoTest);
				return IsFiltered(IPtoTest,filter);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.Message);
				return false;
			}
		}
		
		public bool BlockIP(string in_IPtoTest)
		{
			return BlockIP(in_IPtoTest, m_maxFilter);
		}

		public bool BlockIP(string in_IPtoTest, byte in_filter)
		{
			try
			{	
				byte filter = in_filter;
				uint IPtoTest = IPConvert(in_IPtoTest);
				return IsFiltered(IPtoTest,filter);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.Message);
				return false;
			}
		}
		
		/// <summary>
		/// Filter the IP.
		/// </summary>
		/// <param name="IPtoTest">
		/// The IP to test, always the integer value.
		/// </param>
		/// <param name="filter">
		/// The max filter.
		/// </param>
		private bool IsFiltered(uint IPtoTest, byte filter)
		{	
			if(m_IPFilterList.Count==0 || IPtoTest==0) return false; 
			
			if(DateTime.Now-m_LastDisplayTotal>new TimeSpan(0,20,0))
			{
				m_LastDisplayTotal=DateTime.Now;
				CLog.Log(Constants.Log.Notify,"TOTAL_IP_BLOCKED",m_TotalHits.ToString());
			}

			foreach(IPRange ip in m_IPFilterList)
			{
				if(ip.IPfrom>IPtoTest) return false;
				if(IPtoTest<=ip.IPto && IPtoTest>=ip.IPfrom && ip.Filter<filter)
				{	
					m_BlockedIPDescription = ip.Description;
					m_TotalHits++;
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Converts an IP(192.168.100.10 or uint IP) into an integer.
		/// </summary>
		/// <returns>
		/// Returns an IP into an integer
		/// </returns>
		/// <param name="in_IP">
		/// The IP, could be a string or a unit.
		/// </param>
		private uint IPConvert(uint in_IP)
		{
			string ipString = (new IPAddress(in_IP)).ToString();
			return IPConvert(ipString);
		}
		
		private uint IPConvert(string in_IP)
		{
			string [] IP = in_IP.Split(".".ToCharArray());
			
			try
			{
				return (Convert.ToUInt32(IP[0])*(16777216) + Convert.ToUInt32(IP[1])*65536 + Convert.ToUInt32(IP[2])*256 + Convert.ToUInt32(IP[3]));
			}
			catch(Exception e)
			{
				throw(e);
			}
		}

		private class IPRangeSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				if ((x==null)||(y==null)) return 0;
				IPRange IPRang1 = (IPRange)x;
				IPRange IPRang2 = (IPRange)y;
				
				if(IPRang1.IPfrom<IPRang2.IPfrom)
					return -1;
				else
					return 1;
			}
		}

		#endregion
	}
}
