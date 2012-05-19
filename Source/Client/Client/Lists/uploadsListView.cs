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
	/// Summary description for uploadsListView.
	/// </summary>
	public class uploadsListView : eLePhantListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private CkernelGateway krnGateway;
		private ContextMenu ContextMenuUploads;
		private ArrayList itemsToRemove;
		private ColumnSorter TheColumnSorter;
		private System.ComponentModel.Container components = null;
		private string[] m_States;
		
		public void Initilize(CkernelGateway in_krnGateway)
		{
			Name = "uploadsList";
			krnGateway=in_krnGateway;
			Columns.Add("Name",90,false);
			Columns.Add("File Name",190,false);
			Columns.Add("Speed",60,false);
			Columns.Add("Uploaded",60,false);
			Columns.Add("Downloaded",60,false);
			Columns.Add("Status",60,false);
			Columns.Add("Progress",120,true);
			Columns.Add("Software",60,false);
			Columns.Add("Version",60,false);
			krnGateway.OnNewUpload+=new ClientEvent(m_InvokeOnNewUpload);
			krnGateway.OnRefreshUploads+=new RefreshEvent(m_InvokeRefreshList);

			ContextMenuUploads = new ContextMenu();
			ContextMenuUploads.MenuItems.Add("Send message",new EventHandler(OnSendMessage));
			ContextMenuUploads.MenuItems.Add("View files",new EventHandler(OnViewFiles));
			ContextMenuUploads.MenuItems.Add("Add to friends",new EventHandler(OnAddFriend));
			ContextMenuUploads.Popup+=new System.EventHandler(myContextMenuPopup);

			ContextMenu=ContextMenuUploads;
			TheColumnSorter=new ColumnSorter();
			this.ClickHeader+=new HeaderEventHandler(miColumnClick);
			this.DoubleClick+=new EventHandler(OnSendMessage);
			m_DefaultWidths=new int[] {90,190,60,60,60,60,190,60,60};
			m_LoadWidths();
			m_Globalize();
		}
		
		private void m_Globalize()
		{
			Columns[0].Text=eLePhantForm.Globalization["LBL_NAME"];
			Columns[1].Text=eLePhantForm.Globalization["LBL_FILENAME"];
			Columns[2].Text=eLePhantForm.Globalization["LBL_SPEED"];
			Columns[3].Text=eLePhantForm.Globalization["LBL_UPLOADED"];
			Columns[4].Text=eLePhantForm.Globalization["LBL_DOWNLOADED"];
			Columns[5].Text=eLePhantForm.Globalization["LBL_STATUS"];
			Columns[6].Text=eLePhantForm.Globalization["LBL_PROGRESS"];
			Columns[7].Text=eLePhantForm.Globalization["LBL_SOFTWARE"];
			Columns[8].Text=eLePhantForm.Globalization["LBL_VERSION"];
			ContextMenuUploads.MenuItems[0].Text=eLePhantForm.Globalization["LBL_SENDMESSAGE"];
			ContextMenuUploads.MenuItems[1].Text=eLePhantForm.Globalization["LBL_VIEWFILES"];
			ContextMenuUploads.MenuItems[2].Text=eLePhantForm.Globalization["LBL_ADDFRIEND"];
			m_States=new string[] {
									  eLePhantForm.Globalization["LBL_NONE"],
									  eLePhantForm.Globalization["LBL_PENDING"],
									  eLePhantForm.Globalization["LBL_ONQUEUE"],
									  "",
									  eLePhantForm.Globalization["LBL_UPLOADING"],
									  "","","",
									  eLePhantForm.Globalization["LBL_CONNECTING_LOWID"],
									  eLePhantForm.Globalization["LBL_CONNECTING"]
									};
		}
	
		public void Globalize()
		{
			m_Globalize();
			this.Refresh();
		}

		private void myContextMenuPopup(object sender, System.EventArgs e)
		{											
			ContextMenuUploads.MenuItems[0].DefaultItem=true;
			if (SelectedItems.Count==0)
			{
				ContextMenuUploads.MenuItems[0].Enabled=false;
				ContextMenuUploads.MenuItems[1].Enabled=false;
				ContextMenuUploads.MenuItems[2].Enabled=false;
			}
			if (SelectedItems.Count==1)
			{								
				ContextMenuUploads.MenuItems[0].Enabled=true;
				ContextMenuUploads.MenuItems[1].Enabled=true;
				ContextMenuUploads.MenuItems[2].Enabled=true;	
			}

			if (SelectedItems.Count>1)
			{
				ContextMenuUploads.MenuItems[0].Enabled=false;
				ContextMenuUploads.MenuItems[1].Enabled=false;
				ContextMenuUploads.MenuItems[2].Enabled=false;	
			}
		}

		private void miColumnClick(object sender, HeaderEventArgs e)
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

		private void OnViewFiles(object sender, System.EventArgs e)
		{
			if (this.SelectedItems.Count<=0) return;
			InterfaceClient source=(InterfaceClient)SelectedItems[0].Tag;
			krnGateway.RequestSharedList(source.ID,source.Port,source.ServerIP,source.UserHash);
		}
		private void OnAddFriend(object sender, System.EventArgs e)
		{
			if (this.SelectedItems.Count<=0) return;
			InterfaceClient source=(InterfaceClient)SelectedItems[0].Tag;
			krnGateway.AddFriend(source);
		}
		protected override void OnDrawItem(int item,int column,Graphics g,Rectangle rec)
		{
			InterfaceClient source=(InterfaceClient)(Items[item].Tag);
			
			Color b=Color.Red;
			Color end;
			LinearGradientBrush chunkBrush;

			int xpos = rec.Left;
			int ChunkLength;
			
			if (source.UpFileChunks==null) 
			{
				//draw empty rect
				g.FillRectangle(new SolidBrush(this.BackColor),rec);
				return;
			}

			for (int i=0;i<source.UpFileChunks.Length;i++)
			{
				//ChunkLength=(int)Decimal.Round((decimal)(rec.Right-xpos)/(decimal)(source.PartesFicheroSubida.Length-i),0);
				ChunkLength=(int)Decimal.Round((decimal)(rec.Right-xpos)/(decimal)(source.UpFileChunks.Length-i),0);
				switch(source.UpFileChunks[i])
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
					
					g.FillRectangle(chunkBrush, xpos, rec.Top, ChunkLength, rec.Height);
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

		private void m_InvokeOnNewUpload(InterfaceClient client)
		{
			this.BeginInvoke(new ClientEvent(m_OnNewUpload),new object[] {client});
		}
		
		private void m_OnNewUpload(InterfaceClient client)
		{
			//we fist check if the upload is in the list, possible when interface is sleeping and a deleted source
			//is added again (the deleted source was not really removed because interface was sleeping
			foreach (ListViewItem item in Items)
			{
				if ((((InterfaceClient)item.Tag).ID==client.ID)&&
					(((InterfaceClient)item.Tag).Port==client.Port))
					return;
			}
			
			ListViewItem itemUpload=new ListViewItem(new string[]{"","","","","","","","",""});
			
			itemUpload.Tag=client;
			ClientToItem(client,itemUpload);
			Items.Add(itemUpload);
		}
		private string StatusToString(byte status)
		{
//  		public const byte   US_NONE=0;
//        public const byte 	US_PENDING=1;
//        public const byte 	US_ONUPLOADQUEUE=2;
//        public const byte 	US_UPLOADING=4;
//        public const byte   US_BANNED=6;
//        public const byte   US_ERROR=7;
//        public const byte 	US_WAITCALLBACK=8;
//        public const byte 	US_CONNECTING=9;
//        public const byte 	US_LOWTOLOWIP=10;
			
			if (status>m_States.Length) return "Unknown";
			return m_States[status];
		}
		
		private void ClientToItem(InterfaceClient client,ListViewItem itemUpload)
		{
			if (client==null) return;
			if (itemUpload.SubItems[0].Text!=client.Name) 
				itemUpload.SubItems[0].Text=client.Name;
			if (itemUpload.SubItems[1].Text!=client.UpFileName) 
				itemUpload.SubItems[1].Text=client.UpFileName;
			if (itemUpload.SubItems[2].Text!=SpeedToString(client.UploadSpeed)) 
				itemUpload.SubItems[2].Text=SpeedToString(client.UploadSpeed);
			if (itemUpload.SubItems[3].Text!=SizeToString(client.UploadedBytes)) 
				itemUpload.SubItems[3].Text=SizeToString(client.UploadedBytes);
			if (itemUpload.SubItems[4].Text!=SizeToString(client.DownloadedBytes)) 
				itemUpload.SubItems[4].Text=SizeToString(client.DownloadedBytes);
			if (itemUpload.SubItems[5].Text!=StatusToString(client.UploadState)) 
				itemUpload.SubItems[5].Text=StatusToString(client.UploadState);
			if (itemUpload.SubItems[6].Text!="") 
				itemUpload.SubItems[6].Text=""; //progress
			if (itemUpload.SubItems[7].Text!=client.Software) 
				itemUpload.SubItems[7].Text=client.Software;
//			if (client.Software=="eMule")
//			{
//				if (itemUpload.SubItems[8].Text!=client.Version.ToString("X")) 
//					itemUpload.SubItems[8].Text=client.Version.ToString("X");
//			}
//			else
//			{
			float div=100F;
			if (client.Software=="eDonkeyHybrid") div=1000F;	
			if (itemUpload.SubItems[8].Text!=(client.Version/div).ToString("0.00")) 
					itemUpload.SubItems[8].Text=(client.Version/div).ToString("0.00");
//			}
			itemUpload.Tag=client;
		}
		private void m_InvokeRefreshList(CkernelGateway in_krnGateway)
		{
			this.BeginInvoke(new RefreshEvent(m_RefreshList),new object[] {in_krnGateway});
		}
		private void m_RefreshList(CkernelGateway in_krnGateway)
		{
			int nItem=0;
			itemsToRemove.Clear();
			while (nItem<Items.Count)
			{
				ListViewItem Item=Items[nItem];
				if (Item==null) break;
				InterfaceClient clientant=(InterfaceClient)Item.Tag;
				InterfaceClient client=in_krnGateway.GetUploadClient(clientant.ID,clientant.Port,clientant.ServerIP,clientant.UserHash);
				if (client==null) itemsToRemove.Add(Item);//Items.RemoveAt(nItem);
				else ClientToItem(client,Item);
				nItem++;
			}
			foreach (ListViewItem Item in itemsToRemove)
			{
				this.Items.Remove(Item);
			}
		}

		public uploadsListView(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			itemsToRemove=new ArrayList();
			container.Add(this);
			InitializeComponent();
			View = System.Windows.Forms.View.Details;
		}

		public uploadsListView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
			View = System.Windows.Forms.View.Details;			
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
						if ((((InterfaceClient)rowA.Tag).UploadSpeed)>(((InterfaceClient)rowB.Tag).UploadSpeed))
								return sortOrder;
							else
								return -sortOrder;
					case 3:
						if ((((InterfaceClient)rowA.Tag).UploadedBytes)>(((InterfaceClient)rowB.Tag).UploadedBytes))
							return sortOrder;
						else
							return -sortOrder;	
					
					case 4:
						if ((((InterfaceClient)rowA.Tag).DownloadedBytes)>(((InterfaceClient)rowB.Tag).DownloadedBytes))
								return sortOrder;
							else
								return -sortOrder;
					case 6:
						int a=0;
						int b=0;
						if (((InterfaceClient)rowA.Tag).UpFileChunks==null) return -sortOrder;
						if (((InterfaceClient)rowB.Tag).UpFileChunks==null) return sortOrder;
						foreach (byte ap in ((InterfaceClient)rowA.Tag).UpFileChunks)
						{
							a+=ap;
						}
						foreach (byte bp in ((InterfaceClient)rowB.Tag).UpFileChunks)
						{
							b+=bp;
						}
						if (a>b)
							return sortOrder;
						else
							return -sortOrder;	
					case 8:
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
