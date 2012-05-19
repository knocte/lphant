using System;

namespace eLePhant.Client
{
	/// <summary>
	/// General interface static functions should go here
	/// </summary>
	public class CUtils
	{
		public CUtils()
		{
		}
		static public bool IsVideo(string fileName)
		{
			string fileExtension=GetExtension(fileName);
			string videoExtensions=".asf.avi.divx.m1v.m2v.mkv.mov.mp1v.mp2v.mpe.mpeg.mpg.mps.mpv.mpv1.mpv2.ogm.qt.ram.rm.rv.vivo.vob.wmv.rv9";
			return (videoExtensions.IndexOf(fileExtension)>0);
		}
		static public bool IsAudio(string fileName)
		{
			string fileExtension=GetExtension(fileName);
			string audioExtensions=".669.aac.aif.aiff.amf.ams.ape.au.dbm.dmf.dsm.far.flac.it.mdl.med.mid.midi.mod.mol.mp1.mp2.mp3.mp4.mpa.mpc.mpp.mtm.nst.ogg.okt.psm.ptm.ra.rmi.s3m.stm.ult.umx.wav.wma.wow.xm";
			return (audioExtensions.IndexOf(fileExtension)>0);
		}
		static public bool IsFile(string fileName)
		{
			return (!IsAudio(fileName)&&(!IsVideo(fileName)));
		}
		static public string GetExtension(string fileName)
		{
			//dont use Path.GetExtension to avoid exceptions if fileName contains invalid characters
			int location=fileName.LastIndexOf(".");
			string fileExtension="";
			if (location>0)
				fileExtension=fileName.Substring(location);
			return fileExtension.ToLower();
		}
	}
	public class CFilterSummary
	{
		public ulong TotalSize;
		public uint Items;
	}
}
