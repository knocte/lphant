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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace eLePhant.Interface
{
	/// <summary>
	/// Class to display up/down speed graphic use SetMinValue and SetMaxValue to set the limits and AddValue to add new values to the graphic
	/// </summary>
	public class eLePhantGraphic : PictureBox
	{
		private float maxvalue;
		private float minvalue;
		private Pen gridPen;
		private Pen downPen;
		private Pen upPen;
		private Rectangle screen;
		private Graphics clientDC;
		private Font txtFont;
		private ContextMenu ContextMenuGraphic;
		private ArrayList downValues;
		private ArrayList upValues;
		private ArrayList allDownValues;
		private ArrayList allUpValues;
		private System.ComponentModel.Container components = null;
		private int m_Scale;
		private float m_SumUpForAverage;
		private float m_SumDownForAverage;
		private uint m_AvgCount;
		private uint m_TotalValues;
		private bool m_AutoScale;
		private Thread m_ReScaleThread;
		private bool m_IsRescaling;

		public void SetMinValue(float in_min){minvalue=in_min;}
		public void SetMaxValue(float in_max){maxvalue=in_max;}

		private string m_seconds;
		private string m_minutes;
		private string m_hours;
		private string m_auto;

		private const int margin=2;
		
		public string strAuto
		{
			set
			{
				m_auto=value;
			}
		}

		public string strSeconds
		{
			set
			{
				m_seconds=value;
			}
		}

		public string strMinutes
		{
			set
			{
				m_minutes=value;
			}
		}

		public string strHours
		{
			set
			{
				m_hours=value;
			}
		}

		public int GraphicScale
		{
			get
			{
				return m_Scale;
			}
			set
			{
				m_Scale=value;
				if(m_AutoScale) m_Scale=1;
			}
		}

		public bool AutoGraphicScale
		{
			get
			{
				return m_AutoScale;
			}
			set
			{
				m_AutoScale=value;
				if(m_AutoScale) m_Scale=1;
			}
		}

		public Color GridColor
		{
			set
			{
				gridPen=new Pen(value);
			}
		}

		public Color DownColor
		{
			set
			{
				downPen=new Pen(value);
			}
		}

		public Color UpColor
		{
			set
			{
				upPen=new Pen(value);
			}
		}

		public int GraphicLinesWidth
		{
			set
			{
				downPen.Width=value;
				upPen.Width=value;
			}
		}
		public eLePhantGraphic(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// 
			/// </summary>
			container.Add(this);
			InitializeComponent();
			
			maxvalue=27.0F;
			minvalue=0.0F;
			gridPen=new Pen(Color.FromArgb(222, 231, 247));
			downPen=Pens.LightGreen;
			upPen=Pens.Red;
			txtFont=new Font("System",6);
			downValues=new ArrayList();
			upValues=new ArrayList();
			allDownValues=new ArrayList();
			allUpValues=new ArrayList();
			m_Scale=1;
			m_SumUpForAverage=0;
			m_SumDownForAverage=0;
			m_AvgCount=0;
			m_TotalValues=0;
			m_IsRescaling=false;
			m_AutoScale=false;
			m_CreateMenu();
			ContextMenu=ContextMenuGraphic;
			ContextMenu.Popup+=new System.EventHandler(m_ContextMenuPopup);
			m_seconds="seconds";
			m_minutes="minutes";
			m_hours="hours";
			m_auto="auto";
		}

		~eLePhantGraphic()
		{
			if (m_ReScaleThread!=null)
				if (m_ReScaleThread.IsAlive) m_ReScaleThread.Abort();
		}


		private void m_CreateMenu()
		{
			ContextMenuGraphic = new ContextMenu();
			string menuText="";
			int interval=Width;
			for (int i=0;i<8;i++)
			{
				if (interval<=60) menuText=(interval).ToString()+" "+m_seconds;
				else if ((interval>60)&&(interval<=3600)) menuText=(interval/60).ToString()+" "+m_minutes;
				else if (interval>3600) menuText=(interval/3600).ToString()+" "+m_hours;
								
				if(i==6)
				{
						ContextMenuGraphic.MenuItems.Add("-");
						continue;
				}
				if(i==7) menuText = m_auto;
				
				MenuItem subMenu = new MenuItem(menuText, new EventHandler(m_OnScaleChanged)); 
				subMenu.RadioCheck=true;
				ContextMenuGraphic.MenuItems.Add(subMenu);
				interval*=3;
			}
			
			if (ContextMenu!=null) ContextMenu.Popup-=new System.EventHandler(m_ContextMenuPopup);
			ContextMenu=ContextMenuGraphic;
			ContextMenu.Popup+=new System.EventHandler(m_ContextMenuPopup);
		}

		private void m_ContextMenuPopup(object sender, System.EventArgs e)
		{	
			ContextMenuGraphic.MenuItems[7].Text = m_AutoScaleValue();

			if(m_AutoScale)
			{
				for (int i=0;i<6;i++)
					ContextMenuGraphic.MenuItems[i].Checked=false;

				ContextMenuGraphic.MenuItems[7].Checked=true;
				return;
			}

			for (int i=0;i<6;i++)
			{
				if (Math.Pow(3,i)==m_Scale) (ContextMenuGraphic.MenuItems[i]).Checked=true;
					else (ContextMenuGraphic.MenuItems[i]).Checked=false;
			}
				ContextMenuGraphic.MenuItems[7].Checked=false;
		}
		
		private string m_AutoScaleValue()
		{
			string AutoValue = " (";
			try
			{
				int MaxScale = ((int)(m_TotalValues/screen.Width)+1)*screen.Width;
				TimeSpan TimeScale = TimeSpan.FromSeconds(MaxScale);
			
				if(TimeScale.Hours>0) AutoValue+=TimeScale.Hours.ToString() + " " + m_hours+ " ";
				if(TimeScale.Minutes>0) AutoValue+=TimeScale.Minutes.ToString() + " " + m_minutes;
			
				AutoValue+=")";
				return m_auto+" "+AutoValue;
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
				return m_auto;
			}
			
		}

		private void m_OnScaleChanged(object sender, System.EventArgs e)
		{
			int i=ContextMenuGraphic.MenuItems.IndexOf((MenuItem)sender);
			if (i<0) return;
			if(i<7)
			{
				m_Scale=(int)Math.Pow(3,i);
				m_AutoScale=false;
			}
			else
			{
				m_Scale=(int)((m_TotalValues/screen.Width)+1);
				m_AutoScale=true;
			}
			
			if(!m_IsRescaling) m_StartReScale();
		}
		protected override void OnPaint(PaintEventArgs pe)
		{
			try
			{
				if (gridPen==null)
				{
					base.OnPaint(pe);
					return;
				}
				clientDC=pe.Graphics;
				//clientDC.Clear(Color.Black);
				screen=new Rectangle(margin,margin,this.Width-margin-margin,this.Height-margin-margin);
				clientDC.DrawLine(gridPen,margin,screen.Height/2+margin,screen.Right,screen.Height/2+margin);
				clientDC.DrawRectangle(gridPen,margin,screen.Height/4+margin,screen.Width,screen.Height*2/4);
				clientDC.DrawRectangle(gridPen,screen);

				clientDC.DrawString(maxvalue.ToString(),txtFont,gridPen.Brush,margin,margin-1);
				//clientDC.DrawString(minvalue.ToString(),txtFont,Brushes.Green,margin,screen.Bottom-7);
				clientDC.DrawString(((maxvalue-minvalue)/2).ToString(),txtFont,gridPen.Brush,margin,screen.Bottom/2+margin/2-1);
				//clientDC.DrawString(((maxvalue-minvalue)/4).ToString(),txtFont,Brushes.Green,1,screen.Bottom*3/4);
				//clientDC.DrawString(((maxvalue-minvalue)*3/4).ToString(),txtFont,Brushes.Green,1,screen.Bottom/4);
			
				if ((downValues.Count==0)||upValues.Count==0) return;
				int pos;
				int leftPos=screen.Left;
				int antx=leftPos+1;
				int anty=(int)(screen.Bottom-((float)downValues[0]*(float)screen.Height/(float)maxvalue));//screen.Bottom;
			
				lock(downValues.SyncRoot)
				{
					lock(upValues.SyncRoot)
					{

						foreach (float valor in downValues)
						{
							pos=(int)(screen.Bottom-(valor*(float)screen.Height/(float)maxvalue));
							if (pos<screen.Top) pos=screen.Top;
							clientDC.DrawLine(downPen,antx, anty,leftPos,pos);
							antx=leftPos;
							leftPos+=1;
							anty=pos;
						}
						
						leftPos=screen.Left;
						antx=leftPos+1;
						anty=(int)(screen.Bottom-((float)upValues[0]*(float)screen.Height/(float)maxvalue));//screen.Bottom;
						
						foreach (float valor in upValues)
						{
							pos=(int)(screen.Bottom-(valor*(float)screen.Height/(float)maxvalue));
							if (pos<screen.Top) pos=screen.Top;
							clientDC.DrawLine(upPen,antx, anty,leftPos,pos);
							antx=leftPos;
							leftPos+=1;
							anty=pos;
						}
					}
				}
			}
			catch(InvalidOperationException ie)
			{
				Debug.WriteLine(ie.ToString());
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}

		public void AddValue(float downRate, float upRate)
		{
			m_TotalValues++;
			allDownValues.Add(downRate);
			allUpValues.Add(upRate);
						
			if(m_AutoScale && ((int)(m_TotalValues/screen.Width)+1)!=m_Scale)
			{
				m_Scale=(int)(m_TotalValues/screen.Width)+1;
				m_StartReScale();
			}

			if(m_IsRescaling) return;

			m_SumUpForAverage+=upRate;
			m_SumDownForAverage+=downRate;
			m_AvgCount++;
			
			try
			{
				if (m_AvgCount>=m_Scale)
				{
					while (downValues.Count>screen.Width) 
						downValues.RemoveAt(0);
					downValues.Add(m_SumDownForAverage/m_AvgCount);
					while (upValues.Count>screen.Width) 
						upValues.RemoveAt(0);
					upValues.Add(m_SumUpForAverage/m_AvgCount);
					this.Refresh();
				
					m_SumUpForAverage=0;
					m_SumDownForAverage=0;
					m_AvgCount=0;
						
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}
		
		private void m_StartReScale()
		{	
			if(m_IsRescaling) return;

			m_ReScaleThread=new Thread(new ThreadStart(m_ReScaleValues));
			m_ReScaleThread.Name = "m_ReScaleValues";
			m_ReScaleThread.Start();
		}

		private void m_ReScaleValues()
		{
			float SumDown=0;
			float SumUp=0;
			uint Count=0;
			uint RefreshInterval=0;

			m_IsRescaling = true;
		
			try
			{
				downValues.Clear();
				upValues.Clear();

				for(int i=0;i<m_TotalValues;i=i+m_Scale)
				{
					for(int j=0;j<m_Scale;j++)
					{
						Count++;
						if(i+j>m_TotalValues-1) break;
				
						SumDown+=(float)allDownValues[i+j];
						SumUp+=(float)allUpValues[i+j];
					}

					while (downValues.Count>screen.Width) 
						downValues.RemoveAt(0);
					downValues.Add(SumDown/Count);
					while (upValues.Count>screen.Width) 
						upValues.RemoveAt(0);
					upValues.Add(SumUp/Count);
										
					RefreshInterval++;												
					
					if(RefreshInterval%10==0)
					{
						this.Refresh();
						RefreshInterval=0;
					}

					SumDown=0;
					SumUp=0;
					Count=0;
				}
							
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
			finally
			{
				this.Refresh();
				m_IsRescaling = false;
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			//int antWidth=Width;
			base.OnSizeChanged(e);
			this.Refresh();
			//if (Width!=antWidth)
			m_CreateMenu();
		}
		public void UpdateGraphic()
		{
		}
		public eLePhantGraphic()
		{
			InitializeComponent();
		}

		#region Component Designer generated code
		/// <summary>
		/// Método necesario para admitir el Diseñador, no se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
