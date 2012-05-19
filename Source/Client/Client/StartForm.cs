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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using eLePhant.Classes;

namespace eLePhant.Client
{
	/// <summary>
	/// Starting form
	/// </summary>
	public class StartForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton IniciarLocal;
		private System.Windows.Forms.RadioButton IniciarServicio;
		private System.Windows.Forms.RadioButton ConectarRemoto;
		private System.Windows.Forms.TextBox IPRemota1;
		private System.Windows.Forms.Label IP;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox IPRemota4;
		private System.Windows.Forms.TextBox IPRemota3;
		private System.Windows.Forms.TextBox IPRemota2;
		private System.Windows.Forms.TextBox pw;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox PuertoR;
		private CedonkeyCRemoto CRemoto;
		
		private System.ComponentModel.Container components = null;

		private static Mutex m_Mutex;
		private static string m_elink;		

		public StartForm(string in_elink,Mutex in_mutex)
		{
			m_Mutex=in_mutex;
			m_elink=in_elink;
			InitializeComponent();
			this.PuertoR.Enabled=false;
			this.pw.Enabled=false;
			this.IPRemota1.Enabled=false;
			this.IPRemota2.Enabled=false;
			this.IPRemota3.Enabled=false;
			this.IPRemota4.Enabled=false;
		}

