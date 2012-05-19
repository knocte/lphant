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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions; 
using System.Net;
using eLePhant.eDonkey;
using eLePhant.Types;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for FormFriends.
	/// </summary>
	public class FormFriends : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.ComponentModel.IContainer components;
		private CkernelGateway krnGateway;
		private InterfaceClient m_LastClient;
		private Crownwood.Magic.Controls.TabControl tabMessages;
		private eLePhantListView listViewFriends;
		private System.Windows.Forms.TextBox textBoxMessage;
		private System.Windows.Forms.Button buttonSend;
		private string m_LastMessage;
		private System.Windows.Forms.Panel panel2;
		private SFriend[] m_Friends;
		private System.Windows.Forms.Button buttonAddFriend;
		private System.Windows.Forms.Panel panelAddFriend;
		private System.Windows.Forms.Label labelIP;
		private System.Windows.Forms.Label labelPort;
		private System.Windows.Forms.TextBox textBoxIP;
		private System.Windows.Forms.TextBox textBoxPort;
		private System.Windows.Forms.Button buttonAddFriendOk;
		private System.Windows.Forms.Button buttonAddFriendCancel;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label labelUserHash;
		private System.Windows.Forms.TextBox textBoxUserHash;
		private System.Windows.Forms.Label labelOptional;
		private System.Windows.Forms.ImageList imageListFriends;
		private ContextMenu ContextMenuFriends;
		
		public FormFriends()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Initialize();
			tabMessages.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiBox;
			tabMessages.PositionTop = true;
			tabMessages.PositionTop = true;
			tabMessages.ShowArrows = true;
			tabMessages.ShowClose = true;
			tabMessages.ShrinkPagesToFit = false;	
			tabMessages.ClosePressed += new EventHandler(OnClosePressed);

			m_Globalize();
		}

		private void m_Globalize()
		{
			listViewFriends.Columns.Clear();
			listViewFriends.Columns.Add(eLePhantForm.Globalization["LBL_FRIENDS"],134,false /*HorizontalAlignment.Left*/);
			//labelFriends.Text = eLePhantForm.Globalization["LBL_FRIENDS"];
			buttonSend.Text = eLePhantForm.Globalization["LBL_SEND"];
			ContextMenuFriends.MenuItems[0].Text=eLePhantForm.Globalization["LBL_SENDMESSAGE"];
			ContextMenuFriends.MenuItems[1].Text=eLePhantForm.Globalization["LBL_VIEWFILES"];
			ContextMenuFriends.MenuItems[3].Text=eLePhantForm.Globalization["LBL_RENAMEFRIEND"];
			ContextMenuFriends.MenuItems[4].Text=eLePhantForm.Globalization["LBL_DELETEFRIEND"];
			ContextMenuFriends.MenuItems[6].Text=eLePhantForm.Globalization["LBL_FRIENDSLOT"];
			buttonAddFriendCancel.Text = eLePhantForm.Globalization["LBL_CANCEL"];
			labelIP.Text = eLePhantForm.Globalization["LBL_IP_OR_ID"];
			labelName.Text = eLePhantForm.Globalization["LBL_NAME"];
			labelPort.Text = eLePhantForm.Globalization["LBL_PORT"];
			labelUserHash.Text = eLePhantForm.Globalization["LBL_USERHASH"];
			buttonAddFriend.Text = eLePhantForm.Globalization["LBL_ADDFRIEND"];
			labelOptional.Text = "(" + eLePhantForm.Globalization["LBL_OPTIONAL"] + ")";
		}

		public void Globalize()
		{
			m_Globalize();
		}

		private void Initialize()
		{			
			ContextMenuFriends = new ContextMenu();
			ContextMenuFriends.MenuItems.Add("Send message",new EventHandler(OnSendMessage));
			ContextMenuFriends.MenuItems.Add("View files",new EventHandler(OnViewFiles));
			ContextMenuFriends.MenuItems.Add("-");
			ContextMenuFriends.MenuItems.Add("Rename friend",new EventHandler(OnRenameFriend));
			ContextMenuFriends.MenuItems.Add("Delete friend",new EventHandler(OnDeleteFriend));
			ContextMenuFriends.MenuItems.Add("-");
			ContextMenuFriends.MenuItems.Add("Friend slot",new EventHandler(OnFriendSlot));
			listViewFriends.ContextMenu = ContextMenuFriends;
			listViewFriends.DoubleClick+=new EventHandler(OnSendMessage);
			ContextMenuFriends.Popup+=new EventHandler(ContextMenuFriends_Popup);
			listViewFriends.DefaultCustomDraw=true;
		}

		private void OnAddFriend(object sender, System.EventArgs e)
		{
			
			if (tabMessages.TabPages.Count <=0) return;
			if (tabMessages.SelectedTab==null) return;
			InterfaceClient source=(InterfaceClient)tabMessages.SelectedTab.Tag;
			krnGateway.AddFriend(source);
		}

		private void OnSendMessage(object sender, System.EventArgs e)
		{
			if (listViewFriends.SelectedItems.Count<=0) return;
			int index = listViewFriends.SelectedIndices[0];
			InterfaceClient source = new InterfaceClient();
			source.ID	= m_Friends[index].ID;
			source.Name	= m_Friends[index].Name;
			if (m_Friends[index].OurName!="") source.Name=m_Friends[index].OurName;
			source.Port = m_Friends[index].Port;
			source.ServerIP	= m_Friends[index].ServerIP;
			source.Software = m_Friends[index].Software;
			source.Version	= m_Friends[index].Version;
			source.UserHash	= m_Friends[index].UserHash;
			krnGateway.StartChatSession(source);
		}

		private void OnViewFiles(object sender, System.EventArgs e)
		{
			if (listViewFriends.SelectedItems.Count<=0) return;
			int index = listViewFriends.SelectedIndices[0];
			InterfaceClient source = new InterfaceClient();
			source.ID	= m_Friends[index].ID;
			source.Name	= m_Friends[index].Name;
			if (m_Friends[index].OurName!="") source.Name=m_Friends[index].OurName;
			source.Port = m_Friends[index].Port;
			source.ServerIP	= m_Friends[index].ServerIP;
			source.Software = m_Friends[index].Software;
			source.Version	= m_Friends[index].Version;
			source.UserHash	= m_Friends[index].UserHash;
			krnGateway.RequestSharedList(source.ID,source.Port,source.ServerIP,source.UserHash);
		}

		private void OnDeleteFriend(object sender, System.EventArgs e)
		{
			if (listViewFriends.SelectedItems.Count<=0) return;
			int index = listViewFriends.SelectedIndices[0];
			krnGateway.DeleteFriend(m_Friends[index].UserHash, m_Friends[index].ID, m_Friends[index].Port);
		}

		private void OnFriendSlot(object sender, System.EventArgs e)
		{
			if (listViewFriends.SelectedItems.Count<=0) return;
			int index = listViewFriends.SelectedIndices[0];
			m_Friends[index].FriendSlot=!m_Friends[index].FriendSlot;
			listViewFriends.SelectedItems[0].ImageIndex=m_Friends[index].FriendSlot ? 1 :0;
			krnGateway.ChangeFriendSlotAssigned(m_Friends[index].ID,m_Friends[index].UserHash,m_Friends[index].Port,m_Friends[index].FriendSlot);
		}
		private void ContextMenuFriends_Popup(object sender, EventArgs e)
		{
			ContextMenuFriends.MenuItems[0].DefaultItem=true;
			foreach (MenuItem item in ContextMenuFriends.MenuItems)
			{
				item.Enabled=(listViewFriends.SelectedItems.Count>0);
			}
			if (listViewFriends.SelectedItems.Count<=0) return;
			int index = listViewFriends.SelectedIndices[0];
			ContextMenuFriends.MenuItems[6].Checked=m_Friends[index].FriendSlot;
		}		
		private void OnRenameFriend(object sender, System.EventArgs e)
		{
			if (listViewFriends.SelectedItems.Count<=0) return;
			int index = listViewFriends.SelectedIndices[0];
			buttonAddFriend_Click(sender, e);
			if (m_Friends[index].OurName!="") 
			{
				textBoxName.Text=m_Friends[index].OurName;
			} 
			else 
			{
				textBoxName.Text=m_Friends[index].Name;
			}
			buttonAddFriendOk.Text=eLePhantForm.Globalization["LBL_RENAME"];
			textBoxUserHash.Enabled=false;
			textBoxIP.Enabled=false;
			textBoxPort.Enabled=false;
			if (m_Friends[index].UserHash !=null)
			{
				textBoxUserHash.Text=Convert.ToBase64String(m_Friends[index].UserHash);
			} else textBoxUserHash.Text="";
			textBoxIP.Text=m_Friends[index].ID.ToString();
			textBoxPort.Text=m_Friends[index].Port.ToString();
		}
		private void OnChangeFriend(object sender, System.EventArgs e)
		{
			if(panelAddFriend.Visible) OnRenameFriend(sender,e);
		}

		public void LoadFriendsList()
		{
			listViewFriends.Items.Clear();
			m_Friends = krnGateway.GetFriends();
			for (int n=0;n!=m_Friends.Length;n++)
			{
				string nameFriend = m_Friends[n].Name;
				if (m_Friends[n].OurName!="") nameFriend=m_Friends[n].OurName;
				listViewFriends.Items.Add(nameFriend,m_Friends[n].FriendSlot ? 1 :0);
			}
		}


		public void Connect(CkernelGateway in_krnGateway)
		{
			krnGateway=in_krnGateway;
			krnGateway.OnNewChatMessage+=new SourceEvent(m_OnNewChatMessage);
			krnGateway.OnStartChatSession+=new ClientEvent(m_OnStartChatSession);
			LoadFriendsList();
		}
		private void m_OnStartChatSession(InterfaceClient client)
		{
			m_LastClient=client;
			m_LastMessage="";
			MethodInvoker mi = new MethodInvoker(this.m_ProcessMessage);
			this.BeginInvoke(mi);
		}
		private void m_OnNewChatMessage(InterfaceClient client, string message)
		{
			m_LastClient=client;
			m_LastMessage=message;
			MethodInvoker mi = new MethodInvoker(this.m_ProcessMessage);
			this.BeginInvoke(mi);
		}
		private static string m_HashToString(byte[] Hash)
		{
			string strResult=BitConverter.ToString(Hash);
			strResult=strResult.Replace("-", "");
			return strResult;
		}		
		private void m_ProcessMessage()
		{
			if ((m_LastClient.UserHash==null) && (m_LastClient.ID==0||m_LastClient.Port==0)) return;
			Crownwood.Magic.Controls.TabPage messagePage=null;
			foreach (Crownwood.Magic.Controls.TabPage page in tabMessages.TabPages)
			{
				InterfaceClient clicomp=(InterfaceClient)page.Tag;
				if ((clicomp.UserHash!=null)&& (m_LastClient.UserHash!=null))
				{
					if (m_HashToString(m_LastClient.UserHash)==m_HashToString(clicomp.UserHash))
					{
						messagePage=page;
						break;
					}
				} 
				else 
				{
					if ((m_LastClient.ID==clicomp.ID)&&(m_LastClient.Port==clicomp.Port))
					{
						messagePage=page;
						break;
					}
				}
			}
			if (messagePage==null)
			{
				RichTextBox textBoxMsg=new RichTextBox();
				textBoxMsg.BackColor=Color.White;
				textBoxMsg.Multiline=true;
				textBoxMsg.ReadOnly=true;
				textBoxMsg.BorderStyle=BorderStyle.FixedSingle;
				textBoxMsg.ScrollBars=RichTextBoxScrollBars.Both;
				textBoxMsg.Dock=DockStyle.Fill;
				textBoxMsg.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
				textBoxMsg.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");
				textBoxMsg.DetectUrls=true;
				textBoxMsg.LinkClicked+=new LinkClickedEventHandler(Link_Clicked);
				
				//textBoxMsg.ContextMenu.MenuItems.Add("-");
				textBoxMsg.ContextMenu = new ContextMenu();
				textBoxMsg.ContextMenu.MenuItems.Add("Add to friends",new EventHandler(OnAddFriend));

				messagePage = new Crownwood.Magic.Controls.TabPage(m_LastClient.Name, textBoxMsg, null);
				messagePage.Tag=m_LastClient;
				tabMessages.TabPages.Add(messagePage);
				tabMessages.SelectedIndex=tabMessages.TabPages.Count-1;
				if (m_LastMessage.Length>0) textBoxMsg.Text=DateTime.Now.ToString()+"<-"+m_LastMessage+"\r\n";
				textBoxMessage.Focus();
			}
			else
			{
				RichTextBox textBox=(RichTextBox)messagePage.Control;
				tabMessages.SelectedIndex=tabMessages.TabPages.IndexOf(messagePage);
				if (m_LastMessage.Length>0)
				{
					textBox.Text+=DateTime.Now.ToString()+"<-"+m_LastMessage+"\r\n";
					Win32.SendMessage(textBox.Handle,Win32.WM_VSCROLL, (IntPtr)Win32.SB_BOTTOM,(IntPtr)0);
				}
			}
		}
		protected void OnClosePressed(object sender,EventArgs e)
		{
			if (tabMessages.SelectedIndex<0) return;
			tabMessages.TabPages.RemoveAt(tabMessages.SelectedIndex);
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonAddFriend = new System.Windows.Forms.Button();
			this.listViewFriends = new eLePhant.Client.eLePhantListView();
			this.panelAddFriend = new System.Windows.Forms.Panel();
			this.textBoxUserHash = new System.Windows.Forms.TextBox();
			this.labelOptional = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.buttonAddFriendCancel = new System.Windows.Forms.Button();
			this.buttonAddFriendOk = new System.Windows.Forms.Button();
			this.textBoxPort = new System.Windows.Forms.TextBox();
			this.textBoxIP = new System.Windows.Forms.TextBox();
			this.labelPort = new System.Windows.Forms.Label();
			this.labelIP = new System.Windows.Forms.Label();
			this.labelUserHash = new System.Windows.Forms.Label();
			this.tabMessages = new Crownwood.Magic.Controls.TabControl();
			this.textBoxMessage = new System.Windows.Forms.TextBox();
			this.buttonSend = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.imageListFriends = new System.Windows.Forms.ImageList(this.components);
			this.panel1.SuspendLayout();
			this.panelAddFriend.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.buttonAddFriend);
			this.panel1.Controls.Add(this.listViewFriends);
			this.panel1.Controls.Add(this.panelAddFriend);
			this.panel1.Location = new System.Drawing.Point(8, 6);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(176, 373);
			this.panel1.TabIndex = 0;
			// 
			// buttonAddFriend
			// 
			this.buttonAddFriend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddFriend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddFriend.Location = new System.Drawing.Point(6, 338);
			this.buttonAddFriend.Name = "buttonAddFriend";
			this.buttonAddFriend.Size = new System.Drawing.Size(160, 23);
			this.buttonAddFriend.TabIndex = 2;
			this.buttonAddFriend.Text = "Add Friend";
			this.buttonAddFriend.Click += new System.EventHandler(this.buttonAddFriend_Click);
			// 
			// listViewFriends
			// 
			this.listViewFriends.AllowColumnReorder = true;
			this.listViewFriends.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.listViewFriends.AutoArrange = false;
			this.listViewFriends.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listViewFriends.DefaultCustomDraw = true;
			this.listViewFriends.FullRowSelect = true;
			this.listViewFriends.FullyCustomHeader = true;
			this.listViewFriends.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewFriends.HideSelection = false;
			this.listViewFriends.IncreaseHeaderHeight = 0;
			this.listViewFriends.Location = new System.Drawing.Point(6, 2);
			this.listViewFriends.Name = "listViewFriends";
			this.listViewFriends.Size = new System.Drawing.Size(164, 332);
			this.listViewFriends.sortColumn = -1;
			this.listViewFriends.TabIndex = 0;
			this.listViewFriends.View = System.Windows.Forms.View.Details;
			this.listViewFriends.SelectedIndexChanged += new System.EventHandler(this.OnChangeFriend);
			// 
			// panelAddFriend
			// 
			this.panelAddFriend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panelAddFriend.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.panelAddFriend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelAddFriend.Controls.Add(this.textBoxUserHash);
			this.panelAddFriend.Controls.Add(this.labelOptional);
			this.panelAddFriend.Controls.Add(this.textBoxName);
			this.panelAddFriend.Controls.Add(this.labelName);
			this.panelAddFriend.Controls.Add(this.buttonAddFriendCancel);
			this.panelAddFriend.Controls.Add(this.buttonAddFriendOk);
			this.panelAddFriend.Controls.Add(this.textBoxPort);
			this.panelAddFriend.Controls.Add(this.textBoxIP);
			this.panelAddFriend.Controls.Add(this.labelPort);
			this.panelAddFriend.Controls.Add(this.labelIP);
			this.panelAddFriend.Controls.Add(this.labelUserHash);
			this.panelAddFriend.Location = new System.Drawing.Point(6, 198);
			this.panelAddFriend.Name = "panelAddFriend";
			this.panelAddFriend.Size = new System.Drawing.Size(164, 168);
			this.panelAddFriend.TabIndex = 3;
			this.panelAddFriend.Visible = false;
			// 
			// textBoxUserHash
			// 
			this.textBoxUserHash.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxUserHash.Location = new System.Drawing.Point(4, 88);
			this.textBoxUserHash.Name = "textBoxUserHash";
			this.textBoxUserHash.Size = new System.Drawing.Size(154, 20);
			this.textBoxUserHash.TabIndex = 6;
			this.textBoxUserHash.Text = "";
			// 
			// labelOptional
			// 
			this.labelOptional.BackColor = System.Drawing.Color.Transparent;
			this.labelOptional.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelOptional.Location = new System.Drawing.Point(82, 74);
			this.labelOptional.Name = "labelOptional";
			this.labelOptional.Size = new System.Drawing.Size(62, 18);
			this.labelOptional.TabIndex = 10;
			this.labelOptional.Text = "(optional)";
			this.labelOptional.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBoxName
			// 
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.Location = new System.Drawing.Point(4, 18);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(154, 20);
			this.textBoxName.TabIndex = 9;
			this.textBoxName.Text = "My new friend";
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(2, 4);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(100, 18);
			this.labelName.TabIndex = 8;
			this.labelName.Text = "Name";
			// 
			// buttonAddFriendCancel
			// 
			this.buttonAddFriendCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddFriendCancel.Location = new System.Drawing.Point(4, 140);
			this.buttonAddFriendCancel.Name = "buttonAddFriendCancel";
			this.buttonAddFriendCancel.Size = new System.Drawing.Size(154, 23);
			this.buttonAddFriendCancel.TabIndex = 7;
			this.buttonAddFriendCancel.Text = "Cancel";
			this.buttonAddFriendCancel.Click += new System.EventHandler(this.buttonAddFriendCancel_Click);
			// 
			// buttonAddFriendOk
			// 
			this.buttonAddFriendOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddFriendOk.Location = new System.Drawing.Point(4, 114);
			this.buttonAddFriendOk.Name = "buttonAddFriendOk";
			this.buttonAddFriendOk.Size = new System.Drawing.Size(154, 23);
			this.buttonAddFriendOk.TabIndex = 6;
			this.buttonAddFriendOk.Text = "Ok";
			this.buttonAddFriendOk.Click += new System.EventHandler(this.buttonAddFriendOk_Click);
			// 
			// textBoxPort
			// 
			this.textBoxPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxPort.Location = new System.Drawing.Point(110, 52);
			this.textBoxPort.Name = "textBoxPort";
			this.textBoxPort.Size = new System.Drawing.Size(48, 20);
			this.textBoxPort.TabIndex = 5;
			this.textBoxPort.Text = "";
			this.textBoxPort.TextChanged += new System.EventHandler(this.textBoxPort_TextChanged);
			// 
			// textBoxIP
			// 
			this.textBoxIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxIP.Location = new System.Drawing.Point(4, 52);
			this.textBoxIP.Name = "textBoxIP";
			this.textBoxIP.Size = new System.Drawing.Size(94, 20);
			this.textBoxIP.TabIndex = 4;
			this.textBoxIP.Text = "";
			// 
			// labelPort
			// 
			this.labelPort.Location = new System.Drawing.Point(102, 40);
			this.labelPort.Name = "labelPort";
			this.labelPort.Size = new System.Drawing.Size(52, 23);
			this.labelPort.TabIndex = 3;
			this.labelPort.Text = "Port";
			// 
			// labelIP
			// 
			this.labelIP.Location = new System.Drawing.Point(2, 40);
			this.labelIP.Name = "labelIP";
			this.labelIP.Size = new System.Drawing.Size(68, 23);
			this.labelIP.TabIndex = 2;
			this.labelIP.Text = "IP or ID";
			// 
			// labelUserHash
			// 
			this.labelUserHash.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.labelUserHash.Location = new System.Drawing.Point(2, 74);
			this.labelUserHash.Name = "labelUserHash";
			this.labelUserHash.Size = new System.Drawing.Size(116, 18);
			this.labelUserHash.TabIndex = 0;
			this.labelUserHash.Text = "User Hash";
			// 
			// tabMessages
			// 
			this.tabMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabMessages.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tabMessages.HideTabsMode = Crownwood.Magic.Controls.TabControl.HideTabsModes.ShowAlways;
			this.tabMessages.Location = new System.Drawing.Point(8, 4);
			this.tabMessages.Name = "tabMessages";
			this.tabMessages.PositionTop = true;
			this.tabMessages.ShowArrows = true;
			this.tabMessages.ShowClose = true;
			this.tabMessages.ShrinkPagesToFit = false;
			this.tabMessages.Size = new System.Drawing.Size(404, 333);
			this.tabMessages.TabIndex = 3;
			// 
			// textBoxMessage
			// 
			this.textBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxMessage.Location = new System.Drawing.Point(8, 343);
			this.textBoxMessage.Name = "textBoxMessage";
			this.textBoxMessage.Size = new System.Drawing.Size(320, 20);
			this.textBoxMessage.TabIndex = 2;
			this.textBoxMessage.Text = "";
			this.textBoxMessage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxMessage_KeyUp);
			// 
			// buttonSend
			// 
			this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSend.Location = new System.Drawing.Point(332, 343);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(80, 21);
			this.buttonSend.TabIndex = 14;
			this.buttonSend.Text = "Send";
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.tabMessages);
			this.panel2.Controls.Add(this.textBoxMessage);
			this.panel2.Controls.Add(this.buttonSend);
			this.panel2.Location = new System.Drawing.Point(188, 6);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(422, 373);
			this.panel2.TabIndex = 15;
			// 
			// imageListFriends
			// 
			this.imageListFriends.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListFriends.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// FormFriends
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.ClientSize = new System.Drawing.Size(618, 386);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormFriends";
			this.Text = "FormFriends";
			this.panel1.ResumeLayout(false);
			this.panelAddFriend.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonSend_Click(object sender, System.EventArgs e)
		{
			if ((tabMessages.SelectedIndex<0)||(textBoxMessage.Text.Length<=0)) return;
			Crownwood.Magic.Controls.TabPage messagePage=tabMessages.TabPages[tabMessages.SelectedIndex];
			InterfaceClient client=(InterfaceClient)messagePage.Tag;
			RichTextBox textBox=(RichTextBox)messagePage.Control;
			textBox.Text+=DateTime.Now.ToString()+"->"+textBoxMessage.Text+"\r\n";
			Win32.SendMessage(textBox.Handle,Win32.WM_VSCROLL, (IntPtr)Win32.SB_BOTTOM,(IntPtr)0);
			krnGateway.SendChatMessage(client.ID,client.Port,client.ServerIP,client.UserHash,textBoxMessage.Text);
			textBoxMessage.Text="";
		}

		private void textBoxMessage_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData==Keys.Enter) buttonSend_Click(this,null);
		}

		private void buttonAddFriend_Click(object sender, System.EventArgs e)
		{
			listViewFriends.Height-=panelAddFriend.Height-buttonAddFriend.Height;
			panelAddFriend.Visible=true;
			buttonAddFriend.Visible=false;
			buttonAddFriendOk.Text=eLePhantForm.Globalization["LBL_ADD"];
			textBoxName.Text=eLePhantForm.Globalization["LBL_MYNEWFRIEND"];
			textBoxIP.Text="";
			textBoxPort.Text="";
			textBoxUserHash.Text="";
			textBoxUserHash.Enabled=true;
			textBoxIP.Enabled=true;
			textBoxPort.Enabled=true;
		}

		private void buttonAddFriendOk_Click(object sender, System.EventArgs e)
		{
			bool validate = false;
			uint ip;
			ip=0;
			if ((textBoxIP.Text!="") && (textBoxName.Text!="") && (textBoxPort.Text!="")) 
			{
				//validate ip or id
				//if (Regex.IsMatch(textBoxIP.Text, @"^(((?<=255\\d{1,3})\.){3}(?<=255\\d{1,3}))$")) 
				if (Regex.IsMatch(textBoxIP.Text, @"^(((\d{1,3})\.){3}(\d{1,3}))$")) 
				{
					try 
					{
						IPAddress DirectionIP=IPAddress.Parse(textBoxIP.Text);
						ip=BitConverter.ToUInt32(DirectionIP.GetAddressBytes(),0);
					}
					catch
					{
						ip=0;
					}

				}

				if ((ip==0) && (Regex.IsMatch(textBoxIP.Text,@"^\d{9,12}$"))) 
				{
					ip=Convert.ToUInt32(textBoxIP.Text);
				}
				//validate port

				if ((ip!=0) && (Regex.IsMatch(textBoxPort.Text,@"^\d{1,5}$")) && Convert.ToInt32(textBoxPort.Text)<=65535) 
				{
					validate=true;
				}
				if ((!textBoxIP.Enabled)&&(!textBoxPort.Enabled)) validate=true;
			}				
				
			if (validate)
			{
				SFriend friend= new SFriend();
				friend.Name = textBoxName.Text;
				friend.OurName = textBoxName.Text;
				if (textBoxUserHash.Text!="") 
				{					
					byte[] x = new byte[textBoxUserHash.Text.Length];
					x = Convert.FromBase64String( textBoxUserHash.Text );
					friend.UserHash=x;
				}
				friend.ID = ip;
				friend.FriendSlot=false;
				friend.Port = Convert.ToUInt16(textBoxPort.Text);
				listViewFriends.Height+=panelAddFriend.Height-buttonAddFriend.Height;
				panelAddFriend.Visible=false;
				krnGateway.AddFriend(friend);
				buttonAddFriend.Visible=true;		
			} 
			else 
			{
				//send a message box
			}
		}

		private void buttonAddFriendCancel_Click(object sender, System.EventArgs e)
		{
			listViewFriends.Height+=panelAddFriend.Height-buttonAddFriend.Height;
			panelAddFriend.Visible=false;
			buttonAddFriend.Visible=true;			
		}

		private void textBoxPort_TextChanged(object sender, System.EventArgs e)
		{
			//validar que textboxport es un número
		}
		
		public void ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");

			panel1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			panel2.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			panelAddFriend.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");

			tabMessages.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			tabMessages.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");

			//labelFriends.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			//labelFriends.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelIP.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelIP.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelName.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelName.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelOptional.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelOptional.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelPort.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelPort.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelUserHash.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelUserHash.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			textBoxName.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxName.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxIP.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxIP.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxPort.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxPort.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");
			
			textBoxUserHash.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxUserHash.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxMessage.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxMessage.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			buttonAddFriend.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonAddFriend.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonAddFriendCancel.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonAddFriendCancel.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonAddFriendOk.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonAddFriendOk.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonSend.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonSend.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			listViewFriends.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			listViewFriends.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			listViewFriends.headerForeColor=eLePhantForm.Skin.GetColor("listsHeaderForeColor");
			listViewFriends.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");

			//m_SetFlatScrollBar(eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb());
			listViewFriends.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();

			imageListFriends.Images.Clear();
			imageListFriends.Images.Add(eLePhantForm.Skin.GetBitmap("Friend"));
			imageListFriends.Images.Add(eLePhantForm.Skin.GetBitmap("FriendSlot"));

			listViewFriends.SmallImageList=imageListFriends;

			this.Refresh();
		}
		
		private void Link_Clicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(e.LinkText);

		}
	}
}
