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
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace eLePhant.Classes
{
	/// <summary>
	/// Globalization loads language strings from XML-Files.
	/// </summary>
	public class Globalization
	{
		private string _path;
		private string _prefix;
		private string _extension;

		private string _sysCulture;
		private string _currentCulture;

		private SortedList _dictionary = new SortedList();
		private SortedList _languageFiles = new SortedList();

		public Globalization(string path, string prefix, string extension, string sysCulture)
		{
			_path = path;
			_prefix = prefix;
			_extension = extension;

			// get current system culture
			if(sysCulture == null)
				_sysCulture = CultureInfo.CurrentCulture.Name;
			else
				_sysCulture = sysCulture;

			// languagecode - regioncode
			if (_sysCulture.Length > 2)
			{
				string [] split = null;
				split = _sysCulture.Split( "-".ToCharArray() );
			}

			// get all language files which match the pattern
			GetLanguageFiles();
			LoadLanguage(_sysCulture);
		}

		private void LoadLanguage(string language)
		{
			//if language is not specified system language is selected
			if (language.Length==0)
				language=_sysCulture;

			//if the language is not found english is selected
			if (!_languageFiles.Contains(language)) 
			{
				string [] splitedLanguage=null; 
				splitedLanguage=_sysCulture.Split("-".ToCharArray());
				string compatibleLanguage=splitedLanguage[0]+"-"+splitedLanguage[0].ToUpper();
				if (_languageFiles.Contains(compatibleLanguage))
					language=compatibleLanguage;
				else
					language="en-US";
			}

			string file = _languageFiles[language].ToString();

			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(Path.Combine(_path, file));

				while ( reader.Read() )
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.LocalName.Equals("entry"))
						{
							if (!_dictionary.Contains(reader["key"]))
								try
								{
									_dictionary.Add(reader["key"], reader.ReadString());
								}
								catch(Exception e)
								{
									Debug.WriteLine(e.ToString());
								}
						}
					}
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
			finally
			{
				if (reader != null)
					reader.Close();
				_currentCulture = language;
			}
		}

		public string this[ string key ]
		{
			get
			{
				try
				{
					if (key.IndexOf("_")<0 || key.Length>40) return key;
					string result=(string)_dictionary[key];
					//if the current language does not contain the key try to load the string in english language
					if ((result==null)&&(_currentCulture!="en-US")) 
					{
						string lastCulture=_currentCulture;
						Language="en-US";
						result=(string)_dictionary[key];
						Language=lastCulture;
					}
					if (result==null) result=key;
					return(result);
				}
				catch
				{
					return "";
				}

			}
		}

		public string Language
		{
			get
			{
				return( _currentCulture );
			}
			set
			{
				// clear the dictionary
				_dictionary.Clear();
				// load the new language
				this.LoadLanguage(value);
			}
		}

		public string[] Languages
		{
			get
			{
				string[] result = new string[_languageFiles.Count];
				IList langKeys = _languageFiles.GetKeyList();

				for ( int i = 0; i < _languageFiles.Count; i++ )
					result[i] = langKeys[i].ToString();

				return( result );
			}
		}

		private void GetLanguageFiles()
		{
			// Process the directory list
			DirectoryInfo dir = new DirectoryInfo(_path);
			foreach ( FileSystemInfo entry in dir.GetFiles(_prefix + "*." + _extension) )
			{
				// generate regex
				Regex regex = new Regex(
					_prefix + @"(?<lang>.*)\." + _extension,
					RegexOptions.IgnoreCase
					| RegexOptions.Multiline
					| RegexOptions.IgnorePatternWhitespace
					| RegexOptions.Compiled
					);

				// get language code from filename
				string lang = regex.Match(entry.Name).Result("${lang}");

				// add to language files
				_languageFiles.Add(lang, entry.Name);
			}
		}
	}
}