using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;


namespace eLePhant.Client
{
	/// <summary>
	/// Descripción breve de FormConnections.
	/// </summary>
	public class FormConnections : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		public static Classes.Globalization Globalization;
		private System.Windows.Forms.Button buttonAccept;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textDown1;
		private System.Windows.Forms.TextBox textDown2;
		private System.Windows.Forms.TextBox textDown3;
		private System.Windows.Forms.TextBox textDown4;
		private System.Windows.Forms.TextBox textUp1;
		private System.Windows.Forms.TextBox textUp2;
		private System.Windows.Forms.TextBox textUp3;
		private System.Windows.Forms.TextBox textUp4;
		private System.Windows.Forms.Label lb_cn1;
		private System.Windows.Forms.Label lb_cn2;
		private System.Windows.Forms.Label lb_cn3;
		private System.Windows.Forms.Label lb_cn4;
		private System.Windows.Forms.Label labelUP;
		private System.Windows.Forms.Label labelDOWN;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConnections()
		{
			//
			// Necesario para admitir el Diseñador de Windows Forms
			//
			InitializeComponent();
			Initialize();

			if (eLePhantForm.altConnections.Count>0)
			{
				this.textDown1.Text=((eLePhantForm.connection)eLePhantForm.altConnections[0]).down.ToString();				
				this.textUp1.Text=((eLePhantForm.connection)eLePhantForm.altConnections[0]).up.ToString();
			}
				
			if (eLePhantForm.altConnections.Count>1)
			{
				this.textDown2.Text=((eLePhantForm.connection)eLePhantForm.altConnections[1]).down.ToString();				
				this.textUp2.Text=((eLePhantForm.connection)eLePhantForm.altConnections[1]).up.ToString();
			}

			if (eLePhantForm.altConnections.Count>2)
			{
				this.textDown3.Text=((eLePhantForm.connection)eLePhantForm.altConnections[2]).down.ToString();				
				this.textUp3.Text=((eLePhantForm.connection)eLePhantForm.altConnections[2]).up.ToString();
			}

			if (eLePhantForm.altConnections.Count>3)
			{
				this.textDown4.Text=((eLePhantForm.connection)eLePhantForm.altConnections[3]).down.ToString();				
				this.textUp4.Text=((eLePhantForm.connection)eLePhantForm.altConnections[3]).up.ToString();
			}

			//
			// TODO: agregar código de constructor después de llamar a InitializeComponent
			//
		}

		private void Initialize()
		{
			
			m_Globalize();			
			m_ApplySkin();
			
		}

