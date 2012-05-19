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
using System.Collections;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Summary description for CedonkeyComment.
	/// </summary>
	internal class CedonkeyComment
	{
		public string userName;
		public byte rating;
		public string strComment;

		public CedonkeyComment()
		{
		}
	}

	internal class CedonkeyComments:ArrayList
	{
		public CElement Element;

		public CedonkeyComments(CElement in_Element)
		{
			Element=in_Element;
		}

		public void NewComment(string userName,byte rating,string strComment)
		{
			if (!m_Contains(userName,rating,strComment))
			{
				CedonkeyComment Comment=new CedonkeyComment();
				Comment.userName=userName;
				Comment.rating=rating;
				Comment.strComment=strComment;
				Add(Comment);
			}
		}

		public void RefreshList()
		{
			int i=0;
			this.Clear();
			if (Element.SourcesList==null) return;

			while (i<Element.SourcesList.Count())
			{
				CClient client=(CClient)Element.SourcesList.Item(i);
				if ((client.Comment != null)||(client.Rating!=0))
				{
					NewComment(client.UserName,client.Rating,client.Comment);
				}
				i++;
			}
		}

		private bool m_Contains(string userName, byte rating, string strComment)
		{
			foreach (CedonkeyComment Comment in this)
			{
				if ((Comment.userName==userName)&&(Comment.rating==rating)&&(Comment.strComment==strComment))
					return true;
			}
			return false;
		}
	}
}