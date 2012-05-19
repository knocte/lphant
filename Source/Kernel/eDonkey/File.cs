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
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions; 
using eLePhant.Types;

namespace eLePhant.eDonkey
{

	/// <summary>
	/// Description for CFile.
	/// </summary>
	/// 
	internal class CFile
	{
		public byte[] FileHash;

		#region Instance fields
		private Thread m_CompleteFileThread;
		private ArrayList m_HashSet;
		private ArrayList m_CrumbsHashSet;
		private byte[] m_ChunksStatus;
		private bool m_Completed=false;
		private string m_FileName;
		private string m_PartFileName; //si el fichero no esta completo... nombre del fichero real
		private string m_FileFolder;
		private string m_CompleteName;
		private string m_MetFile="";
		private string m_CompleteNameMet="";
		private uint m_FileSize;
		private uint m_Transferred;
		private uint m_LastSeenComplete;
		private uint m_FileType;
		private ushort m_NumChunks;
		private DateTime m_FileDate;

		private Protocol.FileState m_FileStatus;
		private Constants.Priority m_DownPriority;
		private Constants.Priority m_UpPriority;

		//private byte permisos;
		private bool m_isReadingFile=false; //indica si en ese momento se esta leyendo el fichero
		private bool m_isWritingFile=false; //indica si en ese momento se esta escribiendo en el fichero
		private	FileStream m_MyFile;
		private bool m_LoadedFile= false; //cargado el fichero para escritura??
		private bool m_ReadingReady= false; //cargado el fichero para lectura??
		private bool m_isEmpty=true; // indica que no hay ninguna parte completa
		private SortedList m_Gaps; //los gaps se almacenan como una lista ordenada de con clave inicio y valor final
		private SortedList m_RequestingBlocks;
		private ArrayList m_UnknownParameters;
		private string m_MyComment;
		private byte m_MyRating;
		private ArrayList m_CorruptedChunks;
		private CFileDetails m_FileDetails;
		private Protocol.PartMet m_FileFormat;

		private uint m_CategoryID;
		private uint m_MaxSources;

		//		private sFileInfo Info;
		#endregion

		#region Propertys
		public sFileDetails Details
		{
			get
			{
				return (sFileDetails)m_FileDetails.Details;
			}
		}
		public bool Empty
		{
			get
			{
				return m_isEmpty;
			}
		}

		public bool Completed
		{
			get
			{
				return m_Completed;
			}
		}

		public string FileName
		{
			get
			{
				return m_FileName;
			}
			set
			{
				if (!m_Completed) 
				{
					m_FileName=value;
					m_FileDetails.FileName=m_FileName;
				}
				else
				{
					string dir = Path.GetDirectoryName(m_CompleteName);
					File.Move(m_CompleteName, Path.Combine(dir, value));
					m_FileName=value;
					m_FileDetails.FileName=m_FileName;
					m_CompleteName=Path.Combine(dir, value);
					CKernel.FilesList.SaveList("");
				}
			}
		}

		public string CompleteName
		{
			get
			{
				return m_CompleteName;
			}
		}

		public byte[] ChunksStatus
		{
			get
			{
				return m_ChunksStatus;
			}
		}

		public Protocol.FileState FileStatus
		{
			get
			{
				return m_FileStatus;
			}
			set
			{
				m_FileStatus=value;
				if (value!=Protocol.FileState.Complete) SaveMet();
			}
		}

		public uint Transferred
		{
			get
			{
				return m_Transferred;
			}
		}

		public ArrayList CrumbsHashSet
		{
			get
			{
				return m_CrumbsHashSet;
			}
			set
			{
				m_CrumbsHashSet=value;
			}
		}

		public ArrayList HashSet
		{
			get
			{
				return m_HashSet;
			}
			set
			{
				if ((m_NumChunks==0) && (value!=null))
				{
					m_NumChunks	=(ushort)value.Count;
					m_ChunksStatus=new Byte[m_NumChunks];				
					for (int i=0; i!=m_ChunksStatus.Length; i++)
					{
						m_ChunksStatus[i]=(byte)Protocol.ChunkState.Empty;
					}
					m_HashSet=value;
				}
			}
		}

		public SortedList Gaps
		{
			get { return m_Gaps; }
		}

		public SortedList RequestingBlocks
		{
			get { return m_RequestingBlocks; }
		}

		public uint FileType
		{
			get { return m_FileType; }
		}

		public uint FileSize
		{
			get { return m_FileSize; }
		}

		public ushort NumChunks
		{
			get { return m_NumChunks; }
		}

		public DateTime FileDate
		{
			get { return m_FileDate; }
		}

		public Constants.Priority UpPriority
		{
			get { return m_UpPriority; }
			set { m_UpPriority=value; }
		}

		public Constants.Priority DownPriority
		{
			get { return m_DownPriority; }
			set { m_DownPriority=value; }
		}

		public string Comment
		{
			get { return m_MyComment; }
			set
			{
				m_MyComment=value;
				if (!m_Completed) SaveMet();
				lock (CKernel.Queue.List.SyncRoot)
				{
					foreach (CClient client in CKernel.Queue.List)
					{
						client.ResetSentComment();
					}
				}
			}
		}

		public byte Rating
		{
			get { return m_MyRating; }
			set
			{
				m_MyRating=value;
				if (!m_Completed) SaveMet();
				lock (CKernel.Queue.List.SyncRoot)
				{
					foreach (CClient client in CKernel.Queue.List)
					{
						client.ResetSentComment();
					}
				}
			}
		}

		public bool HasComment
		{
			get
			{
				return ((m_MyRating>0) || ((m_MyComment!=null) && (m_MyComment.Length>0)));
			}
		}

		public uint CategoryID
		{
			get
			{
				return m_CategoryID;
			}
			set
			{
				m_CategoryID=value;
			}
		}
		
		public uint MaxSources
		{
			get
			{
				if (m_MaxSources==0) return (uint)CKernel.Preferences.GetInt("MaxSourcesPerFile");
				else return m_MaxSources;
			}
			set
			{
				m_MaxSources=value;
			}
		}

		public uint MaxSourcesInt
		{
			get
			{
				return m_MaxSources;
			}
		}

		#endregion

		#region Constructors & Initialization

		//TODO controlar la fecha
		public CFile()
		{
		}

		/// <summary>
		/// constructor for complete shared files
		/// </summary>
		public CFile(byte[] in_FileHash, DateTime in_fecha, string in_m_FileFolder,
			string in_nombreFichero,uint in_m_FileSize, Constants.Priority in_Prioridad,
			byte permisos, ArrayList in_HashSet, string in_strComment, byte in_rating)
		{
			FileHash=in_FileHash;
			//fechaFichero=in_fecha;
			m_FileName=in_nombreFichero;
			m_FileFolder=in_m_FileFolder;
			m_FileSize=in_m_FileSize;
			m_Transferred=in_m_FileSize;
			m_FileStatus=Protocol.FileState.Complete;
			m_UpPriority=in_Prioridad;
			permisos=permisos;
			m_Completed=true;
			m_isEmpty=false;
			m_HashSet=in_HashSet;
			//nPartes=(ushort)(((int)(m_FileSize-1)/(int)Protocol.PartSize) + 1);
			m_NumChunks=(ushort)in_HashSet.Count;
			m_MyComment=in_strComment;
			m_MyRating=in_rating;
			m_ChunksStatus=new Byte[m_NumChunks];
			for (int i=0;i!=m_ChunksStatus.Length;i++)
			{
				m_ChunksStatus[i]=(byte)Protocol.ChunkState.Complete;
			}

			m_CompleteName=Path.Combine(m_FileFolder, m_FileName);
			
			m_FileDate=new DateTime();
			m_FileDate=File.GetLastWriteTime(m_CompleteName).ToUniversalTime();

			if (DateTime.Compare(in_fecha,m_FileDate)!=0)
			{
				//TODO: Rehash el fichero
			}

			m_FileType=2; // TODO ver porque el tipo es fijo a 2....

			m_FileDetails = new CFileDetails(m_CompleteName,m_FileName);

			//			Info.Loaded=false;
			//			GetInfo();
			m_MaxSources=0;
		}

		/// <summary>
		/// constructor for a new downloading file
		/// </summary>
		public CFile(byte[] in_FileHash, string in_realfilename, uint in_FileSize)
		{
			//buscar el primer nombre .Part que este vacio en el 1er 
			//TODO controlar posible error: que este vacio el array de directorios temporales
			string in_fileFolder;
			string in_fileName;
			in_fileFolder=(string)CKernel.Preferences.GetStringArray("TempFolders")[0];			
			uint cont=1;
			if (!Directory.Exists(in_fileFolder))
			{
				Directory.CreateDirectory(in_fileFolder);
			}
			while (File.Exists(Path.Combine(in_fileFolder, cont.ToString("000") + ".part")))
			{
				cont++;
			}
			in_fileName=cont.ToString("000") + ".part";

			FileHash		= in_FileHash;
			m_FileName		= in_realfilename;
			m_PartFileName	= in_fileName;
			m_FileFolder	= in_fileFolder;
			m_FileSize		= in_FileSize;
			m_MyComment="";

			m_CompleteName		= Path.Combine(m_FileFolder, m_PartFileName);
			m_MetFile			= string.Concat(m_PartFileName, ".met");
			m_CompleteNameMet	= string.Concat(m_CompleteName, ".met");
			m_Transferred		= 0;
			m_UpPriority		=Constants.Priority.High;
			m_DownPriority		=Constants.Priority.Normal;
			
			//nPartes				= (ushort)(((int)(m_FileSize-1)/(int)Protocol.PartSize) + 1);
			m_NumChunks				= 0;
			
			if (m_CreateWriteFile())
			{
				m_ChunksStatus=new Byte[m_NumChunks];
				for (int i=0;i!=m_ChunksStatus.Length;i++)
				{
					m_ChunksStatus[i]=(byte)Protocol.ChunkState.Empty;
				}

				m_HashSet=new ArrayList(m_NumChunks);

				m_Gaps=new SortedList();
				m_RequestingBlocks=new SortedList();
				m_Gaps.Add((uint)0,m_FileSize-1);

				m_FileDate=new DateTime();
				m_FileDate=File.GetLastWriteTime(m_CompleteName).ToUniversalTime();

				SaveMet();
				SaveMetBackup();
			}
			
			m_CategoryID=CKernel.CategoriesList.GetAutoCategory(in_realfilename);

			m_FileDetails = new CFileDetails(m_CompleteName,m_FileName);

			m_FileType = 2; // TODO ver porque el tipo es fijo a 2....
			m_MaxSources=0;
			//			Info.Loaded=false;
		}

