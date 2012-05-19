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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using eLePhant.eDonkey;

namespace eLePhant.Interface
{
	// use BackColor for background of bar
	// use ForeColor for border around bars
	
	/// <summary>eLePhantInspector displays a zoom of file chunks</summary>
	public class eLePhantInspector : Control
	{
		#region Instance fields
		/// <summary>Store for the BorderWidth property</summary>
		protected int _borderWidth;
		/// <summary>Store for the BarHeight property</summary>
		protected ushort _barHeight;
		/// <summary>Store for the BarSpacing property</summary>
		protected ushort _barSpacing;
		/// <summary>Store for the internal area rectangle</summary>
		protected Rectangle _areaRect;
		/// <summary>Store the single bars</summary>
		ArrayList _bars = new ArrayList();
		/// <summary>File which need to be inspected</summary>
		protected InterfaceFile _file;
		#endregion

		#region Constructors
		/// <summary>Create and initialize the zoom bar control</summary>
		public eLePhantInspector()
		{
			// Prevent flicker with double buffering and all painting inside WM_PAINT
			SetStyle(ControlStyles.DoubleBuffer | 
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint, true);

			// Set initial state
			_borderWidth = 5;
			_barHeight = 29;
			_barSpacing = 3;
			_file = null;

			// recalculate the bars
			Recalculate();
		}
		#endregion

		#region Propertys
		/// <summary>BorderWidth property</summary>
		/// <value>Defines the with between control border and inner bars</value>
		[Category("Appearance")]
		[DefaultValue(5)]
		public int BorderWidth
		{
			get { return _borderWidth; }

			set
			{
				if (_borderWidth != value)
				{
					_borderWidth = value;
					Recalculate();
					Invalidate();
				}
			}
		}

		/// <summary>BarHeight property</summary>
		/// <value>Defines the hight of an single bar</value>
		[Category("Appearance")]
		[DefaultValue(3)]
		public ushort BarHeight
		{
			get { return _barHeight; }

			set
			{
				if (_barHeight != value)
				{
					_barHeight = value;
					Recalculate();
					Invalidate();
				}
			}
		}

		/// <summary>BarSpacing property</summary>
		/// <value>Defines the spacing between the bars</value>
		[Category("Appearance")]
		[DefaultValue(29)]
		public ushort BarSpacing
		{
			get { return _barSpacing; }

			set
			{
				if (_barSpacing != value)
				{
					_barSpacing = value;
					Recalculate();
					Invalidate();
				}
			}
		}

		/// <summary>Set the file which need to be inspected</summary>
		public InterfaceFile File
		{
			get { return _file; }

			set
			{
				if (_file != value)
				{
					_file = value;
					Invalidate();
				}
			}
		}
		#endregion

		#region Calculation
		/// <summary>Recalculate the single bars</summary>
		protected virtual void Recalculate()
		{
			// Calculate inner area rect
			_areaRect = new Rectangle(0, 0, this.Width, this.Height);
			_areaRect.Inflate(- _borderWidth, - _borderWidth);

			// Calculate number of bars
			ushort barCount = 0;
			int height = _areaRect.Height;
			do
			{
				height -= _barHeight;
				if ( height >= 0 )
				{
					barCount++;
					height -= _barSpacing;
				}
			} while ( height >= 0 );

			int space = _barHeight + _barSpacing;
			int top = _areaRect.Top;

			_bars.Clear();
			for(int i = 0; i < barCount; i++)
			{
				Rectangle bar = new Rectangle(_areaRect.Left, top, _areaRect.Width, _barHeight);
				top += space;
				_bars.Add(bar);
			}
		}
		#endregion

