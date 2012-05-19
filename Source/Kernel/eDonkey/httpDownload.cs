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
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// You can download a file from a WebSite.
	/// </summary>
	/// 
	public delegate void DownloadedEventHandler(object sender, EventArgs e);

	internal class ChttpDownload
	{
		#region Instance fields
		private string m_remoteUri;
		private string m_remoteInfoUri;
		private string m_fileName;
		private string m_path;
		private string m_fullpath;
		private string m_information;
		private bool m_isDownloaded;
		private Hashtable m_myInfo;
		private string m_nodeName;
		private string m_info;
		private string[] m_infoToLog;
		private Thread m_GetInfoThread;
		private Thread m_GetInfoXMLThread;
		private Thread m_DownloadFileThread;
		#endregion
		
		#region Propertys
		
		public string information
		{
			get
			{
				return m_information;
			}
			
		}
				
		#endregion
		
		public event DownloadedEventHandler Downloaded;
		
		#region Constructors & Initialization
		
		public ChttpDownload()
		{			
		}

		~ChttpDownload()
		{
			try
			{
				if (m_GetInfoThread!=null)
					if (m_GetInfoThread.IsAlive) m_GetInfoThread.Abort();
				
				if (m_GetInfoXMLThread!=null)
					if (m_GetInfoXMLThread.IsAlive) m_GetInfoXMLThread.Abort();

				if (m_DownloadFileThread!=null)
					if (m_DownloadFileThread.IsAlive) m_DownloadFileThread.Abort();

			}
			catch{}
		}

		
		/// <summary>
		/// Download a file from a web site (server.met, ipfilter.dat,...) .
		/// </summary>
		/// <param name="remoteUri">
		/// The Uri, it must include the file name.
		/// </param>
		/// <param name="path">
		/// The local path where we save the file.
		/// </param>
		/// <param name="fileName">
		/// The file name one the file downloaded.
		/// </param>
		/// 
		public bool DownloadFile(string remoteUri,string path, string fileName, bool IsThreaded)
		{
			m_remoteUri = remoteUri;
			m_path = path;
			m_fileName = fileName;

			if(IsThreaded)
			{
				m_DownloadFileThread=new Thread(new ThreadStart(m_DownloadFile));
				m_DownloadFileThread.Name = "m_DownloadFile";
				m_DownloadFileThread.Start();
			}
			else
			{
				m_DownloadFile();
			}
			return m_isDownloaded;
		}

		private void m_DownloadFile()
		{	
			if (m_path.Length==0) m_path=Application.StartupPath;
	
			m_fullpath = Path.Combine(m_path, m_fileName);
						
			try
			{
				if(File.Exists(m_fullpath)) File.Copy(m_fullpath, m_fullpath + ".backup", true);
			}
			catch{}
			
			HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(m_remoteUri); 
			myHttpWebRequest.UserAgent = "www.lPhant.com";
			HttpWebResponse myHttpWebResponse = null;
						
			try 
			{
				CLog.Log(Constants.Log.Notify,"FIL_ADDED",m_fileName);
				myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
			}
			
			catch (WebException we)
			{
				try
				{
					if(File.Exists(m_fullpath  + ".backup")) File.Copy(m_fullpath + ".backup", m_fullpath, true);
					CLog.Log(Constants.Log.Info,we.Message);
				}
				catch{}
				
				m_isDownloaded=false;
				OnDownloaded(EventArgs.Empty);
				return;
			}

			Stream receiveStream = myHttpWebResponse.GetResponseStream();
			BinaryReader readStream = new BinaryReader(receiveStream);
		
						
			FileStream fileDownloaded;
			fileDownloaded=File.Open(m_fullpath,FileMode.Create);
			BinaryWriter BinWriter=new BinaryWriter(fileDownloaded);

			byte[] read = new byte[1024];
			int count=readStream.Read(read,0,1024);

			while (count >0) 
			{
				BinWriter.Write(read,0,count);
				count=readStream.Read(read,0,1024);
			}
	
			BinWriter.Close();
			fileDownloaded.Close();
			readStream.Close();
			myHttpWebResponse.Close();
			
			CLog.Log(Constants.Log.Info,"FIL_DOWNLOADED",m_fileName,m_remoteUri);
			
			OnDownloaded(EventArgs.Empty);
			m_isDownloaded=true;
		}

		protected virtual void OnDownloaded(EventArgs e) 
		{
			if (Downloaded != null)
				Downloaded(this, e);
		}

		
		/// <summary>
		/// Get informations from a web site. You can receive lphant news.
		/// </summary>
		/// <param name="remoteUri">
		/// The Uri of the information file.
		public void GetInfo(string remoteInfoUri)
		{
			m_remoteInfoUri = remoteInfoUri;
			m_GetInfoThread=new Thread(new ThreadStart(m_GetInfo));
			m_GetInfoThread.Name = "m_GetInfo";
			m_GetInfoThread.Start();
		}

		private void m_GetInfo()
		{
			try 
			{
				WebClient myWebClient = new WebClient();
				Stream uriStream = myWebClient.OpenRead(m_remoteInfoUri);
				StreamReader sr = new StreamReader(uriStream);
				m_information = sr.ReadToEnd();
				CLog.Log(Constants.Log.Info,m_information);
				uriStream.Close();
			}
			catch (WebException we)
			{
				CLog.Log(Constants.Log.Info,we.Message);
			}
		}

		/*XML example file.
		<?xml version="1.0" encoding="UTF-8"?>
		<info> 
		<version>lphant-v0.01-pré5</version> 
		<log>
		-----------
		*lphant-v0.01-pre5
		*lphant-v0.01-pre5
		*lphant-v0.01-pre5
		*lphant-v0.01-pre5
		-----------
		http://www.lphant.com
		</log>
		<statusbar>lphant-v0.01-pre5</statusbar> 
		<messagebox>xxx xxx xx xxxxxx</messagebox> 
		</info>
		
		string[] myLog = new string[] {"log","version"};
		httpDownload.GetInfoXML(@"http://localhost/lphant/xml.xml",mylog,true);
		CLog.Log(Constants.Log.Info,httpDownload.getReadInfoFromXML("log"));
		 */

		/// <summary>
		/// Get informations from a web site. You can receive lphant news.
		/// </summary>
		/// <param name="remoteUri">
		/// The Uri of the information file.
		/// <param name="infoToLog">
		/// Info to log when the methos is used.
		/// <param name="IsThreaded">
		/// This method is used in a thread.
		public void GetInfoXML(string remoteInfoUri,string[] infoToLog, bool IsThreaded)
		{
			m_remoteInfoUri = remoteInfoUri;
			m_infoToLog = infoToLog;
			if(IsThreaded)
			{
				m_GetInfoXMLThread=new Thread(new ThreadStart(m_GetInfoXML));
				m_GetInfoXMLThread.Name = "m_GetInfoXML";
				m_GetInfoXMLThread.Start();
			}
			else
			{
				m_GetInfoXML();
			}
		}
				
		private void m_GetInfoXML()
		{	
			XmlTextReader reader = null;

			try 
			{      
				reader = new XmlTextReader(m_remoteInfoUri);
				m_myInfo = new Hashtable();
				
				while (reader.Read()) 
				{
					switch (reader.NodeType) 
					{
						case XmlNodeType.Element:
							m_nodeName = reader.Name;
							break;
						case XmlNodeType.Text:
							m_myInfo.Add(m_nodeName,reader.Value);
							break;
						case XmlNodeType.CDATA:
							break;
						case XmlNodeType.ProcessingInstruction:
							break;
						case XmlNodeType.Comment:
							break;
						case XmlNodeType.XmlDeclaration:
							break;
						case XmlNodeType.Document:
							break;
						case XmlNodeType.DocumentType:
							break;
						case XmlNodeType.EntityReference:
							break;
						case XmlNodeType.EndElement:
							break;
					}       
				}           
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			finally 
			{
				if (reader!=null)
					reader.Close();
			}
			
			//display info to log. Usefull if threaded
			if(m_infoToLog!=null)
			{
				foreach(string info in m_infoToLog)
				{
					if(info != null)
					{	
						m_information = getReadInfoFromXML(info);
						CLog.Log(Constants.Log.Info,m_information);
					}
				}
			}
		}

		public string getReadInfoFromXML(string info)
		{	
			m_info = info;
			try
			{	
				if(m_myInfo==null || m_myInfo.Count==0) return "";
				return m_myInfo[m_info].ToString();
			}
			catch(Exception e)
			{	
				Debug.WriteLine(e.Message);
				return "";
			}
		}
		#endregion
	}
}
