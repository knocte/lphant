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
using System.Resources;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for CSkin.
	/// </summary>
	public class CSkin
	{
		private string m_CurrentSkin;
		private XmlElement m_XMLColors;
		private XmlElement m_XMLPositions;
		private XmlElement m_XMLSizes;
		private XmlElement m_XMLDefaultColors;
		private XmlElement m_XMLDefaultPositions;
		private XmlElement m_XMLDefaultSizes;
		private string m_LinkText;
		private string m_LinkUrl;

		public string LinkText
		{
			get
			{
				return m_LinkText;
			}
		}

		public string LinkUrl
		{
			get
			{
				return m_LinkUrl;
			}
		}

		public string CurrentSkin
		{
			get
			{
				return m_CurrentSkin;
			}
			set
			{
				m_CurrentSkin=value;
				if (!m_DefaultSkin()) Load();
				else
				{
					m_LinkText="";
					m_LinkUrl="";
				}
			}
		}
		
		private bool m_DefaultSkin()
		{
			return (m_CurrentSkin=="default");
		}
		
		public CSkin()
		{
			m_CurrentSkin="Default";
			XmlDocument doc = new XmlDocument();
			doc.Load(typeof(eLePhantForm).Assembly.GetManifestResourceStream(typeof(eLePhantForm),"Client.Resources.Classic.skin.xml"));
			m_XMLDefaultColors=doc.DocumentElement["colors"];
			m_XMLDefaultPositions=doc.DocumentElement["positions"];
			m_XMLDefaultSizes=doc.DocumentElement["sizes"];
			m_LinkText="";
			m_LinkUrl="";
		}

		public Bitmap GetBitmap(string imageID)
		{
			bool notFound=false;
			if (!m_DefaultSkin())
			{
				try
				{
					string fileName;
					fileName=Application.StartupPath+Path.DirectorySeparatorChar+"skins\\"+m_CurrentSkin+Path.DirectorySeparatorChar+imageID+".bmp";
					if (File.Exists(fileName))
						return new Bitmap(fileName);
					fileName=Application.StartupPath+Path.DirectorySeparatorChar+"skins\\"+m_CurrentSkin+Path.DirectorySeparatorChar+imageID+".gif";
					if (File.Exists(fileName))
						return new Bitmap(fileName);
					fileName=Application.StartupPath+Path.DirectorySeparatorChar+"skins\\"+m_CurrentSkin+Path.DirectorySeparatorChar+imageID+".ico";
					if (File.Exists(fileName))
						return new Bitmap(fileName);
					fileName=Application.StartupPath+Path.DirectorySeparatorChar+"skins\\"+m_CurrentSkin+Path.DirectorySeparatorChar+imageID+".jpg";
					if (File.Exists(fileName))
						return new Bitmap(fileName);
					else 
						notFound=true;
				}
				catch 
				{
					Debug.WriteLine(imageID+" not found");
				}
			}
			if ((m_DefaultSkin())||(notFound)) 
			{
				string resourceName=imageID+".bmp";
				try
				{
					return new Bitmap(typeof(eLePhantForm),"Client.Resources.Classic."+resourceName);
				}
				catch
				{
					try
					{
						resourceName=imageID+".gif";
						return new Bitmap(typeof(eLePhantForm),"Client.Resources.Classic."+resourceName);
					}
					catch 
					{
						try
						{
							resourceName=imageID+".ico";
							return new Bitmap(typeof(eLePhantForm),"Client.Resources.Classic."+resourceName);
						}
						catch
						{
							try
							{
								resourceName=imageID+".jpg";
								return new Bitmap(typeof(eLePhantForm),"Client.Resources.Classic."+resourceName);
							}
							catch
							{
							}
						}
					}
				}
			}
			return null;
		}

		public Color GetColor(string colorID)
		{
			bool notFound=false;
			if (!m_DefaultSkin())
			{
				try
				{
					XmlNode element=m_XMLColors.SelectSingleNode("color[@key='"+colorID+"']");
					int color=int.Parse(element.Attributes["value"].Value,System.Globalization.NumberStyles.HexNumber);
					return Color.FromArgb(color);
				}
				catch
				{
					Debug.WriteLine(colorID + " not found");
					notFound=true;
				}
			}
			if ((m_DefaultSkin())||(notFound))
			{
				try
				{
					XmlNode element=m_XMLDefaultColors.SelectSingleNode("color[@key='"+colorID+"']");
					int color=int.Parse(element.Attributes["value"].Value,System.Globalization.NumberStyles.HexNumber);
					return Color.FromArgb(color);
				}
				catch
				{
					Debug.WriteLine(colorID + " not found default");
					notFound=true;
				}
			}
			return Color.Gray;
		}

		public void GetPosition(string positionID, ref int x, ref int y)
		{
			bool notFound=false;
			if (!m_DefaultSkin())
			{
				try
				{
					XmlNode element=m_XMLPositions.SelectSingleNode("position[@key='"+positionID+"']");
					x=int.Parse(element.Attributes["x"].Value);
					y=int.Parse(element.Attributes["y"].Value);
					return;
				}
				catch
				{
					Debug.WriteLine(positionID + " not found");
					notFound=true;
				}
			}
			if ((m_DefaultSkin())||(notFound))
			{
				try
				{
					XmlNode element=m_XMLDefaultPositions.SelectSingleNode("position[@key='"+positionID+"']");
					x=int.Parse(element.Attributes["x"].Value);
					y=int.Parse(element.Attributes["y"].Value);
					return;
				}
				catch
				{
					Debug.WriteLine(positionID + " not found default");
					notFound=true;
				}
			}
		}

		public void GetSize(string sizeID, ref int width, ref int heigh)
		{
			bool notFound=false;
			if (!m_DefaultSkin())
			{
				try
				{
					XmlNode element=m_XMLSizes.SelectSingleNode("size[@key='"+sizeID+"']");
					width=int.Parse(element.Attributes["width"].Value);
					heigh=int.Parse(element.Attributes["height"].Value);
					return;
				}
				catch
				{
					Debug.WriteLine(sizeID + " not found");
					notFound=true;
				}
			}
			if ((m_DefaultSkin())||(notFound))
			{
				try
				{
					XmlNode element=m_XMLDefaultSizes.SelectSingleNode("size[@key='"+sizeID+"']");
					width=int.Parse(element.Attributes["width"].Value);
					heigh=int.Parse(element.Attributes["height"].Value);
					return;
				}
				catch
				{
					Debug.WriteLine(sizeID + " not found default");
					notFound=true;
				}
			}
		}

		public bool Load()
		{
			string version;
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(Application.StartupPath+Path.DirectorySeparatorChar+"skins\\"+m_CurrentSkin+Path.DirectorySeparatorChar + "skin.xml");
				
				version = doc.DocumentElement.Attributes["version"].InnerText;
				Debug.WriteLine("Version: "+version);
				m_XMLColors=doc.DocumentElement["colors"];
				m_XMLSizes=doc.DocumentElement["sizes"];
				m_XMLPositions=doc.DocumentElement["positions"];
				XmlElement xmlURL=doc.DocumentElement["URL"];
				m_LinkText="";
				m_LinkUrl="";
				if (xmlURL!=null)
				{
					XmlNode nodeURL=xmlURL.FirstChild;
					if (nodeURL!=null)
					{
						this.m_LinkText=nodeURL.Attributes["Text"].Value;
						this.m_LinkUrl=nodeURL.Attributes["url"].Value;
					}
				}

				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception while load Skin:\n " + e.ToString());
				m_CurrentSkin="default";
				return false;
			}			
		}
	}
}
