using System;
using Gtk;
using Glade;
using Gdk;
using Egg;
using eLePhant.eDonkey;

namespace eLePhantGTK
{
	public class TIcon
	{
		private Gtk.Menu menu;
		private TrayIcon t;
		private eLePhant.eDonkey.CInterfaceGateway krnGateway;
		private Gtk.Window mainwindow;

		public TIcon (CInterfaceGateway in_krnGateway, Gtk.Window mwindow)
		{
			krnGateway = in_krnGateway;
			mainwindow = mwindow;
			
		 	menu = new Gtk.Menu ();
   		EventBox eb = new EventBox ();
			eb.ButtonPressEvent += new ButtonPressEventHandler (TIconClicked);
			eb.Add (new Gtk.Image (new Gdk.Pixbuf (null, "lPhant.png")));
			
			MenuItem it_show = new MenuItem ("Show");
			it_show.Activated += new EventHandler (TIconShow);
			
			MenuItem it_options = new MenuItem ("Options");
			it_options.Activated += new EventHandler (TIconOptions);
			
			ImageMenuItem it_quit = new ImageMenuItem("Quit");
			it_quit.Activated += new EventHandler (TIconQuit);
			
			menu.Append (it_show);
			menu.Append (it_options);
			menu.Append (it_quit);
					
   	   t = new TrayIcon ("eLePhantGTK");
   	   t.Add (eb);
   	   t.ShowAll ();
   	   
		}

		private void TIconClicked (object sender, ButtonPressEventArgs args)
		{
			Gdk.EventButton eb = args.Event;

    		if (eb.Button == 3)
    		{
				menu.ShowAll ();
				menu.Popup (null, null, null, IntPtr.Zero, args.Event.Button, args.Event.Time);
			}
		}
		
		private void TIconQuit (object sender, EventArgs args)
		{
			krnGateway.CloseKernel ();
			Application.Quit ();			
		}
		
		private void TIconShow (object sender, EventArgs args)
		{
			mainwindow.Show();
		}
		
		private void TIconOptions (object sender, EventArgs args)
		{
			OptionsDialog options = new OptionsDialog (krnGateway);
   		options.Run();
		}
	}
}