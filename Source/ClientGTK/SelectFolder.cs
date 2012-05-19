using System;
using Gtk;
using Glade;
using Gdk;

namespace eLePhantGTK
{
	public class SelectFolder
	{
		[Glade.Widget] public Gtk.FileSelection dlgSelectFolder;
		
		public SelectFolder()
		{
			Glade.XML gxml = new Glade.XML (null, "elephant.glade", "dlgSelectFolder", null);
			gxml.Autoconnect (this);
			dlgSelectFolder.Icon = new Gdk.Pixbuf (null, "lPhant.png");
		}
	}
}