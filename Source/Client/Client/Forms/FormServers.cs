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
using System.Diagnostics;
using eLePhant.eDonkey;
using eLePhant.Types;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for FormServers.
	/// </summary>
	public class FormServers : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		private CkernelGateway krnGateway;
		private System.Windows.Forms.ContextMenu contextMenu1;
		
		public System.Windows.Forms.Label labelmsg;
		private System.Windows.Forms.MenuItem menuItemClearLog;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textBoxServerMetUri;
		private System.Windows.Forms.Button buttonDownloadServerList;
		private System.Windows.Forms.Label labelPort;
		private System.Windows.Forms.TextBox textBoxIP;
		private System.Windows.Forms.Label labelIP;
		private System.Windows.Forms.Button buttonAddServer;
		private System.Windows.Forms.TextBox textBoxPort;
		public eLePhant.Client.serversListView serversList;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RichTextBox richTextBoxLog;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ImageList imageList1;
		
		private delegate void LogDelegate(object sender, Constants.Log importance,string strMsg);
		private LogDelegate logDelegate;

		public FormServers()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			logDelegate=new LogDelegate(m_Log);
			m_Globalize();
		}
		
		public void Connect(CkernelGateway in_krnGateway)
		{
			krnGateway=in_krnGateway;
			krnGateway.OnLogMessage+=new LogEvent(OnLog);
			serversList.Initilize(krnGateway);
		}
		
		public void OnLog(Constants.Log importance,string strMsg)
		{
			this.BeginInvoke(logDelegate,new object[] {this,importance,strMsg});
		}

		private void m_Log(object sender, Constants.Log importance,string strMsg)
		{
			if (importance==Constants.Log.Notify) labelmsg.Text=strMsg;
			string newline=DateTime.Now.ToShortTimeString()+" "+strMsg+"\n";
			lock(richTextBoxLog)
			{
				richTextBoxLog.AppendText(newline);
			}
			Win32.SendMessage(richTextBoxLog.Handle,Win32.WM_VSCROLL, (IntPtr)Win32.SB_BOTTOM,(IntPtr)0);
//			if (richTextBoxLog.Lines.Length>richTextBoxLog.Height/richTextBoxLog.Font.Height)
//			{
//				Win32.SendMessage(richTextBoxLog.Handle, Win32.EM_LINESCROLL,(IntPtr)0, (IntPtr)(int)(richTextBoxLog.Lines.Length-richTextBoxLog.Height/richTextBoxLog.Font.Height));
//				Debug.WriteLine((richTextBoxLog.Lines.Length-richTextBoxLog.Height/richTextBoxLog.Font.Height).ToString());
//			}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormServers));
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItemClearLog = new System.Windows.Forms.MenuItem();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.serversList = new eLePhant.Client.serversListView(this.components);
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.textBoxServerMetUri = new System.Windows.Forms.TextBox();
			this.buttonDownloadServerList = new System.Windows.Forms.Button();
			this.labelPort = new System.Windows.Forms.Label();
			this.textBoxIP = new System.Windows.Forms.TextBox();
			this.labelIP = new System.Windows.Forms.Label();
			this.buttonAddServer = new System.Windows.Forms.Button();
			this.textBoxPort = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemClearLog});
			this.contextMenu1.Popup += new System.EventHandler(this.ContextMenu1_Popup);
			// 
			// menuItemClearLog
			// 
			this.menuItemClearLog.Index = 0;
			this.menuItemClearLog.Text = "Clear Log";
			this.menuItemClearLog.Click += new System.EventHandler(this.menuItemClearLog_Click);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.panel1);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(528, 352);
			this.panel3.TabIndex = 7;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.serversList);
			this.panel1.Controls.Add(this.textBoxServerMetUri);
			this.panel1.Controls.Add(this.buttonDownloadServerList);
			this.panel1.Controls.Add(this.labelPort);
			this.panel1.Controls.Add(this.textBoxIP);
			this.panel1.Controls.Add(this.labelIP);
			this.panel1.Controls.Add(this.buttonAddServer);
			this.panel1.Controls.Add(this.textBoxPort);
			this.panel1.Location = new System.Drawing.Point(7, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(519, 336);
			this.panel1.TabIndex = 5;
			// 
			// serversList
			// 
			this.serversList.AllowColumnReorder = true;
			this.serversList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.serversList.AutoArrange = false;
			this.serversList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.serversList.DefaultCustomDraw = true;
			this.serversList.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.serversList.FullRowSelect = true;
			this.serversList.FullyCustomHeader = false;
			this.serversList.HideSelection = false;
			this.serversList.IncreaseHeaderHeight = 0;
			this.serversList.Location = new System.Drawing.Point(7, 7);
			this.serversList.Name = "serversList";
			this.serversList.Size = new System.Drawing.Size(503, 256);
			this.serversList.SmallImageList = this.imageList1;
			this.serversList.sortColumn = -1;
			this.serversList.TabIndex = 20;
			this.serversList.View = System.Windows.Forms.View.Details;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// textBoxServerMetUri
			// 
			this.textBoxServerMetUri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxServerMetUri.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxServerMetUri.Location = new System.Drawing.Point(7, 273);
			this.textBoxServerMetUri.Name = "textBoxServerMetUri";
			this.textBoxServerMetUri.Size = new System.Drawing.Size(303, 20);
			this.textBoxServerMetUri.TabIndex = 19;
			this.textBoxServerMetUri.Text = "";
			// 
			// buttonDownloadServerList
			// 
			this.buttonDownloadServerList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDownloadServerList.BackColor = System.Drawing.SystemColors.Control;
			this.buttonDownloadServerList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDownloadServerList.Image = ((System.Drawing.Image)(resources.GetObject("buttonDownloadServerList.Image")));
			this.buttonDownloadServerList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonDownloadServerList.Location = new System.Drawing.Point(318, 273);
			this.buttonDownloadServerList.Name = "buttonDownloadServerList";
			this.buttonDownloadServerList.Size = new System.Drawing.Size(192, 21);
			this.buttonDownloadServerList.TabIndex = 18;
			this.buttonDownloadServerList.Text = "Download servers list";
			this.buttonDownloadServerList.Click += new System.EventHandler(this.ButtonDownloadServerList_Click);
			// 
			// labelPort
			// 
			this.labelPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPort.Location = new System.Drawing.Point(206, 305);
			this.labelPort.Name = "labelPort";
			this.labelPort.Size = new System.Drawing.Size(41, 16);
			this.labelPort.TabIndex = 17;
			this.labelPort.Text = "Port:";
			this.labelPort.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBoxIP
			// 
			this.textBoxIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxIP.Location = new System.Drawing.Point(118, 303);
			this.textBoxIP.MaxLength = 15;
			this.textBoxIP.Name = "textBoxIP";
			this.textBoxIP.Size = new System.Drawing.Size(88, 20);
			this.textBoxIP.TabIndex = 16;
			this.textBoxIP.Text = "";
			// 
			// labelIP
			// 
			this.labelIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelIP.Location = new System.Drawing.Point(103, 306);
			this.labelIP.Name = "labelIP";
			this.labelIP.Size = new System.Drawing.Size(24, 16);
			this.labelIP.TabIndex = 15;
			this.labelIP.Text = "IP:";
			// 
			// buttonAddServer
			// 
			this.buttonAddServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddServer.BackColor = System.Drawing.SystemColors.Control;
			this.buttonAddServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddServer.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddServer.Image")));
			this.buttonAddServer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonAddServer.Location = new System.Drawing.Point(318, 303);
			this.buttonAddServer.Name = "buttonAddServer";
			this.buttonAddServer.Size = new System.Drawing.Size(192, 21);
			this.buttonAddServer.TabIndex = 14;
			this.buttonAddServer.Text = "Add server";
			this.buttonAddServer.Click += new System.EventHandler(this.buttonAddServer_Click);
			// 
			// textBoxPort
			// 
			this.textBoxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxPort.Location = new System.Drawing.Point(254, 303);
			this.textBoxPort.MaxLength = 5;
			this.textBoxPort.Name = "textBoxPort";
			this.textBoxPort.Size = new System.Drawing.Size(56, 20);
			this.textBoxPort.TabIndex = 13;
			this.textBoxPort.Text = "";
			this.textBoxPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPort_KeyDown);
			this.textBoxPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPort_KeyPress);
			this.textBoxPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUpCheck);
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.richTextBoxLog);
			this.panel2.Location = new System.Drawing.Point(8, 8);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(232, 336);
			this.panel2.TabIndex = 6;
			// 
			// richTextBoxLog
			// 
			this.richTextBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBoxLog.ContextMenu = this.contextMenu1;
			this.richTextBoxLog.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.richTextBoxLog.Location = new System.Drawing.Point(6, 8);
			this.richTextBoxLog.Name = "richTextBoxLog";
			this.richTextBoxLog.ReadOnly = true;
			this.richTextBoxLog.Size = new System.Drawing.Size(216, 318);
			this.richTextBoxLog.TabIndex = 3;
			this.richTextBoxLog.Text = "";
			this.richTextBoxLog.WordWrap = false;
			this.richTextBoxLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.Link_Clicked);
			// 
			// panel4
			// 
			this.panel4.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.panel4.Controls.Add(this.panel2);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(528, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(248, 352);
			this.panel4.TabIndex = 8;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(528, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(8, 352);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// FormServers
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.ClientSize = new System.Drawing.Size(776, 352);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.panel3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormServers";
			this.Text = "FormServers";
			this.panel3.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void m_Globalize()
		{
			contextMenu1.MenuItems[0].Text=eLePhantForm.Globalization["LBL_CLEARLOG"];
			labelPort.Text=eLePhantForm.Globalization["LBL_PORT"]+":";
			buttonAddServer.Text=eLePhantForm.Globalization["LBL_ADDSERVER"];
			buttonDownloadServerList.Text=eLePhantForm.Globalization["LBL_DOWNLOAD_SERVERLIST"];
		}

		public void Globalize()
		{
			m_Globalize();
		}

		public void SaveListsSettings()
		{
			serversList.SavePreferences();
		}
		public void ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");

			panel1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			panel2.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			panel3.BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			panel4.BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");

			serversList.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			serversList.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			serversList.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			serversList.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			serversList.headerForeColor=eLePhantForm.Skin.GetColor("listsHeaderForeColor");

			richTextBoxLog.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			richTextBoxLog.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			labelIP.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelIP.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelPort.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelPort.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			buttonAddServer.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonAddServer.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");
			
			buttonDownloadServerList.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonDownloadServerList.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");
			
			this.Refresh();
		}
		#region Events
		/// <summary>
		/// All events
		/// </summary>
		
		private void ContextMenu1_Popup(object sender, System.EventArgs e)
		{
			//Event on Popup
						
		}

		protected void ClearLog_OnClick(System.Object sender, System.EventArgs e)
		{
			richTextBoxLog.Clear();
		}

		private void Link_Clicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(e.LinkText);

		}

		#endregion

		private void buttonAddServer_Click(object sender, System.EventArgs e)
		{
			if((textBoxIP.Text.Length>0)&&(textBoxPort.Text.Length>0))
			{					
				krnGateway.DownloadElink("ed2k://|server|"+textBoxIP.Text+"|"+textBoxPort.Text+"|/",false);
			}

		}

		private void menuItemClearLog_Click(object sender, System.EventArgs e)
		{
			richTextBoxLog.Clear();
		}

		private void ButtonDownloadServerList_Click(object sender, System.EventArgs e)
		{
			if(textBoxServerMetUri.Text.StartsWith("http"))
				krnGateway.DownloadServerList(textBoxServerMetUri.Text);
		}

		private void OnKeyUpCheck(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			TextBox textbox=(TextBox)sender;
			if(textbox.TextLength>0 && uint.Parse(textbox.Text)>ushort.MaxValue)
			{
				textbox.Text = textbox.Text.Remove(textbox.Text.Length-1,1);
				textbox.SelectionStart = textbox.TextLength;
			}
		}

		private void splitter1_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e)
		{
		
		}

		private void textBoxPort_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{

		}

		private void textBoxPort_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ((((byte)e.KeyChar<48)||((byte)e.KeyChar>57))&&
				((byte)e.KeyChar!=8))
				e.Handled=true;		
		}
	}
}
