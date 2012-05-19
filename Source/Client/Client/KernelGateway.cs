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
using eLePhant.eDonkey;
using eLePhant.Types;

using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Lifetime;
using System.Diagnostics;
using System.Collections;

namespace eLePhant.Client
{
	public delegate void RefreshEvent(CkernelGateway kernelGateway);

	[Serializable]
	public class CkernelGateway : MarshalByRefObject
	{
		public event LogEvent OnLogMessage;
		public event FileEvent OnNewFile;
		public event FileEvent OnNewSharedFile;
		public event FileEvent OnRemovedSharedFile;
		public event ClientEvent OnNewUpload;
		public event SourceEvent OnNewSource;
		public event RefreshEvent OnRefreshDownloads;
		public event ServerEvent OnNewServer;
		public event RefreshEvent OnRefreshUploads;
		public event RefreshEvent OnRefreshSources;
		public event RefreshEvent OnRefreshFileNames;
		public event RefreshEvent OnRefreshFileComments;
		public event RefreshEvent OnRefreshStatus;
		public event RefreshEvent OnRefreshServers;
		public event RefreshEvent OnRefreshShared;
		public event SearchEvent OnNewSearched;
		public event SearchEvent OnModifiedSearched;
		public event IPEvent OnDeleteServer;
		public event EndSearchEvent OnEndSearch;
		public event SearchTabEvent OnNewSearchTab;
		public event SearchProgressEvent OnSearchProgress;
		public event SourceEvent OnNewChatMessage;
		
		public event ClientEvent OnStartChatSession;
		public event ClientEvent OnAddingFriend;
		public event ClientEvent OnDeleteFriend;

		private Thread RefreshThread;

		private bool m_isLocal;
		private bool m_NotRefresh;

		private CKernel edonkeyNucleo;
		private CInterfaceGateway krnGateway;

		public Hashtable FilterSummary;
		public string CurrentCategoryFilter;
		public event EventHandler OnSummaryFilterChanged;
		public event EventHandler OnCategoryListChanged;

		public bool IsLocal
		{
			get { return m_isLocal; }
		}

		public bool NotRefresh
		{
			get { return m_NotRefresh; }
			set { m_NotRefresh=value; }
		}

		public CkernelGateway()
		{
			m_isLocal=true;
			edonkeyNucleo=new CKernel();
			krnGateway=CKernel.InterfaceGateway[0];
			Initialize();
		}

		public CkernelGateway(CInterfaceGateway in_krnGateway)
		{
			m_isLocal=false;
			krnGateway=in_krnGateway;
			Initialize();
		}

		private void ResetFiterSummary()
		{
			FilterSummary.Clear();

			CFilterSummary FilerResumeItemAll=new CFilterSummary();
			FilerResumeItemAll.Items=0;
			FilerResumeItemAll.TotalSize=0;
			FilterSummary["All"]=FilerResumeItemAll;
			
			CFilterSummary FilerResumeItemStopped=new CFilterSummary();
			FilerResumeItemStopped.Items=0;
			FilerResumeItemStopped.TotalSize=0;			
			FilterSummary["Stopped"]=FilerResumeItemStopped;
			
			CFilterSummary FilerResumeItemStarted=new CFilterSummary();
			FilerResumeItemStarted.Items=0;
			FilerResumeItemStarted.TotalSize=0;
			FilterSummary["Started"]=FilerResumeItemStarted;
		}

