#region Copyright (c)2003 Juanjo < http://lphant.sourceforge.net >
/*
* This file is part of eLePhant
* Copyright (C)2003 Juanjo < j_u_a_n_j_o@users.sourceforge.net / http://lphant.sourceforge.net >
* 
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either
* version 2 of the License,or (at your option) any later version.
* 
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with this program; if not,write to the Free Software
* Foundation,Inc.,675 Mass Ave,Cambridge,MA 02139,USA.
*/
#endregion

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Net;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for edonkeyCommands.
	/// </summary>
	internal class DonkeyHeader
	{
		public Protocol.ProtocolType eDonkeyID;
		public uint Packetlength;
		public byte Command;

		public DonkeyHeader(byte command)
		{
			eDonkeyID=Protocol.ProtocolType.eDonkey;
			Command=command;
			Packetlength=6;
		}

		public DonkeyHeader(byte command,Protocol.ProtocolType protocol)
		{
			eDonkeyID=protocol;
			Command=command;
			Packetlength=6;
		}

		public DonkeyHeader(byte command,BinaryWriter writer)
		{
			eDonkeyID=Protocol.ProtocolType.eDonkey;
			Command=command;
			Packetlength=6;
			Serialize(writer);
		}

		public DonkeyHeader(byte command,BinaryWriter writer,Protocol.ProtocolType protocol)
		{
			eDonkeyID=protocol;
			Command=command;
			Packetlength=6;
			Serialize(writer);
		}

		public DonkeyHeader(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			eDonkeyID=(Protocol.ProtocolType)reader.ReadByte();
			Packetlength=(uint)reader.ReadInt32();
			Command=reader.ReadByte();
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write((byte)eDonkeyID);
			writer.Write(Packetlength);
			writer.Write(Command);
		}
	}

	#region Parameter Reader & Writer
