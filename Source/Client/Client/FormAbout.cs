using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for FormAbout.
	/// </summary>
	public class FormAbout : System.Windows.Forms.Form
	{
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Timer timer1;
		private eLePhant.Interface.ScrollingCredits scrollingCredits;
		private System.ComponentModel.IContainer components;
		private double m_dblOpacityIncrement = .1;
		private double m_dblOpacityDecrement = .1;
		private const int TIMER_INTERVAL = 50;

		public FormAbout()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Opacity = .0;
			timer1.Interval = TIMER_INTERVAL;
			timer1.Start();

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormAbout));
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.scrollingCredits = new eLePhant.Interface.ScrollingCredits();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.ActiveLinkColor = System.Drawing.Color.Transparent;
			this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
			this.linkLabel1.DisabledLinkColor = System.Drawing.Color.Transparent;
			this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.linkLabel1.ForeColor = System.Drawing.Color.Red;
			this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
			this.linkLabel1.LinkColor = System.Drawing.Color.Red;
			this.linkLabel1.Location = new System.Drawing.Point(32, 236);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(141, 26);
			this.linkLabel1.TabIndex = 5;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "www.lphant.com";
			this.linkLabel1.VisitedLinkColor = System.Drawing.Color.Transparent;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// scrollingCredits
			// 
			this.scrollingCredits.BackColor = System.Drawing.Color.Transparent;
			this.scrollingCredits.Location = new System.Drawing.Point(195, 17);
			this.scrollingCredits.Name = "scrollingCredits";
			this.scrollingCredits.Size = new System.Drawing.Size(187, 207);
			this.scrollingCredits.TabIndex = 0;
			this.scrollingCredits.Text = "scrollingCredits";
			this.scrollingCredits.Click += new System.EventHandler(this.scrollingCredits_Click);
			// 
			// FormAbout
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(398, 263);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.scrollingCredits);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAbout";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About lphant";
			this.Click += new System.EventHandler(this.FormAbout_Click);
			this.Load += new System.EventHandler(this.FormAbout_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormAbout_Click(object sender, System.EventArgs e)
		{
			m_dblOpacityIncrement = -m_dblOpacityDecrement;
		}

		private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.lphant.com");
		}

		private void FormAbout_Load(object sender, System.EventArgs e)
		{
			
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if( m_dblOpacityIncrement > 0 )
			{
				if( this.Opacity < 1 )
				this.Opacity += m_dblOpacityIncrement;
			}
			else
			{
				if( this.Opacity > 0 )
				this.Opacity += m_dblOpacityIncrement;
				else
				this.Close();
			}		
		}

		private void scrollingCredits_Click(object sender, System.EventArgs e)
		{
			m_dblOpacityIncrement = -m_dblOpacityDecrement;
		}
	}
}
