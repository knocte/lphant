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
using System.Threading;
using System.Drawing;
using eLePhant.eDonkey;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for fileNamesListView.
	/// </summary>
	/// 
	
	public class fileNamesListView : eLePhantListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		/// 
		private CkernelGateway krnGateway;
		private System.ComponentModel.Container components = null;
		private ColumnSorter TheColumnSorter;
		private string m_strFileHash;
		
		public void Initialize(CkernelGateway in_krnGateway)
		{
			Name = "listViewNames";
			krnGateway=in_krnGateway;
			Columns.Add("File Name",190,false);
			Columns.Add("Sources",60,false);
			krnGateway.OnRefreshFileNames+=new RefreshEvent(m_InvokeRefreshList);
			TheColumnSorter=new ColumnSorter();
			TheColumnSorter.CurrentColumn=1;
			TheColumnSorter.sortOrder=-1;
			sortColumn=1;
			sortOrder=-1;
			this.ClickHeader+=new HeaderEventHandler(myColumnClick);
			
			m_DefaultWidths=new int[] {190,60};
			m_Globalize();
			m_LoadWidths();
		}

		private void m_Globalize()
		{
			Columns[0].Text=eLePhantForm.Globalization["LBL_FILENAME"];
			Columns[1].Text=eLePhantForm.Globalization["LBL_SOURCES"];
		}

		public void Globalize()
		{
			m_Globalize();
			this.Refresh();
		}

		private void myColumnClick(object sender, HeaderEventArgs e)
		{
			TheColumnSorter.CurrentColumn = e.ColumnIndex;
			TheColumnSorter.sortOrder=this.sortOrder;
			this.ListViewItemSorter = TheColumnSorter;	
			this.Sort();
			this.ListViewItemSorter = null;			
		}

		private void FileNameToItem(InterfaceFileNames fileName,ListViewItem itemFileName)
		{
			/*if (fileName==null) return;
			if (itemFileName.SubItems[0].Text!=fileName.FileName) itemFileName.SubItems[0].Text=fileName.FileName;
			if (itemFileName.SubItems[1].Text!=fileName.nClients.ToString()) itemFileName.SubItems[1].Text=fileName.nClients.ToString();
			*/
			itemFileName.Tag=fileName;
			if (itemFileName.SubItems[0].Text!=fileName.FileName) itemFileName.SubItems[0].Text=fileName.FileName;
			if (itemFileName.SubItems[1].Text!=fileName.nClients.ToString()) itemFileName.SubItems[1].Text=fileName.nClients.ToString();

		}

		private void OnNewFileName(InterfaceFileNames fileName)
		{
			ListViewItem itemFileName=new ListViewItem(new string[]{"",""});
			
			FileNameToItem(fileName,itemFileName);
			Items.Add(itemFileName);
		}
		private void m_UpdateOrAddFilename(InterfaceFileNames fileName)
		{
			bool updated=false;
			foreach (ListViewItem itemFileName in this.Items)
			{
				if (itemFileName.Text==fileName.FileName)
				{
					if (itemFileName.SubItems[1].Text!=fileName.nClients.ToString()) itemFileName.SubItems[1].Text=fileName.nClients.ToString();
					itemFileName.Tag=fileName;
					updated=true;
					break;
				}
			}
			if (!updated) OnNewFileName(fileName);
		}
		private void m_InvokeRefreshList(CkernelGateway in_krnGateway)
		{
			this.BeginInvoke(new RefreshEvent(m_RefreshList),new object[] {in_krnGateway});
		}
//		private void m_RefreshList(CkernelGateway in_krnGateway)
//		{
//			if (m_strFileHash==null) return;
//			ReloadList(m_strFileHash);
//		}
		private void m_RefreshList(CkernelGateway in_krnGateway)
		{
			if (m_strFileHash==null) 
			{	
				Items.Clear();	
				return;
			 }
			InterfaceFileNames[] files=krnGateway.GetFileNamesList(m_strFileHash);
			if (files==null)
			{
				Items.Clear();	
				return;
			}
			foreach (InterfaceFileNames fileName in files)
			{
				m_UpdateOrAddFilename(fileName);
			}
			this.ListViewItemSorter = TheColumnSorter;	
			this.Sort();
			this.ListViewItemSorter = null;
			//clear from list old names
			/*ArrayList toDelete=new ArrayList();
			foreach (ListViewItem itemFileName in this.Items)
			{
				if ((uint)itemFileName.Tag!=refreshID)
					toDelete.Add(itemFileName);
			}
			foreach (ListViewItem itemFileName in toDelete)
			{
				this.Items.Remove(itemFileName);
			}
			toDelete.Clear();*/
		}
		public void ReloadList(string in_strFileHash)
		{
			m_strFileHash=in_strFileHash;
			InterfaceFileNames[] files=krnGateway.GetFileNamesList(m_strFileHash);				
			Items.Clear();
			if (files==null) return;
			foreach (InterfaceFileNames fileName in files)
			{
				if (fileName!=null) OnNewFileName(fileName);
			}
			this.ListViewItemSorter = TheColumnSorter;	
			this.Sort();
			this.ListViewItemSorter = null;
		}
		public fileNamesListView(System.ComponentModel.IContainer container)
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

		public fileNamesListView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public class ColumnSorter : IComparer
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
					case 1:
						if ((((InterfaceFileNames)rowA.Tag).nClients)>(((InterfaceFileNames)rowB.Tag).nClients))
							return sortOrder;
						else
							return -sortOrder;
														
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