		private void CalculateFilterSummary(InterfaceFile[] files)
		{
			ResetFiterSummary();
			string category;
			int i=0;
			bool isCategoryFiltered;
			InterfaceFile file;
			while (i<files.Length)
			{
				file=files[i];
				i++;
				if (file==null) continue;
				category="";
				switch(file.Status)
				{
					case 0:
					case 8:
					case 9:
						category="Started";
						break;
					case 1:
					case 2:
						category="Stopped";
						break;
				}
				CFilterSummary filterSummary;

				isCategoryFiltered=false;
				if (CurrentCategoryFilter=="LBL_VIDEO")
					isCategoryFiltered=(!CUtils.IsVideo(file.Name));
				else if (CurrentCategoryFilter=="LBL_AUDIO")
					isCategoryFiltered=(!CUtils.IsAudio(file.Name));
				else if (CurrentCategoryFilter=="LBL_FILE")
					isCategoryFiltered=(!CUtils.IsFile(file.Name));
				else isCategoryFiltered=((CurrentCategoryFilter.Length!=0)&&(CurrentCategoryFilter!=file.Category));

				if (!isCategoryFiltered)
				{
					if (category.Length>0)
					{
						filterSummary=((CFilterSummary)FilterSummary[category]);
						filterSummary.TotalSize+=file.Size/1024/1024;
						filterSummary.Items++;
					}

					filterSummary=((CFilterSummary)FilterSummary["All"]);
					filterSummary.TotalSize+=file.Size/1024/1024;
					filterSummary.Items++;
				}
			}
			if (OnSummaryFilterChanged!=null) OnSummaryFilterChanged(this,null);
		}

		private void Initialize()
		{
			FilterSummary=new Hashtable();
			CurrentCategoryFilter="";
			ResetFiterSummary();

			krnGateway.OnLogMessage+=new LogEvent(m_OnLogMessage);
			krnGateway.OnNewFile+=new FileEvent(m_OnNewFile);
			krnGateway.OnNewSharedFile+=new FileEvent(m_OnNewSharedFile);
			krnGateway.OnRemovedSharedFile+=new FileEvent(m_OnRemovedSharedFile);
			krnGateway.OnNewUpload+=new ClientEvent(m_OnNewUpload);
			krnGateway.OnNewSource+=new SourceEvent(m_OnNewSource);
			krnGateway.OnNewServer+=new ServerEvent(m_OnNewServer);
			krnGateway.OnDeleteServer+=new IPEvent(m_OnDeletedServer);
			krnGateway.OnNewSearched+=new SearchEvent(m_OnNewSearched);
			krnGateway.OnModifiedSearched+=new SearchEvent(m_OnModifiedSearched);
			krnGateway.OnEndSearch+=new EndSearchEvent(m_OnEndSearch);
			krnGateway.OnNewSearchTab+=new SearchTabEvent(m_OnNewSearchTab);
			krnGateway.OnNewSearchProgress+=new SearchProgressEvent(m_OnSearchProgress);
			krnGateway.OnNewChatMessage+=new SourceEvent(m_OnNewChatMessage);

			RefreshThread=new Thread(new ThreadStart(RefreshProc));
			RefreshThread.Name="Interface refresher";
			m_NotRefresh=false;
			RefreshThread.Start();
			
			if (m_isLocal)
				krnGateway.StartKernel();
		}

		public void CloseKernel()
		{
			krnGateway.OnLogMessage-=new LogEvent(m_OnLogMessage);
			krnGateway.OnNewFile-=new FileEvent(m_OnNewFile);
			krnGateway.OnNewSharedFile-=new FileEvent(m_OnNewSharedFile);
			krnGateway.OnRemovedSharedFile-=new FileEvent(m_OnRemovedSharedFile);
			krnGateway.OnNewUpload-=new ClientEvent(m_OnNewUpload);
			krnGateway.OnNewSource-=new SourceEvent(m_OnNewSource);
			krnGateway.OnNewServer-=new ServerEvent(m_OnNewServer);	
			krnGateway.OnDeleteServer-=new IPEvent(m_OnDeletedServer);
			krnGateway.OnNewSearched-=new SearchEvent(m_OnNewSearched);
			krnGateway.OnModifiedSearched-=new SearchEvent(m_OnModifiedSearched);
			krnGateway.OnEndSearch-=new EndSearchEvent(m_OnEndSearch);
			krnGateway.OnNewSearchTab-=new SearchTabEvent(m_OnNewSearchTab);
			krnGateway.OnNewSearchProgress-=new SearchProgressEvent(m_OnSearchProgress);
			krnGateway.OnNewChatMessage-=new SourceEvent(m_OnNewChatMessage);
			RefreshThread.Abort();
			if (m_isLocal)
			{
				krnGateway.CloseKernel();
			}
		}

		public string Version
		{
			get { return krnGateway.Version; }
		}

		//methods
		public void StopRefresh()
		{
			RefreshThread.Abort();
		}

		public void StartRefresh()
		{
			RefreshThread.Resume();
		}

