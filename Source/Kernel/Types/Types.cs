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

namespace eLePhant.Types
{
	public class Constants
	{
		//logs
		public enum Log : byte
		{
			Verbose			= 2,
			Info			= 1,
			Notify			= 0,
		}
		
		//view shared files list permisions
		public enum AllowViewShared:byte
		{
			All				= 2,
			Friends			= 1,
			Nobody			= 0,
		}
		
		//file type
		public enum FileType:byte 
		{
			Unknow			= 0,
			Avi				= 1,
			Ogm				= 2,
			Mp3				= 3,
			Zip				= 4,
		}

		/// <summary>
		/// Priority includes all values for upload and download pritority
		/// </summary>
		public enum Priority : byte
		{
			VeryLow						= 4,
			Low							= 0,
			Normal						= 1,
			High						= 2,
			VeryHigh					= 3,
			Auto						= 5,
		}

		/// <summary>
		/// Priority includes all values for upload and download pritority
		/// </summary>
		public enum ServerPriority : byte
		{
			Low							= 2,
			High						= 1,
			Normal						= 0,
		}

		//AVI files propertys
		public enum Avi:byte 
		{
			Size			= 0,
			VFps			= 1,
			Length			= 2,
			Width			= 3,
			Height			= 4,
			VCodec			= 5,
			VBitrate		= 6,
			ACodec			= 7,
			AChannel		= 8,
			ABitrate		= 9,
			ASampleRate		= 10,
		}

		public enum SearchResultState:byte
		{
			New					= 0,
			AlreadyDownloading	= 1,
			AlreadyDownloaded	= 2,
		}

		public enum FriendLevel:byte
		{
			NoFriend	= 0,
			Friend		= 1,
			FriendSlot	= 2,
		}
	}
}
