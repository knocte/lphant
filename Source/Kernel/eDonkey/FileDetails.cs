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
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	[Serializable]
	public struct sFileDetails
	{
		public string FileName;
		public string DiskFileName;
		
		public byte Type; //values in Constants.FileType
		public Hashtable ListDetails; //keys depends type value: Constants.Avi, .Mp3, .Ogm, .Zip

	}


	/// <summary>
	/// Descripción breve de Class1.
	/// </summary>
	internal class CFileDetails 
	{
		
		#region Instance fields

		private bool m_Loaded;
		private sFileDetails m_Details;

		#endregion

	
		#region Propertys	

		public sFileDetails Details
		{
			get
			{
				if (!m_Loaded) m_GetInfo();
				return m_Details;
			}
		}
		public string FileName
		{
			get
			{
				return m_Details.FileName;
			}
			set 
			{
				m_Details.FileName = value;
			}
		}

		public string DiskFileName
		{
			get
			{
				return m_Details.DiskFileName;
			}
			set 
			{
				m_Details.DiskFileName = value;
			}
		}

		#endregion


		#region Constructors & Initialization

		public CFileDetails()
		{
			m_Loaded = false;   
			m_Details.ListDetails	= new Hashtable();   

		}

		public CFileDetails(string diskFileName, string fileName)
		{
			m_Loaded = false;
			m_Details.DiskFileName	= diskFileName;
			m_Details.FileName		= fileName;
			m_Details.ListDetails	= new Hashtable();
		}

		public CFileDetails(string diskFileName)
		{
			m_Loaded = false;
			m_Details.DiskFileName	= diskFileName;
			m_Details.ListDetails	= new Hashtable();
		}

		#endregion	
	

		private bool m_GetInfo()
		{
			m_Loaded			= false;

			string fileName		= m_Details.FileName;
			string diskFile		= m_Details.DiskFileName;

			FileStream knownfile;		
				
			try 
			{
				knownfile	= new FileStream(diskFile,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
			} 
			catch 
			{
				m_Loaded	= false;
				Debug.WriteLine ("[FILE DETAILS] Opening file error: " + fileName );
				
				return m_Loaded;
			}

			if (!m_Loaded) m_Loaded = m_AviInfo(knownfile);



			knownfile.Close();
			return m_Loaded;
		}

		/// <summary>
		/// function to convert XXXXX bits to the format X.XXX.XXX bits (X,XXX Gb/Mb/...)
		/// </summary>
		private string m_BitsToString(int bits)
		{
			return bits.ToString();
		}

		/// <summary>
		/// function to convert XXXXX secs to the format X h XX min XX secs
		/// </summary>
		private string m_SecsToString(int secs)
		{
			TimeSpan tiempo = new TimeSpan(0,0,secs);
			return  tiempo.ToString();
		}



		private bool m_AviInfo (FileStream knownfile)
		{
				  
			try 
			{
				if(m_Details.ListDetails.ContainsKey(Constants.Avi.Size)) return true;
				
				BinaryReader reader	= new BinaryReader(knownfile);
				string r_string;
				knownfile.Position=8;


				r_string=new string(reader.ReadChars(3));
			

				if (r_string=="AVI") 
				{
			
					m_Details.Type			= (byte)Constants.FileType.Avi;
					knownfile.Position		= 4;

					//file size
					int file_size = reader.ReadInt32();
					m_Details.ListDetails.Add(Constants.Avi.Size, m_BitsToString(file_size));

			
					// header sizes
					knownfile.Position		= 28;
					int AviHeaderSize		= reader.ReadInt32();
					int AviHeaderStart		= 32;
					int VHeaderStart		= AviHeaderStart + AviHeaderSize + 20;

					//misc
					knownfile.Position		= AviHeaderStart;
					int Msecs				= reader.ReadInt32();

					knownfile.Position		= AviHeaderStart + 16;
					int LengthInFrames		= reader.ReadInt32();

					//fps
					double fps				= (double)1000000 / (double)Msecs;
					m_Details.ListDetails.Add(Constants.Avi.VFps, fps.ToString() + " fps");
			
					// length
					int video_length = ((int)(LengthInFrames / fps));
					m_Details.ListDetails.Add(Constants.Avi.Length, m_SecsToString((int)(video_length)));

					// video width
					knownfile.Position		= AviHeaderStart + 32;
					int video_width	= reader.ReadInt32();
					m_Details.ListDetails.Add(Constants.Avi.Width, video_width.ToString() + " pxs");


					// video height
					knownfile.Position		= AviHeaderStart + 36;
					int video_height	= reader.ReadInt32();
					m_Details.ListDetails.Add(Constants.Avi.Height, video_height.ToString() + " pxs");

					// video codec
					knownfile.Position		= VHeaderStart + 4;
					string video_codec = new string(reader.ReadChars(4));
				
					switch (video_codec)
					{
						case "div3":		
							video_codec = "DivX MPEG-4 v3";
							break; 
						case "div4":		
							video_codec = "DivX MPEG-4 v4";
							break; 
						case "divx":		
							video_codec = "DivX 4/5";
							break;
						case "div2":		
							video_codec = "MS MPEG-4 v2";
							break;
						case "mp43":		
							video_codec = "Microcrap MPEG-4 v3";
							break;
						case "mp42":	
							video_codec = "Microcrap MPEG-4 v2";							
							break;
						default:
							break;
					}
					m_Details.ListDetails.Add(Constants.Avi.VCodec, video_codec);
				

					// header sizes
					knownfile.Position		= AviHeaderStart + AviHeaderSize + 4;
					int VHeaderSize			= reader.ReadInt32();

					int AHeaderStart		= VHeaderStart + VHeaderSize + 8;	//first databyte of audio header

					knownfile.Position		= AHeaderStart - 4;
					int AStrhSize			= reader.ReadInt32();

					// audio codec
					knownfile.Position		= AHeaderStart + AStrhSize + 8;
					int audioCodec			= reader.ReadInt16();
					string audio_Codec;
					switch (audioCodec)
					{
						case 0:
							audio_Codec	= "Unknown";//Microsoft Corporation
							break;
						case 1:
							audio_Codec	= "PCM"; //Microsoft Corporation
							break;
						case 0x0002: 
							audio_Codec="ADPCM"; //Microsoft Corporation 
							break;
						case 0x0003: 
							audio_Codec="IEEE_FLOAT"; //Microsoft Corporation 
							break;
						case 0x0004: 
							audio_Codec="VSELP"; //Compaq Computer Corp. 
							break;
						case 0x0005: 
							audio_Codec="IBM_CVSD"; //IBM Corporation 
							break;
						case 0x0006: 
							audio_Codec="ALAW"; //Microsoft Corporation 
							break;
						case 0x0007: 
							audio_Codec="MULAW"; //Microsoft Corporation 
							break;
						case 0x0008: 
							audio_Codec="DTS"; //Microsoft Corporation 
							break;
						case 0x0009: 
							audio_Codec="DRM"; //Microsoft Corporation 
							break;
						case 0x0010: 
							audio_Codec="OKI_ADPCM"; //OKI 
							break;
						case 0x0011: 
							audio_Codec="DVI_ADPCM"; //Intel Corporation 
							break;
						case 0x0012: 
							audio_Codec="MEDIASPACE_ADPCM"; //Videologic 
							break;
						case 0x0013: 
							audio_Codec="SIERRA_ADPCM"; //Sierra Semiconductor Corp 
							break;
						case 0x0014: 
							audio_Codec="G723_ADPCM"; //Antex Electronics Corporation 
							break;
						case 0x0015: 
							audio_Codec="DIGISTD"; //DSP Solutions, Inc. 
							break;
						case 0x0016: 
							audio_Codec="DIGIFIX"; //DSP Solutions, Inc. 
							break;
						case 0x0017: 
							audio_Codec="DIALOGIC_OKI_ADPCM"; //Dialogic Corporation 
							break;
						case 0x0018: 
							audio_Codec="MEDIAVISION_ADPCM"; //Media Vision, Inc. 
							break;
						case 0x0019: 
							audio_Codec="CU_CODEC"; //Hewlett-Packard Company 
							break;
						case 0x0020: 
							audio_Codec="YAMAHA_ADPCM"; //Yamaha Corporation of America 
							break;
						case 0x0021: 
							audio_Codec="SONARC"; //Speech Compression 
							break;
						case 0x0022: 
							audio_Codec="DSPGROUP_TRUESPEECH"; //DSP Group, Inc 
							break;
						case 0x0023: 
							audio_Codec="ECHOSC1"; //Echo Speech Corporation 
							break;
						case 0x0024: 
							audio_Codec="AUDIOFILE_AF36"; //Virtual Music, Inc. 
							break;
						case 0x0025: 
							audio_Codec="APTX"; //Audio Processing Technology 
							break;
						case 0x0026: 
							audio_Codec="AUDIOFILE_AF10"; //Virtual Music, Inc. 
							break;
						case 0x0027: 
							audio_Codec="PROSODY_1612"; //Aculab plc 
							break;
						case 0x0028: 
							audio_Codec="LRC"; //Merging Technologies S.A. 
							break;
						case 0x0030: 
							audio_Codec="DOLBY_AC2"; //Dolby Laboratories 
							break;
						case 0x0031: 
							audio_Codec="GSM610"; //Microsoft Corporation 
							break;
						case 0x0032: 
							audio_Codec="MSNAUDIO"; //Microsoft Corporation 
							break;
						case 0x0033: 
							audio_Codec="ANTEX_ADPCME"; //Antex Electronics Corporation 
							break;
						case 0x0034: 
							audio_Codec="CONTROL_RES_VQLPC"; //Control Resources Limited 
							break;
						case 0x0035: 
							audio_Codec="DIGIREAL"; //DSP Solutions, Inc. 
							break;
						case 0x0036: 
							audio_Codec="DIGIADPCM"; //DSP Solutions, Inc. 
							break;
						case 0x0037: 
							audio_Codec="CONTROL_RES_CR10"; //Control Resources Limited 
							break;
						case 0x0038: 
							audio_Codec="NMS_VBXADPCM"; //Natural MicroSystems 
							break;
						case 0x0039: 
							audio_Codec="CS_IMAADPCM"; //Crystal Semiconductor IMA ADPCM 
							break;
						case 0x003A: 
							audio_Codec="ECHOSC3"; //Echo Speech Corporation 
							break;
						case 0x003B: 
							audio_Codec="ROCKWELL_ADPCM"; //Rockwell International 
							break;
						case 0x003C: 
							audio_Codec="ROCKWELL_DIGITALK"; //Rockwell International 
							break;
						case 0x003D: 
							audio_Codec="XEBEC"; //Xebec Multimedia Solutions Limited 
							break;
						case 0x0040: 
							audio_Codec="G721_ADPCM"; //Antex Electronics Corporation 
							break;
						case 0x0041: 
							audio_Codec="G728_CELP"; //Antex Electronics Corporation 
							break;
						case 0x0042: 
							audio_Codec="MSG723"; //Microsoft Corporation 
							break;
						case 0x0050: 
							audio_Codec="MPEG"; //Microsoft Corporation 
							break;
						case 0x0052: 
							audio_Codec="RT24"; //InSoft, Inc. 
							break;
						case 0x0053: 
							audio_Codec="PAC"; //InSoft, Inc. 
							break;
						case 0x0055: 
							audio_Codec="MPEG Layer 3"; //ISO/MPEG-1 Layer 3 
							break;
						case 0x0059: 
							audio_Codec="LUCENT_G723"; //Lucent Technologies 
							break;
						case 0x0060: 
							audio_Codec="CIRRUS"; //Cirrus Logic 
							break;
						case 0x0061: 
							audio_Codec="ESPCM"; //ESS Technology 
							break;
						case 0x0062: 
							audio_Codec="VOXWARE"; //Voxware Inc 
							break;
						case 0x0063: 
							audio_Codec="CANOPUS_ATRAC"; //Canopus, co., Ltd. 
							break;
						case 0x0064: 
							audio_Codec="G726_ADPCM"; //APICOM 
							break;
						case 0x0065: 
							audio_Codec="G722_ADPCM"; //APICOM 
							break;
						case 0x0067: 
							audio_Codec="DSAT_DISPLAY"; //Microsoft Corporation 
							break;
						case 0x0069: 
							audio_Codec="VOXWARE_BYTE_ALIGNED"; //Voxware Inc 
							break;
						case 0x0070: 
							audio_Codec="VOXWARE_AC8"; //Voxware Inc 
							break;
						case 0x0071: 
							audio_Codec="VOXWARE_AC10"; //Voxware Inc 
							break;
						case 0x0072: 
							audio_Codec="VOXWARE_AC16"; //Voxware Inc 
							break;
						case 0x0073: 
							audio_Codec="VOXWARE_AC20"; //Voxware Inc 
							break;
						case 0x0074: 
							audio_Codec="VOXWARE_RT24"; //Voxware Inc 
							break;
						case 0x0075: 
							audio_Codec="VOXWARE_RT29"; //Voxware Inc 
							break;
						case 0x0076: 
							audio_Codec="VOXWARE_RT29HW"; //Voxware Inc 
							break;
						case 0x0077: 
							audio_Codec="VOXWARE_VR12"; //Voxware Inc 
							break;
						case 0x0078: 
							audio_Codec="VOXWARE_VR18"; //Voxware Inc 
							break;
						case 0x0079: 
							audio_Codec="VOXWARE_TQ40"; //Voxware Inc 
							break;
						case 0x0080: 
							audio_Codec="SOFTSOUND"; //Softsound, Ltd. 
							break;
						case 0x0081: 
							audio_Codec="VOXWARE_TQ60"; //Voxware Inc 
							break;
						case 0x0082: 
							audio_Codec="MSRT24"; //Microsoft Corporation 
							break;
						case 0x0083: 
							audio_Codec="G729A"; //AT&T Labs, Inc. 
							break;
						case 0x0084: 
							audio_Codec="MVI_MVI2"; //Motion Pixels 
							break;
						case 0x0085: 
							audio_Codec="DF_G726"; //DataFusion Systems (Pty) (Ltd) 
							break;
						case 0x0086: 
							audio_Codec="DF_GSM610"; //DataFusion Systems (Pty) (Ltd) 
							break;
						case 0x0088: 
							audio_Codec="ISIAUDIO"; //Iterated Systems, Inc. 
							break;
						case 0x0089: 
							audio_Codec="ONLIVE"; //OnLive! Technologies, Inc. 
							break;
						case 0x0091: 
							audio_Codec="SBC24"; //Siemens Business Communications Sys 
							break;
						case 0x0092: 
							audio_Codec="DOLBY_AC3_SPDIF"; //Sonic Foundry 
							break;
						case 0x0093: 
							audio_Codec="MEDIASONIC_G723"; //MediaSonic 
							break;
						case 0x0094: 
							audio_Codec="PROSODY_8KBPS"; //Aculab plc 
							break;
						case 0x0097: 
							audio_Codec="ZYXEL_ADPCM"; //ZyXEL Communications, Inc. 
							break;
						case 0x0098: 
							audio_Codec="PHILIPS_LPCBB"; //Philips Speech Processing 
							break;
						case 0x0099: 
							audio_Codec="PACKED"; //Studer Professional Audio AG 
							break;
						case 0x00A0: 
							audio_Codec="MALDEN_PHONYTALK"; //Malden Electronics Ltd. 
							break;
						case 0x0100: 
							audio_Codec="RHETOREX_ADPCM"; //Rhetorex Inc. 
							break;
						case 0x0101: 
							audio_Codec="IRAT"; //BeCubed Software Inc. 
							break;
						case 0x0111: 
							audio_Codec="VIVO_G723"; //Vivo Software 
							break;
						case 0x0112: 
							audio_Codec="VIVO_SIREN"; //Vivo Software 
							break;
						case 0x0123: 
							audio_Codec="DIGITAL_G723"; //Digital Equipment Corporation 
							break;
						case 0x0125: 
							audio_Codec="SANYO_LD_ADPCM"; //Sanyo Electric Co., Ltd. 
							break;
						case 0x0130: 
							audio_Codec="SIPROLAB_ACEPLNET"; //Sipro Lab Telecom Inc. 
							break;
						case 0x0131: 
							audio_Codec="SIPROLAB_ACELP4800"; //Sipro Lab Telecom Inc. 
							break;
						case 0x0132: 
							audio_Codec="SIPROLAB_ACELP8V3"; //Sipro Lab Telecom Inc. 
							break;
						case 0x0133: 
							audio_Codec="SIPROLAB_G729"; //Sipro Lab Telecom Inc. 
							break;
						case 0x0134: 
							audio_Codec="SIPROLAB_G729A"; //Sipro Lab Telecom Inc. 
							break;
						case 0x0135: 
							audio_Codec="SIPROLAB_KELVIN"; //Sipro Lab Telecom Inc. 
							break;
						case 0x0140: 
							audio_Codec="G726ADPCM"; //Dictaphone Corporation 
							break;
						case 0x0150: 
							audio_Codec="QUALCOMM_PUREVOICE"; //Qualcomm, Inc. 
							break;
						case 0x0151: 
							audio_Codec="QUALCOMM_HALFRATE"; //Qualcomm, Inc. 
							break;
						case 0x0155: 
							audio_Codec="TUBGSM"; //Ring Zero Systems, Inc. 
							break;
						case 0x0160: 
							audio_Codec="MSAUDIO1"; //Microsoft Corporation 
							break;
						case 0x0170: 
							audio_Codec="UNISYS_NAP_ADPCM"; //Unisys Corp. 
							break;
						case 0x0171: 
							audio_Codec="UNISYS_NAP_ULAW"; //Unisys Corp. 
							break;
						case 0x0172: 
							audio_Codec="UNISYS_NAP_ALAW"; //Unisys Corp. 
							break;
						case 0x0173: 
							audio_Codec="UNISYS_NAP_16K"; //Unisys Corp. 
							break;
						case 0x0200: 
							audio_Codec="CREATIVE_ADPCM"; //Creative Labs, Inc 
							break;
						case 0x0202: 
							audio_Codec="CREATIVE_FASTSPEECH8"; //Creative Labs, Inc 
							break;
						case 0x0203: 
							audio_Codec="CREATIVE_FASTSPEECH10"; //Creative Labs, Inc 
							break;
						case 0x0210: 
							audio_Codec="UHER_ADPCM"; //UHER informatic GmbH 
							break;
						case 0x0220: 
							audio_Codec="QUARTERDECK"; //Quarterdeck Corporation 
							break;
						case 0x0230: 
							audio_Codec="ILINK_VC"; //I-link Worldwide 
							break;
						case 0x0240: 
							audio_Codec="RAW_SPORT"; //Aureal Semiconductor 
							break;
						case 0x0241: 
							audio_Codec="ESST_AC3"; //ESS Technology, Inc. 
							break;
						case 0x0250: 
							audio_Codec="IPI_HSX"; //Interactive Products, Inc. 
							break;
						case 0x0251: 
							audio_Codec="IPI_RPELP"; //Interactive Products, Inc. 
							break;
						case 0x0260: 
							audio_Codec="CS2"; //Consistent Software 
							break;
						case 0x0270: 
							audio_Codec="SONY_SCX"; //Sony Corp. 
							break;
						case 0x0300: 
							audio_Codec="FM_TOWNS_SND"; //Fujitsu Corp. 
							break;
						case 0x0400: 
							audio_Codec="BTV_DIGITAL"; //Brooktree Corporation 
							break;
						case 0x0401: 
							audio_Codec="IMC"; //Intel Music Coder for MSACM
							break;
						case 0x0450: 
							audio_Codec="QDESIGN_MUSIC"; //QDesign Corporation 
							break;
						case 0x0680: 
							audio_Codec="VME_VMPCM"; //AT&T Labs, Inc. 
							break;
						case 0x0681: 
							audio_Codec="TPC"; //AT&T Labs, Inc. 
							break;
						case 0x1000: 
							audio_Codec="OLIGSM"; //Ing C. Olivetti & C., S.p.A. 
							break;
						case 0x1001: 
							audio_Codec="OLIADPCM"; //Ing C. Olivetti & C., S.p.A. 
							break;
						case 0x1002: 
							audio_Codec="OLICELP"; //Ing C. Olivetti & C., S.p.A. 
							break;
						case 0x1003: 
							audio_Codec="OLISBC"; //Ing C. Olivetti & C., S.p.A. 
							break;
						case 0x1004: 
							audio_Codec="OLIOPR"; //Ing C. Olivetti & C., S.p.A. 
							break;
						case 0x1100: 
							audio_Codec="LH_CODEC"; //Lernout & Hauspie 
							break;
						case 0x1400: 
							audio_Codec="NORRIS"; //Norris Communications, Inc. 
							break;
						case 0x1500: 
							audio_Codec="SOUNDSPACE_MUSICOMPRESS"; //AT&T Labs, Inc. 
							break;
							//					case 0x2000: 
							//						audio_Codec="DVM"; //FAST Multimedia AG" 
							//						break;
						case 353:  //0x00eb
							audio_Codec = "DivX Audio";
							break;
							//					case 85:  //0x0055
							//						audio_Codec	= "MPEG Layer 3";
							//						break;
						case 8192:  //0x2000
							audio_Codec	= "AC3-Digital";
							break;
						default:
							audio_Codec	= "Unknown";
							break;
					};
					m_Details.ListDetails.Add(Constants.Avi.ACodec, audio_Codec);
			
					// audio channel
					knownfile.Position		= AHeaderStart + 2 + AStrhSize + 8;
					int audioChannel		= reader.ReadInt16();
					string audio_Channel;
					switch(audioChannel)
					{
						case 1:
							audio_Channel="1 (mono)";
							break;
						case 2:
							audio_Channel="2 (stereo)";
							break;
						case 5:
							audio_Channel="5.1 (surround)";
							break;
						default:
							audio_Channel=audioChannel.ToString();
							break;
					};
					m_Details.ListDetails.Add(Constants.Avi.AChannel, audio_Channel);				

					// audio samplerate
					knownfile.Position		= AHeaderStart + 4 + AStrhSize + 8;
					string audio_SampleRate=((int)reader.ReadInt32()).ToString();
					m_Details.ListDetails.Add(Constants.Avi.ASampleRate, audio_SampleRate + " Hz");
				
					// audio bitrate
					knownfile.Position		= AHeaderStart + 8 + AStrhSize + 8;
					int audio_Bitrate		= (int)reader.ReadInt32();
					m_Details.ListDetails.Add(Constants.Avi.ABitrate, audio_Bitrate.ToString() + " Kbps");
			
					// video bitrate
					string video_Bitrate=((uint)((file_size / video_length  - audio_Bitrate) / 128)).ToString();
					m_Details.ListDetails.Add(Constants.Avi.VBitrate, video_Bitrate + " Kbps");

					return  true;

				}
				else
				{
					return	false;
				}
			} 
			catch(Exception e)
			{
				Debug.WriteLine("ERROR IN FILE:" + knownfile.Name + " Position: "+knownfile.Position.ToString());
				Debug.WriteLine(e.ToString());
				return false;
			}

		}

	}
}
