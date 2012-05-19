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
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

using eLePhant.Types;
using eLePhant.Classes;

namespace eLePhant.eDonkey
{
	/// <summary>
	/// Open source eDonkey client for the .NET platform, it supports the eMule extended protocol
	/// </summary>
	public class CKernel
	{
		//internal static CPreferences Preferences;
		internal static Config Preferences;

		internal static Classes.Globalization Globalization;

		internal static CListener Listener;
		internal static CConnectionsList ConnectionsList;
		internal static CServersList ServersList;
		internal static CFilesList FilesList;
		internal static CClientsList ClientsList;
		internal static CFriendsList FriendsList;
		public static CInterfaceGateway[] InterfaceGateway=new CInterfaceGateway[5];
		internal static CQueue Queue;
		internal static SortedList Searchs;
		internal static int LastSearch;
		internal static CUDPListener UDPListener;
		internal static CRemoting Sremoto;
		internal static CStatistics GlobalStatistics;
		internal static ChttpDownload httpDownload;
		internal static CIPFilter IPFilter;
		internal static CSourcesOld SourcesOld;

		internal static CCategoriesList CategoriesList;

		private static bool m_PortChanged;
		private static string m_Version;
#if VERBOSE
		internal static CLogClient LogClient;
#endif
		public CKernel()
		{
#if VERBOSE
			LogClient=new CLogClient();
#endif			
			m_PortChanged=false;
			m_Version=(Protocol.ELEPHANT_VERSION_MAJOR-1).ToString()+"."+Protocol.ELEPHANT_VERSION.ToString("00");

			Preferences=new Config(DllDirectory, "config.xml", "0.02", "lphantKernel");
			Preferences.PropertyChanged+=new Config.PropertyEventHandler(OnPropertyChanged);
			Preferences.PropertyDefaults+=new Config.PropertyDefaultHandler(OnGetDefaultProperty);
			Preferences.PropertyLoaded+=new Config.PropertyLoadedHandler(OnPropertyLoaded);
			Preferences.LoadProperties();

			Globalization=new Classes.Globalization(LangDirectory, "kernel_", "xml",Preferences.GetString("Language"));

			SourcesOld=new CSourcesOld();
			Listener=new CListener();
			ConnectionsList=new CConnectionsList();
			ServersList=new CServersList();
			
			FilesList=new CFilesList();
			ClientsList=new CClientsList();
			InterfaceGateway[0]=new CInterfaceGateway();		
			InterfaceGateway[0].apw=true;
			//InterfaceGateway[0].CheckPw(Preferences.GetString("RemotePassword"));
			//FilesList.LoadSharedFiles(); // > StartKernel()
			FriendsList=new CFriendsList();

			GlobalStatistics=new CStatistics();
			if (Preferences.GetBool("RemoteControlEnabled"))
			{
				Sremoto=new CRemoting();
			}
			Searchs=new SortedList();
			Queue=new CQueue();
			UDPListener=new CUDPListener(); // UDPListener.Start(); > StartKernel()

			CategoriesList=new CCategoriesList();

			httpDownload=new ChttpDownload();
			httpDownload.Downloaded+=new DownloadedEventHandler(OnFileDownloaded);
		}

		internal static void StartKernel()
		{
			FilesList.LoadSharedFiles();

			UDPListener.Start();
			Listener.Start();

			CLog.Log(Constants.Log.Notify, "INIT_KRN", Version);
			LastSearch=1;

			if (Preferences.GetBool("IPFilterEnabled"))
			{
				IPFilter=new CIPFilter();
				IPFilter.LoadIPFilter(DllDirectory,"ipfilter.dat",128);
			}
			else
				IPFilter=null;

			if(Preferences.GetBool("DownloadServerMet"))
				httpDownload.DownloadFile(Preferences.GetString("ServerMetUri"),"","server.met",false);
			else
				CKernel.ServersList.Load("",true);

			//currently for fun only, should be removed
			//httpDownload.GetInfo(@"http://www.lphant.com/Messages/TestVersions-pre5-test2.txt");

			// TODO: Version
			//Xml example :
			string[] myLog=new string[] {"version","log"};
			httpDownload.GetInfoXML(@"http://www.lphant.com/Messages/v1.01.xml?lang="+Globalization.Language,myLog,true);

			//if threaded = true, it returns "", else can be used later to get
			//CLog.Log(Constants.Log.Info,httpDownload.getReadInfoFromXML("log"));
			//

			if (Preferences.GetBool("AutoReconnect"))
			{
				ServersList.ConnectToAnyServer();
			}
		}

