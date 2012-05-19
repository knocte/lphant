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

using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.Win32;
using eLePhant.eDonkey;
using eLePhant.Interface;
using eLePhant.Types;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for downloadsListView.
	/// </summary>
	public class downloadsListView : eLePhantListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private CkernelGateway krnGateway;
		private System.ComponentModel.Container components = null;
		public ContextMenu ContextMenuDownloads;
		private  ColumnSorter TheColumnSorter;
		private byte m_filter;
		private uint m_CategoryFilter;
		private bool m_ClearingCompleted;
		private string[] m_States;
		private string[] m_Priorities;
		private MenuItem PriorityMenu;
		private MenuItem MenuItemInfo;
		public CMenuInfo MenuInfo;

		private MenuItem CategoryMenu;

		public downloadsListView(System.ComponentModel.IContainer container,CInterfaceGateway in_krnGateway)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			if (container!=null) container.Add(this);
			InitializeComponent();
			//m_InitControls();
		}
		public downloadsListView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
			//if (!base.DesignMode) m_InitControls(); //this doesnt work
			if (Application.ExecutablePath.ToLower().IndexOf("\u0065\u006C\u0065\u0070\u0068\u0061\u006E\u0074")>-1) m_InitControls();
		}
		private void m_InitControls()
		{
			Name = "downloadList";
			Columns.Add("File Name",190,false);
			Columns.Add("Size",60,false);
			Columns.Add("Speed",60,false);
			Columns.Add("Completed",60,false);
			Columns.Add("Remaining",60,false);
			Columns.Add("Sources",60,false);
			Columns.Add("Usefull sources",60,false);
			Columns.Add("Transfers",60,false);
			Columns.Add("Status",60,false);
			Columns.Add("Progress",190,true);
			Columns.Add("Priority",60,false);
			Columns.Add("Estimated",70,false);
			Columns.Add("Category",70,false);
			ContextMenuDownloads = new ContextMenu();
			ContextMenuDownloads.MenuItems.Add("Start",new EventHandler(OnStartFile));
			ContextMenuDownloads.MenuItems.Add("Stop",new EventHandler(OnStopFile));
			ContextMenuDownloads.MenuItems.Add("Cancel",new EventHandler(OnCancelFile));
			ContextMenuDownloads.MenuItems.Add("-");
			ContextMenuDownloads.MenuItems.Add("Preview",new EventHandler(OnPreviewFile));
			ContextMenuDownloads.MenuItems.Add("Open File",new EventHandler(OnOpenFile));
			ContextMenuDownloads.MenuItems.Add("-");
			ContextMenuDownloads.MenuItems.Add("Copy eLink to clipboard", new EventHandler(OnCopyLink));
			ContextMenuDownloads.MenuItems.Add("Copy eLink to clipboard (HTML)", new EventHandler(OnCopyLinkHTML));
			ContextMenuDownloads.MenuItems.Add("-");
			PriorityMenu = new MenuItem( "&Priority"); 
			ContextMenuDownloads.MenuItems.Add(PriorityMenu);
			PriorityMenu.MenuItems.Add("High",new EventHandler(OnHighPriority));
			PriorityMenu.MenuItems.Add("Normal",new EventHandler(OnNormalPriority));
			PriorityMenu.MenuItems.Add("Low",new EventHandler(OnLowPriority));

			CategoryMenu=new MenuItem("Category");
			ContextMenuDownloads.MenuItems.Add(CategoryMenu);

			ContextMenuDownloads.MenuItems.Add("-");
			ContextMenuDownloads.MenuItems.Add("Show details");
			ContextMenuDownloads.MenuItems.Add("-");

			MenuItemInfo=new MenuItem("Search more info");
			ContextMenuDownloads.MenuItems.Add(MenuItemInfo);
			MenuInfo=new CMenuInfo(MenuItemInfo);
			//MenuInfo.DisplayAllMenuItem.Click+=new EventHandler(m_DisplayAllMenuItem_Click);
			//
			ContextMenuDownloads.MenuItems.Add("-");
			ContextMenuDownloads.MenuItems.Add("Clear completed", new EventHandler(OnClearCompleted));
			
			TheColumnSorter=new ColumnSorter();
			TheColumnSorter.CurrentColumn=3;
			sortColumn=3;
			sortOrder=-1;
			TheColumnSorter.sortOrder=-1;
			this.ClickHeader+=new HeaderEventHandler(myColumnClick);
			this.KeyDown+=new KeyEventHandler(OnKeyDown);
			
			ContextMenu=ContextMenuDownloads;

			ContextMenuDownloads.Popup+=new System.EventHandler(myContextMenuPopup);
			m_DefaultWidths=new int[] {190,60,60,60,60,60,60,60,60,190,60,70,70};
			m_LoadWidths();
			SmallImageList=eLePhantForm.SystemIconsList.list;
			m_Globalize();
		}

		private void m_Globalize()
		{
			Columns[0].Text=eLePhantForm.Globalization["LBL_FILENAME"];
			Columns[1].Text=eLePhantForm.Globalization["LBL_SIZE"];
			Columns[2].Text=eLePhantForm.Globalization["LBL_SPEED"];
			Columns[3].Text=eLePhantForm.Globalization["LBL_COMPLETED"];
			Columns[4].Text=eLePhantForm.Globalization["LBL_REMAINING"];
			Columns[5].Text=eLePhantForm.Globalization["LBL_SOURCES"];
			Columns[6].Text=eLePhantForm.Globalization["LBL_USEFULLSOURCES"];
			Columns[7].Text=eLePhantForm.Globalization["LBL_TRANSFERS"];
			Columns[8].Text=eLePhantForm.Globalization["LBL_STATUS"];
			Columns[9].Text=eLePhantForm.Globalization["LBL_PROGRESS"];
			Columns[10].Text=eLePhantForm.Globalization["LBL_PRIORITY"];
			Columns[11].Text=eLePhantForm.Globalization["LBL_ESTIMATED"];
			Columns[12].Text=eLePhantForm.Globalization["LBL_CATEGORY"];
			ContextMenuDownloads.MenuItems[0].Text=eLePhantForm.Globalization["LBL_START"];
			ContextMenuDownloads.MenuItems[1].Text=eLePhantForm.Globalization["LBL_STOP"];
			ContextMenuDownloads.MenuItems[2].Text=eLePhantForm.Globalization["LBL_CANCEL"];
			ContextMenuDownloads.MenuItems[4].Text=eLePhantForm.Globalization["LBL_PREVIEW"];
			ContextMenuDownloads.MenuItems[5].Text=eLePhantForm.Globalization["LBL_OPENFILE"];
			ContextMenuDownloads.MenuItems[7].Text=eLePhantForm.Globalization["LBL_COPYLINK"];
			ContextMenuDownloads.MenuItems[8].Text=eLePhantForm.Globalization["LBL_COPYLINK"]+" (HTML)";
			ContextMenuDownloads.MenuItems[10].Text=eLePhantForm.Globalization["LBL_PRIORITY"];
			ContextMenuDownloads.MenuItems[11].Text=eLePhantForm.Globalization["LBL_CATEGORY"];

			ContextMenuDownloads.MenuItems[13].Text=eLePhantForm.Globalization["LBL_SHOWHIDEDETAILS"];
			ContextMenuDownloads.MenuItems[15].Text=eLePhantForm.Globalization["LBL_SEARCHINFO"];
			ContextMenuDownloads.MenuItems[17].Text=eLePhantForm.Globalization["LBL_CLEARCOMPLETED"];
			
			PriorityMenu.MenuItems[0].Text=eLePhantForm.Globalization["LBL_HIGH"];
			PriorityMenu.MenuItems[1].Text=eLePhantForm.Globalization["LBL_NORMAL"];
			PriorityMenu.MenuItems[2].Text=eLePhantForm.Globalization["LBL_LOW"];
			m_States=new string[] {
				eLePhantForm.Globalization["LBL_READY"],
				eLePhantForm.Globalization["LBL_PAUSED"],
				eLePhantForm.Globalization["LBL_STOPPED"],
				"","","","","",
				eLePhantForm.Globalization["LBL_COMPLETING"],
				eLePhantForm.Globalization["LBL_COMPLETE"],
			};
			m_Priorities=new string[] {
				eLePhantForm.Globalization["LBL_LOW"],
				eLePhantForm.Globalization["LBL_NORMAL"],
				eLePhantForm.Globalization["LBL_HIGH"],
				eLePhantForm.Globalization["LBL_AUTO"],
			};
		}
		
		public void Globalize()
		{
			m_Globalize();
			this.Refresh();
		}

		public void Initilize(CkernelGateway in_krnGateway)
		{
			krnGateway=in_krnGateway;
			krnGateway.OnNewFile+=new FileEvent(m_InvokeOnNewFile);	
			krnGateway.OnRefreshDownloads+=new RefreshEvent(m_InvokeRefreshList);
			m_ClearingCompleted=false;

			m_filter=0;

			ReloadList();
		}

		~downloadsListView()
		{

		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{			
			switch(e.KeyCode)
			{
				case Keys.Delete: 
					m_OnCancelFile();
					break;
				case Keys.Space:
					m_OnClearCompleted();
					break;
			}
		}

		private void myColumnClick(object sender, HeaderEventArgs e)
		{
			TheColumnSorter.CurrentColumn = e.ColumnIndex;
			TheColumnSorter.sortOrder=this.sortOrder;
			this.ListViewItemSorter = TheColumnSorter;	
			this.Sort();
			this.ListViewItemSorter = null;			
		}

		private void myContextMenuPopup(object sender, System.EventArgs e)
		{	
			try //to fix a not able to reproduce exception reported
			{
				ContextMenuDownloads.MenuItems[13].DefaultItem = true;
				MenuInfo.OnMenuInfoChange();
				m_CheckPriority();
				m_CheckCategories();

				if (SelectedItems.Count==0)
				{
					ContextMenuDownloads.MenuItems[0].Enabled=false;
					ContextMenuDownloads.MenuItems[1].Enabled=false;
					ContextMenuDownloads.MenuItems[2].Enabled=false;
					ContextMenuDownloads.MenuItems[4].Enabled=false;
					ContextMenuDownloads.MenuItems[5].Enabled=false;
					ContextMenuDownloads.MenuItems[7].Enabled=false;
					ContextMenuDownloads.MenuItems[8].Enabled=false;
					ContextMenuDownloads.MenuItems[10].Enabled=false;
					ContextMenuDownloads.MenuItems[13].Enabled=false;
					ContextMenuDownloads.MenuItems[14].Enabled=false;
				}
				if (SelectedItems.Count==1)
				{	
					ContextMenuDownloads.MenuItems[0].Enabled=(!IsCompleted()&&!m_IsStarted());
					ContextMenuDownloads.MenuItems[1].Enabled=(!IsCompleted()&& m_IsStarted());
					ContextMenuDownloads.MenuItems[2].Enabled=!IsCompleted();
					ContextMenuDownloads.MenuItems[4].Enabled=!IsCompleted();
					ContextMenuDownloads.MenuItems[5].Enabled=IsCompleted();
					ContextMenuDownloads.MenuItems[7].Enabled=true;
					ContextMenuDownloads.MenuItems[8].Enabled=true;
					ContextMenuDownloads.MenuItems[10].Enabled=!IsCompleted();
					ContextMenuDownloads.MenuItems[13].Enabled=true;
					ContextMenuDownloads.MenuItems[14].Enabled=true;
				}
				if (SelectedItems.Count>1)
				{
					ContextMenuDownloads.MenuItems[0].Enabled=true;
					ContextMenuDownloads.MenuItems[1].Enabled=true;
					ContextMenuDownloads.MenuItems[2].Enabled=true;
					ContextMenuDownloads.MenuItems[4].Enabled=false;
					ContextMenuDownloads.MenuItems[5].Enabled=false;
					ContextMenuDownloads.MenuItems[7].Enabled=false;
					ContextMenuDownloads.MenuItems[8].Enabled=false;
					ContextMenuDownloads.MenuItems[10].Enabled=true;
					ContextMenuDownloads.MenuItems[13].Enabled=false;
					ContextMenuDownloads.MenuItems[14].Enabled=false;
				}
				if (!krnGateway.IsLocal) ContextMenuDownloads.MenuItems[4].Enabled=false;
			}
			catch
			{}
		}

		private void OnPreviewFile(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			sFileDetails FileInfo = (sFileDetails)krnGateway.GetFileDetails(((InterfaceFile)SelectedItems[0].Tag).strHash);
			string fileExtension=CUtils.GetExtension(FileInfo.FileName);
			
			if (CUtils.IsVideo(FileInfo.FileName))
			{
				string player=eLePhantForm.preferences.GetString("PreviewPlayer");
				if ((player==null)||(player==""))
				{
					MessageBox.Show(eLePhantForm.Globalization["LBL_NOPLAYER"],"lphant",MessageBoxButtons.OK,MessageBoxIcon.Information);
					return;
				}
				try
				{
					Process.Start("\""+player+"\"","\""+FileInfo.DiskFileName+"\"");
				}
				catch{}
			}
			else //not a video file, try to open the default application for the extension
			{
				try
				{
					RegistryKey key=Registry.ClassesRoot.OpenSubKey(fileExtension);
					if (key==null) return;
					string type=(string)key.GetValue("");
					if (type.Length==0) return;
					key=Registry.ClassesRoot.OpenSubKey(type+"\\shell\\open\\command");
					if (key==null) return;
					string command=(string)key.GetValue("");
					if (command.Length==0) return;
					string exeCommand;
					if (command.IndexOf("%1")>0)
						exeCommand=command.Replace("%1",FileInfo.DiskFileName);
					else
						exeCommand=command.Replace("%L",FileInfo.DiskFileName);
					string parameters;

					if (exeCommand.StartsWith("\""))
					{
						parameters=exeCommand.Substring(exeCommand.IndexOf("\"",2)+2);
						exeCommand=exeCommand.Substring(0,exeCommand.IndexOf("\"",2)+1);
					}
					else
					{
						parameters=exeCommand.Substring(exeCommand.IndexOf(" ",2)+2);
						exeCommand=exeCommand.Substring(0,exeCommand.IndexOf(" ",2)+1);
					}
					if (!parameters.StartsWith("\""))
						parameters="\""+parameters+"\"";
					Debug.WriteLine(exeCommand);
					Debug.WriteLine(parameters);
					Process.Start(exeCommand,parameters);
				}
				catch(Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}

		private void OnStartFile(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.ResumeFile(((InterfaceFile)SelectedItems[i].Tag).strHash);
			}
		}

		private void m_OnClearCompleted()
		{
			m_ClearingCompleted=true;
			m_RefreshList(krnGateway);
			m_ClearingCompleted=false;
			krnGateway.CalculateFilterSummary();
		}

		private void OnClearCompleted(object sender, System.EventArgs e)
		{
			m_OnClearCompleted();
		}

		private void OnStopFile(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.StopFile(((InterfaceFile)SelectedItems[i].Tag).strHash);
			}
		}
		private void OnCancelFile(object sender, System.EventArgs e)
		{
			m_OnCancelFile();	
		}

		private void m_OnCancelFile()
		{
			if (SelectedItems.Count==0) return;
			
			try
			{
				int SelectedItem = SelectedItems.Count;
				InterfaceFile[] Items= new InterfaceFile[SelectedItem];

				for(int i=0;i!=SelectedItem;i++)
				{
					Items[i] = (InterfaceFile)SelectedItems[i].Tag;
				}

				for(int i=0;i!=SelectedItem;i++)
				{
					string message = String.Format(eLePhantForm.Globalization["MSG_DELETEFILE"],(Items[i].Name));
					if (MessageBox.Show(message,"lphant",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)
					{						
						krnGateway.CancelFile(Items[i].strHash);
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private void OnOpenFile(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
					
			try
			{
				System.Diagnostics.Process.Start(file.CompleteName);
			}
			catch{}
		}

		private void OnCopyLink(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			Clipboard.SetDataObject("ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strHash+"|/");
		}
		
		private void OnCopyLinkHTML(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			Clipboard.SetDataObject("<a href=\"ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strHash+"|/\">"+file.Name+"</a>");
		}
		private void OnHighPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileDownloadPrioriry(((InterfaceFile)SelectedItems[i].Tag).strHash,Types.Constants.Priority.High);
			}
		}
		private void OnNormalPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileDownloadPrioriry(((InterfaceFile)SelectedItems[i].Tag).strHash,Types.Constants.Priority.Normal);
			}
		}
		private void OnLowPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileDownloadPrioriry(((InterfaceFile)SelectedItems[i].Tag).strHash,Types.Constants.Priority.Low);
			}
		}

		protected override void OnDrawItem(int item,int column,Graphics g,Rectangle rec)
		{
			try 
			{
				InterfaceFile file = (InterfaceFile)(Items[item].Tag);
				Bitmap bar=new Bitmap(rec.Width,rec.Height,g);
				Graphics g2=Graphics.FromImage(bar);
				eLePhantInspector.DrawBar(file, g2, new Rectangle(0,0,rec.Width,rec.Height), true, true);
				g2.FillRectangle(new SolidBrush(this.BackColor),0,rec.Height-1,rec.Width,1);
				g.DrawImageUnscaled(bar, rec.Left, rec.Top);
				bar.Dispose();

				//draw a backcolor margin
				//g.FillRectangle(new SolidBrush(this.BackColor),rec.X,rec.Bottom-1,rec.Width,1);
			} 
			catch
			{
				Debug.WriteLine ("GDI Error: drawing element bar");
			}
		}
		
		private string StatusToString(byte status)
		{
			if (status>m_States.Length) return "Unknown";
			return m_States[status];
		}
		private void FileToItem(InterfaceFile file,ListViewItem itemFile)
		{
			if (file==null) return;
			if (itemFile.SubItems[0].Text!=file.Name) itemFile.SubItems[0].Text=file.Name;
														  //CNameCleaner.Clean(file.Name);
			if (itemFile.SubItems[1].Text!=SizeToString(file.Size)) itemFile.SubItems[1].Text=SizeToString(file.Size);
			if (itemFile.SubItems[2].Text!=SpeedToString(file.DownSpeed)) itemFile.SubItems[2].Text=SpeedToString(file.DownSpeed);
			if (itemFile.SubItems[3].Text!=SizeToString(file.BytesDownloaded)) itemFile.SubItems[3].Text=SizeToString(file.BytesDownloaded);

			if (itemFile.SubItems[4].Text!=SizeToString(file.RemainingBytes)) itemFile.SubItems[4].Text=SizeToString(file.RemainingBytes);
			if (itemFile.SubItems[5].Text!=Convert.ToString(file.nSources)) itemFile.SubItems[5].Text=Convert.ToString(file.nSources);
			if (itemFile.SubItems[6].Text!=Convert.ToString(file.nValidSources)) itemFile.SubItems[6].Text=Convert.ToString(file.nValidSources);
			if (itemFile.SubItems[7].Text!=Convert.ToString(file.nTransferringSources)) itemFile.SubItems[7].Text=Convert.ToString(file.nTransferringSources);
			if (itemFile.SubItems[8].Text!=StatusToString(file.Status)) itemFile.SubItems[8].Text=StatusToString(file.Status);
			if (itemFile.SubItems[9].Text!="") itemFile.SubItems[9].Text="";
			if (itemFile.SubItems[10].Text!=PriorityToString(file.DownloadPriority)) itemFile.SubItems[10].Text=PriorityToString(file.DownloadPriority);
			
			string estimatedTime="?";
			if (file.DownSpeed>0.3F)
			{
				TimeSpan ts=TimeSpan.FromSeconds(file.RemainingBytes/1024/file.DownSpeed);//new TimeSpan(0,0,file.RemainingBytes*1024/file.DownSpeed);
				if (ts.Days<=99)
				{
					if (ts.Days>0) 
						estimatedTime=ts.Days+"d ";
					else
						estimatedTime="";
					estimatedTime+=ts.Hours+"h "+ts.Minutes+"m";
				}
			}
			if (itemFile.SubItems[11].Text!=estimatedTime) itemFile.SubItems[11].Text=estimatedTime;

			if (itemFile.SubItems[12].Text!=file.Category) itemFile.SubItems[12].Text=file.Category;

			itemFile.ImageIndex=eLePhantForm.SystemIconsList.GetIconIndexOf(file.Name);
			itemFile.Tag=file;
		}

		private void m_InvokeOnNewFile(InterfaceFile file)
		{
			if (this.InvokeRequired)
				this.BeginInvoke(new FileEvent(m_OnNewFile),new object[] {file});
			else
				m_OnNewFile(file);
		}
		private void m_OnNewFile(InterfaceFile file)
		{
			ListViewItem itemFile=new ListViewItem(new string[]{"","","","","","","","","","","","",""});
		
			itemFile.Tag=file;
			FileToItem(file,itemFile);
			lock(this)
			{
				if ((!IsFiltered(file.Status))&&
					(!IsCategoryFiltered(file.CategoryID,file.Name))) Items.Add(itemFile);
			}
		}
	
		/*private void m_DisplayAllMenuItem_Click(object sender, EventArgs e)
		{
			MenuInfo.DisplayAll = !MenuInfo.DisplayAllMenuItem.Checked;
			MenuInfo.DisplayAllMenuItem.Checked = !MenuInfo.DisplayAllMenuItem.Checked;
			MenuInfo=new CMenuInfo(MenuItemInfo,"fr-FR",MenuInfo.DisplayAll);
		}*/

		private void m_InvokeRefreshList(CkernelGateway in_krnGateway)
		{
			this.Invoke(new RefreshEvent(m_RefreshList),new object[] {in_krnGateway});			
		}
		private void m_RefreshList(CkernelGateway in_krnGateway)
		{
			foreach (ListViewItem Item in Items)
			{
				InterfaceFile file=in_krnGateway.GetDownloadFile(((InterfaceFile)Item.Tag).strHash);
				if ((file==null)||((m_ClearingCompleted)&&(((InterfaceFile)Item.Tag).Status==9))) Items.Remove(Item);
				else FileToItem(file,Item);
			}
			//enable this to autosort the list
			if (eLePhantForm.preferences.GetBool("AutoSort"))
			{
				this.ListViewItemSorter = TheColumnSorter;	
				this.Sort();
				this.ListViewItemSorter = null;
			}
		}
		private void ReloadList()
		{
			InterfaceFile[] files=krnGateway.GetDownloadFileList();			
			Items.Clear();
			if (files==null) return;
			lock(this)
			{
				foreach (InterfaceFile file in files)
				{
					if (file!=null)
					{
						m_OnNewFile(file);
					}
				}
			}
			this.ListViewItemSorter = TheColumnSorter;
			this.Sort();
			this.ListViewItemSorter = null;
		}
		
		public void SetCategoryFilter(string category)
		{
			uint antFilter=m_CategoryFilter;
			if (category.Length==0) 
				m_CategoryFilter=0;
			else if (category=="LBL_VIDEO")
				m_CategoryFilter=uint.MaxValue-1;
			else if (category=="LBL_AUDIO")
				m_CategoryFilter=uint.MaxValue-2;
			else if (category=="LBL_FILE")
				m_CategoryFilter=uint.MaxValue-3;
			else
			{
				CCategory cat=krnGateway.GetCategoryByName(category);
				if (cat!=null)
					m_CategoryFilter=cat.ID;
			}
			if (m_CategoryFilter!=antFilter) ReloadList();
		}

		public void SetFilter(byte filter) 
		{
			if (m_filter!=filter)
			{
				m_filter=filter;
				ReloadList();
			}

		}
		private bool IsFiltered(byte status)
		{
			if (m_filter==1) return ((status==1)||(status==2));
			if (m_filter==2) return ((status!=1)&&(status!=2));
			return false;
		}
		
		private bool IsCategoryFiltered(uint catID,string fileName)
		{
			if (m_CategoryFilter==uint.MaxValue-1)
				return (!CUtils.IsVideo(fileName));
			if (m_CategoryFilter==uint.MaxValue-2)
				return (!CUtils.IsAudio(fileName));
			if (m_CategoryFilter==uint.MaxValue-3)
				return (!CUtils.IsFile(fileName));
			return ((m_CategoryFilter!=0)&&(m_CategoryFilter!=catID));
		}

		private bool IsCompleted()
		{
			if (SelectedItems.Count==0) return false;
			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			
			//status = 9 -> completed
			if(file.Status==9) 
				return true;
			else
				return false;

		}
		
		private bool m_IsStarted()
		{
			if (SelectedItems.Count==0) return false;
			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			
			//status = 9 -> completed
			if (file.Status==0) 
				return true;
			else
				return false;
		}

		private void m_CheckPriority()
		{	
			PriorityMenu.MenuItems[0].Checked=false;
			PriorityMenu.MenuItems[1].Checked=false;
			PriorityMenu.MenuItems[2].Checked=false;
			
			if (SelectedItems.Count==0) return;
			if (SelectedItems.Count>1) return;

			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			byte index=1;
			
			switch(file.DownloadPriority)
			{
				case Constants.Priority.High:index=0;
					break;
				case Constants.Priority.Normal:index=1;
					break;
				case Constants.Priority.Low:index=2;
					break;
				default: index=1;
					break;
			}
			PriorityMenu.MenuItems[index].Checked=true;
		}

		private void m_CheckCategories()
		{
			ArrayList categories=krnGateway.GetCategories();
			CategoryMenu.MenuItems.Clear();
			MenuItem noneItem=CategoryMenu.MenuItems.Add(eLePhantForm.Globalization["LBL_NONE"], new EventHandler(OnSelectCategory));
			CategoryMenu.MenuItems.Add("-");
			MenuItem catItem;
			InterfaceFile file=null;
			if (SelectedItems.Count>0)
				file=(InterfaceFile)SelectedItems[0].Tag;
			
			if ((file!=null)&&(file.CategoryID==0))
				noneItem.Checked=true;

			foreach (CCategory category in categories)
			{
				catItem=CategoryMenu.MenuItems.Add(category.Name, new EventHandler(OnSelectCategory));
				if ((file!=null)&&(file.CategoryID==category.ID)) catItem.Checked=true;
			}

			if (categories.Count>0) CategoryMenu.MenuItems.Add("-");
			CategoryMenu.MenuItems.Add(eLePhantForm.Globalization["LBL_MANAGECATEGORIES"], new EventHandler(OnAddCategory));

		}

		private void OnSelectCategory(object sender, System.EventArgs e)
		{
			MenuItem item=(MenuItem)sender;
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileCategory(((InterfaceFile)SelectedItems[i].Tag).strHash,item.Text);
			}
		}
		private void OnAddCategory(object sender, System.EventArgs e)
		{
			FormCategory form=new FormCategory();
			form.Initilize(krnGateway);
			form.ShowDialog();
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		public class ColumnSorter : IComparer
		{
			public int CurrentColumn = -1;
			public int sortOrder;
			public int Compare(object x, object y)
			{
				if ((x==null)||(y==null)) return 0;
				ListViewItem rowA = (ListViewItem)x;
				ListViewItem rowB = (ListViewItem)y;
				switch(CurrentColumn)
				{
					case 1:
						if ((((InterfaceFile)rowA.Tag).Size)>(((InterfaceFile)rowB.Tag).Size))
								return sortOrder;
							else
								return -sortOrder;
					case 2:
						if (((((InterfaceFile)rowA.Tag).DownSpeed)>(((InterfaceFile)rowB.Tag).DownSpeed)))
								return -sortOrder;
							else
								return sortOrder;
					case 3:
						if ((((InterfaceFile)rowA.Tag).BytesDownloaded)>(((InterfaceFile)rowB.Tag).BytesDownloaded))
								return sortOrder;
							else
								return -sortOrder;
					case 4:
						if ((((InterfaceFile)rowA.Tag).RemainingBytes)>(((InterfaceFile)rowB.Tag).RemainingBytes))
								return sortOrder;
							else
								return -sortOrder;						
					case 5:
						if ((((InterfaceFile)rowA.Tag).nSources)>(((InterfaceFile)rowB.Tag).nSources))
								return sortOrder;
							else
								return -sortOrder;	
					case 6:
						if ((((InterfaceFile)rowA.Tag).nValidSources)>(((InterfaceFile)rowB.Tag).nValidSources))
								return sortOrder;
							else
								return -sortOrder;	
					case 7:
						if ((((InterfaceFile)rowA.Tag).nTransferringSources)>(((InterfaceFile)rowB.Tag).nTransferringSources))
								return sortOrder;
							else
								return -sortOrder;
					case 9:
						if (((InterfaceFile)rowA.Tag).PercentCompleted>((InterfaceFile)rowB.Tag).PercentCompleted)
							return sortOrder;
						else
							return -sortOrder;

					case 10:
						
						byte prioA=(byte)((byte)((InterfaceFile)rowA.Tag).DownloadPriority);
						byte prioB=(byte)((byte)((InterfaceFile)rowB.Tag).DownloadPriority);
						
						if (prioA<prioB)
							return -sortOrder;
						else if (prioA>prioB) 
							return sortOrder;
						else 
							return String.Compare(rowA.Text,rowB.Text);

					default:
						return sortOrder*String.Compare(rowA.SubItems[CurrentColumn].Text,
								rowB.SubItems[CurrentColumn].Text);

				}
				 
			}

			public ColumnSorter()
			{

			}

		}

		
	}
}
