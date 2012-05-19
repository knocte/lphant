using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using Gtk;
using Glade;
using Gdk;
using eLePhant.eDonkey;
using eLePhant.Types;
using eLePhant.Classes;
using System.Runtime.Remoting.Messaging;
using GLib;

namespace eLePhantGTK
{
	public class TSearch 
	{
		private Gtk.TreeView tvSearch;
		private eLePhant.eDonkey.CInterfaceGateway krnGateway;
		private Gtk.TreeStore stSearch;
		private Gtk.Button btnBeginSearch;
		private Gtk.Button btnStopSearch;
		private Gtk.Entry txtSearch;
		private Gtk.Menu menu;
		private Hashtable TotalResult = new Hashtable();
		private int m_SearchCount;
		private Hashtable SearchesList;
		private Gtk.ProgressBar progressbar;
		private delegate void SearchFileDelegate(InterfaceSearchedFile file, int searchID);
		private delegate void SearchProgressDelegate (int progress, int searchID);
		
		public TSearch (CInterfaceGateway in_krnGateway,Glade.XML gxml)
		{
			krnGateway = in_krnGateway;
			progressbar = (gxml.GetWidget("progressbar") as ProgressBar);
			btnBeginSearch = (gxml.GetWidget("btnBeginSearch") as Button);
			btnStopSearch = (gxml.GetWidget("btnStopSearch") as Button);
			txtSearch = (gxml.GetWidget("txtSearch") as Entry);
			tvSearch = (gxml.GetWidget("tvSearch") as TreeView);
			m_SearchCount = 0;
			
			CreateColumns();
			CreatePopupMenu();
			AddEvents();
		}
		
		private void CreateColumns()
		{
			stSearch = new TreeStore (typeof (string),typeof (string),typeof (string),
			  	                     typeof (string),typeof (string),typeof (string),typeof (string));
			
			tvSearch.Model = stSearch;
			
			Gtk.TreeViewColumn tvc = new TreeViewColumn ("File Name", new CellRendererText (), "text", 0);
			tvSearch.AppendColumn(tvc);
			tvc.SortColumnId = 0;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("Size", new CellRendererText (), "text", 1);
			tvSearch.AppendColumn(tvc);
			tvc.SortColumnId = 1;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("Sources", new CellRendererText (), "text", 2);
			tvSearch.AppendColumn(tvc);
			tvc.SortColumnId = 2;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("Codec", new CellRendererText (), "text", 3);
			tvSearch.AppendColumn(tvc);
			tvc.SortColumnId = 3;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("Duration", new CellRendererText (), "text", 4);
			tvSearch.AppendColumn(tvc);
			tvc.SortColumnId = 4;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("Bitrate", new CellRendererText (), "text", 5);
			tvSearch.AppendColumn(tvc);
			tvc.SortColumnId = 5;
			tvc.Resizable = true;
			
			tvc = new TreeViewColumn ("File type", new CellRendererText (), "text", 6);
			tvSearch.AppendColumn(tvc);
			tvc.SortColumnId = 6;
			tvc.Resizable = true;
		}
		
		
		private void CreatePopupMenu()
		{
			menu = new Gtk.Menu ();
			MenuItem itDownload = new MenuItem ("Download");
			SeparatorMenuItem itSeparator = new SeparatorMenuItem();
			MenuItem itCopy = new MenuItem ("Copy elink to clipboard");
			
			menu.Append (itDownload);
			menu.Append (itSeparator);
			menu.Append (itCopy);
		}
		