		/// <summary>
		/// constructor for incomplete files with its own .met
		/// the file is opened in write mode and it remains open until exit or file completes
		/// </summary>
		public CFile(string in_m_FileFolder, string in_filename)
		{
			m_PartFileName	= in_filename;
			m_FileFolder	= in_m_FileFolder;
			m_CompleteName	= Path.Combine(m_FileFolder, m_PartFileName);
			m_MetFile		= string.Concat(m_PartFileName, ".met");
			m_CompleteNameMet	= string.Concat(m_CompleteName, ".met");
			m_MyComment="";
			m_Gaps=new SortedList();
			m_UnknownParameters=new ArrayList();
			m_RequestingBlocks=new SortedList();
			m_MaxSources=0;
			if (m_LoadMet())
			{	
				DateTime fileDate=new DateTime();
				fileDate=File.GetLastWriteTime(m_CompleteName).ToUniversalTime();

				//m_LoadChunkStatus();

				m_Completed=false;
				if (!m_OpenFileWrite()) //se abre el fichero en modo escritura y se mantiene abierto hasta que se cierra el objeto
				{
					//si da un error al abrir el fichero: devolver un error en el constructor???
				}

				//if (Gaps.Count>0) m_RepairLphantOldGaps();				
				m_LoadChunkStatus();
				//m_LoadCrumbsStatus();

				if (DateTime.Compare(fileDate,m_FileDate)!=0) 
				{
					m_FileDate=fileDate;
					//TODO: Rehash las partes completas del fichero y comprobar que estan completas las que dicen que lo estan??
					//m_CheckCompleteFile();
				}

				m_FileDetails = new CFileDetails(m_CompleteName,m_FileName);

				if (m_Gaps.Count==0) 
				{
					//comprobar que esta completo... llamar a m_CheckCompleteFile
					if (m_HashSet.Count>0) 
					{
						m_CheckCompleteFile();
					} 
					else 
					{						
						m_Gaps.Add((uint)0,m_FileSize-1);
					}
				}
			} 
			else 
			{
				FileHash=null; //file bad loaded
				CLog.Log(Constants.Log.Notify,"Error loading MET file: " + m_CompleteNameMet);
			}
			m_FileType = 2; // TODO ver porque el tipo es fijo a 2....

			//			Info.Loaded=false;
			//			GetInfo();
		}
		
		~CFile()
		{
			try
			{
				if (m_CompleteFileThread!=null)
					if (m_CompleteFileThread.IsAlive) m_CompleteFileThread.Abort();
			}
			catch {}

			if (m_LoadedFile)
			{
				m_CloseFileWrite();
				SaveMet();
			}
		}
		#endregion

		private bool m_LoadMet()
		{
			return m_LoadMet(m_CompleteNameMet);
		}

		private void m_LoadMetParameters(BinaryReader reader)
		{
			CParameterReader ParameterReader; 
			uint nparameters=reader.ReadUInt32();
			for (uint parameter=0;parameter!=nparameters;parameter++)
			{
				ParameterReader=new CParameterReader(reader);
				string parametername;
				byte parameterid;
				parametername=ParameterReader.nombreParam;
				parameterid	= ParameterReader.id;
				switch( (Protocol.FileTag)ParameterReader.id )
				{
					case Protocol.FileTag.Name:
						m_FileName=ParameterReader.valorString;
						break;
					case Protocol.FileTag.Size:
						m_FileSize=ParameterReader.valorNum;
						break;
					case Protocol.FileTag.Priority:
						m_DownPriority=(Constants.Priority)ParameterReader.valorNum;
						break;
					case Protocol.FileTag.UploadPriority:
						m_UpPriority=(Constants.Priority)ParameterReader.valorNum;
						break;
					case Protocol.FileTag.Transfered:
						m_Transferred=ParameterReader.valorNum;
						break;
					case Protocol.FileTag.LastSeenComplete:
						m_LastSeenComplete=ParameterReader.valorNum;
						break;
					case Protocol.FileTag.State:
						m_FileStatus=(Protocol.FileState)ParameterReader.valorNum;
						break;
					case Protocol.FileTag.Comment:
						m_MyComment=ParameterReader.valorString;
						break;
					case Protocol.FileTag.Rating:
						m_MyRating=(byte)ParameterReader.valorNum;
						break;
					case Protocol.FileTag.CorruptedPart:
						if (m_CorruptedChunks == null) m_CorruptedChunks=new ArrayList();
						m_CorruptedChunks.Add((int)ParameterReader.valorNum);
						break;
					case Protocol.FileTag.Category:
						m_CategoryID=ParameterReader.valorNum;
						break;
					default: 
						if (!(ParameterReader.nombreParam == null))
						{
							if ((byte)ParameterReader.nombreParam.ToCharArray()[0] == (byte)Protocol.FileTag.GapStart)
							{
								uint inicio_gap;
								inicio_gap=ParameterReader.valorNum;
								ParameterReader=new CParameterReader(reader);
								parameter++;
								if ((byte)ParameterReader.nombreParam.ToCharArray()[0]!=(byte)Protocol.FileTag.GapEnd)
								{
									//TODO: lanzar error en gap
								}
								else 
								{
									uint fin_gap;
									fin_gap=ParameterReader.valorNum;
									//TODO: for next revision: m_AddGap(inicio_gap, fin_gap-1);

									//Debug.WriteLine("Gap:"+inicio_gap.ToString()+" : "+(fin_gap-1).ToString());
									if (((byte)m_FileFormat==(byte)Protocol.PartMet.eDonkeyOld)&&(ParameterReader.nombreParam.IndexOf(" ")>0))
										continue;
									m_AddGap(inicio_gap, fin_gap-1);
									//if (fin_gap>=m_FileSize) fin_gap=m_FileSize-1;
									//if ((fin_gap>=inicio_gap) && (fin_gap>=0) && (inicio_gap>=0) && (fin_gap<m_FileSize) && (inicio_gap<m_FileSize)) Gaps.Add((uint)inicio_gap, (uint)fin_gap);
								}
							}
							else
							{
								if (ParameterReader.nombreParam==Protocol.FT_MAXSOURCES)
									m_MaxSources=ParameterReader.valorNum;
								else if (ParameterReader.nombreParam==Protocol.FT_COMMENT)
									m_MyComment=ParameterReader.valorString;
								else if (ParameterReader.nombreParam==Protocol.FT_RATING)
									m_MyRating=(byte)ParameterReader.valorNum;
								else
									m_UnknownParameters.Add(ParameterReader);
							}
						}
						else
							m_UnknownParameters.Add(ParameterReader);
						break;
						//TODO: faltan los parameters para las estadisticas....
				}
			}
		}

		private void m_AddGap(uint start, uint end)
		{
			if (end>=m_FileSize) end=m_FileSize-1;
			
			if (end==m_FileSize-2) end=m_FileSize-1; //for compatibility with old lphant format::: remove for next releases

			if ((end>=start) && (end>=0) && (start>=0) && (end<m_FileSize) && (start<m_FileSize)) 
			{
				//for more sure (not to repeat and not to overlap gaps).. if i have something of (start, end) in gaps.. i delete (inicio, fin) in gaps 
				m_FillGap(start, end);
				//put (start, end) in gaps
				m_Gaps.Add((uint)start, (uint)end);
			}
		}

		private void m_RepairLphantOldGaps()
		{
			bool oldFormat;
			oldFormat=false;
			if ((uint)m_Gaps.GetByIndex(m_Gaps.Count-1)==m_FileSize -2)
			{
				if (m_MyFile.Length<=m_FileSize -2) 
				{
					oldFormat = true;
				}
			}

			if (oldFormat == false)
			{
				for (int n=0;n<m_Gaps.Count;n++)
				{
					if ((!m_IsInGaps((uint)m_Gaps.GetByIndex(n) + 1, (uint)m_Gaps.GetByIndex(n) +1)) && ((m_CorruptedChunks==null)||((m_CorruptedChunks!=null) && (!m_CorruptedChunks.Contains(((uint)m_Gaps.GetByIndex(n) + 1)/Protocol.PartSize)))))
					{
						if (m_MyFile.Length> (uint)m_Gaps.GetByIndex(n) + 1) 
						{
							m_MyFile.Seek((uint)m_Gaps.GetByIndex(n) + 1,SeekOrigin.Begin);
							if (m_MyFile.ReadByte()==0)
							{
								oldFormat = true;
							} 
							else 
							{
								oldFormat = false;
								break;
							}
						}
					}
				}
			}
			if (oldFormat)
			{
				for (int n=0;n<m_Gaps.Count;n++)
				{
					m_Gaps.SetByIndex(n ,(uint)m_Gaps.GetByIndex(n) + 1);
				}

				SaveMet();
			}
		}

