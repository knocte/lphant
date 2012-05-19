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
using System.Threading;
using System.Windows.Forms;
using eLePhant.eDonkey;
namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for commentsListView.
	/// </summary>
	public class commentsListView : eLePhantListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		
		private CkernelGateway krnGateway;
		private ColumnSorter TheColumnSorter;
		private string m_strFileHash;
		private System.ComponentModel.Container components = null;

		public commentsListView(System.ComponentModel.IContainer container)
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

		public commentsListView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		public void Initialize(CkernelGateway in_krnGateway)
		{
			Name = "listViewComments";
			krnGateway=in_krnGateway;
			Columns.Add("User",100,false);
			Columns.Add("Rating",60,false);
			Columns.Add("Comment",220,false);
			krnGateway.OnRefreshFileComments+=new RefreshEvent(m_InvokeRefreshList);
			TheColumnSorter=new ColumnSorter();
			TheColumnSorter.CurrentColumn=1;
			TheColumnSorter.sortOrder=-1;
			this.ClickHeader+=new HeaderEventHandler(myColumnClick);
			
			m_DefaultWidths=new int[] {100,60,220};
			m_Globalize();
			m_LoadWidths();
		}
		
		private void m_Globalize()
		{
			Columns[0].Text=eLePhantForm.Globalization["LBL_NAME"];
			Columns[1].Text=eLePhantForm.Globalization["LBL_RATING"];
			Columns[2].Text=eLePhantForm.Globalization["LBL_COMMENT"];
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

		private void CommentToItem(InterfaceComment comment,ListViewItem itemComment)
		{
			if (comment==null) return;
			if (itemComment.SubItems[0].Text!=comment.ClientName) itemComment.SubItems[0].Text=comment.ClientName;
			if (itemComment.SubItems[1].Text!=comment.Rating) itemComment.SubItems[1].Text=comment.Rating;
			if (itemComment.SubItems[2].Text!=comment.Comment) itemComment.SubItems[2].Text=comment.Comment;
			//itemComment.Tag=comment;
		}
		private void m_UpdateOrAddComment(InterfaceComment comment)
		{
			bool updated=false;
			foreach (ListViewItem itemComment in this.Items)
			{
				if (itemComment.SubItems[2].Text==comment.Comment)
				{
					CommentToItem(comment,itemComment);
					updated=true;
					break;
				}
			}
			if (!updated) OnNewComment(comment);
		}
		private void OnNewComment(InterfaceComment comment)
		{
			ListViewItem itemComment=new ListViewItem(new string[]{"","",""});
			
			itemComment.Tag=comment;
			CommentToItem(comment,itemComment);
			lock(this)
			{
				Items.Add(itemComment);
			}
		}
		private void m_InvokeRefreshList(CkernelGateway in_krnGateway)
		{
			this.BeginInvoke(new RefreshEvent(m_RefreshList),new object[] {in_krnGateway});
		}

		private void m_RefreshList(CkernelGateway in_krnGateway)
		{
			if (m_strFileHash==null)
			{	
				Items.Clear();
				return;
			}
			InterfaceComment[] comments=krnGateway.GetComments(m_strFileHash);
			if (comments==null) 
			{	
				Items.Clear();
				return;
			}
			foreach (InterfaceComment comment in comments)
			{
				if (comment!=null) m_UpdateOrAddComment(comment);
			}
		}
		public bool ReloadList(string in_strFileHash)
		{
			m_strFileHash=in_strFileHash;	
			
			InterfaceComment[] comments=krnGateway.GetComments(m_strFileHash);				
			Items.Clear();
			if (comments==null) return false;
			bool anyComments=false;
			foreach (InterfaceComment comment in comments)
			{
				if (comment!=null) 
				{
					OnNewComment(comment);
					anyComments=true;
				}
			}
			ListViewItemSorter = TheColumnSorter;	
			Sort();
			ListViewItemSorter = null;
			return anyComments;
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
					
				return sortOrder*String.Compare(rowA.SubItems[CurrentColumn].Text,
							rowB.SubItems[CurrentColumn].Text);
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