		/// <summary>
		/// Clean system resources
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

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.PuertoR = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.pw = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.IPRemota4 = new System.Windows.Forms.TextBox();
			this.IPRemota3 = new System.Windows.Forms.TextBox();
			this.IPRemota2 = new System.Windows.Forms.TextBox();
			this.IP = new System.Windows.Forms.Label();
			this.IPRemota1 = new System.Windows.Forms.TextBox();
			this.ConectarRemoto = new System.Windows.Forms.RadioButton();
			this.IniciarServicio = new System.Windows.Forms.RadioButton();
			this.IniciarLocal = new System.Windows.Forms.RadioButton();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.PuertoR);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.pw);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.IPRemota4);
			this.groupBox1.Controls.Add(this.IPRemota3);
			this.groupBox1.Controls.Add(this.IPRemota2);
			this.groupBox1.Controls.Add(this.IP);
			this.groupBox1.Controls.Add(this.IPRemota1);
			this.groupBox1.Controls.Add(this.ConectarRemoto);
			this.groupBox1.Controls.Add(this.IniciarServicio);
			this.groupBox1.Controls.Add(this.IniciarLocal);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(328, 184);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Interface connection:";
			// 
			// PuertoR
			// 
			this.PuertoR.Location = new System.Drawing.Point(184, 128);
			this.PuertoR.Name = "PuertoR";
			this.PuertoR.Size = new System.Drawing.Size(48, 20);
			this.PuertoR.TabIndex = 9;
			this.PuertoR.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(40, 128);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(88, 16);
			this.label6.TabIndex = 16;
			this.label6.Text = "Remote port:";
			// 
			// pw
			// 
			this.pw.Location = new System.Drawing.Point(112, 152);
			this.pw.Name = "pw";
			this.pw.PasswordChar = '*';
			this.pw.Size = new System.Drawing.Size(120, 20);
			this.pw.TabIndex = 10;
			this.pw.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(40, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 16);
			this.label5.TabIndex = 14;
			this.label5.Text = "Password:";
			// 
			// IPRemota4
			// 
			this.IPRemota4.Location = new System.Drawing.Point(208, 104);
			this.IPRemota4.Name = "IPRemota4";
			this.IPRemota4.Size = new System.Drawing.Size(24, 20);
			this.IPRemota4.TabIndex = 7;
			this.IPRemota4.Text = "";
			// 
			// IPRemota3
			// 
			this.IPRemota3.Location = new System.Drawing.Point(176, 104);
			this.IPRemota3.Name = "IPRemota3";
			this.IPRemota3.Size = new System.Drawing.Size(24, 20);
			this.IPRemota3.TabIndex = 6;
			this.IPRemota3.Text = "";
			// 
			// IPRemota2
			// 
			this.IPRemota2.Location = new System.Drawing.Point(144, 104);
			this.IPRemota2.Name = "IPRemota2";
			this.IPRemota2.Size = new System.Drawing.Size(24, 20);
			this.IPRemota2.TabIndex = 5;
			this.IPRemota2.Text = "";
			// 
			// IP
			// 
			this.IP.Location = new System.Drawing.Point(40, 112);
			this.IP.Name = "IP";
			this.IP.Size = new System.Drawing.Size(16, 16);
			this.IP.TabIndex = 5;
			this.IP.Text = "IP";
			// 
			// IPRemota1
			// 
			this.IPRemota1.Location = new System.Drawing.Point(112, 104);
			this.IPRemota1.Name = "IPRemota1";
			this.IPRemota1.Size = new System.Drawing.Size(24, 20);
			this.IPRemota1.TabIndex = 4;
			this.IPRemota1.Text = "";
			// 
			// ConectarRemoto
			// 
			this.ConectarRemoto.Location = new System.Drawing.Point(24, 80);
			this.ConectarRemoto.Name = "ConectarRemoto";
			this.ConectarRemoto.Size = new System.Drawing.Size(264, 16);
			this.ConectarRemoto.TabIndex = 3;
			this.ConectarRemoto.Text = "Remote (Connect to a remote lphant)";
			this.ConectarRemoto.CheckedChanged += new System.EventHandler(this.ConectarRemoto_CheckedChanged);
			// 
			// IniciarServicio
			// 
			this.IniciarServicio.Enabled = false;
			this.IniciarServicio.Location = new System.Drawing.Point(24, 56);
			this.IniciarServicio.Name = "IniciarServicio";
			this.IniciarServicio.Size = new System.Drawing.Size(184, 16);
			this.IniciarServicio.TabIndex = 2;
			this.IniciarServicio.Text = "Local (Start lphant service";
			// 
			// IniciarLocal
			// 
			this.IniciarLocal.Checked = true;
			this.IniciarLocal.Location = new System.Drawing.Point(24, 32);
			this.IniciarLocal.Name = "IniciarLocal";
			this.IniciarLocal.Size = new System.Drawing.Size(184, 16);
			this.IniciarLocal.TabIndex = 1;
			this.IniciarLocal.TabStop = true;
			this.IniciarLocal.Text = "Local (Start lphant).";
			this.IniciarLocal.CheckedChanged += new System.EventHandler(this.IniciarLocal_CheckedChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(144, 200);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(88, 24);
			this.button1.TabIndex = 11;
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.Accept_OnClick);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(240, 200);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(96, 24);
			this.button2.TabIndex = 12;
			this.button2.Text = "Cancel";
			this.button2.Click += new System.EventHandler(this.Cancel_OnClick);
			// 
			// StartForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(344, 226);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.Name = "StartForm";
			this.Text = "Start lphant";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void Cancel_OnClick(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void Accept_OnClick(object sender, System.EventArgs e)
		{
			string IP=IPRemota1.Text + "." + IPRemota2.Text + "." + IPRemota3.Text + "." + IPRemota4.Text;	
			if (this.ConectarRemoto.Checked) 
			{
				if (CRemoto==null)
					CRemoto=new CedonkeyCRemoto();

				int puertor;
				if (this.PuertoR.Text=="")			
					puertor=4670;
				else
					puertor=System.Convert.ToInt32(this.PuertoR.Text);

				CRemoto.DisConnect();
				if (CRemoto.Connect(IP,pw.Text,puertor)) 
				{
					eLePhant.Client.eLePhantForm FormeLePhant=new eLePhantForm(m_elink,m_Mutex,CRemoto.interfazremota);
					FormeLePhant.Show();
					this.Hide();
				}
				else
					MessageBox.Show("Can not connect or invalid password","lphant remote connection",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			if (this.IniciarLocal.Checked) 
			{
				eLePhant.Client.eLePhantForm FormeLePhant=new eLePhantForm(m_elink,m_Mutex);
				this.Hide();
				FormeLePhant.Show();
			}
		}

		private void IniciarLocal_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.IniciarLocal.Checked)
			{
				this.PuertoR.Enabled=false;
				this.pw.Enabled=false;
				this.IPRemota1.Enabled=false;
				this.IPRemota2.Enabled=false;
				this.IPRemota3.Enabled=false;
				this.IPRemota4.Enabled=false;
			}
		}

		private void ConectarRemoto_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.ConectarRemoto.Checked)
			{
				this.PuertoR.Enabled=true;
				this.pw.Enabled=true;
				this.IPRemota1.Enabled=true;
				this.IPRemota2.Enabled=true;
				this.IPRemota3.Enabled=true;
				this.IPRemota4.Enabled=true;
			}
		}
	}
}
