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
using System.IO;
using System.Diagnostics;
using eLePhant.eDonkey;
using eLePhant.Types;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for FormUploads.
	/// </summary>
	public class FormUploads : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private FormAviDetails FAviDetails;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private Crownwood.Magic.Controls.TabControl tabControlDetails;
		private Crownwood.Magic.Controls.TabPage tabPageDetails;
		private System.Windows.Forms.TextBox tx_fileName;
		private System.Windows.Forms.TextBox tx_completeName;
		private System.Windows.Forms.Label label6;
		private Crownwood.Magic.Controls.TabPage tabPagStats;
		private System.Windows.Forms.Label labelSessionUploadedRes;
		private System.Windows.Forms.Label labelSessionDownloadedRes;
		private System.Windows.Forms.Label labelSessionRequestsRes;
		private System.Windows.Forms.Label labelSessionUploaded;
		private System.Windows.Forms.Label labelSessionDownloaded;
		private System.Windows.Forms.Label labelSessionRequests;
		private System.Windows.Forms.Panel panel_fileDetails;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelComment;
		private System.Windows.Forms.TextBox textBoxComment;
		private System.Windows.Forms.Button buttonSetCommet;
		private System.Windows.Forms.Button buttonOpenFolder;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonRename;
		private System.Windows.Forms.Panel panelLeft;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel panelLeftDetails;
		private System.Windows.Forms.Button buttonReload;
		private System.Windows.Forms.Label labelSharedFiles;
		public eLePhant.Client.sharedListView sharedListView;
		private System.Windows.Forms.Panel panelUpDetails;
		public eLePhant.Client.uploadsListView uploadsList;
		private System.Windows.Forms.ImageList imageList1;

		private CkernelGateway krnGateway;

		public FormUploads()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Initialize();
			this.tabControlDetails.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiBox;
			this.tabControlDetails.PositionTop = true;
			//tabPageQueue.Visible=false;
			m_Globalize();
        }
		
		private void m_Globalize()
		{
			labelSharedFiles.Text=eLePhantForm.Globalization["LBL_SHAREDFILES"];
			buttonReload.Text=eLePhantForm.Globalization["LBL_RELOADSHAREDFILES"];
			tabPageDetails.Title=eLePhantForm.Globalization["LBL_DETAILS"];
			//tabPageQueue.Title=eLePhantForm.Globalization["LBL_QUEUE"];
			label6.Text = eLePhantForm.Globalization["LBL_PATH"]+":";
			label5.Text = eLePhantForm.Globalization["LBL_FILE"]+":";
			labelSessionDownloaded.Text=eLePhantForm.Globalization["LBL_DOWNLOADED"];
			labelSessionRequests.Text=eLePhantForm.Globalization["LBL_REQUESTS"];
			labelSessionUploaded.Text=eLePhantForm.Globalization["LBL_UPLOADED"];
			labelComment.Text=eLePhantForm.Globalization["LBL_COMMENT"];
			buttonSetCommet.Text=eLePhantForm.Globalization["LBL_SETCOMMENT"];
			toolTip1.SetToolTip(this.buttonOpenFolder, eLePhantForm.Globalization["LBL_OPENFOLDER"]);
			buttonRename.Text=eLePhantForm.Globalization["LBL_RENAME"];
		}

		public void Globalize()
		{
			m_Globalize();
		}

		private void Initialize()
		{
			FAviDetails=new FormAviDetails();
			FAviDetails.TopLevel=false;
			FAviDetails.Dock = DockStyle.Fill;
			panel_fileDetails.Controls.Add(FAviDetails);
			FAviDetails.Dock = DockStyle.Fill;
			FAviDetails.ApplySkin();
		}

		public void Connect(CkernelGateway in_krnGateway)
		{
			krnGateway=in_krnGateway;
			uploadsList.Initilize(krnGateway);
			sharedListView.Initilize(krnGateway);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormUploads));
			this.panel1 = new System.Windows.Forms.Panel();
			this.panelUpDetails = new System.Windows.Forms.Panel();
			this.uploadsList = new eLePhant.Client.uploadsListView(this.components);
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.tabControlDetails = new Crownwood.Magic.Controls.TabControl();
			this.tabPageDetails = new Crownwood.Magic.Controls.TabPage();
			this.buttonRename = new System.Windows.Forms.Button();
			this.buttonOpenFolder = new System.Windows.Forms.Button();
			this.buttonSetCommet = new System.Windows.Forms.Button();
			this.textBoxComment = new System.Windows.Forms.TextBox();
			this.labelComment = new System.Windows.Forms.Label();
			this.tx_fileName = new System.Windows.Forms.TextBox();
			this.tx_completeName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.panel_fileDetails = new System.Windows.Forms.Panel();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.tabPagStats = new Crownwood.Magic.Controls.TabPage();
			this.labelSessionUploadedRes = new System.Windows.Forms.Label();
			this.labelSessionDownloadedRes = new System.Windows.Forms.Label();
			this.labelSessionRequestsRes = new System.Windows.Forms.Label();
			this.labelSessionUploaded = new System.Windows.Forms.Label();
			this.labelSessionDownloaded = new System.Windows.Forms.Label();
			this.labelSessionRequests = new System.Windows.Forms.Label();
			this.panelLeft = new System.Windows.Forms.Panel();
			this.panelLeftDetails = new System.Windows.Forms.Panel();
			this.buttonReload = new System.Windows.Forms.Button();
			this.labelSharedFiles = new System.Windows.Forms.Label();
			this.sharedListView = new eLePhant.Client.sharedListView(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.panel1.SuspendLayout();
			this.panelUpDetails.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.tabPageDetails.SuspendLayout();
			this.tabPagStats.SuspendLayout();
			this.panelLeft.SuspendLayout();
			this.panelLeftDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panelUpDetails);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(776, 128);
			this.panel1.TabIndex = 0;
			// 
			// panelUpDetails
			// 
			this.panelUpDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelUpDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelUpDetails.Controls.Add(this.uploadsList);
			this.panelUpDetails.Location = new System.Drawing.Point(8, 8);
			this.panelUpDetails.Name = "panelUpDetails";
			this.panelUpDetails.Size = new System.Drawing.Size(760, 118);
			this.panelUpDetails.TabIndex = 2;
			// 
			// uploadsList
			// 
			this.uploadsList.AllowColumnReorder = true;
			this.uploadsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.uploadsList.AutoArrange = false;
			this.uploadsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.uploadsList.DefaultCustomDraw = true;
			this.uploadsList.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.uploadsList.FullRowSelect = true;
			this.uploadsList.FullyCustomHeader = false;
			this.uploadsList.HideSelection = false;
			this.uploadsList.IncreaseHeaderHeight = 0;
			this.uploadsList.Location = new System.Drawing.Point(8, 8);
			this.uploadsList.Name = "uploadsList";
			this.uploadsList.Size = new System.Drawing.Size(744, 104);
			this.uploadsList.sortColumn = -1;
			this.uploadsList.TabIndex = 2;
			this.uploadsList.View = System.Windows.Forms.View.Details;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 128);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(776, 7);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.splitter2);
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Controls.Add(this.panelLeft);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 135);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(776, 265);
			this.panel2.TabIndex = 2;
			// 
			// splitter2
			// 
			this.splitter2.Location = new System.Drawing.Point(392, 0);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(8, 265);
			this.splitter2.TabIndex = 5;
			this.splitter2.TabStop = false;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel3.Controls.Add(this.panel4);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(392, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(384, 265);
			this.panel3.TabIndex = 3;
			// 
			// panel4
			// 
			this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel4.BackColor = System.Drawing.Color.White;
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.tabControlDetails);
			this.panel4.Location = new System.Drawing.Point(8, 3);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(370, 254);
			this.panel4.TabIndex = 15;
			// 
			// tabControlDetails
			// 
			this.tabControlDetails.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tabControlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlDetails.HideTabsMode = Crownwood.Magic.Controls.TabControl.HideTabsModes.ShowAlways;
			this.tabControlDetails.Location = new System.Drawing.Point(0, 0);
			this.tabControlDetails.Name = "tabControlDetails";
			this.tabControlDetails.PositionTop = true;
			this.tabControlDetails.SelectedIndex = 0;
			this.tabControlDetails.SelectedTab = this.tabPageDetails;
			this.tabControlDetails.Size = new System.Drawing.Size(368, 252);
			this.tabControlDetails.TabIndex = 0;
			this.tabControlDetails.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																								this.tabPageDetails,
																								this.tabPagStats});
			this.tabControlDetails.SelectionChanged += new System.EventHandler(this.tabControlDetails_SelectionChanged);
			// 
			// tabPageDetails
			// 
			this.tabPageDetails.Controls.Add(this.buttonRename);
			this.tabPageDetails.Controls.Add(this.buttonOpenFolder);
			this.tabPageDetails.Controls.Add(this.buttonSetCommet);
			this.tabPageDetails.Controls.Add(this.textBoxComment);
			this.tabPageDetails.Controls.Add(this.labelComment);
			this.tabPageDetails.Controls.Add(this.tx_fileName);
			this.tabPageDetails.Controls.Add(this.tx_completeName);
			this.tabPageDetails.Controls.Add(this.label6);
			this.tabPageDetails.Controls.Add(this.label5);
			this.tabPageDetails.Controls.Add(this.panel_fileDetails);
			this.tabPageDetails.ImageIndex = 0;
			this.tabPageDetails.ImageList = this.imageList1;
			this.tabPageDetails.Location = new System.Drawing.Point(0, 0);
			this.tabPageDetails.Name = "tabPageDetails";
			this.tabPageDetails.Size = new System.Drawing.Size(368, 227);
			this.tabPageDetails.TabIndex = 0;
			this.tabPageDetails.Title = "Details";
			// 
			// buttonRename
			// 
			this.buttonRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRename.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonRename.Location = new System.Drawing.Point(232, 2);
			this.buttonRename.Name = "buttonRename";
			this.buttonRename.Size = new System.Drawing.Size(137, 19);
			this.buttonRename.TabIndex = 37;
			this.buttonRename.Text = "Rename";
			this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
			// 
			// buttonOpenFolder
			// 
			this.buttonOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonOpenFolder.Image = ((System.Drawing.Image)(resources.GetObject("buttonOpenFolder.Image")));
			this.buttonOpenFolder.Location = new System.Drawing.Point(340, 20);
			this.buttonOpenFolder.Name = "buttonOpenFolder";
			this.buttonOpenFolder.Size = new System.Drawing.Size(29, 19);
			this.buttonOpenFolder.TabIndex = 36;
			this.toolTip1.SetToolTip(this.buttonOpenFolder, "Open folder");
			this.buttonOpenFolder.Click += new System.EventHandler(this.buttonOpenFolder_Click);
			// 
			// buttonSetCommet
			// 
			this.buttonSetCommet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSetCommet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonSetCommet.Location = new System.Drawing.Point(232, 38);
			this.buttonSetCommet.Name = "buttonSetCommet";
			this.buttonSetCommet.Size = new System.Drawing.Size(137, 19);
			this.buttonSetCommet.TabIndex = 34;
			this.buttonSetCommet.Text = "Set my comment";
			this.buttonSetCommet.Click += new System.EventHandler(this.buttonSetCommet_Click);
			// 
			// textBoxComment
			// 
			this.textBoxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxComment.Font = new System.Drawing.Font("Tahoma", 7F);
			this.textBoxComment.Location = new System.Drawing.Point(84, 38);
			this.textBoxComment.Name = "textBoxComment";
			this.textBoxComment.Size = new System.Drawing.Size(149, 19);
			this.textBoxComment.TabIndex = 33;
			this.textBoxComment.Text = "";
			// 
			// labelComment
			// 
			this.labelComment.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelComment.Font = new System.Drawing.Font("Tahoma", 7F);
			this.labelComment.Location = new System.Drawing.Point(4, 38);
			this.labelComment.Name = "labelComment";
			this.labelComment.Size = new System.Drawing.Size(81, 19);
			this.labelComment.TabIndex = 32;
			this.labelComment.Text = "Comment:";
			// 
			// tx_fileName
			// 
			this.tx_fileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tx_fileName.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_fileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_fileName.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tx_fileName.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_fileName.Location = new System.Drawing.Point(84, 2);
			this.tx_fileName.Name = "tx_fileName";
			this.tx_fileName.Size = new System.Drawing.Size(149, 19);
			this.tx_fileName.TabIndex = 30;
			this.tx_fileName.Text = "";
			// 
			// tx_completeName
			// 
			this.tx_completeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tx_completeName.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_completeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_completeName.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tx_completeName.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_completeName.Location = new System.Drawing.Point(84, 20);
			this.tx_completeName.Name = "tx_completeName";
			this.tx_completeName.ReadOnly = true;
			this.tx_completeName.Size = new System.Drawing.Size(258, 19);
			this.tx_completeName.TabIndex = 29;
			this.tx_completeName.Text = "";
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label6.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.ForeColor = System.Drawing.Color.Black;
			this.label6.Location = new System.Drawing.Point(4, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(81, 19);
			this.label6.TabIndex = 28;
			this.label6.Text = "File:";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.Black;
			this.label5.Location = new System.Drawing.Point(4, 2);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(81, 19);
			this.label5.TabIndex = 27;
			this.label5.Text = "File Name:";
			// 
			// panel_fileDetails
			// 
			this.panel_fileDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel_fileDetails.AutoScroll = true;
			this.panel_fileDetails.Font = new System.Drawing.Font("Tahoma", 7F);
			this.panel_fileDetails.Location = new System.Drawing.Point(4, 58);
			this.panel_fileDetails.Name = "panel_fileDetails";
			this.panel_fileDetails.Size = new System.Drawing.Size(405, 171);
			this.panel_fileDetails.TabIndex = 31;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tabPagStats
			// 
			this.tabPagStats.Controls.Add(this.labelSessionUploadedRes);
			this.tabPagStats.Controls.Add(this.labelSessionDownloadedRes);
			this.tabPagStats.Controls.Add(this.labelSessionRequestsRes);
			this.tabPagStats.Controls.Add(this.labelSessionUploaded);
			this.tabPagStats.Controls.Add(this.labelSessionDownloaded);
			this.tabPagStats.Controls.Add(this.labelSessionRequests);
			this.tabPagStats.ImageIndex = 1;
			this.tabPagStats.ImageList = this.imageList1;
			this.tabPagStats.Location = new System.Drawing.Point(0, 0);
			this.tabPagStats.Name = "tabPagStats";
			this.tabPagStats.Selected = false;
			this.tabPagStats.Size = new System.Drawing.Size(368, 227);
			this.tabPagStats.TabIndex = 2;
			this.tabPagStats.Title = "Stats";
			// 
			// labelSessionUploadedRes
			// 
			this.labelSessionUploadedRes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSessionUploadedRes.Font = new System.Drawing.Font("Tahoma", 7F);
			this.labelSessionUploadedRes.Location = new System.Drawing.Point(194, 44);
			this.labelSessionUploadedRes.Name = "labelSessionUploadedRes";
			this.labelSessionUploadedRes.Size = new System.Drawing.Size(123, 19);
			this.labelSessionUploadedRes.TabIndex = 16;
			this.labelSessionUploadedRes.Text = "0";
			// 
			// labelSessionDownloadedRes
			// 
			this.labelSessionDownloadedRes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSessionDownloadedRes.Font = new System.Drawing.Font("Tahoma", 7F);
			this.labelSessionDownloadedRes.Location = new System.Drawing.Point(194, 26);
			this.labelSessionDownloadedRes.Name = "labelSessionDownloadedRes";
			this.labelSessionDownloadedRes.Size = new System.Drawing.Size(123, 19);
			this.labelSessionDownloadedRes.TabIndex = 15;
			this.labelSessionDownloadedRes.Text = "0";
			// 
			// labelSessionRequestsRes
			// 
			this.labelSessionRequestsRes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSessionRequestsRes.Font = new System.Drawing.Font("Tahoma", 7F);
			this.labelSessionRequestsRes.Location = new System.Drawing.Point(194, 8);
			this.labelSessionRequestsRes.Name = "labelSessionRequestsRes";
			this.labelSessionRequestsRes.Size = new System.Drawing.Size(123, 19);
			this.labelSessionRequestsRes.TabIndex = 14;
			this.labelSessionRequestsRes.Text = "0";
			// 
			// labelSessionUploaded
			// 
			this.labelSessionUploaded.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelSessionUploaded.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSessionUploaded.Font = new System.Drawing.Font("Tahoma", 7F);
			this.labelSessionUploaded.Location = new System.Drawing.Point(10, 44);
			this.labelSessionUploaded.Name = "labelSessionUploaded";
			this.labelSessionUploaded.Size = new System.Drawing.Size(185, 19);
			this.labelSessionUploaded.TabIndex = 13;
			this.labelSessionUploaded.Text = "Session Uploaded:";
			// 
			// labelSessionDownloaded
			// 
			this.labelSessionDownloaded.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelSessionDownloaded.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSessionDownloaded.Font = new System.Drawing.Font("Tahoma", 7F);
			this.labelSessionDownloaded.Location = new System.Drawing.Point(10, 26);
			this.labelSessionDownloaded.Name = "labelSessionDownloaded";
			this.labelSessionDownloaded.Size = new System.Drawing.Size(185, 19);
			this.labelSessionDownloaded.TabIndex = 12;
			this.labelSessionDownloaded.Text = "Session Downloaded:";
			// 
			// labelSessionRequests
			// 
			this.labelSessionRequests.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelSessionRequests.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSessionRequests.Font = new System.Drawing.Font("Tahoma", 7F);
			this.labelSessionRequests.Location = new System.Drawing.Point(10, 8);
			this.labelSessionRequests.Name = "labelSessionRequests";
			this.labelSessionRequests.Size = new System.Drawing.Size(185, 19);
			this.labelSessionRequests.TabIndex = 11;
			this.labelSessionRequests.Text = "Session Requests:";
			// 
			// panelLeft
			// 
			this.panelLeft.Controls.Add(this.panelLeftDetails);
			this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelLeft.Location = new System.Drawing.Point(0, 0);
			this.panelLeft.Name = "panelLeft";
			this.panelLeft.Size = new System.Drawing.Size(392, 265);
			this.panelLeft.TabIndex = 4;
			// 
			// panelLeftDetails
			// 
			this.panelLeftDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelLeftDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelLeftDetails.Controls.Add(this.buttonReload);
			this.panelLeftDetails.Controls.Add(this.labelSharedFiles);
			this.panelLeftDetails.Controls.Add(this.sharedListView);
			this.panelLeftDetails.Location = new System.Drawing.Point(8, 3);
			this.panelLeftDetails.Name = "panelLeftDetails";
			this.panelLeftDetails.Size = new System.Drawing.Size(382, 255);
			this.panelLeftDetails.TabIndex = 0;
			// 
			// buttonReload
			// 
			this.buttonReload.BackColor = System.Drawing.SystemColors.Control;
			this.buttonReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonReload.Image = ((System.Drawing.Image)(resources.GetObject("buttonReload.Image")));
			this.buttonReload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonReload.Location = new System.Drawing.Point(152, 8);
			this.buttonReload.Name = "buttonReload";
			this.buttonReload.Size = new System.Drawing.Size(208, 24);
			this.buttonReload.TabIndex = 17;
			this.buttonReload.Text = "Reload shared list";
			this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
			// 
			// labelSharedFiles
			// 
			this.labelSharedFiles.Location = new System.Drawing.Point(8, 8);
			this.labelSharedFiles.Name = "labelSharedFiles";
			this.labelSharedFiles.Size = new System.Drawing.Size(140, 20);
			this.labelSharedFiles.TabIndex = 15;
			this.labelSharedFiles.Text = "Shared Files";
			this.labelSharedFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// sharedListView
			// 
			this.sharedListView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
			this.sharedListView.AllowColumnReorder = true;
			this.sharedListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.sharedListView.AutoArrange = false;
			this.sharedListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.sharedListView.DefaultCustomDraw = true;
			this.sharedListView.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.sharedListView.FullRowSelect = true;
			this.sharedListView.FullyCustomHeader = false;
			this.sharedListView.HideSelection = false;
			this.sharedListView.IncreaseHeaderHeight = 0;
			this.sharedListView.Location = new System.Drawing.Point(8, 40);
			this.sharedListView.Name = "sharedListView";
			this.sharedListView.Size = new System.Drawing.Size(364, 207);
			this.sharedListView.sortColumn = -1;
			this.sharedListView.TabIndex = 16;
			this.sharedListView.View = System.Windows.Forms.View.Details;
			this.sharedListView.SelectedIndexChanged += new System.EventHandler(this.sharedListView_SelectedIndexChanged);
			// 
			// FormUploads
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.ClientSize = new System.Drawing.Size(776, 400);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormUploads";
			this.Text = "FormUploads";
			this.panel1.ResumeLayout(false);
			this.panelUpDetails.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.tabPageDetails.ResumeLayout(false);
			this.tabPagStats.ResumeLayout(false);
			this.panelLeft.ResumeLayout(false);
			this.panelLeftDetails.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonReload_Click(object sender, System.EventArgs e)
		{
			sharedListView.ReloadList(true);
		}
		public void SaveListsSettings()
		{
			uploadsList.SavePreferences();
			sharedListView.SavePreferences();
		}

		private void m_RefreshStatistics(InterfaceFile file)
		{
			FileStatistics fstatistics=krnGateway.GetFileStatistics(file.strHash);
			labelSessionDownloadedRes.Text=eLePhantListView.SizeToString((uint)fstatistics.SessionDownload);
			labelSessionUploadedRes.Text=eLePhantListView.SizeToString((uint)fstatistics.SessionUpload);
			labelSessionRequestsRes.Text=fstatistics.SessionRequests.ToString();
			byte rating=0;
			string comment="";
			krnGateway.GetFileComment(file.strHash,ref comment,ref rating);
			this.textBoxComment.Text=comment;
		}
		private void sharedListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (sharedListView.SelectedItems.Count>0)
			{
				m_RefreshStatistics((InterfaceFile)sharedListView.SelectedItems[0].Tag);
				showInfo(((InterfaceFile)sharedListView.SelectedItems[0].Tag));
			}
		}

		private void buttonSetCommet_Click(object sender, System.EventArgs e)
		{
			if ((sharedListView.SelectedItems.Count>0)&&(textBoxComment.Text.Length>0))
			{			
				krnGateway.SetFileComment(((InterfaceFile)sharedListView.SelectedItems[0].Tag).strHash,textBoxComment.Text,0);
			}
		}

		private void showInfo(InterfaceFile file)
		{
			sFileDetails FileInfo	= (sFileDetails)krnGateway.GetFileDetails(file.strHash);
			this.tx_fileName.Text	= file.Name;
			this.tx_completeName.Text	= file.CompleteName;
			switch ((byte)FileInfo.Type)
			{
				case ((byte)Constants.FileType.Avi):
					FAviDetails.ShowData((Hashtable)FileInfo.ListDetails);
					FAviDetails.Show();
					break;
				default:
					FAviDetails.Hide();
					break;
			}
		}
		public void ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			
			panel1.BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			panel2.BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			panelLeft.BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			panel3.BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");

			panelLeftDetails.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			panel4.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			panelUpDetails.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");

			panel_fileDetails.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");

			splitter1.BackColor=eLePhantForm.Skin.GetColor("SplittersBackColor");

			buttonReload.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonReload.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonSetCommet.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonSetCommet.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonRename.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonRename.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");
			
			labelSharedFiles.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelSharedFiles.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			sharedListView.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			sharedListView.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			sharedListView.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			sharedListView.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			sharedListView.headerForeColor=eLePhantForm.Skin.GetColor("listsHeaderForeColor");

			uploadsList.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			uploadsList.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			uploadsList.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			uploadsList.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			uploadsList.headerForeColor=eLePhantForm.Skin.GetColor("listsHeaderForeColor");

			tabControlDetails.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			tabControlDetails.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			tabControlDetails.TextInactiveColor=eLePhantForm.Skin.GetColor("tabsInactiveForeColor");
			
			label5.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			label5.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			label6.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			label6.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			tx_fileName.BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");
			tx_fileName.ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");

			tx_completeName.BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");
			tx_completeName.ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");

			textBoxComment.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxComment.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			labelSessionRequests.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			labelSessionRequests.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			labelSessionDownloaded.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			labelSessionDownloaded.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			labelSessionUploaded.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			labelSessionUploaded.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			labelComment.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			labelComment.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			labelSessionRequestsRes.ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");
			labelSessionRequestsRes.BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");

			labelSessionDownloadedRes.ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");
			labelSessionDownloadedRes.BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");

			labelSessionUploadedRes.ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");
			labelSessionUploadedRes.BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");

			FAviDetails.ApplySkin();

			this.Refresh();
		}

		private void tabControlDetails_SelectionChanged(object sender, System.EventArgs e)
		{
		
		}

		private void buttonOpenFolder_Click(object sender, System.EventArgs e)
		{
			if (tx_completeName.Text.Length>0)
			{
				FileInfo finfo=new FileInfo(this.tx_completeName.Text);
				Process.Start(finfo.DirectoryName);		
			}
		}

		private void buttonRename_Click(object sender, System.EventArgs e)
		{
			krnGateway.SetFileName(((InterfaceFile)sharedListView.SelectedItems[0].Tag).strHash,tx_fileName.Text);
			sharedListView.SelectedItems[0].Text=tx_fileName.Text;
		}
	}
}