		#region Paint
		/// <summary>
		/// Paint a InterfaceFile chunk to any graphics
		/// </summary>
		/// <param name="file"></param>
		/// <param name="g"></param>
		/// <param name="rec"></param>
		/// <param name="PercentBar"></param>
		static public void DrawBar(InterfaceFile file, Graphics g, Rectangle rec, bool PercentBar, bool PercentNumber)
		{
			Rectangle barRect;
			Color end;
			LinearGradientBrush chunkBrush;

			// fill background with white
			//SolidBrush white = new SolidBrush(Color.White);
			//g.FillRectangle(white, rec);

			barRect = new Rectangle(rec.Left, rec.Top, rec.Width, rec.Height);
			//SolidBrush b = new SolidBrush(Color.Red);
			Color b=Color.Red;

			int xpos = barRect.Left;//this.HeaderLeft[column];
			//if (xpos==0) xpos = barRect.Left;

			int ChunkLength;
			for (int i = 0; i < file.ChunksStatus.Length; i++)
			{
				//ChunkLength=(int)Decimal.Round((decimal)(this.HeaderRight[column]-xpos)/(decimal)(file.ChunksStatus.Length-i),0);
				ChunkLength = (int)Decimal.Round((decimal)(barRect.Right-xpos)/(decimal)(file.ChunksStatus.Length-i),0);

				switch(file.ChunksStatus[i])
				{
					case 3: //if completed : green else black
						if(file.Status==9)
							b=System.Drawing.Color.Green;//Color.FromArgb(82, 246, 91);
						else
							b=Color.Black;
						break;
					case 0: b=Color.Red;
						break;
					case 1: b=Color.Gray;
						break;
					case 2: b=Color.LightGray;
						break;
					default:b=Color.Red;
						break;
				}
				if ((file.ChunksStatus[i]==0)&&(file.ChunksAvaibility!=null)&&(file.ChunksAvaibility[i]>0))
				{
					int col=file.ChunksAvaibility[i]*4;
					if (col>200) col=200;
					col=Math.Abs(200-col);
					
					b=Color.FromArgb(255,col,col,255);
				}

				try
				{
					if (ChunkLength<=0) continue;
					// calculate the new end color based on start color
					end = ControlPaint.Dark(b, 0.3F);

					// generate the linear brush
					chunkBrush = new LinearGradientBrush(new Rectangle(xpos, barRect.Top, ChunkLength, barRect.Height), b, end, 90);
					
					g.FillRectangle(chunkBrush, xpos, barRect.Top, ChunkLength, barRect.Height);					
				}
				catch(Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
				xpos += ChunkLength;
			}
			
			decimal percent = file.PercentCompleted;

			if (PercentBar)
			{
				int CompleteChunkLength = (int)(percent*(rec.Right-rec.Left));
				Pen myProgressBarPen = new Pen(Color.FromArgb(82, 246, 91),2); 
				g.DrawLine(myProgressBarPen,rec.Left,rec.Top+1,rec.Left+CompleteChunkLength,rec.Top+1);
			}

			if (PercentNumber)
			{
				g.DrawString(((byte)(percent*100)).ToString()+ " %",new System.Drawing.Font("Microsoft Sans Serif",7.25F,GraphicsUnit.Point),new SolidBrush(Color.White),barRect);
			}


		}

		/// <summary>Draw the inspector</summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				using (SolidBrush pageAreaBrush = new SolidBrush(this.BackColor))
				{
					// Fill background
					e.Graphics.FillRectangle(pageAreaBrush, this.Bounds);
				}

				Pen borderPen = new Pen(this.ForeColor);

				if( (_bars.Count > 0) && (_file != null) )
				{
					// Rectangle for all chunks
					Rectangle barRect = new Rectangle(0, 0, (_areaRect.Width - 2) * _bars.Count, _barHeight - 2);

					// Create temporary bitmap to draw the chunks
					Bitmap barBmp = new Bitmap(barRect.Width, barRect.Height, PixelFormat.Format32bppArgb);
					Graphics g = Graphics.FromImage(barBmp);

					// paint parts to bitmap
					PaintParts(g, barRect);

					// init src rectangle
					Rectangle src = new Rectangle(0, 0, _areaRect.Width - 2, _barHeight - 2);
					foreach(Rectangle bar in _bars)
					{
						// draw border around single bar
						e.Graphics.DrawRectangle(borderPen, bar.Left, bar.Top, bar.Width-1, bar.Height-1);

						// copy parts of bitmap to screen
						bar.Inflate(-1, -1);
						e.Graphics.DrawImage(barBmp, bar, src, GraphicsUnit.Pixel);
						bar.Inflate(1, 1);

						src.Offset(_areaRect.Width - 2, 0);
					}
				}
			
				// paint only empty bars
				if( (_bars.Count > 0) && (_file == null) )
				{
					foreach(Rectangle bar in _bars)
					{
						// draw border around single bar
						e.Graphics.DrawRectangle(borderPen, bar.Left, bar.Top, bar.Width-1, bar.Height-1);

						Color start = Color.FromArgb(255, 255, 255);
						Color end = Color.FromArgb(220, 217, 220); 
						LinearGradientBrush barBrush = new LinearGradientBrush(bar, start, end, 90);

						bar.Inflate(-1, -1);
						e.Graphics.FillRectangle(barBrush, bar);
						bar.Inflate(1, 1);
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		/// <summary>paint the chunks to the bar</summary>
		/// <param name="g">Graphics where we draw the chunks</param>
		/// <param name="rec">Defines the Rectangle of the full area</param>
		protected void PaintChunks(Graphics g, Rectangle rec)
		{
			// currently nothing
		}

		/// <summary>paint the parts to the bar</summary>
		/// <param name="g">Graphics where we draw the parts</param>
		/// <param name="rec">Defines the Rectangle of the full area</param>
		protected void PaintParts(Graphics g, Rectangle rec)
		{
			try
			{
				// initialize the colors
				Color start = Color.FromArgb(255, 255, 255);
				Color end = Color.FromArgb(220, 217, 220); 
			
				int xpos = rec.Left;
			
				int ChunkLength;
				for (int i = 0; i < _file.ChunksStatus.Length; i++)
				{
					ChunkLength = (int)Decimal.Round((decimal)(rec.Width-xpos)/(decimal)(_file.ChunksStatus.Length-i), 0);
	
					switch(_file.ChunksStatus[i])
					{
						case 3:
							// color = black -> green
							if(_file.Status==9)
								start = Color.FromArgb(82, 246, 91);	
							else
								start = Color.FromArgb(80, 80, 80);
							break;
						case 0:
							// color = red
							start = System.Drawing.Color.FromArgb(255, 91, 77);
							break;
						case 1:
							// color = gray
							start = Color.FromArgb(194, 194, 194);
							break;
						case 2:
							// color = lightgray -> white
							start = Color.FromArgb(255, 255, 255);
							break;
						default:
							// color = red
							start = System.Drawing.Color.FromArgb(255, 91, 77);
							break;
					}
					if ( (_file.ChunksStatus[i] == 0) &&
						(_file.ChunksAvaibility != null) &&
						(_file.ChunksAvaibility[i] > 0) )
					{
						int col = _file.ChunksAvaibility[i] * 4;
						if (col > 200) col = 200;
						col = Math.Abs(200-col);
					
						// color = blue
						start = Color.FromArgb(255, col, col, 255);
					}

					// calculate the new end color based on start color
					end = ControlPaint.Dark(start, 0.3F);

					// generate the linear brush
					LinearGradientBrush chunkBrush = new LinearGradientBrush(rec, start, end, 90);

					// fill the part area
					g.FillRectangle(chunkBrush, xpos, rec.Top, ChunkLength, rec.Height);
					xpos += ChunkLength;
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}
		#endregion

		#region Overriden Methods
		/// <summary>Recalculate the bars if control was resized.</summary>
		protected override void OnResize(EventArgs e)
		{
			Recalculate();
			Invalidate();

			base.OnResize(e);
		}

		/// <summary>Recalculate the bars if control size was changed.</summary>
		protected override void OnSizeChanged(EventArgs e)
		{
			Recalculate();
			Invalidate();

			base.OnSizeChanged(e);
		}
		#endregion
	}
}