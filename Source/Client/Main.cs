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
using System.Threading;
using eLePhant.Classes;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace eLePhant.Client
{
	/// <summary>
	/// Provides the program entry point
	/// </summary>
	public class CMain
	{
		private static Mutex m_Mutex;
		private static string m_elink;
		private static Config m_Preferences;
		public CMain(string[] args)
		{
			m_Preferences = new Config(Application.StartupPath, "configInterface.xml", "0.01", "lphantInterface");
			m_Preferences.LoadProperties();
			
			if (args.Length>0)
				m_elink=args[0];
			
			//bool noInstanceRuning;
			//m_Mutex=new Mutex(true,InterfaceConstants.GUID,out noInstanceRuning);
			m_Mutex=new Mutex(true,InterfaceConstants.GUID);
			//if (noInstanceRuning)			
			if (m_Mutex.WaitOne(0,true))
			{
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnAppDomainException);

				if (m_Preferences.GetBool("StartupLocal",true))
				{
					Application.Run(new eLePhantForm(m_elink,m_Mutex));
				}
				else if (m_Preferences.GetBool("StartupRemote",false))
				{
					string remoteIP=m_Preferences.GetString("RemoteIP","");
					int remotePort=m_Preferences.GetInt("RemotePort",0);
					string remotePass=m_Preferences.GetString("RemotePassword","");
					if ((remotePort>0)&&(remoteIP.Length>0)&&(remotePass.Length>0))
					{
						CedonkeyCRemoto CRemoto=new CedonkeyCRemoto();
						if (CRemoto.Connect(remoteIP,remotePass,remotePort))
						{
							Application.Run(new eLePhantForm(m_elink,m_Mutex,CRemoto.interfazremota));
						}
						else
						{
							MessageBox.Show("Can not connect with the host specified in preferences","lphant",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
							Application.Run(new StartForm(m_elink,m_Mutex));
						}
					}
					else
						Application.Run(new StartForm(m_elink,m_Mutex));
				}
				else
					Application.Run(new StartForm(m_elink,m_Mutex));
			}
			else
			{
				NotifyToMainInstance();
			}
		}
		private static void NotifyToMainInstance()
		{
			Win32.EnumWindows(m_EnumProc,0);
		}
		private static Win32.EnumWindowsProc m_EnumProc=new Win32.EnumWindowsProc(m_EnumWindows);
		public static int m_EnumWindows(IntPtr hwnd, int lParam)
		{
			if (Win32.GetProp(hwnd,InterfaceConstants.GUID)==1)
			{
				if (m_elink==null) m_elink="";
				byte[] lpStr=Encoding.Default.GetBytes(m_elink);
				IntPtr lpB=Marshal.AllocHGlobal(lpStr.Length);
				Marshal.Copy(lpStr, 0, lpB, lpStr.Length);
				Win32.COPYDATASTRUCT stMsg;
				stMsg.dwData=0;
				stMsg.cbData=lpStr.Length;
				stMsg.lpData=lpB.ToInt32();
                IntPtr lpCD= Marshal.AllocHGlobal(Marshal.SizeOf(stMsg));
                Marshal.StructureToPtr(stMsg, lpCD, false);
                Win32.SendMessage(hwnd,Win32.WM_COPYDATA,hwnd,lpCD);
                Marshal.FreeHGlobal(lpCD);
				Marshal.FreeHGlobal(lpB);
				return 0;
			}
			else
			{
				return 1;
			}
		}
		
		// thanks to rss bandit developers
		private void OnAppDomainException(object sender,UnhandledExceptionEventArgs e)
		{

			// this seems to be the only place to "handle" the 
			// System.NullReferenceException: Object reference not set to aninstance of an object.
			// at System.Net.OSSOCK.WSAGetOverlappedResult(IntPtrsocketHandle, IntPtr overlapped, UInt32& bytesTransferred, Boolean wait,IntPtr ignored)
			// at System.Net.Sockets.OverlappedAsyncResult.CompletionPortCallback(UInt32 errorCode, UInt32 numBytes,NativeOverlapped* nativeOverlapped)
			// that occurs on some systems running behind a NAT/Router/Dialer network connection.
			// See also the discussions here: 
			//http://groups.google.com/groups?hl=de&ie=UTF-8&oe=UTF-8&q=WSAGetOverlappedResult+%22Object+reference+not+set%22&sa=N&tab=wg&lr=
			//http://groups.google.com/groups?hl=de&lr=&ie=UTF-8&oe=UTF-8&threadm=7P-cnbOVWf_pEtKiXTWc-g%40speakeasy.net&rnum=4&prev=/groups%3Fhl%3Dde%26ie%3DUTF-8%26oe%3DUTF-8%26q%3DWSAGetOverlappedResult%2B%2522Object%2Breference%2Bnot%2Bset%2522%26sa%3DN%26tab%3Dwg%26lr%3D
			//http://groups.google.com/groups?hl=de&lr=&ie=UTF-8&oe=UTF-8&threadm=3fd6eba3.432257543%40news.microsoft.com&rnum=3&prev=/groups%3Fhl%3Dde%26ie%3DUTF-8%26oe%3DUTF-8%26q%3DWSAGetOverlappedResult%2B%2522Object%2Breference%2Bnot%2Bset%2522%26sa%3DN%26tab%3Dwg%26lr%3D

			if (e.ExceptionObject is NullReferenceException) 
			{
				string message =((Exception)e.ExceptionObject).ToString();
				if (message.IndexOf("WSAGetOverlappedResult") >= 0 && message.IndexOf("CompletionPortCallback") >= 0 )
				Debug.WriteLine("Unhandled exception ignored: " + message);
				return; // ignore. See comment above :-(
			}
		}
	}
}
