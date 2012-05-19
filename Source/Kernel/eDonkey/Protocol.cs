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
	/// Protocol includes the eDonkey network protocol
	/// </summary>
	internal class Protocol
	{
		public enum Header: byte
		{
			eDonkey						= 0xE3,			// OP_EDONKEYHEADER
		}

		public enum ProtocolType : byte
		{
			eDonkey						= 0xE3,			// OP_EDONKEYPROT
			Packet						= 0xD4,			// OP_PACKEDPROT
			eMule						= 0xC5,			// OP_EMULEPROT
			mlDonkey					= 0x00,			// OP_MLDONKEYPROT
		}

		/// <summary>
		/// ServerCommand includes all supported server commands (TCP)
		/// </summary>
		public enum ServerCommand : byte
		{
			LoginRequest				= 0x01,
			ServerMessage				= 0x38,
			IDChange					= 0x40,
			GetServerList				= 0x14,
			OfferFiles					= 0x15,
			SearchRequest				= 0x16,
			ServerList					= 0x32,
			SearchResult				= 0x33,
			GetSources					= 0x19,
			QueryMoreResults			= 0x21,
			FoundSources				= 0x42,
			CallBackRequest				= 0x1C,
			ServerState					= 0x34,
			ServerIdent					= 0x41,
			CallBackRequested			= 0x35,
		}

		/// <summary>
		/// ServerCommand includes all supported server commands (UDP)
		/// </summary>
		public enum ServerCommandUDP : byte
		{
			GlobalStateRequest			= 0x96,
			GlobalStateResult			= 0x97,
			GlobalSearchRequest			= 0x98,
			GlobalSearchRequest2		= 0x92,
			GlobalSearchResult			= 0x99,
			GlobalGetSources			= 0x9A,
			GlobalFoundSources			= 0x9B,
			GlobalCallBackRequest		= 0x9C,
			DescriptionRequest			= 0xA2,
			DescriptionResult			= 0xA3,
		}

		/// <summary>
		/// ClientCommand includes all supported client commands (TCP)
		/// </summary>
		public enum ClientCommand : byte
		{
			Hello						= 0x01,
			HelloAnswer					= 0x4C,
			ChangeClientID				= 0x4D,
			FileRequest					= 0x58,
			FileRequestAnswer			= 0x59,
			FileState					= 0x50,
			StartUploadRequest			= 0x54,
			AcceptUploadRequest			= 0x55,
			CancelTransfer				= 0x56,
			OutOfPartRequest			= 0x57,
			ChangeSlot					= 0x5B,
			RequestParts				= 0x47,
			SendingPart					= 0x46,
			SetRequestFileID			= 0x4F,
			HashSetRequest				= 0x51,
			HashSetAnswer				= 0x52,
			AskSharedFiles				= 0x4A,
			AskSharedFilesAnswer		= 0x4B,
			Message						= 0x4E,
			QueueRank					= 0x5C,
			FileRequestAnswerNoFile		= 0x48,
			EndOfDownload				= 0x49,
			HordeSlotRequest			= 0x65,
			CrumbSetRequest				= 0x69,
			CrumbSetResponse			= 0x68,
		}

		/// <summary>
		/// ClientCommandExt includes all supported extended client commands (TCP)
		/// </summary>
		public enum ClientCommandExt : byte
		{
			eMuleInfo					= 0x01,
			eMuleInfoAnswer				= 0x02,
			QueueRanking				= 0x60,
			SourcesRequest				= 0x81,
			SourcesResult				= 0x82,
			CompressedPart				= 0x40,
			FileDescription				= 0x61,
		}

		/// <summary>
		/// ClientCommandExtUDP includes all supported extended client commands (UDP)
		/// </summary>
		public enum ClientCommandExtUDP : byte
		{
			ReaskFilePing				= 0x90,
			ReaskAck					= 0x91,
			FileNotFound				= 0x92,
			QueueFull					= 0x93,
		}

		/// <summary>
		/// ClientParameter includes the parameters for client hello packet
		/// </summary>
		public enum ClientParameter : byte
		{
			Name						= 0x01,
			Version						= 0x11,
			Port						= 0x0F,
			Compression					= 0x20,
			EmuleVersion				= 0xFB,
			Emule_MiscOptions1			= 0xFA,
			EmuleUDPPort				= 0xF9,
			EmuleCompatOptions			= 0xEF
		}

		public const byte MaxRequestsPerServer = 35;
		public const uint MaxSearchResults = 300;

		/// <summary>
		/// ServerFlagTCP includes all supported TCP server flags
		/// </summary>
		public enum ServerFlagTCP : uint
		{
			Compression					= 0x00000001,
		}

		/// <summary>
		/// ServerFlagUDP includes all supported UDP server flags
		/// </summary>
		public enum ServerFlagUDP : uint
		{
			GetSources					= 0x00000001,
			GetFiles					= 0x00000002,
		}

		/// <summary>
		/// Client includes all known clients
		/// </summary>
		public enum Client : byte
		{
			eMule						= 0,
			cDonkey						= 1,
			lMule						= 2,
			aMule						= 3,
			Shareaza					= 4,
			eLePhant					= 20,
			eDonkeyHybrid				= 50,
			eDonkey						= 51,
			mlDonkey					= 10,
			eMuleOld					= 53,
			Unknown						= 54,
		}

		public const byte EDONKEYVERSION = 0x3C;
		public const byte EMULE_PROTOCOL_VERSION=0x01;

		public const byte EMULE_VERSION = 0x30; // it identified the version of emule
		public const uint EMULE_VERSION_MAJOR = ELEPHANT_VERSION_MAJOR;
		public const uint EMULE_VERSION_MIN = ELEPHANT_VERSION;
		public const uint EMULE_VERSION_REVISON = 0;
		public const uint EMULE_VERSION_COMPLEX=(uint)(
			(int)Protocol.Client.eLePhant<<24)|
			(Protocol.EMULE_VERSION_MAJOR<<17)|
			(Protocol.EMULE_VERSION_MIN<<10)|
			(Protocol.EMULE_VERSION_REVISON<<7);

		public const uint EMULE_VERSION_COMPRESION=0x01;
		public const uint EMULE_VERSION_SOURCEEXCHANGE=0x02;
		public const uint EMULE_VERSION_COMMENTS=0x01;
		public const uint EMULE_VERSION_EXTENDEDREQUEST=0x01;
		public const uint EMULE_VERSION_UDP=0x04;

		public const uint ELEPHANT_VERSION=1;
		public const uint ELEPHANT_VERSION_MAJOR=1+1;

		public const byte ET_COMPRESSION=0x20;
		public const byte ET_UDPPORT=0x21;
		public const byte ET_UDPVER=0x22;
		public const byte ET_SOURCEEXCHANGE=0x23;
		public const byte ET_COMMENTS=0x24;
		public const byte ET_EXTENDEDREQUEST=0x25;
		public const byte ET_COMPATIBLECLIENT=0x26;
		public const byte ET_ELEPHANTVERSION=0x99;

		public const string HordeParameter="pr";
		public const uint HORDE_VERSION = 1;
		/// <summary>
		/// UploadState includes the different upload states of client
		/// </summary>
		public enum UploadState : byte
		{
			None						= 0,
			Pending						= 1,
			OnQueue						= 2,
			Uploading					= 4,
			Banned						= 6,
			Error						= 7,
			WaitCallBack				= 8,
			Connecting					= 9,
			LowToLowIP					= 10,
		}

		/// <summary>
		/// DownloadState includes the different download states of client
		/// </summary>
		public enum DownloadState : byte
		{
			None						= 0,
			Downloading					= 1,
			OnQueue						= 2,
			Connecting					= 3,
			WaitCallBack				= 4,
			NoNeededParts				= 5,
			Banned						= 6,
			Error						= 7,
			Connected					= 9,
			LowToLowIP					= 10,
			ReqestHashSet				= 11,
			TooManyConnections			= 12,
		}

		/// <summary>
		/// ConnectionReason includes the reasons why connection can be closed
		/// </summary>
		public enum ConnectionReason : byte
		{
			CannotConnect				= 1,
			ClosedConnection			= 2,
			NullPacket					= 3,
			NullID						= 4,
			InvalidHeader				= 5,
			TimeOut						= 6,
		}

		public const byte MaxParalelConnections = 3;

		/// <summary>
		/// ServerTag includes all supportet tags for binary files (server.met)
		/// </summary>
		public enum ServerTag : byte
		{
			Name						= 0x01,
			Description					= 0x0B,
			Ping						= 0x0C,
			Preference					= 0x0E,
			Fails						= 0x0D,
			DynIP						= 0x85,
			LastPing					= 0x86,
			MaxUsers					= 0x87,
		}

		/// <summary>
		/// ServerMet includes all supportet protocols for binary files (server.met)
		/// </summary>
		public enum ServerMet : byte
		{
			eDonkey						= 0xE0,
			eMule						= 0x0E,
		}

		/// <summary>
		/// KnownMet includes all supportet protocols for binary files (known.met)
		/// </summary>
		public enum KnownMet : byte
		{
			eDonkey						= 0x0E,
		}

		public enum PartMet : byte
		{
			eMule						=0xE0,
			eDonkeyOld					=0xE2, //really it is E002
			eDonkeyNew					=0xE1,
		}
		/// <summary>
		/// FileTag includes all supportet tags for binary part files (part.met)
		/// </summary>
		public enum FileTag : byte
		{
			Name						= 0x01,
			Size						= 0x02,
			Type						= 0x03,
			Format						= 0x04,
			LastSeenComplete			= 0x05,
			Transfered					= 0x08,
			GapStart					= 0x09,
			GapEnd						= 0x0A,
			PartFileName				= 0x12,
			Priority					= 0x13,
			State						= 0x14,
			Sources						= 0x15,
			Permissions					= 0x16,
			UploadPriority				= 0x17,
			// eLePhant CFile
			Comment						= 0x50,
			Rating						= 0x51,
			Category					= 0x53,
			CorruptedPart				= 0x70,
			//used for server search results:
			Completed					= 0x30,
		}
		public const string FT_MAXSOURCES="lphantMaxSources";
		public const string FT_COMMENT="lphantComment";
		public const string FT_RATING="lphantRating";

		public enum FileExtraTags
		{
//			FT_MEDIA_ARTIST		=	"Artist",
//			FT_MEDIA_ALBUM		=	"Album",
//			FT_MEDIA_TITLE		=	"Title",
//			FT_MEDIA_LENGTH		=	"length",
//			FT_MEDIA_BITRATE	=	"bitrate",
//			FT_MEDIA_CODEC		=	"codec",
			Artist,
			Album,
			Title,
			length,
			bitrate,
			codec,
		}
		// file
		public const uint PartSize = 9728000;
		public const uint CrumbSize = 486400;
		public const uint BlockSize = 184320;

		/// <summary>
		/// ChunkState includes the different chunk states
		/// </summary>
		public enum ChunkState : byte
		{
			Complete					= 3,
			Completing					= 2, // part in process to which the last block has been requested
			InProgress					= 1, // the part has stuffed blocks but not this flood
			Empty						= 0,
		}

		/// <summary>
		/// FileState includes all file states
		/// </summary>
		public enum FileState : byte
		{
			Ready						= 0,
			Pause						= 1,
			Stopped						= 2,
			Completing					= 8,
			Complete					= 9,
		}

		public enum ParameterType : byte
		{
			Hash						= 0x01,
			String					= 0x02,
			UInt32					= 0x03,
			Float						= 0x04,
			Bool						= 0x05,
			BoolArray				= 0x06,
			Blob						= 0x07,
			UInt16					= 0x08,
			Byte						= 0x09,
		}

		public const byte ConnectionRetries = 2;
		public const uint LowIDLimit = 16777216;
		public const int ConnectionTimeOut = 40;//000;
		public const int TransferConnectionTimeOutFactor = 5;

		public const int ReaskServerTCP = 13;// 13 minutes
		public const uint ReaskServerUDP =  3000000000; //300 sec = 5.0 minutes
		public const int ReaskClient = 20;//minutes
		public const long ReaskSameSource = 12000000000; // 1200 s = 20 mminutes
		public const long ReaskSourceAtStartUp = 200000000; //20 s 
		public const long StartUpPeriod = 1200000000;//120 sec = 2.0 minutes
		public const long ReaskSource = 2000000000;//200 sec = 3.33 minutes
		public const int ReaskPingUDP = 240;//minutes 
		public const int ReaskNextTCPFile = 20; //seconds
		public const int RetryServerListConnection = 300; //seconds

		public const int MinPurgeQueueTime = 45; // 60
		public const int MinClientReask = 21;
		public const int MinClientBan = 15;
		
		public const int SourcesPerTCPFrame = 15;
		public const float MaxPartners = 0.35F;
		public const int BadPartnerTime = 90; //minutes
		public const byte MaxRetriesLowID = 3; //The maximun number of retries
	}
}