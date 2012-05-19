using System;
using Gtk;
using Glade;
using Gdk;

namespace eLePhantGTK
{
	public class AboutDialog
	{
		[Glade.Widget] Gtk.Dialog dlgAbout;
		[Glade.Widget] Gtk.Label lblAbout;
		
		public AboutDialog ()
		{
			Glade.XML gxml = new Glade.XML (null, "elephant.glade", "dlgAbout", null);
			gxml.Autoconnect (this);	
		}
		
		public void Run ()
		{
			dlgAbout.Icon = new Gdk.Pixbuf (null, "lPhant.png");
			dlgAbout.Run ();
			dlgAbout.Hide ();
			dlgAbout.Dispose ();
		}
		
	}
}