		public InterfaceFile GetDownloadFile(string hash)
		{
			return krnGateway.GetDownloadFile(hash);
		}

		public void CalculateFilterSummary()
		{
			InterfaceFile[] files=krnGateway.GetDownloadFileList();
			CalculateFilterSummary(files);
		}

		public InterfaceFile[] GetDownloadFileList()
		{
			InterfaceFile[] files=krnGateway.GetDownloadFileList();
			CalculateFilterSummary(files);
			return files;
		}

		public SFriend[] GetFriends()
		{
			return krnGateway.GetFriends();
		}

		public void ResumeFile(string strFileHash)
		{
			krnGateway.ResumeFile(strFileHash);
			InterfaceFile[] files=krnGateway.GetDownloadFileList();
			CalculateFilterSummary(files);
		}

		public void StopFile(string strFileHash)
		{
			krnGateway.StopFile(strFileHash);
			InterfaceFile[] files=krnGateway.GetDownloadFileList();
			CalculateFilterSummary(files);
		}

		public void CancelFile(string strFileHash)
		{
			krnGateway.CancelFile(strFileHash);
			InterfaceFile[] files=krnGateway.GetDownloadFileList();
			CalculateFilterSummary(files);
		}

		public InterfaceClient GetUploadClient(uint ID, ushort Puerto,uint IPServidor, byte[] HashCliente)
		{
			return krnGateway.GetUploadClient(ID, Puerto, IPServidor, HashCliente);
		}

		public InterfaceClient GetDownloadClient(uint IP, ushort Puerto,uint IPServidor,byte[] HashCliente,string strHashFicheroDescarga)
		{
			return krnGateway.GetDownloadClient(IP,Puerto,IPServidor,HashCliente,strHashFicheroDescarga);
		}

		public InterfaceClient[] GetFileSources(string strHashFichero)
		{
			return krnGateway.GetFileSources(strHashFichero);
		}

		public InterfaceServer[] GetServerList()
		{
			return krnGateway.GetServerList();
		}

		public InterfaceClient[] GetQueuedSourcesList(byte[] fileHashFilter)
		{
			return krnGateway.GetQueuedSourcesList(fileHashFilter);
		}

		public InterfaceFileNames[] GetFileNamesList(string strFileHash)
		{
			return krnGateway.GetFileNamesList(strFileHash);
		}

		public InterfaceComment[] GetComments(string strFileHash)
		{
			return krnGateway.GetComments(strFileHash);
		}

		public void SetFileComment(string strFileHash, string strComment, byte rating)
		{
			krnGateway.SetFileComment(strFileHash, strComment, rating);
		}

		public void GetFileComment(string strFileHash, ref string strComment, ref byte rating)
		{	
			krnGateway.GetFileComment(strFileHash, ref strComment, ref rating);
		}

		public void SetFileName(string strFileHash,string newName)
		{
			krnGateway.SetFileName(strFileHash,newName);
		}

		public FileStatistics GetFileStatistics(string strFileHash)
		{
			return krnGateway.GetFileStatistics(strFileHash);
		}

		public sFileDetails GetFileDetails(string strFileHash)
		{
			return krnGateway.GetFileDetails(strFileHash);
		}

		public void Search(int searchID, string condicion, bool matchAnyWords,string type,uint maxSize, uint minSize,uint avaibility, string exclude)
		{
			krnGateway.Search(searchID, condicion, matchAnyWords, type, maxSize, minSize,avaibility, exclude);
		}

		public void CancelSearch(int SearchIndex)
		{
			krnGateway.CancelSearch(SearchIndex);
		}

		public void DeleteSearch(int SearchIndex)
		{
			krnGateway.DeleteSearch(SearchIndex);
		}

		public void ExtendSearch(int SearchIndex)
		{
			krnGateway.ExtendSearch(SearchIndex);
		}

		public void DownloadFile(InterfaceSearchedFile fichero,int SearchIndex)
		{
			krnGateway.DownloadFile(fichero,SearchIndex);
			InterfaceFile[] files=krnGateway.GetDownloadFileList();
			CalculateFilterSummary(files);
		}

		public GlobalStatus GetGlobalStatus()
		{
			return krnGateway.GetGlobalStatus();
		}

