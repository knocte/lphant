using System;
using System.Text.RegularExpressions;
//using System.IO;
namespace eLePhant.Client
{
	/// <summary>
	/// Summary description for NameCleaner.
	/// </summary>
	public class CNameCleaner
	{
		
		
		public CNameCleaner()
		{
		}
		public static string Clean(string fileName)
		{
			try
			{
				string org=fileName;
				fileName=Regex.Replace(fileName, @"[_\.-]"," ");
				Regex r;
				do
				{
				r = new Regex(@"(?<name1>.*)(\[|DVD|VHS|KVCD|xvid|CD1|CD2|Screener|Spanish|German|French|Español|italian|divx|CVCD|.*RIP|\()"
					,RegexOptions.Compiled|RegexOptions.IgnoreCase);
				if (r.IsMatch(fileName))
					fileName=r.Match(fileName).Result("${name1}");
				}
				while (r.IsMatch(fileName));
				fileName=Regex.Replace(fileName, @"[^\w]"," ");
				if (fileName.Length==0) fileName=org;
			}
			catch{}
			return fileName.Trim();
		}

	}
}
