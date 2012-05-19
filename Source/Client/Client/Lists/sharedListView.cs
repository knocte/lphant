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
using System.Drawing.Drawing2D;
using eLePhant.eDonkey;
using eLePhant.Types;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for sharedListView.
	/// </summary>
	public class sharedListView : eLePhantListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private CkernelGateway krnGateway;
		private ContextMenu ContextMenuShared;
		private ColumnSorter TheColumnSorter;
		private MenuItem PriorityMenu;
		private InterfacePreferences m_config;
		
		public CMenuInfo MenuInfo;

		private System.ComponentModel.Container components = null;

		public sharedListView(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public sharedListView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		public void Initilize(CkernelGateway in_krnGateway)
		{
			Name = "sharedListView";
			krnGateway=in_krnGateway;
			krnGateway.OnRefreshShared+=new RefreshEvent(m_InvokeOnRefreshShared);
			krnGateway.OnNewSharedFile+=new FileEvent(m_InvokeOnNewSharedFile);
			krnGateway.OnRemovedSharedFile+=new FileEvent(m_InvokeOnRemovedSharedFile);

			Columns.Add("File Name",190,false);
			Columns.Add("Size",75,false);
			Columns.Add("Priority",60,false);
			Columns.Add("Progress",60,true);
			
			ContextMenuShared = new ContextMenu();
			ContextMenuShared.MenuItems.Add("Open file",new EventHandler(OnOpenFile));
			ContextMenuShared.MenuItems.Add("Delete file",new EventHandler(OnDeleteFile));
			ContextMenuShared.MenuItems.Add("-");
			ContextMenuShared.MenuItems.Add("Copy eLink to clipboard",new EventHandler(OnCopyLink));
			ContextMenuShared.MenuItems.Add("Copy eLink to clipboard (HTML)", new EventHandler(OnCopyLinkHTML));
			ContextMenuShared.MenuItems.Add("Copy eLink to clipboard (Sources)", new EventHandler(OnCopyLinkSources));
			ContextMenuShared.MenuItems.Add("-");
			
			PriorityMenu = new MenuItem( "&Priority"); 
			
			ContextMenuShared.MenuItems.Add(PriorityMenu);
			PriorityMenu.MenuItems.Add("Very high Priority",new EventHandler(OnReleasePriority));
			PriorityMenu.MenuItems.Add("High Priority",new EventHandler(OnHighPriority));
			PriorityMenu.MenuItems.Add("Normal Priority",new EventHandler(OnNormalPriority));
			PriorityMenu.MenuItems.Add("Low Priority",new EventHandler(OnLowPriority));
			PriorityMenu.MenuItems.Add("Very low Priority",new EventHandler(OnVeryLowPriority));

			ContextMenuShared.MenuItems.Add("-");
			MenuItem MenuItemInfo=new MenuItem("Search more info");
			ContextMenuShared.MenuItems.Add(MenuItemInfo);
			MenuInfo=new CMenuInfo(MenuItemInfo);

			ContextMenu=ContextMenuShared;
			ContextMenuShared.Popup+=new System.EventHandler(myContextMenuPopup);
			
			TheColumnSorter=new ColumnSorter();
			TheColumnSorter.CurrentColumn=1;
			TheColumnSorter.sortOrder=-1;

			this.ClickHeader+=new HeaderEventHandler(myColumnClick);
			this.DoubleClick+=new EventHandler(OnOpenFile);
			this.KeyDown+=new KeyEventHandler(OnKeyDown);

			m_DefaultWidths=new int[] {190,75,60,60};
			m_Globalize();
			m_LoadWidths();
			SmallImageList=eLePhantForm.SystemIconsList.list;
			SelectedIndexChanged+=new EventHandler(sharedListView_SelectedIndexChanged);
			ReloadList(false);
		}
		
		private void m_Globalize()
		{
			Columns[0].Text=eLePhantForm.Globalization["LBL_FILENAME"];
			Columns[1].Text=eLePhantForm.Globalization["LBL_SIZE"];
			Columns[2].Text=eLePhantForm.Globalization["LBL_PRIORITY"];
			Columns[3].Text=eLePhantForm.Globalization["LBL_PROGRESS"];
			ContextMenuShared.MenuItems[0].Text=eLePhantForm.Globalization["LBL_OPENFILE"];
			ContextMenuShared.MenuItems[1].Text=eLePhantForm.Globalization["LBL_DELETEFILE"];
			ContextMenuShared.MenuItems[3].Text=eLePhantForm.Globalization["LBL_COPYLINK"];
			ContextMenuShared.MenuItems[4].Text=eLePhantForm.Globalization["LBL_COPYLINK"] + " (HTML)";
			ContextMenuShared.MenuItems[5].Text=eLePhantForm.Globalization["LBL_COPYLINK"] + " ("+eLePhantForm.Globalization["LBL_SOURCES"]+")";
			ContextMenuShared.MenuItems[7].Text=eLePhantForm.Globalization["LBL_PRIORITY"];
			ContextMenuShared.MenuItems[9].Text=eLePhantForm.Globalization["LBL_SEARCHINFO"];
			PriorityMenu.MenuItems[0].Text=eLePhantForm.Globalization["LBL_VERYHIGH"];
			PriorityMenu.MenuItems[1].Text=eLePhantForm.Globalization["LBL_HIGH"];
			PriorityMenu.MenuItems[2].Text=eLePhantForm.Globalization["LBL_NORMAL"];
			PriorityMenu.MenuItems[3].Text=eLePhantForm.Globalization["LBL_LOW"];
			PriorityMenu.MenuItems[4].Text=eLePhantForm.Globalization["LBL_VERYLOW"];
		}

		public void Globalize()
		{
			m_Globalize();
			this.Refresh();
		}

		private void myContextMenuPopup(object sender, System.EventArgs e)
		{
			m_config=krnGateway.GetConfig();
			ContextMenuShared.MenuItems[0].DefaultItem = true;
			m_CheckPriority();
			MenuInfo.OnMenuInfoChange();
									
			if (SelectedItems.Count==0)
			{
				ContextMenuShared.MenuItems[0].Enabled=false;
				ContextMenuShared.MenuItems[1].Enabled=false;
				ContextMenuShared.MenuItems[3].Enabled=false;
				ContextMenuShared.MenuItems[4].Enabled=false;
				ContextMenuShared.MenuItems[5].Enabled=false;
				ContextMenuShared.MenuItems[7].Enabled=false;
				ContextMenuShared.MenuItems[9].Enabled=false;
				PriorityMenu.MenuItems[0].Enabled=false;
				PriorityMenu.MenuItems[1].Enabled=false;
				PriorityMenu.MenuItems[2].Enabled=false;
				PriorityMenu.MenuItems[3].Enabled=false;
				PriorityMenu.MenuItems[4].Enabled=false;
				
			}
			if (SelectedItems.Count==1)
			{								
				ContextMenuShared.MenuItems[0].Enabled=m_IsCompleted();
				ContextMenuShared.MenuItems[1].Enabled=m_IsCompleted();
				ContextMenuShared.MenuItems[3].Enabled=true;
				ContextMenuShared.MenuItems[4].Enabled=true;
				ContextMenuShared.MenuItems[5].Enabled=m_config.IsHighID;
				ContextMenuShared.MenuItems[7].Enabled=true;
				ContextMenuShared.MenuItems[9].Enabled=true;
				PriorityMenu.MenuItems[0].Enabled=true;
				PriorityMenu.MenuItems[1].Enabled=true;
				PriorityMenu.MenuItems[2].Enabled=true;
				PriorityMenu.MenuItems[3].Enabled=true;
				PriorityMenu.MenuItems[4].Enabled=true;
			}

			if (SelectedItems.Count>1)
			{
				ContextMenuShared.MenuItems[0].Enabled=false;
				ContextMenuShared.MenuItems[1].Enabled=true;
				ContextMenuShared.MenuItems[3].Enabled=false;
				ContextMenuShared.MenuItems[4].Enabled=false;
				ContextMenuShared.MenuItems[5].Enabled=false;
				ContextMenuShared.MenuItems[7].Enabled=true;
				ContextMenuShared.MenuItems[9].Enabled=false;
				PriorityMenu.MenuItems[0].Enabled=true;
				PriorityMenu.MenuItems[1].Enabled=true;
				PriorityMenu.MenuItems[2].Enabled=true;
				PriorityMenu.MenuItems[3].Enabled=true;
				PriorityMenu.MenuItems[4].Enabled=true;
			}
		}
		
		private void OnKeyDown(object sender, KeyEventArgs e)
		{			
			switch(e.KeyCode)
			{
				case Keys.Delete: 
					m_OnDeleteFile();
					break;

				case Keys.Enter: 
					m_OnOpenFile();
					break;
			}
		}

		private void m_OnOpenFile()
		{
			if (SelectedItems.Count==0) return;
			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			if(file.Status==9)
			{
				try
				{
					System.Diagnostics.Process.Start(file.CompleteName);
				}
				catch{}
			}
		}

		private void OnOpenFile(object sender, System.EventArgs e)
		{
			m_OnOpenFile();	
		}
				
		private void m_OnDeleteFile()
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
					if(Items[i].Status!=9) continue;
					string message = String.Format(eLePhantForm.Globalization["MSG_DELETEFILE"],Items[i].Name);
					if (MessageBox.Show(message,"lphant",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)
					{						
						krnGateway.DeleteFile(Items[i].strHash);
						ReloadList(true);
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private void OnDeleteFile(object sender, System.EventArgs e)
		{
			m_OnDeleteFile();
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

		private void OnCopyLinkSources(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			
			string IP = (new System.Net.IPAddress(m_config.UserID)).ToString();
			Clipboard.SetDataObject("ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strHash+"|/|sources,"+IP+":"+m_config.TCPPort+"|/");
		}

		private void OnReleasePriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileUploadPrioriry(((InterfaceFile)SelectedItems[i].Tag).strHash,Types.Constants.Priority.VeryHigh);
			}
			m_RefreshList();
		}
		private void OnHighPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileUploadPrioriry(((InterfaceFile)SelectedItems[i].Tag).strHash,Types.Constants.Priority.High);
			}
			m_RefreshList();
		}
		private void OnNormalPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileUploadPrioriry(((InterfaceFile)SelectedItems[i].Tag).strHash,Types.Constants.Priority.Normal);
			}
			m_RefreshList();
		}
		private void OnLowPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileUploadPrioriry(((InterfaceFile)SelectedItems[i].Tag).strHash,Types.Constants.Priority.Low);
			}
			m_RefreshList();
		}
		private void OnVeryLowPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				krnGateway.SetFileUploadPrioriry(((InterfaceFile)SelectedItems[i].Tag).strHash,Types.Constants.Priority.VeryLow);
			}
			m_RefreshList();
		}

		private void myColumnClick(object sender, HeaderEventArgs e)
		{
			TheColumnSorter.CurrentColumn = e.ColumnIndex;
			TheColumnSorter.sortOrder=this.sortOrder;
			this.ListViewItemSorter = TheColumnSorter;	
			this.Sort();
			this.ListViewItemSorter = null;			
		}

		private void m_OnNewShared(InterfaceFile shared)
		{
			ListViewItem ItemShared =new ListViewItem(new string[]{"","","",""});
			ItemShared.Tag=shared;
			SharedToItem(shared,ItemShared);
			Items.Add(ItemShared);
		}

		private void m_OnRemovedSharedFile(InterfaceFile shared)
		{
			try
			{
				for (int i=0;i<this.Items.Count;i++)
				{
					if (shared.strHash==((InterfaceFile)Items[i].Tag).strHash)
					{
						Items.RemoveAt(i);
					}
				}
			}
			catch{}
		}
		private void m_InvokeOnNewSharedFile(InterfaceFile file)
		{
			if (this.InvokeRequired)
				this.BeginInvoke(new FileEvent(m_OnNewShared),new object[] {file});
			else
				m_OnNewShared(file);
		}

		private void m_InvokeOnRemovedSharedFile(InterfaceFile file)
		{
			if (this.InvokeRequired)
				this.BeginInvoke(new FileEvent(m_OnRemovedSharedFile),new object[] {file});
			else
				m_OnRemovedSharedFile(file);
		}

		private void SharedToItem(InterfaceFile shared,ListViewItem ItemShared)
		{
			if (shared==null) return;
			if (ItemShared.SubItems[0].Text!=shared.Name) ItemShared.SubItems[0].Text=shared.Name;
			if (ItemShared.SubItems[1].Text!=SizeToString(shared.Size)) ItemShared.SubItems[1].Text=SizeToString(shared.Size);
			if (ItemShared.SubItems[2].Text!=PriorityToString(shared.UploadPriority)) ItemShared.SubItems[2].Text=PriorityToString(shared.UploadPriority);
			ItemShared.Tag=shared;
			ItemShared.ImageIndex=eLePhantForm.SystemIconsList.GetIconIndexOf(shared.Name);
		}

		private void m_InvokeOnRefreshShared(CkernelGateway krnGateway)
		{
			this.BeginInvoke(new RefreshEvent(m_OnRefreshShared),new object[] {krnGateway});
		}

		private void m_OnRefreshShared(CkernelGateway krnGateway)
		{
			m_RefreshList();
		}

		private void m_RefreshList()
		{
			foreach (ListViewItem Item in Items)
			{
				InterfaceFile file=krnGateway.GetDownloadFile(((InterfaceFile)Item.Tag).strHash);
				if (file==null) Items.Remove(Item);
				else SharedToItem(file,Item);
			}
		}

		private bool m_IsCompleted()
		{
			if (SelectedItems.Count==0) return false;
			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			
			//status = 9 -> completed
			if(file.Status==9) 
				return true;
			else
				return false;
		}

		private void m_CheckPriority()
		{	
			PriorityMenu.MenuItems[0].Checked=false;
			PriorityMenu.MenuItems[1].Checked=false;
			PriorityMenu.MenuItems[2].Checked=false;
			PriorityMenu.MenuItems[3].Checked=false;
			PriorityMenu.MenuItems[4].Checked=false;

			if (SelectedItems.Count==0) return;
			if (SelectedItems.Count>1) return;

			InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
			byte index=2;
			
			switch(file.UploadPriority)
			{
				case Constants.Priority.VeryHigh:index=0;
					break;
				case Constants.Priority.High:index=1;
					break;
				case Constants.Priority.Normal:index=2;
					break;
				case Constants.Priority.Low:index=3;
					break;
				case Constants.Priority.VeryLow:index=4;
					break;
				default: index=2;
					break;
            }
			PriorityMenu.MenuItems[index].Checked=true;
		}

		public void ReloadList(bool reloadToServer)
		{
			if (reloadToServer) krnGateway.RefreshSharedList();
			InterfaceFile[] shareds=krnGateway.GetSharedFileList();
			Items.Clear();
			if (shareds==null) return;
			lock(this)
			{
				foreach (InterfaceFile shared in shareds)
				{
					if (shared!=null) m_OnNewShared(shared);
				}
			}
			this.ListViewItemSorter = TheColumnSorter;	
			this.Sort();
			this.ListViewItemSorter = null;
		}
		
		private void sharedListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedItems.Count>0)
			{
				InterfaceFile file=(InterfaceFile)SelectedItems[0].Tag;
				
				MenuInfo.SearchString=SelectedItems[0].Text;
				MenuInfo.FileHash=file.strHash;
				MenuInfo.eD2kLink="ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strHash+"|/";
			}
		}

		protected override void OnDrawItem(int item,int column,Graphics g,Rectangle rec)
		{

			InterfaceFile file=(InterfaceFile)(Items[item].Tag);
			LinearGradientBrush chunkBrush;
			int xpos=rec.Left;
			Color b=Color.Red;
			Color end;
			if (file.UploadChunksAvaibility==null)
			{
				
				try
				{
					// calculate the new end color based on start color
					end = ControlPaint.Dark(b, 0.3F);

					// generate the linear brush
					chunkBrush = new LinearGradientBrush(new Rectangle(xpos, rec.Top, rec.Right-xpos, rec.Height), b, end, 90);
					
					g.FillRectangle(chunkBrush,xpos,rec.Top,rec.Right-xpos,rec.Height);
					//draw a backcolor margin
					g.FillRectangle(new SolidBrush(this.BackColor),rec.X,rec.Bottom-1,rec.Width,1);

				}
				catch(Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
				return;
			}
			int ChunkLength;
			for (int i=0;i<file.UploadChunksAvaibility.Length;i++)
			{
				ChunkLength=(int)Decimal.Round((decimal)(this.HeaderRight[column]-xpos)/(decimal)(file.UploadChunksAvaibility.Length-i),0);
				b=Color.Red;
				if ((file.UploadChunksAvaibility!=null)&&(file.UploadChunksAvaibility[i]>0))
				{
					int col=file.UploadChunksAvaibility[i]*4;
					if (col>200) col=200;
					col=Math.Abs(200-col);
					
					b=Color.FromArgb(255,col,col,255);
				}
				try
				{
					// calculate the new end color based on start color
					if (ChunkLength<=0) continue;
					end = ControlPaint.Dark(b, 0.3F);

					// generate the linear brush
					chunkBrush = new LinearGradientBrush(new Rectangle(xpos, rec.Top, ChunkLength, rec.Height), b, end, 90);
					
					g.FillRectangle(chunkBrush, xpos, rec.Top, ChunkLength, rec.Height);
					//draw a backcolor margin, TODO it produces some flickering
					g.FillRectangle(new SolidBrush(this.BackColor),rec.X,rec.Bottom-1,rec.Width,1);
					
				}
				catch(Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
				xpos+=ChunkLength;
			}
		}

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
						
						byte prioA=(byte)((byte)((InterfaceFile)rowA.Tag).UploadPriority+1);
						byte prioB=(byte)((byte)((InterfaceFile)rowB.Tag).UploadPriority+1);
						
						if (prioA==5) prioA=0;
						if (prioB==5) prioB=0;

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
	}
}
