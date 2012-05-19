using System;
using Gtk;
using Glade;
using Gdk;
using eLePhant.eDonkey;

namespace eLePhantGTK
{
	public class TUploads
	{
		public eLePhant.eDonkey.CInterfaceGateway krnGateway;
		
		public TUploads (Gtk.TreeView tvUploads, CInterfaceGateway in_krnGateway)
		{
			krnGateway = in_krnGateway;
			
			Gtk.TreeViewColumn tvc = new TreeViewColumn ("Name",new CellRendererText(),"text",0);
			tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 0;
			
			tvc = new TreeViewColumn ("File name",new CellRendererText(),"text",1); 
   	   tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 1;
   	   
   	   tvc = new TreeViewColumn ("Speed",new CellRendererText(),"text",2);
   	   tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 2;
   	   
   	   tvc = new TreeViewColumn ("Position",new CellRendererText(),"text",3);
   	   tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 3;
   	   
   	   tvc = new TreeViewColumn ("Downloaded",new CellRendererText(),"text",4);
   	   tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 4;
   	   
   	   tvc = new TreeViewColumn ("Uploaded",new CellRendererText(),"text",5);
   	   tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 5;
   	   
   	   tvc = new TreeViewColumn ("Progress",new CellRendererText(),"text",6);
   	   tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 6;
   	   
   	   tvc = new TreeViewColumn ("Software",new CellRendererText(),"text",7);
   	   tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 7;
   	   
   	   tvc = new TreeViewColumn ("Version",new CellRendererText(),"text",8);
   	   tvUploads.AppendColumn (tvc);
			tvc.SortColumnId = 8;
		} 
	}
}