/*
#define	TAGTYPE_HASH			0x01
#define	TAGTYPE_STRING			0x02
#define	TAGTYPE_UINT32			0x03
#define	TAGTYPE_FLOAT32			0x04
#define	TAGTYPE_BOOL			0x05
#define	TAGTYPE_BOOLARRAY		0x06
#define	TAGTYPE_BLOB			0x07
#define	TAGTYPE_UINT16			0x08
#define	TAGTYPE_UINT8			0x09
*/

	internal class CParameterReader
	{
		public byte type;
		public byte id;
		public string nombreParam;
		public string valorString;
		public uint valorNum;

		public CParameterReader(BinaryReader reader)
		{
			ushort longitud;
			type=reader.ReadByte();
			longitud=reader.ReadUInt16();

			//Console.WriteLine("type: " + type.ToString());

			if (longitud==1)
				id=reader.ReadByte();
			else
				if (longitud>0) nombreParam=new string(reader.ReadChars(longitud));

			if (type==2)
			{
				longitud=reader.ReadUInt16();
				//valorString=new string(reader.ReadChars(longitud));
				//valorString=Encoding.UTF8.GetString(reader.ReadBytes(longitud));
				byte [] buf;//=new byte[longitud];
				buf=reader.ReadBytes(longitud);
				valorString=Encoding.Default.GetString(buf);
			} 
			else if (type==3) valorNum=reader.ReadUInt32();
			else if (type==4) reader.ReadBytes(4);
			else if (type==1) reader.ReadBytes(16);
			else if (type==5) reader.ReadByte();
		}
	}

	internal class CParametersSaver
	{
		public CParametersSaver(byte type, byte id, string name, string valueString, uint valueNum, BinaryWriter writer)
		{
			switch(type)
			{
				case 2:
					if (name!=null)
						new ParameterWriter(name,valueString,writer);
					else
						new ParameterWriter(id,valueString,writer);
					break;
				case 3:
					if (name!=null)
						new ParameterWriter(name,valueNum,writer);
					else
						new ParameterWriter(id,valueNum,writer);
					break;
			}
		}
	}

	internal struct ParameterWriter
	{
		private const short typeParameter = 1;

		private byte m_type;
		private object m_id;
		private object m_value;

		public ParameterWriter(byte id, uint value, BinaryWriter writer)
		{
			m_type = 3;
			m_id = id;
			m_value = value;
			Serialize(writer);
		}

		public ParameterWriter(byte id, uint value)
		{
			m_type = 3;
			m_id = id;
			m_value = value;
		}
	
		public ParameterWriter(byte id, string value, BinaryWriter writer)
		{
			m_type = 2;
			m_id = id;
			m_value = value;
			Serialize(writer);
		}

		public ParameterWriter(byte id, string value)
		{
			m_type = 2;
			m_id = id;
			m_value = value;
		}

		public ParameterWriter(string id, uint value, BinaryWriter writer)
		{
			m_type = 3;
			m_id = id;
			m_value = value;
			Serialize(writer);
		}

		public ParameterWriter(string id, uint value)
		{
			m_type = 3;
			m_id = id;
			m_value = value;
		}

		public ParameterWriter(string id, string value, BinaryWriter writer)
		{
			m_type = 2;
			m_id = id;
			m_value = value;
			Serialize(writer);
		}

		public ParameterWriter(string id, string value)
		{
			m_type = 2;
			m_id = id;
			m_value = value;
		}

		public void Serialize(BinaryWriter writer)
		{
			if (m_id is string)
				SerializeString(writer);
			else
				SerializeNumeric(writer);
		}

		internal void SerializeNumeric(BinaryWriter writer)
		{
			byte id = (byte)m_id;

			writer.Write(m_type);
			writer.Write(typeParameter);
			writer.Write(id);
			if (m_value is string)
			{
				string value = (string)m_value;
				byte[] buffer = Encoding.Default.GetBytes(value);
				writer.Write((ushort)buffer.Length);
				writer.Write(buffer);
			}
			else
			{
				uint value = (uint)m_value;
				writer.Write(value);
			}
		}

		internal void SerializeString(BinaryWriter writer)
		{
			string id = (string)m_id;

			writer.Write(m_type);
			writer.Write((ushort)id.ToCharArray().Length);
			writer.Write(id.ToCharArray());
			if (m_value is string)
			{
				string value = (string)m_value;
				byte[] buffer = Encoding.Default.GetBytes(value);
				writer.Write((ushort)buffer.Length);
				writer.Write(buffer);
//				string value = (string)m_value;
//				writer.Write((ushort)value.ToCharArray().Length);
//				writer.Write(value.ToCharArray());
			}
			else
			{
				uint value = (uint)m_value;
				writer.Write(value);
			}
		}
	}

	#endregion

	public struct CReceiveHello
	{
		public byte HashSize;
		public byte[] Hash;
		public uint UserID;
		public ushort UserPort;
		public string UserName;
		public uint Version;
		public uint ServerIP;
		public ushort ServerPort;
		public byte software;

		public uint VersioneMuleMajor;
		public uint VersioneMuleMinor;
		public uint VersioneMuleRevision;
		//public byte VersionEmuleProtocol;
		public byte VersionCompression;
		public byte VersionSourceExchange;
		public byte VersionUDP;
		public byte VersionComments;
		public byte VersionExtendedRequests;
		public uint VersionLphant;
		public uint IDClientCompatible;
		public bool AllowViewSharedFiles;
		public ushort PortUDP;
		public bool ExtendedInfoComplete;
		public bool SupportsPartner;
		public bool PartnerAllowed;
		public bool SupportsHorde;

		public CReceiveHello(bool bRespuesta,MemoryStream buffer)
		{
			int emuleinfo=0;
			AllowViewSharedFiles=true;
			IDClientCompatible=0;
			VersionUDP=0;
			VersionComments=0;
			VersionCompression=0;
			VersioneMuleMinor=0;
			VersioneMuleRevision=0;
			VersioneMuleMajor=0;
			VersionExtendedRequests=0;
			VersionLphant=0;
			VersionSourceExchange=0;
			PortUDP=0;
			PartnerAllowed=false;
			SupportsPartner=false;
			SupportsHorde=false;

			BinaryReader reader=new BinaryReader(buffer);
			
			ExtendedInfoComplete=false;

			if (bRespuesta)
				HashSize=16;
			else
				HashSize=reader.ReadByte();

			Hash=reader.ReadBytes(HashSize);
			software = (byte)Protocol.Client.eDonkey;

			if ((Hash[5]==14)&&(Hash[14]==111))
			{
				software = (byte)Protocol.Client.eMule;
			}

			UserID=reader.ReadUInt32();
			UserPort=reader.ReadUInt16();
			uint nParametros=reader.ReadUInt32();
			UserName="";
			Version=0;

			for (int i=0;i!=nParametros;i++)
			{
				CParameterReader ParameterReader=new CParameterReader(reader);
				switch( (Protocol.ClientParameter)ParameterReader.id )
				{
					case Protocol.ClientParameter.Name:
						UserName = ParameterReader.valorString;
						break;
					case Protocol.ClientParameter.Version:
						Version = ParameterReader.valorNum;
						break;
					case Protocol.ClientParameter.Port:
						UserPort = (ushort)ParameterReader.valorNum;
						break;
					case Protocol.ClientParameter.EmuleVersion:
						VersioneMuleMajor =((ParameterReader.valorNum & 0x00FFFFFF) >> 17) & 0x7F;
						VersioneMuleMinor =((ParameterReader.valorNum & 0x00FFFFFF) >> 10) & 0x7F;
						VersioneMuleRevision =((ParameterReader.valorNum & 0x00FFFFFF) >> 7) & 0x07;
						//Revision=( (ParameterReader.valorNum & 0x00FFFFFF) >> 7) & 0x07;
						IDClientCompatible =(ParameterReader.valorNum >> 24);
						emuleinfo|=4;
						break;
					case Protocol.ClientParameter.EmuleUDPPort:
						PortUDP=(ushort)ParameterReader.valorNum;
						emuleinfo|=1;
						break;
					case Protocol.ClientParameter.Emule_MiscOptions1:
						VersionUDP				= (byte)((ParameterReader.valorNum >> 4*6) & 0x0f);
						VersionCompression		= (byte)((ParameterReader.valorNum >> 4*5) & 0x0f);
						//m_bySupportSecIdent		= ((ParameterReader.valorNum >> 4*4) & 0x0f);
						VersionSourceExchange	= (byte)((ParameterReader.valorNum >> 4*3) & 0x0f);
						VersionExtendedRequests	= (byte)((ParameterReader.valorNum >> 4*2) & 0x0f);
						VersionComments			= (byte)((ParameterReader.valorNum >> 4*1) & 0x0f);
						AllowViewSharedFiles	= ((ParameterReader.valorNum >> 1*2) & 0x01)>0;
						//m_bMultiPacket			= (ParameterReader.valorNum >> 1*1) & 0x01;
						//m_fSupportsPreview		= (ParameterReader.valorNum >> 1*0) & 0x01;						
						emuleinfo|=2;
						break;
					case Protocol.ClientParameter.EmuleCompatOptions:
						if ((ParameterReader.valorNum & 0x00000001)>0) SupportsPartner=true;
						if ((ParameterReader.valorNum & 0x00000002)>0) PartnerAllowed=true;
						break;
					default:
						if (ParameterReader.id==0) 
						{
							if ((ParameterReader.nombreParam==Protocol.HordeParameter)&&(ParameterReader.valorNum==Protocol.HORDE_VERSION))
								SupportsHorde=true;
						}
						break;
				}
			}
			ServerIP=reader.ReadUInt32();
			ServerPort=reader.ReadUInt16();
			
			if ((emuleinfo & 4)== 4) ExtendedInfoComplete=true;

			if (buffer.Length-buffer.Position>=3)
			{
				software  = (byte)Protocol.Client.eDonkeyHybrid;
				uint extra=reader.ReadUInt32();
				if (extra==1262767181)
					software=(byte)Protocol.Client.mlDonkey;
			}
			
			if ((IDClientCompatible==(byte)Protocol.Client.eLePhant)&&(VersioneMuleMajor>0)) VersioneMuleMajor--;

			if (Version > 10000 && Version < 100000)
				Version = Version - (Version/10000)*10000;

			if (Version > 1000)
				Version = Version - (Version/1000)*1000;

			if (Version < 100)
				Version *= 10;

			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	internal class CSendHello
	{
		//private byte opcode;
		public CSendHello(bool response,MemoryStream buffer,CServer server, bool sendCompatOptions, bool allowPartner)
		{
			DonkeyHeader header;
			BinaryWriter writer=new BinaryWriter(buffer);
			if (response)
				header=new DonkeyHeader((byte)Protocol.ClientCommand.HelloAnswer,writer);
			else
			{
				header=new DonkeyHeader((byte)Protocol.ClientCommand.Hello,writer);
				writer.Write((byte)CKernel.Preferences.GetByteArray("UserHash").Length);
			}
			writer.Write(CKernel.Preferences.GetByteArray("UserHash"));
			writer.Write(CKernel.Preferences.GetUInt("ID"));
			writer.Write(CKernel.Preferences.GetUShort("TCPPort"));
			uint nParametros=5;
			if (sendCompatOptions) nParametros++;
			writer.Write(nParametros);
			// username
			new ParameterWriter((byte)Protocol.ClientParameter.Name,CKernel.Preferences.GetString("UserName"),writer);
			// version
			new ParameterWriter((byte)Protocol.ClientParameter.Version,Protocol.EDONKEYVERSION,writer); 
			//ParameterNumericNumeric port=new ParameterWriter((byte)Protocol.ClientParameter.Port,(uint)CKernel.Preferences.GetUShort("TCPPort"),writer); 
			// emule version
			new ParameterWriter((byte)Protocol.ClientParameter.EmuleVersion,Protocol.EMULE_VERSION_COMPLEX,writer);
			// upd port
			new ParameterWriter((byte)Protocol.ClientParameter.EmuleUDPPort,(uint)CKernel.Preferences.GetUShort("UDPPort"),writer); 
			// emule flags
			new ParameterWriter((byte)Protocol.ClientParameter.Emule_MiscOptions1,
//				(						<< 4*7) |
				(Protocol.EMULE_VERSION_UDP				<< 4*6) |
				(Protocol.EMULE_VERSION_COMPRESION		<< 4*5) |
				(0 /*secureID */						<< 4*4) |
				(Protocol.EMULE_VERSION_SOURCEEXCHANGE	<< 4*3) |
				(Protocol.EMULE_VERSION_EXTENDEDREQUEST	<< 4*2) |
				(Protocol.EMULE_VERSION_COMMENTS		<< 4*1) |
//				(										<< 1*3) |
				((((Types.Constants.AllowViewShared)CKernel.Preferences.GetEnum("AllowViewShared",Types.Constants.AllowViewShared.Nobody)==Types.Constants.AllowViewShared.Nobody) ? (uint)1 : (uint)0 )<< 1*2) |
				(0 /*uMultiPacket*/						<< 1*1) |
				(0 /*uSupportPreview*/					<< 1*0)
				,writer);
			uint compatValue=1;
			
			if (sendCompatOptions)
			{
				if (allowPartner) compatValue=3;
				//CLog.Log(Types.Constants.Log.Verbose,"Sent partner flag "+compatValue.ToString());
				new ParameterWriter((byte)Protocol.ClientParameter.EmuleCompatOptions,compatValue,writer);
			}

			if (server==null)
			{
				writer.Write((uint)0);
				writer.Write((ushort)0);    
			}
			else
			{
				writer.Write(server.IP);
				writer.Write(server.MainPort);
			}
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);      
		}
	}

	internal class CFileRequest
	{
		public byte[] FileHash;
		public byte[] Partes;

		public CFileRequest(MemoryStream buffer)
		{
			if (buffer.Length>=16)
			{
				BinaryReader reader=new BinaryReader(buffer);
				FileHash=reader.ReadBytes(16);
				if (buffer.Length>16)
				{
					Partes=ReadFileStatus(reader);

				}
				reader.Close();
				buffer.Close();
				buffer=null;
			}
		}

		public CFileRequest(byte[] Hash,byte[] Partes,MemoryStream buffer)
		{
			uint startpos=(uint)buffer.Position;
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.FileRequest,writer);
			writer.Write(Hash,0,16);
			
			WriteFileStatus(Partes,writer);

			header.Packetlength=(uint)buffer.Position-startpos-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);              
		}

		public static byte[] ReadFileStatus(BinaryReader reader)
		{
			byte[] Partes=null;
			ushort nChunks=reader.ReadUInt16();
			if (nChunks>0)
			{
				Partes=new byte[nChunks];
				short processedBits=0;
				byte bitArray;
				short i;
				while (processedBits!=nChunks)
				{
					bitArray=reader.ReadByte();
					i=0;
					do //for (i=0;i!=8;i++)
					{
						Partes[processedBits]=(((bitArray>>i)&1)==1) ? (byte)Protocol.ChunkState.Complete : (byte)Protocol.ChunkState.Empty;
						processedBits++;
						i++;
					}
					while ((i!=8)&(processedBits!=nChunks));
				}
			}
			return Partes;
		}

		public static void WriteFileStatus(byte[] Partes,BinaryWriter writer)
		{
			if ((Partes!=null)&&(Partes.Length>0))
			{
				ushort nPartes=(ushort)Partes.Length;
				writer.Write(nPartes);
				if (nPartes>0)
				{
					short bitProcesados=0;
					byte bitArray;
					short i;
					while (bitProcesados!=nPartes)
					{
						bitArray=0;
						i=0;
						do
						{
							if ((Protocol.ChunkState)Partes[bitProcesados]==Protocol.ChunkState.Complete) bitArray |= (byte)(1<<i);
							i++;
							bitProcesados++;
						}
						while ((i!=8)&(bitProcesados!=nPartes));
						writer.Write(bitArray);
					}        
				}
			}
		}
	}

	internal class CIDFileChange
	{
		public byte[] FileHash;

		public CIDFileChange(MemoryStream buffer)
		{
			if (buffer.Length==16)
			{
				FileHash=new Byte[16];
				buffer.Read(FileHash,0,16);
				buffer.Close();
				buffer=null;
			}
		}

		public CIDFileChange(byte[] Hash,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.SetRequestFileID);
			header.Packetlength=17;
			header.Serialize(writer);
			writer.Write(Hash,0,16);
//			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
//			writer.Seek(0,SeekOrigin.Begin);
//			header.Serialize(writer);              
		}
	}

	internal class CFileInfo
	{
		public byte[] FileHash;
		public string ClientFileName;

		public CFileInfo(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			//FileHash=new byte[16];
			FileHash=reader.ReadBytes(16);
			ushort LongitudClientFileName;
			LongitudClientFileName=reader.ReadUInt16();
			byte [] buf;//=new byte[longitud];
			buf=reader.ReadBytes(LongitudClientFileName);
			ClientFileName=Encoding.Default.GetString(buf);
			//ClientFileName=new string(reader.ReadChars(LongitudClientFileName));
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}

		public CFileInfo(byte[] in_FileHash,string ClientFileName,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.FileRequestAnswer,writer);
			writer.Write(in_FileHash,0,16);
			writer.Write((ushort)ClientFileName.ToCharArray().Length);
			writer.Write(ClientFileName.ToCharArray());
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);                        
		}
	}

	internal class CFileStatus
	{
		public byte[] FileHash;
		public ushort nChunks;
		public byte[] Chunks;

		public CFileStatus(MemoryStream buffer, bool readHash)
		{
			BinaryReader reader=new BinaryReader(buffer);  

			if (readHash) FileHash=reader.ReadBytes(16);
			if (reader.PeekChar()==-1)
				nChunks=0;
			else
				nChunks=reader.ReadUInt16();
			if (nChunks>0)
			{
				Chunks=new byte[nChunks];
				short bitProcesados=0;
				byte bitArray;
				short i;
				while (bitProcesados!=nChunks)
				{
					bitArray=reader.ReadByte();
					i=0;
					do //for (i=0;i!=8;i++)
					{
						Chunks[bitProcesados] = (((bitArray>>i)&1)==1)? (byte)Protocol.ChunkState.Complete : (byte)Protocol.ChunkState.Empty;
						bitProcesados++;
						i++;
					}
					while ((i!=8)&(bitProcesados!=nChunks));
				}
			}
//			reader.Close();
//			buffer.Close();
//			reader=null;
//			buffer=null;
		}

		public CFileStatus(byte[] in_FileHash,byte[] in_Partes,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.FileState,writer);        
			writer.Write(in_FileHash,0,16);

			WriteFileStatus(writer,in_Partes);

			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);      
		}

		public static void WriteFileStatus(BinaryWriter writer, byte[] in_Partes)
		{
			ushort nChunks;
			if  ((in_Partes==null)||(in_Partes.Length==0)) nChunks=0;
			else nChunks=(ushort)(in_Partes.Length);
			writer.Write(nChunks);
			if (nChunks>0)
			{
				short bitProcesados=0;
				byte bitArray;
				short i;
				while (bitProcesados!=nChunks)
				{
					bitArray=0;
					i=0;
					do
					{
						if ( (Protocol.ChunkState)in_Partes[bitProcesados]==Protocol.ChunkState.Complete )
							bitArray |= (byte)(1<<i);
						i++;
						bitProcesados++;
					}
					while ((i!=8)&(bitProcesados!=nChunks));
					writer.Write(bitArray);
				}        
			}
		}
	}

	internal class CHashSetRequest
	{
		public CHashSetRequest(byte[] in_FileHash,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.HashSetRequest,writer);
			writer.Write(in_FileHash,0,16);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);         
		}
	}

	internal class CHashSetResponse
	{
		public CHashSetResponse(MemoryStream buffer,CFile File)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.HashSetAnswer,writer);			
			writer.Write(File.FileHash);
			writer.Write((ushort)File.HashSet.Count);
			foreach (byte[] PartialHash in File.HashSet)
			{
				writer.Write(PartialHash);
			}
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer); 
		}

		public CHashSetResponse(MemoryStream buffer,ref CFile File)
		{
			BinaryReader reader=new BinaryReader(buffer);
			byte[] FileHash;//=new byte[16];
			FileHash=reader.ReadBytes(16);
			if (!CKernel.SameHash(ref FileHash,ref File.FileHash)) return;
			ArrayList NewHashSet=new ArrayList();
			ushort nChunks=reader.ReadUInt16();
			if (nChunks==0)
			{
				NewHashSet.Add(FileHash);
				//Fichero.GetHashSet().Clear();
				//Fichero.SetHashSet(null);
				File.HashSet=NewHashSet;
			}
			else
			{
				byte[] PartialHash;//=new byte[16];
				while (nChunks>0)
				{
					PartialHash=reader.ReadBytes(16);	
					NewHashSet.Add(PartialHash);
					nChunks--;
				}
				byte[] HashSetChecked=CHash.DoHashSetHash(NewHashSet);
				if (CKernel.SameHash(ref HashSetChecked,ref File.FileHash)) 
				{
					//Fichero.GetHashSet().Clear();
					//Fichero.SetHashSet(null);
					File.HashSet=NewHashSet;
				}
			}
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	internal class CStartDownload
	{
		public CStartDownload(MemoryStream buffer, byte[] in_FileHash)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.StartUploadRequest,writer);
			if (in_FileHash!=null) writer.Write(in_FileHash,0,16);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);         
		}
	}

	internal class CCancelTransfer
	{
		public CCancelTransfer(MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.CancelTransfer,writer);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);         
		}
	}

	internal class CIDClientChange
	{
		public uint NewID;
		public uint ServerID;

		public CIDClientChange(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			NewID=reader.ReadUInt32();
			ServerID=reader.ReadUInt32();
		}
	}

	internal class CSharedFiles
	{
		public CSharedFiles(MemoryStream buffer,CFilesList fileList)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.AskSharedFilesAnswer,writer);
			writer.Write((uint)fileList.Count);
			lock(fileList)
			{
				foreach (CElement Elemento in fileList.Values)
				{
					writer.Write(Elemento.File.FileHash);
					writer.Write((uint)0);
					writer.Write((ushort)0);
					writer.Write(Elemento.File.FileType);
					// file name
					new ParameterWriter((byte)Protocol.FileTag.Name,Elemento.File.FileName,writer);
					// file size
					new ParameterWriter((byte)Protocol.FileTag.Size,Elemento.File.FileSize,writer);
				}
			}
			header.Packetlength = (uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer); 
		}
	}

	internal class CAskSharedFiles
	{
		public CAskSharedFiles(MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.AskSharedFiles,writer);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer); 
		}
	}

	internal class CMuleHello
	{
		public byte VersioneMule;
		public byte VersionEmuleProtocol;
		public byte VersionCompression;
		public byte VersionSourceExchange;
		public byte VersionUDP;
		public byte VersionComments;
		public byte VersionExtendedRequests;
		public uint VersionLphant;
		public uint IDClientCompatible;
		public ushort PortUDP;

		public CMuleHello(MemoryStream buffer,bool isResponse)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header;
			if (isResponse)
				header=new DonkeyHeader((byte)Protocol.ClientCommandExt.eMuleInfoAnswer,writer,Protocol.ProtocolType.eMule);
			else            
				header=new DonkeyHeader((byte)Protocol.ClientCommandExt.eMuleInfo,writer,Protocol.ProtocolType.eMule);
			writer.Write(Protocol.EMULE_VERSION);
			writer.Write(Protocol.EMULE_PROTOCOL_VERSION);
			uint nParameters=8;
			writer.Write(nParameters);
			// compression
			new ParameterWriter(Protocol.ET_COMPRESSION,Protocol.EMULE_VERSION_COMPRESION,writer); 
			// udp port
			new ParameterWriter(Protocol.ET_UDPPORT,(uint)CKernel.Preferences.GetUShort("UDPPort"),writer); 
			// version udp
			new ParameterWriter(Protocol.ET_UDPVER,(uint)Protocol.EMULE_VERSION_UDP,writer);
			// version source exchange
			new ParameterWriter(Protocol.ET_SOURCEEXCHANGE,(uint)Protocol.EMULE_VERSION_SOURCEEXCHANGE,writer);
			// version comments
			new ParameterWriter(Protocol.ET_COMMENTS,(uint)Protocol.EMULE_VERSION_COMMENTS,writer);
			// version extended requests
			new ParameterWriter(Protocol.ET_EXTENDEDREQUEST,(uint)Protocol.EMULE_VERSION_EXTENDEDREQUEST,writer);
			// client compatible
			new ParameterWriter(Protocol.ET_COMPATIBLECLIENT,(byte)Protocol.Client.eLePhant,writer);
			// version lphant
			new ParameterWriter(Protocol.ET_ELEPHANTVERSION,Protocol.ELEPHANT_VERSION,writer);

			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);             
		}

		public CMuleHello(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			VersioneMule=reader.ReadByte();
			if (VersioneMule==0x2B) VersioneMule=0x22;
			VersionEmuleProtocol=reader.ReadByte();
			if (VersionEmuleProtocol==Protocol.EMULE_PROTOCOL_VERSION) 
			{
				if(VersioneMule < 0x25 && VersioneMule > 0x22)
					VersionUDP = 1;

				if(VersioneMule < 0x25 && VersioneMule > 0x21)
					VersionSourceExchange = 1;

				if(VersioneMule==0x24)
					VersionComments = 1;
			}
			PortUDP=0;
			VersionCompression=0;
			VersionSourceExchange=1;
			uint nParametros=reader.ReadUInt32();
			if (VersionEmuleProtocol!=Protocol.EMULE_PROTOCOL_VERSION) return;
			for (int i=0;i!=nParametros;i++)
			{
				CParameterReader ParameterReader=new CParameterReader(reader);
				switch(ParameterReader.id)
				{
					case Protocol.ET_COMPRESSION:
						VersionCompression = (byte)ParameterReader.valorNum;
						break;
					case Protocol.ET_UDPPORT:
						PortUDP = (ushort)ParameterReader.valorNum;
						break;
					case Protocol.ET_UDPVER:
						VersionUDP= (byte)ParameterReader.valorNum;
						break;
					case Protocol.ET_SOURCEEXCHANGE:
						VersionSourceExchange=(byte)ParameterReader.valorNum;
						break;
					case Protocol.ET_COMMENTS:
						VersionComments = (byte)ParameterReader.valorNum;
						break;
					case Protocol.ET_EXTENDEDREQUEST:
						VersionExtendedRequests = (byte)ParameterReader.valorNum;
						break;
					case Protocol.ET_COMPATIBLECLIENT:
						IDClientCompatible = ParameterReader.valorNum;
						break;
					case Protocol.ET_ELEPHANTVERSION:
						VersionLphant = ParameterReader.valorNum & 0x00FFFFFF;
						break;
				}
			}
			if (VersionCompression==0)
			{
				VersionSourceExchange = 0;
				VersionExtendedRequests = 0;
				VersionComments = 0;
				PortUDP = 0;
			}
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	internal class CStartUpload
	{
		public CStartUpload(MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.AcceptUploadRequest,writer);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);         
		}
	}

	internal class CFileBlock
	{
		public uint start;
		public uint end;
		public byte[] FileHash;
		public byte[] buffer;
		public uint position;
		public bool compressed;

		public CFileBlock()
		{
			start=0;
			end=0;
			FileHash=null;
			buffer=null;
			position=0;
			compressed=false;
		}
	}

	internal class CFileBlockRequest
	{
		public ArrayList RequestedBlocks;

		public CFileBlockRequest(MemoryStream buffer)
		{
			byte[] FileHash;
			if (buffer.Length!=40) return;
			CFileBlock block1=new CFileBlock();
			CFileBlock block2=new CFileBlock();
			CFileBlock block3=new CFileBlock();
			RequestedBlocks=new ArrayList();
			BinaryReader reader=new BinaryReader(buffer);
			//FileHash=new Byte[16];
			FileHash=reader.ReadBytes(16);
			block1.start=reader.ReadUInt32();
			block2.start=reader.ReadUInt32();
			block3.start=reader.ReadUInt32();
			block1.end=reader.ReadUInt32();
			block2.end=reader.ReadUInt32();
			block3.end=reader.ReadUInt32();
			block1.FileHash=FileHash;
			block2.FileHash=FileHash;
			block3.FileHash=FileHash;
			if (block1.end>block1.start) RequestedBlocks.Add(block1);
			if (block2.end>block2.start) RequestedBlocks.Add(block2);
			if (block3.end>block3.start) RequestedBlocks.Add(block3);
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}

		public CFileBlockRequest(MemoryStream buffer,byte[] FileHash,ref CFileBlock block1,ref CFileBlock block2,ref CFileBlock block3)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.RequestParts,writer);
			writer.Write(FileHash);
			writer.Write(block1.start);
			writer.Write(block2.start);
			writer.Write(block3.start);
			writer.Write(block1.end+1);
			writer.Write(block2.end+1);
			writer.Write(block3.end+1);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer); 
		}
	}

	internal class CSendBlock
	{
		public CSendBlock(MemoryStream data,uint start,uint end,byte[] FileHash,ref ArrayList UploadDataPackets)
		{
			MemoryStream buffer;
			uint size;
			BinaryReader reader=new BinaryReader(data);
			BinaryWriter writer;
			//byte[] aux_buffer;
			while (start!=end)
			{
				buffer=new MemoryStream();
				writer=new BinaryWriter(buffer);
				DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.SendingPart,writer);
				writer.Write(FileHash);
				if (end-start>10240) size=10240;
				else size=end-start;
				writer.Write(start);
				writer.Write(start+size);
				//aux_buffer=reader.ReadBytes((int)size);
				//writer.Write(aux_buffer);
				writer.Write(reader.ReadBytes((int)size));

				start+=size;
				header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
				writer.Seek(0,SeekOrigin.Begin);
				header.Serialize(writer);  
				UploadDataPackets.Add(buffer);
				writer=null;
				buffer=null;
			}
			reader.Close();
			data.Close();
			data=null;
			reader=null;
		}
	}

	internal class CCompressedPacket
	{
		public CCompressedPacket(ref MemoryStream packet)
		{
			byte[] compressedBuffer=null;
			
			BinaryReader reader=new BinaryReader(packet);
			MemoryStream compressedPacket=new MemoryStream();
			packet.Seek(0,SeekOrigin.Begin);
			BinaryWriter writer=new BinaryWriter(compressedPacket);
			
			writer.Write(reader.ReadBytes(5));
			byte opcode=reader.ReadByte();
			writer.Write(opcode);
			byte[] uncompressedBuffer=reader.ReadBytes((int)packet.Length-6);
			int compressedsize=CCompressedBlockSend.ComprimirBuffer(uncompressedBuffer,ref compressedBuffer);
			if (compressedsize+6>=packet.Length)
				return;
			writer.Write(compressedBuffer);
			compressedPacket.Seek(0,SeekOrigin.Begin);
			
			DonkeyHeader header=new DonkeyHeader(opcode,writer,Protocol.ProtocolType.Packet);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer); 
			packet.Close();
			packet=compressedPacket;
		}
	}

	internal class CCompressedBlockSend
	{
		//cualquiera de las dos funciones de comprimir causa un leak de memoria enorme
		public static int ComprimirBuffer(byte[] in_buffer,ref byte[] out_buffer)
		{
			out_buffer=new byte[in_buffer.Length+300];
			Deflater compresor=new Deflater();
			compresor.SetInput(in_buffer);
			compresor.Flush();
			compresor.Finish();
			int compressedsize=compresor.Deflate(out_buffer,0,(int)(in_buffer.Length)+300);
			compresor=null;
			return compressedsize;
		}

		public static int ComprimirBuffer2(byte[] in_buffer,ref byte[] out_buffer)
		{
			try
			{
				MemoryStream ms = new MemoryStream();
				Stream s = new DeflaterOutputStream(ms);
				s.Write(in_buffer,0,in_buffer.Length);
				s.Close();
				out_buffer = (byte[])ms.ToArray();
				return out_buffer.Length;
			}
			catch
			{
				return in_buffer.Length;
			}
		}

		public CCompressedBlockSend(byte[] data,uint start,uint end,byte[] FileHash,ref ArrayList UploadDataPackets)
		{
			//			byte[] buffercomp=new byte[final-inicio+300];
			//			Deflater descompresor=new Deflater();
			//			descompresor.SetInput(datos);
			//			descompresor.Flush();
			//			int compressedsize=descompresor.Deflate(buffercomp,0,(int)(final-inicio)+300);
			//			descompresor.Finish();
			byte[] buffercomp=null;
			int compressedsize=ComprimirBuffer(data,ref buffercomp);
			if (compressedsize>=end-start)
			{
				buffercomp=null;
				MemoryStream strmdatos=new MemoryStream(data);
				CSendBlock EnvioBloque=new CSendBlock(strmdatos,start,end,FileHash,ref UploadDataPackets);
				return;
			}
			Debug.Write("Compressed comp:"+compressedsize.ToString()+" win: "+Convert.ToString(end-start-compressedsize)+"\n");
			MemoryStream datosComp=new MemoryStream(buffercomp);
			end=start+(uint)compressedsize;
			MemoryStream buffer;
			uint size;
			BinaryReader reader=new BinaryReader(datosComp);
			BinaryWriter writer;
			byte[] aux_buffer;
			while (start!=end)
			{
				buffer=new MemoryStream();
				writer=new BinaryWriter(buffer);
				DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommandExt.CompressedPart,writer,Protocol.ProtocolType.eMule);
				writer.Write(FileHash);
				if (end-start>10240) size=10240;
				else size=end-start;
				writer.Write(start);
				writer.Write(compressedsize);
				aux_buffer=reader.ReadBytes((int)size);
				writer.Write(aux_buffer);

				start+=size;
				header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
				writer.Seek(0,SeekOrigin.Begin);
				header.Serialize(writer);  
				UploadDataPackets.Add(buffer);
				writer=null;
				buffer=null;
			}
			reader.Close();
			datosComp.Close();
			datosComp=null;
			reader=null;						
		}
	}

	internal class CReceivedBlock
	{
		public uint Start;
		public uint End;
		public byte[] FileHash;
		public byte[] Data;
		public CReceivedBlock(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			FileHash=reader.ReadBytes(16);
			Start=reader.ReadUInt32();
			End=reader.ReadUInt32();
			Data=reader.ReadBytes((int)(End-Start+1));
			reader.Close();
			buffer.Close();
			buffer=null;
		}
	}

	internal class CNoFile
	{
		public CNoFile(MemoryStream buffer,byte[] FileHash)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.FileRequestAnswerNoFile,writer);
			writer.Write(FileHash);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);         
		}
	}

	internal class CReceivedCompressedBlock
	{
		public uint Start;
		public uint End;
		public byte[] FileHash;
		public byte[] Data;

		public CReceivedCompressedBlock(ref MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			FileHash=reader.ReadBytes(16);
			Start=reader.ReadUInt32();
			End=reader.ReadUInt32()+Start;
			Data=reader.ReadBytes((int)buffer.Length-(int)buffer.Position+1);
			reader.Close();
			buffer.Close();
			buffer=null;
		}

		public static int Uncompress(ref byte[] Data)
		{
			int maxsize=Data.Length*10+300;
			if (maxsize>50000) maxsize=50000;
			byte[] outputData=new byte[maxsize];
			MemoryStream dataStream=new MemoryStream(Data);
			InflaterInputStream inflater=new InflaterInputStream(dataStream);
			//int res=descompresor.Read(packetsalida,0,packetsalida.Length);
			//if (res>0)
			int res;
			int resTotal=0;
			MemoryStream uncompressedStream=new MemoryStream();
			do
			{
				if (inflater.Position==Data.Length) res=0;
				else
					try
					{
						res=inflater.Read(outputData,0,outputData.Length);
					}
					catch 
					{
						res=0;
					}
				if (res>0)
				{
					uncompressedStream.Write(outputData,0,res);
				}
				resTotal+=res;
			}
			while (res>0);
			if (resTotal==0)
			{
				return 0;
			}
			else
			{
				inflater.Close();
				inflater=null;
				dataStream.Close();
				dataStream=null;
				Data=null;
				Data=new byte[resTotal];
				uncompressedStream.Seek(0,SeekOrigin.Begin);
				uncompressedStream.Read(Data,0,resTotal);
				uncompressedStream.Close();
				uncompressedStream=null;
				return resTotal;
			}
		}
	}

	internal class CMuleQR
	{
		ushort QR;

		public CMuleQR(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			QR=reader.ReadUInt16();
		}

		public CMuleQR(MemoryStream buffer,ushort QR)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header;
			header=new DonkeyHeader((byte)Protocol.ClientCommandExt.QueueRanking,writer,Protocol.ProtocolType.eMule);
			writer.Write(QR);
			byte[] empty=new byte[10];
			writer.Write(empty);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);   
		}
	}

	class CQueueFullUDP
	{
		public CQueueFullUDP(MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Write((byte)Protocol.ProtocolType.eMule);
			writer.Write((byte)Protocol.ClientCommandExtUDP.QueueFull);
		}
	}

	class CFileNotFoundUDP
	{
		public CFileNotFoundUDP(MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Write((byte)Protocol.ProtocolType.eMule);
			writer.Write((byte)Protocol.ClientCommandExtUDP.FileNotFound);
		}
	}

	class CQueuePositionUDP
	{
		public CQueuePositionUDP(MemoryStream buffer,ushort queuePosition, byte[] partes, byte version)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Write((byte)Protocol.ProtocolType.eMule);
			writer.Write((byte)Protocol.ClientCommandExtUDP.ReaskAck);
			if ((version>3)&&(partes!=null))
				CFileStatus.WriteFileStatus(writer,partes);
			writer.Write(queuePosition);
		}
	}

	class CFileReaskUDP
	{
		public byte[] partes;
		public byte[] FileHash;

		public CFileReaskUDP(MemoryStream buffer,byte[] FileHash,byte version,byte[] partes)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			writer.Write((byte)Protocol.ProtocolType.eMule);
			writer.Write((byte)Protocol.ClientCommandExtUDP.ReaskFilePing);
			writer.Write(FileHash);
			if (version>3)
				CFileRequest.WriteFileStatus(partes,writer);
			if (version==3)
				writer.Write((ushort)0);//ignore version 3 complete sources count
		}
		public CFileReaskUDP(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			if (buffer.Length<16) return;
			FileHash=reader.ReadBytes(16);
			if (buffer.Length==18) return; //ignore version 3 complete sources count
			if (buffer.Length>18)
			{
				partes=CFileRequest.ReadFileStatus(reader);
			}
		}

	}

	public struct stDatosFuente
	{
		public uint IP;
		public ushort Port;
		public uint ServerIP;
		public ushort ServerPort;
	}

	internal class CRequestSourceExchange
	{
		public byte[] FileHash;

		public CRequestSourceExchange(MemoryStream buffer,byte[] FileHash)
		{
			BinaryWriter writer = new BinaryWriter(buffer);
			DonkeyHeader header = new DonkeyHeader((byte)Protocol.ClientCommandExt.SourcesRequest,Protocol.ProtocolType.eMule);
			header.Packetlength=17;
			header.Serialize(writer);
			writer.Write(FileHash);
//			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
//			writer.Seek(0,SeekOrigin.Begin);
//			header.Serialize(writer);     
		}

		public CRequestSourceExchange(MemoryStream buffer)
		{
			BinaryReader reader = new BinaryReader(buffer);
			FileHash = reader.ReadBytes(16);
			reader.Close();
			buffer.Close();
			reader = null;
			buffer = null;			
		}
	}

	internal class CSourceExchangeResponse
	{
		public byte[] FileHash;
		public ushort nSources;
		public stDatosFuente[] Sources;

		public CSourceExchangeResponse(MemoryStream buffer, int version)
		{
			BinaryReader reader=new BinaryReader(buffer);
			FileHash=reader.ReadBytes(16);
			nSources=reader.ReadUInt16();
			Sources=new stDatosFuente[nSources];
			for (int i=0; i!=nSources; i++)
			{
				Sources[i].IP=reader.ReadUInt32();
				Sources[i].Port=reader.ReadUInt16();
				Sources[i].ServerIP=reader.ReadUInt32();
				Sources[i].ServerPort=reader.ReadUInt16();
				if (version>1) reader.ReadBytes(16);
			}
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}

		public CSourceExchangeResponse(MemoryStream buffer,byte[] FileHash,uint RequesterID,ushort RequesterPort, int version)
		{
			//Hashtable IPList=new Hashtable();
			ArrayList clientsList=new ArrayList();
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommandExt.SourcesResult,writer,Protocol.ProtocolType.eMule);
			CElement Element=(CElement)CKernel.FilesList[FileHash];
			nSources=0;
			writer.Write(FileHash);
			writer.Write(nSources);
			uint myID=CKernel.Preferences.GetUInt("ID");
			if ((Element!=null)&&(Element.SourcesList!=null))
			{
				lock(Element.SourcesList.SyncRoot())
				{
					foreach (CClient Client in Element.SourcesList)
					{
						if ((Client.DownloadState != Protocol.DownloadState.None)&&
							(Client.UserID>Protocol.LowIDLimit)&&
							(Client.UserHash!=null)&&
							(Client.UserID!=RequesterID)&&
							(Client.UserID!=myID))
						{
							//IPList[((ulong)Client.UserID << 16) + Client.Port]=((ulong)Client.ServerIP<<16) + Client.ServerPort;
							clientsList.Add(Client);
							nSources++;
						}
						if (nSources>=200) break;
					}
				}
				if (nSources<200)
				{
					int i=0;
					CClient QueueClient;
					while (i<CKernel.Queue.Count)
					{
						QueueClient=(CClient)CKernel.Queue[i];
						if ((QueueClient.UploadElement==Element)&&
							(QueueClient.UserID > Protocol.LowIDLimit)&&
							(QueueClient.DownloadState==Protocol.DownloadState.OnQueue)&&
							(QueueClient.UserHash!=null)&&
							(QueueClient.UserID!=RequesterID)&&
							(QueueClient.UserID!=myID))
						{
							//IPList[((ulong)QueueClient.UserID<<16)+QueueClient.Port]=((ulong)QueueClient.ServerIP<<16)+QueueClient.ServerPort;
							if (!clientsList.Contains(QueueClient)) clientsList.Add(QueueClient);
							nSources++;
						}
						if (nSources>=200) break;
						i++;
					}
				}

			}	
/*			//do not send oursef
			IPList.Remove(((ulong)CKernel.Preferences.GetUInt("ID")<<16)+CKernel.Preferences.GetUShort("TCPPort"));
			//do not send himself
			IPList.Remove(((ulong)RequesterID<<16)+RequesterPort);
			foreach (ulong ipPort in IPList.Keys)
			{
				uint IP=(uint)(ipPort>>16);
				ushort Port=(ushort)(ipPort&0x00000000FFFF);
				//ushort Port=(ushort)(ipPort-((ulong)IP<<16));
				
				ulong ipServerPort=(ulong)IPList[ipPort];
				uint ServerIP=(uint)(ipServerPort>>16);
				ushort ServerPort=(ushort)(ipServerPort&0x00000000FFFF);
				
				writer.Write(IP);
				writer.Write(Port);
				writer.Write(ServerIP);
				writer.Write(ServerPort);
			}
*/
			foreach (CClient client in clientsList)
			{
				writer.Write(client.UserID);
				writer.Write(client.Port);
				writer.Write(client.ServerIP);
				writer.Write(client.ServerPort);
				if (version>1)
					writer.Write(client.UserHash);
			}
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);
			writer.Seek(16,SeekOrigin.Current);
			writer.Write((ushort)clientsList.Count);
		}
	}

	internal class CComment
	{
		public byte rating;
		public string comment;

		public CComment(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			rating=reader.ReadByte();
			int length=reader.ReadInt32();
			if (length>128) length=128;
			if (length==0) comment="";
			else
			{
				byte[] buf=reader.ReadBytes(length);
				comment=Encoding.Default.GetString(buf);
			}
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}

		public CComment(MemoryStream buffer,byte rating,string comment)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommandExt.FileDescription,writer,Protocol.ProtocolType.eMule);
			writer.Write(rating);
			if (comment.Length>128) comment=comment.Substring(0,128);
			byte[] byteStringValue=Encoding.Default.GetBytes(comment);
			writer.Write((uint)byteStringValue.Length);
			writer.Write(byteStringValue);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);
		}
	}

	internal class CClientMessage
	{
		public string Message;

		public CClientMessage(MemoryStream buffer)
		{
			BinaryReader reader=new BinaryReader(buffer);
			ushort length=reader.ReadUInt16();
			if (length>128) length=128;
			if (length==0) Message="";
			else
			{
				byte[] buf=reader.ReadBytes(length);
				Message=Encoding.Default.GetString(buf);
			}
			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}

		public CClientMessage(MemoryStream buffer,string message)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.Message,writer,Protocol.ProtocolType.eDonkey);
			if (message.Length>128) Message=message.Substring(0,128);
			byte[] byteStringValue=Encoding.Default.GetBytes(message);
			writer.Write((ushort)byteStringValue.Length);
			writer.Write(byteStringValue);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);
		}
	}

	internal class CCrumbSetRequest
	{
		public byte[] FileHash;

		public CCrumbSetRequest(MemoryStream buffer)
		{
			if (buffer.Length==16)
			{
				FileHash=new Byte[16];
				buffer.Read(FileHash,0,16);
				buffer.Close();
				buffer=null;
			}
		}

		public CCrumbSetRequest(byte[] Hash,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.CrumbSetRequest,writer);
			writer.Write(Hash,0,16);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);              
		}
	}

	internal class CCrumbSetResponse
	{
		public CCrumbSetResponse(MemoryStream buffer,CFile File)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.HashSetAnswer,writer);			
			writer.Write(File.FileHash);
			writer.Write((byte)1);
			foreach (byte[] PartialHash in File.HashSet)
			{
				writer.Write(PartialHash);
			}
			writer.Write((byte)1);
			foreach (byte[] CrumbHash in File.CrumbsHashSet)
			{
				writer.Write(CrumbHash);
			}
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer); 
		}

		public CCrumbSetResponse(MemoryStream buffer,ref CFile File)
		{
			BinaryReader reader=new BinaryReader(buffer);
			byte[] FileHash;
			FileHash=reader.ReadBytes(16);
			if (!CKernel.SameHash(ref FileHash,ref File.FileHash)) return;
			if (reader.ReadByte()!=1) 
				return;
			ArrayList NewHashSet=new ArrayList();
			ArrayList NewCrumbsHashSet=new ArrayList();
			if (reader.PeekChar()<0)
			{
				NewHashSet.Add(FileHash);
				if (File.HashSetNeeded()) File.HashSet=NewHashSet;
				NewCrumbsHashSet.Add(FileHash);
				File.CrumbsHashSet=NewHashSet;
				return;
			}
			byte[] PartialHash;
			for (int i=0;i<CHash.GetChunksCount(File.FileSize);i++)
			{
				PartialHash=reader.ReadBytes(16);
				NewHashSet.Add(PartialHash);
			}
			if (File.HashSetNeeded())
			{
				byte[] HashSetChecked=CHash.DoHashSetHash(NewHashSet);
				if (CKernel.SameHash(ref HashSetChecked,ref File.FileHash)) File.HashSet=NewHashSet;
			}
			//crumbs hashes
			if (reader.ReadByte()!=1) 
				return;
			for (int i=0;i<CHash.GetCrumbsCount(File.FileSize);i++)
			{
				PartialHash=reader.ReadBytes(8);	
				NewCrumbsHashSet.Add(PartialHash);
			}
			//we can not check here hashes integrity we must trust edonkey, at least we check that the number matches
			if (CHash.GetCrumbsCount(File.FileSize)!=NewCrumbsHashSet.Count) return;
			CLog.Log(Types.Constants.Log.Verbose,"crumbset ok for "+File.FileName);
			File.CrumbsHashSet=NewCrumbsHashSet;

			reader.Close();
			buffer.Close();
			reader=null;
			buffer=null;
		}
	}

	internal class CHordeSlotRequest
	{
		public byte[] FileHash;

		public CHordeSlotRequest(MemoryStream buffer)
		{
			if (buffer.Length==16)
			{
				FileHash=new Byte[16];
				buffer.Read(FileHash,0,16);
				buffer.Close();
				buffer=null;
			}
		}

		public CHordeSlotRequest(byte[] Hash,MemoryStream buffer)
		{
			BinaryWriter writer=new BinaryWriter(buffer);
			DonkeyHeader header=new DonkeyHeader((byte)Protocol.ClientCommand.HordeSlotRequest,writer);
			writer.Write(Hash,0,16);
			header.Packetlength=(uint)writer.BaseStream.Length-header.Packetlength+1;
			writer.Seek(0,SeekOrigin.Begin);
			header.Serialize(writer);              
		}
	}

}