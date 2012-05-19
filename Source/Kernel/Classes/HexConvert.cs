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
using System.Text;

namespace eLePhant.Classes
{
	internal class HexConvert
	{
		public HexConvert() { }

		public static string ToBinHex(byte[] inArray)
		{
			return BinHexEncoder.EncodeToBinHex(inArray, 0, inArray.Length);
		}

		public static string ToBinHex(byte value)
		{
			return BinHexEncoder.EncodeToBinHex(value);
		}

		public static byte[] FromBinHex(string inArray)
		{
			BinHexDecoder binHexDecoder = new BinHexDecoder();
			return binHexDecoder.DecodeBinHex(inArray.ToCharArray(), 0, true);
		}

		private class ArrayManager
		{
			private class Buffer
			{
				public char[] _charBuffer;
				public int _offset;
				public int _size;

				public Buffer(char[] buffer, int offset, int size)
				{
					_charBuffer = buffer;
					_offset = offset;
					_size = size;
				}
			}

			private Queue _BufferQueue;
			private int _offset;
			private Buffer _CurrentBuffer;

			private Queue BufferQueue
			{
				get
				{
					if (_BufferQueue == null)
						_BufferQueue = new Queue();
					return _BufferQueue;
				}
				set
				{
					_BufferQueue = value;
				}
			}

			private int Offset
			{
				get
				{
					return _offset;
				}
				set
				{
					_offset = value;
				}
			}

			internal char[] CurrentBuffer
			{
				get
				{
					if (_CurrentBuffer != null)
						return _CurrentBuffer._charBuffer;
					return null;
				}
			}

			internal int CurrentBufferOffset
			{
				get
				{
					if (_CurrentBuffer != null)
						return _CurrentBuffer._offset;
					return 0;
				}
			}

			internal int CurrentBufferLength
			{
				get
				{
					if (_CurrentBuffer != null)
						return _CurrentBuffer._size;
					return 0;
				}
			}

			internal int Length
			{
				get
				{
					int len = 0;
					if (_CurrentBuffer != null)
						len += (_CurrentBuffer._size - _CurrentBuffer._offset);
					IEnumerator enumerator = BufferQueue.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Buffer element = (Buffer)enumerator.Current;
						len += (element._size - element._offset);
					}
					return len;
				}
			}

			internal char this[int index]
			{
				get
				{
					char ch = '\0';
					if (_CurrentBuffer == null)
					{
						if (BufferQueue.Count > 0)
							_CurrentBuffer = (Buffer)BufferQueue.Dequeue();
						else
							return ch;
					}

					if (!((_CurrentBuffer._offset + index - Offset) < _CurrentBuffer._size))
					{
						Offset = index;
						_CurrentBuffer = (BufferQueue.Count > 0) ? (Buffer)BufferQueue.Dequeue() : null;
					}

					if (_CurrentBuffer != null)
						ch = _CurrentBuffer._charBuffer[_CurrentBuffer._offset + (index - Offset)];
					return ch;
				}
			}

			internal void Append(char[] buffer, int offset, int size)
			{
				BufferQueue.Enqueue(new Buffer(buffer, offset, size));
			}

			internal void CleanUp(int internalBufferOffset)
			{
				if (_CurrentBuffer != null)
				{
					_CurrentBuffer._offset += internalBufferOffset - Offset;
					Offset = 0;
				}
			}

			internal void Refresh()
			{
				BufferQueue = new Queue();
				_CurrentBuffer = null;
				_offset = 0;
			}

			internal ArrayManager()
			{
				BufferQueue = null;
				_offset = 0;
				_CurrentBuffer = null;
			}
		}

		private class BinHexEncoder
		{
			private const string s_hexDigits = "0123456789ABCDEF";

			internal static string EncodeToBinHex(byte value)
			{
				char[] outArray = new char[2];

				outArray[0] = s_hexDigits[value >> 4];
				outArray[1] = s_hexDigits[value & 0xF];

				return new String(outArray, 0, 2);
			}

			internal static string EncodeToBinHex(byte[] inArray, int offsetIn, int count)
			{
				if (null == inArray)
				{
					throw new ArgumentNullException("inArray");
				}

				if (0 > offsetIn)
				{
					throw new ArgumentOutOfRangeException("offsetIn");
				}

				if (0 > count)
				{
					throw new ArgumentOutOfRangeException("count");
				}

				if (count > inArray.Length - offsetIn)
				{
					throw new ArgumentException("count > inArray.Length - offsetIn");
				}

				char[] outArray = new char[2 * count];
				int lenOut = EncodeToBinHex(inArray, offsetIn, count, outArray);
				return new String(outArray, 0, lenOut);
			}

