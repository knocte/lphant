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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace eLePhant.Client
{
	//The delegate for the HeaderEventArgs class
	public delegate void HeaderEventHandler(object sender, HeaderEventArgs e);
	//The delegate for drawing the entire header
	public delegate void DrawHeaderEventHandler(DrawHeaderEventArgs e);
	//The delegate for click the header
	public delegate void ClickHeaderEventHandler(ColumnClickEventHandler e);

	public class eLePhantListView : ListView
	{
		public int SortColumnIndex;
		//The event for drawing columns
		public event DrawItemEventHandler DrawColumn;
		//The event for handling the entire border drawing of the header
		public event DrawHeaderEventHandler DrawHeader;

		//Event handlers associated with the header control
		public event HeaderEventHandler BeginDragHeaderDivider;
		public event HeaderEventHandler DragHeaderDivider;
		public event HeaderEventHandler EndDragHeaderDivider;
		public event HeaderEventHandler ClickHeader;

		private System.ComponentModel.Container components = null;
		//private StringBuilder strB;
		private Pen borderPen;
		public int ScrollBarBKColor;
		ListColumnArray myColumns;		
		public ImageList headerImages;
		HeaderControl header;
		bool ownerDrawHeader;
		int increaseHeaderHeight;
		public Color headerBackColor;
		public Color headerForeColor;
		int headerHeight;
		bool defaultCustomDraw;
		bool m_Loaded;
		private int m_sortColumn;

		protected int[] m_DefaultWidths;
		public int sortOrder;
		
		private int[] m_headerleft=new int[20];
		private int[] m_headerright=new int[20];
		private Pen m_ArrowPen;
		public Color HeaderBorderColor
		{
			set 
			{
				borderPen=new Pen(value);
			}
		}

		public int sortColumn
		{
			get
			{
				return m_sortColumn;
			}
			set
			{
				m_sortColumn=value;
				this.Refresh();
			}
		}

		public eLePhantListView()
		{
			InitializeComponent();
			
			//strB=new StringBuilder();
			myColumns = new ListColumnArray(); 
			ownerDrawHeader = false;			
			headerBackColor = Color.FromArgb(222,231,247);//SystemColors.Control;
			headerForeColor= Color.Black;
			ScrollBarBKColor = 0x00F7E7DE;
			//m_ArrowPen=new Pen(headerForeColor);
			m_ArrowPen=new Pen(Color.FromArgb(0x80,0,0,0));
			borderPen=new Pen(Color.FromArgb(68,69,151));
			increaseHeaderHeight = 0;
			defaultCustomDraw = false;
			this.CheckBoxes = false;
			this.FullRowSelect=true;
			this.AllowColumnReorder=true;
			this.AutoArrange=false;
			this.HideSelection=false;
			ownerDrawHeader = true;
			this.defaultCustomDraw=false;
			headerImages=new ImageList();
			sortColumn=-1;
			sortOrder=1;
			m_Loaded=false;
			InsertColumns();
		}

		public int GetColumnPosition(int column)
		{
			Win32.LV_COLUMN pcol = new Win32.LV_COLUMN();
			pcol.mask = Win32.LVCF_ORDER;
			Win32.SendMessage(Handle, Win32.LVM_GETCOLUMN, column, ref pcol);
			return (int)pcol.iOrder;
		}
		public void SetColumnPosition(int column, int position)
		{
			Win32.LV_COLUMN pcol = new Win32.LV_COLUMN();
			pcol.mask = Win32.LVCF_ORDER;
			pcol.iOrder = position;
			Win32.SendMessage(Handle, Win32.LVM_SETCOLUMN, column, ref pcol);
		}
		public int GetColumnsWidth(int column)
		{
			return Win32.SendMessage(Handle,Win32.LVM_GETCOLUMNWIDTH,column,0);
		}
		public void SetColumnWidth(int column, int width)
		{
			IntPtr lPram=Win32.MakeLParam(width,0);
			Win32.SendMessage(Handle,Win32.LVM_SETCOLUMNWIDTH,column,lPram);
		}
		public void SetFlatScrollBar()
		{
			IntPtr stylePtr;
			int style;
			stylePtr=Win32.SendMessage(this.Handle, 4151, IntPtr.Zero , IntPtr.Zero);
			style=stylePtr.ToInt32();
			style+=256;
			stylePtr=(IntPtr)style;
			Win32.SendMessage(this.Handle, 4150, IntPtr.Zero , stylePtr);	
			Win32.FlatSB_SetScrollProp(this.Handle,0x80,(IntPtr)ScrollBarBKColor,true);
			Win32.FlatSB_SetScrollProp(this.Handle,0x40,(IntPtr)ScrollBarBKColor,true);
		}
		public void SavePreferences()
		{
			for (int i=0;i<Columns.Count;i++)
			{
				eLePhantForm.preferences.SetProperty(Name+"ColumnWidth-"+i.ToString(),GetColumnsWidth(i));
				eLePhantForm.preferences.SetProperty(Name+"ColumnPosition-"+i.ToString(),this.GetColumnPosition(i));
			}
		}
		protected override void OnVisibleChanged(EventArgs e)
		{
			if (this.Visible) m_LoadPreferences();
			base.OnVisibleChanged(e);
		}
		protected void m_LoadWidths()
		{
			int def;
			for (int i=0;i<Columns.Count;i++)
			{
				if ((m_DefaultWidths==null)||(i>m_DefaultWidths.Length))
					def=60;
				else 
					def=m_DefaultWidths[i];
				Columns[i].Width=eLePhantForm.preferences.GetInt(Name+"ColumnWidth-"+i.ToString(),def);
			}
		}
		private void m_LoadPreferences()
		{
			if (m_Loaded) return;
			m_Loaded=true;
			int newPosition;
			for (int i=0;i<Columns.Count;i++)
			{
				newPosition=eLePhantForm.preferences.GetInt(Name+"ColumnPosition-"+i.ToString(),i);
				this.SetColumnPosition(i,newPosition);
			}
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
	    protected virtual void OnDrawItem(int item,int column,Graphics g,Rectangle rec)
		{

		}
		#region Overriden methods
		
		protected override void OnResize(EventArgs e)
		{
			this.Invalidate();
			base.OnResize (e);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			//Create a new HeaderControl object
			header = new HeaderControl(this);
			if(header.Handle != IntPtr.Zero)
			{
				if(headerImages != null)//If we have a valid header handle and a valid ImageList for it
					//send a message HDM_SETIMAGELIST
					Win32.SendMessage(header.Handle,0x1200+8,IntPtr.Zero,headerImages.Handle);
				//Insert all the columns in Columns collection
				if(this.Columns.Count > 0)
					InsertColumns();				
			}
			base.OnHandleCreated(e);
			SetFlatScrollBar();
		}

		protected override void WndProc(ref Message m)
		{
			Win32.NMHEADER nm;

			switch(m.Msg)
			{
				
				case 0x004E://WM_NOTIFY
					base.WndProc(ref m);
					Win32.NMHDR nmhdr = (Win32.NMHDR)m.GetLParam(typeof(Win32.NMHDR));
					switch(nmhdr.code)
					{
						case (int)Win32.NM.NM_CLICK-320:
							nm=(Win32.NMHEADER)m.GetLParam(typeof(Win32.NMHEADER));
							if (ClickHeader!=null)
							{
								sortColumn=nm.iItem;
								if (sortOrder==1) sortOrder=-1;
											else sortOrder=1;
								ClickHeader(this.Columns[nm.iItem], 
									new HeaderEventArgs(nm.iItem, nm.iButton));
								
							}
							break;
						case (0-300-26)://HDN_BEGINTRACK
							nm=(Win32.NMHEADER)m.GetLParam(typeof(Win32.NMHEADER));
							if(BeginDragHeaderDivider != null)
								BeginDragHeaderDivider(this.Columns[nm.iItem], 
									new HeaderEventArgs(nm.iItem, nm.iButton));
							break;
						case (0-300-20)://HDN_ITEMCHANGING
							nm=(Win32.NMHEADER)m.GetLParam(typeof(Win32.NMHEADER));
							//Adjust the column width
							Win32.RECT rect = new Win32.RECT();
							//HDM_GETITEMRECT
							Win32.SendMessage(header.Handle, 0x1200+7, nm.iItem, ref rect);
							//Get the item height which is actually header's height
							this.headerHeight = rect.bottom-rect.top;
							//para pintar correctamente el progreso
							//Debug.Write("Parte 1: id:" + nm.iItem + " left=" + rect.left + "\n");
							this.m_headerleft[nm.iItem]=rect.left;
							this.m_headerright[nm.iItem]=rect.right;
							this.Columns[nm.iItem].Width = rect.right - rect.left;
							if(DragHeaderDivider != null)
								DragHeaderDivider(this.Columns[nm.iItem],
									new HeaderEventArgs(nm.iItem, nm.iButton));
							break;
						case (0-300-27)://HDN_ENDTRACK
							nm=(Win32.NMHEADER)m.GetLParam(typeof(Win32.NMHEADER));
							if(EndDragHeaderDivider != null)
								EndDragHeaderDivider(this.Columns[nm.iItem],
									new HeaderEventArgs(nm.iItem, nm.iButton));
							break;
//						case (0-300-11): //HDN_ENDDRAG
//							break;
					}
					break;
				case (int)Win32.OCM.OCM_NOTIFY://Reflected WM_NOTIFY message
					Win32.NMHDR nmh = (Win32.NMHDR)m.GetLParam(typeof(Win32.NMHDR));
					switch(nmh.code)
					{
						case (int)Win32.NM.NM_CUSTOMDRAW:
							Win32.NMCUSTOMDRAW nmcd = 
									(Win32.NMCUSTOMDRAW)m.GetLParam(typeof(Win32.NMCUSTOMDRAW));
							if(nmcd.hdr.hwndFrom != this.Handle)
								break;
							switch(nmcd.dwDrawStage)
							{
								case (int)Win32.CDDS.CDDS_PREPAINT:
									m.Result = (IntPtr)Win32.CDRF.CDRF_NOTIFYITEMDRAW;
									break;
								case (int)Win32.CDDS.CDDS_ITEMPREPAINT:
									m.Result = (IntPtr)Win32.CDRF.CDRF_NOTIFYITEMDRAW;
									break;
								case (int)(Win32.CDDS.CDDS_SUBITEM|Win32.CDDS.CDDS_ITEMPREPAINT):
									try
									{
									Win32.NMLVCUSTOMDRAW nmlv = (Win32.NMLVCUSTOMDRAW)
										m.GetLParam(typeof(Win32.NMLVCUSTOMDRAW));
									
									if ((Columns[nmlv.iSubItem].subItemOwnerDraw)&&(nmlv.nmcd.dwItemSpec<Items.Count))
									{
										Rectangle recgrande=base.GetItemRect(nmlv.nmcd.dwItemSpec);
										Win32.RECT rc=new Win32.RECT();
										if (nmlv.iSubItem>0)
										{
											rc.top=nmlv.iSubItem;
											rc.left=0;
											Win32.SendMessage(this.Handle,0x1000 + 56,nmlv.nmcd.dwItemSpec,ref rc);
										}
										else
										{
											rc.top=0;
											rc.left=2;
											Win32.SendMessage(this.Handle,0x1000 + 14,nmlv.nmcd.dwItemSpec,ref rc);
										}
										
										//Rectangle rec=new Rectangle(rc.left+1,rc.top,rc.right-rc.left-2, rc.bottom-rc.top-2);
										Rectangle rec=new Rectangle(rc.left,rc.top,rc.right-rc.left, rc.bottom-rc.top);
										//Point p=this.PointToClient(new Point(rc.left,rc.top));
										//Point p2=this.PointToClient(new Point(rc.right,rc.bottom));
										//Rectangle rec=new Rectangle(p.X,rc.top,p2.X-p.X,rc.bottom-rc.top-2);
										Graphics g = Graphics.FromHdc(nmlv.nmcd.hdc);									
										OnDrawItem(nmlv.nmcd.dwItemSpec,nmlv.iSubItem,g,rec);
										g.Dispose();
										//Win32.ReleaseDC(nmcd.hdr.hwndFrom,nmlv.nmcd.hdc);
										m.Result = (IntPtr)Win32.CDRF.CDRF_SKIPDEFAULT;
									}
									else
										base.WndProc(ref m);//m.Result = (IntPtr)Win32.CDRF.CDRF_DODEFAULT;
									Marshal.StructureToPtr(nmlv,m.LParam,true);
									}
									catch
									{
										base.WndProc(ref m);
									}
									break;
								}
								break;
						default:
							base.WndProc(ref m);
							break;
						}
						break;
				case 0x002B://WM_DRAWITEM
					try
					{
						//Get the DRAWITEMSTRUCT from the LParam of the message
						Win32.DRAWITEMSTRUCT dis = (Win32.DRAWITEMSTRUCT)Marshal.PtrToStructure(
							m.LParam,typeof(Win32.DRAWITEMSTRUCT));
						//agregar aqui la posicion x para pintar bien el progreso.
						//Debug.Write("parte3: id: " + dis.itemID + "\n");
						this.m_headerleft[dis.itemID] = dis.rcItem.left;
						this.m_headerright[dis.itemID] = dis.rcItem.right;
						//Check if this message comes from the header
						if(dis.ctrlType == 100)//ODT_HEADER - it do comes from the header
						{
							//Get the graphics from the hdc field of the DRAWITEMSTRUCT
							Graphics g = Graphics.FromHdc(dis.hdc);
							//Create a rectangle from the RECT struct
							Rectangle r = new Rectangle(dis.rcItem.left, dis.rcItem.top, dis.rcItem.right -
								dis.rcItem.left, dis.rcItem.bottom - dis.rcItem.top);						
							//Create new DrawItemState in its default state					
							DrawItemState d = DrawItemState.Default;
							//Set the correct state for drawing
							if(dis.itemState == 0x0001)
								d = DrawItemState.Selected;
							//Create the DrawItemEventArgs object
							DrawItemEventArgs e = new DrawItemEventArgs(g,this.Font,r,dis.itemID,d);
							//If we have a handler attached call it and we don't want the default drawing
							if(DrawColumn != null && !defaultCustomDraw)
								DrawColumn(this.Columns[dis.itemID], e);
							else if(defaultCustomDraw)
								DoMyCustomHeaderDraw(this.Columns[dis.itemID],e);
							//Release the graphics object					
							g.Dispose();					
						}
					}
					catch
					{
						try
						{
							base.WndProc(ref m);
						}
						catch{}
					}
					break;

				case 0x0002://WM_DESTROY
					//Release the handle associated with the header control window
					header.ReleaseHandle();
					base.WndProc(ref m);
					break;
				case 0x0014://WM_ERASEBKGND
					if (this.Items.Count==0)
						base.WndProc(ref m);
					else
					{
						try
						{
						Brush backBrush=new SolidBrush(BackColor);
						//up
						Rectangle clientRect=this.ClientRectangle;//this.GetItemAt(0,0).Index
						Rectangle rec=clientRect;
						Graphics g = Graphics.FromHdc(m.WParam);
						rec.X=clientRect.X;
						rec.Y=clientRect.Y;
						rec.Width=clientRect.Width;
						rec.Height=this.HeaderHeight+2;
						g.FillRectangle(backBrush,rec);
						//left
						Rectangle itemRect=this.GetItemRect(0,ItemBoundsPortion.Entire);
						rec.Width=itemRect.Left-clientRect.Left+2;
						rec.Height=clientRect.Height+itemRect.Height+2;
						if (rec.Width>0)
						{
							g.FillRectangle(backBrush,rec);
						}
						//right
						rec.X=itemRect.Right;
						rec.Y=clientRect.Top;
						rec.Width=clientRect.Width-itemRect.Width;
						rec.Height=clientRect.Height;
						g.FillRectangle(backBrush,rec);
						//Debug.WriteLine(rec.ToString());
						//bottom
						itemRect=this.GetItemRect(this.Items.Count-1,ItemBoundsPortion.Entire);
						rec.X=clientRect.Left;
						rec.Y=itemRect.Bottom;
						rec.Width=clientRect.Width;
						rec.Height=clientRect.Height-rec.Y+clientRect.Width;
						if (rec.Height>0)
						{
							g.FillRectangle(backBrush,rec);
						}
						}
						catch
						{
							base.WndProc(ref m);
						}
					}
					break;
				default:
					try //Message 0x7b WM_CONTEXTMENU can cause exceptions if control is invisible
					{
						base.WndProc(ref m);
					}
					catch//(Exception e)
					{
						//Debug.WriteLine(e.ToString());
						Visible=true;
					} 
					break;
			}
			
		}

		#endregion

		#region Drawing methods

		void DrawHeaderBorder(DrawHeaderEventArgs e)
		{
			Graphics g = e.Graphics;
			Rectangle r = new Rectangle(e.Bounds.Left,e.Bounds.Top,e.Bounds.Width,e.Bounds.Height);
			if(r.Left == 0)
				g.DrawLine(/*SystemPens.ControlLightLight*/borderPen,r.Left,r.Bottom,r.Left,r.Top);
			if(r.Top == 0)
				g.DrawLine(/*SystemPens.ControlLightLight*/borderPen,r.Left,r.Top,r.Right,r.Top);
			if(r.Bottom == e.HeaderHeight)
				g.DrawLine(/*SystemPens.ControlDark*/borderPen,r.Left,r.Bottom-1,r.Right,r.Bottom-1);
		}

		void DoMyCustomHeaderDraw(object sender, DrawItemEventArgs e)
		{
			ListColumn m = sender as ListColumn;			
			Graphics g = e.Graphics;
			//Get the text width
			SizeF szf = g.MeasureString(m.Text, this.Font);
			int textWidth = (int)szf.Width+10;
			Image image = null;
			
			Rectangle r = e.Bounds;
			int leftOffset = 4;
			int rightOffset = 4;
			
			StringFormat s = new StringFormat();
			s.FormatFlags = StringFormatFlags.NoWrap;
			s.Trimming = StringTrimming.EllipsisCharacter;
			switch(m.TextAlign)
			{
				case HorizontalAlignment.Left:
					s.Alignment = StringAlignment.Near;
					break;
				case HorizontalAlignment.Center:
					s.Alignment = StringAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					s.Alignment = StringAlignment.Far;
					break;
			}
			s.LineAlignment = StringAlignment.Center;
			//Adjust the proper text bounds and get the correct image(if any)
//			if(m.ImageIndex != -1 && headerImages != null)
//			{
//				if(m.ImageIndex + 1 > headerImages.Images.Count)
//					image = null;
//				else
//				{
//					if(m.ImageOnRight)
//						rightOffset += 20;
//					else
//						leftOffset += 20;
//					image = new Bitmap(headerImages.Images[m.ImageIndex],16,16);
//				}
//			}
			if (this.Columns.IndexOf(m)==sortColumn)
				rightOffset=13;
			else
				rightOffset=4;
			leftOffset=2;
			if(textWidth+leftOffset+rightOffset > r.Width)
				textWidth = r.Width - leftOffset - rightOffset;
			
			Rectangle text = new Rectangle(r.Left+leftOffset, r.Top, 
				textWidth, r.Height);
			Rectangle img = Rectangle.Empty;
			image=null;
			
			if(!this.FullyCustomHeader)
				g.FillRectangle(new SolidBrush(this.headerBackColor),r);

//			if (HeaderImageList.Images.Count>1)
//			{
//				if ((this.Columns.IndexOf(m)==sortColumn)&&(sortOrder==1))
//					image=this.HeaderImageList.Images[1];
//				if ((this.Columns.IndexOf(m)==sortColumn)&&(sortOrder==-1))
//					image=this.HeaderImageList.Images[0];
//			}	
//			if(image != null)
//			{
//				if(m.ImageOnRight)
//					img = new Rectangle(text.Right+4,(r.Height-16)/2,16,16);
//				else
//					img = new Rectangle(r.Left+2,(r.Height-16)/2,16,16);
//			}
			if ((this.Columns.IndexOf(m)==sortColumn)&&((e.State & DrawItemState.Selected)==0))
			{
				img = new Rectangle(text.Right+1,(r.Height-12)/2+5,8,7);
				if (sortOrder<0)
					g.DrawLines(m_ArrowPen,new Point[] {new Point(img.Left,img.Top),
														new Point(img.Right,img.Top),
														new Point(img.Left+(img.Width/2),img.Bottom-img.Height/2),
														new Point(img.Left,img.Top)}); 
//For Filled arrows uncomment this
//					g.FillPolygon(Brushes.Black,new Point[] {new Point(img.Left,img.Top),
//														new Point(img.Right,img.Top),
//														new Point(img.Left+(img.Width/2),img.Bottom-img.Height/2),
//														new Point(img.Left,img.Top)}); 
				else
					g.DrawLines(m_ArrowPen,new Point[] {new Point(img.Left,img.Bottom-img.Height/2),
														new Point(img.Right,img.Bottom-img.Height/2),
														new Point(img.Left+(img.Width/2),img.Top),
														new Point(img.Left,img.Bottom-img.Height/2)}); 
            }

			//This occurs when column is pressed
			if ((e.State & DrawItemState.Selected)!=0)
			{
				g.DrawLine(SystemPens.ControlDark,r.Right-1, r.Bottom-1, r.Right-1, r.Top);
				g.DrawLine(SystemPens.ControlLightLight,r.Right, r.Bottom-1, r.Right, r.Top);

				g.DrawLine(SystemPens.ControlDark,r.Left+2, r.Bottom-3, r.Left+2, r.Top+2);
				g.DrawLine(SystemPens.ControlDark,r.Left+2, r.Top+2, r.Right-3, r.Top+2);
				g.DrawLine(SystemPens.ControlLightLight,r.Right-3, r.Top+2, r.Right-3, r.Bottom-3);
				g.DrawLine(SystemPens.ControlLightLight,r.Right-3, r.Bottom-3, r.Left+2, r.Bottom-3);

				if(image != null)
				{
					img.Offset(1,1);
					g.DrawImage(image,img);
					img.Offset(-1,-1);
				}

				text.Offset(1,1);
				g.DrawString(m.Text,e.Font,new SolidBrush(headerForeColor),text,s);
				text.Offset(-1,-1);
			}
			//Default state
			else
			{
				g.DrawLine(new Pen(this.headerBackColor),r.Right-2, r.Bottom, r.Right-2, r.Top);
				g.DrawLine(new Pen(this.headerBackColor),r.Right-1, r.Bottom, r.Right-1, r.Top);

				g.DrawLine(SystemPens.ControlDark,r.Right-1, r.Bottom-1, r.Right-1, r.Top);
				g.DrawLine(SystemPens.ControlLightLight,r.Right, r.Bottom-1, r.Right, r.Top);
				
				g.DrawString(m.Text,e.Font,new SolidBrush(headerForeColor),text,s);
				if(image != null)
					g.DrawImage(image,img);
				
			}

		}

		#endregion

		#region InsertColumns Method

		void InsertColumns()
		{
			int counter = 0;
			foreach(ListColumn m in myColumns)
			{
				Win32.LVCOLUMN lvc = new Win32.LVCOLUMN();
				lvc.mask = 0x0001|0x0008|0x0002|0x0004;//LVCF_FMT|LVCF_SUBITEM|LVCF_WIDTH|LVCF_TEXT
				lvc.cx = m.Width;
				lvc.subItem = counter;
				lvc.text = m.Text;
				switch(m.TextAlign)
				{
					case HorizontalAlignment.Left:
						lvc.fmt = 0x0000;
						break;
					case HorizontalAlignment.Center:
						lvc.fmt = 0x0002;
						break;
					case HorizontalAlignment.Right:
						lvc.fmt = 0x0001;
						break;
				}
				if(headerImages != null && m.ImageIndex != -1)
				{
					lvc.mask |= 0x0010;//LVCF_IMAGE
					lvc.iImage = m.ImageIndex;
					lvc.fmt |= 0x0800;//LVCFMT_IMAGE
					if(m.ImageOnRight)
						lvc.fmt |= 0x1000;
				}
				//Send message LVN_INSERTCOLUMN
				Win32.SendMessage(this.Handle,0x1000+97,counter,ref lvc);
				//Check if column is set to owner-draw
				//If so - send message HDM_SETITEM with HDF_OWNERDRAW flag set
				if(m.OwnerDraw)
				{
					Win32.HDITEM hdi = new Win32.HDITEM();
					hdi.mask = (int)Win32.HDI.HDI_FORMAT;
					hdi.fmt = (int)Win32.HDF.HDF_OWNERDRAW;
					Win32.SendMessage(header.Handle,0x1200+12,counter,ref hdi);
				}
				counter++;
			}
		}

		#endregion

		#region Public properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ListColumnArray Columns
		{
			get{return myColumns;}
		}

		public int[] HeaderLeft 
		{
			get {return m_headerleft;}
		}
		public int[] HeaderRight 
		{
			get {return m_headerright;}
		}
		public ImageList HeaderImageList
		{
			get{return headerImages;}
			set{headerImages = value;}
		}

		public bool FullyCustomHeader
		{
			get{return ownerDrawHeader;}
			set{ownerDrawHeader = value;}
		}

		public IntPtr HeaderHandle
		{
			get{return header.Handle;}
		}

		public int IncreaseHeaderHeight
		{
			get{return increaseHeaderHeight;}
			set
			{
				increaseHeaderHeight = value;
			}
		}

		public int HeaderHeight
		{
			get{return headerHeight;}
		}

		public bool DefaultCustomDraw
		{
			get{return defaultCustomDraw;}
			set{defaultCustomDraw = value;}
		}

		#endregion

		#region HeaderControl class

		internal class HeaderControl : NativeWindow
		{
			eLePhantListView parent;
			bool mouseDown;
			public HeaderControl(eLePhantListView m)
			{
				parent = m;
				//Get the header control handle
				IntPtr header = Win32.SendMessage(parent.Handle, (0x1000+31), IntPtr.Zero, IntPtr.Zero);
				this.AssignHandle(header);				
			}

			#region Overriden WndProc

			protected override void WndProc(ref Message m)
			{
				switch(m.Msg)
				{
					case 0x000F://WM_PAINT
						if(parent.FullyCustomHeader)
						{
							Win32.RECT update = new Win32.RECT();
							if(Win32.GetUpdateRect(m.HWnd,ref update, false)==0)
								break;
							//Fill the paintstruct
							Win32.PAINTSTRUCT ps = new Win32.PAINTSTRUCT();
							IntPtr hdc = Win32.BeginPaint(m.HWnd, ref ps);
							//Create graphics object from the hdc
							Graphics g = Graphics.FromHdc(hdc);
							//Get the non-item rectangle
							int left = 0;
							Win32.RECT itemRect = new Win32.RECT();
							for(int i=0; i<parent.Columns.Count; i++)
							{								
								//HDM_GETITEMRECT
								Win32.SendMessage(m.HWnd, 0x1200+7, i, ref itemRect);
								left += itemRect.right-itemRect.left;								
							}
							parent.headerHeight = itemRect.bottom-itemRect.top;
							if(left >= ps.rcPaint.left)
								left = ps.rcPaint.left;

                            Rectangle r = new Rectangle(left, ps.rcPaint.top, 
								ps.rcPaint.right-left, ps.rcPaint.bottom-ps.rcPaint.top);
							Rectangle r1 = new Rectangle(ps.rcPaint.left, ps.rcPaint.top, 
								ps.rcPaint.right-left, ps.rcPaint.bottom-ps.rcPaint.top);

							g.FillRectangle(new SolidBrush(parent.headerBackColor),r);

							//If we have a valid event handler - call it
							if(parent.DrawHeader != null && !parent.DefaultCustomDraw)
								parent.DrawHeader(new DrawHeaderEventArgs(g,r,
									itemRect.bottom-itemRect.top));
							else
								parent.DrawHeaderBorder(new DrawHeaderEventArgs(g,r,
									itemRect.bottom-itemRect.top));
							//Now we have to check if we have owner-draw columns and fill
							//the DRAWITEMSTRUCT appropriately
							int counter = 0;
							foreach(ListColumn mm in parent.Columns)
							{
								if(mm.OwnerDraw)
								{
									Win32.DRAWITEMSTRUCT dis = new Win32.DRAWITEMSTRUCT();
									dis.ctrlType = 100;//ODT_HEADER
									dis.hwnd = m.HWnd;
									dis.hdc = hdc;
									dis.itemAction = 0x0001;//ODA_DRAWENTIRE
									dis.itemID = counter;
									//Must find if some item is pressed
									Win32.HDHITTESTINFO hi = new Win32.HDHITTESTINFO();
									hi.pt.X = parent.PointToClient(MousePosition).X;									
									hi.pt.Y = parent.PointToClient(MousePosition).Y;
									int hotItem = Win32.SendMessage(m.HWnd, 0x1200+6, 0, ref hi);
									//If clicked on a divider - we don't have hot item
									if(hi.flags == 0x0004 || hotItem != counter)
										hotItem = -1;
									if(hotItem != -1 && mouseDown)
										dis.itemState = 0x0001;//ODS_SELECTED
									else
										dis.itemState = 0x0020;
									//HDM_GETITEMRECT
									Win32.SendMessage(m.HWnd, 0x1200+7, counter, ref itemRect);
									dis.rcItem = itemRect;
									//Send message WM_DRAWITEM
									Win32.SendMessage(parent.Handle,0x002B,0,ref dis);
								}
								counter++;
							}
							Win32.EndPaint(m.HWnd, ref ps);
							
						}
						else
							base.WndProc(ref m);						
						break;
					case 0x0014://WM_ERASEBKGND
						//We don't need to do anything here in order to reduce flicker
						if(parent.FullyCustomHeader)
							break;						
						else
							base.WndProc(ref m);
						break;
				case 0x0201://WM_LBUTTONDOWN
						mouseDown = true;
						base.WndProc(ref m);
						break;
				case 0x0202://WM_LBUTTONUP
						mouseDown = false;
						base.WndProc(ref m);
						break;
				case 0x1200+5://HDM_LAYOUT
						base.WndProc(ref m);
						break;
				case 0x0030://WM_SETFONT						
						if(parent.IncreaseHeaderHeight > 0)
						{
							System.Drawing.Font f = new System.Drawing.Font(parent.Font.Name,
								parent.Font.SizeInPoints + parent.IncreaseHeaderHeight);
							m.WParam = f.ToHfont();
						}						
                        base.WndProc(ref m);						
						break;
					default:
						base.WndProc(ref m);
						break;
				}
			}

			#endregion
		}

		#endregion		

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
	#region transformationRoutines
		public static string SizeToString(uint size)
		{
			return SizeToString((ulong)size);
		}
		public static string SizeToString(ulong size)
		{
			if(size==0) return "0";
			
			StringBuilder strB=new StringBuilder();
			//strB.Remove(0,strB.Length);
			
			if (size<1024) 
			{ 
				strB.Append(size);
				strB.Append(" "+eLePhantForm.Globalization["LBL_BYTE"]);
			}
			else if ((size>=1024)&&(size<1048576)) 
			{
				strB.Append(size/1024);
				strB.Append(" "+eLePhantForm.Globalization["LBL_KBYTE"]);
			}
			else if ((size>=1048576)&&(size<1073741824))
			{
				strB.Append(((float)size/1048576F).ToString("###0.##"));
				strB.Append(" "+eLePhantForm.Globalization["LBL_MBYTE"]);
			}
			else if (size>=1073741824) 
			{
				strB.Append(((float)size/1073741824F).ToString("###0.##"));
				strB.Append(" "+eLePhantForm.Globalization["LBL_GBYTE"]);
			}
			return strB.ToString();
		}
		public static string SpeedToString(float speed)
		{
//			if (speed<1024) return Convert.ToString(speed)+" B/s";
//			if (speed<10240) return speed.ToString()+" KB/s";
//			if (speed>=102400) return Convert.ToString(speed/1000)+" MB/s";
//			return"";
			StringBuilder strB=new StringBuilder();
			strB.Remove(0,strB.Length);
			if (speed<10240) 
			{
				strB.Append(speed);
				strB.Append(" "+eLePhantForm.Globalization["LBL_KBYTE"]+"/s");
			}
			else 
			{
				strB.Append(speed/1024);
				strB.Append(" "+eLePhantForm.Globalization["LBL_MBYTE"]+"/s");
			}
			return strB.ToString();
		}
		protected string PriorityToString(Types.Constants.Priority priority)
		{
			switch (priority)
			{
				case Types.Constants.Priority.VeryHigh: return eLePhantForm.Globalization["LBL_VERYHIGH"];
				case Types.Constants.Priority.High: return eLePhantForm.Globalization["LBL_HIGH"];
				case Types.Constants.Priority.Normal: return eLePhantForm.Globalization["LBL_NORMAL"];
				case Types.Constants.Priority.Low: return eLePhantForm.Globalization["LBL_LOW"];
				case Types.Constants.Priority.VeryLow: return eLePhantForm.Globalization["LBL_VERYLOW"];
			}
			return "";
		}
		protected string ServerPriorityToString(Types.Constants.ServerPriority priority)
		{
			switch (priority)
			{
				case Types.Constants.ServerPriority.High: return eLePhantForm.Globalization["LBL_HIGH"]; 
				case Types.Constants.ServerPriority.Normal: return eLePhantForm.Globalization["LBL_NORMAL"];
				case Types.Constants.ServerPriority.Low: return eLePhantForm.Globalization["LBL_LOW"];
			}
			return "";
		}
	#endregion
	}

	#region HeaderEventArgs class

	public class HeaderEventArgs : EventArgs
	{
		int columnIndex;
		int mouseButton;
		public HeaderEventArgs(int index, int button)
		{
			columnIndex = index;
			mouseButton = button;
		}
		public int ColumnIndex
		{
			get{return columnIndex;}
		}
		public int MouseButton
		{
			get{return mouseButton;}
		}
	}

		#endregion


	#region DrawHeaderEventArgs class

	public class DrawHeaderEventArgs : EventArgs
	{
		Graphics graphics;
		Rectangle bounds;
		int height;
		public DrawHeaderEventArgs(Graphics dc, Rectangle rect, int h)
		{
			graphics = dc;
			bounds = rect;
			height = h;
		}
		public Graphics Graphics
		{
			get{return graphics;}
		}		
		public Rectangle Bounds
		{
			get{return bounds;}
		}
		public int HeaderHeight
		{
			get{return height;}
		}
	#endregion

	

	}	
}
