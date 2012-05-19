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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using eLePhant.eDonkey;
//dos clases para obtener acceso a las preferecias de la aplicacion.
//y todo por no existir todavia el formulario principal.
using eLePhant.Classes;
using System.Windows.Forms;

using System.Diagnostics;
using System.Security.Cryptography;
using System.Collections;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib;

namespace eLePhant.Client
{
	/// <summary>
	/// Descripción breve de edonkeyCRemoto.
	/// </summary>
	public class CedonkeyCRemoto
	{
		private TcpClientChannel m_lPhantChannel;

		private IPAddress m_ip;
		public IPAddress ip(){ return m_ip;}

		private int m_puertos=4670;
		public int puertos(){ return m_puertos;}

		private string m_url;
		public string url(){ return m_url;}

		private int m_puertor;
		public int puertor(){ return m_puertor;}

		public CInterfaceGateway  interfazremota;		
		private Hashtable props ;		
		private IClientChannelSinkProvider provider;

		public CedonkeyCRemoto()
		{	
			props = new Hashtable();
			props.Add("name","eLePhantClient");
			props.Add("priority","10");
			props.Add("port",0);

#if (!COMPRESSION)
			
			props.Add("supressChannelData",true);
			props.Add("useIpAddress",true);
			props.Add("rejectRemoteRequests",false);			
			provider = new BinaryClientFormatterSinkProvider();	

#else
			//iniciacion
			Hashtable propsinks = new Hashtable();
			propsinks.Add("includeVersions",true);
			Hashtable datasinks = new Hashtable();
			
			//2ª Opcion			
			provider = new eLePhantClientSinkProvider(propsinks,datasinks);	
				
#endif
		}
		~CedonkeyCRemoto()
		{
			RemotingConfiguration.Configure(null);
			ChannelServices.UnregisterChannel(m_lPhantChannel);		
		}
		public void DisConnect()
		{
			try 
			{
				RemotingConfiguration.Configure(null);
				ChannelServices.UnregisterChannel(m_lPhantChannel);				
			} 
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}
		public bool Connect(string IP,string clave,int puertor)
		{			
			m_ip = IPAddress.Parse(IP);
			if (puertor!=0)
				this.m_puertos=puertor;

			this.m_url="tcp://" + IP + ":" + this.m_puertos + "/InterfazRemota";
			this.m_puertor=puertor;

			System.Security.Cryptography.MD5 cripto=System.Security.Cryptography.MD5.Create();

			bool valor=false;
			try
			{
				m_lPhantChannel = new TcpClientChannel(props, provider);	
				ChannelServices.RegisterChannel(m_lPhantChannel);
			}
			catch
			{				
				DisConnect();
				if (m_lPhantChannel!=null)
					ChannelServices.RegisterChannel(m_lPhantChannel);
				else
					m_lPhantChannel = new TcpClientChannel(props, provider);	
			}
			interfazremota = (CInterfaceGateway) Activator.GetObject(typeof(eLePhant.eDonkey.CInterfaceGateway),
				this.m_url);
			
			if (interfazremota == null)
				Debug.Write("No se pudo encontrar el servidor");
			else 
			{									
				try
				{
					valor = interfazremota.CheckPw( clave );						
				}
				catch
				{					
					Debug.Write("\nNo se pudo encontrar el servidor\n");
				}
			}
			return valor;
		}
		public bool Connect(string IP,int puertol,string clave,int puertor)
		{			
			m_ip = IPAddress.Parse(IP);
			if (puertor!=0)
				this.m_puertos=puertor;

			this.m_url="tcp://" + IP + ":" + this.m_puertos + "/InterfazRemota";
			this.m_puertor=puertor;

			System.Security.Cryptography.MD5 cripto=System.Security.Cryptography.MD5.Create();

			bool valor=false;
			try
			{
				m_lPhantChannel = new TcpClientChannel(props, provider);	
				ChannelServices.RegisterChannel(m_lPhantChannel);
			}
			catch
			{				
				DisConnect();
				if (m_lPhantChannel!=null)
					ChannelServices.RegisterChannel(m_lPhantChannel);
				else
					m_lPhantChannel = new TcpClientChannel(props, provider);
			}
			interfazremota = (CInterfaceGateway) Activator.GetObject(typeof(eLePhant.eDonkey.CInterfaceGateway),
				this.m_url);
			
			if (interfazremota == null)
				Debug.Write("No se pudo encontrar el servidor");
			else 
			{					
				//c = new byte[clave.Length];
				//for (int i=0;i<clave.Length;i++) c[i]=System.Convert.ToByte(clave[i]);
				//cripto.ComputeHash(c);					
				//try
				//{
				valor = interfazremota.CheckPw( clave );						
				/*}
				catch
				{
					ChannelServices.UnregisterChannel(this.canalCeLephant);
					this.canalCeLephant = null;
					Debug.Write("\nNo se pudo encontrar el servidor\n");
				}*/
			}
			return valor;
		}		
	}

