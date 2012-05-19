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
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using eLePhant.eDonkey;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for FormSearch.
	/// </summary>
	public class FormSearch : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox checkBoxSearchAny;
		private System.Windows.Forms.TextBox textBoxAvaibility;
		private System.Windows.Forms.ComboBox comboBoxSearchType;
		private System.Windows.Forms.TextBox textBoxMinSize;
		private System.Windows.Forms.TextBox textBoxMaxSize;
		private System.Windows.Forms.Button buttonExtend;
		private System.Windows.Forms.Button buttonSearch;
		private System.Windows.Forms.TextBox textBoxSearch;

		private CkernelGateway krnGateway;
		private Crownwood.Magic.Controls.TabControl tabSearchs;
		private Hashtable SearchesList;
		private int m_SearchCount;
		private string m_NewTabName;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label labelSizeUpper;
		private System.Windows.Forms.Label labelSizeLower;
		private System.Windows.Forms.Label labelAvailability;
		private System.Windows.Forms.ProgressBar progressBarSearch;
		private Hashtable TotalResult = new Hashtable();
		private Hashtable TabTitle = new Hashtable();
		private System.Windows.Forms.Label labelExclude;
		private System.Windows.Forms.TextBox textBoxExclude;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;

		public FormSearch()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			SearchesList=new Hashtable();

			tabSearchs.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiBox;
			tabSearchs.PositionTop = true;
			this.tabSearchs.PositionTop = true;
			this.tabSearchs.ShowArrows = true;
			this.tabSearchs.ShowClose = true;
			this.tabSearchs.ShrinkPagesToFit = false;
			tabSearchs.ClosePressed += new EventHandler(OnClosePressed);
			m_SearchCount=0;
			m_Globalize();
		}
		public void Connect(CkernelGateway in_krnGateway)
		{
			krnGateway=in_krnGateway;
			krnGateway.OnNewSearched+=new SearchEvent(m_InvokeOnNewSearched);
			krnGateway.OnModifiedSearched+=new SearchEvent(m_InvokeOnModifiedSearched);
			krnGateway.OnEndSearch+=new EndSearchEvent(m_InvokeOnEndSearch);
			krnGateway.OnNewSearchTab+=new SearchTabEvent(m_OnNewSearchTab);
			krnGateway.OnSearchProgress+=new SearchProgressEvent(m_InvokeOnSearchProgress);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormSearch));
			this.panel1 = new System.Windows.Forms.Panel();
			this.textBoxExclude = new System.Windows.Forms.TextBox();
			this.labelExclude = new System.Windows.Forms.Label();
			this.progressBarSearch = new System.Windows.Forms.ProgressBar();
			this.checkBoxSearchAny = new System.Windows.Forms.CheckBox();
			this.textBoxAvaibility = new System.Windows.Forms.TextBox();
			this.labelAvailability = new System.Windows.Forms.Label();
			this.comboBoxSearchType = new System.Windows.Forms.ComboBox();
			this.textBoxMinSize = new System.Windows.Forms.TextBox();
			this.labelSizeUpper = new System.Windows.Forms.Label();
			this.textBoxMaxSize = new System.Windows.Forms.TextBox();
			this.labelSizeLower = new System.Windows.Forms.Label();
			this.buttonExtend = new System.Windows.Forms.Button();
			this.buttonSearch = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.textBoxSearch = new System.Windows.Forms.TextBox();
			this.tabSearchs = new Crownwood.Magic.Controls.TabControl();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.textBoxExclude);
			this.panel1.Controls.Add(this.labelExclude);
			this.panel1.Controls.Add(this.progressBarSearch);
			this.panel1.Controls.Add(this.checkBoxSearchAny);
			this.panel1.Controls.Add(this.textBoxAvaibility);
			this.panel1.Controls.Add(this.labelAvailability);
			this.panel1.Controls.Add(this.comboBoxSearchType);
			this.panel1.Controls.Add(this.textBoxMinSize);
			this.panel1.Controls.Add(this.labelSizeUpper);
			this.panel1.Controls.Add(this.textBoxMaxSize);
			this.panel1.Controls.Add(this.labelSizeLower);
			this.panel1.Controls.Add(this.buttonExtend);
			this.panel1.Controls.Add(this.buttonSearch);
			this.panel1.Controls.Add(this.textBoxSearch);
			this.panel1.Location = new System.Drawing.Point(6, 9);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(158, 355);
			this.panel1.TabIndex = 0;
			// 
			// textBoxExclude
			// 
			this.textBoxExclude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxExclude.Location = new System.Drawing.Point(7, 202);
			this.textBoxExclude.Name = "textBoxExclude";
			this.textBoxExclude.Size = new System.Drawing.Size(143, 20);
			this.textBoxExclude.TabIndex = 24;
			this.textBoxExclude.Text = "";
			this.textBoxExclude.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyUp);
			// 
			// labelExclude
			// 
			this.labelExclude.Location = new System.Drawing.Point(4, 169);
			this.labelExclude.Name = "labelExclude";
			this.labelExclude.Size = new System.Drawing.Size(147, 28);
			this.labelExclude.TabIndex = 23;
			this.labelExclude.Text = "Exclude results containing:";
			this.labelExclude.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// progressBarSearch
			// 
			this.progressBarSearch.Location = new System.Drawing.Point(7, 300);
			this.progressBarSearch.Name = "progressBarSearch";
			this.progressBarSearch.Size = new System.Drawing.Size(147, 24);
			this.progressBarSearch.TabIndex = 22;
			this.progressBarSearch.Visible = false;
			// 
			// checkBoxSearchAny
			// 
			this.checkBoxSearchAny.Location = new System.Drawing.Point(8, 35);
			this.checkBoxSearchAny.Name = "checkBoxSearchAny";
			this.checkBoxSearchAny.Size = new System.Drawing.Size(145, 16);
			this.checkBoxSearchAny.TabIndex = 21;
			this.checkBoxSearchAny.Text = "Match any words";
			// 
			// textBoxAvaibility
			// 
			this.textBoxAvaibility.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxAvaibility.Location = new System.Drawing.Point(98, 140);
			this.textBoxAvaibility.Name = "textBoxAvaibility";
			this.textBoxAvaibility.Size = new System.Drawing.Size(40, 20);
			this.textBoxAvaibility.TabIndex = 20;
			this.textBoxAvaibility.Text = "";
			this.textBoxAvaibility.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMaxSize_KeyPress);
			this.textBoxAvaibility.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyUp);
			// 
			// labelAvailability
			// 
			this.labelAvailability.Location = new System.Drawing.Point(8, 142);
			this.labelAvailability.Name = "labelAvailability";
			this.labelAvailability.Size = new System.Drawing.Size(89, 16);
			this.labelAvailability.TabIndex = 19;
			this.labelAvailability.Text = "Availability >";
			this.labelAvailability.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboBoxSearchType
			// 
			this.comboBoxSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSearchType.Location = new System.Drawing.Point(8, 59);
			this.comboBoxSearchType.Name = "comboBoxSearchType";
			this.comboBoxSearchType.Size = new System.Drawing.Size(143, 21);
			this.comboBoxSearchType.TabIndex = 18;
			// 
			// textBoxMinSize
			// 
			this.textBoxMinSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxMinSize.Location = new System.Drawing.Point(98, 114);
			this.textBoxMinSize.Name = "textBoxMinSize";
			this.textBoxMinSize.Size = new System.Drawing.Size(40, 20);
			this.textBoxMinSize.TabIndex = 17;
			this.textBoxMinSize.Text = "";
			this.textBoxMinSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMaxSize_KeyPress);
			this.textBoxMinSize.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyUp);
			// 
			// labelSizeUpper
			// 
			this.labelSizeUpper.Location = new System.Drawing.Point(8, 115);
			this.labelSizeUpper.Name = "labelSizeUpper";
			this.labelSizeUpper.Size = new System.Drawing.Size(93, 16);
			this.labelSizeUpper.TabIndex = 16;
			this.labelSizeUpper.Text = "Size (MB) >";
			this.labelSizeUpper.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBoxMaxSize
			// 
			this.textBoxMaxSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxMaxSize.Location = new System.Drawing.Point(98, 91);
			this.textBoxMaxSize.Name = "textBoxMaxSize";
			this.textBoxMaxSize.Size = new System.Drawing.Size(40, 20);
			this.textBoxMaxSize.TabIndex = 15;
			this.textBoxMaxSize.Text = "";
			this.textBoxMaxSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMaxSize_KeyPress);
			this.textBoxMaxSize.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyUp);
			// 
			// labelSizeLower
			// 
			this.labelSizeLower.Location = new System.Drawing.Point(8, 91);
			this.labelSizeLower.Name = "labelSizeLower";
			this.labelSizeLower.Size = new System.Drawing.Size(92, 16);
			this.labelSizeLower.TabIndex = 14;
			this.labelSizeLower.Text = "Size (MB) <";
			this.labelSizeLower.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// buttonExtend
			// 
			this.buttonExtend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonExtend.Location = new System.Drawing.Point(5, 263);
			this.buttonExtend.Name = "buttonExtend";
			this.buttonExtend.Size = new System.Drawing.Size(147, 24);
			this.buttonExtend.TabIndex = 13;
			this.buttonExtend.Text = "Search more";
			this.buttonExtend.Click += new System.EventHandler(this.buttonExtend_Click);
			// 
			// buttonSearch
			// 
			this.buttonSearch.BackColor = System.Drawing.SystemColors.Control;
			this.buttonSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSearch.Image = ((System.Drawing.Image)(resources.GetObject("buttonSearch.Image")));
			this.buttonSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonSearch.Location = new System.Drawing.Point(6, 231);
			this.buttonSearch.Name = "buttonSearch";
			this.buttonSearch.Size = new System.Drawing.Size(146, 24);
			this.buttonSearch.TabIndex = 12;
			this.buttonSearch.Text = "Search";
			this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Red;
			// 
			// textBoxSearch
			// 
			this.textBoxSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxSearch.Location = new System.Drawing.Point(8, 11);
			this.textBoxSearch.Name = "textBoxSearch";
			this.textBoxSearch.Size = new System.Drawing.Size(142, 20);
			this.textBoxSearch.TabIndex = 11;
			this.textBoxSearch.Text = "";
			this.textBoxSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyUp);
			// 
			// tabSearchs
			// 
			this.tabSearchs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabSearchs.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tabSearchs.HideTabsMode = Crownwood.Magic.Controls.TabControl.HideTabsModes.ShowAlways;
			this.tabSearchs.Location = new System.Drawing.Point(7, 7);
			this.tabSearchs.Name = "tabSearchs";
			this.tabSearchs.PositionTop = true;
			this.tabSearchs.ShowArrows = true;
			this.tabSearchs.ShowClose = true;
			this.tabSearchs.ShrinkPagesToFit = false;
			this.tabSearchs.Size = new System.Drawing.Size(498, 340);
			this.tabSearchs.TabIndex = 1;
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.tabSearchs);
			this.panel2.Location = new System.Drawing.Point(171, 9);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(515, 356);
			this.panel2.TabIndex = 2;
			// 
			// FormSearch
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.ClientSize = new System.Drawing.Size(693, 372);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormSearch";
			this.Text = "FormSearch";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonSearch_Click(object sender, System.EventArgs e)
		{
			buttonSearch.Enabled=false;
			buttonExtend.Text=eLePhantForm.Globalization["LBL_CANCEL"];
			Crownwood.Magic.Controls.TabPage searchPage;
			searchListView searchList=new searchListView();

			searchList.AllowColumnReorder = true;
			searchList.Dock=DockStyle.Fill;
			searchList.AutoArrange = false;
			searchList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			searchList.DefaultCustomDraw = true;
			searchList.FullRowSelect = true;
			searchList.FullyCustomHeader = false;
			searchList.HideSelection = false;
			searchList.IncreaseHeaderHeight = 0;
			searchList.View = System.Windows.Forms.View.Details;
			searchList.Initilize(krnGateway);
			searchList.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			searchList.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			searchList.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			searchList.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			
			searchPage = new Crownwood.Magic.Controls.TabPage(textBoxSearch.Text, searchList, null);
			searchPage.Tag=m_SearchCount;
			searchList.Tag=m_SearchCount;
			SearchesList.Add(m_SearchCount,searchList);
			tabSearchs.TabPages.Add(searchPage);
			tabSearchs.SelectedIndex=tabSearchs.TabPages.Count-1;
			uint MinSize;
			try{MinSize=Convert.ToUInt32(textBoxMinSize.Text)*1048576;}	catch{MinSize=0;}
			uint MaxSize;
			try{MaxSize=Convert.ToUInt32(textBoxMaxSize.Text)*1048576;}	catch{MaxSize=0;}
			uint Avaibility;
			try{Avaibility=Convert.ToUInt32(textBoxAvaibility.Text);} catch{Avaibility=0;}
			string searchType="";
			switch (comboBoxSearchType.SelectedIndex)
			{
				case 0: searchType="";
						break;
				case 1: searchType="Video";
						break;
				case 2: searchType="Pro";
						break;
				case 3: searchType="Audio";
						break;
				case 4: searchType="Image";
						break;
			}
			TotalResult.Add(m_SearchCount,(uint)0);
			TabTitle.Add(m_SearchCount,textBoxSearch.Text);
			krnGateway.Search(m_SearchCount, textBoxSearch.Text,checkBoxSearchAny.Checked,searchType,MaxSize,MinSize,Avaibility, textBoxExclude.Text);
			m_SearchCount++;
		}

		private void m_InvokeOnNewSearched(InterfaceSearchedFile file, int searchID)
		{
			this.BeginInvoke(new SearchEvent(m_OnNewSearched),new object[] {file,searchID});
		}
		
		private void m_OnNewSearched(InterfaceSearchedFile file, int searchID)
		{			
			foreach (searchListView searchList in SearchesList.Values)
			{
				if ((int)searchList.Tag==searchID)
				{	
					m_AddTotalResultsToTabPage(file,searchID,false,0);
					searchList.OnNewFile(file);
					break;
				}
			}			
		}

		private void m_AddTotalResultsToTabPage(InterfaceSearchedFile file, int searchID,bool ondiff, uint diff)
		{	
			try
			{	
				if(!TotalResult.ContainsKey(searchID)) return;
				uint Total;

				foreach (Crownwood.Magic.Controls.TabPage TabPage in tabSearchs.TabPages)
				{
					if ((int)TabPage.Tag==searchID)
					{	
						if(ondiff)
							Total = (uint)TotalResult[searchID]+diff;
						else
							Total = (uint)TotalResult[searchID]+file.Avaibility;

						TotalResult[searchID] = Total;
						TabPage.Title = TabTitle[searchID] + " (" +Total+")";
					}
				}
			}
			catch(Exception Ex)
			{
				Debug.WriteLine(Ex.ToString());
			}
		}

		private void m_InvokeOnModifiedSearched(InterfaceSearchedFile file, int searchID)
		{
			this.BeginInvoke(new SearchEvent(m_OnModifiedSearched),new object[] {file,searchID});
		}

		private void m_OnModifiedSearched(InterfaceSearchedFile file, int searchID)
		{	
			uint diff;
			foreach (searchListView searchList in SearchesList.Values)
			{
				if ((int)searchList.Tag==searchID)
				{	
					diff=searchList.OnUpdateFile(file);
					m_AddTotalResultsToTabPage(file,searchID,true,diff);
				}
			}
		}

		private void m_InvokeOnEndSearch(int searchID)
		{
			this.BeginInvoke(new EndSearchEvent(m_OnEndSearch),new object[] {searchID});
		}

		public void m_OnEndSearch(int searchID)
		{
			buttonSearch.Enabled=true;
			progressBarSearch.Value=0;
			progressBarSearch.Visible=false;
			buttonExtend.Text=eLePhantForm.Globalization["LBL_SEARCHEXTEND"];
		}
		private void m_OnNewSearchTab(int searchID, string tabName)
		{
			MethodInvoker mi = new MethodInvoker(this.m_CreateTab);
			m_SearchCount=searchID;
			m_NewTabName=tabName;
			this.BeginInvoke(mi);
			Thread.Sleep(300) ;
		}

		private void m_CreateTab()
		{
			Crownwood.Magic.Controls.TabPage searchPage;
			searchListView searchList=new searchListView();
			searchList.AllowColumnReorder = true;
			searchList.Dock=DockStyle.Fill;
			searchList.AutoArrange = false;
			searchList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			searchList.DefaultCustomDraw = true;
			searchList.FullRowSelect = true;
			searchList.FullyCustomHeader = false;
			searchList.HideSelection = false;
			searchList.IncreaseHeaderHeight = 0;
			searchList.View = System.Windows.Forms.View.Details;
			searchList.Initilize(krnGateway);
			searchList.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
			searchList.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
			searchList.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
			searchList.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
			searchList.AlreadyDownloadingItemColor=eLePhantForm.Skin.GetColor("SearchAlreadyDownloadingItemColor");
			searchList.AlreadyDownloadedItemColor=eLePhantForm.Skin.GetColor("SearchAlreadyDownloadedItemColor");

			searchPage = new Crownwood.Magic.Controls.TabPage(m_NewTabName, searchList, null);
			if (SearchesList.ContainsKey(m_SearchCount)) m_SearchCount++; //TODO review this !
			searchPage.Tag=m_SearchCount;
			searchList.Tag=m_SearchCount;
			SearchesList.Add(m_SearchCount,searchList);
			tabSearchs.TabPages.Add(searchPage);
			tabSearchs.SelectedIndex=tabSearchs.TabPages.Count-1;
			m_SearchCount++;
		}
		private void buttonExtend_Click(object sender, System.EventArgs e)
		{
			if (buttonExtend.Text==eLePhantForm.Globalization["LBL_CANCEL"]) 
			{
				if (tabSearchs.SelectedIndex>=0) m_OnEndSearch((int)tabSearchs.TabPages[tabSearchs.SelectedIndex].Tag);
				if (tabSearchs.SelectedIndex>=0) krnGateway.CancelSearch((int)tabSearchs.TabPages[tabSearchs.SelectedIndex].Tag);
			}
			else 
			{
				if (tabSearchs.SelectedIndex>=0) krnGateway.ExtendSearch((int)tabSearchs.TabPages[tabSearchs.SelectedIndex].Tag);
			}
		}
		protected void OnClosePressed(object sender,EventArgs e)
		{
			if (tabSearchs.SelectedIndex<0) return;
			if (tabSearchs.SelectedIndex==tabSearchs.TabPages.Count-1) 
				m_OnEndSearch((int)tabSearchs.TabPages[tabSearchs.SelectedIndex].Tag);
			int tabIndexToRemove=(int)tabSearchs.TabPages[tabSearchs.SelectedIndex].Tag;
			krnGateway.DeleteSearch((int)tabSearchs.TabPages[tabSearchs.SelectedIndex].Tag);
			tabSearchs.TabPages.RemoveAt(tabSearchs.SelectedIndex);
			TotalResult.Remove(tabIndexToRemove);
			TabTitle.Remove(tabIndexToRemove);
			
			//SearchesList.Remove(tabIndexToRemove);
		}

		private void textBoxSearch_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if ((e.KeyData==Keys.Enter)&&(buttonSearch.Enabled)) buttonSearch_Click(this,null);
		}

		private void m_Globalize()
		{
			checkBoxSearchAny.Text = eLePhantForm.Globalization["LBL_SEARCHANY"];
			buttonExtend.Text = eLePhantForm.Globalization["LBL_SEARCHEXTEND"];
			buttonSearch.Text = eLePhantForm.Globalization["LBL_SEARCHFILE"];
			labelAvailability.Text = eLePhantForm.Globalization["LBL_AVAILABILITY"] + " > ";
			labelSizeLower.Text = eLePhantForm.Globalization["LBL_SIZELOWER"] + " < ";
			labelSizeUpper.Text = eLePhantForm.Globalization["LBL_SIZEUPPER"] + " > ";
			labelExclude.Text=eLePhantForm.Globalization["LBL_EXCLUDE"]+":";
			
			comboBoxSearchType.Items.Clear();
			comboBoxSearchType.Items.AddRange(new object[] {   eLePhantForm.Globalization["LBL_ANY"],
															   eLePhantForm.Globalization["LBL_VIDEO"],
															   eLePhantForm.Globalization["LBL_PROGRAM"],
															   eLePhantForm.Globalization["LBL_AUDIO"],
															   eLePhantForm.Globalization["LBL_IMAGE"]});
			if (comboBoxSearchType.SelectedIndex==-1) comboBoxSearchType.SelectedIndex=0;
		}
		
		public void Globalize()
		{
			m_Globalize();
		}

		public void SaveListsSettings()
		{
			if (tabSearchs.TabPages.Count>0)
			{
				searchListView searchList;
				searchList=(searchListView)SearchesList[tabSearchs.TabPages[tabSearchs.TabPages.Count-1].Tag];
				searchList.SavePreferences();
			}
		}

		private void m_InvokeOnSearchProgress(int progress, int searchID)
		{
			this.Invoke(new SearchProgressEvent(m_OnSearchProgress),new object[] {progress,searchID});
		}
		private void m_OnSearchProgress(int progress, int searchID)
		{
			progressBarSearch.Visible=true;
			progressBarSearch.Value=progress;
		}
		public void ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");

			panel1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			panel2.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");

			tabSearchs.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			tabSearchs.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");

			labelAvailability.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelAvailability.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelSizeLower.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelSizeLower.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelSizeUpper.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelSizeUpper.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			labelExclude.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			labelExclude.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			checkBoxSearchAny.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			checkBoxSearchAny.BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");

			textBoxAvaibility.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxAvaibility.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxSearch.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxSearch.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxMaxSize.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxMaxSize.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxMinSize.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxMinSize.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			textBoxExclude.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			textBoxExclude.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			comboBoxSearchType.ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
			comboBoxSearchType.BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");

			buttonSearch.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonSearch.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");

			buttonExtend.BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
			buttonExtend.ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");
			
			if ((SearchesList!=null)&&(SearchesList.Count>0))
			{
				foreach (searchListView searchList in SearchesList.Values)
				{
					searchList.ForeColor=eLePhantForm.Skin.GetColor("listsForeColor");
					searchList.BackColor=eLePhantForm.Skin.GetColor("listsBackColor");
					searchList.headerBackColor=eLePhantForm.Skin.GetColor("listsHeaderBackColor");
					searchList.ScrollBarBKColor=eLePhantForm.Skin.GetColor("listsScrollBarBackColor").ToArgb();
					searchList.AlreadyDownloadingItemColor=eLePhantForm.Skin.GetColor("SearchAlreadyDownloadingItemColor");
					searchList.AlreadyDownloadedItemColor=eLePhantForm.Skin.GetColor("SearchAlreadyDownloadedItemColor");
				}
			}
			
			this.Refresh();
		}

		private void textBoxMaxSize_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ((((byte)e.KeyChar<48)||((byte)e.KeyChar>57))&&
				((byte)e.KeyChar!=8))
				e.Handled=true;
		}
	}
}
