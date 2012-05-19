using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using eLePhant.Types;

namespace eLePhant.eDonkey
{

	internal class CServerHello
	{
		public CServerHello(MemoryStream buffer)
		{
			DonkeyHeader header;
			BinaryWriter writer=new BinaryWriter(buffer);
			header=new DonkeyHeader((byte)Protocol.ServerCommand.LoginRequest,writer);
			writer.Write(CKernel.Preferences.GetByteArray("UserHash"));
			writer.Write(CKernel.Preferences.GetUInt("ID"));
			writer.Write(CKernel.Preferences.GetUShort("TCPPort"));
			uint nParameters=5;
			writer.Write(nParameters);
			// user
			new ParameterWriter((byte)Protocol.ClientParameter.Name,CKernel.Preferences.GetString("UserName"),writer);
			// version
			new ParameterWriter((byte)Protocol.ClientParameter.Version,Protocol.EDONKEYVERSION,writer); 
			// port
			new ParameterWriter((byte)Protocol.ClientParameter.Port,(uint)CKernel.Preferences.GetUShort("TCPPort"),writer); 
			// compression
			new ParameterWriter((byte)Protocol.ClientParameter.Compression,5,writer); //1 (supports compression) + 5 (supports alternative server ports)
			// emule version
			new ParameterWriter((byte)Protocol.ClientParameter.EmuleVersion,Protocol.EMULE_VERSION_COMPLEX,writer);
			
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);      
		}
	}

	/// <summary>
	/// CServerStatus process the server status send by CServer.
	/// </summary>
	internal class CServerStatus
	{
		public uint Users;
		public uint Files;

		public CServerStatus(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			Users=reader.ReadUInt32();
			Files=reader.ReadUInt32();
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	/// <summary>
	/// CServerDescription process description send by CServer.
	/// </summary>
	internal class CServerDescription
	{
		public string Name;
		public string Description;

		public CServerDescription(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			reader.ReadBytes(30);
			short length;
			length=reader.ReadInt16();
			Name=Encoding.ASCII.GetString(reader.ReadBytes(length));
			reader.ReadBytes(4);
			length=reader.ReadInt16();
			Description=Encoding.ASCII.GetString(reader.ReadBytes(length));
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	/// <summary>
	/// CServerServerList process the serverlist send by/to CServer.
	/// </summary>
	internal class CServerServerList
	{
		public byte NewServers;

		public CServerServerList(MemoryStream buffer)
		{
			DonkeyHeader header;
			BinaryWriter writer=new BinaryWriter(buffer);
			header=new DonkeyHeader((byte)Protocol.ServerCommand.GetServerList,writer);				
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);
		}

		public CServerServerList(MemoryStream buffer,CServersList ServerList)
		{
			BinaryReader reader=new BinaryReader(buffer);
			byte nServers=reader.ReadByte();
			uint IP;
			ushort port;
			NewServers=0;
			try
			{
				for (int i=0;i!=nServers;i++)
				{
					IP=reader.ReadUInt32();
					port=reader.ReadUInt16();
					if (ServerList.Add(IP,port)!=null) NewServers++;
				}
			}
			catch {}
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	internal class CServerPublishFiles
	{
		public uint LastFilePublished;
		//constructor to publish a list of files
		public CServerPublishFiles(MemoryStream buffer,CFilesList FilesList,uint startIndex)
		{
			DonkeyHeader header;
			LastFilePublished=startIndex;
			BinaryWriter writer=new BinaryWriter(buffer);
			header=new DonkeyHeader((byte)Protocol.ServerCommand.OfferFiles,writer);
			writer.Write((uint)FilesList.Count);
			uint nfiles=0;
			int fileIndex=0;
			foreach (CElement element in FilesList.Values)
			{
				fileIndex++;
				if (fileIndex<startIndex) continue;
				if (m_AddFileToPacket(writer,element)) nfiles++;
				LastFilePublished++;
				if (nfiles>=200) break; //TODO: check server soft limits
			}
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);
			writer.Write(nfiles);
			CLog.Log(Types.Constants.Log.Info,"FIL_PUBLISHED",nfiles);
		}
		//constructor to publish one file
		public CServerPublishFiles(MemoryStream buffer,CElement element)
		{
			DonkeyHeader header;
			BinaryWriter writer=new BinaryWriter(buffer);
			header=new DonkeyHeader((byte)Protocol.ServerCommand.OfferFiles,writer);
			writer.Write(1);
			m_AddFileToPacket(writer,element);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);
			writer.Write(1);
			CLog.Log(Types.Constants.Log.Info,"FIL_PUBLISHED",1);
		}
		private bool m_AddFileToPacket(BinaryWriter writer,CElement element)
		{
			try
			{
				if (!element.File.Empty)
				{
					writer.Write(element.File.FileHash);
					if (element.File.Completed)
					{
						writer.Write((uint)0xfbfbfbfb);
						writer.Write((ushort)0xfbfb);
					}
					else
					{
						writer.Write((uint)0xfcfcfcfc);
						writer.Write((ushort)0xfcfc);
					}
					uint nParameters=2;
					if (element.File.Details.ListDetails.ContainsKey(Constants.Avi.Length)) nParameters++;
					if (element.File.Details.ListDetails.ContainsKey(Constants.Avi.VBitrate)) nParameters++;
					if (element.File.Details.ListDetails.ContainsKey(Constants.Avi.VCodec)) nParameters++;
					writer.Write(nParameters);
					// name
					new ParameterWriter((byte)Protocol.FileTag.Name,element.File.FileName,writer);
					// size
					new ParameterWriter((byte)Protocol.FileTag.Size,element.File.FileSize,writer);
					if (element.File.Details.Type==(byte)Constants.FileType.Avi)
					{
						if (element.File.Details.ListDetails.ContainsKey(Constants.Avi.Length))
							new ParameterWriter(Protocol.FileExtraTags.length.ToString(),(string)element.File.Details.ListDetails[Constants.Avi.Length],writer);
						if (element.File.Details.ListDetails.ContainsKey(Constants.Avi.VBitrate))
							new ParameterWriter(Protocol.FileExtraTags.bitrate.ToString(), Convert.ToUInt32(((string)element.File.Details.ListDetails[Constants.Avi.VBitrate]).Replace(" Kbps","")), writer);
						if (element.File.Details.ListDetails.ContainsKey(Constants.Avi.VCodec))
							new ParameterWriter(Protocol.FileExtraTags.codec.ToString(),(string)element.File.Details.ListDetails[Constants.Avi.VCodec],writer);
					}
					return true;
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
			return false;
		}
	}

	class CServerAskSources
	{
		public CServerAskSources(byte[] FileHash,MemoryStream buffer)
		{
			DonkeyHeader header;
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Seek(0,SeekOrigin.End);
			header=new DonkeyHeader((byte)Protocol.ServerCommand.GetSources/*,writer*/);
			header.Packetlength=17;
			header.Serialize(writer);
			writer.Write(FileHash);
//			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
//			writer.Seek(0,SeekOrigin.Begin);
//			header.Serialize(writer);
		}
	}

	class CServerRequestCallback
	{
		public CServerRequestCallback(uint ID,MemoryStream buffer)
		{
			DonkeyHeader header;
			BinaryWriter writer=new BinaryWriter(buffer);
			header=new DonkeyHeader((byte)Protocol.ServerCommand.CallBackRequest,writer);
			writer.Write(ID);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);				
		}
	}

	class CServerRequestCallBackUDP
	{
		public CServerRequestCallBackUDP(uint ID,uint IP,ushort Port,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Write((byte)Protocol.Header.eDonkey);
			writer.Write((byte)Protocol.ServerCommandUDP.GlobalCallBackRequest);
			writer.Write(IP);
			writer.Write(Port);
			writer.Write(ID);
		}
	}

	class CServerAskSourcesUDP
	{
		public CServerAskSourcesUDP(byte[] FileHash,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Write((byte)Protocol.Header.eDonkey);
			writer.Write((byte)Protocol.ServerCommandUDP.GlobalGetSources);
			writer.Write(FileHash);
		}

		public CServerAskSourcesUDP(ArrayList FilesHash,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Write((byte)Protocol.Header.eDonkey);
			writer.Write((byte)Protocol.ServerCommandUDP.GlobalGetSources);
			foreach (byte[] fileHash in FilesHash)
			{
				writer.Write(fileHash);
			}
		}
	}

	class CServerRequestPingUDP
	{	
		public uint rndvalping;

		public CServerRequestPingUDP(MemoryStream buffer)
		{
			BinaryWriter writer = new BinaryWriter(buffer);
			writer.Write((byte)Protocol.Header.eDonkey);
			writer.Write((byte)Protocol.ServerCommandUDP.GlobalStateRequest);
			Random rnd=new Random();
			rndvalping=(uint)0x55AA0000 + (ushort)rnd.Next();
			writer.Write(rndvalping);
		}
	}

	class CServerPingResponseUDP
	{
		public uint rndretval;
		public uint nUsers;
		public uint nfiles;
		public uint nMaxUsers;

		public CServerPingResponseUDP(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			rndretval=reader.ReadUInt32();
			nUsers=reader.ReadUInt32();
			nfiles=reader.ReadUInt32();
			if (buffer.Length>=16)
				nMaxUsers=reader.ReadUInt32();
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}


	class CServerRequestStatusUDP
	{
		public CServerRequestStatusUDP(MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Write((byte)Protocol.ProtocolType.eDonkey);
			writer.Write((byte)Protocol.ServerCommandUDP.DescriptionRequest);
		}
	}

	class CServerResponseStatusUDP
	{
		public string name;
		public string description;
		public CServerResponseStatusUDP(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			byte [] buf;
			ushort length=reader.ReadUInt16();
			buf=reader.ReadBytes(length);
			name=Encoding.Default.GetString(buf);
			length=reader.ReadUInt16();
			buf=reader.ReadBytes(length);
			description=Encoding.Default.GetString(buf);
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	class CServerProcessSources
	{
		public CServerProcessSources(MemoryStream buffer,uint ServerIP,ushort ServerPort)
		{
			byte[] FileHash;
			uint IP;
			ushort Port;
			byte nSources;
			bool moreFiles=true;
			BinaryReader reader=new BinaryReader(buffer);
			do
			{
				FileHash=reader.ReadBytes(16);
				nSources=reader.ReadByte();
				Debug.WriteLine("Received "+nSources.ToString()+" for "+CKernel.HashToString(FileHash));
				while (nSources>0)
				{
					IP=reader.ReadUInt32();
					Port=reader.ReadUInt16();
					nSources--;
					CKernel.ClientsList.AddClientToFile(IP,Port,ServerIP,ServerPort,FileHash);
				}
				if ((reader.PeekChar()!=0)&&(reader.PeekChar()!=-1)) 
				{
					if ((Protocol.ProtocolType)reader.ReadByte() != Protocol.ProtocolType.eDonkey) moreFiles=false;
					if ((reader.PeekChar()==-1)||(reader.ReadByte()!= (byte)Protocol.ServerCommandUDP.GlobalFoundSources))
						moreFiles = false;
				}
				else moreFiles = false;
			}
			while (moreFiles);
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	class CServerRequestSearch
	{
		const byte typeNemonic1=0x03;
		const short typeNemonic2=0x0001;
		const uint extensionNemonic= 0x00040001;
		const uint avaibilityNemonic=0x15000101;
		const uint minNemonic=0x02000101;
		const uint maxNemonic=0x02000102;
		const byte stringParameter=1;
		const byte typeParameter=2;
		const byte numericParameter=3;
		const short andParameter=0x0000;
		const short orParameter=0x0100;
		const short notParameter=0x0200;

		public CServerRequestSearch(MemoryStream buffer,string searchString,bool matchAnyWords,string type,uint maxSize,uint minSize,uint avaibility, string exclude, bool esUDP)
		{
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ServerCommand.SearchRequest);
			BinaryWriter writer=new BinaryWriter(buffer);
			if (esUDP)
			{
				writer.Write((byte)Protocol.Header.eDonkey);
				//writer.Write((byte)Protocol.ServerCommandsUDP.GlobalSearchRequest);
				writer.Write((byte)Protocol.ServerCommandUDP.GlobalSearchRequest2);
			}
			else
				header.Serialize(writer);

			if (type=="Any") type="";
			//header
			int parametercount=0; //must be written parametercount-1 parmeter headers
			
			if (exclude.Length>0)
				writer.Write(notParameter);

			if ( searchString.Length>0)
			{
				parametercount++;
				if (parametercount>1)
					writer.Write(andParameter);
			}

			if (type.Length>0)
			{
				parametercount++;
				if (parametercount>1)
					writer.Write(andParameter);
			}
			  
			if (minSize>0)
			{
				parametercount++;
				if (parametercount>1)
					writer.Write(andParameter);
			}
		        
			if (maxSize>0)
			{
				parametercount++;
				if (parametercount>1)
					writer.Write(andParameter);
			}
		        
			if (avaibility>0)
			{
				parametercount++;
				if (parametercount>1)
					writer.Write(andParameter);
			}
		        
			//				if (extension.GetLength()>0)
			//				{
			//				parametercount++;
			//				if (parametercount>1)
			//					writer.Write(andParameter);
			//				}
			//body
			if (searchString.Length>0) //search a string
			{
				//writer.Write(notParameter);

				string[] list=searchString.Split(" ".ToCharArray());
				byte[] searchbyte;
				if ((matchAnyWords)&&(list.Length>1))
				{
					for (int i=0;i<list.Length;i++)
					{
						if (i!=list.Length-1) writer.Write(orParameter);
						writer.Write(stringParameter); //write the parameter type
						searchbyte=Encoding.Default.GetBytes(list[i]);
						writer.Write((ushort)searchbyte.Length);
						writer.Write(searchbyte);
					}
				}
				else
				{
						writer.Write(stringParameter); //write the parameter type
						searchbyte=Encoding.Default.GetBytes(searchString);
						writer.Write((ushort)searchbyte.Length);
						writer.Write(searchbyte);
				}
			}

			if (type.Length>0)
			{
				writer.Write(typeParameter); //write the parameter type
				byte[] searchbyte=Encoding.Default.GetBytes(type);
				writer.Write((ushort)searchbyte.Length);
				writer.Write(searchbyte);
				writer.Write(typeNemonic2);
				writer.Write(typeNemonic1);
			}

			if (minSize>0)
			{
				writer.Write(numericParameter); //write the parameter type
				writer.Write(minSize);		   //write the parameter
				writer.Write(minNemonic);    //nemonic for this kind of parameter
			}

			if (maxSize>0)
			{
				writer.Write(numericParameter); //write the parameter type
				writer.Write(maxSize);		   //write the parameter
				writer.Write(maxNemonic);    //nemonic for this kind of parameter
			}

			if (avaibility>0)
			{
				writer.Write(numericParameter); //write the parameter type
				writer.Write(avaibility);    //write the parameter
				writer.Write(avaibilityNemonic);  //nemonic for this kind of parameter
			}

			//				if (extension.GetLength()>0)
			//				{
			//					data.Write(&stringParameter,1); //write the parameter type
			//					nSize=extension.GetLength(); 
			//					data.Write(&nSize,2);   //write the length
			//					formatC=extension.GetBuffer(); 
			//					data.Write(formatC,nSize); //write parameter
			//					data.Write(&extensionNemonic,3); //nemonic for this kind of parameter (only 3 bytes!!)
			//				}
			if (exclude.Length>0)
			{
				writer.Write(stringParameter); //write the parameter type
				byte[] searchbyte2=Encoding.Default.GetBytes(exclude);
				writer.Write((ushort)searchbyte2.Length);
				writer.Write(searchbyte2);
			}

			if (!esUDP)
			{
				header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
				writer.Seek(0,SeekOrigin.Begin);
				header.Serialize(writer);				
			}
		}
	}

	class CServerCallbackResponse
	{
		public uint IP;
		public ushort Port;
		public CServerCallbackResponse(MemoryStream buffer)
		{
			if (buffer.Length!=6) return;
			BinaryReader reader=new BinaryReader(buffer);
			IP=reader.ReadUInt32();
			Port=reader.ReadUInt16();
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	class CServerSearchResults
	{
		public CServerSearchResults(MemoryStream buffer,CSearcher search,bool esUDP)
		{
			BinaryReader reader=new BinaryReader(buffer);
			if (!esUDP)
			{
				uint nResultados=reader.ReadUInt32();
				for (uint i=0;i<nResultados;i++)
				{
					m_ExtractResult(reader,search);
				}
				search.OnTCPSearchEnded();
			}
			else
			{
				m_ExtractResult(reader,search);
				while ((reader.PeekChar()!=0)&&(reader.PeekChar()!=-1)) 
				{
					Debug.WriteLine("MoreUDP results in one packet");
					if ((Protocol.ProtocolType)reader.ReadByte() != Protocol.ProtocolType.eDonkey) break;
					if ((reader.PeekChar()==-1)||(reader.ReadByte() != (byte)Protocol.ServerCommandUDP.GlobalSearchResult))
						break;
					m_ExtractResult(reader,search);
				}
			}
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;				
		}

		private uint m_ExtractResult(BinaryReader reader,CSearcher search)
		{
			CParameterReader parameterReader;
			byte[] HashEncontrado = reader.ReadBytes(16);
			uint ip = reader.ReadUInt32();
			ushort port = reader.ReadUInt16();
			uint nParametros = reader.ReadUInt32();
			string fileName = "?";
			uint fileSize = 0;
			uint nSources = 1;
			string codec="";
			uint bitrate=0;
			string length="";
			bool complete=false;
			for (uint param = 0; param != nParametros; param++)
			{
				parameterReader = new CParameterReader(reader);
				switch( (Protocol.FileTag)parameterReader.id )
				{
					case Protocol.FileTag.Name:
						fileName = parameterReader.valorString;
						break;
					case Protocol.FileTag.Size:
						fileSize = parameterReader.valorNum;
						break;
					case Protocol.FileTag.Sources:
						nSources = parameterReader.valorNum;
						break;
					case Protocol.FileTag.Completed:
						complete = parameterReader.valorNum>0;
						break;
					default:
						if (parameterReader.nombreParam==Protocol.FileExtraTags.codec.ToString())
							codec=parameterReader.valorString;
						else if (parameterReader.nombreParam==Protocol.FileExtraTags.length.ToString())
							length=parameterReader.valorString;
						else if (parameterReader.nombreParam==Protocol.FileExtraTags.bitrate.ToString())
							bitrate=parameterReader.valorNum;
						//Debug.WriteLine(parameterReader.id+" name: "+parameterReader.nombreParam+" valString:"+parameterReader.valorString+" valNum: "+parameterReader.valorNum);
						break;
				}
			}
			if (fileSize<Protocol.PartSize) complete=true;
			search.AddFileFound(HashEncontrado,fileName,fileSize,nSources,codec,length,bitrate,complete,ip,port);
			return nSources;
		}
	}

	class CServerQueryMoreResults
	{
		public CServerQueryMoreResults(MemoryStream buffer)
		{
			DonkeyHeader header;
			BinaryWriter writer=new BinaryWriter(buffer);
			header=new DonkeyHeader((byte)Protocol.ServerCommand.QueryMoreResults,writer);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);				
		}
	}
}