	[Serializable]
	public class eLePhantClientSinkProvider : IClientChannelSinkProvider, IClientFormatterSinkProvider
	{
		private IClientChannelSinkProvider m_NextChannelSinkProvider=null;
		private IClientChannelSink m_NextChannelSink=null;
		private Hashtable m_Properties=new Hashtable();
		private Hashtable m_ProviderData=new Hashtable();

		#region Constructor
		public eLePhantClientSinkProvider()
		{
			m_Properties.Add("includeVersions",true);
			//this.Next=new BinaryClientFormatterSinkProvider(m_Properties,m_ProviderData);
		}
		public eLePhantClientSinkProvider(Hashtable Properties, Hashtable ProviderData)
		{
			this.m_Properties=Properties;
			this.m_ProviderData=ProviderData;
			//this.Next=new BinaryClientFormatterSinkProvider(m_Properties,m_ProviderData);
		}
		#endregion

		#region Miembros de IClientChannelSinkProvider

		public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			object NextSink=null;
			if (this.m_NextChannelSinkProvider == null)
				this.m_NextChannelSinkProvider=new BinaryClientFormatterSinkProvider(m_Properties,m_ProviderData);

			NextSink = this.m_NextChannelSinkProvider.CreateSink(channel,url,remoteChannelData);

			if (this.m_NextChannelSink==null)
			{
				this.m_NextChannelSink=new eLePhantClientSink(this,url,NextSink);
				return this.m_NextChannelSink;
			} 
			else 
			{
				return new eLePhantClientSink(this,url,NextSink);
			}

			
		}

		public IClientChannelSinkProvider Next
		{
			get	{ return m_NextChannelSinkProvider;  }
			set	{ m_NextChannelSinkProvider = value; }
		}