		private void AddEvents()
		{
		
			tvSearch.ButtonPressEvent += new ButtonPressEventHandler (ButtonPressEvent);
			//I should review these events
			krnGateway.OnNewSearched+=new SearchEvent(m_InvokeOnNewSearched);
			krnGateway.OnNewSearchProgress+=new SearchProgressEvent(m_InvokeOnSearchProgress);
			krnGateway.OnEndSearch += new EndSearchEvent(m_InvokeOnEndSearch);
			
			btnBeginSearch.Clicked += new EventHandler(on_btnBeginSearch_clicked);
			txtSearch.Activated += new EventHandler(on_btnBeginSearch_clicked);
			//krnGateway.OnSearchProgress+=new SearchProgressEvent(m_InvokeOnSearchProgress);
		}
		
		[GLib.ConnectBefore]
		private void ButtonPressEvent (object sender, ButtonPressEventArgs args)
		{
			//Activar parte del menú según el número de elementos seleccionados
			if (tvSearch.Selection.CountSelectedRows()>0)
			{
				Gdk.EventButton eb = args.Event;

    			if (eb.Button == 3)
    			{
    				menu.ShowAll ();
    				menu.Popup (null, null, null, IntPtr.Zero, args.Event.Button, args.Event.Time);
    			}
    		}
		}

		private void on_btnBeginSearch_clicked (object sender, EventArgs args)
		{
			btnBeginSearch.Sensitive = false;
			btnStopSearch.Sensitive = true;
			
			/*uint MinSize;
			try{MinSize=Convert.ToUInt32(textBoxMinSize.Text)*1048576;}	catch{MinSize=0;}
			uint MaxSize;
			try{MaxSize=Convert.ToUInt32(textBoxMaxSize.Text)*1048576;}	catch{MaxSize=0;}
			uint Avaibility;
			try{Avaibility=Convert.ToUInt32(textBoxAvaibility.Text);} catch{Avaibility=0;}
			string searchType="";
			switch (comboBoxSearchType.SelectedIndex)
			{
				case 0: searchType="";
						break;
				case 1: searchType="Video";
						break;
				case 2: searchType="Pro";
						break;
				case 3: searchType="Audio";
						break;
				case 4: searchType="Image";
						break;
			}*/
			
			krnGateway.Search(m_SearchCount,txtSearch.Text,true,"",0,
			                 0,0, "");
			m_SearchCount++;
			stSearch.Clear();
		}
		
		public void MyCallback(IAsyncResult result)
      {
                AsyncResult ar=(AsyncResult)result;
                SearchFileDelegate d=(SearchFileDelegate)ar.AsyncDelegate;
                //int state=(int)ar.AsyncState;
                d.EndInvoke(result);
                //int i=d.EndInvoke(result);
                //Console.WriteLine(i);
                //Console.WriteLine(state);
                //Console.ReadLine();
      }
      
		private void m_InvokeOnNewSearched(InterfaceSearchedFile file, int searchID)
		{
			AsyncCallback callback=new AsyncCallback(MyCallback);
			SearchFileDelegate d=new SearchFileDelegate(m_OnNewSearched);
			d.BeginInvoke(file,searchID,callback,1);
		}
		
		private void m_OnNewSearched(InterfaceSearchedFile file, int searchID)
		{			
			progressbar.Visible = true;
			stSearch.AppendValues(file.Name,file.Size.ToString(),file.Avaibility.ToString(),
										file.Codec,file.Length.ToString(),file.BitRate.ToString(),
										file.ResultState.ToString());		
			/* Añadir cantidad de resultados */
		}
		
		private void m_InvokeOnSearchProgress(int progress, int searchID)
		{
			//AsyncCallback callback=new AsyncCallback(MyCallback);
			//SearchProgressDelegate d=new SearchProgressDelegate(m_OnSearchProgress);
			//d.BeginInvoke(progress,searchID,callback,1);
			progressbar.Pulse();
		}
		
		private void m_OnSearchProgress(int progress, int searchID)
		{
			progressbar.Pulse();
		}
		
		private void m_InvokeOnEndSearch(int searchID)
		{
			progressbar.Visible = false;
			btnBeginSearch.Sensitive = true;
			btnStopSearch.Sensitive = false;
		}
	}
}