		~CKernel()
		{
			Preferences.PropertyChanged-=new Config.PropertyEventHandler(OnPropertyChanged);
			Preferences.PropertyDefaults-=new Config.PropertyDefaultHandler(OnGetDefaultProperty);
			Preferences.PropertyLoaded-=new Config.PropertyLoadedHandler(OnPropertyLoaded);
			httpDownload.Downloaded-=new DownloadedEventHandler(OnFileDownloaded);
		}

		/// <summary>
		/// DoReportBug sends an exception message to the lphant webpage.
		/// </summary>
		/// <param name="exception">Exception details.</param>
		public void DoReportBug(string exception)
		{
			try
			{
				string uriString = "http://www.lphant.com/report/bugs.php";

				// Create a new WebClient instance.
				WebClient webClient = new WebClient();

				// Create a new NameValueCollection instance to hold some custom parameters to be posted to the URL.
				NameValueCollection values = new NameValueCollection();

				values.Add("version", Protocol.ELEPHANT_VERSION.ToString());
				values.Add("exception", exception);
				values.Add("username", Preferences.GetString("UserName", ""));

				// Upload the values.
				byte[] responseArray = webClient.UploadValues(uriString, "POST", values);

				// Decode and display the response.
				string result = Encoding.ASCII.GetString(responseArray);
				if (result == "100") {} // all is OK
			}
			catch {}
		}

		/// <summary>
		/// Return the lphant kernel version as string.
		/// See: AssemblyInfo.cs - AssemblyVersion
		/// </summary>
		public static string Version
		{
			get
			{
				//return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
				//current version is 0.07 not 0.7.0.0 so assemply version does not reflect lphant version
				//this is called continuously so we cach the version value to not to have to build it each time
				return m_Version;
			}
		}

		/// <summary>
		/// Return the directory of the lphant kernel.
		/// </summary>
		public static string DllDirectory
		{
			get
			{
				// Application.StartupPath needs System.Windows.Forms
				string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
				return System.IO.Path.GetDirectoryName(location) + Path.DirectorySeparatorChar;
			}
		}

		/// <summary>
		/// Return the directory for the language files.
		/// </summary>
		public static string LangDirectory
		{
			get
			{
				string directory = DllDirectory;
				return directory + "Language" + Path.DirectorySeparatorChar;
			}
		}

		internal void OnFileDownloaded(object sender, EventArgs e)
		{
			CKernel.ServersList.Load("",false);
		}