		private bool m_LoadMetEdonkeyOld(string metfile)
		{
			m_UnknownParameters=new ArrayList();

			FileStream knownmet;
			BinaryReader reader;

			try
			{
				knownmet=File.OpenRead(metfile);
			}
			catch 
			{
				if (metfile==m_CompleteNameMet) 
				{
					CLog.Log (Constants.Log.Notify, "Load MET File Error. " + m_MetFile + ". File not found. Trying to recover from backup file.");
					return m_LoadMet(m_CompleteNameMet + ".backup");
				} 
				else 
				{
					return false;
				}
			}
			reader=new BinaryReader(knownmet);
			
			try
			{	
				if (((byte)reader.ReadByte()!=(byte)0xE0)||(reader.ReadByte()!=(byte)02))
				{
					reader.Close();
					knownmet.Close();
					if (metfile==m_CompleteNameMet) 
					{
						CLog.Log (Constants.Log.Notify, "Load MET File Error. " + m_MetFile + ". Header not match. Trying to recover from backup file.");
						return m_LoadMet(m_CompleteNameMet + ".backup");
					} 
					else 
					{
						return false;
					}
				}

				// test for new edonkey format
				byte[] test = new byte[4];
				reader.BaseStream.Seek(24, SeekOrigin.Begin);
				test = reader.ReadBytes(4);

				reader.BaseStream.Seek(2, SeekOrigin.Begin);

				if ((test[0] == 0) &&
					(test[1] == 0) &&
					(test[2] == 2) &&
					(test[3] == 1))
				{
					// new edonkey format
					m_FileDate=new DateTime(((long)reader.ReadUInt32()*10000000L)+621355968000000000L);
					FileHash=reader.ReadBytes(16);

					m_LoadMetParameters(reader);

					if (reader.BaseStream.Position < reader.BaseStream.Length) 
					{
						byte temp = reader.ReadByte();

						m_NumChunks=(ushort)CHash.GetChunksCount(m_FileSize);
						m_HashSet = new ArrayList(m_NumChunks);

						for (uint i = 0; i < m_NumChunks && (reader.BaseStream.Position + 16 < reader.BaseStream.Length); i++)
						{
							byte[] cur_hash = new byte[16];
							cur_hash = reader.ReadBytes(16);
							m_HashSet.Add(cur_hash);
						}
						//Console.WriteLine(reader.BaseStream.Position.ToString());
						//Console.WriteLine(reader.BaseStream.Length.ToString());

						byte[] hash = CHash.DoHashSetHash(m_HashSet);
						//Console.WriteLine(CKernel.HashToString(hash));
					}
				}
				else
				{
					// old edonkey format
					m_FileDate=new DateTime(((long)reader.ReadUInt32()*10000000L)+621355968000000000L);			

					FileHash=reader.ReadBytes(16);
					m_FileFormat=Protocol.PartMet.eDonkeyOld;
					m_LoadMetParameters(reader);

					m_NumChunks=(ushort)CHash.GetChunksCount(m_FileSize);
					m_HashSet=new ArrayList(m_NumChunks);
					m_CrumbsHashSet=new ArrayList((ushort)CHash.GetCrumbsCount(m_FileSize));

					if (reader.PeekChar()>=0)
					{
						if ((m_NumChunks>1)&&(reader.ReadByte()==(byte)1))
						{
							for (uint parte=0;parte!=m_NumChunks;parte++)
							{
								m_HashSet.Add(reader.ReadBytes(16));
							}
						}
						if ((reader.PeekChar()>=0)&&
							(reader.ReadByte()==(byte)1))
						{
							for (uint crumb=0;crumb!=CHash.GetCrumbsCount(m_FileSize);crumb++)
							{
								m_CrumbsHashSet.Add(reader.ReadBytes(8));
							}
						}
					}
				}
			}
			catch
			{
				reader.Close();
				knownmet.Close();
				if (metfile==m_CompleteNameMet) 
				{
					CLog.Log (Constants.Log.Notify, "Load MET File Error. " + m_MetFile + ". Error in data. Trying to recover from backup file.");
					return m_LoadMetEdonkeyOld(m_CompleteNameMet + ".backup");
				}
				else
				{
					return false;
				}
			}
			reader.Close();
			knownmet.Close();
			return true;
		}

		private bool m_LoadMet(string metfile)
		{
			m_UnknownParameters=new ArrayList();

			FileStream knownmet;
			BinaryReader reader;

			try
			{
				knownmet=File.OpenRead(metfile);
			}
			catch
			{
				if (metfile==m_CompleteNameMet) 
				{
					CLog.Log (Constants.Log.Notify, "Load MET File Error. " + m_MetFile + ". File not found. Trying to recover from backup file.");
					return m_LoadMet(m_CompleteNameMet + ".backup");
				}
				else
				{
					return false;
				}
			}
			reader=new BinaryReader(knownmet);

			try
			{	
				if ((byte)reader.ReadByte()!=(byte)0xE0)
				{
					reader.Close();
					knownmet.Close();
					if (metfile==m_CompleteNameMet) 
					{
						CLog.Log (Constants.Log.Notify, "Load MET File Error. " + m_MetFile + ". Header not match. Trying to recover from backup file.");
						return m_LoadMet(m_CompleteNameMet + ".backup");
					}
					else
					{
						return false;
					}
				}
			
				m_FileDate=new DateTime(((long)reader.ReadUInt32()*10000000L)+621355968000000000L);			
			
				FileHash=reader.ReadBytes(16);	

				m_NumChunks=reader.ReadUInt16();
			
				m_HashSet=new ArrayList(m_NumChunks);

				for (uint parte=0;parte!=m_NumChunks;parte++)
				{
					m_HashSet.Add(reader.ReadBytes(16));
				}

				//Gaps=new SortedList();
				m_LoadMetParameters(reader);
			}
			catch
			{
				reader.Close();
				knownmet.Close();
				if (metfile==m_CompleteNameMet) 
				{
					// try lo load with the .met edonkey format
					if (this.m_LoadMetEdonkeyOld(m_CompleteNameMet))
					{
						return true;
					}
					else
					{
						// try to recover a backup
						CLog.Log (Constants.Log.Notify, "Load MET File Error. " + m_MetFile + ". Error in data. Trying to recover from backup file.");
						return m_LoadMet(m_CompleteNameMet + ".backup");
					}
				} 
				else 
				{
					return false;
				}
			}
			m_FileFormat=Protocol.PartMet.eMule;
			reader.Close();
			knownmet.Close();
			return true;
		}

		private bool m_MetFileCorrect(string nameFile)
		{
			FileStream knownmet;
			BinaryReader reader;

			try
			{
				knownmet=File.OpenRead(nameFile);
			}
			catch 
			{
				return false;
			}
			reader=new BinaryReader(knownmet);

			try
			{
				if ((byte)reader.ReadByte()!=(byte)0xE0)
				{
					reader.Close();
					knownmet.Close();
					return false;
				}
			}
			catch
			{
				reader.Close();
				knownmet.Close();
				return false;
			}

			reader.Close();
			knownmet.Close();
			return true;
		}

