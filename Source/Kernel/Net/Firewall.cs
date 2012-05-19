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
using NetFwTypeLib;

namespace eLePhant.Net
{
	/// <summary>
	/// Firewall handles Windows XP Firewall (Service Pack 2 Release Candidate 2)
	/// Released: June 15, 2004
	/// </summary>
	public class Firewall
	{
		// example: AddToPermissionsList("MyTest", Application.ExecutablePath, true);
		public static void AddToPermissionsList(string name, string imageName, bool enabled)
		{
			// Add the application to the ICF Permissions List
			NetFwTypeLib.INetFwMgr mgr = (NetFwTypeLib.INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));

			NetFwTypeLib.INetFwAuthorizedApplication app = (NetFwTypeLib.INetFwAuthorizedApplication)
				Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication"));

			app.Name = name;
			app.ProcessImageFileName = imageName;
			app.Enabled = enabled;

			mgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(app);
		}

		// example: RemoveFromPermissionsList(Application.ExecutablePath);
		public static void RemoveFromPermissionsList(string imageName)
		{
			// Remove the application from the ICF Permissions List
			NetFwTypeLib.INetFwMgr mgr = (NetFwTypeLib.INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));

			mgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Remove(imageName);
		}

		// example: AddPort(true, "TCP Port", Int32.Parse(port.Text), NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
		public static bool AddPort(bool enabled, string name, int port, NET_FW_IP_PROTOCOL_ protocol)
		{
			// Add the port to the ICF Permissions List
			INetFwMgr mgr = null;

			try
			{
				mgr = (NetFwTypeLib.INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
			}
			catch
			{
				// XPSP2 not installed
				return false;
			}

			try
			{
				INetFwProfile profile;
				INetFwOpenPort portClass = (NetFwTypeLib.INetFwOpenPort)Activator.CreateInstance(
					Type.GetTypeFromProgID("HNetCfg.FWOpenPort"));

				// Get the current profile
				profile = mgr.LocalPolicy.CurrentProfile;

				// Set the port properties
				portClass.Enabled = enabled;
				portClass.Name = name;
				portClass.Port = port;
				portClass.Protocol = protocol;

				// Add the port to the ICF Permissions List
				profile.GloballyOpenPorts.Add(portClass);
				return true;
			}
			catch
			{
				// Log the error or prompt the user
				return false;
			}
		}

		// example: RemovePort(Int32.Parse(port.Text), NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
		public static bool RemovePort(int port, NET_FW_IP_PROTOCOL_ protocol)
		{
			// Remove the port to the ICF Permissions List
			INetFwMgr mgr = null;

			try
			{
				mgr = (NetFwTypeLib.INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
			}
			catch
			{
				// XPSP2 not installed
				return false;
			}

			try
			{
				INetFwProfile profile;

				// Get the current profile
				profile = mgr.LocalPolicy.CurrentProfile;

				// Remove the port to the ICF Permissions List
				profile.GloballyOpenPorts.Remove(port, protocol);
				return true;
			}
			catch
			{
				// Log the error or prompt the user
				return false;
			}
		}
	}
}
