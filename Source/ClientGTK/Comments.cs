using System;
using Gtk;
using Glade;
using Gdk;
using eLePhant.eDonkey;

namespace eLePhantGTK
{
	public class TComments
	{
		public eLePhant.eDonkey.CInterfaceGateway krnGateway;
		
		public TComments (Gtk.TreeView tvComments, CInterfaceGateway in_krnGateway)
		{
			krnGateway = in_krnGateway;
			
			Gtk.TreeViewColumn tvc = new TreeViewColumn ("Name", new CellRendererText(),"text",0); 
			tvComments.AppendColumn (tvc);
			tvc.SortColumnId = 0;
			
			tvc = new TreeViewColumn ("Rating", new CellRendererText(),"text",1); 
   	   tvComments.AppendColumn (tvc);
   	   tvc.SortColumnId = 1;
   	   
   	   tvc = new TreeViewColumn ("Comment", new CellRendererText(),"text",2);
   	   tvComments.AppendColumn (tvc);
   	   tvc.SortColumnId = 2;
		}
	}
}