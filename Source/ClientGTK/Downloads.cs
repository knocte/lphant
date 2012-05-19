using System;
using Gtk;
using Glade;
using Gdk;
using eLePhant.eDonkey;

namespace eLePhantGTK
{
	public class TDownloads
	{
		public eLePhant.eDonkey.CInterfaceGateway krnGateway;
		
		public TDownloads (Gtk.TreeView tvDownloads, CInterfaceGateway in_krnGateway)
		{
			krnGateway = in_krnGateway;
			
			Gtk.TreeViewColumn tvc = new TreeViewColumn ("File name",new CellRendererText(),"text",0);
			tvDownloads.AppendColumn (tvc);
			tvc.SortColumnId = 0;
			
			tvc = new TreeViewColumn ("Size",new CellRendererText(),"text",1);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 1;
   	   
   	   tvc = new TreeViewColumn ("Speed",new CellRendererText(),"text",2);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 2;
   	   
   	   tvc = new TreeViewColumn ("Completed",new CellRendererText(),"text",3);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 3;
   	   
   	   tvc = new TreeViewColumn ("Remaining",new CellRendererText(),"text",4);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 4;
   	   
   	   tvc = new TreeViewColumn ("Sources",new CellRendererText(),"text",5);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 5;
   	   
   	   tvc = new TreeViewColumn ("Useful S.",new CellRendererText(),"text",6);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 6;
   	   
   	   tvc = new TreeViewColumn ("Transfer",new CellRendererText(),"text",7);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 7;
   	   
   	   tvc = new TreeViewColumn ("Status",new CellRendererText(),"text",8);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 8;
   	   
   	   tvc = new TreeViewColumn ("Progress",new CellRendererText(),"text",9);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 9;
   	   
   	   tvc = new TreeViewColumn ("Priority",new CellRendererText(),"text",10);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 10;
   	   
   	   tvc = new TreeViewColumn ("Estimated",new CellRendererText(),"text",11);
   	   tvDownloads.AppendColumn (tvc);
   	   tvc.SortColumnId = 11;
		}
	}
}