		#endregion
		
	}

	[Serializable]
	public class eLePhantClientSink:BaseChannelObjectWithProperties,IClientChannelSink,IMessageSink
	{
		private string m_url=null;
		private IClientChannelSink m_NextChannelSink=null;
		private IMessageSink m_NextMessageSink=null;
		private eLePhantClientSinkProvider m_Provider=null;
		private Hashtable m_properties = new Hashtable();
		private eLePhant.Classes.Config m_preferences;
		private CompressionType m_CompressionMethod;

		#region Constructor
		public eLePhantClientSink(IClientChannelSinkProvider Provider, string url)
		{
			object nextobject=new BinaryClientFormatterSink(this);			
			this.m_url=url;
			this.m_Provider=Provider as eLePhantClientSinkProvider;
			this.m_NextChannelSink = nextobject as IClientChannelSink;
			this.m_NextMessageSink = nextobject as IMessageSink;
			m_preferences = new Config(Application.StartupPath, "config.xml", "0.02", "lphantKernel");
			this.m_CompressionMethod=(CompressionType)m_preferences.GetEnum("CompressionMethod",CompressionType.Zip);
		}

		public eLePhantClientSink(IClientChannelSinkProvider Provider, string url, object nextobject)
		{
			this.m_url=url;
			this.m_Provider=Provider as eLePhantClientSinkProvider;
			if (nextobject != null)
			{
				this.m_NextChannelSink = nextobject as IClientChannelSink;
				if (this.m_NextChannelSink ==null)
					this.m_NextChannelSink = (IClientChannelSink)new BinaryClientFormatterSink(this);
				
				this.m_NextMessageSink = nextobject as IMessageSink;
				if (this.m_NextMessageSink == null)
					this.m_NextMessageSink = (IMessageSink)new BinaryClientFormatterSink(this);
					
			}
			m_preferences = new Config(Application.StartupPath, "config.xml", "0.02", "lphantKernel");
			this.m_CompressionMethod=(CompressionType)m_preferences.GetEnum("CompressionMethod",CompressionType.Zip);
		}
		#endregion

		#region metodos publicos
		public void SetNext(object nextobject)
		{
			this.m_NextChannelSink = nextobject as IClientChannelSink;
			this.m_NextMessageSink = nextobject as IMessageSink;
		}
		#endregion

		#region Miembros de IClientChannelSink

		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			object state = null;
			//requestStream esta cerrado, sustitucion por otra.
			if (!stream.CanWrite)
				AbrirStream(ref stream,
					System.Convert.ToInt32((string)headers["TamañoComprimido"]) );

			//Procesar el mensaje
			ProcessRequest(msg, headers, ref stream, ref state);

			try 
			{
				if ( (bool)state)
					this.m_NextChannelSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
			} 
			catch (Exception e)
			{
				Console.Write(e.Message);				
			}
		}

		public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			// Iniciacion de variable, respuesta no inicializadas, ver la documentacion de esta funcion
			responseHeaders = null;
			responseStream = null;
			object state = null;

			//requestStream esta cerrado, abir o susutituir por otra.
			if (!requestStream.CanWrite)
				AbrirStream(ref requestStream);

			//Procesar el mensaje
			ProcessRequest(msg, requestHeaders, ref requestStream, ref state);

			// Envio atraves del siguiente canal.
			try 
			{
				if ( (bool)state)
					this.m_NextChannelSink.ProcessMessage(msg, requestHeaders, requestStream, 
						out responseHeaders, out responseStream);
			}
			catch (Exception e)
			{
				Console.Write(e.Message);				
			}

			//responseStream esta cerrado, sustitucion por otra.
			if (!responseStream.CanWrite)
				AbrirStream(ref responseStream,
					System.Convert.ToInt32( (string)responseHeaders["TamañoComprimido"]) );
			
			//PostProcesado del mensaje
			ProcessResponse(null, responseHeaders, ref responseStream, state);

			//Depuracion
			/*if (msg.Properties["__MethodName"]==null)
				Console.WriteLine("Peticion desconocida");
			else
				Console.WriteLine("Peticion:" + (string)msg.Properties["__MethodName"]);
			Console.WriteLine("Datos enviados:" + 
				System.Convert.ToString(requestHeaders["Tamaño"]) + "/" +
				System.Convert.ToString(requestHeaders["TamañoComprimido"]) );
			Console.WriteLine("Datos recibidos:" + 
				(string)responseHeaders["Tamaño"] + "/" +
				(string)responseHeaders["TamañoComprimido"] );
			*///fin depuracion


			//Se debe devolver los datos en la variable original que debe salir de esta funcion para que la 
			//interprete el sistema.

			//responseStream=null;
			//responseStream=my_responseStream;

		}

		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			// process response como no lo he visto ejecutarse, mas vale prevenir.
			if (!stream.CanWrite)
			{
				try 
				{
					AbrirStream(ref stream);
				} 
				catch
				{
					AbrirStream(ref stream,
						System.Convert.ToInt32( (string)headers["TamañoComprimido"]) );
				}
			}
			ProcessResponse(null, headers, ref stream, state);

			try 
			{
				sinkStack.AsyncProcessResponse(headers, stream);
			}
			catch (Exception e)
			{
				Console.Write(e.Message);				
			}
		}

		public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			return this.m_NextChannelSink.GetRequestStream(msg, headers);
		}

		public IClientChannelSink NextChannelSink
		{
			get{return this.m_NextChannelSink;}
		}

		#endregion

		#region Miembros de IChannelSinkBase

		IDictionary IChannelSinkBase.Properties
		{
			get{ return m_properties; }
		}

		#endregion

		#region Miembros de IMessageSink

		//Aunque en ImessageSink Tambien se podria procesar los mensajes, estos no tienen header.
		//Y en el servidor no hay implementacion de ImessageSink
		public IMessage SyncProcessMessage(IMessage msg)
		{
			IMessage respMsg = null;
			try 
			{
				respMsg = this.m_NextMessageSink.SyncProcessMessage(msg);		
			}
			catch (Exception e)
			{
				Console.Write(e.Message);
			}

			return respMsg;
		}

		public IMessageSink NextSink
		{
			get{ return this.m_NextMessageSink;}
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			IMessageCtrl respMsg = null;
			try 
			{
				respMsg = this.m_NextMessageSink.AsyncProcessMessage(msg, replySink);
			} 
			catch (Exception e)
			{
				Console.Write(e.Message);
			}
			return respMsg;
		}

		#endregion

		#region Procesado de los mensajes
		private void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream, ref object state)
		{
			state = true;
			if (headers!=null) 
			{		
				//Comprimir peticion
				Compresion compresor=new Compresion(stream,  m_CompressionMethod  );
				Stream comprimido=compresor.ToStream;
				if (comprimido != null)
				{ 	
					if (comprimido.Length < stream.Length)
					{
						headers["edonkeyCompress"] = "Yes";
						headers["TamañoComprimido"]=comprimido.Length;
						headers["Tamaño"]=stream.Length;
						headers["CompressionType"]= (int)compresor.CompressionProvider;
						stream=comprimido;
					}
				}
				
			}
		}
		private void ProcessResponse(IMessage message, ITransportHeaders headers, ref Stream stream, object state)
		{			
			if (headers != null)
			{
				if ((string)headers["edonkeyCompress"] == "Yes")			
				{						
					//Descomprimir respuesta
					CompressionType c=CompressionType.Zip;
					int com=System.Convert.ToInt32(headers["CompressionType"]);
					if (Enum.GetName(typeof(CompressionType),com) != null)
					{
						c =(CompressionType)com;
						Descompresion descompresor =new Descompresion(stream,c );
						Stream descomprimido = descompresor.ToStream;
						if (descomprimido != null)
						{
							headers["CompressionType"] = null;
							headers["edonkeyCompress"] = null;
							//Si descomentamo no podemos hacer la depuracion
							//headers["TamañoComprimido"] = null;
							//headers["Tamaño"] = null;
							stream = descomprimido;
						} 
					} 
					else 
					{
						stream=null;
					}
				}
			}
		}
		#endregion

		#region Otras Funciones
		private void AbrirStream(ref Stream stream)
		{
			AbrirStream(ref stream,(int)stream.Length);
		}
		private void AbrirStream(ref Stream stream,int tamano)
		{
			MemoryStream ms = new MemoryStream(tamano);
			byte[] Data = new byte[tamano];
			stream.Read(Data,0,tamano);
			ms.Write(Data,0,tamaño);
			stream=null;
			stream=ms;
		}

		#endregion
	}
}