		private void m_SaveParameters(BinaryWriter writer)
		{
			uint parameterCount;
			parameterCount=(uint)7 + (uint)(2 * m_Gaps.Count);
			if ((m_CorruptedChunks!=null)&&(m_CorruptedChunks.Count>0)) parameterCount+=(uint)m_CorruptedChunks.Count;
			if (m_UnknownParameters!=null) parameterCount+=(uint)m_UnknownParameters.Count;
			if (m_MyRating>0) parameterCount++;
			if ((m_MyComment!=null)&&(m_MyComment!="")) parameterCount++;
			if (m_CategoryID>0) parameterCount++;
			if (m_MaxSources>0) parameterCount++;

			writer.Write((uint)parameterCount);

			new ParameterWriter((byte)Protocol.FileTag.Name, m_FileName, writer);
			new ParameterWriter((byte)Protocol.FileTag.Size, m_FileSize, writer);
			new ParameterWriter((byte)Protocol.FileTag.Priority, (byte)m_DownPriority, writer);
			new ParameterWriter((byte)Protocol.FileTag.UploadPriority, (byte)m_UpPriority, writer);
			new ParameterWriter((byte)Protocol.FileTag.State, (byte)m_FileStatus, writer);
			new ParameterWriter((byte)Protocol.FileTag.Transfered, m_Transferred, writer);
			new ParameterWriter((byte)Protocol.FileTag.LastSeenComplete, m_LastSeenComplete, writer);
			
			if (m_CategoryID>0)
				new ParameterWriter((byte)Protocol.FileTag.Category, m_CategoryID, writer);
/* Old style changed due emule used that tags number
			if (m_MyRating>0)
				new ParameterWriter((byte)Protocol.FileTag.Rating, m_MyRating, writer);
			if ((m_MyComment!=null)&&(m_MyComment!="")) 
				new ParameterWriter((byte)Protocol.FileTag.Comment, m_MyComment, writer);
*/
			if (m_MyRating>0)
				new ParameterWriter(Protocol.FT_RATING, m_MyRating, writer);
			if ((m_MyComment!=null)&&(m_MyComment!="")) 
				new ParameterWriter(Protocol.FT_COMMENT, m_MyComment, writer);

			if (m_CorruptedChunks!=null)
				foreach (int parte in m_CorruptedChunks)
					new ParameterWriter((byte)Protocol.FileTag.CorruptedPart, (uint)parte, writer);
			if (m_MaxSources>0)
				new ParameterWriter(Protocol.FT_MAXSOURCES, m_MaxSources, writer);
			string str_gap_inicio=""; 
			string str_gap_fin=""; 

			//(char)str_gap_inicio[0]=(char)Protocol.FT_GAPSTART;
			//(char)str_gap_fin[0]=(char)Protocol.FT_GAPEND;

			for (int n=0; n!=m_Gaps.Count;n++)
			{		
				str_gap_inicio=(char)Protocol.FileTag.GapStart + n.ToString();
				str_gap_fin=(char)Protocol.FileTag.GapEnd + n.ToString();
				if ((byte)m_FileFormat==(byte)Protocol.PartMet.eMule)
				{
					//se pasan 10 carcteres como etiqueta del gap  (para hacerlo compatible con el emule)
					for (int char_cont=str_gap_inicio.Length; char_cont!=10;char_cont++)
					{
						str_gap_inicio=str_gap_inicio + " ";
					}
					
					for (int char_cont=str_gap_fin.Length; char_cont!=10;char_cont++)
					{
						str_gap_fin=str_gap_fin + " ";
					}
				}
				//Gaps.GetKey(n) es el inicio del gap
				//Gaps.GetByIndex(n) es el fin del gap	
				new ParameterWriter(str_gap_inicio,(uint)m_Gaps.GetKey(n),writer);
				new ParameterWriter(str_gap_fin,(uint)m_Gaps.GetByIndex(n)+1,writer);
			}

			//TODO: faltan los parameters para las estadisticas....
			if (m_UnknownParameters!=null)
			{
				foreach (CParameterReader ParameterReader in m_UnknownParameters)
				{
					CParametersSaver ParametersSaver=new CParametersSaver(ParameterReader.type,ParameterReader.id,ParameterReader.nombreParam,ParameterReader.valorString,ParameterReader.valorNum,writer);
				}
			}
		}

		private bool m_SaveMetEdonkeyOld()
		{
			lock(this) 
			{
				//if (m_MetFileCorrect(m_CompleteNameMet)) File.Copy(m_CompleteNameMet, m_CompleteNameMet + ".backup", true);

				FileStream knownmet;
				BinaryWriter writer;
				try
				{
					knownmet=File.Open(m_CompleteNameMet,FileMode.Create,FileAccess.Write,FileShare.None);
				}
				catch {return false;}
				writer=new BinaryWriter(knownmet);
				writer.Write((byte)0xE0);
				writer.Write((byte)0x02);

				//volvemos a leer la fecha de moficacin del fichero (per si de cas)
				try
				{
					m_FileDate=File.GetLastWriteTime(m_CompleteName).ToUniversalTime();
				}
				catch {return false;}				

				writer.Write((uint)(((long)m_FileDate.Ticks-621355968000000000L)/10000000L));
				writer.Write(FileHash);
				m_SaveParameters(writer);
				if ((m_HashSet!=null)&&(m_HashSet.Count>1))
				{
					writer.Write((byte)0x01);
					for (int i=0;i!=m_HashSet.Count;i++)
					{
						writer.Write((byte[])m_HashSet[i]);
					}
				}

				if ((m_CrumbsHashSet!=null)&&(m_CrumbsHashSet.Count>0))
				{
					writer.Write((byte)0x01);
					for (int i=0;i!=m_CrumbsHashSet.Count;i++)
					{
						writer.Write((byte[])m_CrumbsHashSet[i]);
					}
				}
				else
					writer.Write((byte)0x00);

				writer.Flush();
				writer.Close();
				knownmet.Close();

				return true; 
			}
		}

		public bool SaveMet()
		{			
			if ((byte)m_FileFormat==(byte)Protocol.PartMet.eDonkeyOld)
				return m_SaveMetEdonkeyOld();

			lock(this) 
			{
				//if (m_MetFileCorrect(m_CompleteNameMet)) File.Copy(m_CompleteNameMet, m_CompleteNameMet + ".backup", true);
				
				FileStream knownmet;
				BinaryWriter writer;
				try
				{
					knownmet=File.Open(m_CompleteNameMet,FileMode.Create,FileAccess.Write,FileShare.None);
				}
				catch (Exception e)

				{
					Debug.WriteLine("Error abriendo met para grabar: " + e.ToString());
					return false;
				}
				writer=new BinaryWriter(knownmet);
				writer.Write((byte)0xE0);

				//volvemos a leer la fecha de moficacin del fichero (per si de cas)
				try
				{
					m_FileDate=File.GetLastWriteTime(m_CompleteName).ToUniversalTime();
				}
				catch
				{
					Debug.WriteLine("Error grabando met (leyendo hora modificacion)");
					return false;
				}

				writer.Write((uint)(((long)m_FileDate.Ticks-621355968000000000L)/10000000L));
				writer.Write(FileHash);
				writer.Write((ushort)m_NumChunks);
				for (int i=0;i!=m_HashSet.Count;i++)
				{
					writer.Write((byte[])m_HashSet[i]);
				}
				m_SaveParameters(writer);
				writer.Flush();
				writer.Close();
				knownmet.Close();

				return true; 
			}
		}

		public bool SaveMetBackup ()
		{
			if (m_MetFileCorrect(m_CompleteNameMet)) 
			{
				File.Copy(m_CompleteNameMet, m_CompleteNameMet + ".backup", true);
				return true; 
			}
			return false;
		}

		//		private void m_LoadCrumbsStatus()
		//		{
		//			m_CrumbsStatus=new byte[CHash.GetCrumbsCount(m_FileSize)];
		//			uint start=0;
		//			uint end=0;
		//			for (int i=0;i!=m_CrumbsStatus.Length;i++)
		//			{
		//				start=(uint)(Protocol.CrumbSize*i);
		//				end=start+Protocol.CrumbSize;
		//				if (this.m_isFilled(start, end))
		//					m_CrumbsStatus[i]=(byte)Protocol.ChunkState.Complete;
		//				else if (this.m_IsEmptyBlock(start,end))
		//					m_CrumbsStatus[i]=(byte)Protocol.ChunkState.Empty;
		//				else 
		//					m_CrumbsStatus[i]=(byte)Protocol.ChunkState.InProgress;
		//			}
		//		}

		//mira los gaps y comprueba si las partes estan completas, vacias o a mitad
		private bool m_LoadChunkStatus()
		{			
			m_ChunksStatus=new Byte[m_NumChunks];
			for (int i=0;i!=m_ChunksStatus.Length;i++) 
			{
				m_ChunksStatus[i]=(byte)Protocol.ChunkState.InProgress;  //si no es una parte vacia y no es una parte completa... esta en proceso (a mitad)
				if ( IsEmptyChunk(i) )
					m_ChunksStatus[i]=(byte)Protocol.ChunkState.Empty;
				if ( m_ChunkCompleted(i) ) 
				{
					m_ChunksStatus[i]=(byte)Protocol.ChunkState.Complete;
					m_isEmpty=false;
				}
			}
			return true;
		}

		public bool IsEmptyChunk(int chunk_number)
		{
			uint chunk_start;
			uint chunk_end;
			chunk_start=(uint)(Protocol.PartSize*chunk_number);
			chunk_end=(uint)(Protocol.PartSize*(chunk_number+1)-1);
			if (chunk_end >= m_FileSize) chunk_end=m_FileSize -1;
			
			if (m_IsEmptyBlock(chunk_start,chunk_end)) return true;
			return false;
		}

		private bool m_ChunkCompleted(int chunk_number)
		{
			uint chunk_start;
			uint chunk_end;
			chunk_start=(uint)(Protocol.PartSize*chunk_number);
			chunk_end=(uint)(Protocol.PartSize*(chunk_number+1))-1;
			if (chunk_end > m_FileSize) chunk_end=m_FileSize-1;

			if (m_isFilled(chunk_start,chunk_end)) return true;

			return false;
		}

		private bool m_CheckCompleteChunk(uint start, uint end)
		{
			return m_CheckCompleteChunk(start,end, true, null);
		}

