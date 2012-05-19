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
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using eLePhant.eDonkey;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for searchListView.
	/// </summary>
	public class searchListView : eLePhantListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private CkernelGateway krnGateway;
		private ContextMenu ContextMenuSearch;
		private System.ComponentModel.IContainer components;
		private SearchColumnSorter TheColumnSorter;
		private System.Windows.Forms.ImageList imageListImages;

		public CMenuInfo MenuInfo;
		public Color AlreadyDownloadingItemColor;
		public Color AlreadyDownloadedItemColor;

		public searchListView(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();
		}

		public searchListView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
		}
		
		public void Initilize(CkernelGateway in_krnGateway)
		{
			Name="listViewQueue";
			AlreadyDownloadingItemColor=Color.Red;
			AlreadyDownloadedItemColor=Color.Green;
			try
			{
				Font=new Font("Microsoft Sans Serif",8.25F,GraphicsUnit.Point);
			}
			catch
			{
				Font=(Font)this.Parent.Font.Clone();
			}
			krnGateway=in_krnGateway;

			Columns.Add("File Name",400,true);
			Columns.Add("Size",80,true);
			Columns.Add("Sources",60,true);
			Columns.Add("Codec",60,false);
			Columns.Add("Length",60,false);
			Columns.Add("Bitrate",60,false);
			Columns.Add("File type",90,false);

			ContextMenuSearch = new ContextMenu();
						
			ContextMenuSearch.MenuItems.Add("Download",new EventHandler(OnDownloadFile));
			ContextMenuSearch.MenuItems.Add("Download (Stopped)",new EventHandler(OnDownloadFileStopped));
			ContextMenuSearch.MenuItems.Add("-");
			ContextMenuSearch.MenuItems.Add("Copy eLink to clipboard", new EventHandler(OnCopyLink));
			ContextMenuSearch.MenuItems.Add("Copy eLink to clipboard (HTML)", new EventHandler(OnCopyLinkHTML));
			ContextMenuSearch.MenuItems.Add("-");
			MenuItem MenuItemInfo=new MenuItem("Search more info");
			ContextMenuSearch.MenuItems.Add(MenuItemInfo);
			MenuInfo=new CMenuInfo(MenuItemInfo);			
			ContextMenu=ContextMenuSearch;
			ContextMenuSearch.Popup+=new System.EventHandler(myContextMenuPopup);
			
			TheColumnSorter=new SearchColumnSorter();
			TheColumnSorter.CurrentColumn=1;
			TheColumnSorter.sortOrder=-1;
			
			this.ClickHeader+=new HeaderEventHandler(myColumnClick);
			this.DoubleClick+=new EventHandler(OnDownloadFile);
			this.KeyDown+=new KeyEventHandler(OnKeyDown);

			m_DefaultWidths=new int[] {400,80,60,60,60,60,90};
			
			m_Globalize();
			m_LoadWidths();
			this.SelectedIndexChanged+=new EventHandler(searchListView_SelectedIndexChanged);
		}

		private void m_Globalize()
		{
			Columns[0].Text=eLePhantForm.Globalization["LBL_FILENAME"];
			Columns[1].Text=eLePhantForm.Globalization["LBL_SIZE"];
			Columns[2].Text=eLePhantForm.Globalization["LBL_SOURCES"];
			Columns[6].Text=eLePhantForm.Globalization["LBL_FILETYPE"];
			Columns[3].Text=eLePhantForm.Globalization["LBL_CODEC"];
			Columns[4].Text=eLePhantForm.Globalization["LBL_LENGTH"];
			Columns[5].Text=eLePhantForm.Globalization["LBL_BITRATE"];
			ContextMenuSearch.MenuItems[0].Text=eLePhantForm.Globalization["LBL_DOWNLOAD"];
			ContextMenuSearch.MenuItems[1].Text=eLePhantForm.Globalization["LBL_DOWNLOAD"]+" ("+eLePhantForm.Globalization["LBL_STOPPED"]+")";
			ContextMenuSearch.MenuItems[3].Text=eLePhantForm.Globalization["LBL_COPYLINK"];
			ContextMenuSearch.MenuItems[4].Text=eLePhantForm.Globalization["LBL_COPYLINK"]+" (HTML)";
			ContextMenuSearch.MenuItems[6].Text=eLePhantForm.Globalization["LBL_SEARCHINFO"];
		}

		public void Globalize()
		{
			m_Globalize();
			this.Refresh();
		}

		protected override void OnDrawItem(int item,int column,Graphics g,Rectangle rec)
		{
			InterfaceSearchedFile file = (InterfaceSearchedFile)(Items[item].Tag);
			SolidBrush white = new SolidBrush(BackColor);
			SolidBrush textBrush=new SolidBrush(ForeColor);
			
			Rectangle barRect=rec;
			
			if (column==0)
			{
				barRect.X+=barRect.Height+2;
				barRect.Width-=barRect.Height+2;
			}
			ListViewItem itemFile=this.Items[item];
			
			if (this.SelectedItems.Contains(itemFile))
			{
				Brush br;
				if (this.Focused)
					br=new SolidBrush(Color.FromKnownColor(KnownColor.Highlight));
				else
					br=new SolidBrush(Color.FromKnownColor(KnownColor.InactiveCaptionText));
				g.FillRectangle(br, barRect);
			}
			else
			{
				g.FillRectangle(white, barRect);
			}
			if ((this.Focused)&&(this.SelectedItems.Contains(itemFile)))
			{
				Brush br=new SolidBrush(Color.FromKnownColor(KnownColor.HighlightText));
				g.DrawString(itemFile.SubItems[column].Text,this.Font,br,barRect,new StringFormat(StringFormatFlags.NoWrap));
			}
			else
				switch(file.ResultState)
				{
					case eLePhant.Types.Constants.SearchResultState.AlreadyDownloaded:
						g.DrawString(itemFile.SubItems[column].Text,this.Font,new SolidBrush(AlreadyDownloadedItemColor),barRect,new StringFormat(StringFormatFlags.NoWrap));
						break;
					case eLePhant.Types.Constants.SearchResultState.AlreadyDownloading:
						g.DrawString(itemFile.SubItems[column].Text,this.Font,new SolidBrush(AlreadyDownloadingItemColor),barRect,new StringFormat(StringFormatFlags.NoWrap));
						break;
					case eLePhant.Types.Constants.SearchResultState.New:
						if (file.Avaibility<40)
							g.DrawString(itemFile.SubItems[column].Text,Font,textBrush,barRect,new StringFormat(StringFormatFlags.NoWrap));
						else
						{	uint ColorAvaibility = file.Avaibility;
							if (file.Avaibility<100) ColorAvaibility=100;
							else if (file.Avaibility>200) ColorAvaibility=200;
							int col=(int)Math.Abs(200-ColorAvaibility);
							Brush b=new SolidBrush(Color.FromArgb(255,col,col,255));
							g.DrawString(itemFile.SubItems[column].Text,this.Font,b,barRect,new StringFormat(StringFormatFlags.NoWrap));
						}
						break;
				}
			if (column==0)
			{
				Image icon=eLePhantForm.SystemIconsList.GetIconImageOf(file.Name);
				if (icon!=null)
				{
					g.FillRectangle(white, barRect.Left-barRect.Height-4,barRect.Top,barRect.Height+4,barRect.Height);
					g.DrawImage(icon,barRect.Left-barRect.Height,barRect.Top,barRect.Height,barRect.Height);
				}
			}
			if ((column==2)&&(file.Completed))
			{
//				g.FillRectangle(white, barRect.Left-barRect.Height-4,barRect.Top,barRect.Height+4,barRect.Height);
//				g.DrawImage(this.imageListImages.Images[0],barRect.Left-barRect.Height,barRect.Top,barRect.Height,barRect.Height);
				g.DrawImage(this.imageListImages.Images[0],barRect.Left+g.MeasureString(itemFile.SubItems[2].Text,this.Font).Width,barRect.Top,barRect.Height,barRect.Height);
			}
		}
		
		private void myContextMenuPopup(object sender, System.EventArgs e)
		{
			ContextMenuSearch.MenuItems[0].DefaultItem=true;
			MenuInfo.OnMenuInfoChange();

			if (SelectedItems.Count==0)
			{
				ContextMenuSearch.MenuItems[0].Enabled=false;
				ContextMenuSearch.MenuItems[1].Enabled=false;
				ContextMenuSearch.MenuItems[3].Enabled=false;
				ContextMenuSearch.MenuItems[4].Enabled=false;
				ContextMenuSearch.MenuItems[6].Enabled=false;
			}

			if (SelectedItems.Count==1)
			{								
				ContextMenuSearch.MenuItems[0].Enabled=true;
				ContextMenuSearch.MenuItems[0].Enabled=true;
				ContextMenuSearch.MenuItems[3].Enabled=true;
				ContextMenuSearch.MenuItems[4].Enabled=true;
				ContextMenuSearch.MenuItems[6].Enabled=true;
			}
			if (SelectedItems.Count>1)
			{
				ContextMenuSearch.MenuItems[0].Enabled=true;
				ContextMenuSearch.MenuItems[1].Enabled=true;
				ContextMenuSearch.MenuItems[3].Enabled=false;
				ContextMenuSearch.MenuItems[4].Enabled=false;
				ContextMenuSearch.MenuItems[6].Enabled=false;
			}
		}
	
		private void OnKeyDown(object sender, KeyEventArgs e)
		{			
			switch(e.KeyCode)
			{
				case Keys.Delete: 
					m_OnDownloadFileStopped();
					break;

				case Keys.Enter: 
					m_OnDownloadFile();
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

		public void FileToItem(InterfaceSearchedFile file,ListViewItem itemFile)
		{
			if (file==null) return;
			if (itemFile.SubItems[0].Text!=file.Name) itemFile.SubItems[0].Text=file.Name;
			if (itemFile.SubItems[1].Text!=SizeToString(file.Size)) itemFile.SubItems[1].Text=SizeToString(file.Size);
			if (itemFile.SubItems[2].Text!=file.Avaibility.ToString()) itemFile.SubItems[2].Text=file.Avaibility.ToString();
			if (itemFile.SubItems[6].Text!="") itemFile.SubItems[6].Text="";
			if (itemFile.SubItems[3].Text!=file.Codec) itemFile.SubItems[3].Text=file.Codec;
			if (itemFile.SubItems[4].Text!=file.Length) itemFile.SubItems[4].Text=file.Length;
			if ((file.BitRate>0)&&(itemFile.SubItems[5].Text!=file.BitRate.ToString())) itemFile.SubItems[5].Text=file.BitRate.ToString();
			itemFile.Tag=file;
		}

		private void OnCopyLink(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			InterfaceSearchedFile file=(InterfaceSearchedFile)SelectedItems[0].Tag;
			Clipboard.SetDataObject("ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strFileHash+"|/");
		}

		private void OnCopyLinkHTML(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			InterfaceSearchedFile file=(InterfaceSearchedFile)SelectedItems[0].Tag;
			Clipboard.SetDataObject("<a href=\"ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strFileHash+"|/\">"+file.Name+"</a>");
		}

		private void m_OnDownloadFile()
		{
			int i=0;
			while (i<this.SelectedItems.Count) 
			{
				krnGateway.DownloadFile((InterfaceSearchedFile)(SelectedItems[i].Tag),(int)this.Tag);
				i++;
			}
		}

		private void OnDownloadFile(object sender, System.EventArgs e)
		{
			m_OnDownloadFile();
		}
		
		private void m_OnDownloadFileStopped()
		{
			int i=0;
			InterfaceSearchedFile file;
			while (i<this.SelectedItems.Count) 
			{
				file=(InterfaceSearchedFile)(SelectedItems[i].Tag);
				krnGateway.DownloadFile(file,(int)this.Tag);
				krnGateway.StopFile(file.strFileHash);
				i++;
			}
		}

		private void OnDownloadFileStopped(object sender, System.EventArgs e)
		{
			m_OnDownloadFileStopped();
		}

		public void OnNewFile(InterfaceSearchedFile file)
		{
			ListViewItem ItemFile =new ListViewItem(new string[]{"","","","","","",""});
			ItemFile.Tag=file;
			FileToItem(file,ItemFile);
			Items.Add(ItemFile);
			ItemFile.Tag=file;
		}

		public uint OnUpdateFile(InterfaceSearchedFile file)
		{
			int i=0;
			try
			{
				ListViewItem iFile;
				do
				{
					iFile=Items[i];
					i++;
				}
				while ((((InterfaceSearchedFile)iFile.Tag).strFileHash!=file.strFileHash)&&(i<Items.Count));
				if (i-1<Items.Count)
				{	
					uint diff = file.Avaibility-Convert.ToUInt32(iFile.SubItems[2].Text);
					FileToItem(file,iFile);
					return diff;
				}
				else
					return 0;
			}
			catch
			{
				return 0;
			}
		}

		public class SearchColumnSorter : IComparer
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
//					case 1:
//						if ((((InterfaceSearchedFile)rowA.Tag).Name)>(((InterfaceSearchedFile)rowB.Tag).Name))
//								return sortOrder;
//							else
//								return -sortOrder;
					case 1:
						if (((((InterfaceSearchedFile)rowA.Tag).Size)>(((InterfaceSearchedFile)rowB.Tag).Size)))
								return -sortOrder;
							else
								return sortOrder;
					case 2:
						if ((((InterfaceSearchedFile)rowA.Tag).Avaibility)>(((InterfaceSearchedFile)rowB.Tag).Avaibility))
								return sortOrder;
							else
								return -sortOrder;
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(searchListView));
			this.imageListImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// headerImages
			// 
			this.headerImages.ImageSize = new System.Drawing.Size(16, 16);
			this.headerImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// imageListImages
			// 
			this.imageListImages.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListImages.ImageStream")));
			this.imageListImages.TransparentColor = System.Drawing.Color.Transparent;

		}
		#endregion

		private void searchListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedItems.Count>0)
			{
				InterfaceSearchedFile file=(InterfaceSearchedFile)SelectedItems[0].Tag;
				
				MenuInfo.SearchString=SelectedItems[0].Text;
				MenuInfo.FileHash=file.strFileHash;
				MenuInfo.eD2kLink="ed2k://|file|"+file.Name+"|"+file.Size.ToString()+"|"+file.strFileHash+"|/";
			}
		}
	}
}
