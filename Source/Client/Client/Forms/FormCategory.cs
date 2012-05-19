using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using eLePhant.eDonkey;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for FormCategory.
	/// </summary>
	public class FormCategory : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonAccept;
		private System.Windows.Forms.Button buttonCancel;
		private CkernelGateway krnGateway;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxAutoAssign;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label labelAutoSelect;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.ListBox listBoxCategories;
		private System.Windows.Forms.Label labelID;
		private System.Windows.Forms.Button buttonRemove;

		private string m_AntName;
		private System.Windows.Forms.ColorDialog colorDialog1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormCategory()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public void Initilize(CkernelGateway in_krnGateway)
		{
			krnGateway=in_krnGateway;
			RefreshList();
			labelID.Visible=false;
			labelID.Text="0";
			buttonRemove.Text=eLePhantForm.Globalization["LBL_REMOVECATEGORY"];
			buttonAccept.Text=eLePhantForm.Globalization["LBL_ACCEPT"];
			buttonCancel.Text=eLePhantForm.Globalization["LBL_CANCEL"];
			labelName.Text=eLePhantForm.Globalization["LBL_NAME"]+":";
			labelAutoSelect.Text=eLePhantForm.Globalization["LBL_AUTOSELECTCAT"]+":";
			label1.Text=eLePhantForm.Globalization["LBL_COLOR"]+":";
			m_ApplySkin();
		}

		private void RefreshList()
		{
			listBoxCategories.Items.Clear();
			ArrayList categories=krnGateway.GetCategories();
	
			listBoxCategories.Items.Add("*"+eLePhantForm.Globalization["LBL_NEWCATEGORY"]);

			foreach (CCategory category in categories)
			{
				this.listBoxCategories.Items.Add(category.Name);
			}

			listBoxCategories.SelectedIndex=0;

		}

		private void m_ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			m_DoSkin(this);
		}

		private void m_DoSkin(Control parentControl)
		{
//			tabControl1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
//			tabControl1.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
//			tabControl1.TextInactiveColor=eLePhantForm.Skin.GetColor("tabsInactiveForeColor");
			groupBox1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
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
				else if	(c.GetType()==typeof(GroupBox))
				{
					((GroupBox)c).ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
					if ((c.Controls!=null)&&(c.Controls.Count>0)) m_DoSkin(c);
				}
				else if ((c.Controls!=null)&&(c.Controls.Count>0)) m_DoSkin(c);
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonAccept = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.labelID = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxAutoAssign = new System.Windows.Forms.TextBox();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.labelAutoSelect = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.listBoxCategories = new System.Windows.Forms.ListBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonAccept
			// 
			this.buttonAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAccept.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAccept.Location = new System.Drawing.Point(375, 159);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.Size = new System.Drawing.Size(98, 24);
			this.buttonAccept.TabIndex = 7;
			this.buttonAccept.Text = "Accept";
			this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.Location = new System.Drawing.Point(477, 159);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(98, 24);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.buttonRemove);
			this.groupBox1.Controls.Add(this.labelID);
			this.groupBox1.Controls.Add(this.panel1);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textBoxAutoAssign);
			this.groupBox1.Controls.Add(this.textBoxName);
			this.groupBox1.Controls.Add(this.labelAutoSelect);
			this.groupBox1.Controls.Add(this.labelName);
			this.groupBox1.Location = new System.Drawing.Point(140, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(436, 149);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonRemove.Location = new System.Drawing.Point(30, 118);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(188, 24);
			this.buttonRemove.TabIndex = 13;
			this.buttonRemove.Text = "Remove this category";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// labelID
			// 
			this.labelID.Location = new System.Drawing.Point(261, 28);
			this.labelID.Name = "labelID";
			this.labelID.Size = new System.Drawing.Size(18, 15);
			this.labelID.TabIndex = 12;
			this.labelID.Text = "ID";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Desktop;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.panel1.Location = new System.Drawing.Point(321, 110);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(75, 18);
			this.panel1.TabIndex = 11;
			this.panel1.Visible = false;
			this.panel1.Click += new System.EventHandler(this.panel1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(307, 92);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 16);
			this.label1.TabIndex = 10;
			this.label1.Text = "Color:";
			this.label1.Visible = false;
			// 
			// textBoxAutoAssign
			// 
			this.textBoxAutoAssign.Location = new System.Drawing.Point(31, 86);
			this.textBoxAutoAssign.Name = "textBoxAutoAssign";
			this.textBoxAutoAssign.Size = new System.Drawing.Size(187, 20);
			this.textBoxAutoAssign.TabIndex = 9;
			this.textBoxAutoAssign.Text = "";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(29, 30);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(190, 20);
			this.textBoxName.TabIndex = 8;
			this.textBoxName.Text = "";
			// 
			// labelAutoSelect
			// 
			this.labelAutoSelect.Location = new System.Drawing.Point(15, 52);
			this.labelAutoSelect.Name = "labelAutoSelect";
			this.labelAutoSelect.Size = new System.Drawing.Size(417, 32);
			this.labelAutoSelect.TabIndex = 7;
			this.labelAutoSelect.Text = "Files with any of these words in the name will be auto-assigned to this category:" +
				"";
			this.labelAutoSelect.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Location = new System.Drawing.Point(15, 14);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(38, 16);
			this.labelName.TabIndex = 6;
			this.labelName.Text = "Name:";
			// 
			// listBoxCategories
			// 
			this.listBoxCategories.Location = new System.Drawing.Point(6, 7);
			this.listBoxCategories.Name = "listBoxCategories";
			this.listBoxCategories.Size = new System.Drawing.Size(127, 147);
			this.listBoxCategories.TabIndex = 9;
			this.listBoxCategories.SelectedIndexChanged += new System.EventHandler(this.listBoxCategories_SelectedIndexChanged);
			// 
			// FormCategory
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(578, 184);
			this.Controls.Add(this.listBoxCategories);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonAccept);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FormCategory";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Category";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAccept_Click(object sender, System.EventArgs e)
		{
			if (textBoxName.Text.Length>0)
				krnGateway.AddorUpdateCategory(uint.Parse(labelID.Text),this.textBoxName.Text,this.textBoxAutoAssign.Text,(uint)panel1.BackColor.ToArgb());
			this.Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void buttonRemove_Click(object sender, System.EventArgs e)
		{
			krnGateway.RemoveCategory(uint.Parse(labelID.Text));
			labelID.Text="0";
			textBoxName.Text="";
			textBoxAutoAssign.Text="";
			m_AntName="";
			RefreshList();
			buttonRemove.Enabled=false;
		}

		private void listBoxCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listBoxCategories.SelectedIndex<0) return;
			int selected=listBoxCategories.SelectedIndex;
			if (textBoxName.Text.Length>0)
			{
				krnGateway.AddorUpdateCategory(uint.Parse(labelID.Text),this.textBoxName.Text,this.textBoxAutoAssign.Text,(uint)panel1.BackColor.ToArgb());
				//RefreshList();
				if (m_AntName.Length>0)
				{
					if (m_AntName!=textBoxName.Text) listBoxCategories.Items[listBoxCategories.Items.IndexOf(m_AntName)]=textBoxName.Text;
				}
				else
					listBoxCategories.Items.Add(textBoxName.Text);
			}

			if (listBoxCategories.SelectedIndex==0)
			{
				labelID.Text="0";
				textBoxName.Text="";
				textBoxAutoAssign.Text="";
				m_AntName="";
				buttonRemove.Enabled=false;
				panel1.BackColor=Color.Black;
			}
			else
			{
				ArrayList categories=krnGateway.GetCategories();
				foreach (CCategory category in categories)
				{
					if (category.Name==listBoxCategories.Items[listBoxCategories.SelectedIndex].ToString())
					{
						labelID.Text=category.ID.ToString();;
						textBoxName.Text=category.Name;
						m_AntName=category.Name;
						textBoxAutoAssign.Text=category.AutoString;	
						panel1.BackColor=Color.FromArgb((int)category.Color);
						buttonRemove.Enabled=true;
						break;
					}
				}
			}
		}

		private void panel1_Click(object sender, System.EventArgs e)
		{
			colorDialog1.Color=panel1.BackColor;
			if (colorDialog1.ShowDialog()==DialogResult.OK)
				panel1.BackColor=colorDialog1.Color;
		}
	}
}
