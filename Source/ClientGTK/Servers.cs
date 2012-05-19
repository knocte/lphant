using System;
using System.Net;
using System.Diagnostics;
using Gtk;
using Glade;
using Gdk;
using eLePhant.eDonkey;
using eLePhant.Types;

namespace eLePhantGTK
{
	public class TServers
	{		
		public eLePhant.eDonkey.CInterfaceGateway krnGateway;
		public Gtk.TreeStore stServers;
		public Gtk.TreeView tvServers;
		public Gtk.Menu menu;
		public Gtk.TextView txtServerLog;
		public Gtk.TextBuffer buffer;
		private Gtk.Button btnAddServer;
		private Gtk.Button btnDownloadServers;
		private Gtk.Entry txtDownloadServers;
		private Gtk.Entry txtServerIp;
		private Gtk.Entry txtServerPort;
		
		public TServers (CInterfaceGateway in_krnGateway,Glade.XML gxml,Gtk.TextView log)
		/*public TServers (Gtk.TreeView tv, Gtk.TextView log, CInterfaceGateway in_krnGateway,
							Gtk.Button btn1,Gtk.Button btn2,Gtk.Entry txt1,Gtk.Entry txt2,Gtk.Entry txt3)*/
		{
			krnGateway=in_krnGateway;
			krnGateway.OnLogMessage+=new LogEvent(m_Log);
			krnGateway.OnNewServer += new ServerEvent(OnNewServer);
			//btnAddServer = btn1;
			btnAddServer = (gxml.GetWidget("btnAddServer") as Button);
			//btnDownloadServers = btn2;
			btnDownloadServers = (gxml.GetWidget("btnDownloadServers") as Button);
			//txtDownloadServers = txt1;
			txtDownloadServers = (gxml.GetWidget("txtDownloadServers") as Entry);
			//txtServerIp = txt2;
			txtServerIp = (gxml.GetWidget("txtServerIp") as Entry);
			//txtServerPort = txt3;
			txtServerPort = (gxml.GetWidget("txtServerPort") as Entry);
			txtServerLog = log; //I don't use gxml due to problems with buffer
			buffer = txtServerLog.Buffer;
			/*tvServers = tv;*/
			tvServers = (gxml.GetWidget("tvServers") as TreeView);
			
			stServers = new TreeStore (typeof (string), typeof (string), typeof (string),
   	                     typeof (string), typeof (string), typeof (string));
   	   tvServers.Model = stServers;
   	   
			Gtk.TreeViewColumn tvc = new TreeViewColumn ("Server Name", new CellRendererText (), "text", 0);
			tvServers.AppendColumn(tvc);
			tvc.SortColumnId = 0;
			tvc.Resizable = true;
   	   
   	   tvc = new TreeViewColumn ("Address", new CellRendererText (), "text", 1);
			tvServers.AppendColumn(tvc);
			tvc.SortColumnId = 1;
			tvc.Resizable = true;
			
   	   tvc = new TreeViewColumn ("Failed", new CellRendererText (), "text", 2);
			tvServers.AppendColumn(tvc);
			tvc.SortColumnId = 2;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("Files", new CellRendererText (), "text", 3);
			tvServers.AppendColumn(tvc);
			tvc.SortColumnId = 3;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("Users", new CellRendererText (), "text", 4);
			tvServers.AppendColumn(tvc);
			tvc.SortColumnId = 4;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("Priority", new CellRendererText (), "text", 5);
			tvServers.AppendColumn(tvc);
			tvc.SortColumnId = 5;
			tvc.Resizable = true;
			
			menu = new Gtk.Menu ();
			MenuItem it_Connect = new MenuItem ("Connect");
			it_Connect.Activated += new EventHandler (ConnectTo); 
			MenuItem it_Copy = new MenuItem ("Copy to clipboard");
			it_Copy.Activated += new EventHandler (CopyLink);
			MenuItem it_Delete = new MenuItem ("Delete");
			it_Delete.Activated += new EventHandler (Delete);
			
			menu.Append (it_Connect);
			menu.Append (it_Copy);
			menu.Append (it_Delete);
			
			tvServers.ButtonPressEvent += new ButtonPressEventHandler (ButtonPressEvent);
			// tvServers.PopupMenu += new PopupMenuHandler(PopupMenuEvent);
			loadServers();
		}
		