		/// <summary>
		/// recorrer todas las parte que hay entre inicio y fin y comprobar que no estan llenas
		/// si estan llenas.... comprobar el hash de la parte
		/// si el hash no coincide... liberar por entero esa parte
		/// </summary>
		private bool m_CheckCompleteChunk(uint start, uint end, bool toComplete, ArrayList calculatedHashSet)
		{
			bool is_complete_so=false;
			for (int num_chunk=(int)((uint)start / (uint)Protocol.PartSize); num_chunk<=(int)(((uint)(end-1)) / ((uint)(Protocol.PartSize))); num_chunk ++)
			{
				if ((m_ChunkCompleted(num_chunk))||((m_CorruptedChunks!=null)&&(m_CorruptedChunks.Contains(num_chunk))))
				{
#if DEBUG
					if ((m_CorruptedChunks!=null)&&(m_CorruptedChunks.Contains(num_chunk)))
					{
						CLog.Log(Constants.Log.Verbose, "trying to recover corrupted part");
					}
#endif
					//miro a ver si coincide el Hash

					//saco del fichero el contenido de la parte
					uint chunk_start;
					uint chunk_end;
					chunk_start=(uint)(Protocol.PartSize*num_chunk);
					chunk_end=(uint)(Protocol.PartSize*(num_chunk+1))-1;
					if (chunk_end>=m_FileSize) chunk_end=m_FileSize-1;

					//se halla el hash de ese contenido
					byte[] hash_chunk=null;
					if (calculatedHashSet!=null) 
					{
						if (calculatedHashSet.Count>num_chunk)
							hash_chunk=(byte[])calculatedHashSet[num_chunk];
					}
					else
					{
						byte[] content;
						//contenido=new Byte[(int)(chunk_end - chunk_start)+1];
						content=ReadBlock(chunk_start,chunk_end,((m_CorruptedChunks!=null)&&(m_CorruptedChunks.Contains(num_chunk))));
						hash_chunk=CHash.HashChunk(content);
						content=null;
						GC.Collect();
					}

					byte[] hash_correct = null;

					if (m_HashSet.Count>num_chunk)
					{
						hash_correct=(byte[])m_HashSet[num_chunk];
					}

					//si coincide el hash con el que tenemos en HashSet

					if ((hash_correct!=null) && (CKernel.SameHash(ref (byte[])hash_chunk, ref (byte[]) hash_correct)))
					{
						//ICH
						if ((m_CorruptedChunks!=null)&&(m_CorruptedChunks.Contains(num_chunk)))
						{
							m_CorruptedChunks.Remove(num_chunk);
							m_FillGap(chunk_start,chunk_end);
							CLog.Log(Constants.Log.Notify, "ICH_SAVEDPART",num_chunk.ToString());
						}
						//pongo como completa esta parte
						m_ChunksStatus[num_chunk]=(byte)Protocol.ChunkState.Complete;
						
						if (m_isEmpty==true)
						{
							m_isEmpty=false;
							CKernel.FilesList.EmptyFileToSharedFile(this);
						}
						else
							m_isEmpty = false;

						//compruebo si el fichero esta completo

						if (toComplete) m_CheckCompleteFile();

						is_complete_so=true;
					}
					else
					{
						if ((m_CorruptedChunks==null)||(!m_CorruptedChunks.Contains(num_chunk))||((m_CorruptedChunks.Contains(num_chunk))&&(m_ChunkCompleted(num_chunk))))
						{
							if (CKernel.Preferences.GetBool("ICHEnabled"))
							{
								if (m_CorruptedChunks==null) m_CorruptedChunks=new ArrayList();
								if (!m_CorruptedChunks.Contains(num_chunk)) m_CorruptedChunks.Add(num_chunk);
							}
							CLog.Log(Constants.Log.Notify, "FIL_PARTCORRUPT",num_chunk, m_FileName);
							//pongo la parte como vacia
							m_ChunksStatus[num_chunk]=(byte)Protocol.ChunkState.Empty;
							//poner como gap toda la parte.. para volver a empezar a descargarla
							m_Gaps.Add(chunk_start,chunk_end);
							m_TryToRecoverCorruptedPartCCH(num_chunk);
						}
					}
				}
				else   //no es parte completa
				{
					//aqui miramos si esta en estado completando y no existen blocks pedidos de esta parte -> pasar a estado en_proceso
					
					if ( ((Protocol.ChunkState)m_ChunksStatus[num_chunk] == Protocol.ChunkState.Completing) && (!RequestingBlocksInChunk(num_chunk)) ) 
					{
						m_ChunksStatus[num_chunk]=(byte)Protocol.ChunkState.InProgress;
					}
				}
			}
			return is_complete_so;
		}

		private void m_TryToRecoverCorruptedPartCCH(int chunkNumber)
		{
			if ((this.m_CrumbsHashSet==null)||(m_CrumbsHashSet.Count==0)) return;
			CLog.Log(Constants.Log.Verbose,"Trying to recover corrupted part with CCH "+m_FileName);
			byte[] content;

			uint chunk_start;
			uint chunk_end;
			chunk_start=(uint)(Protocol.PartSize*chunkNumber);
			chunk_end=(uint)(Protocol.PartSize*(chunkNumber+1))-1;
			if (chunk_end>=m_FileSize) chunk_end=m_FileSize-1;

			content=ReadBlock(chunk_start,chunk_end,true);
			ArrayList currentCrumbs=CHash.HashCrumbsInChunk(content);
			//compare crumbs...
			if (currentCrumbs.Count!=20) return;
			int recoveredBlocks=0;
			for (uint i=0;i<currentCrumbs.Count;i++)
			{
				byte[] c1=(byte[])currentCrumbs[(int)i];
				byte[] c2=(byte[])m_CrumbsHashSet[(int)i+(20*chunkNumber)];
				if (CKernel.SameCrumb(ref c1, ref c2))
				{
					//crumb is ok free the gap
					CLog.Log(Constants.Log.Verbose,"Crumb "+i.ToString()+" in chunk "+chunkNumber.ToString()+" is OK");
					m_FillGap(chunk_start+i*Protocol.CrumbSize,(chunk_start+(i+1)*Protocol.CrumbSize)-1);
					m_ChunksStatus[chunkNumber]=(byte)Protocol.ChunkState.InProgress;
					recoveredBlocks++;
				}
				else
				{
					CLog.Log(Constants.Log.Verbose,"Crumb "+i.ToString()+" in chunk "+chunkNumber.ToString()+" is WRONG");
				}
			}
			if (recoveredBlocks>0) 
				CLog.Log(Constants.Log.Info,"CCH_RECOVER",new string[]{(recoveredBlocks*(int)(Protocol.CrumbSize/1024)).ToString(),((int)(Protocol.PartSize/1024)).ToString(),chunkNumber.ToString(),FileName});
			
		}

