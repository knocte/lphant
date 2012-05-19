using System;

namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for Types.
	/// </summary>
	public class InterfaceConstants
	{
#if DEBUG		
		public const string GUID="{lphant-A587881B-3091-4770-A6F7-C182B37DA26C}";
#else
		public const string GUID="{lphant-DD14EC11-CB90-4956-B8F4-F5D6D708DC33}";
#endif
	}
}
