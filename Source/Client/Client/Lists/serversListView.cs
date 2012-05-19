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
using System.Net;
using System.Drawing;
using eLePhant.eDonkey;
using eLePhant.Types;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for serversList.
	/// </summary>

	public class serversListView : eLePhantListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private CkernelGateway krnGateway;
		private ContextMenu ContextMenuServers; 
		private System.ComponentModel.Container components = null;
		private ServerColumnSorter TheColumnSorter;
		private MenuItem ConnectMenu; 
		private MenuItem DeleteMenu;
		private MenuItem CopyLinkMenu;
		private MenuItem CopyLinkHTMLMenu;
		private MenuItem PriorityMenu; 
		private MenuItem HighPriorityMenu; 
		private MenuItem NormalPriorityMenu; 
		private MenuItem LowPriorityMenu; 

		public serversListView(System.ComponentModel.IContainer container)
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

		public serversListView()
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
			Name="searchListView";
			krnGateway=in_krnGateway;
			krnGateway.OnNewServer+=new ServerEvent(OnNewServer);
			krnGateway.OnDeleteServer+=new IPEvent(OnDeleteServer);
			krnGateway.OnRefreshServers+=new RefreshEvent(m_OnRefreshList);

			Columns.Add("Server Name",170,false);
			Columns.Add("Address",100,false);
			Columns.Add("Failed",50,false);
			Columns.Add("Files",60,false);
            Columns.Add("Users",60,false);
			Columns.Add("Priority",50,false);

			ContextMenuServers = new ContextMenu(); 
			ConnectMenu = new MenuItem("&Connect", new EventHandler(OnConnectServer) ); 
			DeleteMenu = new MenuItem("&Delete", new EventHandler(DeleteServer) ); 
			CopyLinkMenu = new MenuItem("Copy &link", new EventHandler(OnCopyLink));
			CopyLinkHTMLMenu = new MenuItem("Copy link (&HTML)", new EventHandler(OnCopyLinkHTML)); 
			PriorityMenu = new MenuItem("&Priority"); 
			HighPriorityMenu = new MenuItem("&High priority",new EventHandler(OnHighPriority)); 
			NormalPriorityMenu = new MenuItem("&Normal priority",new EventHandler(OnNormalPriority)); 
			LowPriorityMenu = new MenuItem("&Low priority",new EventHandler(OnLowPriority)); 
			
			ContextMenuServers.MenuItems.Add(ConnectMenu);          
			ContextMenuServers.MenuItems.Add(DeleteMenu); 
			ContextMenuServers.MenuItems.Add("-");
			ContextMenuServers.MenuItems.Add(CopyLinkMenu);
			ContextMenuServers.MenuItems.Add(CopyLinkHTMLMenu);
			ContextMenuServers.MenuItems.Add("-");
			ContextMenuServers.MenuItems.Add(PriorityMenu);
			PriorityMenu.MenuItems.Add(HighPriorityMenu); 
			PriorityMenu.MenuItems.Add(NormalPriorityMenu); 
			PriorityMenu.MenuItems.Add(LowPriorityMenu); 
          
			ContextMenuServers.Popup += new EventHandler(OnContextMenuPopup); 

			ContextMenu=ContextMenuServers; 
			
			TheColumnSorter=new ServerColumnSorter();
			TheColumnSorter.CurrentColumn=4;
			TheColumnSorter.sortOrder=1;

			this.ClickHeader+=new HeaderEventHandler(myColumnClick);
			this.DoubleClick+=new EventHandler(OnConnectServer);
			this.KeyDown+=new KeyEventHandler(OnKeyDown);
			m_DefaultWidths=new int[] {170,100,50,60,60,50};
			m_Globalize();
			ReloadList();
			m_LoadWidths();
		}
		
		private void m_Globalize()
		{
			Columns[0].Text=eLePhantForm.Globalization["LBL_SERVERNAME"];
			Columns[1].Text=eLePhantForm.Globalization["LBL_ADDRESS"];
			Columns[2].Text=eLePhantForm.Globalization["LBL_FAILED"];
			Columns[3].Text=eLePhantForm.Globalization["LBL_FILES"];
			Columns[4].Text=eLePhantForm.Globalization["LBL_USERS"];
			Columns[5].Text=eLePhantForm.Globalization["LBL_PRIORITY"];
			ContextMenu.MenuItems[ContextMenuServers.MenuItems.IndexOf(ConnectMenu)].Text = eLePhantForm.Globalization["LBL_CONNECT"]; 
			ContextMenu.MenuItems[ContextMenuServers.MenuItems.IndexOf(DeleteMenu)].Text = eLePhantForm.Globalization["LBL_DELETE"]; 
			ContextMenu.MenuItems[ContextMenuServers.MenuItems.IndexOf(CopyLinkMenu)].Text = eLePhantForm.Globalization["LBL_COPYLINK"]; 
			ContextMenu.MenuItems[ContextMenuServers.MenuItems.IndexOf(CopyLinkHTMLMenu)].Text = eLePhantForm.Globalization["LBL_COPYLINK"]+" (HTML)";
			ContextMenu.MenuItems[ContextMenuServers.MenuItems.IndexOf(PriorityMenu)].Text = eLePhantForm.Globalization["LBL_PRIORITY"]; 
			PriorityMenu.MenuItems[PriorityMenu.MenuItems.IndexOf(HighPriorityMenu)].Text = eLePhantForm.Globalization["LBL_HIGH"]; 
			PriorityMenu.MenuItems[PriorityMenu.MenuItems.IndexOf(NormalPriorityMenu)].Text = eLePhantForm.Globalization["LBL_NORMAL"]; 
			PriorityMenu.MenuItems[PriorityMenu.MenuItems.IndexOf(LowPriorityMenu)].Text = eLePhantForm.Globalization["LBL_LOW"]; 
		}

		public void Globalize()
		{
			m_Globalize();
			this.Refresh();
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{			
			switch(e.KeyCode)
			{
				case Keys.Delete: 
					m_DeleteServer();
					break;

				case Keys.Enter: 
					m_OnConnectServer();
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

		private void OnDeleteServer(uint ip, ushort port)
		{
			InterfaceServer server;
			foreach (ListViewItem ItemServer in this.Items)
			{
				server=(InterfaceServer)ItemServer.Tag;
				IPAddress iptodelete = new IPAddress(ip);
				if ((IPAddress.Parse(server.IP).Equals(iptodelete)&&(server.Port==port))) 
				{
					this.Items.Remove(ItemServer);
					break;
				}
			}
		}

		private void OnCopyLink(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			InterfaceServer server=(InterfaceServer)SelectedItems[0].Tag;
			Clipboard.SetDataObject("ed2k://|server|"+server.IP+"|"+server.Port.ToString()+"|/");
		}

		private void OnCopyLinkHTML(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			InterfaceServer server=(InterfaceServer)SelectedItems[0].Tag;
			Clipboard.SetDataObject("<a href=\"ed2k://|server|"+server.IP+"|"+server.Port.ToString()+"|/\">"+server.Name+"</a>");
		}

		private void OnHighPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			
			for(int i=0;i!=SelectedItems.Count;i++)
			{			
				string[] strIPPort;
				ListViewItem ItemServer=this.SelectedItems[i];
				if (ItemServer==null) return;
				strIPPort=ItemServer.SubItems[1].Text.Split(":".ToCharArray());
				if (strIPPort.Length==0) return;
				IPAddress ip=IPAddress.Parse(strIPPort[0]);
				krnGateway.SetServerPriority(BitConverter.ToUInt32(ip.GetAddressBytes(),0),Convert.ToUInt16(strIPPort[1]),Constants.ServerPriority.High);
				InterfaceServer server=(InterfaceServer)ItemServer.Tag;
				server.Priority=Constants.ServerPriority.High;
				ServerToItem(server,ItemServer);
			}
		}
		private void OnNormalPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			
			for(int i=0;i!=SelectedItems.Count;i++)
			{
				string[] strIPPort;
				ListViewItem ItemServer=this.SelectedItems[i];
				if (ItemServer==null) return;
				strIPPort=ItemServer.SubItems[1].Text.Split(":".ToCharArray());
				if (strIPPort.Length==0) return;
				IPAddress ip=IPAddress.Parse(strIPPort[0]);
				krnGateway.SetServerPriority(BitConverter.ToUInt32(ip.GetAddressBytes(),0),Convert.ToUInt16(strIPPort[1]),Constants.ServerPriority.Normal);
				InterfaceServer server=(InterfaceServer)ItemServer.Tag;
				server.Priority=Constants.ServerPriority.Normal;
				ServerToItem(server,ItemServer);

			}
		}
		private void OnLowPriority(object sender, System.EventArgs e)
		{
			if (SelectedItems.Count==0) return;
			
			for(int i=0;i!=SelectedItems.Count;i++)
			{
			
				string[] strIPPort;
				ListViewItem ItemServer=this.SelectedItems[i];
				if (ItemServer==null) return;
				strIPPort=ItemServer.SubItems[1].Text.Split(":".ToCharArray());
				if (strIPPort.Length==0) return;
				IPAddress ip=IPAddress.Parse(strIPPort[0]);
				krnGateway.SetServerPriority(BitConverter.ToUInt32(ip.GetAddressBytes(),0),Convert.ToUInt16(strIPPort[1]),Constants.ServerPriority.Low);
				InterfaceServer server=(InterfaceServer)ItemServer.Tag;
				server.Priority=Constants.ServerPriority.Low;
				ServerToItem(server,ItemServer);
			}
		}
		
		private void m_OnConnectServer()
		{
			if (SelectedItems.Count==0) return;
			string[] strIPPort;
			ListViewItem ItemServer=this.SelectedItems[0];
			if (ItemServer==null) return;
			strIPPort=ItemServer.SubItems[1].Text.Split(":".ToCharArray());
			if (strIPPort.Length==0) return;
			IPAddress ip=IPAddress.Parse(strIPPort[0]);
			krnGateway.ConnectToServer(BitConverter.ToUInt32(ip.GetAddressBytes(),0),Convert.ToUInt16(strIPPort[1]));
		}

		private void OnConnectServer(object sender, System.EventArgs e)
		{
			m_OnConnectServer();
		}

		private void m_DeleteServer()
		{
			if (SelectedItems.Count==0) return;
			
			try
			{
				int SelectedItem = SelectedItems.Count;
				InterfaceServer[] Items= new InterfaceServer[SelectedItem];

				for(int i=0;i!=SelectedItem;i++)
				{
					Items[i] = (InterfaceServer)SelectedItems[i].Tag;
				}
						
				for(int i=0;i!=SelectedItem;i++)
				{
					try
					{
						IPAddress ip=IPAddress.Parse(Items[i].IP);
						krnGateway.DeleteServer(BitConverter.ToUInt32(ip.GetAddressBytes(),0),Items[i].Port);
					}
					catch(Exception ex)
					{
						Debug.WriteLine(ex.ToString());
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private void DeleteServer(object sender, System.EventArgs e)
		{
			m_DeleteServer();
		}

		private void ServerToItem(InterfaceServer server,ListViewItem ItemServer)
		{
			if (server==null) return;
			if (ItemServer.SubItems[0].Text!=server.Name) ItemServer.SubItems[0].Text=server.Name;
			if (ItemServer.SubItems[1].Text!=server.IP+":"+server.Port.ToString()) ItemServer.SubItems[1].Text=server.IP+":"+server.Port.ToString();
			if (ItemServer.SubItems[2].Text!=server.FailedConnections.ToString()) ItemServer.SubItems[2].Text=server.FailedConnections.ToString();
			if (ItemServer.SubItems[3].Text!=server.Files.ToString()) ItemServer.SubItems[3].Text=server.Files.ToString();
			if (ItemServer.SubItems[4].Text!=server.Users.ToString()) ItemServer.SubItems[4].Text=server.Users.ToString();
			//if (ItemServer.SubItems[5].Text!=server.LastConnection.ToShortTimeString()) ItemServer.SubItems[5].Text=server.LastConnection.ToShortTimeString();
			if (ItemServer.SubItems[5].Text!=ServerPriorityToString(server.Priority)) ItemServer.SubItems[5].Text=ServerPriorityToString(server.Priority);
			ItemServer.Tag=server;
			if (this.SmallImageList!=null) 
			{
				if (server.Priority==Types.Constants.ServerPriority.High)
					ItemServer.ImageIndex=0;
				else if (server.Priority==Types.Constants.ServerPriority.Normal)
					ItemServer.ImageIndex=1;
				else
					ItemServer.ImageIndex=2;
			}
		}		

		public void OnNewServer(InterfaceServer server)
		{
			ListViewItem ItemServer =new ListViewItem(new string[]{"","","","","",""});
			ItemServer.Tag=server;
			ServerToItem(server,ItemServer);
			Items.Add(ItemServer);
		}
		
		public void UpdateOrAddServer(InterfaceServer server)
		{
			string ipPort=server.IP+":"+server.Port.ToString();
			bool found=false;
			foreach (ListViewItem ItemServer in this.Items)
			{
				if (ItemServer.SubItems[1].Text==ipPort)
				{
					ServerToItem(server,ItemServer);
					found=true;
				}
			}
			if (!found)
				OnNewServer(server);
		}		
		
		private void m_OnRefreshList(CkernelGateway kernelGateway)
		{
			InterfaceServer[] servers=krnGateway.GetServerList();
			if (servers==null) 
			{
				Items.Clear();
				return;
			}
			foreach (InterfaceServer server in servers)
			{
				if (server!=null)
				{
					UpdateOrAddServer(server);
				}
			}
			/*not real time sorting
			ListViewItemSorter = TheColumnSorter;	
			Sort();
			ListViewItemSorter = null;*/
		}
		
		public void ReloadList()
		{
			InterfaceServer[] servers=krnGateway.GetServerList();
			Items.Clear();
			if (servers==null) return;
			foreach (InterfaceServer server in servers)
			{
				if (server!=null)
				{
					OnNewServer(server);
				}
			}
			ListViewItemSorter = TheColumnSorter;	
			Sort();
			ListViewItemSorter = null;	
		}
    
		//gczobe
		private void OnContextMenuPopup(object sender, System.EventArgs e) 
		{ 
			ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(ConnectMenu)].DefaultItem = true;
			m_CheckPriority();

			if (SelectedItems.Count==0) 
			{ 
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(ConnectMenu) ].Enabled = false; 
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(DeleteMenu) ].Enabled = false;
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(PriorityMenu) ].Enabled = false;
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(CopyLinkMenu) ].Enabled = false;
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(CopyLinkHTMLMenu) ].Enabled = false; 
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(HighPriorityMenu) ].Enabled = false; 
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(NormalPriorityMenu) ].Enabled = false; 
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(LowPriorityMenu) ].Enabled = false; 
			}    
			
			if(SelectedItems.Count==1)
			{ 
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(ConnectMenu) ].Enabled = true; 
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(DeleteMenu) ].Enabled = true;          
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(PriorityMenu) ].Enabled = true; 
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(CopyLinkMenu) ].Enabled = true;
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(CopyLinkHTMLMenu) ].Enabled = true; 
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(HighPriorityMenu) ].Enabled = true; 
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(NormalPriorityMenu) ].Enabled = true; 
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(LowPriorityMenu) ].Enabled = true; 
			}

			if(SelectedItems.Count>1)
			{ 
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(ConnectMenu) ].Enabled = false; 
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(DeleteMenu) ].Enabled = true;          
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(PriorityMenu) ].Enabled = true; 
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(CopyLinkMenu) ].Enabled = false;
				ContextMenu.MenuItems[ ContextMenuServers.MenuItems.IndexOf(CopyLinkHTMLMenu) ].Enabled = false;
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(HighPriorityMenu) ].Enabled = true; 
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(NormalPriorityMenu) ].Enabled = true; 
				PriorityMenu.MenuItems[ PriorityMenu.MenuItems.IndexOf(LowPriorityMenu) ].Enabled = true; 
			} 
		} 
		//end gczobe

		private void m_CheckPriority()
		{	
			PriorityMenu.MenuItems[0].Checked=false;
			PriorityMenu.MenuItems[1].Checked=false;
			PriorityMenu.MenuItems[2].Checked=false;
			
			if (SelectedItems.Count==0) return;
			if (SelectedItems.Count>1) return;

			InterfaceServer server=(InterfaceServer)SelectedItems[0].Tag;
			byte index=1;
			
			switch(server.Priority)
			{
				case Constants.ServerPriority.High:index=0;
					break;
				case Constants.ServerPriority.Normal:index=1;
					break;
				case Constants.ServerPriority.Low:index=2;
					break;
				default: index=1;
					break;
			}
			PriorityMenu.MenuItems[index].Checked=true;
		}

		public class ServerColumnSorter : IComparer
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
						if ((((InterfaceServer)rowA.Tag).FailedConnections)>(((InterfaceServer)rowB.Tag).FailedConnections))
							return sortOrder;
						else
							return -sortOrder;
					case 3:
						if (((((InterfaceServer)rowA.Tag).Files)>(((InterfaceServer)rowB.Tag).Files)))
							return -sortOrder;
						else
							return sortOrder;
					case 4:
						if (((((InterfaceServer)rowA.Tag).Users)>(((InterfaceServer)rowB.Tag).Users)))
							return -sortOrder;
						else
							return sortOrder;
					case 5:
						
										
						byte prioA=(byte)((byte)((InterfaceServer)rowA.Tag).Priority+1);
						byte prioB=(byte)((byte)((InterfaceServer)rowB.Tag).Priority+1);
						
						if (prioA==2) prioA=0;
						if (prioB==2) prioB=0;
						
						if (prioA<prioB)
							return -sortOrder;
						else if (prioA>prioB) 
							return sortOrder;

						if((((InterfaceServer)rowA.Tag).Files)>(((InterfaceServer)rowB.Tag).Files))
							return -sortOrder;
						else
							return sortOrder;
					
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
