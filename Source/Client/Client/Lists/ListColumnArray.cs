using System;
using System.Collections;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for ListColumnArray.
	/// </summary>
	public class ListColumnArray: CollectionBase
	{
		public event EventHandler ColumnAdded;
		public event EventHandler ColumnRemoved;
		public ListColumn Add(ListColumn value)
		{
			base.List.Add(value as object);
			if(ColumnAdded != null)
				ColumnAdded(value, new EventArgs());
			return value;
		}
		public void Add(string title, int width,bool subItemOwnerDraw)
		{
			ListColumn column=new ListColumn();
			column.Width=width;
			column.Text=title;
			column.OwnerDraw=true;
			column.subItemOwnerDraw=subItemOwnerDraw;
			Add(column);
		}
		public void AddRange(ListColumn[] values)
		{
			foreach(ListColumn ip in values)
				Add(ip);
		}

		public void Remove(ListColumn value)
		{
			base.List.Remove(value as object);
			if(ColumnRemoved != null)
				ColumnRemoved(value, new EventArgs());
		}

		public void Insert(int index, ListColumn value)
		{
			base.List.Insert(index, value as object);
			if(ColumnAdded != null)
				ColumnAdded(this, new EventArgs());
		}

		public bool Contains(ListColumn value)
		{
			return base.List.Contains(value as object);
		}

		public ListColumn this[int index]
		{
			get { return (base.List[index] as ListColumn); }
		}

		public int IndexOf(ListColumn value)
		{
			return base.List.IndexOf(value);
		}
	}
}