		public void ConnectToaServer()
		{
			krnGateway.ConnectToaServer();
		}

		public void ConnectToServer(uint ip, ushort port)
		{
			krnGateway.ConnectToServer(ip,port);
		}

		public void DeleteServer(uint ip, ushort port)
		{
			krnGateway.DeleteServer(ip,port);
		}

		public void SetServerPriority(uint ip, ushort port,Constants.ServerPriority priority)
		{
			krnGateway.SetServerPriority(ip,port,priority);
		}

		public InterfacePreferences GetConfig()
		{
			return krnGateway.GetConfig();
		}

		public void SetConfig(InterfacePreferences config)
		{
			krnGateway.SetConfig(config);
		}

		public void DownloadElink(string elink, bool stopped)
		{
			krnGateway.DownloadElink(elink,stopped);
			InterfaceFile[] files=krnGateway.GetDownloadFileList();
			CalculateFilterSummary(files);
		}

		public void DownloadServerList(string uri)
		{
			krnGateway.DownloadServerList(uri);
		}

		public void RefreshSharedList()
		{
			krnGateway.RefreshSharedList();
		}

		public InterfaceFile[] GetSharedFileList()
		{
			return krnGateway.GetSharedFileList();
		}

		public void SetFileUploadPrioriry(string strFileHash, Types.Constants.Priority priority)
		{
			krnGateway.SetFileUploadPrioriry(strFileHash,priority);
		}

		public void SetFileDownloadPrioriry(string strFileHash, Types.Constants.Priority priority)
		{
			krnGateway.SetFileDownloadPrioriry(strFileHash,priority);
		}

		public void RequestSharedList(uint ID, ushort Port,uint ServerIP, byte[] ClientHash)
		{
			krnGateway.RequestSharedList(ID,Port,ServerIP,ClientHash);
		}

		public void SendChatMessage(uint ID, ushort Port,uint ServerIP, byte[] ClientHash, string message)
		{
			krnGateway.SendChatMessage(ID, Port, ServerIP, ClientHash, message);
		}

		public void StartChatSession(InterfaceClient source)
		{
			if (OnStartChatSession!=null) OnStartChatSession(source);
		}

		public void AddFriend(InterfaceClient source)
		{
			krnGateway.AddFriend(source.ID, source.Port, source.ServerIP, source.UserHash, 0, "", source.Name, source.Software, source.Version,false);
			OnAddingFriend(source);
		}
		
		public void AddFriend(SFriend friend)
		{
			krnGateway.AddFriend(friend.ID, friend.Port, friend.ServerIP, friend.UserHash, 0, friend.OurName, friend.Name, friend.Software, friend.Version, friend.FriendSlot);
			InterfaceClient source = new InterfaceClient();
			source.Name=friend.OurName;
			source.ID=friend.ID;
			source.Port=friend.Port;
			if (friend.UserHash!=null) source.UserHash=friend.UserHash;
			OnAddingFriend(source);
		}

		public void ChangeFriendSlotAssigned(uint ID, byte[] ClientHash, ushort Port, bool assigned)
		{
			krnGateway.ChangeFriendSlotAssigned(ID,ClientHash, Port, assigned);
		}

		public void DeleteFriend(byte[] ClientHash,uint ID, ushort Port)
		{
			krnGateway.DeleteFriend(ClientHash, ID, Port);
			InterfaceClient source = new InterfaceClient();
			source.ID=ID;
			source.UserHash=ClientHash;
			source.Port=Port;
			OnDeleteFriend(source);
		}

		public ArrayList GetCategories()
		{
			return krnGateway.GetCategories();
		}

		public bool AddorUpdateCategory(uint ID, string name, string auto, uint color)
		{
			bool res=krnGateway.AddorUpdateCategory(ID,name,auto,color);
			if (OnCategoryListChanged!=null) OnCategoryListChanged(this,null);
			return res;
		}

		public bool RemoveCategory(uint ID)
		{
			bool res=krnGateway.RemoveCategory(ID);
			if (OnCategoryListChanged!=null) OnCategoryListChanged(this,null);
			return res;
		}

		public void SetFileCategory(string strFileHash,string category)
		{
			krnGateway.SetFileCategory(strFileHash,category);
		}