			private static int EncodeToBinHex(byte[] inArray, int offsetIn, int count, char[] outArray)
			{
				int curOffsetOut = 0, offsetOut = 0;
				byte b;
				int lengthOut = outArray.Length;

				for (int j = 0; j < count; j++)
				{
					b = inArray[offsetIn++];
					outArray[curOffsetOut++] = s_hexDigits[b >> 4];
					if (curOffsetOut == lengthOut)
					{
						break;
					}
					outArray[curOffsetOut++] = s_hexDigits[b & 0xF];
					if (curOffsetOut == lengthOut)
					{
						break;
					}
				}
				return curOffsetOut - offsetOut;
			} // function
		}

		private class BinHexDecoder
		{
			private ArrayManager _charBuffer = new ArrayManager();
			bool _HighNibblePresent = false;
			byte _highHalfByte = 0;

			internal byte[] DecodeBinHex(char[] inArray, int offset, bool flush)
			{
				int len = inArray.Length;

				// divide in 1/2 with round up since two chars will be encoded into one byte
				byte[] outArray = new byte[(len - offset + 1) / 2];
				int retLength = DecodeBinHex(inArray, offset, inArray.Length, outArray, 0, outArray.Length, flush);

				if (retLength != outArray.Length)
				{
					byte[] tmpResult = new byte[retLength];
					Array.Copy(outArray, tmpResult, retLength);
					outArray = tmpResult;
				}
				return outArray;
			}

			internal int DecodeBinHex(char[] inArray, int offset, int inLength, byte[] outArray, int offsetOut, int countOut, bool flush)
			{
				//String msg;

				if (0 > offset)
				{
					throw new ArgumentOutOfRangeException("offset");
				}

				if (0 > offsetOut)
				{
					throw new ArgumentOutOfRangeException("offsetOut");
				}

				int len = (null == inArray) ? 0 : inArray.Length;
				if (len < inLength)
				{
					throw new ArgumentOutOfRangeException("inLength");
				}

				// make sure that countOut + offsetOut are okay
				int outArrayLen = outArray.Length;
				if (outArrayLen < (countOut + offsetOut))
				{
					throw new ArgumentOutOfRangeException("offsetOut");
				}

				int inBufferCount = inLength - offset;

				if (flush)
					_charBuffer.Refresh();

				if (inBufferCount > 0)
					_charBuffer.Append(inArray, offset, inLength);

				if ((_charBuffer.Length == 0) || (countOut == 0))
					return 0;

				// let's just make sure countOut > 0 and countOut < outArray.Length
				countOut += offsetOut;
				byte lowHalfByte = 0; //TODO: Changed

				// walk hex digits pairing them up and shoving the value of each pair into a byte
				int internalBufferLength = _charBuffer.Length;
				int offsetOutCur = offsetOut;
				char ch;
				int internalBufferOffset = 0;
				do
				{
					ch = _charBuffer[internalBufferOffset++];
					if (ch >= 'a' && ch <= 'f')
					{
						lowHalfByte = (byte)(10 + ch - 'a');
					}
					else if (ch >= 'A' && ch <= 'F')
					{
						lowHalfByte = (byte)(10 + ch - 'A');
					}
					else if (ch >= '0' && ch <= '9')
					{
						lowHalfByte = (byte)(ch - '0');
					}
					else if (Char.IsWhiteSpace(ch))
					{
						continue; // skip whitespace
					}/*
				else {
					msg = new String(_charBuffer.CurrentBuffer, _charBuffer.CurrentBufferOffset, (_charBuffer.CurrentBuffer == null) ? 0:(_charBuffer.CurrentBufferLength - _charBuffer.CurrentBufferOffset));
					throw new XmlException(Res.Xml_InvalidBinHexValue, msg);
				}
*/
					if (_HighNibblePresent)
					{
						outArray[offsetOutCur++] = (byte)((_highHalfByte << 4) + lowHalfByte);
						_HighNibblePresent = false;
						if (offsetOutCur == countOut)
						{
							break;
						}
					}
					else
					{
						// shift nibble into top half of byte
						_highHalfByte = lowHalfByte;
						_HighNibblePresent = true;
					}
				}
				while (internalBufferOffset < internalBufferLength);

				_charBuffer.CleanUp(internalBufferOffset);

				return offsetOutCur - offsetOut;
			}

			internal int BitsFilled
			{
				get { return (_HighNibblePresent ? 4 : 0); }
			}

			internal void Flush()
			{
				if (null != _charBuffer)
				{
					_charBuffer.Refresh();
				}
				_HighNibblePresent = false;
			}
		}
	}
}