		/// <summary>
		/// Event handler for preferences class to get the default values.
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="e">default property arguments</param>
		internal void OnGetDefaultProperty(object sender, PropertyDefaultArgs e)
		{
			switch (e.Key)
			{
				case "TCPPort":
					e.Value=(ushort)4662;
					break;
				case "UDPPort":
					e.Value=(ushort)4672;
					break;
				case "UseEmuleProtocol":
					e.Value=true;
					break;
				case "MaxServerFails":
					e.Value=(short)2;
					break;
				case "UserName":
					e.Value="http://www.lphant.com";
					break;
				case "AutoReconnect":
					e.Value=true;
					break;
				case "ICHEnabled":
					e.Value=true;
					break;
				case "MaxUploadRate":
					e.Value=(float)10;
					break;
				case "AutoExtendSearch":
					e.Value=true;
					break;
				case "ID":
					e.Value=(uint)0;
					break;
				case "MaxDownloadRate":
					e.Value=55.0F;
					break;
				case "MaxConnections":
					e.Value=(int)140;
					break;
				case "QueueLength":
					e.Value=(int)200;
					break;
				case "MaxSourcesPerFile":
					e.Value=(int)900;
					break;
				case "RemoteControlEnabled":
					e.Value=false;
					break;
				case "DownloadServerMet":
					e.Value=false;
					break;
				case "ServerMetUri":
					e.Value="http://www.esel-paradies.de/server/best/server.met";
					break;
				case "AllowViewShared":
					e.Value=Constants.AllowViewShared.Nobody;
					break;
				case "UserHash":
					byte[] myHash=new Byte[16];
					Random rnd=new Random();
					rnd.NextBytes(myHash);
					// identify as eMule?
					if (Preferences.GetBool("UseEmuleProtocol", true))
					{
						myHash[5]=14;
						myHash[14]=111;
					}
					e.Value=myHash;
					break;
				case "SharedFolders":
					string[] sharedFolders=new string[1];
					Directory.CreateDirectory(DllDirectory+"incoming");
					sharedFolders[0]=DllDirectory+"incoming";
					e.Value=sharedFolders;
					break;
				case "TempFolders":
					string[] tempFolders=new string[1];
					Directory.CreateDirectory(DllDirectory+"temp");
					tempFolders[0]=DllDirectory+"temp";
					e.Value=tempFolders;
					break;
				case "RemoteControlPort":
					Random rnd2=new Random();
					e.Value=rnd2.Next(6000,8000);
					break;
				case "RemotePassword":
					Random rnd3=new Random();
					byte[] bPass=new byte[10];
					rnd3.NextBytes(bPass);
					e.Value=CKernel.HashToString(bPass);
					break;
				case "Language":
					e.Value=CultureInfo.CurrentCulture.Name;
					break;
				case "OSigEnabled":
					e.Value=false;
					break;
				case "UpdateServerListFromClient":
					e.Value=false;
					break;
				case "UpdateServerListFromServer":
					e.Value=true;
					break;
				case "IPFilterEnabled":
					e.Value=false;
					break;
				case "ReceiveMessageFromFriendOnly":
					e.Value=false;
					break;
				case "CompressionMethod":
					e.Value="Zip";
					break;
				case "StartNextStoppedFile":
					e.Value=false;
					break;
			}
		}

