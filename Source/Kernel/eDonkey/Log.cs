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
using System.Diagnostics;
using eLePhant.Types;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Send log messages to the interface
	/// </summary>
	internal class CLog
	{
		public CLog()
		{
		}

		public static void Log(Constants.Log importance, string message)
		{
#if !DEBUG
			if (importance==Constants.Log.Verbose) return;
#endif
			CKernel.NewLogMessage(importance,CKernel.Globalization[message]);
		}

		public static void Log(Constants.Log importance, string message, Object[] args)
		{
#if !DEBUG
			if (importance==Constants.Log.Verbose) return;
#endif			
			string translatedMsg=CKernel.Globalization[message];
			try
			{
				CKernel.NewLogMessage(importance,String.Format(translatedMsg,args));
			}
			catch
			{
				Debug.Write("Invalid  message format:"+message+"\n");
			}
		}

		public static void Log(Constants.Log importance, string message, Object arg1)
		{
#if !DEBUG
			if (importance==Constants.Log.Verbose) return;
#endif			
			string translatedMsg=CKernel.Globalization[message];
			try
			{
				CKernel.NewLogMessage(importance,String.Format(translatedMsg,arg1));
			}
			catch
			{
				Debug.Write("Invalid  message format:"+message+"\n");
			}
		}

		public static void Log(Constants.Log importance, string message, Object arg1, Object arg2)
		{
#if !DEBUG
			if (importance==Constants.Log.Verbose) return;
#endif
			string translatedMsg=CKernel.Globalization[message];
			try
			{
				CKernel.NewLogMessage(importance,String.Format(translatedMsg,arg1,arg2));
			}
			catch
			{
				Debug.Write("Invalid  message format:"+message+"\n");
			}
		}

		public static void Log(Constants.Log importance, string message, Object arg1, Object arg2, Object arg3)
		{
#if !DEBUG
			if (importance==Constants.Log.Verbose) return;
#endif			
			string translatedMsg=CKernel.Globalization[message];
			try
			{
				CKernel.NewLogMessage(importance,String.Format(translatedMsg,arg1,arg2,arg3));
			}
			catch
			{
				Debug.Write("Invalid  message format:"+message+"\n");
			}
		}
	}
}