		public CCategory GetCategoryByName(string name)
		{
			return krnGateway.GetCategoryByName(name);
		}

		public void DeleteFile(string strFileHash)
		{
			krnGateway.DeleteFile(strFileHash);
		}
		
		public void SetFileMaxSources(string strFileHash, uint maxSources)
		{
			krnGateway.SetFileMaxSources(strFileHash,maxSources);
		}

		//Refresh thread
		private void RefreshProc()
		{
			int i=0;
			int refreshMinimized=0; //refresh each 1200 seconds even minimized
			while (RefreshThread.IsAlive)
			{ 
				Thread.Sleep(1000);
				if (i>60) i=60;
				else i++;
				refreshMinimized++;
				if (OnRefreshStatus!=null) OnRefreshStatus(this);
				if ((m_NotRefresh)&&(refreshMinimized<1200)) continue;
				refreshMinimized=0;
				// monitor.Enter(s);
				try
				{
					if (OnRefreshDownloads!=null) OnRefreshDownloads(this);
					if (OnRefreshUploads!=null) OnRefreshUploads(this);
					if (OnRefreshSources!=null) OnRefreshSources(this);
					if (i % 10==0)
					{
						if (OnRefreshFileComments!=null) OnRefreshFileComments(this);
						if (OnRefreshFileNames!=null) OnRefreshFileNames(this);
					}
					if (i % 30==0)
					{
						if (OnRefreshShared!=null) OnRefreshShared(this);
					}
					if (i % 60==0)
					{
						if (OnRefreshServers!=null) OnRefreshServers(this);
						i=0;
					}
				}
				catch(Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
			}
			Debug.WriteLine("Thread finalizado");
		}

		//events
		public void m_OnLogMessage(Constants.Log importance,string strMsg)
		{
			if (OnLogMessage!=null) OnLogMessage(importance,strMsg);
		}

		public void m_OnNewFile(InterfaceFile file)
		{
			if (OnNewFile!=null) OnNewFile(file);
		}

		public void m_OnNewSharedFile(InterfaceFile file)
		{
			if (OnNewSharedFile!=null) OnNewSharedFile(file);
		}

		public void m_OnRemovedSharedFile(InterfaceFile file)
		{
			if (OnRemovedSharedFile!=null) OnRemovedSharedFile(file);
		}

		public void m_OnNewUpload(InterfaceClient client)
		{
			if (OnNewUpload!=null) OnNewUpload(client);
		}

		public void m_OnNewSource(InterfaceClient client,string strFileHash)
		{
			if (OnNewSource!=null) OnNewSource(client,strFileHash);
		}

		public void m_OnNewServer(InterfaceServer server)
		{
			if (OnNewServer!=null) OnNewServer(server);
		}

		public void m_OnDeletedServer(uint ip, ushort port)
		{
			if (OnDeleteServer!=null) OnDeleteServer(ip,port);
		}

		public void m_OnNewSearched(InterfaceSearchedFile file, int searchID)
		{
			if (OnNewSearched!=null) OnNewSearched(file,searchID);
		}

		public void m_OnModifiedSearched(InterfaceSearchedFile file, int searchID)
		{
			if (OnModifiedSearched!=null) OnModifiedSearched(file,searchID);
		}

		public void m_OnEndSearch(int searchID)
		{
			if (OnEndSearch!=null) OnEndSearch(searchID);
		}

		public void m_OnNewSearchTab(int searchID, string tabName)
		{
			if (OnNewSearchTab!=null) OnNewSearchTab(searchID,tabName);
		}

		public void m_OnSearchProgress(int progress, int searchID)
		{
			if (OnSearchProgress!=null) OnSearchProgress(progress,searchID);
		}

		public void m_OnNewChatMessage(InterfaceClient source, string message)
		{
			if (OnNewChatMessage!=null) OnNewChatMessage(source, message);
		}

		public override Object InitializeLifetimeService()
		{
			return null;
		}

		/*public override Object InitializeLifetimeService()
		{
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromSeconds(1);
				lease.SponsorshipTimeout = TimeSpan.FromSeconds(5);
				lease.RenewOnCallTime = TimeSpan.FromSeconds(2);
			}
			return lease;
		}*/
	}
}
