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
using System.Drawing.Drawing2D;


namespace eLePhant.Interface
{
	/// <summary>
	/// Descripción breve de eLePhantButton.
	/// </summary>
	public class eLePhantToggleButton : System.Windows.Forms.RadioButton
	{
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Pen focusPen;
		private SolidBrush textBrush;
		private Color m_SelectedBackColor;
		private Color m_MouseUpBackColor;
		private Color m_TextColor;
		private Color m_InactiveBackColor;

		public Color SelectedBackColor
		{
			set
			{
				m_SelectedBackColor=value;
			}
		}

		public Color MouseUpBackColor
		{
			set
			{
				m_MouseUpBackColor=value;
			}
		}

		public Color TextColor
		{
			set
			{
				m_TextColor=value;
				textBrush=new SolidBrush(m_TextColor);
			}
		}

		public Color InactiveBackColor
		{
			set
			{
				m_InactiveBackColor=value;
				if (!Checked) BackColor=m_InactiveBackColor;
			}
		}

		public eLePhantToggleButton(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// Requerido para la compatibilidad con el Diseñador de composiciones de clases Windows.Forms
			/// </summary>
			container.Add(this);
			InitializeComponent();
			base.Appearance = Appearance.Button;

			m_SelectedBackColor=Color.White;
			m_MouseUpBackColor=Color.White;
			m_TextColor=Color.FromArgb(68,69,151);
			m_InactiveBackColor=Color.Transparent;
			BackColor=m_InactiveBackColor;
			focusPen=new Pen(Color.Black,1);
			focusPen.DashStyle=DashStyle.Dot;
			textBrush=new SolidBrush(Color.FromArgb(68,69,151));
			this.FlatStyle=FlatStyle.Popup;
			this.Appearance=Appearance.Button;

		}

		public eLePhantToggleButton()
		{
			/// <summary>
			/// Requerido para la compatibilidad con el Diseñador de composiciones de clases Windows.Forms
			/// </summary>
			InitializeComponent();
			m_SelectedBackColor=Color.White;
			m_MouseUpBackColor=Color.White;
			m_TextColor=Color.FromArgb(68,69,151);
			m_InactiveBackColor=Color.Transparent;
			BackColor=m_InactiveBackColor;
			focusPen=new Pen(Color.Black,1);
			focusPen.DashStyle=DashStyle.Dot;
			textBrush=new SolidBrush(Color.FromArgb(68,69,151));
			this.FlatStyle=FlatStyle.Popup;
			this.Appearance=Appearance.Button;
			//textBrush=new SolidBrush(Color.FromArgb(4481175));

		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.Image==null)
			{
				base.OnPaint(e);
				return;
			}
			if ((this.BackColor!=m_InactiveBackColor/*Color.Transparent*/)||(this.Checked))
				base.OnPaint(e);
			else
			{
				this.InvokePaintBackground(this,e);
				e.Graphics.DrawImage(this.Image,Width/2-Image.Width/2,11);
				if (Focused) e.Graphics.DrawRectangle(focusPen,3,3,Width-5,Height-5);
			}
			StringFormat drawFormat = new StringFormat();
			drawFormat.Alignment = StringAlignment.Center;

			e.Graphics.DrawString(Tag.ToString(),this.Font,textBrush,Width/2,this.Height-this.Font.Height-1, drawFormat);

		}
		protected override void OnMouseEnter(EventArgs e )
		{
			this.BackColor=m_MouseUpBackColor;
		}
		protected override void OnMouseLeave(EventArgs e )
		{
			if (!this.Checked) this.BackColor=m_InactiveBackColor;//Color.Transparent;
		}
		protected override void OnCheckedChanged(EventArgs e)
		{
			if (!this.Checked) this.BackColor=m_InactiveBackColor;
			else this.BackColor=m_SelectedBackColor;
			base.OnCheckedChanged(e);
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
