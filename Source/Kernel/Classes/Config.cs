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
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace eLePhant.Classes
{
	/// <summary>
	/// The Config class is a file storage for propertys.
	/// </summary>
	public class Config
	{
		#region Instance fields
		/// <summary>Store for the preferences</summary>
		protected Hashtable _preferences = new Hashtable();
		protected ArrayList _runtimePrefs = new ArrayList();
		/// <summary>Store for the configuration directory</summary>
		protected string _configDirectory;
		/// <summary>Store for the configuration file</summary>
		protected string _fileName;
		/// <summary>Store for the property file version</summary>
		protected string _version;
		/// <summary>Store for the property current file version</summary>
		protected string _currentversion;
		/// <summary>Store for the property file root node name</summary>
		protected string _xmlRootNode;
		#endregion

		#region Constructors & Initialization
		/// <summary>
		/// Create a new config class.
		/// </summary>
		public Config()
		{
			_xmlRootNode = "lphant";
			_version = "0.1";
		}

		public Config(string directory, string file)
		{
			this._xmlRootNode = "lphant";
			this._version = "0.1";

			this._configDirectory = directory;
			this._fileName = file;
		}

		public Config(string directory, string file, string version)
		{
			this._xmlRootNode = "lphant";
			this._version = version;

			this._configDirectory = directory;
			this._fileName = file;
		}

		public Config(string directory, string file, string version, string rootNodeName)
		{
			this._xmlRootNode = rootNodeName;
			this._version = version;

			this._configDirectory = directory;
			this._fileName = file;
		}
		#endregion

		#region Propertys
		/// <summary>
		/// Set the file name for the preferences file.
		/// </summary>
		public string PropertyFileName
		{
			get
			{
				return this._fileName;
			}
			set
			{
				if (this._fileName != value)
				{
					this._fileName = value;
				}
			}
		}

		/// <summary>
		/// Set the directory where preferences stored.
		/// </summary>
		public string ConfigDirectory
		{
			get
			{
				return this._configDirectory;
			}
			set
			{
				if (this._configDirectory != value)
				{
					this._configDirectory = value;
				}
			}
		}

		/// <summary>
		/// Set the version of config file.
		/// </summary>
		public string Version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}
		#endregion

		#region Load & Save Properties
		/// <summary>
		/// Loads the global properties from the current users application data folder, or
		/// if it doesn't exists or couldn't read them it reads the default properties out
		/// of the application folder.
		/// </summary>
		/// <exception cref="PropertyFileLoadException">
		/// Is thrown when no property file could be loaded.
		/// </exception>
		public void LoadProperties()
		{
			if (((this._configDirectory!="")&&(!Directory.Exists(this._configDirectory))))
			{
				Directory.CreateDirectory(this._configDirectory);
			}

			if (!LoadPropertiesFromStream(Path.Combine(this._configDirectory, this._fileName)))
			{
/*				if (!LoadPropertiesFromStream(DataDirectory + Path.DirectorySeparatorChar + "options" + Path.DirectorySeparatorChar + propertyFileName)) 
				{
					throw new PropertyFileLoadException();
				}
*/			}
		}

		/// <summary>
		/// Saves the current global property state to a file in the users application data folder.
		/// </summary>
		public void SaveProperties()
		{
			if (((this._configDirectory!="")&&(!Directory.Exists(this._configDirectory))))
			{
				Directory.CreateDirectory(this._configDirectory);
			}
			WritePropertiesToFile(Path.Combine(_configDirectory, _fileName));
		}

		protected bool LoadPropertiesFromStream(string filename)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);

				_currentversion = doc.DocumentElement.Attributes["version"].InnerText;

				SetValueFromXmlElement(doc.DocumentElement["properties"]);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception while load properties from stream :\n " + e.ToString());
				return false;
			}

			OnPropertyLoaded( new PropertyLoadedArgs(_currentversion) );

			return true;
		}

		protected void WritePropertiesToFile(string fileName)
		{
			lock(this)
			{
				try
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml("<?xml version=\"1.0\"?>\n<" + this._xmlRootNode + " version = \"" + this._version + "\" />");
					doc.DocumentElement.AppendChild(ToXmlElement(doc));
					doc.Save(Path.Combine(this._configDirectory, this._fileName));
				}
				catch(Exception e)
				{
					Debug.WriteLine(e.ToString());
				}
			}
		}

		protected void SetValueFromXmlElement(XmlElement element)
		{
			XmlNodeList nodes = element.ChildNodes;
			foreach (XmlElement el in nodes)
			{
				if ( el.Name == "property" )
				{
					try
					{
						// which type of property we should read
						switch ( el.Attributes["type"].InnerText )
						{
							case "byte":
								// read byte value
								_preferences[el.Attributes["key"].InnerText] = byte.Parse(el.Attributes["value"].InnerText);
								break;
							case "int":
								// read int value
								_preferences[el.Attributes["key"].InnerText] = int.Parse(el.Attributes["value"].InnerText);
								break;
							case "uint":
								// read uint value
								_preferences[el.Attributes["key"].InnerText] = uint.Parse(el.Attributes["value"].InnerText);
								break;
							case "bool":
								// read byte value
								_preferences[el.Attributes["key"].InnerText] = bool.Parse(el.Attributes["value"].InnerText);
								break;
							case "short":
								// read short value
								_preferences[el.Attributes["key"].InnerText] = short.Parse(el.Attributes["value"].InnerText);
								break;
							case "ushort":
								// read ushort value
								_preferences[el.Attributes["key"].InnerText] = ushort.Parse(el.Attributes["value"].InnerText);
								break;
							case "float":
								// read float value
								_preferences[el.Attributes["key"].InnerText] = float.Parse(el.Attributes["value"].InnerText);
								break;
							case "long":
								// read long value
								_preferences[el.Attributes["key"].InnerText] = long.Parse(el.Attributes["value"].InnerText);
								break;
							case "ulong":
								// read ulong value
								_preferences[el.Attributes["key"].InnerText] = ulong.Parse(el.Attributes["value"].InnerText);
								break;
							case "double":
								// read double value
								_preferences[el.Attributes["key"].InnerText] = double.Parse(el.Attributes["value"].InnerText);
								break;
							case "string":
								// read string value
								_preferences[el.Attributes["key"].InnerText] = el.Attributes["value"].InnerText;
								break;
							case "string[]":
								if (el.HasChildNodes)
								{
									string[] entrys = new string[el.ChildNodes.Count];
									int i = 0;
									foreach (XmlElement e in el)
									{
										entrys[i] = e.InnerText;
										i++;
									}

									_preferences[el.Attributes["key"].InnerText] = (object)entrys;
								}
								break;
							case "byte[]":
								byte[] x=null;
								switch(_currentversion)
								{
									case "0.01":
										x = new byte[el.Attributes["value"].InnerText.Length];
										x = Encoding.Default.GetBytes( el.Attributes["value"].InnerText );
										break;

									case "0.02":
										x = Convert.FromBase64String( el.Attributes["value"].InnerText );
										break;
								}
								_preferences[el.Attributes["key"].InnerText] = (object)x;
								break;
							case "enum":
								// read enum value
								_preferences[el.Attributes["key"].InnerText] = el.Attributes["value"].InnerText;
								break;
						}
					}
					catch(Exception e)
					{
						Debug.WriteLine("Exception while load property type from stream :\n " + e.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Converts the <code>Properties</code> object to a <code>XmlElement</code>
		/// </summary>
		/// <returns>
		/// A new <code>XmlElement</code> object which represents the state
		/// of the <code>Properties</code> object.
		/// </returns>
		private XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement propertiesnode  = doc.CreateElement("properties");

			foreach (DictionaryEntry entry in _preferences) 
			{
				if ((entry.Value != null) && (!_runtimePrefs.Contains(entry.Key)))
				{
					if (entry.Value is XmlElement)
					{
						// write unchanged XmlElement back
						propertiesnode.AppendChild(doc.ImportNode((XmlElement)entry.Value, true));
					}
					else
					{
						string valueType = "";

						XmlElement el = doc.CreateElement("property");

						// append key
						XmlAttribute key = doc.CreateAttribute("key");
						key.InnerText = entry.Key.ToString();
						el.Attributes.Append(key);

						// write byte value
						if ( entry.Value is byte)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "byte";
						}

						// write int value
						if ( entry.Value is int)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "int";
						}

						// write uint value
						if ( entry.Value is uint)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "uint";
						}

						// write bool value
						if ( entry.Value is bool)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "bool";
						}

						// write short value
						if ( entry.Value is short)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "short";
						}

						// write ushort value
						if ( entry.Value is ushort)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "ushort";
						}

						// write float value
						if ( entry.Value is float)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "float";
						}

						// write long value
						if ( entry.Value is long)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "long";
						}

						// write ulong value
						if ( entry.Value is ulong)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "ulong";
						}

						// write double value
						if ( entry.Value is double)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "double";
						}

						// write string value
						if ( entry.Value is string)
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "string";
						}

						// write string[] value
						if ( entry.Value is string[] )
						{
							valueType = "string[]";

							foreach ( string e in (string[])entry.Value )
							{
								XmlElement x = doc.CreateElement("entry");
								x.InnerText = e;
								el.AppendChild(x);
							}
						}

						// write byte[] value
						if ( entry.Value is byte[] )
						{
							XmlAttribute val = doc.CreateAttribute("value");
							switch(_version)
							{
								case "0.01":
									val.InnerText = Encoding.Default.GetString( (byte[])entry.Value );
									break;

								case "0.02":
									val.InnerText = Convert.ToBase64String((byte[])entry.Value);
									break;
							}
							el.Attributes.Append(val);
							valueType = "byte[]";
						}

						// write enum value
						if ( entry.Value is System.Enum )
						{
							XmlAttribute val = doc.CreateAttribute("value");
							val.InnerText = entry.Value.ToString();
							el.Attributes.Append(val);

							valueType = "enum";
						}

						// append value type if needed
						if ( valueType != "" )
						{
							XmlAttribute type = doc.CreateAttribute("type");
							type.InnerText = valueType;
							el.Attributes.Append(type);
						}

						propertiesnode.AppendChild(el);
					}
				}
			}
			return propertiesnode;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets a property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public object GetProperty(string key, object defaultvalue)
		{
			if (!_preferences.ContainsKey(key)) 
			{
				if (defaultvalue != null) 
				{
					_preferences[key] = defaultvalue;
				}
				return defaultvalue;
			}

			object obj = _preferences[key];

			return obj;
		}

		/// <summary>
		/// Gets a property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>null</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		public object GetProperty(string key)
		{
			if (!_preferences.ContainsKey(key)) 
			{
				object def = null;

				PropertyDefaultArgs e = new PropertyDefaultArgs(key, def);
				OnGetDefault(e);
				if (e.Value != null)
				{
					_preferences[key] = e.Value;
					return e.Value;
				}
			}

			object obj = _preferences[key];

			return obj;
		}

		/// <summary>
		/// Gets a <code>byte</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public byte GetByte(string key, byte defaultvalue)
		{
			return (byte)GetProperty(key, (object)defaultvalue);
		}

		public byte GetByte(string key)
		{
			return (byte)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>int</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public int GetInt(string key, int defaultvalue)
		{
			return (int)GetProperty(key, (object)defaultvalue);
		}

		public int GetInt(string key)
		{
			return (int)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>uint</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public uint GetUInt(string key, uint defaultvalue)
		{
			return (uint)(int)GetProperty(key, (object)defaultvalue);
		}

		public uint GetUInt(string key)
		{
			object v=GetProperty(key);
			if (v.GetType()==typeof(System.UInt32))
				return (uint)v;
			else 
				return (uint)(int)v;
		}

		/// <summary>
		/// Gets a <code>bool</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public bool GetBool(string key, bool defaultvalue)
		{
			return (bool)GetProperty(key, (object)defaultvalue);
		}

		public bool GetBool(string key)
		{
			return (bool)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>short</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public short GetShort(string key, short defaultvalue)
		{
			return (short)GetProperty(key, (object)defaultvalue);
		}

		public short GetShort(string key)
		{
			return (short)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>ushort</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public ushort GetUShort(string key, ushort defaultvalue)
		{
			return (ushort)GetProperty(key, (object)defaultvalue);
		}

		public ushort GetUShort(string key)
		{
			return (ushort)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>float</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public float GetFloat(string key, float defaultvalue)
		{
			return (float)GetProperty(key, (object)defaultvalue);
		}

		public float GetFloat(string key)
		{
			return (float)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>long</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public long GetLong(string key, long defaultvalue)
		{
			return (long)GetProperty(key, (object)defaultvalue);
		}

		public long GetLong(string key)
		{
			return (long)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>ulong</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public ulong GetULong(string key, ulong defaultvalue)
		{
			return (ulong)GetProperty(key, (object)defaultvalue);
		}

		public ulong GetULong(string key)
		{
			return (ulong)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>double</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public double GetDouble(string key, double defaultvalue)
		{
			return (double)GetProperty(key, (object)defaultvalue);
		}

		public double GetDouble(string key)
		{
			return (double)GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>string</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public string GetString(string key, string defaultvalue)
		{
			return GetProperty(key, (object)defaultvalue).ToString();
		}

		public string GetString(string key)
		{
			return GetProperty(key).ToString();
		}

		/// <summary>
		/// Gets a <code>byte[]</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public byte[] GetByteArray(string key, byte[] defaultvalue)
		{
			return (byte[])GetProperty(key, (object)defaultvalue);
			//return Encoding.Default.GetBytes(GetProperty(key, (object)defaultvalue).ToString());
		}

		public byte[] GetByteArray(string key)
		{
			return (byte[])GetProperty(key);
			//return Encoding.Default.GetBytes(GetProperty(key).ToString());
		}

		/// <summary>
		/// Gets a <code>string[]</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public string[] GetStringArray(string key, string[] defaultvalue)
		{
			return (string[])GetProperty(key, (object)defaultvalue);
		}

		public string[] GetStringArray(string key)
		{
			return (string[])GetProperty(key);
		}

		/// <summary>
		/// Gets a <code>enum</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public System.Enum GetEnum(string key, System.Enum defaultvalue)
		{
			return (System.Enum)Enum.Parse(defaultvalue.GetType(), GetProperty(key, (object)defaultvalue).ToString());
		}

		/// <summary>
		/// Sets the property <code>key</code> to the value <code>val</code>.
		/// If <code>val</code> is null, the property will be taken out from the
		/// properties.
		/// </summary>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="val">
		/// The value of the property.
		/// </param>
		public void SetProperty(string key, object val)
		{
			if (val!=null)
			{

				object oldValue = _preferences[key];
			
				if (!val.Equals(oldValue)) 
				{
					_preferences[key] = val;

					PropertyEventArgs e = new PropertyEventArgs(key, oldValue, val);
					OnPropertyChanged(e);
					if (e.NewValue != oldValue)
					{
						_preferences[key] = e.NewValue;
					}
				}
			}
		}

		public void SetProperty(string key, object val, bool raise)
		{
			object oldValue = _preferences[key];
			if (!val.Equals(oldValue)) 
			{
				_preferences[key] = val;

				if (raise)
				{
					PropertyEventArgs e = new PropertyEventArgs(key, oldValue, val);
					OnPropertyChanged(e);
					if (e.NewValue != oldValue)
					{
						_preferences[key] = e.NewValue;
					}
				}
			}
		}

		public void SetRuntimeProperty(string key, object val)
		{
			_runtimePrefs.Add(key);

			object oldValue = _preferences[key];
			if (!val.Equals(oldValue)) 
			{
				_preferences[key] = val;

				PropertyEventArgs e = new PropertyEventArgs(key, oldValue, val);
				OnPropertyChanged(e);
				if (e.NewValue != oldValue)
				{
					_preferences[key] = e.NewValue;
				}
			}
		}

		public void SetRuntimeProperty(string key, object val, bool raise)
		{
			_runtimePrefs.Add(key);

			object oldValue = _preferences[key];
			if (!val.Equals(oldValue)) 
			{
				_preferences[key] = val;

				if (raise)
				{
					PropertyEventArgs e = new PropertyEventArgs(key, oldValue, val);
					OnPropertyChanged(e);
					if (e.NewValue != oldValue)
					{
						_preferences[key] = e.NewValue;
					}
				}
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// The property changed event handler is called when a property has changed.
		/// </summary>
		public event PropertyEventHandler PropertyChanged;
		public event PropertyDefaultHandler PropertyDefaults;
		public event PropertyLoadedHandler PropertyLoaded;

		public delegate void PropertyEventHandler(object sender, PropertyEventArgs e);
		public delegate void PropertyDefaultHandler(object sender, PropertyDefaultArgs e);
		public delegate void PropertyLoadedHandler(object sender, PropertyLoadedArgs e);

		protected void OnPropertyChanged(PropertyEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}

		protected void OnGetDefault(PropertyDefaultArgs e)
		{
			if (PropertyDefaults != null)
			{
				PropertyDefaults(this, e);
			}
		}

		protected void OnPropertyLoaded(PropertyLoadedArgs e)
		{
			if (PropertyLoaded != null)
			{
				PropertyLoaded(this, e);
			}
		}
		#endregion
	}

	#region Property Event Arguments
	public class PropertyEventArgs : EventArgs
	{
		string _key;
		object _newValue;
		object _oldValue;

		/// <returns>
		/// The key of the changed property
		/// </returns>
		public string Key
		{
			get
			{
				return _key;
			}
		}

		/// <returns>
		/// The new value of the property
		/// </returns>
		public object NewValue
		{
			get
			{
				return _newValue;
			}
			set
			{
				_newValue = value;
			}
		}

		/// <returns>
		/// The old value of the property
		/// </returns>
		public object OldValue
		{
			get
			{
				return _oldValue;
			}
		}

		public PropertyEventArgs(string key, object oldValue, object newValue)
		{
			this._key = key;
			this._oldValue = oldValue;
			this._newValue = newValue;
		}
	}

	public class PropertyDefaultArgs : EventArgs
	{
		string _key;
		object _value;

		/// <returns>
		/// The key of the changed property
		/// </returns>
		public string Key
		{
			get
			{
				return _key;
			}
		}

		/// <returns>
		/// The new value of the property
		/// </returns>
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				this._value = value;
			}
		}

		public PropertyDefaultArgs(string key, object value)
		{
			this._key = key;
			this._value = value;
		}
	}

	public class PropertyLoadedArgs : EventArgs
	{
		string _version;

		/// <returns>
		/// The version of loaded file
		/// </returns>
		public string Version
		{
			get
			{
				return _version;
			}
		}

		public PropertyLoadedArgs(string version)
		{
			this._version = version;
		}
	}
	#endregion
}