		/// <summary>
		/// Event handler for preferences class if any property was changed.
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="e">changed property arguments</param>
		internal void OnPropertyChanged(object sender, PropertyEventArgs e)
		{
			if (e.Key=="MaxDownloadRate")
			{
				float upRate=Preferences.GetFloat("MaxUploadRate");
				float downRate=(float)e.NewValue;
				if (downRate<1.0F) e.NewValue=1.0F;

				if ((upRate*4.0F<downRate)&&(upRate<9))
				{
					e.NewValue = upRate*4.0F;
				}
				else
				{
					// do nothing
				}
			}

			if (e.Key=="MaxUploadRate")
			{
				float upRate=(float)e.NewValue;
				float downRate=Preferences.GetFloat("MaxDownloadRate");
				if (upRate<1.0F) e.NewValue=1.0F;

				if ((upRate*4.0F<downRate)&&(upRate<9))
				{
					Preferences.SetProperty("MaxDownloadRate", upRate*4.0F, false);
				}
				else
				{
					// do nothing
				}
			}

			if (e.Key=="RemotePassword")
			{
				System.Security.Cryptography.MD5 crypto=System.Security.Cryptography.MD5.Create();
				byte [] c=null;
				string clearPass=(string)e.NewValue;
				c=new byte[clearPass.Length];
				for (int i=0; i < clearPass.Length; i++)
				{
					c[i]=System.Convert.ToByte(clearPass[i]);
				}
				byte[] res=crypto.ComputeHash(c);
				e.NewValue=CKernel.HashToString(res);
			}
			if(e.Key=="Language") 
			{
				Globalization.Language = e.NewValue.ToString();
			}
			
			if (e.Key=="IPFilterEnabled")
			{
				bool enabled=(bool)e.NewValue;
				if (enabled)
				{
					IPFilter = new CIPFilter();
					IPFilter.LoadIPFilter(DllDirectory,"ipfilter.dat",128);
				}
				else
					IPFilter=null;
			}

			if (e.Key=="TCPPort")
			{
				m_PortChanged=true;
				if (e.NewValue!=e.OldValue)
				{
					CKernel.Listener.Stop();
					CKernel.Listener=new CListener((ushort)e.NewValue);
					CKernel.Listener.Start();
					CKernel.ServersList.ConnectToAnyServer();//first call disconnects
					CKernel.ServersList.ConnectToAnyServer();//secons call connects
				}
			}

			if (e.Key=="UDPPort")
			{
				if (e.NewValue!=e.OldValue)
				{
					CKernel.UDPListener.Close();
					CKernel.UDPListener=new CUDPListener();
					CKernel.UDPListener.Start();
				}
			}

			if (e.Key=="TempFolders")
			{
				ArrayList newFolders=new ArrayList((Array)e.NewValue);
				ArrayList oldFolders=new ArrayList((Array)e.OldValue);
				foreach (string oldfolder in (Array)e.OldValue)
				{
					if (!newFolders.Contains(oldfolder))
					{
						//remove files from folder
						CKernel.FilesList.RemoveIncompleteFolder(oldfolder);
					}
				}
				foreach (string newfolder in (Array)e.NewValue)
				{
					if (!oldFolders.Contains(newfolder))
					{
						//remove files from folderadd new temp folder
						CKernel.FilesList.AddNewIncompleteFolder(newfolder);
					}
				}
			}
			if (e.Key=="SharedFolders")
			{
				string sharedOld="";
				string sharedNew="";
				foreach (string oldfolder in (Array)e.OldValue)
				{
					sharedOld+=oldfolder;
				}
				foreach (string newfolder in (Array)e.NewValue)
				{
					sharedNew+=newfolder;
				}
				if (sharedOld!=sharedNew) CKernel.FilesList.RefreshSharedList();
			}
		}

		internal void OnPropertyLoaded(object sender, PropertyLoadedArgs e)
		{
			// get up & download rate from preferences
			float upRate=Preferences.GetFloat("MaxUploadRate");
			float downRate=Preferences.GetFloat("MaxDownloadRate");

			if ((upRate*4.0F<downRate)&&(upRate<9))
			{
				Preferences.SetProperty("MaxDownloadRate", upRate*4.0F, false);
			}

			// placeholder for later import of older config files
			/*if (e.Version=="0.1")
			{
			}*/
		}

		internal static void CloseKernel()
		{
			CategoriesList.Save();
			// save preferences
			if (m_PortChanged) 
				Preferences.SetProperty("ID",0);
			Preferences.SaveProperties();

			Listener.Stop();
			ConnectionsList.CloseAll();
			Preferences.SetProperty("AutoReconnect", false);
			ServersList.Save("");

			FilesList.CloseDownloads("",true);
			FilesList.SaveList("");

			Queue.Clean(true);

			if (ServersList.ActiveServer!=null) ServersList.ActiveServer.Disconnect();
			FilesList.CancelFilesHashing();
		}

		internal bool StartServer()
		{
			return Listener.Start();
		}

		internal void StopServer()
		{
			Listener.Stop();
		}

		internal int ConnectedClientsNumber()
		{
			return ConnectionsList.Count();
		}

		internal static string HashToString(byte[] Hash)
		{
			string strResult=BitConverter.ToString(Hash);
			strResult=strResult.Replace("-", "");
			return strResult;
		}

