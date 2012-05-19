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

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for sourcesListView.
	/// </summary>
	public class sourcesListView : eLePhantListView// baseListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private CkernelGateway krnGateway;
		private System.ComponentModel.Container components = null;
		private ArrayList itemsToRemove;
		private ContextMenu ContextMenuSources;
		private string strFileHash;
		private  ColumnSorter TheColumnSorter;
		private int m_LastItem;
		private string[] m_States;

		public void Initilize(CkernelGateway in_krnGateway)
		{
			Name = "sourcesList";
			m_LastItem=0;
			itemsToRemove=new ArrayList();
			krnGateway=in_krnGateway;
			krnGateway.OnNewSource+=new SourceEvent(m_InvokeOnNewSource);
			krnGateway.OnRefreshSources+=new RefreshEvent(m_InvokeRefreshList);

			Columns.Add("Name",90,false);
			Columns.Add("File Name",190,false);
			Columns.Add("Speed",60,false);
			Columns.Add("Status",60,false);
			Columns.Add("Position",60,false);
			Columns.Add("Downloaded",60,false);
			Columns.Add("Uploaded",60,false);
			Columns.Add("Progress",190,true);
			Columns.Add("Software",60,false);
			Columns.Add("Version",60,false);
			ContextMenuSources = new ContextMenu();
			ContextMenuSources.MenuItems.Add("Send message",new EventHandler(OnSendMessage));
			ContextMenuSources.MenuItems.Add("View files",new EventHandler(OnViewFiles));
			ContextMenuSources.MenuItems.Add("Add to friends",new EventHandler(OnAddFriend));
			ContextMenuSources.Popup+=new System.EventHandler(myContextMenuPopup);
			TheColumnSorter=new ColumnSorter();
			TheColumnSorter.CurrentColumn=4;
			TheColumnSorter.sortOrder=1;
			
			this.ClickHeader+=new HeaderEventHandler(myColumnClick);
			this.DoubleClick+=new EventHandler(OnSendMessage);

			ContextMenu=ContextMenuSources;
			m_DefaultWidths=new int[] {90,190,60,60,60,60,60,190,60,60};
			m_LoadWidths();
			m_Globalize();
		}

		private void m_Globalize()
		{
			Columns[0].Text=eLePhantForm.Globalization["LBL_NAME"];
			Columns[1].Text=eLePhantForm.Globalization["LBL_FILENAME"];
			Columns[2].Text=eLePhantForm.Globalization["LBL_SPEED"];
			Columns[3].Text=eLePhantForm.Globalization["LBL_STATUS"];
			Columns[4].Text=eLePhantForm.Globalization["LBL_POSITION"];
			Columns[5].Text=eLePhantForm.Globalization["LBL_DOWNLOADED"];
			Columns[6].Text=eLePhantForm.Globalization["LBL_UPLOADED"];
			Columns[7].Text=eLePhantForm.Globalization["LBL_PROGRESS"];
			Columns[8].Text=eLePhantForm.Globalization["LBL_SOFTWARE"];
			Columns[9].Text=eLePhantForm.Globalization["LBL_VERSION"];
			ContextMenuSources.MenuItems[0].Text=eLePhantForm.Globalization["LBL_SENDMESSAGE"];
			ContextMenuSources.MenuItems[1].Text=eLePhantForm.Globalization["LBL_VIEWFILES"];
			ContextMenuSources.MenuItems[2].Text=eLePhantForm.Globalization["LBL_ADDFRIEND"];
			m_States=new string[] {
									  eLePhantForm.Globalization["LBL_PENDING"],
									  eLePhantForm.Globalization["LBL_DOWNLOADING"],
									  eLePhantForm.Globalization["LBL_ONQUEUE"],
									  eLePhantForm.Globalization["LBL_CONNECTING"],
									  eLePhantForm.Globalization["LBL_CONNECTING_LOWID"],
									  eLePhantForm.Globalization["LBL_NONEEDEDPARTS"],
									  eLePhantForm.Globalization["LBL_LOWIDTOLOWID"],
									  "","",
									  eLePhantForm.Globalization["LBL_ASKING"],
								  };
		}

		public void Globalize()
		{
			m_Globalize();
			this.Refresh();
		}

		private void myContextMenuPopup(object sender, System.EventArgs e)
		{	
			ContextMenuSources.MenuItems[0].DefaultItem=true;							
			if (SelectedItems.Count==0)
			{
				ContextMenuSources.MenuItems[0].Enabled=false;
				ContextMenuSources.MenuItems[1].Enabled=false;
				ContextMenuSources.MenuItems[2].Enabled=false;
			}
			if (SelectedItems.Count==1)
			{								
				ContextMenuSources.MenuItems[0].Enabled=true;
				ContextMenuSources.MenuItems[1].Enabled=true;
				ContextMenuSources.MenuItems[2].Enabled=true;	
			}

			if (SelectedItems.Count>1)
			{
				ContextMenuSources.MenuItems[0].Enabled=false;
				ContextMenuSources.MenuItems[1].Enabled=false;
				ContextMenuSources.MenuItems[2].Enabled=false;	
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
		private void OnSendMessage(object sender, System.EventArgs e)
		{
			if (this.SelectedItems.Count<=0) return;
			InterfaceClient source=(InterfaceClient)SelectedItems[0].Tag;
			krnGateway.StartChatSession(source);
		}
		private void OnAddFriend(object sender, System.EventArgs e)
		{
			if (this.SelectedItems.Count<=0) return;
			InterfaceClient source=(InterfaceClient)SelectedItems[0].Tag;
			krnGateway.AddFriend(source);
		}

		private void OnViewFiles(object sender, System.EventArgs e)
		{
			if (this.SelectedItems.Count<=0) return;
			InterfaceClient source=(InterfaceClient)SelectedItems[0].Tag;
			krnGateway.RequestSharedList(source.ID,source.Port,source.ServerIP,source.UserHash);
		}
		protected override void OnDrawItem(int item,int column,Graphics g,Rectangle rec)
		{
			InterfaceClient source=(InterfaceClient)(Items[item].Tag);
			Color b=new Color();
			LinearGradientBrush chunkBrush;
			Color end;
			//SolidBrush b=new SolidBrush(Color.Red);
			int xpos=rec.Left;
			int ChunkLength;
			if (source.DownFileChunks==null)
			{
				//draw empty rect
				g.FillRectangle(new SolidBrush(this.BackColor),rec);
				return;
			}			
			for (int i=0;i<source.DownFileChunks.Length;i++)
			{
				ChunkLength=(int)Decimal.Round((decimal)(rec.Right-xpos)/(decimal)(source.DownFileChunks.Length-i),0);
				switch(source.DownFileChunks[i])
				{
					case 3: b=Color.Black;
							break;
					case 0: b=Color.Red;
							break;
//					case 1: b=Color.DarkBlue;
//							break;
//					case 2: b=Color.Green;
//							break;
					default: b=Color.Red;
							break;
				}
				try
				{
					if (ChunkLength<=0) continue;
					// calculate the new end color based on start color
					end = ControlPaint.Dark(b, 0.3F);

					// generate the linear brush
					chunkBrush = new LinearGradientBrush(new Rectangle(xpos, rec.Top, ChunkLength, rec.Height), b, end, 90);

					g.FillRectangle(chunkBrush,xpos,rec.Top,ChunkLength,rec.Height);
					//draw a backcolor margin
					g.FillRectangle(new SolidBrush(this.BackColor),rec.X,rec.Bottom-1,rec.Width,1);
				}
				catch(Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
				xpos+=ChunkLength;
			}
		}
		
		private string StatusToString(byte status)
		{
			if (status>m_States.Length) return "Unknown";
			return m_States[status];
		}

		private void SourceToItem(InterfaceClient source,ListViewItem itemSource)
		{
			if (source==null) return;
			if (itemSource.SubItems[0].Text!=source.Name) 
				itemSource.SubItems[0].Text=source.Name;
			if (itemSource.SubItems[1].Text!=source.DownFileName) 
				itemSource.SubItems[1].Text=source.DownFileName;
			if (itemSource.SubItems[2].Text!=SpeedToString(source.DownloadSpeed)) 
				itemSource.SubItems[2].Text=SpeedToString(source.DownloadSpeed);
			if (itemSource.SubItems[3].Text!=StatusToString(source.DownloadState)) 
				itemSource.SubItems[3].Text=StatusToString(source.DownloadState);
			if (itemSource.SubItems[4].Text!=Convert.ToString(source.DownQR)) 
				itemSource.SubItems[4].Text=Convert.ToString(source.DownQR);
			if (itemSource.SubItems[5].Text!=SizeToString(source.DownloadedBytes)) 
				itemSource.SubItems[5].Text=SizeToString(source.DownloadedBytes);
			if (itemSource.SubItems[6].Text!=SizeToString(source.UploadedBytes)) 
				itemSource.SubItems[6].Text=SizeToString(source.UploadedBytes);
			if (itemSource.SubItems[7].Text!="") 
				itemSource.SubItems[7].Text=""; //progress
			if (itemSource.SubItems[8].Text!=source.Software) 
				itemSource.SubItems[8].Text=source.Software;
//			if (source.Software=="eMule")
//			{
//				if (itemSource.SubItems[9].Text!=source.Version.ToString("X")) 
//					itemSource.SubItems[9].Text=source.Version.ToString("X");
//			}
//			else
//			{
			float div=100F;
			if (source.Software=="eDonkeyHybrid") div=1000F;	
			if (itemSource.SubItems[9].Text!=(source.Version/div).ToString("0.00")) 
					itemSource.SubItems[9].Text=(source.Version/div).ToString("0.00");
//			}
			itemSource.Tag=source;
		}
		private void m_InvokeOnNewSource(InterfaceClient source,string strHashFicheroDescarga)
		{
			this.BeginInvoke(new SourceEvent(m_OnNewSource),new object[] {source,strHashFicheroDescarga});
		}
		private void m_OnNewSource(InterfaceClient source,string strHashFicheroDescarga)
		{
			if (strHashFicheroDescarga!=strFileHash) return;
			ListViewItem itemSource=new ListViewItem(new string[]{"","","","","","","","","",""});
			
			itemSource.Tag=source;
			SourceToItem(source,itemSource);
			Items.Add(itemSource);
		}

		private void m_InvokeRefreshList(CkernelGateway in_krnGateway)
		{
			this.Invoke(new RefreshEvent(m_RefreshList),new object[] {in_krnGateway});
		}
		private void m_RefreshList(CkernelGateway in_krnGateway)
		{
			//refresh 30 items each second
			itemsToRemove.Clear();
			int nRefreshed=0;
			int nItem=0;
			bool refresh;
			ListViewItem Item;
			while (nItem<Items.Count)
			{
				Item=Items[nItem];
				if (Item==null) break;
				InterfaceClient sourceant=(InterfaceClient)Item.Tag;

				refresh=false;
				if ((nItem>=m_LastItem)&&(nItem<=m_LastItem+30)) //only refresh not queued sources
				{
					refresh=true;
				}
				if ((sourceant.DownloadState!=2)&&(sourceant.DownloadState!=6))
					refresh=true;
				if (refresh)
				{
					nRefreshed++;
					InterfaceClient source=in_krnGateway.GetDownloadClient(sourceant.ID,sourceant.Port,sourceant.ServerIP,sourceant.UserHash,strFileHash);
					if (source==null) itemsToRemove.Add(Item);
						else SourceToItem(source,Item);
				}
				nItem++;
				
			}
			m_LastItem+=30;
			if (m_LastItem>=Items.Count) m_LastItem=0;
			foreach (ListViewItem ItemRemove in itemsToRemove)
			{
				this.Items.Remove(ItemRemove);
			}
			//Debug.WriteLine("Refreshed: "+ nRefreshed.ToString() +" : "+m_LastItem.ToString());
		}
		public void ReloadList(string strHashFichero)
		{
			strFileHash=strHashFichero;
			InterfaceClient[] sources=krnGateway.GetFileSources(strHashFichero);				
			Items.Clear();
			if (sources==null) return;
			foreach (InterfaceClient source in sources)
			{
				if (source!=null) 
				{
					m_OnNewSource(source,strHashFichero);
				}
			}
			ListViewItemSorter = TheColumnSorter;	
			Sort();
			ListViewItemSorter = null;	
		}

		public sourcesListView(System.ComponentModel.IContainer container)
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

		public sourcesListView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
					case 2:
						if ((((InterfaceClient)rowA.Tag).DownloadSpeed)>(((InterfaceClient)rowB.Tag).DownloadSpeed))
								return sortOrder;
							else
								return -sortOrder;
					case 4:
								
						uint DownQRA=((InterfaceClient)rowA.Tag).DownQR;
						uint DownQRB=((InterfaceClient)rowB.Tag).DownQR;
						float DownloadSpeedA=((InterfaceClient)rowA.Tag).DownloadSpeed;
						float DownloadSpeedB=((InterfaceClient)rowB.Tag).DownloadSpeed;
						
						if ((DownQRA<DownQRB && DownQRA!=0) || (DownloadSpeedA>DownloadSpeedB)) return -sortOrder;
						if ((DownQRA>DownQRB && DownQRB!=0) || (DownloadSpeedB>DownloadSpeedA)) return sortOrder;
																		
						if (DownQRA==0 && DownQRB!=0) return 1;
						if (DownQRB==0 && DownQRA!=0) return -1;
						
						return String.Compare(rowA.Text,rowB.Text);

					case 5:
						if ((((InterfaceClient)rowA.Tag).DownloadedBytes)>(((InterfaceClient)rowB.Tag).DownloadedBytes))
								return sortOrder;
							else
								return -sortOrder;
					case 6:
						if ((((InterfaceClient)rowA.Tag).UploadedBytes)>(((InterfaceClient)rowB.Tag).UploadedBytes))
								return sortOrder;
							else
								return -sortOrder;
					case 7:
						int a=0;
						int b=0;
						if (((InterfaceClient)rowA.Tag).DownFileChunks==null) return -sortOrder;
						if (((InterfaceClient)rowB.Tag).DownFileChunks==null) return sortOrder;
						foreach (byte ap in ((InterfaceClient)rowA.Tag).DownFileChunks)
						{
							a+=ap;
						}
						foreach (byte bp in ((InterfaceClient)rowB.Tag).DownFileChunks)
						{
							b+=bp;
						}
						if (a>b)
							return sortOrder;
						else
							return -sortOrder;					
					case 9:
						if ((((InterfaceClient)rowA.Tag).Version)>(((InterfaceClient)rowB.Tag).Version))
								return sortOrder;
							else
								return -sortOrder;
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
