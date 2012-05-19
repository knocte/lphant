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
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	internal class CElement
	{
		public CFile File;
		public CSourcesList SourcesList;
		public CedonkeyComments Comments;
		public CFileStatistics Statistics;
	}

	internal class CKnownFileAttributes
	{
		public string FileName;
		public DateTime Date;
		public uint Size;
		public Constants.Priority Priority;
		public byte Permisions;
		public ArrayList HashSet;
		public byte[] FileHash;
		public string Comment;
		public byte Rating;
	}

	/// <summary>
	/// Summary description for CFilesList.
	/// </summary>
	internal class CFilesList
	{
		#region Instance fields
		private ArrayList m_FileList;
		private float m_GlobalDownSpeed;
		private Hashtable m_KnownMetList;
		private Thread m_FileListThread;
		private CHash m_EdonkeyHash;
		private int m_relayAdjust;
		private int m_OSigInterval;
		//private bool m_SharedLoaded;
		private bool m_IncompleteLoaded;
		#endregion

		#region Propertys
		public CElement this[byte[] key]
		{
			get
			{
				try
				{
					CElement result=null;
					lock(m_FileList.SyncRoot)
					{
						foreach (CElement element in m_FileList)
						{
							if (CKernel.SameHash(ref key, ref element.File.FileHash))
							{
								result=element;
								break;
							}
						}
					}
					return result;
				}
				catch
				{
					return null;
				}
			}
		}

		public CElement this[int index]
		{
			get
			{
				return (CElement)m_FileList[index];
			}
		}

		/*public CElement this[string strKey]
		{
			get
			{
				byte[] key=CKernel.StringToHash(strKey);
				foreach (CElement element in m_FileList)
				{
					if (CKernel.SameHash(ref key, ref element.File.FileHash))
						return element;
				}
				return null;
			}
		}*/

		public int Count 
		{
			get
			{
				return m_FileList.Count;
			}
		}

		public ArrayList Values
		{
			get
			{
				return m_FileList;
			}
		}

		public float GlobalDownSpeed
		{
			get
			{
				return m_GlobalDownSpeed;
			}
		}
		#endregion

		#region Constructors & Initialization
		
		public CFilesList()
		{
			m_relayAdjust=5;
			m_FileList=ArrayList.Synchronized(new ArrayList());
			//m_FileListThread=new System.Threading.Timer(new TimerCallback(m_ProcessFiles),null,1000/*rnd.Next(60)*1000*/,1000);
			
			m_FileListThread=new Thread(new ThreadStart(m_ProcessFiles));
			m_FileListThread.Name="FilesThread";
			m_FileListThread.Start();
			
			m_IncompleteLoaded=false;
			m_OSigInterval=0;
		}
		#endregion

		public bool SaveList(string filefullpath)
		{
			FileStream knownmet;
			BinaryWriter writer;
			int nFiles=0;
			if (filefullpath.Length==0)
				filefullpath=Path.Combine(CKernel.DllDirectory, "known.met");
			
			if ((m_KnownMetList==null)||((m_KnownMetList!=null)&&(m_KnownMetList.Count==0)))
				m_LoadKnownFiles("",false);
			
			try
			{
				knownmet=File.Open(filefullpath,FileMode.Create,FileAccess.Write,FileShare.None);
			}
			catch 
			{return false;}
			writer=new BinaryWriter(knownmet);
			writer.Write((byte)Protocol.KnownMet.eDonkey);
			writer.Write(this.Count);
			//save old known.met files, usefull if a unshared file is shared again
			if (m_KnownMetList!=null)
			{
				foreach (CKnownFileAttributes KnwonFile in m_KnownMetList.Values)
				{
					if (KnwonFile.FileHash==null) continue;//we dont have the hash
					if (this[KnwonFile.FileHash]!=null) continue;
					nFiles++;
					writer.Write((uint)(((long)KnwonFile.Date.Ticks-621355968000000000L)/10000000L));
					writer.Write(KnwonFile.FileHash);
					writer.Write((ushort)KnwonFile.HashSet.Count);
					for (int i=0;i!=KnwonFile.HashSet.Count;i++)
					{
						writer.Write((byte[])KnwonFile.HashSet[i]);
					}
					uint nParametros=4;
					if (KnwonFile.Rating>0) nParametros++;
					if ((KnwonFile.Comment!=null)&&(KnwonFile.Comment!="")) nParametros++;
					writer.Write(nParametros);

					new ParameterWriter((byte)Protocol.FileTag.Name, KnwonFile.FileName, writer);
					new ParameterWriter((byte)Protocol.FileTag.Size, KnwonFile.Size, writer);
					new ParameterWriter((byte)Protocol.FileTag.Priority, (byte)KnwonFile.Priority, writer);
					new ParameterWriter((byte)Protocol.FileTag.Permissions, 0, writer);
					
					if (KnwonFile.Rating>0)
						new ParameterWriter((byte)Protocol.FileTag.Rating, KnwonFile.Rating, writer);
					if ((KnwonFile.Comment!=null)&&(KnwonFile.Comment!="")) 
						new ParameterWriter((byte)Protocol.FileTag.Comment, KnwonFile.Comment, writer);
				}
				m_KnownMetList.Clear();
			}
			//save current shared files
			foreach (CElement Element in this.Values)
			{
				if (Element.File.FileHash==null) continue;//we dont have the hash
				if (!Element.File.Completed) continue;
				nFiles++;
				writer.Write((uint)(((long)Element.File.FileDate.Ticks-621355968000000000L)/10000000L));
				writer.Write(Element.File.FileHash);
				writer.Write((ushort)Element.File.HashSet.Count);
				for (int i=0;i!=Element.File.HashSet.Count;i++)
				{
					writer.Write((byte[])Element.File.HashSet[i]);
				}
				uint nParametros=4;
				if (Element.File.Rating>0) nParametros++;
				if ((Element.File.Comment!=null)&&(Element.File.Comment!="")) nParametros++;
				writer.Write(nParametros);

				new ParameterWriter((byte)Protocol.FileTag.Name, Element.File.FileName, writer);
				new ParameterWriter((byte)Protocol.FileTag.Size, Element.File.FileSize, writer);
				new ParameterWriter((byte)Protocol.FileTag.Priority, (byte)Element.File.UpPriority, writer);
				new ParameterWriter((byte)Protocol.FileTag.Permissions, 0, writer);
				
				if (Element.File.Rating>0)
					new ParameterWriter((byte)Protocol.FileTag.Rating, Element.File.Rating, writer);
				if ((Element.File.Comment!=null)&&(Element.File.Comment!="")) 
					new ParameterWriter((byte)Protocol.FileTag.Comment, Element.File.Comment, writer);
			}
			writer.Seek(1,SeekOrigin.Begin);
			writer.Write(nFiles);
			writer.Close();
			knownmet.Close();
			return true;
		}
		
		private bool m_LoadKnownFiles(string filefullpath,bool showErrorMessage)
		{
			FileStream knownmet=null;
			BinaryReader reader=null;
			if (filefullpath.Length==0)
				filefullpath=Path.Combine(CKernel.DllDirectory, "known.met");
			try
			{
				knownmet=File.OpenRead(filefullpath);

				reader=new BinaryReader(knownmet);

				if ( !Enum.IsDefined( typeof(Protocol.KnownMet), reader.ReadByte() ) )
					//	if (reader.ReadByte() != 0x0E)
				{
					reader.Close();
					knownmet.Close();
					CLog.Log(Constants.Log.Notify,"KNWMET_INVALID");
					return false;
				}
				int nFiles=reader.ReadInt32();
				m_KnownMetList=new Hashtable(nFiles);
				for (uint i=0;i!=nFiles;i++)
				{
					CKnownFileAttributes attributesKnownFile=new CKnownFileAttributes();
					attributesKnownFile.Date=new DateTime(((long)reader.ReadUInt32()*10000000L)+621355968000000000L);
					attributesKnownFile.FileHash=reader.ReadBytes(16);
					ushort nChunks=reader.ReadUInt16();
					attributesKnownFile.HashSet=new ArrayList(nChunks);
					for (uint chunk=0;chunk!=nChunks;chunk++)
					{
						attributesKnownFile.HashSet.Add(reader.ReadBytes(16));
					}
					CParameterReader ParameterReader; 
					uint nparameters=reader.ReadUInt32();
					for (uint parameter=0;parameter!=nparameters;parameter++)
					{
						ParameterReader=new CParameterReader(reader);
						switch( (Protocol.FileTag)ParameterReader.id )
						{
							case Protocol.FileTag.Name:
								attributesKnownFile.FileName=ParameterReader.valorString;
								break;
							case Protocol.FileTag.Size:
								attributesKnownFile.Size=ParameterReader.valorNum;
								break;
							case Protocol.FileTag.Priority:
								attributesKnownFile.Priority=(Constants.Priority)ParameterReader.valorNum;
								break;
							case Protocol.FileTag.Permissions:
								attributesKnownFile.Permisions=(byte)ParameterReader.valorNum;
								break;
							case Protocol.FileTag.Comment:
								attributesKnownFile.Comment=ParameterReader.valorString;
								break;
							case Protocol.FileTag.Rating:
								attributesKnownFile.Rating=(byte)ParameterReader.valorNum;
								break;
							default: 
								break;
								//faltan los parametros para las estadisticas....
						}
					}

					if (!m_KnownMetList.ContainsKey(attributesKnownFile.FileName+(attributesKnownFile.Date.Ticks/10000000L).ToString()))
						m_KnownMetList.Add(attributesKnownFile.FileName+(attributesKnownFile.Date.Ticks/10000000L).ToString(),attributesKnownFile);
				}

				reader.Close();
				knownmet.Close();
				return true;
			}
			catch(Exception e) 
			{
				Debug.WriteLine(e.ToString());
				if (showErrorMessage) CLog.Log(Constants.Log.Notify,"Can not load known.met, hashing shared files");
				try
				{
					if (reader!=null) reader.Close();
					if (knownmet!=null) knownmet.Close();
				}
				catch{};
				return false;
			}
		}

		private CKnownFileAttributes m_IsKnown(string name,DateTime date)
		{
			if ((m_KnownMetList==null)||(m_KnownMetList.Count==0)) return null;
			CKnownFileAttributes attributesKnownFile=(CKnownFileAttributes)m_KnownMetList[name+(date.Ticks/10000000L).ToString()];
			if (attributesKnownFile!=null) 
				return attributesKnownFile;
			return null;
		}

		public void RemoveSharedFileIfNotExists(byte[] fileHash)
		{
			CElement Element=this[fileHash];
			if ((Element!=null)&&(Element.File!=null)&&(Element.File.Completed)&&(!File.Exists(Element.File.CompleteName)))
			{
				m_FileList.Remove(Element);
			
				ArrayList toRemove=null;
				foreach (CClient Client in CKernel.Queue.List)
				{
					if (Client.UploadElement==Element)
					{
						if (toRemove==null) toRemove=new ArrayList();
						toRemove.Add(Client);
					}

				}
				if (toRemove!=null)
				{
					foreach (CClient Client in toRemove)
					{
						CKernel.Queue.List.Remove(Client);
						//Debug.WriteLine(Client.UserName+" cleaned from queue due to deleted file");
						CKernel.ClientsList.IsClientNeeded(Client);
					}
				}
			}
		}
		
		//Clear deleted or deleted incoming folders
		private void m_ClearDeletedFiles()
		{
			ArrayList toRemove=null;
			string IncomingFolder = String.Join(":",CKernel.Preferences.GetStringArray("SharedFolders"))+":";
			
			lock (m_FileList.SyncRoot)
			{
				foreach (CElement Element in m_FileList)
				{	
					string m_CompleteName = Element.File.CompleteName;
					if ((Element.File.Completed) 
						&& (!File.Exists(m_CompleteName)||(IncomingFolder.IndexOf(Path.GetDirectoryName(m_CompleteName)+":")==-1)))
					{	
						if (toRemove==null) toRemove=new ArrayList();
						toRemove.Add(Element);
					}
				}
			}
			if (toRemove!=null)
			{
				foreach (CElement Element in toRemove)
				{
					//Debug.WriteLine("File deleted from FilesList: "+ Element.File.CompleteName);
					m_FileList.Remove(Element);
					CKernel.RemovedSharedFile(Element);
				}
			}
		}
		
		private bool m_LoadFilesFromFolder(string folder/*,bool refreshing*/)
		{
			if (!Directory.Exists(folder)) return false;
			string [] ficheros=Directory.GetFiles(folder);
			DateTime date;
			string name;
			if (m_EdonkeyHash==null) m_EdonkeyHash=new CHash();
			foreach (string file in ficheros)
			{	
				// 0 Kb file are ignored, avoid a crash
				FileInfo filInfo = new FileInfo(file);
				if((filInfo.Length == 0)||
					((filInfo.Attributes&(FileAttributes.Hidden|FileAttributes.System))!=0)) continue;

				date=File.GetLastWriteTime(file).ToUniversalTime();
				name=Path.GetFileName(file);
				CKnownFileAttributes Atributos;
				if ((Atributos=m_IsKnown(name,date))!=null)
				{
					//if (refreshing) continue;
					if (!m_Contains(Atributos.FileHash))
					{
					CElement Element=new CElement();
					Element.File=new CFile(Atributos.FileHash,Atributos.Date,folder,name,Atributos.Size, Atributos.Priority, Atributos.Permisions, Atributos.HashSet, Atributos.Comment, Atributos.Rating);
					Element.SourcesList=null;
					Element.Statistics=new CFileStatistics();
					this.m_FileList.Add(Element);
					CKernel.NewSharedFile(Element);
					}
					//if (!m_Contains(Atributos.FileHash))this.m_FileList.Add(Element);
				}
				else //new file
				{
					m_EdonkeyHash.AddFile(file);
				}
			}
			return true;
		}

		private bool m_Contains(byte[] fileHash)
		{
			foreach (CElement element in m_FileList)
			{
				if (CKernel.SameHash(ref fileHash, ref element.File.FileHash))
					return true;
			}
			return false;
		}

		public void AddFile(string name, uint size, byte[] Hash, stDatosFuente[] sources)
		{
			CElement Element;
			if (this.m_Contains(Hash))
			{
				CLog.Log(Constants.Log.Notify,"FIL_DUP",name);
				Element=(CElement)this[Hash];
			}
			else
			{
				CFile file=new CFile(Hash,name,size);
				Element=new CElement();
				Element.File=file;
				Element.Statistics=new CFileStatistics();
				m_FileList.Add(Element);
				CKernel.NewFile(Element);
				CLog.Log(Constants.Log.Notify,"FIL_ADDED",name);
			}
			if ((Element.SourcesList==null)&&((Element.File.FileStatus==Protocol.FileState.Ready)||(Element.File.FileStatus==Protocol.FileState.Completing)))
			{
				CSourcesList sourcesList=new CSourcesList(Element);
				Element.SourcesList=sourcesList;
				if (CKernel.ServersList.ActiveServer!=null) CKernel.ServersList.ActiveServer.RequestSources(Element.File.FileHash);
			}
			if ((Element.SourcesList!=null)&&
				((Element.File.FileStatus==Protocol.FileState.Ready)||(Element.File.FileStatus==Protocol.FileState.Completing))&&
				(sources!=null))
				CKernel.ClientsList.AddClientsToFile(sources,Hash);

		}

		public void AddFile(string fichero, byte[] Hash, ArrayList HashSet)
		{
			FileInfo info= new FileInfo(fichero);
			CFile Fichero=new CFile(Hash,File.GetLastWriteTime(fichero).ToUniversalTime(),Path.GetDirectoryName(fichero),Path.GetFileName(fichero),(uint)info.Length,Constants.Priority.Normal,0, HashSet,"",0);
			AddFile(Fichero);
			SaveList("");
		}

		private void AddFile(CFile file)
		{
			if (!m_Contains(file.FileHash))
			{
				CElement Element=new CElement();
				Element.File=file;
				Element.Statistics=new CFileStatistics();
				m_FileList.Add(Element);
				if (((file.FileStatus==Protocol.FileState.Ready)||(Element.File.FileStatus==Protocol.FileState.Completing))&&(!file.Completed))
				{
					Element.SourcesList=new CSourcesList(Element);
					Element.Comments=new CedonkeyComments(Element);
				}
				if (!file.Completed) 
					CKernel.NewFile(Element);
				else
					CKernel.NewSharedFile(Element);
			}
			else
			{
				CLog.Log(Constants.Log.Notify, "FIL_DUP",CKernel.HashToString(file.FileHash));
				file=null;
			}
		}

		public void LoadEnded()
		{
			SaveList("");
			if ((CKernel.ServersList.ActiveServer!=null)&&(m_IncompleteLoaded))
				CKernel.ServersList.ActiveServer.PublishSharedFiles();
		}

		public void LoadSharedFiles()
		{
			m_IncompleteLoaded=false;			
			m_LoadKnownFiles("",true);
			LoadIncompletedFiles();
			if (CKernel.Preferences.GetStringArray("SharedFolders") != null)
			foreach (string folder in CKernel.Preferences.GetStringArray("SharedFolders"))
			{
				m_LoadFilesFromFolder(folder/*,false*/);
			}
			if (m_KnownMetList!=null) m_KnownMetList.Clear();
			if ((m_EdonkeyHash!=null)&&(m_EdonkeyHash.Files.Count>0)) m_EdonkeyHash.DoHashes();
				else LoadEnded();
			m_KnownMetList=null;
			m_IncompleteLoaded=true;
		}

		public void RefreshSharedList()
		{
			if ((m_EdonkeyHash!=null)&&(m_EdonkeyHash.Hashing)) return; //don't allow add files while hashing
			m_ClearDeletedFiles();
			m_LoadKnownFiles("",true);
			foreach (string folder in CKernel.Preferences.GetStringArray("SharedFolders"))
			{
				m_LoadFilesFromFolder(folder/*,true*/);
			}
			if (m_KnownMetList!=null) m_KnownMetList.Clear();
			if ((m_EdonkeyHash!=null)&&(m_EdonkeyHash.Files.Count>0)) m_EdonkeyHash.DoHashes();
				else LoadEnded();
			m_KnownMetList=null;
			if (CKernel.ServersList.ActiveServer!=null) CKernel.ServersList.ActiveServer.AllFilesPublished=false;
		}
		
		public void EmptyFileToSharedFile(CFile file)
		{
			CElement Element=this[file.FileHash];
			if (Element!=null) CKernel.NewSharedFile(Element);
			//TODO publish the file on the conected server
			if (CKernel.ServersList.ActiveServer!=null) CKernel.ServersList.ActiveServer.PublishSharedFile(Element);
		}

		public void DeleteFile(string strFileHash)
		{
			CElement Element=(CElement)this[CKernel.StringToHash(strFileHash)];	
			if ((Element!=null)&&(Element.File.FileStatus==Protocol.FileState.Complete))
			{	

				if (Element.SourcesList!=null) m_StopFile(Element);
				if (Element.Comments!=null) Element.Comments.Element=null;
				Element.Comments=null;

				try
				{
					File.Delete(Element.File.CompleteName);
				}
				catch{}
				
				Element.File=null;
				m_FileList.Remove(Element);
				Element=null;
			}
		}

		public void LoadIncompletedFiles()
		{
			foreach (string folder in CKernel.Preferences.GetStringArray("TempFolders"))
			{
				try
				{
					string [] files=Directory.GetFiles(folder);
					foreach (string strfile in files)
					{
						if (Path.GetExtension(strfile).Equals(".part"))
						{
							CFile file=new CFile(Path.GetDirectoryName(strfile),Path.GetFileName(strfile));
							if (file.FileHash!=null) AddFile(file); //file loaded properly
						}
					}
				}
				catch (IOException e)
				{
					// any temp directory don't exist
					Debug.Write( e.ToString() );
				}
			}
		}

		public void AddNewIncompleteFolder(string folder)
		{
			try
			{
				string [] files=Directory.GetFiles(folder);
				foreach (string strfile in files)
				{
					if (Path.GetExtension(strfile).Equals(".part"))
					{
						CFile file=new CFile(Path.GetDirectoryName(strfile),Path.GetFileName(strfile));
						if (file.FileHash!=null) AddFile(file); //file loaded properly
					}
				}
			}
			catch (IOException e)
			{
				// any temp directory don't exist
				Debug.Write( e.ToString() );
			}
		}

		public void RemoveIncompleteFolder(string folder)
		{
			ArrayList toStop=new ArrayList();
			lock(m_FileList.SyncRoot)
			{
				foreach (CElement element in m_FileList)
				{
					if ((element.File.Completed==false)&&(Path.GetDirectoryName(element.File.CompleteName)==folder))
					{
						Debug.WriteLine("To remove:"+element.File.CompleteName);
						toStop.Add(element);
					}
				}
			}
			foreach (CElement element in toStop)
			{
				m_StopFile(element);
				element.File.CloseFile();
				element.File=null;
				m_FileList.Remove(element);
			}
		}

		public void AddSource(byte[] FileHash,ref CClient client)
		{
			CElement Element=(CElement)this[FileHash];
			if ((Element!=null)&& (Element!=null)&&(!Element.File.Completed)) Element.SourcesList.AddSource(client);			
		}

		public void CancelFilesHashing()
		{
			if (m_EdonkeyHash!=null) m_EdonkeyHash.KillThread();
		}

		private void m_StopFile(CElement Element)
		{
			if ((Element!=null)&& (Element.SourcesList!=null))
			{
				Element.SourcesList.Clean();
				Element.SourcesList=null;
			}
		}

		public void StopFile(byte[] FileHash)
		{
			CElement Element=(CElement)this[FileHash];			
			if ((Element!=null)&&(Element.File.FileStatus==Protocol.FileState.Ready))
			{
				m_StopFile(Element);
				Element.File.FileStatus=Protocol.FileState.Stopped;
			}
		}

		public void CompleteFile(string strFileHash)
		{
			CElement Element=(CElement)this[CKernel.StringToHash(strFileHash)];			
			if (Element!=null)
			{
				m_StopFile(Element);
				Element.File.FileStatus=Protocol.FileState.Complete;
				Element.File.UpPriority=Constants.Priority.Normal;
				CLog.Log(Constants.Log.Notify,"FIL_COMPLETED",Element.File.FileName);
				Element.Statistics.TimeCompleted=DateTime.Now;
				if (CKernel.Preferences.GetBool("StartNextStoppedFile")) m_StartNextPausedFile(Element.File.CategoryID);
			}
		}

		private void m_StartNextPausedFile(uint category)
		{
			CElement elementCandidate=null;
			lock(m_FileList.SyncRoot)
			{
				foreach (CElement element in m_FileList)
				{
					if ((element.File.FileStatus==Protocol.FileState.Stopped)||
						(element.File.FileStatus==Protocol.FileState.Pause))
					{
						if (elementCandidate==null) //no one selected, select first
							elementCandidate=element;
						else if (element.File.CategoryID==category) //same category
						{
							if (elementCandidate.File.CategoryID!=category) //selected from other category
								 elementCandidate=element;
							else if (elementCandidate.File.DownPriority<element.File.DownPriority) //selected from same category lower priority
								elementCandidate=element;

						}
						else if ((elementCandidate.File.CategoryID!=category)&&(elementCandidate.File.DownPriority<element.File.DownPriority))
								elementCandidate=element;

					}
				}
			}
			if (elementCandidate!=null)
			{
				this.ResumeFile(CKernel.HashToString(elementCandidate.File.FileHash));
			}
		}

		public void CloseDownloads(string strFileHash, bool stop)
		{
			if (strFileHash.Length==0) //stop all downloads
			{
				foreach (CElement Elemento in this.Values)
				{
					if (Elemento.SourcesList!=null) Elemento.SourcesList.Clean();
					Elemento.SourcesList=null;
				}
				if (stop) m_FileListThread.Abort();
			}
			else  //stop specified download
			{
				CElement Elemento=(CElement)this[CKernel.StringToHash(strFileHash)];
				m_StopFile(Elemento);
			}
		}

		public void ResumeFile(string strFileHash)
		{
			CElement Elemento=(CElement)this[CKernel.StringToHash(strFileHash)];
			if ((Elemento!=null)&& (Elemento.SourcesList==null)&&((Elemento.File.FileStatus==Protocol.FileState.Stopped)||(Elemento.File.FileStatus==Protocol.FileState.Pause)))
			{
				Elemento.SourcesList=new CSourcesList(Elemento);
				Elemento.File.FileStatus=Protocol.FileState.Ready;
				if (Elemento.Comments==null) Elemento.Comments=new CedonkeyComments(Elemento);
				Elemento.Statistics.ResetStartTime();
				lock (CKernel.Queue.List.SyncRoot)
				{
					foreach (CClient Client in CKernel.Queue.List)
					{
						if (Client.UploadElement==Elemento)
							CKernel.ClientsList.AddClientToFile(Client, Elemento.File.FileHash);
					}
					if (CKernel.ServersList.ActiveServer!=null) CKernel.ServersList.ActiveServer.RequestSources(Elemento.File.FileHash);
				}
			}
		}

		public void CancelFile(string strFileHash)
		{
			CElement Element=(CElement)this[CKernel.StringToHash(strFileHash)];	
			if ((Element!=null)&&(Element.File.FileStatus!=Protocol.FileState.Complete))
			{
				Element.File.FileStatus=Protocol.FileState.Stopped;
				if (Element.SourcesList!=null) m_StopFile(Element);
				if (Element.Comments!=null) Element.Comments.Element=null;
				Element.Comments=null;
				Element.File.CancelFile();
				Element.File=null;
				m_FileList.Remove(Element);
				Element=null;
			}
		}

		public void SetDownloadPriority(string strFileHash, Constants.Priority priority)
		{
			CElement ChangedElement=(CElement)this[CKernel.StringToHash(strFileHash)];
			if ((ChangedElement==null)||(ChangedElement.File.DownPriority==priority)||(ChangedElement.File.FileStatus==Protocol.FileState.Complete)) return;
			ChangedElement.File.DownPriority=(Constants.Priority)priority;
			CKernel.ClientsList.SwapClientsToHigherPriorityFile();
		}

		public void SetFileCategory(string strFileHash,string category)
		{
			CElement Element=(CElement)this[CKernel.StringToHash(strFileHash)];			
			if (Element!=null)
			{
				CCategory cat=CKernel.CategoriesList.GetCategoryByName(category);
				if (cat!=null) 
					Element.File.CategoryID=cat.ID;
				else
					Element.File.CategoryID=0;
			}
		}

		public void UnMarkFilesWithCategory(uint catID)
		{
			lock(m_FileList.SyncRoot)
			{
				foreach (CElement Elemento in m_FileList)
				{
					if (Elemento.File.CategoryID==catID) Elemento.File.CategoryID=0;
				}
			}
		}

		public void CheckAllFilesForAllCategories()
		{
			lock(m_FileList.SyncRoot)
			{
				foreach (CElement Elemento in m_FileList)
				{
					if ((!Elemento.File.Completed)&&(Elemento.File.CategoryID==0))
					{
						Elemento.File.CategoryID=CKernel.CategoriesList.GetAutoCategory(Elemento.File.FileName);
					}
				}
			}
		}

		//private void m_ProcessFiles(Object state)
		private void m_ProcessFiles()
		{
			DateTime startTime;
			float velocidadTotal=0;
			TimeSpan timeUsed;
			int timeSleep;
			//m_FileListThread.Change(Timeout.Infinite,Timeout.Infinite);
			
			while (m_FileListThread.IsAlive)
			{
				startTime=DateTime.Now;
				CKernel.ConnectionsList.CheckTimeOuts();
				if (m_OSigInterval>=30) 
				{
					CKernel.OSig("");
					m_OSigInterval=0;
				}
				else
					m_OSigInterval++;
				
				CKernel.SourcesOld.CleanOldSources();
				velocidadTotal=0;
				lock(m_FileList.SyncRoot)
				{
					foreach (CElement Elemento in m_FileList)
					{
						if (Elemento.SourcesList!=null) Elemento.SourcesList.Process();
						if (Elemento.SourcesList!=null) velocidadTotal+=Elemento.SourcesList.GetDownloadSpeed();
					}
				}
				m_GlobalDownSpeed=velocidadTotal;
			
				m_relayAdjust--;
				if (m_relayAdjust==0) 
				{
					m_relayAdjust=5;
					AdjustSpeed();			
				}
				timeUsed=DateTime.Now-startTime;
				if (timeUsed.TotalMilliseconds>1000)
				{
					timeSleep=1;
				}
				else
				{
					timeSleep=1000-(int)timeUsed.TotalMilliseconds;
				}				
				Thread.Sleep(timeSleep);
				
//				if (timeUsed.TotalMilliseconds>1000)
//					m_FileListThread.Change(1,Timeout.Infinite);
//				else
//					m_FileListThread.Change(1000-(int)timeUsed.TotalMilliseconds,Timeout.Infinite);
			}
		}

		private void AdjustSpeed()
		{
			float variation=0;
			variation=(CKernel.Preferences.GetFloat("MaxDownloadRate"))/(m_GlobalDownSpeed+0.001F);
			lock(m_FileList.SyncRoot)
			{
				float maxDownRate=CKernel.Preferences.GetFloat("MaxDownloadRate")*1024F*0.20F;
				foreach (CElement Element in m_FileList)
				{
					if ((Element.SourcesList!=null)&&(Element.SourcesList.GetDownloadSpeed()>0))
					{
						lock(Element.SourcesList.SyncRoot())
						{
							foreach (CClient Client in Element.SourcesList)
							{
								if (Client.DownloadState==Protocol.DownloadState.Downloading)//&&(!Client.estaEsperando))
								{
									if (Client.connection==null) continue;
									int newLimit=(int)((float)Client.connection.DownloadLimit * variation);
									if (newLimit<10) newLimit=10;
									if (newLimit>maxDownRate)
										newLimit=(int)(maxDownRate);
									if (Client.connection!=null) Client.connection.DownloadLimit=newLimit;
									//Debug.Write("Spd:"+Convert.ToString(m_GlobalDownSpeed)+" Cli:"+Convert.ToString(Client.DownloadSpeed)+" Var:"+Convert.ToString(variation)+" Lim:"+Convert.ToString(newLimit)+"\n");
								}
							}
						}
					}
				}
			}
		}
	}
}