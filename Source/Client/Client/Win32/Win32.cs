using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace eLePhant.Client
{
	public class Win32
	{
		#region Device context

		[DllImport("user32.dll")]
		public static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern IntPtr GetWindowDC(IntPtr handle);

		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hDC);

		[DllImport("Gdi32.dll",CharSet = CharSet.Auto)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		#endregion

		#region Send message prototypes

		[DllImport("User32.dll",CharSet = CharSet.Auto,SetLastError=true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref RECT r);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref HDHITTESTINFO hti);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref HDLAYOUT hdl);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref HDITEM hdi);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref LVCOLUMN lvc);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref DRAWITEMSTRUCT dis);

		[DllImport("user32.dll")]
		public static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, ref LV_COLUMN lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, int nulo);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, IntPtr lParam);

		[DllImport("comctl32.dll", CharSet=CharSet.Auto)]
		public static extern bool FlatSB_SetScrollProp(IntPtr hWnd, uint index, IntPtr pValue, bool fRedraw);
		
		#endregion

		#region Windows tasks	    
		public delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

		[DllImport("user32")]
        public extern static int EnumWindows(EnumWindowsProc lpEnumFunc,int lParam);
		
		[DllImport("user32")]
		public extern static int GetProp(IntPtr hwnd, string lpString);
		
		[DllImport("user32")]
        public extern static int SetProp(IntPtr hwnd, string lpString, int hData);

		[DllImport("User32",EntryPoint="FindWindow")]
		public static extern IntPtr FindWindow(string lpClassName,string lpWindowName);

		[DllImport("User32",EntryPoint="SetForegroundWindow")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		#endregion

		#region Subclassing P/Invokes

		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern int CallWindowProc(IntPtr wndProc, IntPtr hwnd, 
			int msg, IntPtr wparam,	IntPtr lparam);

		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern int GetWindowThreadProcessId(IntPtr hwnd, int ID);

		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hwnd, char[] className, int maxCount);

		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern int CallNextHookEx(IntPtr hookHandle, 
								int code, IntPtr wparam, ref CWPSTRUCT cwp);
		
		[DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern int SetWindowPos(IntPtr hwnd, IntPtr hwndAfter, int x, int y, 
			int cx, int cy, uint flags);

		#endregion

		#region Paint related functions

        [DllImport("User32.dll",CharSet = CharSet.Auto)]
		public static extern int GetUpdateRect(IntPtr hwnd, ref RECT rect, bool erase);

		[DllImport("User32.dll",CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool GetWindowRect(IntPtr handle, ref RECT r);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

		#endregion

		#region Structs
		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO 
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};
		[StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
		{
            public int dwData;
            public int cbData;
            public int lpData;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CWPSTRUCT
		{
			public IntPtr lparam;
			public IntPtr wparam;
			public int message;
			public IntPtr hwnd;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS
		{
			public IntPtr hwnd;
			public IntPtr hwndAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public uint flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NCCALCSIZE_PARAMS
		{
			public RECT rgc;
			public WINDOWPOS wndpos;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NMHDR
		{
			public IntPtr hwndFrom;
			public int idFrom;
			public int code;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NMHEADER
		{
			public NMHDR nhdr;
			public int iItem;
			public int iButton;
			public IntPtr pHDITEM;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PAINTSTRUCT
		{
			public IntPtr hdc;
			public int fErase;
			public RECT rcPaint;
			public int fRestore;
			public int fIncUpdate;
			public int Reserved1;
			public int Reserved2;
			public int Reserved3;
			public int Reserved4;
			public int Reserved5;
			public int Reserved6;
			public int Reserved7;
			public int Reserved8;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HDHITTESTINFO 
		{  
			public Point pt;  
			public uint flags; 
			public int iItem; 
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HDLAYOUT
		{
			public IntPtr prc;//RECT*
			public IntPtr pwpos;//WINDOWPOS*
		}

		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
		public struct HDITEM 
		{
			public int    mask; 
			public int     cxy; 
			public string  pszText; 
			public IntPtr  hbm; 
			public int     cchTextMax; 
			public int     fmt; 
			public int     lParam; 
			public int     iImage;
			public int     iOrder;
			public uint    type;
			public IntPtr  pvFilter;
		}

		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
		public struct DRAWITEMSTRUCT
		{
			public int ctrlType;
			public int ctrlID;
			public int itemID;
			public int itemAction;
			public int itemState;
			public IntPtr hwnd;
			public IntPtr hdc;
			public RECT rcItem;
			public IntPtr itemData;
		}

		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
		public struct LVCOLUMN
		{
			public int mask;
			public int fmt;
			public int cx;
			public string text;
			public int textMax;
			public int subItem;
			public int iImage;
			public int iOrder;
		}

		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
		public struct NMCUSTOMDRAW
		{
			public NMHDR hdr;
			public int dwDrawStage;
			public IntPtr hdc;
			public RECT rc;
			public int dwItemSpec;
			public int uItemState;
			public int lItemlParam;
		}

		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
		public struct NMLVCUSTOMDRAW 
		{
			public NMCUSTOMDRAW nmcd;
			public uint clrText;
			public uint clrTextBk;
			public int iSubItem;
		} 
		
		[StructLayoutAttribute(LayoutKind.Sequential)]
		public struct LV_COLUMN
		{
			public UInt32 mask;
			public Int32 fmt;
			public Int32 cx;
			public String pszText;
			public Int32 cchTextMax;
			public Int32 iSubItem;
			public Int32 iImage;
			public Int32 iOrder;
		} 
		#endregion

		#region Enums & constants

		public enum HDI : int//Header item
		{
			HDI_WIDTH             =  0x0001,
			HDI_HEIGHT            =  HDI_WIDTH,
			HDI_TEXT              =  0x0002,
			HDI_FORMAT            =  0x0004,
			HDI_LPARAM            =  0x0008,
			HDI_BITMAP            =  0x0010,
			HDI_IMAGE             =  0x0020,
			HDI_DI_SETITEM        =  0x0040,
			HDI_ORDER             =  0x0080,
			HDI_FILTER            =  0x0100
		}

		public enum HDF : int//Header format
		{
			HDF_LEFT             =   0x0000,
			HDF_RIGHT            =   0x0001,
			HDF_CENTER           =   0x0002,
			HDF_JUSTIFYMASK      =   0x0003,
			HDF_RTLREADING       =   0x0004,
			HDF_OWNERDRAW        =   0x8000,
			HDF_STRING           =   0x4000,
			HDF_BITMAP           =   0x2000,
			HDF_BITMAP_ON_RIGHT  =   0x1000,
			HDF_IMAGE            =   0x0800
		}

		public enum NM : int//Notification messages
		{
			NM_FIRST = (0-  0),
			NM_CUSTOMDRAW = (NM_FIRST-12),
			NM_CLICK =(NM_FIRST-2),
			NM_RCLICK =(NM_FIRST-5)
		}

		public enum CDRF : int//Custom draw return flags
		{
			CDRF_DODEFAULT		 = 0x00000000,
			CDRF_NEWFONT		 = 0x00000002,
			CDRF_SKIPDEFAULT	 = 0x00000004,
			CDRF_NOTIFYPOSTPAINT = 0x00000010,
			CDRF_NOTIFYITEMDRAW  = 0x00000020
		}

		public enum CDDS : int//Custom draw draw state
		{
			CDDS_PREPAINT           = 0x00000001,
			CDDS_POSTPAINT          = 0x00000002,
			CDDS_PREERASE           = 0x00000003,
			CDDS_POSTERASE          = 0x00000004,
			CDDS_ITEM               = 0x00010000,
			CDDS_ITEMPREPAINT       = (CDDS_ITEM | CDDS_PREPAINT),
			CDDS_ITEMPOSTPAINT      = (CDDS_ITEM | CDDS_POSTPAINT),
			CDDS_ITEMPREERASE       = (CDDS_ITEM | CDDS_PREERASE),
			CDDS_ITEMPOSTERASE      = (CDDS_ITEM | CDDS_POSTERASE),
			CDDS_SUBITEM            = 0x00020000
		}

		public enum CDIS : int//Custom draw item state
		{
			CDIS_SELECTED  = 0x0201,
			CDIS_DEFAULT   = 0x0200
		}

		public enum OCM : int//Reflected messages
		{
			OCM__BASE				      = 0x0400 + 0x1c00,
			OCM_COMMAND				    = ( OCM__BASE + 0x0111),
			OCM_CTLCOLORBTN			  = ( OCM__BASE + 0x0135 ),
			OCM_CTLCOLOREDIT		  = ( OCM__BASE + 0x0133 ),
			OCM_CTLCOLORDLG			  = ( OCM__BASE + 0x0136 ),
			OCM_CTLCOLORLISTBOX		= ( OCM__BASE + 0x0134 ),
			OCM_CTLCOLORMSGBOX		= ( OCM__BASE + 0x0132 ),
			OCM_CTLCOLORSCROLLBAR = ( OCM__BASE + 0x0137 ),
			OCM_CTLCOLORSTATIC		= ( OCM__BASE + 0x0138 ),
			OCM_CTLCOLOR		    	= ( OCM__BASE + 0x0019 ),
			OCM_DRAWITEM			    = ( OCM__BASE + 0x002B ),
			OCM_MEASUREITEM			  = ( OCM__BASE + 0x002C ),
			OCM_DELETEITEM        = ( OCM__BASE + 0x002D ),
			OCM_VKEYTOITEM        = ( OCM__BASE + 0x002E ),
			OCM_CHARTOITEM        = ( OCM__BASE + 0x002F ),
			OCM_COMPAREITEM       = ( OCM__BASE + 0x0039 ),
			OCM_HSCROLL           = ( OCM__BASE + 0x0114 ),
			OCM_VSCROLL           = ( OCM__BASE + 0x0115 ),
			OCM_PARENTNOTIFY      = ( OCM__BASE + 0x0210 ),
			OCM_NOTIFY            = ( OCM__BASE + 0x004E )
		}

		public const uint SHGFI_ICON = 0x100;
		public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
		public const uint SHGFI_SMALLICON = 0x1; // 'Small icon
		public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
		public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
		public const uint SHGFI_SYSICONINDEX = 0x000004000;
		#endregion

		#region RGB Windows Macros

		static public byte GetRValue(uint color)
		{
			return (byte)color;
		}

		static public byte GetGValue(uint color)
		{
			return ((byte)(((short)(color)) >> 8));
		}

		static public byte GetBValue(uint color)
		{
			return ((byte)((color)>>16));
		}

		static public uint RGB(int r, int g, int b)
		{
			return ((uint)(((byte)(r)|((short)((byte)(g))<<8))|(((short)(byte)(b))<<16)));

		}

		#endregion

		#region functions
		public static IntPtr MakeLParam(int LoWord, int HiWord) 
		{ 
			return (IntPtr) ((HiWord << 16) | (LoWord & 0xffff)); 
		} 

		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
		 
		#endregion

		#region Messages
		public const int WM_USER=0x0400;
		public const int WM_COPYDATA=0x004A;
		public const int EM_LINESCROLL=0x00B6;
		public const int WM_VSCROLL=0x0115;
		
		public const int SB_BOTTOM=7;
		public const int SB_LINEDOWN=1;
		
		public const Int32 LVM_FIRST=0x1000;
		public const Int32 LVM_GETCOLUMN=LVM_FIRST+95;
		public const Int32 LVM_SETCOLUMN=LVM_FIRST+96;
		public const Int32 LVM_GETCOLUMNWIDTH = LVM_FIRST+29;
		public const Int32 LVM_SETCOLUMNWIDTH = LVM_FIRST+30;
		public const Int32 LVCF_ORDER = 0x0020;
		#endregion

	}
}
