using System;
using System.Windows.Forms;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for ListColumn.
	/// </summary>
	public class ListColumn
	{
		private System.ComponentModel.Container components = null;
		int imageIndex;
		bool ownerDraw;
		bool imageOnRight;
		bool m_subitemOwnerDraw;
		HorizontalAlignment textAlign;
		int width;
		string text;
		
		public ListColumn()
		{
			InitializeComponent();

			//Default values
			imageIndex = -1;
			subItemOwnerDraw=false;
			ownerDraw = false;
			imageOnRight = true;
			textAlign = HorizontalAlignment.Left;
			width = 60;
			text = "ColumnHeader";
		}		

		#region Public Properties

		public bool subItemOwnerDraw
		{
			get{return m_subitemOwnerDraw;}
			set{m_subitemOwnerDraw=value;}
		}
		public int ImageIndex
		{
			get{return imageIndex;}
			set{imageIndex = value;}
		}

		public bool OwnerDraw
		{
			get{return ownerDraw;}
			set{ownerDraw = value;}
		}

		public bool ImageOnRight
		{
			get{return imageOnRight;}
			set{imageOnRight = value;}
		}

		public string Text
		{
			get{return text;}
			set{text = value;}
		}

		public HorizontalAlignment TextAlign
		{
			get{return textAlign;}
			set{textAlign = value;}
		}

		public int Width
		{
			get{return width;}
			set{width = value;}
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

	}
}
