using System;
using System.Collections;

namespace eLePhant.eDonkey
{
	internal class CGapList : SortedList
	{
		private uint filesize;

		public CGapList(uint filesize)
		{
			this.filesize = filesize - 1;
			base.Add(0, this.filesize);
		}

		// add an empty gap
		public void Add(uint start, uint end)
		{
			if (end > filesize) end = filesize;

			for (int n = Count - 1; n >= 0; n--)
			{
				uint gapStart = (uint)GetKey(n); // Keys[n];
				uint gapEnd = (uint)GetByIndex(n); // Values[n];

				if ((gapStart >= start) && (gapEnd <= end))
				{
					// this gap is inside the new gap - delete
					RemoveAt(n);
					continue;
				}
				
				if ((gapStart >= start) && (gapStart <= end))
				{
					// a part of this gap is in the new gap - extend limit and delete
					end = gapEnd;
					RemoveAt(n);
					continue;
				}
				
				if ((gapEnd <= end) && (gapEnd >= start))
				{
					// a part of this gap is in the new gap - extend limit and delete
					start = gapStart;
					RemoveAt(n);
					continue;
				}
				
				if ((start >= gapStart) && (end <= gapEnd))
				{
					// new gap is already inside this gap - return
					return;
				}
			}

			int frontGap = IndexOfValue(start - 1);
			if (frontGap != -1)
			{
				// we had an neighbor in front - merge and delete
				start = (uint)GetKey(frontGap); // Keys[frontGap];
				RemoveAt(frontGap);
			}

			int nextGap = IndexOfKey(end + 1);
			if (nextGap != -1)
			{
				// we had an neighbor at the end - merge and delete
				end = (uint)GetByIndex(nextGap); // Values[nextGap];
				RemoveAt(nextGap);
			}

			// finally add the gap
			base.Add(start, end);
		}

		// substract an gap from list
		public void Fill(uint start, uint end)
		{
			if (end > filesize) end = filesize;

			for (int n = Count - 1; n >= 0; n--)
			{
				uint gapStart = (uint)GetKey(n); // Keys[n];
				uint gapEnd = (uint)GetByIndex(n); // Values[n];

				if ((gapStart >= start) && (gapEnd <= end))
				{
					// our part fills this gap completly
					RemoveAt(n);
				}
				else if ((gapStart >= start) && (gapStart <= end))
				{
					// a part of this gap is in the part - set limit
					RemoveAt(n);
					base.Add(end + 1, gapEnd);
				}
				else if ((gapEnd <= end) && (gapEnd >= start))
				{
					// a part of this gap is in the part - set limit
					RemoveAt(n);
					base.Add(gapStart, start - 1);
				}
				else if ((start >= gapStart) && (end <= gapEnd))
				{
					RemoveAt(n);
					base.Add(gapStart, start - 1);
					base.Add(end + 1, gapEnd);
					break;
				}
			}
		}

		public uint RemainingBytes()
		{
			uint bytes = 0;

			for (int i = 0; i != Count; i++)
			{
				bytes += ((uint)GetByIndex(i) - (uint)GetKey(i) + 1);
			}
			return bytes;
		}
	}
}