		internal static byte[] StringToHash(string strHash)
		{
			byte[] Hash;
			Hash=new byte[16];
			for (int i=0;i!=16;i++)
			{
				Hash[i]=Byte.Parse((string)strHash.Substring(i*2,2), NumberStyles.HexNumber);
			}
			return Hash;
		}

		internal static bool SameHash(ref byte[] Hash1, ref byte[] Hash2)
		{
			if ((Hash1==null)||(Hash2==null)) return false;

			if (Hash1.Length!=Hash2.Length) return false;

			return ((Hash1[0]==Hash2[0])&&
				(Hash1[1]==Hash2[1])&&
				(Hash1[2]==Hash2[2])&&
				(Hash1[3]==Hash2[3])&&
				(Hash1[4]==Hash2[4])&&
				(Hash1[5]==Hash2[5])&&
				(Hash1[6]==Hash2[6])&&
				(Hash1[7]==Hash2[7])&&
				(Hash1[8]==Hash2[8])&&
				(Hash1[9]==Hash2[9])&&
				(Hash1[10]==Hash2[10])&&
				(Hash1[11]==Hash2[11])&&
				(Hash1[12]==Hash2[12])&&
				(Hash1[13]==Hash2[13])&&
				(Hash1[14]==Hash2[14])&&
				(Hash1[15]==Hash2[15]));

			//return (BitConverter.ToUInt64(Hash1, 0)==BitConverter.ToUInt64(Hash2, 0))
			//	&& (BitConverter.ToUInt64(Hash1, 8)==BitConverter.ToUInt64(Hash2, 8));
		}

		internal static bool SameCrumb(ref byte[] Hash1, ref byte[] Hash2)
		{
			if ((Hash1==null)||(Hash2==null)) return false;

			if (Hash1.Length!=Hash2.Length) return false;

			return ((Hash1[0]==Hash2[0])&&
				(Hash1[1]==Hash2[1])&&
				(Hash1[2]==Hash2[2])&&
				(Hash1[3]==Hash2[3])&&
				(Hash1[4]==Hash2[4])&&
				(Hash1[5]==Hash2[5])&&
				(Hash1[6]==Hash2[6])&&
				(Hash1[7]==Hash2[7]));
		}

		internal static CClient GetClient(ArrayList list, uint ID,ushort port,uint serverIP,byte[] ClientHash)
		{
			CClient resClient=null;
			byte[] HashComp;
			bool isSame;

			lock (list.SyncRoot)
			{
				foreach (CClient Client in list)
				{
					HashComp=Client.UserHash;

					if ((ClientHash!=null)&&(HashComp!=null))
					{
/*						isSame=true;

						for (int i=0;i!=16;i++)
						{
							if (HashComp[i]!=ClientHash[i]) 
							{
								isSame=false;
								break;
							}
						}
*/
						isSame= ((HashComp[0]==ClientHash[0])&&
								(HashComp[1]==ClientHash[1])&&
								(HashComp[2]==ClientHash[2])&&
								(HashComp[3]==ClientHash[3])&&
								(HashComp[4]==ClientHash[4])&&
								(HashComp[5]==ClientHash[5])&&
								(HashComp[6]==ClientHash[6])&&
								(HashComp[7]==ClientHash[7])&&
								(HashComp[8]==ClientHash[8])&&
								(HashComp[9]==ClientHash[9])&&
								(HashComp[10]==ClientHash[10])&&
								(HashComp[11]==ClientHash[11])&&
								(HashComp[12]==ClientHash[12])&&
								(HashComp[13]==ClientHash[13])&&
								(HashComp[14]==ClientHash[14])&&
								(HashComp[15]==ClientHash[15]));
						if (isSame)
						{
							resClient=Client;
							break;
						}
						else
						{
							continue;
						}
					}

					if (ID>Protocol.LowIDLimit)
					{
						if (port>0)
						{
							if ((Client.UserID==ID)&&(Client.Port==port))
							{
								resClient=Client;
								break;
							}
						}
						else
						{
							if (Client.UserID==ID)
							{
								resClient=Client;
								break;
							}
						}
					}
					else
					{
						if ((Client.UserID==ID)&&(Client.Port==port)&&(Client.ServerIP==serverIP))
						{
							resClient=Client;
							break;
						}
					}
				}
			}
			return resClient;
		}