		public bool RequestingBlocksInChunk(int chunkNumber)
		{
			uint chunk_start=(uint)chunkNumber*(uint)Protocol.PartSize;
			uint chunk_end= ((uint)(chunkNumber+1)*(uint)Protocol.PartSize)-1;

			for (int n=0;n!=m_RequestingBlocks.Count;n++)
			{
				if (((uint)m_RequestingBlocks.GetKey(n)<=chunk_start) && ((uint)m_RequestingBlocks.GetByIndex(n)>=chunk_start))
				{
					return true;
				}
				if (((uint)m_RequestingBlocks.GetKey(n)<=chunk_end) && ((uint)m_RequestingBlocks.GetByIndex(n)>=chunk_end))
				{
					return true;
				}
				if (((uint)m_RequestingBlocks.GetKey(n)>=chunk_start) && ((uint)m_RequestingBlocks.GetByIndex(n)<=chunk_end))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Comprueba si el fichero esta completo...
		/// lo graba y cambia las vbles directorio, nombre fichero, etc...
		/// </summary>
		private bool m_CheckCompleteFile()
		{
			lock(this)
			{
				if ((m_CompleteFileThread==null)||(!m_CompleteFileThread.IsAlive))
				{  
					if (m_Gaps.Count==0)  //si no quedan gaps por rellenar -> esta lleno el fichero
					{
						//TODO comprobar el hash???
						//si no coincide el hash del fichero.. hacer las hash de cada una de las partes
						FileStatus=Protocol.FileState.Completing;


						m_CompleteFileThread=new Thread(new ThreadStart(m_ThreadCompleteFile));
						m_CompleteFileThread.Name="Hash Complete File";
						m_CompleteFileThread.Start();
					}
				}
			}
			return true;
		}

		private void m_ThreadCompleteFile()
		{
			CHash m_EdonkeyHash;
			byte[] m_Hash;
			m_EdonkeyHash	= new CHash();

			//lock the file for read/write
			while ((m_isReadingFile) ||(m_isWritingFile))
			{
				//Debug.WriteLine("[File] Intentando entrar a ESCRIBIR BLOQUE: " + start.ToString() + "-" + end.ToString() + ". " + m_FileName);
				Thread.Sleep(500);
			}
			m_isWritingFile=true;

			m_Hash=m_EdonkeyHash.DoFileHash(m_MyFile); 

			//unlock the file
			m_isWritingFile	= false;

			if (!(CKernel.SameHash(ref (byte[])m_Hash, ref (byte[])FileHash)))
			{	
				FileStatus=Protocol.FileState.Ready;
				if (m_MyFile.Length>0) m_CheckCompleteChunk(0,m_FileSize-1,false,m_EdonkeyHash.HashSet);
				if (m_Gaps.Count==0)
				{
					m_Gaps.Add((uint)0,m_FileSize-1);
					//inicializo el hashset... para volverlo a pedir (por si ha llegado corrupto)
					
					m_NumChunks=0;
					m_ChunksStatus=new Byte[m_NumChunks];
					for (int i=0;i!=m_ChunksStatus.Length;i++)
					{
						m_ChunksStatus[i]=(byte)Protocol.ChunkState.Empty;
					}
		
					m_HashSet=new ArrayList(m_NumChunks);

					CLog.Log(Constants.Log.Notify, "Corrupted file: " + m_FileName + ". Returning to download the complete file.");
				}
				else 
				{
					CLog.Log(Constants.Log.Notify, "Corrupted file: " + m_FileName + ". Returning to download the corrupted chunks.");
				}
			}
			else
			{
				if (m_CloseFileWrite())
				{
					//mover el fichero
					m_FileFolder=CKernel.Preferences.GetStringArray("SharedFolders")[0]; //cojo el 1er directorio de los compartidos
					if (!Directory.Exists(m_FileFolder)) 
					{
						Directory.CreateDirectory(m_FileFolder);
					}

					string filenametodisk;

					filenametodisk = m_FileName;

					filenametodisk = filenametodisk.Replace("|","");
					filenametodisk = filenametodisk.Replace("\\","");
					filenametodisk = filenametodisk.Replace("/","");
					filenametodisk = filenametodisk.Replace(":","");
					filenametodisk = filenametodisk.Replace("*","");
					filenametodisk = filenametodisk.Replace("?","");
					filenametodisk = filenametodisk.Replace("<","");
					filenametodisk = filenametodisk.Replace(">","");

					//
					Regex r = new Regex(@"^con$|^prn$|(^lpt[0-9]$)|(^com[0-9]$)|(^con\.)|(^prn\.)|(^lpt[0-9]\.)|(^com[0-9]\.)",RegexOptions.IgnoreCase); 
					// Find a match in the file name.
					Match m = r.Match(filenametodisk); 
					if (m.Success)
					{
						filenametodisk = "_" + filenametodisk;
					}

					//comprobar que la longitud de la ruta de acceso no es superior a 260 caracteres
					if ((m_FileFolder.Length + filenametodisk.Length) >= 260)
					{
						int long_max = 260 - (m_FileFolder.Length +1);
						int index = filenametodisk.LastIndexOf(".");
						if (index > ((uint)filenametodisk.Length-(uint)long_max)) 
						{
							filenametodisk = filenametodisk.Substring(0,(int)(((uint)index-1) - ((uint)filenametodisk.Length-(uint)long_max))) + filenametodisk.Substring(index, (filenametodisk.Length-index));
						} 
						else 
						{
							filenametodisk=filenametodisk.Substring(0,long_max-1);
						}
					}

					string newCompleteName=Path.Combine(m_FileFolder, filenametodisk);
					int i=0;
					while (File.Exists(newCompleteName))
					{
						filenametodisk=string.Concat("["+i+"] " , filenametodisk);
						//comprobar que la longitud de la ruta de acceso no es superior a 260 caracteres
						if ((m_FileFolder.Length + filenametodisk.Length) >= 260)
						{
							int long_max = 260 - (m_FileFolder.Length +1);
							int index = filenametodisk.LastIndexOf(".");
							if (index > ((uint)filenametodisk.Length-(uint)long_max)) 
							{
								filenametodisk = filenametodisk.Substring(0,(int)(((uint)index-1) - ((uint)filenametodisk.Length-(uint)long_max))) + filenametodisk.Substring(index, (filenametodisk.Length-index));
							} 
							else 
							{
								filenametodisk=filenametodisk.Substring(0,long_max-1);
							}
						}
						newCompleteName=Path.Combine(m_FileFolder, filenametodisk);
						i++;
					}
					FileName=filenametodisk;
					
					try
					{
						File.Move(m_CompleteName,newCompleteName);
						m_Completed=true;
						m_CompleteName=newCompleteName;
						m_FileDetails.DiskFileName=m_CompleteName;

						m_FileDate=File.GetLastWriteTime(m_CompleteName).ToUniversalTime();

						//llamar a la funcin de grabar el know.met (despues de poner m_Completed=true
						CKernel.FilesList.SaveList("");

						CKernel.FilesList.CompleteFile(CKernel.HashToString(FileHash));

						//borrar .met
						File.Delete(m_CompleteNameMet);
						File.Delete(m_CompleteNameMet + ".backup"); 

						//							if (File.Exists(m_CompleteNameMet)) 
						//							{
						//								Debug.WriteLine("****[File]***** CAN NOT DELETE .met: " + m_CompleteNameMet);
						//							}
					}
					catch
					{
						CLog.Log(Constants.Log.Notify,"ERR1_COMPLETED",m_FileName);
					}
				}
				else
				{
					//error al cerrar el fichero
				}
			}
		}

		/// <summary>
		/// Funcin para leer de el fichero un block que empieza en la posicin inicio
		/// y termina en la posicin fin. Devuelve true si puede leer, false si no puede leer.
		/// </summary>
		/// 
		public byte[] ReadBlock(uint in_inicio, uint in_fin, bool esCorrupto)
		{
			if ((m_isFilled(in_inicio, in_fin))||(esCorrupto)) 
			{	
				byte[] out_contenido;
				out_contenido=new Byte[(in_fin - in_inicio)+1];
				lock(this)
				{
					if (m_OpenFileRead())
					{
						//BinaryReader lectorFichero =new BinaryReader(MiFichero); 
						//lectorFichero.BaseStream.Seek(in_inicio,SeekOrigin.Begin);

						//					bool salir=false;
						//				
						//					while (!salir)
						//					{
						//						try
						//						{
						//							Debug.WriteLine("[File] BLOQUEADO fichero para leer bloque: " + in_inicio.ToString() + "-" + in_fin.ToString() + ". " + m_FileName);
						//							m_MyFile.Lock(0,m_FileSize-1);
						//							salir=true;
						//						}
						//						catch(Exception e)
						//						{
						//							Debug.WriteLine(e.ToString() + " Leyendo bloque: " + in_inicio.ToString() + "-" + in_fin.ToString() + ". " + m_FileName);
						//							Thread.Sleep(500);
						//						}
						//					}

						m_MyFile.Seek(in_inicio,SeekOrigin.Begin);
						m_MyFile.Read(out_contenido,0,out_contenido.Length);
						//out_contenido	=	lectorFichero.ReadBytes(((int)in_fin - (int)in_inicio) +1);
						//					
						//					m_MyFile.Unlock(0,m_FileSize-1);
						//					Debug.WriteLine("[File] DESBLOQUEADO fichero para leer bloque: " + in_inicio.ToString() + "-" + in_fin.ToString() + ". " + m_FileName);

						m_CloseFile();	
						//lectorFichero.Close();
						//lectorFichero=null;
						return out_contenido;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Writes a block in a fil from the "start" position to "end" position
		/// Returns true if successfull
		/// </summary>
		public bool WriteBlock(uint start, uint end, byte[] content)
		{
			lock (this) 
			{
				//if ((FileStatus==Protocol.FileState.Complete)||(FileStatus==Protocol.FileState.Completing)) return true;
				bool writed=false;
				if (m_IsEmptyBlock(start, end)) 
				{
					if (m_CanWrite()) 
					{
						//while ((m_isWritingFile))
						while ((m_isReadingFile) ||(m_isWritingFile))
						{
							//Debug.WriteLine("[File] Intentando entrar a ESCRIBIR BLOQUE: " + start.ToString() + "-" + end.ToString() + ". " + m_FileName);
							Thread.Sleep(500);
						}
						if ((FileStatus==Protocol.FileState.Complete)) return true;
				
						m_isWritingFile=true;
						
						bool salir=false;
						while (!salir)
						{
							try
							{
								//Debug.WriteLine("[File] BLOQUEADO fichero para escribir bloque: " + start.ToString() + "-" + end.ToString() + ". " + m_FileName);
								m_MyFile.Lock(0,m_FileSize-1);
								salir=true;
							}
							catch(Exception e)
							{
								Debug.WriteLine(e.ToString() + " Para escribir bloque: " + start.ToString() + "-" + end.ToString() + ". " + m_FileName);
								Thread.Sleep(500);
							}
						}
						try
						{
							if (end>=(uint)m_MyFile.Length) m_MyFile.SetLength((long)end + 1);

							m_MyFile.Seek((long)start,SeekOrigin.Begin);
							m_MyFile.Write(content,0,(int)(end-start+1));
							m_MyFile.Flush();
							content=null;

							m_FileDate=File.GetLastWriteTime(m_CompleteName).ToUniversalTime();
							m_Transferred += (uint)end - (uint)start + 1;
							m_isWritingFile=false;
							writed=true;
							m_FillGap(start,end);
							RemoveRequestedBlock (start,end);
						}
						catch(Exception e)
						{
							writed=false;
							m_isWritingFile=false;
							CLog.Log(Constants.Log.Notify,"DISK_FULL");
							CLog.Log(Constants.Log.Info,e.ToString());
							Debug.WriteLine(e.ToString() +  m_FileName);
							Debug.WriteLine("stopped: " +  m_FileName);
						}
						m_MyFile.Unlock(0,m_FileSize-1);							
						//Debug.WriteLine("[File] DESBLOQUEADO fichero para escribir bloque: " + start.ToString() + "-" + end.ToString() + ". " + m_FileName);
						
						bool iscompletechunk;
						iscompletechunk = false;
						if (writed) iscompletechunk = m_CheckCompleteChunk(start, end);
						if (!m_Completed) 
						{
							//writed=(writed && SaveMet());
							bool metsaved;
							metsaved = SaveMet();
							//only copy the .met file backup when a part is completed
							if (!metsaved) CLog.Log(Constants.Log.Info,"Error saving met");
							if (writed && metsaved && iscompletechunk) 
							{
								SaveMetBackup();
							}
						}
						
					}
				}
				else 
				{
					writed=true;
					CLog.Log(Constants.Log.Info, "[File] Sending to write writed block: " + start.ToString() + " - " + end.ToString() + " (" + m_FileName + ")");
				}
				return writed;
			}
		}


		/// <summary>
		/// Check a block if it's completely full.
		/// </summary>
		/// <param name="start">Start of block</param>
		/// <param name="end">End of block</param>
		/// <returns>Returns true if block is completely full.</returns>
		private bool m_isFilled(uint start, uint end)
		{
			//si inicio<inicio_del_fichero o fin>fin_del_fichero devuelve false
			if ((end<start) || (end >= m_FileSize)) return false;

			//si el fichero esta completado devuelve true
			if (m_Completed) return true;
			
			//si no esta pendiente algun trozo que contenga algn elemento entre inicio y fin devuelve true... si no false
			if (m_SomethingInGaps(start, end)) return false;			

			return true;
		}

		/// <summary>
		/// Check a block if it's completely empty.
		/// </summary>
		/// <param name="start">Start of block</param>
		/// <param name="end">End of block</param>
		/// <returns>Returns true if block is completely empty.</returns>
		private bool m_IsEmptyBlock(uint start, uint end)
		{
			//si el fichero esta completado devuelve false
			if (m_Completed) return false;

			//si inicio<inicio_del_fichero o fin>fin_del_fichero devuelve false
			if ((end<start) || (end >= m_FileSize)) return false;

			//si esta pendiente todo entre inicio y fin devuelve true... si no false
			if (m_IsInGaps(start, end)) return true;

			return false;
		}

		/// <summary>
		/// Devuelve true si no hay ningn trozo entre inicio y fin en GAPS
		/// <summary>
		private bool m_SomethingInGaps(uint start, uint end)
		{
			if (m_Gaps!= null)
			{
				for (int n=0; n!=m_Gaps.Count;n++)
				{	
					//Gaps.GetKey(n) es el inicio del gap
					//Gaps.GetByIndex(n) es el fin del gap

					//como es una listaordenada---> si iniciogap > fin -> se puede devolver false
					if ((uint)m_Gaps.GetKey(n)>(uint)end)
						return false;

					//mirar si existe algn iniciogap>=inicio y <=fin   --|--__|	y	--|----|--
					if (((uint)m_Gaps.GetKey(n)>=(uint)start) && ((uint)m_Gaps.GetKey(n)<=(uint)end))
						return true;

					//mirar si existe algn fingap>=inicio y <=fin		|__--|--	y	--|----|--
					if (((uint)m_Gaps.GetByIndex(n)>=(uint)start) && ((uint)m_Gaps.GetByIndex(n)<=(uint)end))
						return true;

					//mirar si el inicio-fin esta entero dentro del gap   |_--_|
					if (((uint)m_Gaps.GetKey(n)<=(uint)start) && ((uint)m_Gaps.GetByIndex(n)>=(uint)end))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Devuelve true si no el trozo entre inicio y fin esta por completo en los GAPS
		/// <summary>
		private bool m_IsInGaps(uint inicio, uint fin)
		{
			if (m_Gaps!= null)
			{
				for (int n=0; n!=m_Gaps.Count;n++)
				{
					//Gaps.GetKey(n) es el inicio del gap
					//Gaps.GetByIndex(n) es el fin del gap
					
					//como es una listaordenada---> si iniciogap > inicio -> se puede devolver false	--_|__|	y	--|-_|	y	--|--|--
					if ((uint)m_Gaps.GetKey(n)>(uint)inicio) return false;

					//mirar si el inicio-fin esta entero dentro del gap   |_--_|
					if (((uint)m_Gaps.GetKey(n)<=(uint)inicio) && ((uint)m_Gaps.GetByIndex(n)>=(uint)fin)) return true;

					//si no esta completo se pone como inicio el fin del gap + 1
					if (((uint)m_Gaps.GetKey(n)<=(uint)inicio) && ((uint)m_Gaps.GetByIndex(n)>=(uint)inicio) && ((uint)m_Gaps.GetByIndex(n)<(uint)fin)) 
					{
						inicio=(uint)m_Gaps.GetByIndex(n) + 1;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// quita el trozo entre inicio y fin de los gaps y devuelve true si no da error
		/// <summary>
		private bool m_FillGap(uint inicio, uint fin)
		{			
			lock (this)
			{
				if (m_Gaps!= null)
				{
					uint gap_inicio;
					uint gap_fin;
					for (int n=0; n!=m_Gaps.Count;n++)
					{
						//Gaps.GetKey(n) es el inicio del gap
						//Gaps.GetByIndex(n) es el fin del gap
					
						//como es una listaordenada---> si iniciogap > inicio -> se puede devolver false	--_|__|	y	--|-_|	y	--|--|--
						if ((uint)m_Gaps.GetKey(n)>(uint)inicio) return false;

						//mirar si el inicio-fin esta entero dentro del gap   |_--_|
						if (((uint)m_Gaps.GetKey(n)<=(uint)inicio) && ((uint)m_Gaps.GetByIndex(n)>=(uint)fin)) 
						{
							gap_inicio=(uint)m_Gaps.GetKey(n);
							gap_fin	= (uint)m_Gaps.GetByIndex(n);

							m_Gaps.Remove(m_Gaps.GetKey(n));
							if (gap_inicio<inicio) 
							{
								m_Gaps.Add(gap_inicio, inicio-1);
							}
							if (gap_fin>fin) 
							{
								m_Gaps.Add(fin +1, gap_fin);
							}
							return true;
						}

						//si no esta completo se pone como inicio el fin del gap + 1
						if (((uint)m_Gaps.GetKey(n)<=(uint)inicio) && ((uint)m_Gaps.GetByIndex(n)>=(uint)inicio) && ((uint)m_Gaps.GetByIndex(n)<(uint)fin)) 
						{
							gap_inicio=(uint)m_Gaps.GetKey(n);
							gap_fin	= (uint)m_Gaps.GetByIndex(n);
							m_Gaps.Remove(m_Gaps.GetKey(n));
							if (gap_inicio<inicio) 
							{
								m_Gaps.Add(gap_inicio, inicio-1);
							}
							inicio=gap_fin + 1;
						}
					}
				}
				else
				{
					return false;
				}
				return false;
			}
		}

		/// <summary>
		/// Funcin que devuelve un nuevo block a pedir. Devuelve true si se puede pedir un block, false si no se puede pedir
		/// </summary>
		public bool GetNewBlockInChunk(uint chunk, ref uint start, ref uint end)
		{
			lock(this)
			{
				if ((Protocol.ChunkState)m_ChunksStatus[chunk] == Protocol.ChunkState.Complete)
					return false;

				if ((Protocol.ChunkState)m_ChunksStatus[chunk] == Protocol.ChunkState.Empty) 
				{
					start=chunk * (uint)Protocol.PartSize;
					end=start + (uint)Protocol.BlockSize - 1;
					if (end >= m_FileSize)
					{
						end=m_FileSize - 1;
						m_ChunksStatus[chunk]=(byte)Protocol.ChunkState.Completing;
					}
					else
					{
						m_ChunksStatus[chunk]=(byte)Protocol.ChunkState.InProgress;
					}
					AddRequestingBlock(start, end);

					return true;
				}
			
				for (uint i=0; i <= ((uint)Protocol.PartSize/(uint)Protocol.BlockSize);i++)
				{
					uint start_block=((uint)Protocol.PartSize*chunk) + i*(uint)Protocol.BlockSize;
					uint end_block =((uint)Protocol.PartSize*chunk) + ((i+1)*(uint)Protocol.BlockSize)-1;
				
					if (end_block >= m_FileSize)
					{
						end_block=m_FileSize - 1;
					}
                				
					if (end_block >= (uint)Protocol.PartSize*(chunk+1))
					{
						end_block=(uint)((uint)Protocol.PartSize*(chunk+1)) - 1;
					}

					if (start_block >= m_FileSize) return false;

					while (start_block<=end_block)
					{
						if (m_IsEmptyBlock(start_block,end_block))
						{
							break;
						}
						if (m_isFilled(start_block,end_block))
						{
							start_block=end_block + 1;
						} 
						else 
						{
							//buscar el max(inicio de gap)> que inicio y < fin 
							//ya que si no esta vacio el gap... es pq se ha empezado a rellenar por el principio.. solo varia el principio del block a pedir					
							//que hacia esto aqui???? inicio_block=end_block + 1;
							bool changes=false;
							for (int n=0; n!=m_Gaps.Count;n++) 
							{
								if (((uint)m_Gaps.GetKey(n)>=(uint)start_block) && ((uint)m_Gaps.GetKey(n)<=(uint)end_block))
								{
									changes=true;
									start_block=(uint)m_Gaps.GetKey(n);
									if (end_block>=(uint)m_Gaps.GetByIndex(n)) end_block	= (uint)m_Gaps.GetByIndex(n);
									break;
								}
								else if (((uint)m_Gaps.GetByIndex(n)>=(uint)start_block) && ((uint)m_Gaps.GetByIndex(n)<=(uint)end_block))
								{
									changes=true;
									end_block	= (uint)m_Gaps.GetByIndex(n);
									break;
								}
							}
							if (!changes) start_block=end_block + 1;
						}
					}

					if (start_block<=end_block)
					{
						if (BlockNotRequested(start_block, end_block))
						{
							start	= start_block;
							end		= end_block;
							AddRequestingBlock(start, end);

							bool last_block=true;
						
							uint chunk_end=(uint)((uint)Protocol.PartSize*(chunk+1))-1;

							if (end_block >= m_FileSize)
							{
								end_block=m_FileSize - 1;
							}

							uint chunk_start= (uint)(Protocol.PartSize*chunk);
							if (chunk_start < start_block)
							{
								//TODO: utilizar una funcion para comprobar que tp esta en blockspedidos
								if (AnythingToRequest(chunk_start, (start_block-1))) last_block=false;
								//if (AlgoEnGaps(chunk_start, (inicio_block-1))) ultimo_block=false;
							}

							if (chunk_end > end_block)
							{
								//TODO: utilizar una funcion para comprobar que tp esta en blockspedidos
								if (AnythingToRequest((end_block +1), chunk_end)) last_block=false;
								//if (AlgoEnGaps((end_block +1), chunk_end)) ultimo_block=false;
							}

							if (last_block)
							{
								m_ChunksStatus[chunk]=(byte)Protocol.ChunkState.Completing;
							}
							else
							{
								m_ChunksStatus[chunk]=(byte)Protocol.ChunkState.InProgress;
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool RemainingToAskInChunk(int chunk_number)
		{
			uint chunk_start;
			uint chunk_end;
			chunk_start=(uint)(Protocol.PartSize*chunk_number);
			chunk_end=(uint)(Protocol.PartSize*(chunk_number+1)-1);
			if (chunk_end >= m_FileSize) chunk_end=m_FileSize -1;
			
			if (AnythingToRequest(chunk_start,chunk_end)) return true;
			return false;
		}

		public bool AnythingToRequest(uint inicio, uint fin)
		{
			for (int n=0; n!=m_RequestingBlocks.Count;n++)
			{
				if ((uint)m_RequestingBlocks.GetKey(n) > fin) return m_SomethingInGaps(inicio, fin);
				if (inicio<(uint)m_RequestingBlocks.GetKey(n))
				{
					if (m_SomethingInGaps(inicio, (uint)m_RequestingBlocks.GetKey(n)-1)) return true;
				}
				if (((uint)m_RequestingBlocks.GetByIndex(n)>=inicio) && (fin>(uint)m_RequestingBlocks.GetByIndex(n)))
				{
					inicio=(uint)m_RequestingBlocks.GetByIndex(n)+1;
				}
				if (fin<=(uint)m_RequestingBlocks.GetByIndex(n))
				{
					return false;
				}
			}
			return m_SomethingInGaps(inicio, fin);
		}

		public bool AddRequestingBlock(uint start, uint end)
		{
			m_RequestingBlocks.Add(start, end);
			//Debug.WriteLine("[ADD Requested block] " + m_FileName + ": " + start.ToString() + "-" + end.ToString());
			return true;
		}

		public bool RemoveRequestedBlock(uint start, uint end)
		{
			lock(this)
			{
				if (!m_RequestingBlocks.ContainsKey(start)) 
				{
					Debug.WriteLine ("***************[Intentando borrar un request block errneo]************** " + m_FileName );
				}
				//Debug.WriteLine("[LIBERATE Requested blocks] " + m_FileName + ": " + start.ToString() + "-" + end.ToString());
			
				m_RequestingBlocks.Remove(start);
				for (uint chunk_number=start/Protocol.PartSize;chunk_number<=end/Protocol.PartSize;chunk_number++)
				{
					if ((IsEmptyChunk((int)chunk_number)))
					{
						if (!RequestingBlocksInChunk((int)chunk_number))
						{
							m_ChunksStatus[chunk_number]=(byte)Protocol.ChunkState.Empty;
						} 
						else 
						{
							m_ChunksStatus[chunk_number]=(byte)Protocol.ChunkState.InProgress;
						}
					}
					else
					{
						// si falta algo por pedir.. se pone el estado de la parte.. en proceso
						if (AnythingToRequest((uint)(chunk_number*Protocol.PartSize),(uint) (((chunk_number +1 )*Protocol.PartSize)-1)))
							m_ChunksStatus[chunk_number]=(byte)Protocol.ChunkState.InProgress;
					}
				}
				return true;
			}
		}

		public bool BlockNotRequested(uint inicio, uint fin)
		{
			//solo compruebo que no hay un block con un inicio=inicio... ya que el fin siempre hubiera sido el mismo
			if (m_RequestingBlocks.ContainsKey(inicio))
			{
				return false;
			}
			return true;
		}

		public void CloseFile()
		{
			m_CloseFileWrite();
		}

		public void CancelFile()
		{
			m_CloseFileWrite();
			try
			{
				File.Delete(m_CompleteNameMet);
				File.Delete(m_CompleteNameMet + ".backup");
				string dir = Path.GetDirectoryName(m_CompleteNameMet);
				File.Delete(Path.Combine(dir, m_PartFileName));
			}
			catch {}
		}

		public bool CrumbsHashSetNeeded()
		{
			return ((m_CrumbsHashSet==null)||(m_CrumbsHashSet.Count==0));
		}

		/// <summary>
		/// si el HashSet es nulo o lo es su primer elemento -> requiere que se pida el hashset
		/// </summary>
		public bool HashSetNeeded()
		{
			if (m_NumChunks==0)
			{
				if (CHash.GetChunksCount(m_FileSize)==1)
				{
					if (m_ChunksStatus.Length==0)
					{
						ArrayList NewHashSet=new ArrayList();
						NewHashSet.Add(FileHash);
						HashSet=NewHashSet;
					}
					return false;
				}
				else
					return true;
			}
			return false;
		}

		private bool m_OpenFileRead()
		{
			lock(this)
			{
				//if (FileStatus==Protocol.FileState.Completing) return false;

				//while ((m_isWritingFile))
				while ((m_isReadingFile) ||(m_isWritingFile))
				{
					Thread.Sleep(2000);
					//Debug.WriteLine ("[File] Intentando entrar a LEER fichero" + m_FileName);
				}

				m_isReadingFile=true;

				if ((!m_LoadedFile) && (!m_ReadingReady))
				{
					try 
					{ 
						m_MyFile=new FileStream(m_CompleteName,FileMode.Open,FileAccess.Read,FileShare.Read);   
						m_ReadingReady=true;
						return true;
					}
					catch (FileNotFoundException) 
					{
						//El fichero no se encuentra...
						Debug.WriteLine("File not found in the specified folder.");			
						m_ReadingReady=false;
						//m_Completed=false;
						m_isReadingFile=false;
					}
					catch (Exception e)
					{ 
						//Algn otro error...
						Debug.WriteLine(e.ToString());
						m_ReadingReady=false;
						//m_Completed=false;
						m_isReadingFile=false;
					}
				}
			}
			return true;
		}

		private bool m_CreateWriteFile()
		{
			lock(this)
			{
				if ((!m_LoadedFile) && (!m_Completed))
				{
					try 
					{ 
						m_MyFile=new FileStream(m_CompleteName,FileMode.Create,FileAccess.ReadWrite,FileShare.Read);   
						m_LoadedFile=true;
						return true;
					}
					catch (FileNotFoundException)
					{
						//El fichero no se encuentra...
						Debug.WriteLine("No se pudo encontrar el archivo en el directorio especificado.");			
						m_LoadedFile	= false;
						//m_Completed		= false;
					}
					catch (Exception e)
					{
						//Algn otro error...
						Debug.WriteLine(e.ToString());
						m_LoadedFile=false;
						//m_Completed=false;
					}
				}
			}
			return false;
		}

		private bool m_CloseFileWrite()
		{
			lock(this)
			{
				if ((m_LoadedFile) && (!m_Completed))
				{
					while ((m_isReadingFile) ||(m_isWritingFile))
					{
						Thread.Sleep(2000);
					}

					m_MyFile.Close();
					m_LoadedFile = false;
					return true;
				}
			}
			return false;
		}

		private bool m_OpenFileWrite()
		{
			lock(this)
			{
				if ((!m_LoadedFile) && (!m_Completed))
				{
					try 
					{ 
						m_MyFile=new FileStream(m_CompleteName,FileMode.Open,FileAccess.ReadWrite,FileShare.Read);   
						m_LoadedFile=true;
						return true;
					}
					catch (FileNotFoundException) 
					{
						//El fichero no se encuentra...
						Debug.WriteLine("No se pudo encontrar el archivo en el directorio especificado.");			
						m_LoadedFile	= false;
						//m_Completed		= false;
					}
					catch (Exception e)
					{
						//Algn otro error...
						Debug.WriteLine(e.ToString());
						m_LoadedFile=false;
						//m_Completed		= false;
					}
				}
			}
			return false;
		}

		private bool m_CanWrite()
		{
			if (m_Completed) return false;

			if (!m_LoadedFile) return false;
			
			if (m_MyFile.CanWrite) return true;

			return false;
		}

		private bool m_CloseFile()
		{
			lock(this)
			{
				if (m_ReadingReady) 
				{
					try 
					{ 
						m_MyFile.Close();
						m_MyFile=null;
						m_isReadingFile=false;
						m_ReadingReady=false;
						return true;
					}
					catch (Exception e) 
					{ 
						//Algn error...
						Debug.WriteLine(e.ToString());
					}
				}
				m_isReadingFile=false;
			}
			return false; 
		}

		public uint GetRemainingBytes() 
		{
			uint total_bytes;
			if (m_Gaps==null) return 0;
			total_bytes=0;
			for (int cont=0;cont!=m_Gaps.Count;cont++)
			{
				total_bytes += ((uint)m_Gaps.GetByIndex(cont) - (uint)m_Gaps.GetKey(cont) +1);
			}
			return total_bytes;
		}
	}
}