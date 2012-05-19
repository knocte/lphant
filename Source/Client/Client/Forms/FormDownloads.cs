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
using System.IO;
using eLePhant.eDonkey;
using eLePhant.Types;
using Crownwood.Magic.Controls;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for FormDownloads.
	/// </summary>
	public class FormDownloads : System.Windows.Forms.Form
	{
		private int m_selectedTabControlDetails;

		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Panel panel9;
		public downloadsListView downloadList;
		private FormAviDetails FAviDetails;
		private CkernelGateway krnGateway;
		private System.Windows.Forms.Splitter splitter1;
		private eLePhant.Interface.eLePhantButton eLePhantButton1;
		private System.Windows.Forms.ImageList imageListMaximize;
		private System.Windows.Forms.ImageList imageListMinimize;
		private System.Windows.Forms.Label label1;
		private Crownwood.Magic.Controls.TabControl tabControlFilter;
		private Crownwood.Magic.Controls.TabPage tabPageStarted;
		private Crownwood.Magic.Controls.TabPage tabPageStopped;
		private Crownwood.Magic.Controls.TabPage tabPageAll;
		private Crownwood.Magic.Controls.TabControl tabControlDetails;
		private Crownwood.Magic.Controls.TabPage tabPageSources;
		public sourcesListView sourcesList;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		public commentsListView listViewComments;
		public fileNamesListView listViewNames;
		private Crownwood.Magic.Controls.TabPage tabPageLists;
		private Crownwood.Magic.Controls.TabPage tabPageDetails;
		private System.Windows.Forms.TextBox textBoxComment;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Button buttonSetCommet;
		private System.Windows.Forms.Button buttonAddeLink;
		private System.Windows.Forms.Panel paneleLink;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxeLink;
		private System.Windows.Forms.Label label4;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button buttonRename;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button buttonPasteeLink;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TextBox tx_completeName;
		private System.Windows.Forms.TextBox tx_fileName;
		private System.Windows.Forms.Panel panel_fileDetails;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelSessionFileRateDO;
		private System.Windows.Forms.Label labelSessionFileRateUP;
		private System.Windows.Forms.Label labelSessionFileDO;
		private System.Windows.Forms.Label labelSessionFileUP;
		private System.Windows.Forms.TextBox textBoxCompletedAt;
		private System.Windows.Forms.Label labelCompletedAt;
		private System.Windows.Forms.ContextMenu contextMenuFilter;
		private System.Windows.Forms.MenuItem menuItemShowCount;
		private System.Windows.Forms.MenuItem menuItemShowTotalSizes;
		private System.Windows.Forms.MenuItem menuItemAutoSort;
		private System.Windows.Forms.Button buttonOpenFolder;
		private System.Windows.Forms.MenuItem menuItem1;
		private eLePhant.Interface.eLePhantInspector FileInspector;

		private ComboBox comboBoxCategories;
		private MenuItem manageCategories;
		private System.Windows.Forms.GroupBox groupBoxMaxSources;
		private System.Windows.Forms.RadioButton radioButtonUseGlobalMaxSources;
		private System.Windows.Forms.RadioButton radioButtonSpecificMaxSources;
		private System.Windows.Forms.TextBox textBoxMaxSources;
		private System.Windows.Forms.ImageList imageListIcons;

		private int m_DetailsSize;
		public FormDownloads()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Initialize();
			
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			try
			{
				this.tabControlFilter.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiBox;
				this.tabControlDetails.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiBox;
				this.tabControlDetails.PositionTop = true;
				eLePhantButton1_Click(this,null);
			
				m_selectedTabControlDetails = 0;
				
				this.contextMenuFilter.Popup+=new EventHandler(contextMenuFilter_Popup);
				menuItemShowCount.Click+=new EventHandler(menuItemShowCount_Click);
				menuItemShowTotalSizes.Click+=new EventHandler(menuItemShowTotalSizes_Click);
				menuItemAutoSort.Click+=new EventHandler(menuItemAutoSort_Click);
				this.downloadList.ContextMenuDownloads.MenuItems[13].Click+=new EventHandler(downloadList_DoubleClick);
				m_DetailsSize=0;

				//if view categories in a listBox
				comboBoxCategories=new ComboBox();
				this.panel7.Controls.Add(this.comboBoxCategories);

				//comboBoxCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
				comboBoxCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
				
				comboBoxCategories.DropDownStyle=ComboBoxStyle.DropDownList;
				comboBoxCategories.Size = new System.Drawing.Size(100, 21);
				//comboBoxCategories.Location = new System.Drawing.Point(buttonAddeLink.Left-5-comboBoxCategories.Width, 2);
				comboBoxCategories.Location = new System.Drawing.Point(this.label1.Right+5, 2);
				this.tabControlFilter.Left+=comboBoxCategories.Width;
				comboBoxCategories.Name = "comboBoxCategories";
				comboBoxCategories.TabIndex =4;
				comboBoxCategories.BringToFront();
				tabControlFilter.Width-=comboBoxCategories.Width+5;
				comboBoxCategories.SelectedIndexChanged += new System.EventHandler(comboBoxCategories_SelectedIndexChanged);
				comboBoxCategories.ContextMenu=contextMenuFilter;
				label1.ContextMenu=contextMenuFilter;
				//end if
				contextMenuFilter.MenuItems.Add("-");
				manageCategories=new MenuItem("Manage categories...");
				manageCategories.Click+=new EventHandler(manageCategories_Click);
				contextMenuFilter.MenuItems.Add(manageCategories);

				m_Globalize();
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}
		
		public void HideDetails()
		{
				splitter1.SplitPosition=this.Height-10;
				eLePhantButton1.ImageList=imageListMinimize;
				m_selectedTabControlDetails=tabControlDetails.SelectedIndex;
		}

		public void Connect(CkernelGateway in_krnGateway)
		{
			
			krnGateway=in_krnGateway;
			krnGateway.OnSummaryFilterChanged+=new EventHandler(downloadList_OnSummaryFilterChanged);
			krnGateway.OnCategoryListChanged+=new EventHandler(krnGateway_OnCategoryListChanged);
			downloadList.Initilize(krnGateway);
			sourcesList.Initilize(krnGateway);
			listViewNames.Initialize(krnGateway);
			listViewComments.Initialize(krnGateway);
			m_UpdateCategoryLabels();
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				//downloadList.SavePreferences();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormDownloads));
			this.panel5 = new System.Windows.Forms.Panel();
			this.paneleLink = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.textBoxeLink = new System.Windows.Forms.TextBox();
			this.downloadList = new eLePhant.Client.downloadsListView();
			this.panel7 = new System.Windows.Forms.Panel();
			this.buttonPasteeLink = new System.Windows.Forms.Button();
			this.buttonAddeLink = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControlFilter = new Crownwood.Magic.Controls.TabControl();
			this.contextMenuFilter = new System.Windows.Forms.ContextMenu();
			this.menuItemShowCount = new System.Windows.Forms.MenuItem();
			this.menuItemShowTotalSizes = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemAutoSort = new System.Windows.Forms.MenuItem();
			this.tabPageAll = new Crownwood.Magic.Controls.TabPage();
			this.tabPageStarted = new Crownwood.Magic.Controls.TabPage();
			this.tabPageStopped = new Crownwood.Magic.Controls.TabPage();
			this.panel6 = new System.Windows.Forms.Panel();
			this.eLePhantButton1 = new eLePhant.Interface.eLePhantButton(this.components);
			this.imageListMaximize = new System.Windows.Forms.ImageList(this.components);
			this.panel9 = new System.Windows.Forms.Panel();
			this.tabControlDetails = new Crownwood.Magic.Controls.TabControl();
			this.tabPageDetails = new Crownwood.Magic.Controls.TabPage();
			this.buttonOpenFolder = new System.Windows.Forms.Button();
			this.tx_fileName = new System.Windows.Forms.TextBox();
			this.tx_completeName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.panel_fileDetails = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBoxMaxSources = new System.Windows.Forms.GroupBox();
			this.textBoxMaxSources = new System.Windows.Forms.TextBox();
			this.radioButtonSpecificMaxSources = new System.Windows.Forms.RadioButton();
			this.radioButtonUseGlobalMaxSources = new System.Windows.Forms.RadioButton();
			this.textBoxCompletedAt = new System.Windows.Forms.TextBox();
			this.labelCompletedAt = new System.Windows.Forms.Label();
			this.labelSessionFileRateDO = new System.Windows.Forms.Label();
			this.labelSessionFileRateUP = new System.Windows.Forms.Label();
			this.labelSessionFileDO = new System.Windows.Forms.Label();
			this.labelSessionFileUP = new System.Windows.Forms.Label();
			this.FileInspector = new eLePhant.Interface.eLePhantInspector();
			this.imageListIcons = new System.Windows.Forms.ImageList(this.components);
			this.tabPageLists = new Crownwood.Magic.Controls.TabPage();
			this.listViewNames = new eLePhant.Client.fileNamesListView(this.components);
			this.buttonSetCommet = new System.Windows.Forms.Button();
			this.textBoxComment = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.listViewComments = new eLePhant.Client.commentsListView(this.components);
			this.buttonRename = new System.Windows.Forms.Button();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPageSources = new Crownwood.Magic.Controls.TabPage();
			this.sourcesList = new eLePhant.Client.sourcesListView(this.components);
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.imageListMinimize = new System.Windows.Forms.ImageList(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.panel5.SuspendLayout();
			this.paneleLink.SuspendLayout();
			this.panel7.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel9.SuspendLayout();
			this.tabPageDetails.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBoxMaxSources.SuspendLayout();
			this.tabPageLists.SuspendLayout();
			this.tabPageSources.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.paneleLink);
			this.panel5.Controls.Add(this.downloadList);
			this.panel5.Controls.Add(this.panel7);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel5.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.panel5.Location = new System.Drawing.Point(0, 0);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(720, 184);
			this.panel5.TabIndex = 3;
			// 
			// paneleLink
			// 
			this.paneleLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.paneleLink.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.paneleLink.Controls.Add(this.label4);
			this.paneleLink.Controls.Add(this.buttonCancel);
			this.paneleLink.Controls.Add(this.buttonAdd);
			this.paneleLink.Controls.Add(this.textBoxeLink);
			this.paneleLink.Location = new System.Drawing.Point(200, 32);
			this.paneleLink.Name = "paneleLink";
			this.paneleLink.Size = new System.Drawing.Size(512, 26);
			this.paneleLink.TabIndex = 2;
			this.paneleLink.Visible = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 4);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 16);
			this.label4.TabIndex = 5;
			this.label4.Text = "eLink:";
			// 
			// buttonCancel
			// 
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonCancel.Location = new System.Drawing.Point(443, 2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(61, 21);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonAdd.Location = new System.Drawing.Point(379, 2);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(61, 21);
			this.buttonAdd.TabIndex = 3;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.Click += new System.EventHandler(this.button2_Click);
			// 
			// textBoxeLink
			// 
			this.textBoxeLink.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxeLink.Location = new System.Drawing.Point(48, 3);
			this.textBoxeLink.Name = "textBoxeLink";
			this.textBoxeLink.Size = new System.Drawing.Size(328, 20);
			this.textBoxeLink.TabIndex = 0;
			this.textBoxeLink.Text = "";
			// 
			// downloadList
			// 
			this.downloadList.AllowColumnReorder = true;
			this.downloadList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.downloadList.AutoArrange = false;
			this.downloadList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.downloadList.DefaultCustomDraw = true;
			this.downloadList.FullRowSelect = true;
			this.downloadList.FullyCustomHeader = false;
			this.downloadList.HideSelection = false;
			this.downloadList.IncreaseHeaderHeight = 0;
			this.downloadList.Location = new System.Drawing.Point(8, 32);
			this.downloadList.Name = "downloadList";
			this.downloadList.Size = new System.Drawing.Size(704, 152);
			this.downloadList.sortColumn = -1;
			this.downloadList.TabIndex = 1;
			this.downloadList.View = System.Windows.Forms.View.Details;
			this.downloadList.DoubleClick += new System.EventHandler(this.downloadList_DoubleClick);
			this.downloadList.SelectedIndexChanged += new System.EventHandler(this.downloadList_SelectedIndexChanged);
			// 
			// panel7
			// 
			this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel7.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel7.Controls.Add(this.buttonPasteeLink);
			this.panel7.Controls.Add(this.buttonAddeLink);
			this.panel7.Controls.Add(this.label1);
			this.panel7.Controls.Add(this.tabControlFilter);
			this.panel7.Location = new System.Drawing.Point(8, 6);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(704, 27);
			this.panel7.TabIndex = 0;
			// 
			// buttonPasteeLink
			// 
			this.buttonPasteeLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPasteeLink.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonPasteeLink.Location = new System.Drawing.Point(616, 2);
			this.buttonPasteeLink.Name = "buttonPasteeLink";
			this.buttonPasteeLink.Size = new System.Drawing.Size(80, 21);
			this.buttonPasteeLink.TabIndex = 3;
			this.buttonPasteeLink.Text = "Paste eLink";
			this.buttonPasteeLink.Click += new System.EventHandler(this.buttonPasteeLink_Click);
			// 
			// buttonAddeLink
			// 
			this.buttonAddeLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddeLink.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonAddeLink.Location = new System.Drawing.Point(534, 2);
			this.buttonAddeLink.Name = "buttonAddeLink";
			this.buttonAddeLink.Size = new System.Drawing.Size(80, 21);
			this.buttonAddeLink.TabIndex = 2;
			this.buttonAddeLink.Text = "Add eLink";
			this.buttonAddeLink.Click += new System.EventHandler(this.buttonAddeLink_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Filter:";
			// 
			// tabControlFilter
			// 
			this.tabControlFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlFilter.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tabControlFilter.ButtonActiveColor = System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(0)));
			this.tabControlFilter.ButtonInactiveColor = System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(0)));
			this.tabControlFilter.ContextMenu = this.contextMenuFilter;
			this.tabControlFilter.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tabControlFilter.HideTabsMode = Crownwood.Magic.Controls.TabControl.HideTabsModes.ShowAlways;
			this.tabControlFilter.HotTextColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tabControlFilter.Location = new System.Drawing.Point(56, 1);
			this.tabControlFilter.Name = "tabControlFilter";
			this.tabControlFilter.SelectedIndex = 0;
			this.tabControlFilter.SelectedTab = this.tabPageAll;
			this.tabControlFilter.Size = new System.Drawing.Size(472, 26);
			this.tabControlFilter.TabIndex = 0;
			this.tabControlFilter.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																							   this.tabPageAll,
																							   this.tabPageStarted,
																							   this.tabPageStopped});
			this.tabControlFilter.TextColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tabControlFilter.SelectionChanged += new System.EventHandler(this.tabControlFilter_SelectionChanged);
			// 
			// contextMenuFilter
			// 
			this.contextMenuFilter.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							  this.menuItemShowCount,
																							  this.menuItemShowTotalSizes,
																							  this.menuItem1,
																							  this.menuItemAutoSort});
			// 
			// menuItemShowCount
			// 
			this.menuItemShowCount.Checked = true;
			this.menuItemShowCount.Index = 0;
			this.menuItemShowCount.Text = "Show count";
			// 
			// menuItemShowTotalSizes
			// 
			this.menuItemShowTotalSizes.Checked = true;
			this.menuItemShowTotalSizes.Index = 1;
			this.menuItemShowTotalSizes.Text = "Show total sizes";
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// menuItemAutoSort
			// 
			this.menuItemAutoSort.Index = 3;
			this.menuItemAutoSort.Text = "";
			// 
			// tabPageAll
			// 
			this.tabPageAll.Location = new System.Drawing.Point(0, 0);
			this.tabPageAll.Name = "tabPageAll";
			this.tabPageAll.Size = new System.Drawing.Size(472, 1);
			this.tabPageAll.TabIndex = 2;
			this.tabPageAll.Title = "All";
			// 
			// tabPageStarted
			// 
			this.tabPageStarted.Location = new System.Drawing.Point(0, 0);
			this.tabPageStarted.Name = "tabPageStarted";
			this.tabPageStarted.Selected = false;
			this.tabPageStarted.Size = new System.Drawing.Size(472, 1);
			this.tabPageStarted.TabIndex = 0;
			this.tabPageStarted.Title = "Started";
			// 
			// tabPageStopped
			// 
			this.tabPageStopped.Location = new System.Drawing.Point(0, 0);
			this.tabPageStopped.Name = "tabPageStopped";
			this.tabPageStopped.Selected = false;
			this.tabPageStopped.Size = new System.Drawing.Size(472, 1);
			this.tabPageStopped.TabIndex = 1;
			this.tabPageStopped.Title = "Stopped";
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.eLePhantButton1);
			this.panel6.Controls.Add(this.panel9);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel6.Location = new System.Drawing.Point(0, 192);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(720, 248);
			this.panel6.TabIndex = 5;
			// 
			// eLePhantButton1
			// 
			this.eLePhantButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.eLePhantButton1.BackColor = System.Drawing.Color.Transparent;
			this.eLePhantButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.eLePhantButton1.ImageIndex = 0;
			this.eLePhantButton1.ImageList = this.imageListMaximize;
			this.eLePhantButton1.Location = new System.Drawing.Point(672, -5);
			this.eLePhantButton1.Name = "eLePhantButton1";
			this.eLePhantButton1.Size = new System.Drawing.Size(34, 13);
			this.eLePhantButton1.TabIndex = 7;
			this.eLePhantButton1.Click += new System.EventHandler(this.eLePhantButton1_Click);
			// 
			// imageListMaximize
			// 
			this.imageListMaximize.ImageSize = new System.Drawing.Size(34, 9);
			this.imageListMaximize.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMaximize.ImageStream")));
			this.imageListMaximize.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// panel9
			// 
			this.panel9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel9.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel9.Controls.Add(this.tabControlDetails);
			this.panel9.Location = new System.Drawing.Point(8, 8);
			this.panel9.Name = "panel9";
			this.panel9.Size = new System.Drawing.Size(704, 232);
			this.panel9.TabIndex = 2;
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
			this.tabControlDetails.Size = new System.Drawing.Size(702, 230);
			this.tabControlDetails.TabIndex = 0;
			this.tabControlDetails.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																								this.tabPageDetails,
																								this.tabPageLists,
																								this.tabPageSources});
			// 
			// tabPageDetails
			// 
			this.tabPageDetails.Controls.Add(this.buttonOpenFolder);
			this.tabPageDetails.Controls.Add(this.tx_fileName);
			this.tabPageDetails.Controls.Add(this.tx_completeName);
			this.tabPageDetails.Controls.Add(this.label6);
			this.tabPageDetails.Controls.Add(this.label5);
			this.tabPageDetails.Controls.Add(this.panel_fileDetails);
			this.tabPageDetails.Controls.Add(this.panel1);
			this.tabPageDetails.ImageIndex = 0;
			this.tabPageDetails.ImageList = this.imageListIcons;
			this.tabPageDetails.Location = new System.Drawing.Point(0, 0);
			this.tabPageDetails.Name = "tabPageDetails";
			this.tabPageDetails.Size = new System.Drawing.Size(702, 205);
			this.tabPageDetails.TabIndex = 2;
			this.tabPageDetails.Title = "Details";
			// 
			// buttonOpenFolder
			// 
			this.buttonOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonOpenFolder.Image = ((System.Drawing.Image)(resources.GetObject("buttonOpenFolder.Image")));
			this.buttonOpenFolder.Location = new System.Drawing.Point(356, 20);
			this.buttonOpenFolder.Name = "buttonOpenFolder";
			this.buttonOpenFolder.Size = new System.Drawing.Size(28, 19);
			this.buttonOpenFolder.TabIndex = 27;
			this.toolTip1.SetToolTip(this.buttonOpenFolder, "Open folder");
			this.buttonOpenFolder.Click += new System.EventHandler(this.buttonOpenFolder_Click);
			// 
			// tx_fileName
			// 
			this.tx_fileName.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_fileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_fileName.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tx_fileName.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_fileName.Location = new System.Drawing.Point(85, 2);
			this.tx_fileName.Name = "tx_fileName";
			this.tx_fileName.ReadOnly = true;
			this.tx_fileName.Size = new System.Drawing.Size(299, 19);
			this.tx_fileName.TabIndex = 25;
			this.tx_fileName.Text = "";
			// 
			// tx_completeName
			// 
			this.tx_completeName.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_completeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_completeName.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tx_completeName.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_completeName.Location = new System.Drawing.Point(85, 20);
			this.tx_completeName.Name = "tx_completeName";
			this.tx_completeName.ReadOnly = true;
			this.tx_completeName.Size = new System.Drawing.Size(273, 19);
			this.tx_completeName.TabIndex = 24;
			this.tx_completeName.Text = "";
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label6.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.ForeColor = System.Drawing.Color.Black;
			this.label6.Location = new System.Drawing.Point(5, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(81, 19);
			this.label6.TabIndex = 3;
			this.label6.Text = "File:";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.Black;
			this.label5.Location = new System.Drawing.Point(5, 2);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(81, 19);
			this.label5.TabIndex = 2;
			this.label5.Text = "File Name:";
			// 
			// panel_fileDetails
			// 
			this.panel_fileDetails.AutoScroll = true;
			this.panel_fileDetails.Font = new System.Drawing.Font("Tahoma", 7F);
			this.panel_fileDetails.Location = new System.Drawing.Point(5, 38);
			this.panel_fileDetails.Name = "panel_fileDetails";
			this.panel_fileDetails.Size = new System.Drawing.Size(379, 161);
			this.panel_fileDetails.TabIndex = 23;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.groupBoxMaxSources);
			this.panel1.Controls.Add(this.textBoxCompletedAt);
			this.panel1.Controls.Add(this.labelCompletedAt);
			this.panel1.Controls.Add(this.labelSessionFileRateDO);
			this.panel1.Controls.Add(this.labelSessionFileRateUP);
			this.panel1.Controls.Add(this.labelSessionFileDO);
			this.panel1.Controls.Add(this.labelSessionFileUP);
			this.panel1.Controls.Add(this.FileInspector);
			this.panel1.Location = new System.Drawing.Point(390, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(307, 197);
			this.panel1.TabIndex = 26;
			// 
			// groupBoxMaxSources
			// 
			this.groupBoxMaxSources.Controls.Add(this.textBoxMaxSources);
			this.groupBoxMaxSources.Controls.Add(this.radioButtonSpecificMaxSources);
			this.groupBoxMaxSources.Controls.Add(this.radioButtonUseGlobalMaxSources);
			this.groupBoxMaxSources.Location = new System.Drawing.Point(5, 106);
			this.groupBoxMaxSources.Name = "groupBoxMaxSources";
			this.groupBoxMaxSources.Size = new System.Drawing.Size(290, 69);
			this.groupBoxMaxSources.TabIndex = 28;
			this.groupBoxMaxSources.TabStop = false;
			this.groupBoxMaxSources.Text = "Maximum sources limit";
			// 
			// textBoxMaxSources
			// 
			this.textBoxMaxSources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxMaxSources.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxMaxSources.Location = new System.Drawing.Point(231, 39);
			this.textBoxMaxSources.Name = "textBoxMaxSources";
			this.textBoxMaxSources.Size = new System.Drawing.Size(54, 21);
			this.textBoxMaxSources.TabIndex = 7;
			this.textBoxMaxSources.Text = "";
			this.textBoxMaxSources.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMaxSources_KeyPress);
			this.textBoxMaxSources.TextChanged += new System.EventHandler(this.textBoxMaxSources_TextChanged);
			// 
			// radioButtonSpecificMaxSources
			// 
			this.radioButtonSpecificMaxSources.Location = new System.Drawing.Point(12, 45);
			this.radioButtonSpecificMaxSources.Name = "radioButtonSpecificMaxSources";
			this.radioButtonSpecificMaxSources.Size = new System.Drawing.Size(206, 15);
			this.radioButtonSpecificMaxSources.TabIndex = 1;
			this.radioButtonSpecificMaxSources.Text = "Specific value for this file:";
			this.radioButtonSpecificMaxSources.CheckedChanged += new System.EventHandler(this.radioButtonSpecificMaxSources_CheckedChanged);
			// 
			// radioButtonUseGlobalMaxSources
			// 
			this.radioButtonUseGlobalMaxSources.Location = new System.Drawing.Point(11, 21);
			this.radioButtonUseGlobalMaxSources.Name = "radioButtonUseGlobalMaxSources";
			this.radioButtonUseGlobalMaxSources.Size = new System.Drawing.Size(257, 15);
			this.radioButtonUseGlobalMaxSources.TabIndex = 0;
			this.radioButtonUseGlobalMaxSources.Text = "Use global preferences value";
			this.radioButtonUseGlobalMaxSources.CheckedChanged += new System.EventHandler(this.radioButtonUseGlobalMaxSources_CheckedChanged);
			// 
			// textBoxCompletedAt
			// 
			this.textBoxCompletedAt.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.textBoxCompletedAt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxCompletedAt.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.textBoxCompletedAt.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.textBoxCompletedAt.Location = new System.Drawing.Point(139, 84);
			this.textBoxCompletedAt.Name = "textBoxCompletedAt";
			this.textBoxCompletedAt.ReadOnly = true;
			this.textBoxCompletedAt.Size = new System.Drawing.Size(154, 19);
			this.textBoxCompletedAt.TabIndex = 27;
			this.textBoxCompletedAt.Text = "";
			// 
			// labelCompletedAt
			// 
			this.labelCompletedAt.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelCompletedAt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelCompletedAt.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelCompletedAt.ForeColor = System.Drawing.Color.Black;
			this.labelCompletedAt.Location = new System.Drawing.Point(6, 84);
			this.labelCompletedAt.Name = "labelCompletedAt";
			this.labelCompletedAt.Size = new System.Drawing.Size(134, 19);
			this.labelCompletedAt.TabIndex = 26;
			this.labelCompletedAt.Text = "Completed at:";
			// 
			// labelSessionFileRateDO
			// 
			this.labelSessionFileRateDO.AutoSize = true;
			this.labelSessionFileRateDO.BackColor = System.Drawing.Color.Transparent;
			this.labelSessionFileRateDO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labelSessionFileRateDO.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelSessionFileRateDO.ForeColor = System.Drawing.Color.Green;
			this.labelSessionFileRateDO.Location = new System.Drawing.Point(140, 48);
			this.labelSessionFileRateDO.Name = "labelSessionFileRateDO";
			this.labelSessionFileRateDO.Size = new System.Drawing.Size(98, 14);
			this.labelSessionFileRateDO.TabIndex = 12;
			this.labelSessionFileRateDO.Text = "Download rate: 00.0 B";
			// 
			// labelSessionFileRateUP
			// 
			this.labelSessionFileRateUP.AutoSize = true;
			this.labelSessionFileRateUP.BackColor = System.Drawing.Color.Transparent;
			this.labelSessionFileRateUP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labelSessionFileRateUP.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelSessionFileRateUP.ForeColor = System.Drawing.Color.Red;
			this.labelSessionFileRateUP.Location = new System.Drawing.Point(140, 66);
			this.labelSessionFileRateUP.Name = "labelSessionFileRateUP";
			this.labelSessionFileRateUP.Size = new System.Drawing.Size(86, 14);
			this.labelSessionFileRateUP.TabIndex = 11;
			this.labelSessionFileRateUP.Text = "Upload rate: 00.0 B";
			// 
			// labelSessionFileDO
			// 
			this.labelSessionFileDO.AutoSize = true;
			this.labelSessionFileDO.BackColor = System.Drawing.Color.Transparent;
			this.labelSessionFileDO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labelSessionFileDO.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelSessionFileDO.ForeColor = System.Drawing.Color.Green;
			this.labelSessionFileDO.Location = new System.Drawing.Point(5, 49);
			this.labelSessionFileDO.Name = "labelSessionFileDO";
			this.labelSessionFileDO.Size = new System.Drawing.Size(90, 14);
			this.labelSessionFileDO.TabIndex = 10;
			this.labelSessionFileDO.Text = "Downloaded: 00.0 B";
			// 
			// labelSessionFileUP
			// 
			this.labelSessionFileUP.AutoSize = true;
			this.labelSessionFileUP.BackColor = System.Drawing.Color.Transparent;
			this.labelSessionFileUP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labelSessionFileUP.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelSessionFileUP.ForeColor = System.Drawing.Color.Red;
			this.labelSessionFileUP.Location = new System.Drawing.Point(5, 65);
			this.labelSessionFileUP.Name = "labelSessionFileUP";
			this.labelSessionFileUP.Size = new System.Drawing.Size(78, 14);
			this.labelSessionFileUP.TabIndex = 9;
			this.labelSessionFileUP.Text = "Uploaded: 00.0 B";
			// 
			// FileInspector
			// 
			this.FileInspector.BackColor = System.Drawing.Color.Gray;
			// TODO: Code generation for 'this.FileInspector.BarHeight' failed because of Exception 'Invalid Primitive Type: System.UInt16. Only CLS compliant primitive types can be used. Consider using CodeObjectCreateExpression.'.
			// TODO: Code generation for 'this.FileInspector.BarSpacing' failed because of Exception 'Invalid Primitive Type: System.UInt16. Only CLS compliant primitive types can be used. Consider using CodeObjectCreateExpression.'.
			this.FileInspector.File = null;
			this.FileInspector.ForeColor = System.Drawing.Color.Black;
			this.FileInspector.Location = new System.Drawing.Point(3, 3);
			this.FileInspector.Name = "FileInspector";
			this.FileInspector.Size = new System.Drawing.Size(286, 39);
			this.FileInspector.TabIndex = 0;
			// 
			// imageListIcons
			// 
			this.imageListIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcons.ImageStream")));
			this.imageListIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tabPageLists
			// 
			this.tabPageLists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabPageLists.Controls.Add(this.listViewNames);
			this.tabPageLists.Controls.Add(this.buttonSetCommet);
			this.tabPageLists.Controls.Add(this.textBoxComment);
			this.tabPageLists.Controls.Add(this.label3);
			this.tabPageLists.Controls.Add(this.listViewComments);
			this.tabPageLists.Controls.Add(this.buttonRename);
			this.tabPageLists.Controls.Add(this.textBoxName);
			this.tabPageLists.Controls.Add(this.label2);
			this.tabPageLists.ImageIndex = 1;
			this.tabPageLists.ImageList = this.imageListIcons;
			this.tabPageLists.Location = new System.Drawing.Point(0, 0);
			this.tabPageLists.Name = "tabPageLists";
			this.tabPageLists.Selected = false;
			this.tabPageLists.Size = new System.Drawing.Size(702, 205);
			this.tabPageLists.TabIndex = 0;
			this.tabPageLists.Title = "Comments";
			// 
			// listViewNames
			// 
			this.listViewNames.AllowColumnReorder = true;
			this.listViewNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewNames.AutoArrange = false;
			this.listViewNames.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listViewNames.DefaultCustomDraw = true;
			this.listViewNames.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.listViewNames.FullRowSelect = true;
			this.listViewNames.FullyCustomHeader = false;
			this.listViewNames.HideSelection = false;
			this.listViewNames.IncreaseHeaderHeight = 0;
			this.listViewNames.Location = new System.Drawing.Point(341, 20);
			this.listViewNames.Name = "listViewNames";
			this.listViewNames.Size = new System.Drawing.Size(353, 151);
			this.listViewNames.sortColumn = -1;
			this.listViewNames.TabIndex = 11;
			this.listViewNames.View = System.Windows.Forms.View.Details;
			this.listViewNames.SelectedIndexChanged += new System.EventHandler(this.listViewNames_SelectedIndexChanged);
			// 
			// buttonSetCommet
			// 
			this.buttonSetCommet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSetCommet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonSetCommet.Location = new System.Drawing.Point(203, 176);
			this.buttonSetCommet.Name = "buttonSetCommet";
			this.buttonSetCommet.Size = new System.Drawing.Size(135, 22);
			this.buttonSetCommet.TabIndex = 7;
			this.buttonSetCommet.Text = "Set my comment";
			this.buttonSetCommet.Click += new System.EventHandler(this.buttonSetCommet_Click);
			// 
			// textBoxComment
			// 
			this.textBoxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxComment.Location = new System.Drawing.Point(7, 176);
			this.textBoxComment.Name = "textBoxComment";
			this.textBoxComment.Size = new System.Drawing.Size(194, 21);
			this.textBoxComment.TabIndex = 6;
			this.textBoxComment.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(235, 15);
			this.label3.TabIndex = 5;
			this.label3.Text = "File comments:";
			// 
			// listViewComments
			// 
			this.listViewComments.AllowColumnReorder = true;
			this.listViewComments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewComments.AutoArrange = false;
			this.listViewComments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listViewComments.DefaultCustomDraw = true;
			this.listViewComments.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.listViewComments.FullRowSelect = true;
			this.listViewComments.FullyCustomHeader = false;
			this.listViewComments.HideSelection = false;
			this.listViewComments.IncreaseHeaderHeight = 0;
			this.listViewComments.Location = new System.Drawing.Point(6, 20);
			this.listViewComments.Name = "listViewComments";
			this.listViewComments.Size = new System.Drawing.Size(333, 150);
			this.listViewComments.sortColumn = -1;
			this.listViewComments.TabIndex = 4;
			this.listViewComments.View = System.Windows.Forms.View.Details;
			// 
			// buttonRename
			// 
			this.buttonRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRename.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonRename.Location = new System.Drawing.Point(610, 174);
			this.buttonRename.Name = "buttonRename";
			this.buttonRename.Size = new System.Drawing.Size(84, 23);
			this.buttonRename.TabIndex = 3;
			this.buttonRename.Text = "Rename";
			this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
			// 
			// textBoxName
			// 
			this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.Location = new System.Drawing.Point(341, 175);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(267, 21);
			this.textBoxName.TabIndex = 2;
			this.textBoxName.Text = "";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(342, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(234, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "File names:";
			// 
			// tabPageSources
			// 
			this.tabPageSources.Controls.Add(this.sourcesList);
			this.tabPageSources.ImageIndex = 2;
			this.tabPageSources.ImageList = this.imageListIcons;
			this.tabPageSources.Location = new System.Drawing.Point(0, 0);
			this.tabPageSources.Name = "tabPageSources";
			this.tabPageSources.Selected = false;
			this.tabPageSources.Size = new System.Drawing.Size(702, 205);
			this.tabPageSources.TabIndex = 1;
			this.tabPageSources.Title = "Sources";
			// 
			// sourcesList
			// 
			this.sourcesList.AllowColumnReorder = true;
			this.sourcesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.sourcesList.AutoArrange = false;
			this.sourcesList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.sourcesList.DefaultCustomDraw = true;
			this.sourcesList.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.sourcesList.FullRowSelect = true;
			this.sourcesList.FullyCustomHeader = false;
			this.sourcesList.HideSelection = false;
			this.sourcesList.IncreaseHeaderHeight = 0;
			this.sourcesList.Location = new System.Drawing.Point(6, 7);
			this.sourcesList.Name = "sourcesList";
			this.sourcesList.Size = new System.Drawing.Size(691, 191);
			this.sourcesList.sortColumn = -1;
			this.sourcesList.TabIndex = 5;
			this.sourcesList.View = System.Windows.Forms.View.Details;
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 184);
			this.splitter1.MinExtra = 10;
			this.splitter1.MinSize = 10;
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(720, 8);
			this.splitter1.TabIndex = 4;
			this.splitter1.TabStop = false;
			this.splitter1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter1_SplitterMoved);
			// 
			// imageListMinimize
			// 
			this.imageListMinimize.ImageSize = new System.Drawing.Size(34, 9);
			this.imageListMinimize.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMinimize.ImageStream")));
			this.imageListMinimize.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// FormDownloads
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.ClientSize = new System.Drawing.Size(720, 440);
			this.Controls.Add(this.panel6);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel5);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormDownloads";
			this.Text = "FormDownloads";
			this.SizeChanged += new System.EventHandler(this.FormDownloads_SizeChanged);
			this.panel5.ResumeLayout(false);
			this.paneleLink.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.panel9.ResumeLayout(false);
			this.tabPageDetails.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.groupBoxMaxSources.ResumeLayout(false);
			this.tabPageLists.ResumeLayout(false);
			this.tabPageSources.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void Initialize()
		{
			FAviDetails=new FormAviDetails();
			FAviDetails.TopLevel=false;
			FAviDetails.Dock = DockStyle.Fill;
			panel_fileDetails.Controls.Add(FAviDetails);
			FAviDetails.Dock = DockStyle.Fill;
			FAviDetails.ApplySkin();
		}

		private void UpdateFileInfo(string strHash)
		{
			FileStatistics fileStats=krnGateway.GetFileStatistics(strHash);
			labelSessionFileDO.Text=eLePhantForm.Globalization["LBL_DOWNLOADED"]+":"+eLePhantListView.SizeToString(fileStats.SessionDownload);
			labelSessionFileUP.Text=eLePhantForm.Globalization["LBL_UPLOADED"]+":"+eLePhantListView.SizeToString(fileStats.SessionUpload);
			labelSessionFileRateDO.Text=eLePhantForm.Globalization["LBL_AVGDOWNLOAD"]+":"+eLePhantListView.SpeedToString(fileStats.AvgDownSpeed);
			labelSessionFileRateUP.Text=eLePhantForm.Globalization["LBL_AVGUPLOAD"]+":"+eLePhantListView.SpeedToString(fileStats.AvgUpSpeed);
			if (fileStats.CompletedTime!=DateTime.MinValue)
				textBoxCompletedAt.Text=fileStats.CompletedTime.ToString();
			else
				textBoxCompletedAt.Text=" -- ";
		}
		private void downloadList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (downloadList.SelectedItems.Count>0)
			{	
				InterfaceFile file=(InterfaceFile)downloadList.SelectedItems[0].Tag;
                downloadList.MenuInfo.SearchString=downloadList.SelectedItems[0].Text;
				downloadList.MenuInfo.FileHash=file.strHash;
				downloadList.MenuInfo.eD2kLink="ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strHash+"|/";
				sourcesList.ReloadList(file.strHash);
				listViewNames.ReloadList(file.strHash);
				if (listViewComments.ReloadList(file.strHash))
					tabPageLists.ImageIndex=3;
				else
					tabPageLists.ImageIndex=1;

				byte rating=0;
				string comentario="";
				krnGateway.GetFileComment(file.strHash,ref comentario,ref rating);
				textBoxComment.Text=comentario;
				FileInspector.File=file;
				FileInspector.Refresh();
				showInfo(file);
				UpdateFileInfo(file.strHash);
				if (file.MaxSources==0)
				{
					radioButtonUseGlobalMaxSources.Checked=true;
					textBoxMaxSources.Enabled=false;
					textBoxMaxSources.Text="";
				}
				else
				{
					radioButtonSpecificMaxSources.Checked=true;
					textBoxMaxSources.Enabled=true;
					textBoxMaxSources.Text=file.MaxSources.ToString();
				}
			}
			else
				downloadList.MenuInfo.SearchString="";
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

		private void eLePhantButton1_Click(object sender, System.EventArgs e)
		{
			if (splitter1.SplitPosition<=this.Height-20)
			{
				splitter1.SplitPosition=this.Height-10;
				eLePhantButton1.ImageList=imageListMinimize;
				m_selectedTabControlDetails=tabControlDetails.SelectedIndex;
			}
			else
			{
				tabControlDetails.SelectedIndex=m_selectedTabControlDetails;
				splitter1.SplitPosition=this.Height/2-10;
				eLePhantButton1.ImageList=imageListMaximize;
			}
			if (downloadList.SelectedItems.Count>0)
			{
				int selpos=0;
				int maxscroll=100;
				do
				{
					selpos=downloadList.SelectedItems[0].Bounds.Bottom;
					if (selpos+20>downloadList.Height) Win32.SendMessage(downloadList.Handle,Win32.WM_VSCROLL, (IntPtr)Win32.SB_LINEDOWN,(IntPtr)0);
					maxscroll--; //security condition for avioding infinite loops is sendmessage does not work
				}
				while ((selpos>downloadList.Height)&&(maxscroll>0));
			}
		}

		private void splitter1_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e)
		{
			if (splitter1.SplitPosition<=this.Height-20)
			{
				eLePhantButton1.ImageList=imageListMaximize;
				m_DetailsSize=this.Height-splitter1.SplitPosition;
			}
			else
			{
				eLePhantButton1.ImageList=imageListMinimize;
			}		
		}

		private void FormDownloads_SizeChanged(object sender, System.EventArgs e)
		{
			if (this.Height==0) return;
			if ((splitter1.SplitPosition>this.Height-20)||(eLePhantButton1.ImageList==imageListMinimize))
			{
				splitter1.SplitPosition=this.Height-18;
				eLePhantButton1.ImageList=imageListMinimize;
			}
			else 
			{
				splitter1.SplitPosition=this.Height-m_DetailsSize;
				eLePhantButton1.ImageList=imageListMaximize;
			}
		}

		private void tabControlFilter_SelectionChanged(object sender, System.EventArgs e)
		{
			downloadList.SetFilter((byte)tabControlFilter.SelectedIndex);
		}

		private void buttonSetCommet_Click(object sender, System.EventArgs e)
		{
			if (downloadList.SelectedItems.Count>0)
			{
				krnGateway.SetFileComment(((InterfaceFile)downloadList.SelectedItems[0].Tag).strHash,textBoxComment.Text,0);
			}
		}

		private void buttonAddeLink_Click(object sender, System.EventArgs e)
		{
			paneleLink.Visible=true;
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			krnGateway.DownloadElink(this.textBoxeLink.Text,false);
			this.textBoxeLink.Text = null;
			paneleLink.Visible=false;
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.textBoxeLink.Text = null;
			paneleLink.Visible=false;
		}

		private void buttonRename_Click(object sender, System.EventArgs e)
		{
			if ((downloadList.SelectedItems.Count>0)&&(textBoxName.Text.Length>0))
			{
				krnGateway.SetFileName(((InterfaceFile)downloadList.SelectedItems[0].Tag).strHash,textBoxName.Text);
			}		
		}

		private void listViewNames_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listViewNames.SelectedItems.Count>0)
			{
				textBoxName.Text=listViewNames.SelectedItems[0].Text;
			}
		}

		private void buttonPasteeLink_Click(object sender, System.EventArgs e)
		{
			string elink="";
			IDataObject iData = Clipboard.GetDataObject();
			if(iData.GetDataPresent(DataFormats.Text)) 
			{
				elink=(String)iData.GetData(DataFormats.Text);
				if (elink!="")
					krnGateway.DownloadElink(elink,false);
			}
		}

		private void downloadList_DoubleClick(object sender, System.EventArgs e)
		{
			eLePhantButton1_Click(this,null);

		}

		private string SizeMBtoGB(ulong size)
		{
			if (size>102)
				return ((float)size/1024F).ToString("####0.#")+" "+eLePhantForm.Globalization["LBL_GBYTE"];
			else
				return size.ToString()+" "+eLePhantForm.Globalization["LBL_MBYTE"];
		}
		
		private void m_UpdateCategoryLabels()
		{
			//if in listBox
			comboBoxCategories.Items.Clear();
			//fixed labels
			comboBoxCategories.Items.Add(eLePhantForm.Globalization["LBL_ALL"]);
			comboBoxCategories.Items.Add(eLePhantForm.Globalization["LBL_VIDEO"]);
			comboBoxCategories.Items.Add(eLePhantForm.Globalization["LBL_FILE"]);
			comboBoxCategories.Items.Add(eLePhantForm.Globalization["LBL_AUDIO"]);

			//category labels
			ArrayList categories=krnGateway.GetCategories();
	
			foreach (CCategory category in categories)
			{
				comboBoxCategories.Items.Add(category.Name);
			}

			comboBoxCategories.SelectedIndex=0;
			//end if
		}

		private void comboBoxCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (comboBoxCategories.SelectedIndex==0) //all
			{
				krnGateway.CurrentCategoryFilter="";
				downloadList.SetCategoryFilter("");
			}
			else if (comboBoxCategories.SelectedIndex==1) //Video
			{
				krnGateway.CurrentCategoryFilter="LBL_VIDEO";
				downloadList.SetCategoryFilter("LBL_VIDEO");
			}
			else if (comboBoxCategories.SelectedIndex==2) //File
			{
				krnGateway.CurrentCategoryFilter="LBL_FILE";
				downloadList.SetCategoryFilter("LBL_FILE");
			}
			else if (comboBoxCategories.SelectedIndex==3) //Audio
			{
				krnGateway.CurrentCategoryFilter="LBL_AUDIO";
				downloadList.SetCategoryFilter("LBL_AUDIO");
			}
			else
			{
				krnGateway.CurrentCategoryFilter=comboBoxCategories.Items[comboBoxCategories.SelectedIndex].ToString();
				downloadList.SetCategoryFilter(comboBoxCategories.Items[comboBoxCategories.SelectedIndex].ToString());
			}
		}

		private void m_UpdateFilterLabels()
		{
			tabPageAll.Title = eLePhantForm.Globalization["LBL_ALL"];
			if (comboBoxCategories.SelectedIndex>0)
				tabPageAll.Title +="-"+comboBoxCategories.Items[comboBoxCategories.SelectedIndex].ToString();
			if (eLePhantForm.preferences.GetBool("FilterShowCount"))
				tabPageAll.Title +=" ("+((CFilterSummary)krnGateway.FilterSummary["All"]).Items+")";
			if (eLePhantForm.preferences.GetBool("FilterShowTotalSizes"))
				tabPageAll.Title +=" "+SizeMBtoGB(((CFilterSummary)krnGateway.FilterSummary["All"]).TotalSize);

			tabPageStarted.Title = eLePhantForm.Globalization["LBL_STARTED"];
			if (comboBoxCategories.SelectedIndex>0)
				tabPageStarted.Title +="-"+comboBoxCategories.Items[comboBoxCategories.SelectedIndex].ToString();
			if (eLePhantForm.preferences.GetBool("FilterShowCount"))
				tabPageStarted.Title +=" ("+((CFilterSummary)krnGateway.FilterSummary["Started"]).Items+") ";
			if (eLePhantForm.preferences.GetBool("FilterShowTotalSizes"))
				tabPageStarted.Title +=" "+SizeMBtoGB(((CFilterSummary)krnGateway.FilterSummary["Started"]).TotalSize);
			
			tabPageStopped.Title = eLePhantForm.Globalization["LBL_STOPPED"];
			if (comboBoxCategories.SelectedIndex>0)
				tabPageStopped.Title +="-"+comboBoxCategories.Items[comboBoxCategories.SelectedIndex].ToString();
			if (eLePhantForm.preferences.GetBool("FilterShowCount"))
				tabPageStopped.Title +=" ("+((CFilterSummary)krnGateway.FilterSummary["Stopped"]).Items+") ";
			if (eLePhantForm.preferences.GetBool("FilterShowTotalSizes"))
				tabPageStopped.Title +=" "+SizeMBtoGB(((CFilterSummary)krnGateway.FilterSummary["Stopped"]).TotalSize);
		}

		private void m_Globalize()
		{
			toolTip1.SetToolTip(eLePhantButton1,eLePhantForm.Globalization["LBL_SHOWHIDEDETAILS"]);
			buttonPasteeLink.Text = eLePhantForm.Globalization["LBL_PASTELINK"];
			buttonAddeLink.Text = eLePhantForm.Globalization["LBL_ADDLINK"];
			label1.Text = eLePhantForm.Globalization["LBL_FILTER"];
			tabPageAll.Title = eLePhantForm.Globalization["LBL_ALL"];
			tabPageStarted.Title = eLePhantForm.Globalization["LBL_STARTED"];
			tabPageStopped.Title = eLePhantForm.Globalization["LBL_STOPPED"];
			tabPageDetails.Title = eLePhantForm.Globalization["LBL_DETAILS"];
			tabPageLists.Title = eLePhantForm.Globalization["LBL_COMMENTS"];
			buttonSetCommet.Text = eLePhantForm.Globalization["LBL_SETCOMMENT"];
			label3.Text = eLePhantForm.Globalization["LBL_FILCOMMENTS"];
			buttonRename.Text = eLePhantForm.Globalization["LBL_RENAME"];
			label2.Text = eLePhantForm.Globalization["LBL_FILENAMES"];
			tabPageSources.Title = eLePhantForm.Globalization["LBL_SOURCES"];
			buttonAdd.Text = eLePhantForm.Globalization["LBL_ADD"];
			label6.Text = eLePhantForm.Globalization["LBL_PATH"]+":";
			label5.Text = eLePhantForm.Globalization["LBL_FILE"]+":";
			buttonCancel.Text = eLePhantForm.Globalization["LBL_CANCEL"];
			labelCompletedAt.Text = eLePhantForm.Globalization["LBL_COMPLETEDTIME"]+":";
			menuItemShowTotalSizes.Text=eLePhantForm.Globalization["LBL_FILTERSHOWSIZES"];
			menuItemShowCount.Text=eLePhantForm.Globalization["LBL_FILTERSHOWCOUNT"];
			menuItemAutoSort.Text=eLePhantForm.Globalization["LBL_AUTOSORT"];
			toolTip1.SetToolTip(this.buttonOpenFolder, eLePhantForm.Globalization["LBL_OPENFOLDER"]);
			manageCategories.Text=eLePhantForm.Globalization["LBL_MANAGECATEGORIES"];
			groupBoxMaxSources.Text=eLePhantForm.Globalization["LBL_MAXSOURCESFILE"];
			radioButtonUseGlobalMaxSources.Text=eLePhantForm.Globalization["LBL_USEGLOBALMAXSOURCES"];
			radioButtonSpecificMaxSources.Text=eLePhantForm.Globalization["LBL_SPECIFYVALUE"];
		}

		public void Globalize()
		{
			m_Globalize();
			FAviDetails.Globalize();
		}

		public void ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			
			label1.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			label1.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			label4.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			label4.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			label2.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			label2.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			label3.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			label3.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			label5.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			label5.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			label6.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			label6.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			labelCompletedAt.ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
			labelCompletedAt.BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");

			panel_fileDetails.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			panel1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			
			panel5.BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			panel6.BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");

			splitter1.BackColor=eLePhantForm.Skin.GetColor("SplittersBackColor");

			panel7.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			paneleLink.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");

			tabControlFilter.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			tabControlDetails.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			tabControlFilter.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			tabControlDetails.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			tabControlFilter.TextInactiveColor=eLePhantForm.Skin.GetColor("tabsInactiveForeColor");
			tabControlDetails.TextInactiveColor=eLePhantForm.Skin.GetColor("tabsInactiveForeColor");

			imageListMinimize.Images.Clear();
			imageListMinimize.ImageSize=eLePhantForm.Skin.GetBitmap("BtnSeparatorUp").Size;
			imageListMinimize.Images.Add(eLePhantForm.Skin.GetBitmap("BtnSeparatorUp"));
			imageListMinimize.Images.Add(eLePhantForm.Skin.GetBitmap("BtnSeparatorUpMousePressed"));
			imageListMinimize.Images.Add(eLePhantForm.Skin.GetBitmap("BtnSeparatorUpMouseUp"));

			imageListMaximize.Images.Clear();
			imageListMaximize.ImageSize=eLePhantForm.Skin.GetBitmap("BtnSeparatorDown").Size;
			imageListMaximize.Images.Add(eLePhantForm.Skin.GetBitmap("BtnSeparatorDown"));
			imageListMaximize.Images.Add(eLePhantForm.Skin.GetBitmap("BtnSeparatorDownMousePressed"));
			imageListMaximize.Images.Add(eLePhantForm.Skin.GetBitmap("BtnSeparatorDownMouseUp"));

			buttonAdd.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonAdd.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonAddeLink.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonAddeLink.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonCancel.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonCancel.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonPasteeLink.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonPasteeLink.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonRename.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonRename.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonSetCommet.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonSetCommet.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			textBoxComment.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxComment.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			comboBoxCategories.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			comboBoxCategories.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxName.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxName.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxeLink.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxeLink.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			tx_fileName.BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");
			tx_fileName.ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");

			tx_completeName.BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");
			tx_completeName.ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");

			textBoxCompletedAt.BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");
			textBoxCompletedAt.ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");

			listViewComments.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			listViewComments.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			listViewComments.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			listViewComments.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			listViewComments.headerForeColor=eLePhantForm.Skin.GetColor("listsHeaderForeColor");

            listViewNames.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			listViewNames.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			listViewNames.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			listViewNames.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			listViewNames.headerForeColor=eLePhantForm.Skin.GetColor("listsHeaderForeColor");;

			sourcesList.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			sourcesList.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			sourcesList.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			sourcesList.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			sourcesList.headerForeColor=eLePhantForm.Skin.GetColor("listsHeaderForeColor");;

			downloadList.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			downloadList.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			downloadList.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			downloadList.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			downloadList.headerForeColor=eLePhantForm.Skin.GetColor("listsHeaderForeColor");
			
			labelSessionFileDO.ForeColor=eLePhantForm.Skin.GetColor("StatusBarDownSpeedTextColor");
			labelSessionFileUP.ForeColor=eLePhantForm.Skin.GetColor("StatusBarUpSpeedTextColor");
			labelSessionFileRateDO.ForeColor=eLePhantForm.Skin.GetColor("StatusBarDownSpeedTextColor");
			labelSessionFileRateUP.ForeColor=eLePhantForm.Skin.GetColor("StatusBarUpSpeedTextColor");

			textBoxMaxSources.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxMaxSources.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			radioButtonSpecificMaxSources.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			radioButtonUseGlobalMaxSources.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			groupBoxMaxSources.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");

			this.Refresh();

			FAviDetails.ApplySkin();
		}

		public void SaveListsSettings()
		{
			downloadList.SavePreferences();
			sourcesList.SavePreferences();
			listViewNames.SavePreferences();
			listViewComments.SavePreferences();
		}

		private void downloadList_OnSummaryFilterChanged(object sender, EventArgs e)
		{
			m_UpdateFilterLabels();
		}

		private void contextMenuFilter_Popup(object sender, EventArgs e)
		{
			menuItemShowCount.Checked=eLePhantForm.preferences.GetBool("FilterShowCount");
			menuItemShowTotalSizes.Checked=eLePhantForm.preferences.GetBool("FilterShowTotalSizes");
			menuItemAutoSort.Checked=eLePhantForm.preferences.GetBool("AutoSort");
		}

		private void menuItemShowCount_Click(object sender, EventArgs e)
		{
			eLePhantForm.preferences.SetProperty("FilterShowCount",!eLePhantForm.preferences.GetBool("FilterShowCount"));
			m_UpdateFilterLabels();
		}

		private void menuItemShowTotalSizes_Click(object sender, EventArgs e)
		{
			eLePhantForm.preferences.SetProperty("FilterShowTotalSizes",!eLePhantForm.preferences.GetBool("FilterShowTotalSizes"));
			m_UpdateFilterLabels();
		}

		private void menuItemAutoSort_Click(object sender, EventArgs e)
		{
			eLePhantForm.preferences.SetProperty("AutoSort",!eLePhantForm.preferences.GetBool("AutoSort"));
		}

		private void buttonOpenFolder_Click(object sender, System.EventArgs e)
		{
			if (tx_completeName.Text.Length>0)
			{
				FileInfo finfo=new FileInfo(tx_completeName.Text);
				Process.Start(finfo.DirectoryName);
			}
		}

		private void krnGateway_OnCategoryListChanged(object sender, EventArgs e)
		{
			this.m_UpdateCategoryLabels();
		}

		private void manageCategories_Click(object sender, EventArgs e)
		{
			FormCategory form=new FormCategory();
			form.Initilize(krnGateway);
			form.ShowDialog();
		}

		private void radioButtonUseGlobalMaxSources_CheckedChanged(object sender, System.EventArgs e)
		{
			if (!radioButtonUseGlobalMaxSources.Checked) return;
			if (downloadList.SelectedItems.Count>0)
			{
				krnGateway.SetFileMaxSources(((InterfaceFile)downloadList.SelectedItems[0].Tag).strHash,0);
			}
			textBoxMaxSources.Enabled=false;
			textBoxMaxSources.Text="";		
		}

		private void radioButtonSpecificMaxSources_CheckedChanged(object sender, System.EventArgs e)
		{
			if (!radioButtonSpecificMaxSources.Checked) return;
			if (downloadList.SelectedItems.Count>0)
			{
				textBoxMaxSources.Enabled=true;
				textBoxMaxSources.Text=eLePhantForm.m_Preferences.maxSources.ToString();
			}
		}

		private void textBoxMaxSources_TextChanged(object sender, System.EventArgs e)
		{
			if (!radioButtonSpecificMaxSources.Checked) return;
			if (downloadList.SelectedItems.Count>0)
			{
				uint nSources;
				if (textBoxMaxSources.Text.Length==0) 
					nSources=0;
				else
					nSources=uint.Parse(textBoxMaxSources.Text);
				krnGateway.SetFileMaxSources(((InterfaceFile)downloadList.SelectedItems[0].Tag).strHash,nSources);
			}		
		}

		private void textBoxMaxSources_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ((((byte)e.KeyChar<48)||((byte)e.KeyChar>57))&&
				((byte)e.KeyChar!=8))
				e.Handled=true;		
		}
	}
}