		internal void ConnectToServer(string strIP, ushort port)
		{
			if ((port==0)||(port>=IPEndPoint.MaxPort-4)) return;
			CServer server=new CServer(strIP,port);
			server.Connect();
		}

		internal void ConnectToNetwork()
		{
			ServersList.ConnectToAnyServer();
		}

		internal void SaveServerList()
		{
			ServersList.Save("");
		}

		internal void LoadSharedFiles()
		{
			FilesList.LoadSharedFiles();
		}

		internal static void NewSearch(int searchID, string searchString, bool matchAnyWords, string type, uint maxSize, uint minSize,uint avaibility, string exclude)
		{
			CKernel.LastSearch=searchID;
			if (ServersList.ActiveServer==null) 
			{
				SearchEnded(searchID);
				return;
			}
			CSearcher search=new CSearcher(searchString, matchAnyWords, type, maxSize, minSize, avaibility, exclude);
			Searchs.Add(searchID,search);
		}

		internal static void ExtendSearch(int searchID)
		{
			CSearcher searcher=(CSearcher)Searchs[searchID];
			for (int i=0;i<5;i++) if (searcher!=null) searcher.ExtendSearch();
			if (ServersList.ActiveServer!=null)
			{
				ServersList.ActiveServer.QueryMoreResults();
			}
		}

		internal static void DeleteSearch(int searchID)
		{
			CSearcher searcher=(CSearcher)Searchs[searchID];
			if (searcher!=null) searcher.CancelSearch();
			Searchs.Remove(searchID);
		}

		internal static string GetSoftwarebyID(uint ID)
		{
			switch(ID)
			{
				case (uint)Protocol.Client.eMule:
					return "eMule";
				case (uint)Protocol.Client.cDonkey:
					return "cDonkey";
				case (uint)Protocol.Client.lMule:
					return "lMule";
				case (uint)Protocol.Client.aMule:
					return "aMule";
				case (uint)Protocol.Client.eLePhant:
					return "lphant";
				case (uint)Protocol.Client.eDonkeyHybrid:
					return "eDonkeyHybrid";
				case (uint)Protocol.Client.eDonkey:
					return "eDonkey";
				case (uint)Protocol.Client.mlDonkey:
					return "mlDonkey";
				case (uint)Protocol.Client.Shareaza:
					return "Shareaza";
				case (uint)Protocol.Client.eMuleOld:
					return "old eMule";
				case (uint)Protocol.Client.Unknown:
					return "?????";
			}
			return "????:"+ID.ToString();
		}

