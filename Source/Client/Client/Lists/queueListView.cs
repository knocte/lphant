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
using System.Threading;
using System.Drawing;
using eLePhant.eDonkey;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for queueListView.
	/// </summary>
	public class queueListView : eLePhantListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private ContextMenu ContextMenuUploads;
		private ColumnSorter TheColumnSorter;
		private CkernelGateway krnGateway;

		public void Initilize(CkernelGateway in_krnGateway)
		{
			krnGateway=in_krnGateway;
			Columns.Add("Name",90,false);
			Columns.Add("File Name",190,false);
			Columns.Add("Uploaded",60,false);
			Columns.Add("Downloaded",60,false);
			Columns.Add("Status",60,false);
			Columns.Add("Progress",120,false);
			Columns.Add("Software",60,false);
			Columns.Add("Version",60,false);

			ContextMenuUploads = new ContextMenu();
			ContextMenuUploads.MenuItems.Add("Send message",new EventHandler(OnSendMessage));
			ContextMenuUploads.MenuItems.Add("View files",new EventHandler(OnViewFiles));
			ContextMenuUploads.MenuItems.Add("Add to friends",new EventHandler(OnAddFriend));
			
			ContextMenu=ContextMenuUploads;
			TheColumnSorter=new ColumnSorter();
			this.ClickHeader+=new HeaderEventHandler(miColumnClick);
			m_DefaultWidths=new int[] {90,190,60,60,60,190,60,60};
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

		}

		private void OnAddFriend(object sender, System.EventArgs e)
		{

		}

		private void OnViewFiles(object sender, System.EventArgs e)
		{

		}
		protected override void OnDrawItem(int item,int column,Graphics g,Rectangle rec)
		{
			InterfaceClient source=(InterfaceClient)(Items[item].Tag);
			if (source.UpFileChunks==null) return;
			SolidBrush b=new SolidBrush(Color.Red);
			int xpos=rec.Left;

			int ChunkLength;
			
			for (int i=0;i<source.UpFileChunks.Length;i++)
			{
				ChunkLength=(int)Decimal.Round((decimal)(rec.Right-xpos)/(decimal)(source.UpFileChunks.Length-i),0);
				switch(source.UpFileChunks[i])
				{
					case 3: b.Color=Color.Black;
							break;
					case 0: b.Color=Color.Red;
							break;
//					case 1: b.Color=Color.DarkBlue;
//							break;
//					case 2: b.Color=Color.Green;
//							break;
					default: b.Color=Color.Red;
							break;
				}

				g.FillRectangle(b,xpos,rec.Top,ChunkLength,rec.Height);
				xpos+=ChunkLength;
			}
		}

		private void ClientToItem(InterfaceClient client,ListViewItem itemUpload)
		{
			if (client==null) return;
			if (itemUpload.SubItems[0].Text!=client.Name) 
				itemUpload.SubItems[0].Text=client.Name;
			if (itemUpload.SubItems[1].Text!=client.UpFileName) 
				itemUpload.SubItems[1].Text=client.UpFileName;
			if (itemUpload.SubItems[2].Text!=SizeToString(client.UploadedBytes)) 
				itemUpload.SubItems[2].Text=SizeToString(client.UploadedBytes);
			if (itemUpload.SubItems[3].Text!=SizeToString(client.DownloadedBytes)) 
				itemUpload.SubItems[3].Text=SizeToString(client.DownloadedBytes);
			if (itemUpload.SubItems[4].Text!=StatusToString(client.UploadState)) 
				itemUpload.SubItems[4].Text=StatusToString(client.UploadState);
			if (itemUpload.SubItems[5].Text!="") 
				itemUpload.SubItems[5].Text=""; //progress
			if (itemUpload.SubItems[6].Text!=client.Software) 
				itemUpload.SubItems[6].Text=client.Software;
			if (itemUpload.SubItems[7].Text!=client.Version.ToString()) 
				itemUpload.SubItems[7].Text=client.Version.ToString();
		}
		public void ReloadList(byte[] fileHashFilter)
		{
			BeginUpdate();

			Items.Clear();
			InterfaceClient[] clientes=krnGateway.GetQueuedSourcesList(fileHashFilter);
			if ((clientes==null)||(clientes.Length==0)) 
			{
				EndUpdate();
				return;
			}
			ListViewItem[] listaItems=new ListViewItem[clientes.Length];
			int i=0;
			foreach (InterfaceClient cliente in clientes)
			{
				listaItems[i]=new ListViewItem(new string[] {"","","","","","","",""});
				ClientToItem(cliente,listaItems[i]);
				listaItems[i].Tag=cliente;
				//Items.Add(listaItems[i]);
				i++;
			}
			Items.AddRange(listaItems);
			EndUpdate();
		}
		private string StatusToString(byte status)
		{
			switch(status){
				case 2:return "On queue";
				case 1:return "Pending";
				case 0:return "None";
				case 4:return "Uploading";
				case 8:return "Connecting (LowID)";
				case 9:return "Connecting";
				default: return "Unknown";
			}
		}
		public queueListView(System.ComponentModel.IContainer container)
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

		public queueListView()
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
						if ((((InterfaceClient)rowA.Tag).DownloadedBytes)>(((InterfaceClient)rowB.Tag).DownloadedBytes))
								return sortOrder;
							else
								return -sortOrder;
					case 3:
						if ((((InterfaceClient)rowA.Tag).UploadedBytes)>(((InterfaceClient)rowB.Tag).UploadedBytes))
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