		private void loadServers()
		{
			InterfaceServer[] list=krnGateway.GetServerList(); //gets a vector with all the servers in server.met
              
			foreach (InterfaceServer server in list) //display the servers IP in the treeview
			{
			   stServers.AppendValues(server.Name,server.IP+" : "+server.Port,server.FailedConnections.ToString(),
                        server.Files.ToString(),server.Users.ToString(),
                        server.Priority.ToString());
			}
		}
		
		[GLib.ConnectBefore]
   	public void ButtonPressEvent (object o, ButtonPressEventArgs args)
   	{
   		if (tvServers.Selection.CountSelectedRows()>0)
   		{
   			Gdk.EventButton eb = args.Event;

    			if (eb.Button == 3)
    			{
    				menu.ShowAll ();
    				menu.Popup (null, null, null, IntPtr.Zero, args.Event.Button, args.Event.Time);
    			}
    		}
   	}
		
		private void m_Log(Constants.Log importance,string strMsg)
		{
			//if (importance==Constants.Log.Notify) labelmsg.Text=strMsg;
			string newline=DateTime.Now.ToShortTimeString()+" "+strMsg+"\n";
			lock(txtServerLog)
			{
				buffer.Text = buffer.Text + newline;
			}

		}
		
		private void ConnectTo (object sender, EventArgs args)
		{
			Gtk.TreeModel model;
			Gtk.TreeIter iter;
			string ipstr;
			string[] IpPort;
			
			tvServers.Selection.GetSelected (out model, out iter);
			ipstr = model.GetValue (iter,1).ToString();
			IpPort = ipstr.Split(":".ToCharArray());
			IPAddress ip = IPAddress.Parse(IpPort[0]);
			krnGateway.ConnectToServer (BitConverter.ToUInt32(ip.GetAddressBytes(),0),Convert.ToUInt16(IpPort[1]));
		}
		
		private void Delete (object sender, EventArgs args)
		{
			int SelectedItem;
			string ipstr;
			string[] IpPort;
			
			SelectedItem = tvServers.Selection.CountSelectedRows();
			if (SelectedItem == 0) return;

			try
			{
				InterfaceServer[] Items= new InterfaceServer[SelectedItem];
				Gtk.TreeModel model;
				Gtk.TreeIter iter;
				Gtk.TreePath[] paths = tvServers.Selection.GetSelectedRows (out model);
				
				model.GetIter (out iter, paths[0]);

				for(int i=0;i!=SelectedItem;i++)
				{
					try
					{
						ipstr = model.GetValue (iter,1).ToString();
						IpPort = ipstr.Split(":".ToCharArray());
						IPAddress ip = IPAddress.Parse(IpPort[0]);
						krnGateway.DeleteServer(BitConverter.ToUInt32(ip.GetAddressBytes(),0),Convert.ToUInt16 (IpPort[1]));
						stServers.Remove (ref iter);
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.ToString());
					}
					
					model.GetIter (out iter, paths[i+1]);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine (ex.ToString());
			}
		}
		
		public void DownloadServers ()
		{
			if(txtDownloadServers.Text.StartsWith("http"))
				krnGateway.DownloadServerList(txtDownloadServers.Text);
		}
		
		public void AddServer ()
		{
			if((txtServerIp.Text.Length>0)&&(txtServerPort.Text.Length>0))
				krnGateway.DownloadElink("ed2k://|server|"+txtServerIp.Text+"|"+txtServerPort.Text+"|/",false);
		}
		
		private void CopyLink (object sender, EventArgs args)
		{	
			Gtk.TreeModel model;
			Gtk.TreeIter iter;
			string ipstr;
			string[] IpPort;
			string ed2k;
			
			tvServers.Selection.GetSelected (out model, out iter);
			ipstr = model.GetValue (iter,1).ToString();
			IpPort = ipstr.Split(":".ToCharArray());
			IPAddress ip = IPAddress.Parse(IpPort[0]);
			
			string PasteIp = IpPort[0].Trim();
			string PastePort = IpPort[1].Trim();
			Clipboard cb = Clipboard.Get (Gdk.Selection.Clipboard);
			ed2k = "ed2k://|server|"+PasteIp+"|"+PastePort+"|/";
			cb.SetText (ed2k);
		}
		
		public void OnNewServer(InterfaceServer server)
		{	
			stServers.AppendValues(server.Name,server.IP+" : "+server.Port,server.FailedConnections.ToString(),
                        server.Files.ToString(),server.Users.ToString(),
                        server.Priority.ToString());
		}
	}
}