		private void m_ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			//this.tabControl1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			m_DoSkin(this);
		}

		private void m_DoSkin(Control parentControl)
		{

			foreach (Control c in parentControl.Controls)
			{
				if (c.GetType()==typeof(Label))
				{
					((Label)c).ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
					((Label)c).BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");
				}
				else if (c.GetType()==typeof(Button))
				{
					((Button)c).BackColor=eLePhantForm.Skin.GetColor("ButtonBackColor");
					((Button)c).ForeColor=eLePhantForm.Skin.GetColor("ButtonForeColor");
				}
				else if	(c.GetType()==typeof(TextBox))
				{
					((TextBox)c).ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
					((TextBox)c).BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");
				}
				else if	(c.GetType()==typeof(ListBox))
				{
					((ListBox)c).ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
					((ListBox)c).BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");
				}
				else if	(c.GetType()==typeof(ComboBox))
				{
					((ComboBox)c).ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
					((ComboBox)c).BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");
				}
				else if	(c.GetType()==typeof(RadioButton))
				{
					((RadioButton)c).ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
					((RadioButton)c).BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");
				}
				else if	(c.GetType()==typeof(GroupBox))
				{
					((GroupBox)c).ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
					if ((c.Controls!=null)&&(c.Controls.Count>0)) m_DoSkin(c);
				}
				else if (c.GetType()==typeof(CheckBox))
				{
					((CheckBox)c).ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
					((CheckBox)c).BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");
				}
				else if ((c.Controls!=null)&&(c.Controls.Count>0)) m_DoSkin(c);
			}
		}

		private void m_Globalize()
		{
			buttonAccept.Text = eLePhantForm.Globalization["LBL_ACCEPT"];
			buttonCancel.Text = eLePhantForm.Globalization["LBL_CANCEL"];
			lb_cn1.Text=eLePhantForm.Globalization["LBL_PROFILE"]+" 1:";
			lb_cn2.Text=eLePhantForm.Globalization["LBL_PROFILE"]+" 2:";
			lb_cn3.Text=eLePhantForm.Globalization["LBL_PROFILE"]+" 3:";
			lb_cn4.Text=eLePhantForm.Globalization["LBL_PROFILE"]+" 4:";
			labelDOWN.Text=eLePhantForm.Globalization["LBL_DO"];
			labelUP.Text=eLePhantForm.Globalization["LBL_UP"];
		}

		/// <summary>
		/// Limpiar los recursos que se estén utilizando.
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
			this.textDown1 = new System.Windows.Forms.TextBox();
			this.textDown2 = new System.Windows.Forms.TextBox();
			this.textDown3 = new System.Windows.Forms.TextBox();
			this.textDown4 = new System.Windows.Forms.TextBox();
			this.textUp1 = new System.Windows.Forms.TextBox();
			this.textUp2 = new System.Windows.Forms.TextBox();
			this.textUp3 = new System.Windows.Forms.TextBox();
			this.textUp4 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lb_cn1 = new System.Windows.Forms.Label();
			this.lb_cn2 = new System.Windows.Forms.Label();
			this.lb_cn3 = new System.Windows.Forms.Label();
			this.lb_cn4 = new System.Windows.Forms.Label();
			this.labelUP = new System.Windows.Forms.Label();
			this.labelDOWN = new System.Windows.Forms.Label();
			this.buttonAccept = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// textDown1
			// 
			this.textDown1.Location = new System.Drawing.Point(80, 24);
			this.textDown1.Name = "textDown1";
			this.textDown1.Size = new System.Drawing.Size(32, 20);
			this.textDown1.TabIndex = 0;
			this.textDown1.Text = "";
			this.textDown1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UpDown);
			// 
			// textDown2
			// 
			this.textDown2.Location = new System.Drawing.Point(80, 48);
			this.textDown2.Name = "textDown2";
			this.textDown2.Size = new System.Drawing.Size(32, 20);
			this.textDown2.TabIndex = 1;
			this.textDown2.Text = "";
			this.textDown2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UpDown);
			// 
			// textDown3
			// 
			this.textDown3.Location = new System.Drawing.Point(80, 72);
			this.textDown3.Name = "textDown3";
			this.textDown3.Size = new System.Drawing.Size(32, 20);
			this.textDown3.TabIndex = 2;
			this.textDown3.Text = "";
			this.textDown3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UpDown);
			// 
			// textDown4
			// 
			this.textDown4.Location = new System.Drawing.Point(80, 96);
			this.textDown4.Name = "textDown4";
			this.textDown4.Size = new System.Drawing.Size(32, 20);
			this.textDown4.TabIndex = 3;
			this.textDown4.Text = "";
			this.textDown4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UpDown);
			// 
			// textUp1
			// 
			this.textUp1.Location = new System.Drawing.Point(136, 24);
			this.textUp1.Name = "textUp1";
			this.textUp1.Size = new System.Drawing.Size(32, 20);
			this.textUp1.TabIndex = 4;
			this.textUp1.Text = "";
			this.textUp1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UpDown);
			// 
			// textUp2
			// 
			this.textUp2.Location = new System.Drawing.Point(136, 48);
			this.textUp2.Name = "textUp2";
			this.textUp2.Size = new System.Drawing.Size(32, 20);
			this.textUp2.TabIndex = 5;
			this.textUp2.Text = "";
			this.textUp2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UpDown);
			// 
			// textUp3
			// 
			this.textUp3.Location = new System.Drawing.Point(136, 72);
			this.textUp3.Name = "textUp3";
			this.textUp3.Size = new System.Drawing.Size(32, 20);
			this.textUp3.TabIndex = 6;
			this.textUp3.Text = "";
			this.textUp3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UpDown);
			// 
			// textUp4
			// 
			this.textUp4.Location = new System.Drawing.Point(136, 96);
			this.textUp4.Name = "textUp4";
			this.textUp4.Size = new System.Drawing.Size(32, 20);
			this.textUp4.TabIndex = 7;
			this.textUp4.Text = "";
			this.textUp4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UpDown);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(112, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(24, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "Kb";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(112, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(24, 16);
			this.label2.TabIndex = 9;
			this.label2.Text = "Kb";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(112, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(24, 16);
			this.label3.TabIndex = 10;
			this.label3.Text = "Kb";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(112, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 16);
			this.label4.TabIndex = 11;
			this.label4.Text = "Kb";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(168, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(24, 16);
			this.label5.TabIndex = 12;
			this.label5.Text = "Kb";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(168, 72);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(24, 16);
			this.label6.TabIndex = 13;
			this.label6.Text = "Kb";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(168, 48);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(24, 16);
			this.label7.TabIndex = 14;
			this.label7.Text = "Kb";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(168, 96);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(24, 16);
			this.label8.TabIndex = 15;
			this.label8.Text = "Kb";
			// 
			// lb_cn1
			// 
			this.lb_cn1.Location = new System.Drawing.Point(8, 24);
			this.lb_cn1.Name = "lb_cn1";
			this.lb_cn1.Size = new System.Drawing.Size(72, 16);
			this.lb_cn1.TabIndex = 16;
			this.lb_cn1.Text = "Connection1";
			// 
			// lb_cn2
			// 
			this.lb_cn2.Location = new System.Drawing.Point(8, 48);
			this.lb_cn2.Name = "lb_cn2";
			this.lb_cn2.Size = new System.Drawing.Size(72, 16);
			this.lb_cn2.TabIndex = 17;
			this.lb_cn2.Text = "Connection2";
			// 
			// lb_cn3
			// 
			this.lb_cn3.Location = new System.Drawing.Point(8, 72);
			this.lb_cn3.Name = "lb_cn3";
			this.lb_cn3.Size = new System.Drawing.Size(72, 16);
			this.lb_cn3.TabIndex = 18;
			this.lb_cn3.Text = "Connection3";
			// 
			// lb_cn4
			// 
			this.lb_cn4.Location = new System.Drawing.Point(8, 96);
			this.lb_cn4.Name = "lb_cn4";
			this.lb_cn4.Size = new System.Drawing.Size(72, 16);
			this.lb_cn4.TabIndex = 19;
			this.lb_cn4.Text = "Connection4";
			// 
			// labelUP
			// 
			this.labelUP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelUP.Location = new System.Drawing.Point(144, 8);
			this.labelUP.Name = "labelUP";
			this.labelUP.Size = new System.Drawing.Size(24, 16);
			this.labelUP.TabIndex = 20;
			this.labelUP.Text = "UP";
			// 
			// labelDOWN
			// 
			this.labelDOWN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelDOWN.Location = new System.Drawing.Point(72, 8);
			this.labelDOWN.Name = "labelDOWN";
			this.labelDOWN.Size = new System.Drawing.Size(48, 16);
			this.labelDOWN.TabIndex = 21;
			this.labelDOWN.Text = "DOWN";
			this.labelDOWN.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// buttonAccept
			// 
			this.buttonAccept.Location = new System.Drawing.Point(16, 120);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.TabIndex = 22;
			this.buttonAccept.Text = "OK";
			this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(104, 120);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 23;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(200, 152);
			this.panel1.TabIndex = 24;
			// 
			// FormConnections
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(200, 152);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonAccept);
			this.Controls.Add(this.labelDOWN);
			this.Controls.Add(this.labelUP);
			this.Controls.Add(this.lb_cn4);
			this.Controls.Add(this.lb_cn3);
			this.Controls.Add(this.lb_cn2);
			this.Controls.Add(this.lb_cn1);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textUp4);
			this.Controls.Add(this.textUp3);
			this.Controls.Add(this.textUp2);
			this.Controls.Add(this.textUp1);
			this.Controls.Add(this.textDown4);
			this.Controls.Add(this.textDown3);
			this.Controls.Add(this.textDown2);
			this.Controls.Add(this.textDown1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormConnections";
			this.ShowInTaskbar = false;
			this.Text = "FormConnections";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FormConnections_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void OnKeyPress_UpDown(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{			
			this.Close();
		}

		private void buttonAccept_Click(object sender, System.EventArgs e)
		{

			try 
			{
				ArrayList altConnectionsUpList = new ArrayList();
				ArrayList altConnectionsDownList = new ArrayList();

				if ((this.textDown1.Text!="") && (this.textUp1.Text!=""))
				{
					altConnectionsUpList.Add(this.textUp1.Text);
					altConnectionsDownList.Add(this.textDown1.Text);
				}

				if ((this.textDown2.Text!="") && (this.textUp2.Text!=""))
				{
					altConnectionsUpList.Add(this.textUp2.Text);
					altConnectionsDownList.Add(this.textDown2.Text);
				}
				
				if ((this.textDown3.Text!="") && (this.textUp3.Text!=""))
				{
					altConnectionsUpList.Add(this.textUp3.Text);
					altConnectionsDownList.Add(this.textDown3.Text);
				}
				
				if ((this.textDown4.Text!="") && (this.textUp4.Text!=""))
				{
					altConnectionsUpList.Add(this.textUp4.Text);
					altConnectionsDownList.Add(this.textDown4.Text);
				}

				string[] altConnectionsUp = new string[altConnectionsUpList.Count];
				string[] altConnectionsDown = new string[altConnectionsDownList.Count];

				for (byte n=0; n!=altConnectionsUpList.Count; n++)
				{
					altConnectionsUp[n]=(string)altConnectionsUpList[n];
					altConnectionsDown[n]=(string)altConnectionsDownList[n];
				};
				eLePhantForm.preferences.SetProperty("altConnectionsUp",altConnectionsUp);
				eLePhantForm.preferences.SetProperty("altConnectionsDown",altConnectionsDown);
			}
			catch
			{
				//data error
			}

			this.Close();		
		}

		private void FormConnections_Load(object sender, System.EventArgs e)
		{
			
			Point m_windowLocation=this.Location;
			m_windowLocation.Y	= Screen.PrimaryScreen.WorkingArea.Height-this.Height;
			m_windowLocation.X	= Screen.PrimaryScreen.WorkingArea.Width-this.Width;
			this.Location		= m_windowLocation;
		}
	}
}
