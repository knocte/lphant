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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using eLePhant.eDonkey;
using eLePhant.Types;
using Microsoft.Win32;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for FormOptions.
	/// </summary>
	public class FormOptions : System.Windows.Forms.Form
	{
		private string m_SelectedLanguage;
		private string m_CurrentLanguage;
		private ArrayList m_languageFiles = new ArrayList();
		private Crownwood.Magic.Controls.TabControl tabControl1;
		private CkernelGateway krnGateway;
		private System.Windows.Forms.CheckBox checkBoxEnableICH;
		private System.Windows.Forms.TextBox textBoxUDPPort;
		private System.Windows.Forms.TextBox textBoxTCPPort;
		private System.Windows.Forms.TextBox textBoxMaxSourcesPerFile;
		private System.Windows.Forms.TextBox textBoxMaxConnections;
		private System.Windows.Forms.TextBox textBoxMaxUpSpeed;
		private System.Windows.Forms.TextBox textBoxMaxDownSpeed;
		private System.Windows.Forms.TextBox textBoxNick;
		private System.Windows.Forms.CheckBox checkBoxAutoConnect;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.Button buttonExpTemp;
		private System.Windows.Forms.Button buttonExpDown;
		private System.Windows.Forms.ListBox listBoxTmpFolder;
		private System.Windows.Forms.Button buttonRemoveTmp;
		private System.Windows.Forms.ListBox listBoxDownloadsFolder;
		private System.Windows.Forms.RadioButton radioSharedAll;
		private System.Windows.Forms.RadioButton radioSharedFriends;
		private System.Windows.Forms.RadioButton radioSharedNobody;
		private System.Windows.Forms.GroupBox groupBoxViewShared;
		private System.Windows.Forms.CheckBox checkBoxMinTray;
		private System.Windows.Forms.CheckBox checkBoxNotifications;
		private System.Windows.Forms.TextBox textBoxPreview;
		private System.Windows.Forms.Button buttonExplorePlayer;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.CheckBox checkBoxDownloadServerMet;
		private System.Windows.Forms.TextBox textBoxServerMetUri;
		private System.Windows.Forms.GroupBox groupBoxDownloadServerList;
		private System.Windows.Forms.GroupBox groupBoxConnection;
		private System.Windows.Forms.Label labelUDPPort;
		private System.Windows.Forms.Label labelTCPPort;
		private System.Windows.Forms.Label labelMaxSourcesPerFile;
		private System.Windows.Forms.Label labelMaxConnection;
		private System.Windows.Forms.Label labelMaxUpSpeed;
		private System.Windows.Forms.Label labelMaxDownSpeed;
		private System.Windows.Forms.Label labelDownloadFolder;
		private System.Windows.Forms.Label labelTempFolder;
		private System.Windows.Forms.Label labelNick;
		private System.Windows.Forms.Label labelVideoPLayer;
		private System.Windows.Forms.Button buttonTakeLinks;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAccept;
		private Crownwood.Magic.Controls.TabPage tabPageKernel;
		private Crownwood.Magic.Controls.TabPage tabPageInterface;
		private System.Windows.Forms.Button buttonRemoveDown;
		private Crownwood.Magic.Controls.TabPage tabPageRemote;
		private System.Windows.Forms.GroupBox groupBoxRemoteServer;
		private System.Windows.Forms.TextBox textBoxRemoteControlPassword;
		private System.Windows.Forms.Label labelRemoteControlPass;
		private System.Windows.Forms.TextBox textBoxRemoteControlPort;
		private System.Windows.Forms.CheckBox checkBoxRemoteControl;
		private System.Windows.Forms.RadioButton radioButtonStartLocal;
		private System.Windows.Forms.RadioButton radioButtonStartRemote;
		private System.Windows.Forms.RadioButton radioButtonStartSelect;
		private System.Windows.Forms.TextBox textBoxRemoteIP;
		private System.Windows.Forms.TextBox textBoxRemotePort;
		private System.Windows.Forms.TextBox textBoxRemotePassword;
		private System.Windows.Forms.ComboBox comboBoxSkin;
		private System.Windows.Forms.Label labelskin;
		private System.Windows.Forms.Label labelLanguage;
		private System.Windows.Forms.ComboBox comboBoxLanguage;
		private System.Windows.Forms.GroupBox groupBoxRemoteClient;
		private System.Windows.Forms.Label labelRemoteHostPassword;
		private System.Windows.Forms.Label labelRemoteHostPort;
		private System.Windows.Forms.Label labelRemoteHostIP;
		private System.Windows.Forms.Label labelRemoteControlPort;
		private System.Windows.Forms.CheckBox checkBoxOSig;
		private System.Windows.Forms.TextBox textBoxIncomingFolder;
		private System.Windows.Forms.Label labelSharedFolder;
		private System.Windows.Forms.Button buttonExIncomingFolder;
		private System.Windows.Forms.PictureBox pictureBoxPreview;
		private System.Windows.Forms.CheckBox checkBoxeLinkFromBrowserStopped;
		private System.Windows.Forms.CheckBox checkBoxIPFilter;
		private System.Windows.Forms.ToolTip toolTipOptions;
		private System.Windows.Forms.Label labelCompressionMethod;
		private System.Windows.Forms.ComboBox comboBoxCompressionMethod;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Label labelIPAllowed;
		private System.Windows.Forms.ListBox listBoxAllowedIP;
		private System.Windows.Forms.Button buttonAddIP;
		private System.Windows.Forms.Button buttonRemoveIP;
		private System.Windows.Forms.TextBox textBoxAddNewIP;
		private System.Windows.Forms.CheckBox checkBoxStartPaused;

		private Array listaCompressionMethod=Enum.GetValues(typeof(CompressionType));

		private struct LanguageInfo
		{
			private string m_NativeLanguage;
			private string m_CultureInfo;

			public LanguageInfo(string in_CultureInfo, string in_NativeLanguage)
			{
				m_NativeLanguage = in_NativeLanguage;
				m_CultureInfo = in_CultureInfo;

			}
			
			public string NativeLanguage
			{
				get 
				{
					return m_NativeLanguage;
				}
			}

			public string CultureInfo
			{
				get 
				{
					return m_CultureInfo;
				}
			}									
		}

		public FormOptions(CkernelGateway in_krnGateway)
		{
			InitializeComponent();
			krnGateway=in_krnGateway;
			tabPageKernel.Selected = true;
			m_Globalize();
			m_AddToolTip();
			m_ApplySkin();
			m_GetLanguageFiles();
			if (Directory.Exists(Application.StartupPath+Path.DirectorySeparatorChar+"skins\\"))
			{
				string[] dirs = Directory.GetDirectories(Application.StartupPath+Path.DirectorySeparatorChar+"skins\\");
				foreach (string folderName in dirs)
				{
					if (File.Exists(folderName+"\\skin.xml"))
						comboBoxSkin.Items.Add(Path.GetFileName(folderName));
				}
			}
			DisplayPreferences(krnGateway.GetConfig());
			m_DisplayInterfacePreferences();
		}
		
		private void m_Globalize()
		{
			this.Text = eLePhantForm.Globalization["LBL_OPTIONS"];
			tabPageKernel.Title = eLePhantForm.Globalization["LBL_KERNEL"];
			tabPageInterface.Title = eLePhantForm.Globalization["LBL_INTERFACE"];
			labelNick.Text = eLePhantForm.Globalization["LBL_NICK"]+":";
			groupBoxConnection.Text = eLePhantForm.Globalization["LBL_CONNECTION"];
			labelMaxDownSpeed.Text = eLePhantForm.Globalization["LBL_MAXDOWNSPEED"]+":";
			labelMaxUpSpeed.Text = eLePhantForm.Globalization["LBL_MAXUPSPEED"]+":";
			labelMaxConnection.Text = eLePhantForm.Globalization["LBL_MAXCONNECTIONS"]+":";
			labelMaxSourcesPerFile.Text = eLePhantForm.Globalization["LBL_MAXSOURCESPERFILE"]+":";
			labelTCPPort.Text = eLePhantForm.Globalization["LBL_TCPPORT"]+":";
			labelUDPPort.Text = eLePhantForm.Globalization["LBL_UDPPORT"]+":";
			checkBoxEnableICH.Text = eLePhantForm.Globalization["LBL_ENABLEICH"];
			checkBoxAutoConnect.Text= eLePhantForm.Globalization["LBL_AUTOCONNECT"];
			labelTempFolder.Text = eLePhantForm.Globalization["LBL_TEMPFOLDER"]+":";
			labelDownloadFolder.Text = eLePhantForm.Globalization["LBL_DOWNFOLDER"]+":";
			labelSharedFolder.Text = eLePhantForm.Globalization["LBL_SHAREDFOLDERS"]+":";
			buttonExpTemp.Text = eLePhantForm.Globalization["LBL_EXPTEMP"];
			buttonExpDown.Text = eLePhantForm.Globalization["LBL_EXPDOWN"];
			buttonExIncomingFolder.Text = eLePhantForm.Globalization["LBL_EXPSHARED"];
			buttonRemoveTmp.Text = eLePhantForm.Globalization["LBL_REMOVETEMP"];
			buttonRemoveDown.Text = eLePhantForm.Globalization["LBL_REMOVEDOWN"];
			groupBoxViewShared.Text = eLePhantForm.Globalization["LBL_VIEWSHARED"];
			radioSharedAll.Text = eLePhantForm.Globalization["LBL_ALL"];
			radioSharedFriends.Text = eLePhantForm.Globalization["LBL_FRIENDS"];
			radioSharedNobody.Text = eLePhantForm.Globalization["LBL_NOBODY"];
			groupBoxDownloadServerList.Text = eLePhantForm.Globalization["LBL_DOWNSERVERLIST"];
			checkBoxDownloadServerMet.Text = eLePhantForm.Globalization["LBL_DOWNSERVERMET"];
			buttonTakeLinks.Text = eLePhantForm.Globalization["LBL_TAKELINKS"];
			buttonAccept.Text = eLePhantForm.Globalization["LBL_ACCEPT"];
			buttonCancel.Text = eLePhantForm.Globalization["LBL_CANCEL"];
			//checkBoxConnectionAsk.Text = eLePhantForm.Globalization["LBL_CONNECTIONASK"];
			checkBoxMinTray.Text= eLePhantForm.Globalization["LBL_MINIMIZETRAY"];
			checkBoxNotifications.Text = eLePhantForm.Globalization["LBL_NOTIFICATIONS"];
			labelVideoPLayer.Text = eLePhantForm.Globalization["LBL_VIDEOPLAYER"]+":";
			buttonExplorePlayer.Text = eLePhantForm.Globalization["LBL_EXPPLAYER"];
			tabPageRemote.Title = eLePhantForm.Globalization["LBL_REMOTECONTROL"];
			groupBoxRemoteServer.Text = eLePhantForm.Globalization["LBL_REMOTECONTROL_SERVER"];
			groupBoxRemoteClient.Text = eLePhantForm.Globalization["LBL_REMOTECONTROL_CLIENT"];
			checkBoxRemoteControl.Text = eLePhantForm.Globalization["LBL_ENABLE_REMOTECONTROL"];
			labelRemoteControlPass.Text = eLePhantForm.Globalization["LBL_REMOTECONTROL_PASSWORD"]+":";
			labelRemoteControlPort.Text = eLePhantForm.Globalization["LBL_REMOTECONTROL_PORT"]+":";
			labelRemoteHostIP.Text = eLePhantForm.Globalization["LBL_REMOTEHOST_IP"]+":";
			labelRemoteHostPassword.Text = eLePhantForm.Globalization["LBL_REMOTEHOST_PASSWORD"]+":";
			labelRemoteHostPort.Text = eLePhantForm.Globalization["LBL_REMOTEHOST_PORT"]+":";
			radioButtonStartLocal.Text = eLePhantForm.Globalization["LBL_START_LPHANT_LOCALLY"];
			radioButtonStartRemote.Text = eLePhantForm.Globalization["LBL_CONNECTREMOTE"];
			radioButtonStartSelect.Text = eLePhantForm.Globalization["LBL_SELECT_CONNECTIONTYPE"];
			checkBoxOSig.Text = eLePhantForm.Globalization["LBL_ENABLEOSIG"];
			checkBoxeLinkFromBrowserStopped.Text = eLePhantForm.Globalization["LBL_ELINK_FROM_BROWSER_STOPPPED"];
			checkBoxIPFilter.Text=eLePhantForm.Globalization["LBL_ENABLEIPFILTER"];
			labelLanguage.Text=eLePhantForm.Globalization["LBL_LANGUAGE"]+":";
			labelskin.Text=eLePhantForm.Globalization["LBL_SKIN"]+":";
			labelCompressionMethod.Text=eLePhantForm.Globalization["LBL_COMPRESSION"]+":";
			labelIPAllowed.Text=eLePhantForm.Globalization["LBL_ALLOWEDIP"]+":";
			checkBoxStartPaused.Text=eLePhantForm.Globalization["LBL_STARTSTOPPEDFILE"];
		}

		public void Globalize()
		{
			m_Globalize();
		}
		
		private void m_AddToolTip()
		{
			toolTipOptions.SetToolTip(this.buttonTakeLinks, eLePhantForm.Globalization["LBL_TAKELINKS"]);
			toolTipOptions.SetToolTip(this.checkBoxAutoConnect, eLePhantForm.Globalization["LBL_AUTOCONNECT"]);
			toolTipOptions.SetToolTip(this.checkBoxDownloadServerMet, eLePhantForm.Globalization["LBL_DOWNSERVERMET"]);
			toolTipOptions.SetToolTip(this.checkBoxeLinkFromBrowserStopped, eLePhantForm.Globalization["LBL_ELINK_FROM_BROWSER_STOPPPED"]);
			toolTipOptions.SetToolTip(this.checkBoxEnableICH, eLePhantForm.Globalization["LBL_ENABLEICH"]);
			toolTipOptions.SetToolTip(this.checkBoxIPFilter, eLePhantForm.Globalization["LBL_ENABLEIPFILTER"]);
			toolTipOptions.SetToolTip(this.checkBoxMinTray, eLePhantForm.Globalization["LBL_MINIMIZETRAY"]);
			toolTipOptions.SetToolTip(this.checkBoxNotifications, eLePhantForm.Globalization["LBL_NOTIFICATIONS"]);
			toolTipOptions.SetToolTip(this.checkBoxOSig, eLePhantForm.Globalization["LBL_ENABLEOSIG"]);
			toolTipOptions.SetToolTip(this.checkBoxRemoteControl, eLePhantForm.Globalization["LBL_ENABLE_REMOTECONTROL"]);
			toolTipOptions.SetToolTip(this.groupBoxConnection, eLePhantForm.Globalization["LBL_CONNECTION"]);
			toolTipOptions.SetToolTip(this.groupBoxRemoteServer, eLePhantForm.Globalization["LBL_REMOTECONTROL_SERVER"]);
			toolTipOptions.SetToolTip(this.groupBoxRemoteClient, eLePhantForm.Globalization["LBL_REMOTECONTROL_CLIENT"]);
			toolTipOptions.SetToolTip(this.groupBoxDownloadServerList, eLePhantForm.Globalization["LBL_DOWNSERVERLIST"]);
			toolTipOptions.SetToolTip(this.groupBoxViewShared, eLePhantForm.Globalization["LBL_VIEWSHARED"]);
			toolTipOptions.SetToolTip(this.labelDownloadFolder, eLePhantForm.Globalization["LBL_DOWNFOLDER"]);
			toolTipOptions.SetToolTip(this.labelLanguage, eLePhantForm.Globalization["LBL_LANGUAGE"]);
			toolTipOptions.SetToolTip(this.labelMaxConnection, eLePhantForm.Globalization["LBL_MAXCONNECTIONS"]);
			toolTipOptions.SetToolTip(this.labelMaxDownSpeed, eLePhantForm.Globalization["LBL_MAXDOWNSPEED"]);
			toolTipOptions.SetToolTip(this.labelMaxSourcesPerFile, eLePhantForm.Globalization["LBL_MAXSOURCESPERFILE"]);
			toolTipOptions.SetToolTip(this.labelMaxUpSpeed, eLePhantForm.Globalization["LBL_MAXUPSPEED"]);
			toolTipOptions.SetToolTip(this.labelNick, eLePhantForm.Globalization["LBL_NICK"]);
			toolTipOptions.SetToolTip(this.labelRemoteControlPass, eLePhantForm.Globalization["LBL_REMOTECONTROL_PASSWORD"]);
			toolTipOptions.SetToolTip(this.labelRemoteControlPort, eLePhantForm.Globalization["LBL_REMOTECONTROL_PORT"]);
			toolTipOptions.SetToolTip(this.labelRemoteHostIP, eLePhantForm.Globalization["LBL_REMOTEHOST_IP"]);
			toolTipOptions.SetToolTip(this.labelRemoteHostPassword, eLePhantForm.Globalization["LBL_REMOTEHOST_PASSWORD"]);
			toolTipOptions.SetToolTip(this.labelRemoteHostPort, eLePhantForm.Globalization["LBL_REMOTEHOST_PORT"]);
			toolTipOptions.SetToolTip(this.labelSharedFolder, eLePhantForm.Globalization["LBL_SHAREDFOLDERS"]);
			toolTipOptions.SetToolTip(this.labelskin, eLePhantForm.Globalization["LBL_SKIN"]);
			toolTipOptions.SetToolTip(this.labelTCPPort, eLePhantForm.Globalization["LBL_TCPPORT"]);
			toolTipOptions.SetToolTip(this.labelTempFolder, eLePhantForm.Globalization["LBL_TEMPFOLDER"]);
			toolTipOptions.SetToolTip(this.labelUDPPort, eLePhantForm.Globalization["LBL_UDPPORT"]);
			toolTipOptions.SetToolTip(this.labelVideoPLayer, eLePhantForm.Globalization["LBL_VIDEOPLAYER"]);
			toolTipOptions.SetToolTip(this.radioButtonStartLocal, eLePhantForm.Globalization["LBL_START_LPHANT_LOCALLY"]);
			toolTipOptions.SetToolTip(this.radioButtonStartRemote, eLePhantForm.Globalization["LBL_CONNECTREMOTE"]);
			toolTipOptions.SetToolTip(this.radioButtonStartSelect, eLePhantForm.Globalization["LBL_SELECT_CONNECTIONTYPE"]);
			toolTipOptions.SetToolTip(this.radioSharedAll, eLePhantForm.Globalization["LBL_ALL"]);
			toolTipOptions.SetToolTip(this.radioSharedFriends, eLePhantForm.Globalization["LBL_FRIENDS"]);
			toolTipOptions.SetToolTip(this.radioSharedNobody, eLePhantForm.Globalization["LBL_NOBODY"]);
			
			//The diplay is not well displayed
			//toolTipOptions.SetToolTip(this.tabPageKernel, eLePhantForm.Globalization["LBL_KERNEL"]);
			//toolTipOptions.SetToolTip(this.tabPageInterface, eLePhantForm.Globalization["LBL_INTERFACE"]);
			//toolTipOptions.SetToolTip(this.tabPageRemote, eLePhantForm.Globalization["LBL_REMOTECONTROL"]);
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
		
		public void DisplayPreferences(InterfacePreferences preferences)
		{
			foreach (string sharedFolder in preferences.sharedFolders)
			{	
				if (sharedFolder==preferences.sharedFolders[0])
				{
					textBoxIncomingFolder.Text = sharedFolder;
				}
				else
				{
					listBoxDownloadsFolder.Items.Add(sharedFolder);
				}
			}
	
			foreach (string tmpFolder in preferences.TempFolders)
			{
				listBoxTmpFolder.Items.Add(tmpFolder);
			}
			
			this.textBoxMaxConnections.Text=preferences.maxConnexions.ToString();
			this.textBoxMaxDownSpeed.Text=preferences.maxDownloadRate.ToString();
			this.textBoxMaxSourcesPerFile.Text=preferences.maxSources.ToString();
			this.textBoxMaxUpSpeed.Text=preferences.maxUploadRate.ToString();
			this.textBoxNick.Text=preferences.UserName;
			this.textBoxTCPPort.Text=preferences.TCPPort.ToString();
			this.textBoxUDPPort.Text=preferences.UDPPort.ToString();
			this.checkBoxEnableICH.Checked=preferences.ICHEnabled;
			this.checkBoxAutoConnect.Checked=preferences.Autoreconect;
			this.checkBoxDownloadServerMet.Checked=preferences.DownloadServerMet;
			this.checkBoxIPFilter.Checked=preferences.IPFilterEnabled;
			this.textBoxServerMetUri.Text=preferences.ServerMetUri;
			
			this.comboBoxLanguage.DataSource = m_languageFiles;
			this.comboBoxLanguage.ValueMember = "CultureInfo";
			this.comboBoxLanguage.DisplayMember = "NativeLanguage";
			m_CurrentLanguage = m_GetCompatibleLangage(preferences.Language);
			this.comboBoxLanguage.SelectedValue = m_CurrentLanguage;
			
			radioSharedAll.Checked=(preferences.AllowViewShared==eLePhant.Types.Constants.AllowViewShared.All);
			radioSharedFriends.Checked=(preferences.AllowViewShared==eLePhant.Types.Constants.AllowViewShared.Friends);
			radioSharedNobody.Checked=(preferences.AllowViewShared==eLePhant.Types.Constants.AllowViewShared.Nobody);

			RegistryKey root = Registry.ClassesRoot;
			RegistryKey key=root.OpenSubKey("ed2k\\shell\\open\\command",true);
			if (key==null) key=root.CreateSubKey("ed2k\\shell\\open\\command");
			string regPath;
			regPath=(string)key.GetValue("");
			if (regPath!=Application.ExecutablePath+" \"%1\"")
			{
				buttonTakeLinks.Enabled=true;				
			}
			else
				buttonTakeLinks.Enabled=false;
			//remote control
			this.checkBoxRemoteControl.Checked=preferences.RemoteControlEnabled;
			textBoxRemoteControlPort.Text=preferences.RemoteControlPort.ToString();
			textBoxRemoteControlPassword.Text=preferences.RemoteControlPassword;
			checkBoxOSig.Checked=preferences.OSigEnabled;
			this.comboBoxCompressionMethod.DataSource=listaCompressionMethod;
			comboBoxCompressionMethod.SelectedItem=preferences.CompressionMethod;
			if (preferences.AllowedIP!=null)
				foreach (string tmpAllowedIP in preferences.AllowedIP)
				{
					listBoxAllowedIP.Items.Add(tmpAllowedIP);
				}
			this.checkBoxStartPaused.Checked=preferences.StartNextStoppedFile;
		}

		private void m_CheckPreferences()
		{
			Random rnd=new Random();
			
			if(textBoxTCPPort.Text.Equals("0")) textBoxTCPPort.Text="4662";
			if(textBoxUDPPort.Text.Equals("0")) textBoxUDPPort.Text="4672";
			if(textBoxRemotePort.Text.Equals("0")) textBoxRemotePort.Text="7777";

			if(textBoxTCPPort.TextLength==0) textBoxTCPPort.Text="4662";
			if(textBoxUDPPort.TextLength==0) textBoxUDPPort.Text="4672";
			if(textBoxMaxDownSpeed.TextLength==0) textBoxMaxDownSpeed.Text="50";
			if(textBoxMaxUpSpeed.TextLength==0) textBoxMaxUpSpeed.Text="16";
			if(textBoxRemotePort.TextLength==0) textBoxRemotePort.Text="7777";
			if(textBoxRemoteControlPort.TextLength==0) textBoxRemoteControlPort.Text=rnd.Next(6000,8000).ToString();
			if(textBoxRemotePort.TextLength==0) textBoxRemotePort.Text=textBoxRemoteControlPort.Text;
			if(textBoxMaxConnections.TextLength==0) textBoxMaxConnections.Text="180";
			if(textBoxMaxSourcesPerFile.TextLength==0) textBoxMaxSourcesPerFile.Text="900";

			if(textBoxTCPPort.Text.Equals(textBoxUDPPort.Text)) textBoxUDPPort.Text=rnd.Next(1000,65535).ToString();
			if(textBoxRemoteControlPort.Text.Equals(textBoxUDPPort.Text)) textBoxRemoteControlPort.Text=rnd.Next(6000,8000).ToString();
			if(textBoxRemoteControlPort.Text.Equals(textBoxTCPPort.Text)) textBoxRemoteControlPort.Text=rnd.Next(6000,8000).ToString();
		}

		public void SetPreferences()
		{	
			int i;
			
			m_CheckPreferences();
			InterfacePreferences preferences=new InterfacePreferences();
			preferences.sharedFolders=new string[listBoxDownloadsFolder.Items.Count+1];
			preferences.TempFolders=new string[listBoxTmpFolder.Items.Count];
			preferences.AllowedIP=new string[listBoxAllowedIP.Items.Count];
			preferences.sharedFolders[0]=textBoxIncomingFolder.Text;
			
			for (i=0;i<listBoxDownloadsFolder.Items.Count;i++)
			{
				preferences.sharedFolders[i+1]=(string)listBoxDownloadsFolder.Items[i];
			}
						
			for (i=0;i<listBoxTmpFolder.Items.Count;i++)
			{
				preferences.TempFolders[i]=(string)listBoxTmpFolder.Items[i];
			}
			
			preferences.maxConnexions=Convert.ToInt32(this.textBoxMaxConnections.Text);
			preferences.maxDownloadRate=(float)Convert.ToDouble(this.textBoxMaxDownSpeed.Text);
			preferences.maxSources=Convert.ToInt32(this.textBoxMaxSourcesPerFile.Text);
			preferences.maxUploadRate=(float)Convert.ToDouble(this.textBoxMaxUpSpeed.Text);
			preferences.UserName=this.textBoxNick.Text;
			preferences.TCPPort=Convert.ToUInt16(this.textBoxTCPPort.Text);
			preferences.UDPPort=Convert.ToUInt16(this.textBoxUDPPort.Text);
			preferences.ICHEnabled=this.checkBoxEnableICH.Checked;
			preferences.Autoreconect=this.checkBoxAutoConnect.Checked;
			preferences.maxServerFails=2;
			preferences.RemoteControlEnabled=this.checkBoxRemoteControl.Checked;
			preferences.RemoteControlPort=Convert.ToInt32(textBoxRemoteControlPort.Text);
			preferences.RemoteControlPassword=textBoxRemoteControlPassword.Text;
			preferences.DownloadServerMet=this.checkBoxDownloadServerMet.Checked;
			preferences.ServerMetUri=this.textBoxServerMetUri.Text;
			
			try
			{
				m_SelectedLanguage = comboBoxLanguage.SelectedValue.ToString();
			}
			catch
			{
				m_SelectedLanguage = "en-US";
			}
			
			preferences.Language=m_SelectedLanguage;

			if (radioSharedAll.Checked) preferences.AllowViewShared=eLePhant.Types.Constants.AllowViewShared.All;
			else if (radioSharedFriends.Checked) preferences.AllowViewShared=eLePhant.Types.Constants.AllowViewShared.Friends;
			else if (radioSharedNobody.Checked) preferences.AllowViewShared=eLePhant.Types.Constants.AllowViewShared.Nobody;
			
			preferences.OSigEnabled=this.checkBoxOSig.Checked;
			preferences.IPFilterEnabled=this.checkBoxIPFilter.Checked;
			preferences.CompressionMethod=(CompressionType)this.comboBoxCompressionMethod.SelectedItem;
			for (i=0;i<listBoxAllowedIP.Items.Count;i++)
			{
				preferences.AllowedIP[i]=(string)listBoxAllowedIP.Items[i];
			}
			preferences.StartNextStoppedFile=this.checkBoxStartPaused.Checked;
			krnGateway.SetConfig(preferences);
		}
		
		private void m_DisplayInterfacePreferences()
		{
			this.checkBoxMinTray.Checked=eLePhantForm.preferences.GetBool("MinimizeToTray",true);
			this.checkBoxNotifications.Checked=eLePhantForm.preferences.GetBool("TaskBarNotifier",true);
			string defaultplayer="";
			RegistryKey root = Registry.ClassesRoot;
			RegistryKey key=root.OpenSubKey("divxfile\\shell\\open\\command",false);
			if (key!=null) 
			{
				defaultplayer=(string)key.GetValue("");
				if (defaultplayer!=null)
				{
					defaultplayer=defaultplayer.Replace("\"","");
					defaultplayer=defaultplayer.Replace("%1","");
				}
				else defaultplayer="";
			}
			
			this.textBoxPreview.Text=eLePhantForm.preferences.GetString("PreviewPlayer",defaultplayer);
			//remote control
			this.radioButtonStartRemote.Checked=eLePhantForm.preferences.GetBool("StartupRemote",false);
			this.radioButtonStartLocal.Checked=eLePhantForm.preferences.GetBool("StartupLocal",true);
			this.radioButtonStartSelect.Checked=eLePhantForm.preferences.GetBool("StartupSelect",false);
			this.textBoxRemoteIP.Text=eLePhantForm.preferences.GetString("RemoteIP","");
			this.textBoxRemotePort.Text=eLePhantForm.preferences.GetInt("RemotePort",Convert.ToInt32(textBoxRemoteControlPort.Text)).ToString();
			this.textBoxRemotePassword.Text=eLePhantForm.preferences.GetString("RemotePassword",textBoxRemoteControlPort.Text);
			comboBoxSkin.Text=eLePhantForm.preferences.GetString("Skin","default");
			comboBoxSkin_SelectedIndexChanged(this,null);
			this.checkBoxeLinkFromBrowserStopped.Checked=eLePhantForm.preferences.GetBool("eLinkFromBrowserStopped",false);

		}
		private void m_SaveInterfacePreferences()
		{
			eLePhantForm.preferences.SetProperty("MinimizeToTray",this.checkBoxMinTray.Checked);
			eLePhantForm.preferences.SetProperty("TaskBarNotifier",this.checkBoxNotifications.Checked);
			eLePhantForm.preferences.SetProperty("PreviewPlayer",this.textBoxPreview.Text);
			eLePhantForm.preferences.SetProperty("Language",m_SelectedLanguage);
			//remote control
			eLePhantForm.preferences.SetProperty("StartupSelect",this.radioButtonStartSelect.Checked);
			eLePhantForm.preferences.SetProperty("StartupRemote",this.radioButtonStartRemote.Checked);
			eLePhantForm.preferences.SetProperty("StartupLocal",this.radioButtonStartLocal.Checked);
			eLePhantForm.preferences.SetProperty("RemoteIP",this.textBoxRemoteIP.Text);
			eLePhantForm.preferences.SetProperty("RemotePort",Convert.ToInt32(textBoxRemotePort.Text));
			eLePhantForm.preferences.SetProperty("RemotePassword",textBoxRemotePassword.Text);
			eLePhantForm.preferences.SetProperty("Skin",comboBoxSkin.Text);
			eLePhantForm.preferences.SetProperty("eLinkFromBrowserStopped",this.checkBoxeLinkFromBrowserStopped.Checked);
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormOptions));
			this.tabControl1 = new Crownwood.Magic.Controls.TabControl();
			this.tabPageInterface = new Crownwood.Magic.Controls.TabPage();
			this.checkBoxStartPaused = new System.Windows.Forms.CheckBox();
			this.checkBoxeLinkFromBrowserStopped = new System.Windows.Forms.CheckBox();
			this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
			this.checkBoxOSig = new System.Windows.Forms.CheckBox();
			this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
			this.labelLanguage = new System.Windows.Forms.Label();
			this.comboBoxSkin = new System.Windows.Forms.ComboBox();
			this.labelskin = new System.Windows.Forms.Label();
			this.buttonExplorePlayer = new System.Windows.Forms.Button();
			this.textBoxPreview = new System.Windows.Forms.TextBox();
			this.labelVideoPLayer = new System.Windows.Forms.Label();
			this.checkBoxNotifications = new System.Windows.Forms.CheckBox();
			this.checkBoxMinTray = new System.Windows.Forms.CheckBox();
			this.tabPageKernel = new Crownwood.Magic.Controls.TabPage();
			this.buttonExIncomingFolder = new System.Windows.Forms.Button();
			this.labelSharedFolder = new System.Windows.Forms.Label();
			this.textBoxIncomingFolder = new System.Windows.Forms.TextBox();
			this.groupBoxDownloadServerList = new System.Windows.Forms.GroupBox();
			this.textBoxServerMetUri = new System.Windows.Forms.TextBox();
			this.checkBoxDownloadServerMet = new System.Windows.Forms.CheckBox();
			this.groupBoxViewShared = new System.Windows.Forms.GroupBox();
			this.radioSharedNobody = new System.Windows.Forms.RadioButton();
			this.radioSharedFriends = new System.Windows.Forms.RadioButton();
			this.radioSharedAll = new System.Windows.Forms.RadioButton();
			this.buttonRemoveDown = new System.Windows.Forms.Button();
			this.listBoxDownloadsFolder = new System.Windows.Forms.ListBox();
			this.buttonRemoveTmp = new System.Windows.Forms.Button();
			this.listBoxTmpFolder = new System.Windows.Forms.ListBox();
			this.buttonExpDown = new System.Windows.Forms.Button();
			this.buttonExpTemp = new System.Windows.Forms.Button();
			this.groupBoxConnection = new System.Windows.Forms.GroupBox();
			this.checkBoxIPFilter = new System.Windows.Forms.CheckBox();
			this.checkBoxAutoConnect = new System.Windows.Forms.CheckBox();
			this.checkBoxEnableICH = new System.Windows.Forms.CheckBox();
			this.textBoxUDPPort = new System.Windows.Forms.TextBox();
			this.labelUDPPort = new System.Windows.Forms.Label();
			this.textBoxTCPPort = new System.Windows.Forms.TextBox();
			this.labelTCPPort = new System.Windows.Forms.Label();
			this.textBoxMaxSourcesPerFile = new System.Windows.Forms.TextBox();
			this.textBoxMaxConnections = new System.Windows.Forms.TextBox();
			this.textBoxMaxUpSpeed = new System.Windows.Forms.TextBox();
			this.textBoxMaxDownSpeed = new System.Windows.Forms.TextBox();
			this.labelMaxSourcesPerFile = new System.Windows.Forms.Label();
			this.labelMaxConnection = new System.Windows.Forms.Label();
			this.labelMaxUpSpeed = new System.Windows.Forms.Label();
			this.labelMaxDownSpeed = new System.Windows.Forms.Label();
			this.labelDownloadFolder = new System.Windows.Forms.Label();
			this.labelTempFolder = new System.Windows.Forms.Label();
			this.textBoxNick = new System.Windows.Forms.TextBox();
			this.labelNick = new System.Windows.Forms.Label();
			this.tabPageRemote = new Crownwood.Magic.Controls.TabPage();
			this.groupBoxRemoteClient = new System.Windows.Forms.GroupBox();
			this.textBoxRemotePassword = new System.Windows.Forms.TextBox();
			this.labelRemoteHostPassword = new System.Windows.Forms.Label();
			this.labelRemoteHostPort = new System.Windows.Forms.Label();
			this.labelRemoteHostIP = new System.Windows.Forms.Label();
			this.textBoxRemotePort = new System.Windows.Forms.TextBox();
			this.textBoxRemoteIP = new System.Windows.Forms.TextBox();
			this.radioButtonStartSelect = new System.Windows.Forms.RadioButton();
			this.radioButtonStartRemote = new System.Windows.Forms.RadioButton();
			this.radioButtonStartLocal = new System.Windows.Forms.RadioButton();
			this.groupBoxRemoteServer = new System.Windows.Forms.GroupBox();
			this.textBoxAddNewIP = new System.Windows.Forms.TextBox();
			this.buttonRemoveIP = new System.Windows.Forms.Button();
			this.buttonAddIP = new System.Windows.Forms.Button();
			this.listBoxAllowedIP = new System.Windows.Forms.ListBox();
			this.labelIPAllowed = new System.Windows.Forms.Label();
			this.labelCompressionMethod = new System.Windows.Forms.Label();
			this.comboBoxCompressionMethod = new System.Windows.Forms.ComboBox();
			this.textBoxRemoteControlPassword = new System.Windows.Forms.TextBox();
			this.labelRemoteControlPass = new System.Windows.Forms.Label();
			this.textBoxRemoteControlPort = new System.Windows.Forms.TextBox();
			this.labelRemoteControlPort = new System.Windows.Forms.Label();
			this.checkBoxRemoteControl = new System.Windows.Forms.CheckBox();
			this.buttonTakeLinks = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonAccept = new System.Windows.Forms.Button();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.toolTipOptions = new System.Windows.Forms.ToolTip(this.components);
			this.tabPageInterface.SuspendLayout();
			this.tabPageKernel.SuspendLayout();
			this.groupBoxDownloadServerList.SuspendLayout();
			this.groupBoxViewShared.SuspendLayout();
			this.groupBoxConnection.SuspendLayout();
			this.tabPageRemote.SuspendLayout();
			this.groupBoxRemoteClient.SuspendLayout();
			this.groupBoxRemoteServer.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.tabControl1.HideTabsMode = Crownwood.Magic.Controls.TabControl.HideTabsModes.ShowAlways;
			this.tabControl1.Location = new System.Drawing.Point(1, 4);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.PositionTop = true;
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.SelectedTab = this.tabPageKernel;
			this.tabControl1.Size = new System.Drawing.Size(422, 394);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.TabPages.AddRange(new Crownwood.Magic.Controls.TabPage[] {
																						  this.tabPageKernel,
																						  this.tabPageInterface,
																						  this.tabPageRemote});
			this.tabControl1.SelectionChanged += new System.EventHandler(this.tabControl1_SelectionChanged);
			// 
			// tabPageInterface
			// 
			this.tabPageInterface.Controls.Add(this.checkBoxStartPaused);
			this.tabPageInterface.Controls.Add(this.checkBoxeLinkFromBrowserStopped);
			this.tabPageInterface.Controls.Add(this.pictureBoxPreview);
			this.tabPageInterface.Controls.Add(this.checkBoxOSig);
			this.tabPageInterface.Controls.Add(this.comboBoxLanguage);
			this.tabPageInterface.Controls.Add(this.labelLanguage);
			this.tabPageInterface.Controls.Add(this.comboBoxSkin);
			this.tabPageInterface.Controls.Add(this.labelskin);
			this.tabPageInterface.Controls.Add(this.buttonExplorePlayer);
			this.tabPageInterface.Controls.Add(this.textBoxPreview);
			this.tabPageInterface.Controls.Add(this.labelVideoPLayer);
			this.tabPageInterface.Controls.Add(this.checkBoxNotifications);
			this.tabPageInterface.Controls.Add(this.checkBoxMinTray);
			this.tabPageInterface.Location = new System.Drawing.Point(0, 0);
			this.tabPageInterface.Name = "tabPageInterface";
			this.tabPageInterface.Selected = false;
			this.tabPageInterface.Size = new System.Drawing.Size(422, 369);
			this.tabPageInterface.TabIndex = 1;
			this.tabPageInterface.Title = "Interface";
			// 
			// checkBoxStartPaused
			// 
			this.checkBoxStartPaused.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxStartPaused.Location = new System.Drawing.Point(13, 162);
			this.checkBoxStartPaused.Name = "checkBoxStartPaused";
			this.checkBoxStartPaused.Size = new System.Drawing.Size(401, 20);
			this.checkBoxStartPaused.TabIndex = 31;
			this.checkBoxStartPaused.Text = "Start next stopped file when a download completes";
			// 
			// checkBoxeLinkFromBrowserStopped
			// 
			this.checkBoxeLinkFromBrowserStopped.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxeLinkFromBrowserStopped.Location = new System.Drawing.Point(13, 140);
			this.checkBoxeLinkFromBrowserStopped.Name = "checkBoxeLinkFromBrowserStopped";
			this.checkBoxeLinkFromBrowserStopped.Size = new System.Drawing.Size(401, 18);
			this.checkBoxeLinkFromBrowserStopped.TabIndex = 30;
			this.checkBoxeLinkFromBrowserStopped.Text = "eLinks from browser are stopped";
			// 
			// pictureBoxPreview
			// 
			this.pictureBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxPreview.Location = new System.Drawing.Point(193, 194);
			this.pictureBoxPreview.Name = "pictureBoxPreview";
			this.pictureBoxPreview.Size = new System.Drawing.Size(178, 100);
			this.pictureBoxPreview.TabIndex = 29;
			this.pictureBoxPreview.TabStop = false;
			// 
			// checkBoxOSig
			// 
			this.checkBoxOSig.Checked = true;
			this.checkBoxOSig.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxOSig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxOSig.Location = new System.Drawing.Point(13, 117);
			this.checkBoxOSig.Name = "checkBoxOSig";
			this.checkBoxOSig.Size = new System.Drawing.Size(297, 18);
			this.checkBoxOSig.TabIndex = 28;
			this.checkBoxOSig.Text = "Enable Online signature";
			// 
			// comboBoxLanguage
			// 
			this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLanguage.Location = new System.Drawing.Point(154, 89);
			this.comboBoxLanguage.Name = "comboBoxLanguage";
			this.comboBoxLanguage.Size = new System.Drawing.Size(168, 22);
			this.comboBoxLanguage.TabIndex = 27;
			// 
			// labelLanguage
			// 
			this.labelLanguage.Location = new System.Drawing.Point(6, 88);
			this.labelLanguage.Name = "labelLanguage";
			this.labelLanguage.Size = new System.Drawing.Size(142, 17);
			this.labelLanguage.TabIndex = 26;
			this.labelLanguage.Text = "Language:";
			this.labelLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxSkin
			// 
			this.comboBoxSkin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSkin.Items.AddRange(new object[] {
															  "default"});
			this.comboBoxSkin.Location = new System.Drawing.Point(61, 192);
			this.comboBoxSkin.Name = "comboBoxSkin";
			this.comboBoxSkin.Size = new System.Drawing.Size(124, 22);
			this.comboBoxSkin.TabIndex = 25;
			this.comboBoxSkin.SelectedIndexChanged += new System.EventHandler(this.comboBoxSkin_SelectedIndexChanged);
			// 
			// labelskin
			// 
			this.labelskin.Location = new System.Drawing.Point(3, 196);
			this.labelskin.Name = "labelskin";
			this.labelskin.Size = new System.Drawing.Size(51, 14);
			this.labelskin.TabIndex = 24;
			this.labelskin.Text = "Skin:";
			this.labelskin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonExplorePlayer
			// 
			this.buttonExplorePlayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonExplorePlayer.Location = new System.Drawing.Point(327, 60);
			this.buttonExplorePlayer.Name = "buttonExplorePlayer";
			this.buttonExplorePlayer.Size = new System.Drawing.Size(84, 24);
			this.buttonExplorePlayer.TabIndex = 23;
			this.buttonExplorePlayer.Text = "Explore...";
			this.buttonExplorePlayer.Click += new System.EventHandler(this.buttonExplorePlayer_Click);
			// 
			// textBoxPreview
			// 
			this.textBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxPreview.Location = new System.Drawing.Point(153, 62);
			this.textBoxPreview.Name = "textBoxPreview";
			this.textBoxPreview.Size = new System.Drawing.Size(170, 22);
			this.textBoxPreview.TabIndex = 22;
			this.textBoxPreview.Text = "";
			// 
			// labelVideoPLayer
			// 
			this.labelVideoPLayer.Location = new System.Drawing.Point(5, 64);
			this.labelVideoPLayer.Name = "labelVideoPLayer";
			this.labelVideoPLayer.Size = new System.Drawing.Size(144, 14);
			this.labelVideoPLayer.TabIndex = 21;
			this.labelVideoPLayer.Text = "Video player:";
			this.labelVideoPLayer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBoxNotifications
			// 
			this.checkBoxNotifications.Checked = true;
			this.checkBoxNotifications.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxNotifications.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxNotifications.Location = new System.Drawing.Point(13, 37);
			this.checkBoxNotifications.Name = "checkBoxNotifications";
			this.checkBoxNotifications.Size = new System.Drawing.Size(312, 18);
			this.checkBoxNotifications.TabIndex = 1;
			this.checkBoxNotifications.Text = "Enable notifications";
			// 
			// checkBoxMinTray
			// 
			this.checkBoxMinTray.Checked = true;
			this.checkBoxMinTray.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxMinTray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxMinTray.Location = new System.Drawing.Point(13, 9);
			this.checkBoxMinTray.Name = "checkBoxMinTray";
			this.checkBoxMinTray.Size = new System.Drawing.Size(310, 18);
			this.checkBoxMinTray.TabIndex = 0;
			this.checkBoxMinTray.Text = "Minimize to tray";
			// 
			// tabPageKernel
			// 
			this.tabPageKernel.Controls.Add(this.buttonExIncomingFolder);
			this.tabPageKernel.Controls.Add(this.labelSharedFolder);
			this.tabPageKernel.Controls.Add(this.textBoxIncomingFolder);
			this.tabPageKernel.Controls.Add(this.groupBoxDownloadServerList);
			this.tabPageKernel.Controls.Add(this.groupBoxViewShared);
			this.tabPageKernel.Controls.Add(this.buttonRemoveDown);
			this.tabPageKernel.Controls.Add(this.listBoxDownloadsFolder);
			this.tabPageKernel.Controls.Add(this.buttonRemoveTmp);
			this.tabPageKernel.Controls.Add(this.listBoxTmpFolder);
			this.tabPageKernel.Controls.Add(this.buttonExpDown);
			this.tabPageKernel.Controls.Add(this.buttonExpTemp);
			this.tabPageKernel.Controls.Add(this.groupBoxConnection);
			this.tabPageKernel.Controls.Add(this.labelDownloadFolder);
			this.tabPageKernel.Controls.Add(this.labelTempFolder);
			this.tabPageKernel.Controls.Add(this.textBoxNick);
			this.tabPageKernel.Controls.Add(this.labelNick);
			this.tabPageKernel.Location = new System.Drawing.Point(0, 0);
			this.tabPageKernel.Name = "tabPageKernel";
			this.tabPageKernel.Size = new System.Drawing.Size(422, 369);
			this.tabPageKernel.TabIndex = 0;
			this.tabPageKernel.Title = "Kernel";
			// 
			// buttonExIncomingFolder
			// 
			this.buttonExIncomingFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonExIncomingFolder.Location = new System.Drawing.Point(338, 220);
			this.buttonExIncomingFolder.Name = "buttonExIncomingFolder";
			this.buttonExIncomingFolder.Size = new System.Drawing.Size(74, 22);
			this.buttonExIncomingFolder.TabIndex = 30;
			this.buttonExIncomingFolder.Text = "Browse";
			this.buttonExIncomingFolder.Click += new System.EventHandler(this.buttonExIncomingFolder_Click);
			// 
			// labelSharedFolder
			// 
			this.labelSharedFolder.Location = new System.Drawing.Point(2, 267);
			this.labelSharedFolder.Name = "labelSharedFolder";
			this.labelSharedFolder.Size = new System.Drawing.Size(129, 14);
			this.labelSharedFolder.TabIndex = 29;
			this.labelSharedFolder.Text = "Shared folders:";
			this.labelSharedFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxIncomingFolder
			// 
			this.textBoxIncomingFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxIncomingFolder.Location = new System.Drawing.Point(134, 222);
			this.textBoxIncomingFolder.Name = "textBoxIncomingFolder";
			this.textBoxIncomingFolder.ReadOnly = true;
			this.textBoxIncomingFolder.Size = new System.Drawing.Size(200, 22);
			this.textBoxIncomingFolder.TabIndex = 28;
			this.textBoxIncomingFolder.Text = "";
			// 
			// groupBoxDownloadServerList
			// 
			this.groupBoxDownloadServerList.Controls.Add(this.textBoxServerMetUri);
			this.groupBoxDownloadServerList.Controls.Add(this.checkBoxDownloadServerMet);
			this.groupBoxDownloadServerList.Location = new System.Drawing.Point(232, 307);
			this.groupBoxDownloadServerList.Name = "groupBoxDownloadServerList";
			this.groupBoxDownloadServerList.Size = new System.Drawing.Size(180, 58);
			this.groupBoxDownloadServerList.TabIndex = 27;
			this.groupBoxDownloadServerList.TabStop = false;
			this.groupBoxDownloadServerList.Text = "Download server list";
			// 
			// textBoxServerMetUri
			// 
			this.textBoxServerMetUri.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxServerMetUri.Location = new System.Drawing.Point(35, 26);
			this.textBoxServerMetUri.Name = "textBoxServerMetUri";
			this.textBoxServerMetUri.Size = new System.Drawing.Size(136, 22);
			this.textBoxServerMetUri.TabIndex = 1;
			this.textBoxServerMetUri.Text = "";
			// 
			// checkBoxDownloadServerMet
			// 
			this.checkBoxDownloadServerMet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxDownloadServerMet.Location = new System.Drawing.Point(13, 30);
			this.checkBoxDownloadServerMet.Name = "checkBoxDownloadServerMet";
			this.checkBoxDownloadServerMet.Size = new System.Drawing.Size(15, 15);
			this.checkBoxDownloadServerMet.TabIndex = 0;
			// 
			// groupBoxViewShared
			// 
			this.groupBoxViewShared.Controls.Add(this.radioSharedNobody);
			this.groupBoxViewShared.Controls.Add(this.radioSharedFriends);
			this.groupBoxViewShared.Controls.Add(this.radioSharedAll);
			this.groupBoxViewShared.Location = new System.Drawing.Point(8, 307);
			this.groupBoxViewShared.Name = "groupBoxViewShared";
			this.groupBoxViewShared.Size = new System.Drawing.Size(216, 56);
			this.groupBoxViewShared.TabIndex = 26;
			this.groupBoxViewShared.TabStop = false;
			this.groupBoxViewShared.Text = "Allow view shared files to:";
			// 
			// radioSharedNobody
			// 
			this.radioSharedNobody.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioSharedNobody.Location = new System.Drawing.Point(140, 28);
			this.radioSharedNobody.Name = "radioSharedNobody";
			this.radioSharedNobody.Size = new System.Drawing.Size(69, 16);
			this.radioSharedNobody.TabIndex = 2;
			this.radioSharedNobody.Text = "None";
			// 
			// radioSharedFriends
			// 
			this.radioSharedFriends.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioSharedFriends.Location = new System.Drawing.Point(71, 28);
			this.radioSharedFriends.Name = "radioSharedFriends";
			this.radioSharedFriends.Size = new System.Drawing.Size(64, 17);
			this.radioSharedFriends.TabIndex = 1;
			this.radioSharedFriends.Text = "Friends";
			// 
			// radioSharedAll
			// 
			this.radioSharedAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioSharedAll.Location = new System.Drawing.Point(12, 27);
			this.radioSharedAll.Name = "radioSharedAll";
			this.radioSharedAll.Size = new System.Drawing.Size(56, 17);
			this.radioSharedAll.TabIndex = 0;
			this.radioSharedAll.Text = "All";
			// 
			// buttonRemoveDown
			// 
			this.buttonRemoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonRemoveDown.Location = new System.Drawing.Point(338, 278);
			this.buttonRemoveDown.Name = "buttonRemoveDown";
			this.buttonRemoveDown.Size = new System.Drawing.Size(76, 24);
			this.buttonRemoveDown.TabIndex = 25;
			this.buttonRemoveDown.Text = "Remove";
			this.buttonRemoveDown.Click += new System.EventHandler(this.buttonRemoveShared_Click);
			// 
			// listBoxDownloadsFolder
			// 
			this.listBoxDownloadsFolder.ItemHeight = 14;
			this.listBoxDownloadsFolder.Location = new System.Drawing.Point(134, 252);
			this.listBoxDownloadsFolder.Name = "listBoxDownloadsFolder";
			this.listBoxDownloadsFolder.Size = new System.Drawing.Size(200, 46);
			this.listBoxDownloadsFolder.TabIndex = 24;
			// 
			// buttonRemoveTmp
			// 
			this.buttonRemoveTmp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonRemoveTmp.Location = new System.Drawing.Point(338, 188);
			this.buttonRemoveTmp.Name = "buttonRemoveTmp";
			this.buttonRemoveTmp.Size = new System.Drawing.Size(75, 24);
			this.buttonRemoveTmp.TabIndex = 23;
			this.buttonRemoveTmp.Text = "Remove";
			this.buttonRemoveTmp.Click += new System.EventHandler(this.buttonRemoveTmp_Click);
			// 
			// listBoxTmpFolder
			// 
			this.listBoxTmpFolder.ItemHeight = 14;
			this.listBoxTmpFolder.Location = new System.Drawing.Point(134, 164);
			this.listBoxTmpFolder.Name = "listBoxTmpFolder";
			this.listBoxTmpFolder.Size = new System.Drawing.Size(201, 46);
			this.listBoxTmpFolder.TabIndex = 22;
			// 
			// buttonExpDown
			// 
			this.buttonExpDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonExpDown.Location = new System.Drawing.Point(338, 250);
			this.buttonExpDown.Name = "buttonExpDown";
			this.buttonExpDown.Size = new System.Drawing.Size(75, 24);
			this.buttonExpDown.TabIndex = 21;
			this.buttonExpDown.Text = "Add...";
			this.buttonExpDown.Click += new System.EventHandler(this.buttonExpDown_Click);
			// 
			// buttonExpTemp
			// 
			this.buttonExpTemp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonExpTemp.Location = new System.Drawing.Point(339, 161);
			this.buttonExpTemp.Name = "buttonExpTemp";
			this.buttonExpTemp.Size = new System.Drawing.Size(73, 24);
			this.buttonExpTemp.TabIndex = 20;
			this.buttonExpTemp.Text = "Add...";
			this.buttonExpTemp.Click += new System.EventHandler(this.buttonExpTemp_Click);
			// 
			// groupBoxConnection
			// 
			this.groupBoxConnection.Controls.Add(this.checkBoxIPFilter);
			this.groupBoxConnection.Controls.Add(this.checkBoxAutoConnect);
			this.groupBoxConnection.Controls.Add(this.checkBoxEnableICH);
			this.groupBoxConnection.Controls.Add(this.textBoxUDPPort);
			this.groupBoxConnection.Controls.Add(this.labelUDPPort);
			this.groupBoxConnection.Controls.Add(this.textBoxTCPPort);
			this.groupBoxConnection.Controls.Add(this.labelTCPPort);
			this.groupBoxConnection.Controls.Add(this.textBoxMaxSourcesPerFile);
			this.groupBoxConnection.Controls.Add(this.textBoxMaxConnections);
			this.groupBoxConnection.Controls.Add(this.textBoxMaxUpSpeed);
			this.groupBoxConnection.Controls.Add(this.textBoxMaxDownSpeed);
			this.groupBoxConnection.Controls.Add(this.labelMaxSourcesPerFile);
			this.groupBoxConnection.Controls.Add(this.labelMaxConnection);
			this.groupBoxConnection.Controls.Add(this.labelMaxUpSpeed);
			this.groupBoxConnection.Controls.Add(this.labelMaxDownSpeed);
			this.groupBoxConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBoxConnection.Location = new System.Drawing.Point(16, 30);
			this.groupBoxConnection.Name = "groupBoxConnection";
			this.groupBoxConnection.Size = new System.Drawing.Size(397, 128);
			this.groupBoxConnection.TabIndex = 19;
			this.groupBoxConnection.TabStop = false;
			this.groupBoxConnection.Text = "Connection";
			// 
			// checkBoxIPFilter
			// 
			this.checkBoxIPFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxIPFilter.Location = new System.Drawing.Point(226, 108);
			this.checkBoxIPFilter.Name = "checkBoxIPFilter";
			this.checkBoxIPFilter.Size = new System.Drawing.Size(156, 16);
			this.checkBoxIPFilter.TabIndex = 29;
			this.checkBoxIPFilter.Text = "Enable IP Filter";
			// 
			// checkBoxAutoConnect
			// 
			this.checkBoxAutoConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxAutoConnect.Location = new System.Drawing.Point(226, 88);
			this.checkBoxAutoConnect.Name = "checkBoxAutoConnect";
			this.checkBoxAutoConnect.Size = new System.Drawing.Size(156, 16);
			this.checkBoxAutoConnect.TabIndex = 28;
			this.checkBoxAutoConnect.Text = "Auto connect";
			// 
			// checkBoxEnableICH
			// 
			this.checkBoxEnableICH.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxEnableICH.Location = new System.Drawing.Point(226, 68);
			this.checkBoxEnableICH.Name = "checkBoxEnableICH";
			this.checkBoxEnableICH.Size = new System.Drawing.Size(154, 16);
			this.checkBoxEnableICH.TabIndex = 27;
			this.checkBoxEnableICH.Text = "Enable ICH";
			// 
			// textBoxUDPPort
			// 
			this.textBoxUDPPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxUDPPort.Location = new System.Drawing.Point(314, 45);
			this.textBoxUDPPort.MaxLength = 5;
			this.textBoxUDPPort.Name = "textBoxUDPPort";
			this.textBoxUDPPort.Size = new System.Drawing.Size(46, 22);
			this.textBoxUDPPort.TabIndex = 26;
			this.textBoxUDPPort.Text = "";
			this.textBoxUDPPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_UDPport);
			this.textBoxUDPPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUpCheck);
			// 
			// labelUDPPort
			// 
			this.labelUDPPort.Location = new System.Drawing.Point(228, 48);
			this.labelUDPPort.Name = "labelUDPPort";
			this.labelUDPPort.Size = new System.Drawing.Size(83, 14);
			this.labelUDPPort.TabIndex = 25;
			this.labelUDPPort.Text = "UDP Port:";
			this.labelUDPPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxTCPPort
			// 
			this.textBoxTCPPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxTCPPort.Location = new System.Drawing.Point(314, 20);
			this.textBoxTCPPort.MaxLength = 5;
			this.textBoxTCPPort.Name = "textBoxTCPPort";
			this.textBoxTCPPort.Size = new System.Drawing.Size(46, 22);
			this.textBoxTCPPort.TabIndex = 24;
			this.textBoxTCPPort.Text = "";
			this.textBoxTCPPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_TCPport);
			this.textBoxTCPPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUpCheck);
			// 
			// labelTCPPort
			// 
			this.labelTCPPort.Location = new System.Drawing.Point(230, 22);
			this.labelTCPPort.Name = "labelTCPPort";
			this.labelTCPPort.Size = new System.Drawing.Size(81, 14);
			this.labelTCPPort.TabIndex = 23;
			this.labelTCPPort.Text = "TCP Port:";
			this.labelTCPPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxMaxSourcesPerFile
			// 
			this.textBoxMaxSourcesPerFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxMaxSourcesPerFile.Location = new System.Drawing.Point(153, 95);
			this.textBoxMaxSourcesPerFile.MaxLength = 5;
			this.textBoxMaxSourcesPerFile.Name = "textBoxMaxSourcesPerFile";
			this.textBoxMaxSourcesPerFile.Size = new System.Drawing.Size(46, 22);
			this.textBoxMaxSourcesPerFile.TabIndex = 22;
			this.textBoxMaxSourcesPerFile.Text = "";
			this.textBoxMaxSourcesPerFile.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_MaxSourcesPerFile);
			// 
			// textBoxMaxConnections
			// 
			this.textBoxMaxConnections.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxMaxConnections.Location = new System.Drawing.Point(152, 69);
			this.textBoxMaxConnections.MaxLength = 5;
			this.textBoxMaxConnections.Name = "textBoxMaxConnections";
			this.textBoxMaxConnections.Size = new System.Drawing.Size(46, 22);
			this.textBoxMaxConnections.TabIndex = 21;
			this.textBoxMaxConnections.Text = "";
			this.textBoxMaxConnections.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_MaxConnections);
			// 
			// textBoxMaxUpSpeed
			// 
			this.textBoxMaxUpSpeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxMaxUpSpeed.Location = new System.Drawing.Point(152, 45);
			this.textBoxMaxUpSpeed.MaxLength = 5;
			this.textBoxMaxUpSpeed.Name = "textBoxMaxUpSpeed";
			this.textBoxMaxUpSpeed.Size = new System.Drawing.Size(46, 22);
			this.textBoxMaxUpSpeed.TabIndex = 20;
			this.textBoxMaxUpSpeed.Text = "";
			this.textBoxMaxUpSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_MaxUpSpeed);
			// 
			// textBoxMaxDownSpeed
			// 
			this.textBoxMaxDownSpeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxMaxDownSpeed.Location = new System.Drawing.Point(152, 19);
			this.textBoxMaxDownSpeed.MaxLength = 5;
			this.textBoxMaxDownSpeed.Name = "textBoxMaxDownSpeed";
			this.textBoxMaxDownSpeed.Size = new System.Drawing.Size(46, 22);
			this.textBoxMaxDownSpeed.TabIndex = 19;
			this.textBoxMaxDownSpeed.Text = "";
			this.textBoxMaxDownSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_MaxDownSpeed);
			// 
			// labelMaxSourcesPerFile
			// 
			this.labelMaxSourcesPerFile.Location = new System.Drawing.Point(8, 96);
			this.labelMaxSourcesPerFile.Name = "labelMaxSourcesPerFile";
			this.labelMaxSourcesPerFile.Size = new System.Drawing.Size(141, 14);
			this.labelMaxSourcesPerFile.TabIndex = 18;
			this.labelMaxSourcesPerFile.Text = "Max sources per file:";
			this.labelMaxSourcesPerFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMaxConnection
			// 
			this.labelMaxConnection.Location = new System.Drawing.Point(8, 72);
			this.labelMaxConnection.Name = "labelMaxConnection";
			this.labelMaxConnection.Size = new System.Drawing.Size(141, 14);
			this.labelMaxConnection.TabIndex = 17;
			this.labelMaxConnection.Text = "Max connections:";
			this.labelMaxConnection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMaxUpSpeed
			// 
			this.labelMaxUpSpeed.Location = new System.Drawing.Point(9, 46);
			this.labelMaxUpSpeed.Name = "labelMaxUpSpeed";
			this.labelMaxUpSpeed.Size = new System.Drawing.Size(139, 14);
			this.labelMaxUpSpeed.TabIndex = 16;
			this.labelMaxUpSpeed.Text = "Max up speed:";
			this.labelMaxUpSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMaxDownSpeed
			// 
			this.labelMaxDownSpeed.Location = new System.Drawing.Point(9, 22);
			this.labelMaxDownSpeed.Name = "labelMaxDownSpeed";
			this.labelMaxDownSpeed.Size = new System.Drawing.Size(139, 14);
			this.labelMaxDownSpeed.TabIndex = 15;
			this.labelMaxDownSpeed.Text = "Max down speed:";
			this.labelMaxDownSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDownloadFolder
			// 
			this.labelDownloadFolder.Location = new System.Drawing.Point(2, 223);
			this.labelDownloadFolder.Name = "labelDownloadFolder";
			this.labelDownloadFolder.Size = new System.Drawing.Size(130, 14);
			this.labelDownloadFolder.TabIndex = 17;
			this.labelDownloadFolder.Text = "Downloads folder:";
			this.labelDownloadFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTempFolder
			// 
			this.labelTempFolder.Location = new System.Drawing.Point(2, 178);
			this.labelTempFolder.Name = "labelTempFolder";
			this.labelTempFolder.Size = new System.Drawing.Size(129, 14);
			this.labelTempFolder.TabIndex = 15;
			this.labelTempFolder.Text = "Temp. folder:";
			this.labelTempFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxNick
			// 
			this.textBoxNick.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxNick.Location = new System.Drawing.Point(79, 4);
			this.textBoxNick.Name = "textBoxNick";
			this.textBoxNick.Size = new System.Drawing.Size(159, 22);
			this.textBoxNick.TabIndex = 9;
			this.textBoxNick.Text = "";
			// 
			// labelNick
			// 
			this.labelNick.Location = new System.Drawing.Point(14, 7);
			this.labelNick.Name = "labelNick";
			this.labelNick.Size = new System.Drawing.Size(59, 14);
			this.labelNick.TabIndex = 8;
			this.labelNick.Text = "Nick:";
			this.labelNick.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabPageRemote
			// 
			this.tabPageRemote.Controls.Add(this.groupBoxRemoteClient);
			this.tabPageRemote.Controls.Add(this.groupBoxRemoteServer);
			this.tabPageRemote.Location = new System.Drawing.Point(0, 0);
			this.tabPageRemote.Name = "tabPageRemote";
			this.tabPageRemote.Selected = false;
			this.tabPageRemote.Size = new System.Drawing.Size(422, 369);
			this.tabPageRemote.TabIndex = 2;
			this.tabPageRemote.Title = "Remote control";
			// 
			// groupBoxRemoteClient
			// 
			this.groupBoxRemoteClient.Controls.Add(this.textBoxRemotePassword);
			this.groupBoxRemoteClient.Controls.Add(this.labelRemoteHostPassword);
			this.groupBoxRemoteClient.Controls.Add(this.labelRemoteHostPort);
			this.groupBoxRemoteClient.Controls.Add(this.labelRemoteHostIP);
			this.groupBoxRemoteClient.Controls.Add(this.textBoxRemotePort);
			this.groupBoxRemoteClient.Controls.Add(this.textBoxRemoteIP);
			this.groupBoxRemoteClient.Controls.Add(this.radioButtonStartSelect);
			this.groupBoxRemoteClient.Controls.Add(this.radioButtonStartRemote);
			this.groupBoxRemoteClient.Controls.Add(this.radioButtonStartLocal);
			this.groupBoxRemoteClient.Location = new System.Drawing.Point(13, 192);
			this.groupBoxRemoteClient.Name = "groupBoxRemoteClient";
			this.groupBoxRemoteClient.Size = new System.Drawing.Size(405, 166);
			this.groupBoxRemoteClient.TabIndex = 36;
			this.groupBoxRemoteClient.TabStop = false;
			this.groupBoxRemoteClient.Text = "Remote Control Client";
			// 
			// textBoxRemotePassword
			// 
			this.textBoxRemotePassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxRemotePassword.Location = new System.Drawing.Point(215, 131);
			this.textBoxRemotePassword.Name = "textBoxRemotePassword";
			this.textBoxRemotePassword.PasswordChar = '*';
			this.textBoxRemotePassword.Size = new System.Drawing.Size(103, 22);
			this.textBoxRemotePassword.TabIndex = 45;
			this.textBoxRemotePassword.Text = "";
			// 
			// labelRemoteHostPassword
			// 
			this.labelRemoteHostPassword.Location = new System.Drawing.Point(14, 135);
			this.labelRemoteHostPassword.Name = "labelRemoteHostPassword";
			this.labelRemoteHostPassword.Size = new System.Drawing.Size(195, 14);
			this.labelRemoteHostPassword.TabIndex = 44;
			this.labelRemoteHostPassword.Text = "Remote host password:";
			// 
			// labelRemoteHostPort
			// 
			this.labelRemoteHostPort.Location = new System.Drawing.Point(260, 106);
			this.labelRemoteHostPort.Name = "labelRemoteHostPort";
			this.labelRemoteHostPort.Size = new System.Drawing.Size(58, 14);
			this.labelRemoteHostPort.TabIndex = 43;
			this.labelRemoteHostPort.Text = "Port:";
			// 
			// labelRemoteHostIP
			// 
			this.labelRemoteHostIP.Location = new System.Drawing.Point(14, 108);
			this.labelRemoteHostIP.Name = "labelRemoteHostIP";
			this.labelRemoteHostIP.Size = new System.Drawing.Size(131, 14);
			this.labelRemoteHostIP.TabIndex = 42;
			this.labelRemoteHostIP.Text = "Remote host IP:";
			// 
			// textBoxRemotePort
			// 
			this.textBoxRemotePort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxRemotePort.Location = new System.Drawing.Point(324, 102);
			this.textBoxRemotePort.MaxLength = 5;
			this.textBoxRemotePort.Name = "textBoxRemotePort";
			this.textBoxRemotePort.Size = new System.Drawing.Size(46, 22);
			this.textBoxRemotePort.TabIndex = 41;
			this.textBoxRemotePort.Text = "";
			this.textBoxRemotePort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_RemotePort);
			this.textBoxRemotePort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUpCheck);
			// 
			// textBoxRemoteIP
			// 
			this.textBoxRemoteIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxRemoteIP.Location = new System.Drawing.Point(151, 102);
			this.textBoxRemoteIP.Name = "textBoxRemoteIP";
			this.textBoxRemoteIP.Size = new System.Drawing.Size(103, 22);
			this.textBoxRemoteIP.TabIndex = 40;
			this.textBoxRemoteIP.Text = "";
			// 
			// radioButtonStartSelect
			// 
			this.radioButtonStartSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioButtonStartSelect.Location = new System.Drawing.Point(14, 49);
			this.radioButtonStartSelect.Name = "radioButtonStartSelect";
			this.radioButtonStartSelect.Size = new System.Drawing.Size(386, 20);
			this.radioButtonStartSelect.TabIndex = 28;
			this.radioButtonStartSelect.Text = "Select connection type or remote  host when lphant starts";
			// 
			// radioButtonStartRemote
			// 
			this.radioButtonStartRemote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioButtonStartRemote.Location = new System.Drawing.Point(14, 74);
			this.radioButtonStartRemote.Name = "radioButtonStartRemote";
			this.radioButtonStartRemote.Size = new System.Drawing.Size(385, 20);
			this.radioButtonStartRemote.TabIndex = 27;
			this.radioButtonStartRemote.Text = "Connect to this remote lphant host:";
			// 
			// radioButtonStartLocal
			// 
			this.radioButtonStartLocal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioButtonStartLocal.Location = new System.Drawing.Point(14, 25);
			this.radioButtonStartLocal.Name = "radioButtonStartLocal";
			this.radioButtonStartLocal.Size = new System.Drawing.Size(385, 18);
			this.radioButtonStartLocal.TabIndex = 26;
			this.radioButtonStartLocal.Text = "Start lphant locally (not remote control)";
			// 
			// groupBoxRemoteServer
			// 
			this.groupBoxRemoteServer.Controls.Add(this.textBoxAddNewIP);
			this.groupBoxRemoteServer.Controls.Add(this.buttonRemoveIP);
			this.groupBoxRemoteServer.Controls.Add(this.buttonAddIP);
			this.groupBoxRemoteServer.Controls.Add(this.listBoxAllowedIP);
			this.groupBoxRemoteServer.Controls.Add(this.labelIPAllowed);
			this.groupBoxRemoteServer.Controls.Add(this.labelCompressionMethod);
			this.groupBoxRemoteServer.Controls.Add(this.comboBoxCompressionMethod);
			this.groupBoxRemoteServer.Controls.Add(this.textBoxRemoteControlPassword);
			this.groupBoxRemoteServer.Controls.Add(this.labelRemoteControlPass);
			this.groupBoxRemoteServer.Controls.Add(this.textBoxRemoteControlPort);
			this.groupBoxRemoteServer.Controls.Add(this.labelRemoteControlPort);
			this.groupBoxRemoteServer.Controls.Add(this.checkBoxRemoteControl);
			this.groupBoxRemoteServer.Location = new System.Drawing.Point(12, 8);
			this.groupBoxRemoteServer.Name = "groupBoxRemoteServer";
			this.groupBoxRemoteServer.Size = new System.Drawing.Size(406, 176);
			this.groupBoxRemoteServer.TabIndex = 35;
			this.groupBoxRemoteServer.TabStop = false;
			this.groupBoxRemoteServer.Text = "Remote Control Server";
			// 
			// textBoxAddNewIP
			// 
			this.textBoxAddNewIP.Location = new System.Drawing.Point(93, 146);
			this.textBoxAddNewIP.Name = "textBoxAddNewIP";
			this.textBoxAddNewIP.Size = new System.Drawing.Size(112, 22);
			this.textBoxAddNewIP.TabIndex = 48;
			this.textBoxAddNewIP.Text = "";
			// 
			// buttonRemoveIP
			// 
			this.buttonRemoveIP.Location = new System.Drawing.Point(217, 124);
			this.buttonRemoveIP.Name = "buttonRemoveIP";
			this.buttonRemoveIP.Size = new System.Drawing.Size(24, 21);
			this.buttonRemoveIP.TabIndex = 47;
			this.buttonRemoveIP.Text = "-";
			this.buttonRemoveIP.Click += new System.EventHandler(this.buttonRemoveIP_Click);
			// 
			// buttonAddIP
			// 
			this.buttonAddIP.Location = new System.Drawing.Point(217, 148);
			this.buttonAddIP.Name = "buttonAddIP";
			this.buttonAddIP.Size = new System.Drawing.Size(24, 21);
			this.buttonAddIP.TabIndex = 46;
			this.buttonAddIP.Text = "+";
			this.buttonAddIP.Click += new System.EventHandler(this.buttonAddIP_Click);
			// 
			// listBoxAllowedIP
			// 
			this.listBoxAllowedIP.ItemHeight = 14;
			this.listBoxAllowedIP.Location = new System.Drawing.Point(250, 124);
			this.listBoxAllowedIP.Name = "listBoxAllowedIP";
			this.listBoxAllowedIP.Size = new System.Drawing.Size(132, 46);
			this.listBoxAllowedIP.TabIndex = 45;
			// 
			// labelIPAllowed
			// 
			this.labelIPAllowed.Location = new System.Drawing.Point(10, 124);
			this.labelIPAllowed.Name = "labelIPAllowed";
			this.labelIPAllowed.Size = new System.Drawing.Size(196, 17);
			this.labelIPAllowed.TabIndex = 44;
			this.labelIPAllowed.Text = "IP Allowed:";
			// 
			// labelCompressionMethod
			// 
			this.labelCompressionMethod.Location = new System.Drawing.Point(10, 98);
			this.labelCompressionMethod.Name = "labelCompressionMethod";
			this.labelCompressionMethod.Size = new System.Drawing.Size(196, 17);
			this.labelCompressionMethod.TabIndex = 41;
			this.labelCompressionMethod.Text = "Method of compression:";
			// 
			// comboBoxCompressionMethod
			// 
			this.comboBoxCompressionMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCompressionMethod.Location = new System.Drawing.Point(216, 94);
			this.comboBoxCompressionMethod.Name = "comboBoxCompressionMethod";
			this.comboBoxCompressionMethod.Size = new System.Drawing.Size(101, 22);
			this.comboBoxCompressionMethod.TabIndex = 40;
			// 
			// textBoxRemoteControlPassword
			// 
			this.textBoxRemoteControlPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxRemoteControlPassword.Location = new System.Drawing.Point(216, 68);
			this.textBoxRemoteControlPassword.Name = "textBoxRemoteControlPassword";
			this.textBoxRemoteControlPassword.PasswordChar = '*';
			this.textBoxRemoteControlPassword.Size = new System.Drawing.Size(103, 22);
			this.textBoxRemoteControlPassword.TabIndex = 39;
			this.textBoxRemoteControlPassword.Text = "";
			// 
			// labelRemoteControlPass
			// 
			this.labelRemoteControlPass.Location = new System.Drawing.Point(10, 72);
			this.labelRemoteControlPass.Name = "labelRemoteControlPass";
			this.labelRemoteControlPass.Size = new System.Drawing.Size(196, 17);
			this.labelRemoteControlPass.TabIndex = 38;
			this.labelRemoteControlPass.Text = "Remote control password:";
			// 
			// textBoxRemoteControlPort
			// 
			this.textBoxRemoteControlPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxRemoteControlPort.Location = new System.Drawing.Point(216, 44);
			this.textBoxRemoteControlPort.MaxLength = 5;
			this.textBoxRemoteControlPort.Name = "textBoxRemoteControlPort";
			this.textBoxRemoteControlPort.Size = new System.Drawing.Size(46, 22);
			this.textBoxRemoteControlPort.TabIndex = 37;
			this.textBoxRemoteControlPort.Text = "";
			this.textBoxRemoteControlPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress_RemoteControlPort);
			this.textBoxRemoteControlPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUpCheck);
			// 
			// labelRemoteControlPort
			// 
			this.labelRemoteControlPort.Location = new System.Drawing.Point(10, 48);
			this.labelRemoteControlPort.Name = "labelRemoteControlPort";
			this.labelRemoteControlPort.Size = new System.Drawing.Size(196, 17);
			this.labelRemoteControlPort.TabIndex = 36;
			this.labelRemoteControlPort.Text = "Remote control port (TCP):";
			// 
			// checkBoxRemoteControl
			// 
			this.checkBoxRemoteControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBoxRemoteControl.Location = new System.Drawing.Point(10, 25);
			this.checkBoxRemoteControl.Name = "checkBoxRemoteControl";
			this.checkBoxRemoteControl.Size = new System.Drawing.Size(156, 16);
			this.checkBoxRemoteControl.TabIndex = 35;
			this.checkBoxRemoteControl.Text = "Enable remote control";
			// 
			// buttonTakeLinks
			// 
			this.buttonTakeLinks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonTakeLinks.Location = new System.Drawing.Point(23, 405);
			this.buttonTakeLinks.Name = "buttonTakeLinks";
			this.buttonTakeLinks.Size = new System.Drawing.Size(144, 24);
			this.buttonTakeLinks.TabIndex = 27;
			this.buttonTakeLinks.Text = "Take ed2k links";
			this.buttonTakeLinks.Click += new System.EventHandler(this.buttonTake_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.Location = new System.Drawing.Point(316, 404);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(98, 24);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonAccept
			// 
			this.buttonAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAccept.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAccept.Location = new System.Drawing.Point(214, 404);
			this.buttonAccept.Name = "buttonAccept";
			this.buttonAccept.Size = new System.Drawing.Size(98, 24);
			this.buttonAccept.TabIndex = 2;
			this.buttonAccept.Text = "Accept";
			this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
			// 
			// FormOptions
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.ClientSize = new System.Drawing.Size(422, 435);
			this.Controls.Add(this.buttonAccept);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.buttonTakeLinks);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormOptions";
			this.Text = "Options";
			this.tabPageInterface.ResumeLayout(false);
			this.tabPageKernel.ResumeLayout(false);
			this.groupBoxDownloadServerList.ResumeLayout(false);
			this.groupBoxViewShared.ResumeLayout(false);
			this.groupBoxConnection.ResumeLayout(false);
			this.tabPageRemote.ResumeLayout(false);
			this.groupBoxRemoteClient.ResumeLayout(false);
			this.groupBoxRemoteServer.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAccept_Click(object sender, System.EventArgs e)
		{	
			SetPreferences();
			m_SaveInterfacePreferences();
			this.Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
		
		private void buttonExIncomingFolder_Click(object sender, System.EventArgs e)
		{
			folderBrowserDialog1.ShowNewFolderButton=true;
			folderBrowserDialog1.SelectedPath=textBoxIncomingFolder.Text;
			if (folderBrowserDialog1.ShowDialog()==DialogResult.OK)
			{
				textBoxIncomingFolder.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void buttonExpTemp_Click(object sender, System.EventArgs e)
		{
			folderBrowserDialog1.ShowNewFolderButton=true;
			folderBrowserDialog1.SelectedPath=(string)listBoxTmpFolder.Items[0];
			if (folderBrowserDialog1.ShowDialog()==DialogResult.OK)
			{
				if (!listBoxTmpFolder.Items.Contains(folderBrowserDialog1.SelectedPath))
					listBoxTmpFolder.Items.Add(folderBrowserDialog1.SelectedPath);
			}
		}

		private void buttonExpDown_Click(object sender, System.EventArgs e)
		{
			folderBrowserDialog1.ShowNewFolderButton=true;
			if(listBoxDownloadsFolder.Items.Count>0) folderBrowserDialog1.SelectedPath=(string)listBoxDownloadsFolder.Items[0];
			if (folderBrowserDialog1.ShowDialog()==DialogResult.OK)
			{
				if (!listBoxDownloadsFolder.Items.Contains(folderBrowserDialog1.SelectedPath))
					listBoxDownloadsFolder.Items.Add(folderBrowserDialog1.SelectedPath);
			}
		}

		private void buttonRemoveTmp_Click(object sender, System.EventArgs e)
		{
			if ((listBoxTmpFolder.Items.Count>1)&&(listBoxTmpFolder.SelectedIndex>=0))
				listBoxTmpFolder.Items.RemoveAt(listBoxTmpFolder.SelectedIndex);
		}

		private void buttonRemoveShared_Click(object sender, System.EventArgs e)
		{
			if (listBoxDownloadsFolder.SelectedIndex>=0)
				listBoxDownloadsFolder.Items.RemoveAt(listBoxDownloadsFolder.SelectedIndex);		
		}

		private void buttonTake_Click(object sender, System.EventArgs e)
		{
			RegistryKey root = Registry.ClassesRoot;
			RegistryKey key;
			key=root.CreateSubKey("ed2k");
			key.SetValue("","URL: ed2k Protocol");
			key.SetValue("URL Protocol","");
			key=root.CreateSubKey("ed2k\\DefaultIcon");
			key.SetValue("",Application.ExecutablePath);
			key=root.OpenSubKey("ed2k\\shell\\open\\command",true);
			if (key==null) key=root.CreateSubKey("ed2k\\shell\\open\\command");
			string regPath;
			regPath=(string)key.GetValue("");
			if (regPath!=Application.ExecutablePath+" \"%1\"")
			{
				regPath=Application.ExecutablePath+" \"%1\"";
				key.SetValue("",regPath);
				buttonTakeLinks.Enabled=false;
			}		
		}

		private void tabControl1_SelectionChanged(object sender, System.EventArgs e)
		{
		
		}

		private void buttonExplorePlayer_Click(object sender, System.EventArgs e)
		{
			openFileDialog1.FileName=this.textBoxPreview.Text;
			if (openFileDialog1.ShowDialog()==DialogResult.OK)
			{
				textBoxPreview.Text=openFileDialog1.FileName;
			}
		}
		
		private void m_GetLanguageFiles()
		{
			// Process the directory list
			DirectoryInfo dir = new DirectoryInfo(Application.StartupPath+Path.DirectorySeparatorChar+"Language");
			foreach (FileSystemInfo entry in dir.GetFiles("interface_*.xml"))
			{
				// generate regex
				Regex regex = new Regex(
					@"interface_(?<lang>.*)\.xml",
					RegexOptions.IgnoreCase
					| RegexOptions.Multiline
					| RegexOptions.IgnorePatternWhitespace
					| RegexOptions.Compiled
					);

				// get language code from filename
				string lang = regex.Match(entry.Name).Result("${lang}");

				// add to language files
				try
				{
					m_languageFiles.Add(new LanguageInfo(lang,(new CultureInfo(lang)).NativeName));
				}
				catch(Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
			}
		}

		private string  m_GetCompatibleLangage(string in_CultureInfo)
		{
			try
			{
				//if language is not specified system language is selected
				if (in_CultureInfo.Length==0) return "en-US";
				//if the language is not found english is selected
				if (!m_languageFiles.Contains(new LanguageInfo(in_CultureInfo,(new CultureInfo(in_CultureInfo)).NativeName))) 
				{
					string [] splitedLanguage=null; 
					splitedLanguage=in_CultureInfo.Split("-".ToCharArray());
					string compatibleLanguage=splitedLanguage[0]+"-"+splitedLanguage[0].ToUpper();
					if (m_languageFiles.Contains(new LanguageInfo(compatibleLanguage,(new CultureInfo(compatibleLanguage)).NativeName)))
						return compatibleLanguage;
					else
						return "en-US";
				}
				return in_CultureInfo;
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
				return "en-US";
			}
		}


		private void m_DoSkin(Control parentControl)
		{
			tabControl1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			tabControl1.ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
			tabControl1.TextInactiveColor=eLePhantForm.Skin.GetColor("tabsInactiveForeColor");

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
				else if	(c.GetType()==typeof(ComboBox))
				{
					((ComboBox)c).ForeColor=eLePhantForm.Skin.GetColor("TextBoxForeColor");
					((ComboBox)c).BackColor=eLePhantForm.Skin.GetColor("TextBoxBackColor");
				}
				else if	(c.GetType()==typeof(RadioButton))
				{
					((RadioButton)c).ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
					((RadioButton)c).BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");
				}
				else if	(c.GetType()==typeof(GroupBox))
				{
					((GroupBox)c).ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
					if ((c.Controls!=null)&&(c.Controls.Count>0)) m_DoSkin(c);
				}
				else if (c.GetType()==typeof(CheckBox))
				{
						((CheckBox)c).ForeColor=eLePhantForm.Skin.GetColor("labelsForeColor");
						((CheckBox)c).BackColor=eLePhantForm.Skin.GetColor("labelsBackColor");
				}
				else if ((c.Controls!=null)&&(c.Controls.Count>0)) m_DoSkin(c);
			}

		}
		
		private void m_ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("defaultBackColor");
			this.tabControl1.BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			m_DoSkin(this);
		}

		private void comboBoxSkin_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (comboBoxSkin.SelectedIndex==0)
			{
				pictureBoxPreview.Image=new Bitmap(typeof(eLePhantForm),"Client.Resources.Classic.preview.JPG");
			}
			else
			{
				if (File.Exists(Application.StartupPath+Path.DirectorySeparatorChar+"skins\\"+comboBoxSkin.Text+"\\preview.jpg"))
				{
					try
					{
						pictureBoxPreview.Image=new Bitmap(Application.StartupPath+Path.DirectorySeparatorChar+"skins\\"+comboBoxSkin.Text+"\\preview.jpg");
					}
					catch
					{
						pictureBoxPreview.Image=null;
					}
				}
				else 
					pictureBoxPreview.Image=null;
			}
		}
		
		//48=0, 57=9 , 8=CANCEL
		private void OnKeyPress_TCPport(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void OnKeyPress_UDPport(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void OnKeyPress_MaxDownSpeed(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void OnKeyPress_MaxUpSpeed(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void OnKeyPress_MaxConnections(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void OnKeyPress_MaxSourcesPerFile(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void OnKeyPress_RemoteControlPort(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void OnKeyPress_RemotePort(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if((e.KeyChar<(char)48 || e.KeyChar>(char)57) && e.KeyChar!=(char)8) e.Handled=true;
		}

		private void OnKeyUpCheck(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			TextBox textbox=(TextBox)sender;
			if(textbox.TextLength>0 && uint.Parse(textbox.Text)>ushort.MaxValue)
			{
				textbox.Text = textbox.Text.Remove(textbox.Text.Length-1,1);
				textbox.SelectionStart = textbox.TextLength;
			}
		}

		private void buttonRemoveIP_Click(object sender, System.EventArgs e)
		{
			if (listBoxAllowedIP.SelectedItem!=null)
				listBoxAllowedIP.Items.Remove(listBoxAllowedIP.SelectedItem);
		}

		private void buttonAddIP_Click(object sender, System.EventArgs e)
		{
			if (textBoxAddNewIP.Text.Length > 5)
			{
				listBoxAllowedIP.Items.Add(textBoxAddNewIP.Text);
				textBoxAddNewIP.Text="";
			}
		}
	}
}
