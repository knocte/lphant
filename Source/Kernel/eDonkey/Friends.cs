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
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Text;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	[Serializable]
	public struct SFriend
	{
		public string Name;
		public uint ID; 
		public ushort Port;
		public uint ServerIP; 
		public ushort ServerPort; 
		public string Software;
		public uint Version;
		public DateTime LastOnline;
		public byte[] UserHash;
		public string OurName;
		public bool FriendSlot;
	}
	/// <summary>
	/// 
	/// </summary>
	internal class CFriendsList
	{
		public ArrayList List;
		private string _xmlRootNode;
		private string _version;
		private string _currentversion;
		private string _configDirectory;
		private string _fileName;

		public CFriendsList()
		{
			_xmlRootNode		= "lphantFriendsList";
			_version			= "0.2";
			_configDirectory	= Application.StartupPath;
			_fileName			= "friends.xml";
			List = new ArrayList();
			Load();
		}

		private int m_FindFriend(uint ID,byte[] ClientHash, ushort Port)
		{
			//First: i try to locate Friend with UserHash
			if (ClientHash!= null)
			{
				for (int n=0; n!=List.Count;n++)
				{
					byte[] hashFriend = (byte[])((SFriend)List[n]).UserHash;
					if (hashFriend!=null) 
					{
						if (CKernel.SameHash(ref ClientHash, ref hashFriend))
						{
							return n;
						}
					}
				}
			}

			//If i did not locate Friend with UserHash, i try to locate Friend with IP&Port
			for (int n=0; n!=List.Count;n++)
			{
				if ((((SFriend)List[n]).ID==ID) && (((SFriend)List[n]).Port==Port))
				{
					return n;
				}
			}
			return -1;
		}

		public void ChangeFriendSlotAssigned(uint ID, byte[] ClientHash, ushort Port, bool assigned)
		{
			int index = m_FindFriend(ID,ClientHash,Port);
			if (index>=0)
			{
				SFriend Friend=(SFriend)List[index];
				Friend.FriendSlot=assigned;
				CClient client=CKernel.ClientsList.GetClient(Friend.ID,Friend.Port,Friend.ServerIP,Friend.UserHash);
				List[index]=Friend;
				if (client!=null)
				{
					Constants.FriendLevel friendLevel;
					if (assigned)
						friendLevel=Constants.FriendLevel.FriendSlot;
					else
						friendLevel=Constants.FriendLevel.Friend;
					client.FriendStatus=friendLevel;
				}
				this.Save();
			}
		}

		public void Add(uint ID, ushort Port,uint ServerIP, byte[] ClientHash, ushort ServerPort, string OurName, string Name, string Software, uint Version)
		{
			int index = m_FindFriend(ID,ClientHash,Port);
			Constants.FriendLevel friendLevel;
			if (index>=0)
			{
				SFriend Friend		= (SFriend)List[index];
				if (OurName!="")		Friend.OurName		= OurName;
				Friend.Name			= Name;
				Friend.ID			= ID;
				Friend.Port			= Port;
				if (ServerIP!=0)		Friend.ServerIP		= ServerIP;
				if (ServerPort!=0)		Friend.ServerPort	= ServerPort;
				Friend.LastOnline	= DateTime.Now;
				if (Software!=null)		Friend.Software		= Software;
				if (Version!=0)			Friend.Version		= Version;
				if (ClientHash!=null)	Friend.UserHash		= ClientHash;
				List[index]			= Friend;
//				Friend.FriendSlot=friendSlot;
//				CClient client=CKernel.ClientsList.GetClient(Friend.ID,Friend.Port,Friend.ServerIP,Friend.UserHash);
//				if (client!=null)
//				{
//					if (Friend.FriendSlot)
//						friendLevel=Constants.FriendLevel.FriendSlot;
//					else
//						friendLevel=Constants.FriendLevel.Friend;
//					client.FriendStatus=friendLevel;
//				}
			}
			else
			{
				SFriend Friend		= new SFriend();
				Friend.OurName		= OurName;
				Friend.Name			= Name;
				Friend.UserHash		= ClientHash;
				Friend.ID			= ID;
				Friend.Port			= Port;
				Friend.ServerPort	= ServerPort;
				Friend.ServerIP		= ServerIP;
				Friend.LastOnline	= DateTime.Now;
				Friend.Software		= Software;
				Friend.Version		= Version;
				Friend.FriendSlot	= false;
				List.Add(Friend);
				friendLevel=Constants.FriendLevel.Friend;
				CKernel.ClientsList.AddClient(Friend.ID,Friend.Port,0,Friend.UserHash,Friend.Port,null,friendLevel);
			}
				
			this.Save();
		}

		public void Delete(byte[] ClientHash,uint ID, ushort Port)
		{
			int index = m_FindFriend(ID,ClientHash,Port);

			if (index>=0)
			{
				CClient client=CKernel.ClientsList.GetClient(ID,Port,0,ClientHash);
				if (client!=null) client.FriendStatus=Constants.FriendLevel.NoFriend;
				List.RemoveAt(index);
				this.Save();
			}
		}

		public bool Save()
		{
			lock (this) 
			{
				try
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml("<?xml version=\"1.0\"?>\n<" + this._xmlRootNode + " version = \"" + this._version + "\" />");
			
					doc.DocumentElement.AppendChild(ToXmlElement(doc));

					doc.Save(Path.Combine(this._configDirectory, this._fileName));
				}
				catch 
				{
					return false;
				}
				return true;
			}
		}

		public bool Load()
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(Path.Combine(this._configDirectory, this._fileName));
				_currentversion = doc.DocumentElement.Attributes["version"].InnerText;

				return SetValueFromXmlElement(doc.DocumentElement["Friends"]);
			}
			catch (FileNotFoundException)
			{
				return false;
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception while load FriendsList from stream :\n" + e.Message);
				return false;
			}
		}

		private bool SetValueFromXmlElement(XmlElement element)
		{
			try
			{
				XmlNodeList nodes = element.ChildNodes;
				foreach (XmlElement el in nodes)
				{
					if ( el.Name == "Friend" )
					{
						SFriend my_Friend = new SFriend();
						my_Friend.FriendSlot=false;
						//load Friend.UserHash
						if (el.Attributes["UserHash"].InnerText!="") 
						{
							byte[] x = null;
							switch(_currentversion)
							{
								case "0.1":
									x = new byte[el.Attributes["UserHash"].InnerText.Length];
									x = Encoding.Default.GetBytes( el.Attributes["UserHash"].InnerText );
									break;

								case "0.2":
									x = Convert.FromBase64String( el.Attributes["UserHash"].InnerText );
									break;
							}
							my_Friend.UserHash = (byte[])x;
						} 
						else
						{
							my_Friend.UserHash = null;
						}

						//load Friend.ID
						my_Friend.ID = uint.Parse(el.Attributes["ID"].InnerText);

						//load Friend.OurName
						my_Friend.OurName = el.Attributes["OurName"].InnerText;

						//load Friend.Name
						my_Friend.Name = el.Attributes["Name"].InnerText;

						//load Friend.Port
						my_Friend.Port = ushort.Parse(el.Attributes["Port"].InnerText);

						//load Friend.Software
						my_Friend.Software = el.Attributes["Software"].InnerText;

						//load Friend.Version
						my_Friend.Version = uint.Parse(el.Attributes["Version"].InnerText);

						//load Friend.LastOnline
						my_Friend.LastOnline = new DateTime(long.Parse(el.Attributes["LastOnline"].InnerText));
	
						//load friendSlot
						if (el.Attributes["FriendSlot"]!=null) 
						{
							my_Friend.FriendSlot=bool.Parse(el.Attributes["FriendSlot"].InnerText);
						}

						List.Add(my_Friend);
						Constants.FriendLevel friendSlot;
						if (my_Friend.FriendSlot)
							friendSlot=Constants.FriendLevel.FriendSlot;
						else
							friendSlot=Constants.FriendLevel.Friend;

						CKernel.ClientsList.AddClient(my_Friend.ID,my_Friend.Port,0,my_Friend.UserHash,my_Friend.Port,null,friendSlot);

						//switch ( el.Attributes["type"].InnerText )
					}
				}
			} 
			catch 
			{
				CLog.Log(Constants.Log.Info,"Error loading friends list");
				return false;
			}
			return true;

		}

		private XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement friendsnode  = doc.CreateElement("Friends");


			for (int n=0; n < List.Count; n++)
			{
				SFriend my_Friend = (SFriend)List[n];

				XmlElement el = doc.CreateElement("Friend");

				//save Friend.UserHash
				XmlAttribute key = doc.CreateAttribute("UserHash");
				if (my_Friend.UserHash!=null) 
				{
					switch(_version)
					{
						case "0.1":
							key.InnerText = Encoding.Default.GetString( (byte[])my_Friend.UserHash );
							break;

						case "0.2":
							key.InnerText = Convert.ToBase64String((byte[])my_Friend.UserHash);
							break;
					}
				}
				else 
				{
					key.InnerText = "";
				}
				el.Attributes.Append(key);

				//save Friend.ID
				key = doc.CreateAttribute("ID");
				key.InnerText = my_Friend.ID.ToString();
				el.Attributes.Append(key);

				//save Friend.OurName
				key = doc.CreateAttribute("OurName");
				key.InnerText = my_Friend.OurName;
				el.Attributes.Append(key);

				//save Friend.Name
				key = doc.CreateAttribute("Name");
				key.InnerText = my_Friend.Name;
				el.Attributes.Append(key);

				//save Friend.Port
				key = doc.CreateAttribute("Port");
				key.InnerText = my_Friend.Port.ToString();
				el.Attributes.Append(key);

				//save Friend.Software
				key = doc.CreateAttribute("Software");
				key.InnerText = my_Friend.Software;
				el.Attributes.Append(key);

				//save Friend.Version
				key = doc.CreateAttribute("Version");
				key.InnerText = my_Friend.Version.ToString();
				el.Attributes.Append(key);

				//save Friend.LastOnline
				key = doc.CreateAttribute("LastOnline");
				key.InnerText = ((long)my_Friend.LastOnline.Ticks).ToString();
				el.Attributes.Append(key);

				//save Friend.FriendSlot
				key = doc.CreateAttribute("FriendSlot");
				key.InnerText = my_Friend.FriendSlot.ToString();
				el.Attributes.Append(key);

				friendsnode.AppendChild(el);

			}
			return friendsnode;
		}
	}
}
