using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace eLePhant.Client
{
	/// <summary>
	/// Cache of the system icons associated to file extensions
	/// </summary>
	public class CSystemIconsList
	{
		public ImageList list;
		private Hashtable m_table;
		public CSystemIconsList()
		{
			list=new ImageList();
			m_table=new Hashtable();
		}
		public int GetIconIndexOf(string filename)
		{
			string fileExtension=CUtils.GetExtension(filename);
			
			//patch that fixes a crash on search with .mdf and .mds results if Alcohol 120% is sinstalled
			if(fileExtension==".mds" || fileExtension==".mdf")
			{	
				filename+=".iso";
				fileExtension=".iso";
			}
			
			if (m_table[fileExtension]!=null) return (int)m_table[fileExtension];
			Win32.SHFILEINFO shinfo = new Win32.SHFILEINFO();
			IntPtr hImgSmall; //the handle to the system image list
			System.Drawing.Icon myIcon;
			try
			{
				hImgSmall = Win32.SHGetFileInfo(filename, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo,(uint)Marshal.SizeOf(shinfo),
					Win32.SHGFI_USEFILEATTRIBUTES | Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
				myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
			}
			catch
			{
				return -1;
			}
			list.Images.Add(myIcon);
			m_table.Add(fileExtension,list.Images.Count-1);
			return list.Images.Count-1;
		}
		public Image GetIconImageOf(string filename)
		{
			int index=GetIconIndexOf(filename);
			return list.Images[index];
		}
	}
}