		internal static void NewFile(CElement element)
		{
			InterfaceFile file=InterfaceGateway[0].FileToInterfaceFile(element);

			for (int i=0;i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].NewFile(file);
				}
			}
		}
		internal static void NewSharedFile(CElement element)
		{
			InterfaceFile file=InterfaceGateway[0].FileToInterfaceFile(element);

			for (int i=0;i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].NewSharedFile(file);
				}
			}
		}

		internal static void RemovedSharedFile(CElement element)
		{
			InterfaceFile file=InterfaceGateway[0].FileToInterfaceFile(element);

			for (int i=0;i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].RemovedSharedFile(file);
				}
			}
		}

		internal static void NewLogMessage(Constants.Log importance, string strMsg)
		{
			for (int i=0; i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].NewLogMessage(importance, strMsg);
				}
			}
		}

		internal static void SearchFileModified(CFileFound file, int SearchIndex)
		{
			InterfaceSearchedFile searchFile=new InterfaceSearchedFile();
			searchFile=InterfaceGateway[0].FileFountToInterfaceFileFound(file);

			for (int i=0;i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].SearchFileModified(searchFile, SearchIndex);
				}
			}
		}

		internal static void NewFileFound(CFileFound file, int SearchIndex)
		{
			InterfaceSearchedFile searchFile=new InterfaceSearchedFile();
			searchFile=InterfaceGateway[0].FileFountToInterfaceFileFound(file);

			for (int i=0; i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].NewFileFound(searchFile, SearchIndex);
				}
			}
		}

		internal static void SearchEnded(int searchID)
		{
			for (int i=0;i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].SearchEnded(searchID);
				}
			}
		}

		internal static void NewUpload(CClient client)
		{
			InterfaceClient interfaceClient=InterfaceGateway[0].ClientToInterfaceClient(client);

			for (int i=0;i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].NewUpload(interfaceClient);
				}
			}
		}

		internal static void NewSource(CClient client, string strDownFileHash)
		{
			InterfaceClient interfaceClient=InterfaceGateway[0].ClientToInterfaceClient(client);

			for (int i=0;i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].NewSource(interfaceClient,strDownFileHash);
				}
			}
		}

		internal static void NewServer(CServer server)
		{
			for (int i=0;i<=InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].SubmitOnNewServer(server);
				}
			}
		}

		internal static void DeleteServer(uint ip, ushort port)
		{
			for (int i=0;i<= InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].SubmitOnDeleteServer(ip,port);
				}
			}
		}
		internal static void NewSearchTab(int searchID,string tabName)
		{
			for (int i=0;i<= InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].SubmitOnNewSearchTab(searchID,tabName);
				}
			}
		}
		internal static void NewSearchProgress(int progressPerCent,int searchID)
		{
			for (int i=0;i<= InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].SubmitOnNewSearchProgress(progressPerCent,searchID);
				}
			}
		}
		internal static void NewChatMessage(CClient client,string message)
		{
			for (int i=0;i<= InterfaceGateway.Length-1;i++)
			{
				if (CKernel.InterfaceGateway[i]!=null)
				{
					InterfaceGateway[i].SubmitOnNewChatMessage(client, message);
				}
			}
		}
		internal static void OSig(string filefullpath) // Added by C00I90WN
		{
			if (CKernel.Preferences.GetBool("OSigEnabled"))
			{
				FileStream sigdata;
				StreamWriter writer;
				if (filefullpath.Length == 0) filefullpath = DllDirectory + "onlinesig.dat";
				try
				{
					sigdata=File.Open(filefullpath,FileMode.Create,FileAccess.Write,FileShare.None);
				}
				catch {return;}
				writer = new StreamWriter(sigdata);
			
				if(CKernel.ServersList.ActiveServer!=null)
				{   //If connected
					writer.Write("1|"); //Connected to a Server
					writer.Write(CKernel.ServersList.ActiveServer.Name); //Server Name
					writer.Write("|"); //End of Server Name
					writer.Write(CKernel.ServersList.ActiveServer.IP); //Server IP
					writer.Write("|"); //End of Server IP
					writer.Write(CKernel.ServersList.ActiveServer.Port); //Server Port
					writer.Write("|"); //End of Server Port
				}
				else // If not connected
					writer.Write(0);

				writer.Write("\n");
				writer.Write(CKernel.FilesList.GlobalDownSpeed); //Download Speed
				writer.Write("|"); //End of Download Speed
				writer.Write(CKernel.Queue.GetUploadSpeed()); //Upload Speed
				writer.Write("|"); //End of Upload Speed
				writer.Write(CKernel.Queue.Count);
			
				writer.Close();
			}
		}
		/*public static void CleanMemory()
		{
			Debug.Write(GC.GetTotalMemory(false));
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Debug.Write("-> "+GC.GetTotalMemory(true)+"\n");
		}*/
	}
}