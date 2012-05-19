using System;
using Gtk;
using Glade;
using Gdk;
using eLePhant.eDonkey;
using eLePhant.Types;

namespace eLePhantGTK
{
	public class TSharedFiles
	{
		public eLePhant.eDonkey.CInterfaceGateway krnGateway;
		public Gtk.TreeStore store;
		public Gtk.TreeView tvSharedFiles;
		public eLePhant.eDonkey.InterfacePreferences preferences;

		public TSharedFiles (Gtk.TreeView tv, CInterfaceGateway in_krnGateway)
		{
			tvSharedFiles = tv;
			krnGateway = in_krnGateway;
			store = new TreeStore (typeof (string), typeof (string), typeof (string),
   	                     typeof (string));
   	   tvSharedFiles.Model = store;
			Gtk.TreeViewColumn tvc = new TreeViewColumn ("File name", new CellRendererText(),"text",0);			
			tvSharedFiles.AppendColumn (tvc);
			tvc.SortColumnId = 0;
			
			tvc = new TreeViewColumn ("Size", new CellRendererText(),"text",1);
   	   tvSharedFiles.AppendColumn (tvc);
   	   tvc.SortColumnId = 1;
   	   
   	   tvc = new TreeViewColumn ("Priority", new CellRendererText(),"text",2);
   	   tvSharedFiles.AppendColumn (tvc);
   	   tvc.SortColumnId = 2;
   	   
   	   tvc = new TreeViewColumn ("Progress", new CellRendererText(),"text",3);
   	   tvSharedFiles.AppendColumn (tvc);
   	   tvc.SortColumnId = 3;
   	   
   	   loadSharedFiles();
   	   //krnGateway.OnNewSharedFile+=new FileEvent(prueba);
		}
		/*krnGateway.OnRefreshShared+=new RefreshEvent(m_InvokeOnRefreshShared);
			krnGateway.OnNewSharedFile+=new FileEvent(m_InvokeOnNewSharedFile);
			krnGateway.OnRemovedSharedFile+=new FileEvent(m_InvokeOnRemovedSharedFile);*/
		public void loadSharedFiles()
		{
			krnGateway.RefreshSharedList();
			InterfaceFile[] shareds=krnGateway.GetSharedFileList();
			preferences = krnGateway.GetConfig();

			foreach (InterfaceFile file in shareds)
			{
				store.AppendValues(file.Name,file.Size.ToString(),file.DownloadPriority.ToString(),
				                           file.PercentCompleted.ToString());
			}
		}
		
		public void prueba (InterfaceFile file)
		{
			Console.WriteLine ("prueba");
		}
	}
}