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
using System.Net;
using System.Collections;
using System.Runtime.Serialization.Formatters;

//sinks
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;


namespace eLePhant.eDonkey
{
	/// <summary>
	/// Descripción breve de CRemoting.
	/// Servidor remoto, registrar el canal, y publicar la clase.
	/// Previsto algun tipo de autentificación.
	/// </summary>
	internal class CRemoting
	{
		private TcpServerChannel m_lPhantChannel;
		private int puerto;
		private string[] IPPermitida;
		public string[] GetIPPermitida() { return IPPermitida; }
		public CRemoting()
		{
			//
			// TODO: agregar aquí la lógica del constructor
			//
			puerto = CKernel.Preferences.GetInt("RemoteControlPort",4670);
			IPPermitida = CKernel.Preferences.GetStringArray("AllowedIP");
			Hashtable props = new Hashtable();
			props.Add("name","eLePhantService");
			props.Add("priority","10"); //en la ayuda pone que son enteros, pero con ellos da error de conversion.
			props.Add("port", puerto);
			props.Add("supressChannelData",true);
			props.Add("useIpAddress",true);
			props.Add("rejectRemoteRequests",false);

#if (!COMPRESSION)
			
			BinaryServerFormatterSinkProvider provider = 
				new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = 
				System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;			
			m_lPhantChannel = new TcpServerChannel(props, provider);
			ChannelServices.RegisterChannel(m_lPhantChannel);
#else		
			//Iniciacion
			Hashtable propsinks = new Hashtable();
			propsinks.Add("includeVersions",true);
			propsinks.Add("typeFilterLevel","Full");
			Hashtable datasinks = new Hashtable();
			
			//2ª Opcion
			eLePhantServerSinkProvider provider = 
				new eLePhantServerSinkProvider(propsinks,datasinks);

			//Creacion
			m_lPhantChannel = new TcpServerChannel(props, provider);
			ChannelServices.RegisterChannel(m_lPhantChannel);						
			
#endif
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(CInterfaceGateway), 
				"InterfazRemota",
				WellKnownObjectMode.Singleton);	
		}		

		~CRemoting()
		{
			ChannelServices.UnregisterChannel(m_lPhantChannel);
			RemotingConfiguration.Configure(null);
		}
	}

	[Serializable]
	internal class eLePhantServerSinkProvider:IServerChannelSinkProvider
	{
		private IServerChannelSinkProvider m_nextIServerChannelSink = null;		
		private Hashtable m_Properties = new Hashtable();
		private Hashtable m_ProviderData = new Hashtable();
		private TypeFilterLevel m_TypeFilterLevel = TypeFilterLevel.Full;
		private IChannelReceiver m_ChannelReceiver = null;
		private IServerChannelSink m_nextSink = null;
		private IChannelDataStore m_ChannelData = null;

		#region constructor
		public eLePhantServerSinkProvider ()
		{
			m_Properties.Add("includeVersions",true);
			this.m_nextIServerChannelSink = new BinaryServerFormatterSinkProvider(m_Properties,m_ProviderData);	
		}
		public eLePhantServerSinkProvider (Hashtable Properties, Hashtable ProviderData)
		{
			if (Properties != null) m_Properties=Properties;
			if (ProviderData != null) m_ProviderData=ProviderData;
			this.m_nextIServerChannelSink = new BinaryServerFormatterSinkProvider(m_Properties,m_ProviderData);	
		}
		#endregion

		#region Miembros de IServerChannelSinkProvider		

		public IServerChannelSinkProvider Next
		{
			get
			{			
				return m_nextIServerChannelSink;
			}
			set
			{
				m_nextIServerChannelSink = value;
			}
		}
		
		public IServerChannelSink CreateSink(IChannelReceiver channel)
		{
			this.m_ChannelReceiver=channel;
			IServerChannelSink nextSink = null;			
			if (m_nextIServerChannelSink == null) 
				this.Next=new BinaryServerFormatterSinkProvider(m_Properties,m_ProviderData);				
				
			nextSink=this.m_nextIServerChannelSink.CreateSink(channel);
			
			if (this.m_nextSink == null)
			{
				this.m_nextSink = new eLePhantServerChannelSink(this,channel,nextSink);
				return this.m_nextSink;
			}
			else 
			{
				return new eLePhantServerChannelSink(this,channel,nextSink);
			}
		}

		public void GetChannelData(IChannelDataStore channelData)
		{
			m_ChannelData = channelData;
		}

		#endregion

		#region Metodos Publicos				

		public IChannelDataStore GetChannelData()
		{
			return this.m_ChannelData;
		}

		public TypeFilterLevel TypeFilterLevel 
		{
			get
			{
				return m_TypeFilterLevel;
			}
			set
			{
				m_TypeFilterLevel=value;
			}
		}
		
		#endregion
	}
	
	[Serializable]
	internal class eLePhantServerChannelSink: BaseChannelObjectWithProperties, IServerChannelSink
	{
		private IServerChannelSink m_NextIServerChannelSink = null;
		private eLePhantServerSinkProvider m_Provider = null;
		private IChannelReceiver m_channel = null;
		private CompressionType m_CompressionMethod = (CompressionType)
			Enum.Parse( typeof(CompressionType),
			(string)CKernel.Preferences.GetProperty("CompressionMethod"));

		#region Constructor
		
		public eLePhantServerChannelSink(IServerChannelSinkProvider Provider, IChannelReceiver channel)
		{
			IServerChannelSink nextServer=(IServerChannelSink)new BinaryServerFormatterSink(
				BinaryServerFormatterSink.Protocol.Other,this.NextChannelSink,channel);
			if (channel != null) m_channel=channel;
			if (Provider != null) m_Provider=Provider as eLePhantServerSinkProvider;
			m_NextIServerChannelSink=new eLePhantServerChannelSink(Provider,channel,nextServer);
		}

		public eLePhantServerChannelSink(IServerChannelSinkProvider Provider, IChannelReceiver channel, object nextobject)
		{
			if (channel != null) m_channel=channel;
			if (Provider != null) m_Provider=Provider as eLePhantServerSinkProvider;
			if (nextobject != null) 
			{
				m_NextIServerChannelSink=nextobject as IServerChannelSink;
				if (m_NextIServerChannelSink==null)
					m_NextIServerChannelSink=new BinaryServerFormatterSink(
						BinaryServerFormatterSink.Protocol.Other,this.NextChannelSink,channel);

			}		
		}
		#endregion

		#region Miembros de IServerChannelSink

		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, 
			object state, IMessage msg, ITransportHeaders headers)
		{			
			return this.m_NextIServerChannelSink.GetResponseStream(sinkStack,state,msg,headers);
		}

		public System.Runtime.Remoting.Channels.ServerProcessing ProcessMessage
			(IServerChannelSinkStack sinkStack, IMessage requestMsg, 
			ITransportHeaders requestHeaders, Stream requestStream, 
			out IMessage responseMsg, out ITransportHeaders responseHeaders, 
			out Stream responseStream)
		{		 
			//Iniciacion de variables, las respuestas no deben estar inicializadas, ver la documentacion
			//de ProcessMessage
			responseMsg = null;
			responseHeaders = null;
			responseStream = null;
			ServerProcessing processing = ServerProcessing.Complete;
			object state = null;

			//Obtener la ip remota a traves del canal actual y comprobar si esta permitida.
			//Si lo esta la procesamos y la enviamos a la pila de procesos (siguiente canal).
			
			IPAddress ipCliente= (IPAddress) requestHeaders[CommonTransportKeys.IPAddress];
			if ( checkip(ipCliente.ToString()) )
			{	
				//Sustitución de requestStream que esta cerrado. Propiedad CanWrite no deja modificarlo.
				//no se puede ms=requestStream !!!
				//porque copia tambien las propiedades.
				//Tampoco directamente sobre el Stream, no se inicializan!!!
				if (!requestStream.CanWrite)
					AbrirStream(ref requestStream,
						System.Convert.ToInt32( (string)requestHeaders["TamañoComprimido"]) );
			
				//Preprocesade del mensaje, descompresion	
				ProcessRequest(requestMsg, requestHeaders, ref requestStream, ref state);

				//Envio del mensaje atraves del siguiente canal.
				//El siguiente IServerChannelSink es standar y no lo gestionamos. 
				//vease IServerSinkProvider.CreateSink que es llamado automaticamente al crear el TcpServerChannel
				//Y los constructores de esta clase
				try 
				{				
					if ( (bool)state)
						processing = this.NextChannelSink.ProcessMessage (
							sinkStack, requestMsg, requestHeaders, requestStream, 
							out responseMsg, out responseHeaders,out responseStream);			
				}  
				catch (RemotingException e)
				{
					//Console.Write(e.Message);
					responseMsg = new ReturnMessage(e, (IMethodCallMessage)requestMsg);
				}

				//Comprobamos que responseStream esta abierto.
				if (!responseStream.CanWrite)
					AbrirStream(ref responseStream,
						System.Convert.ToInt32( (string)responseMsg.Properties["TamañoComprimido"]) );

				//PostProcesado del mensaje, compresion de la respuesta.
				ProcessResponse(responseMsg, responseHeaders, ref responseStream, state);

				//Depuracion
				/*if (responseMsg.Properties["__MethodName"]==null)
					Console.WriteLine("Peticion desconocida");
				else
					Console.WriteLine("Peticion: " + (string)responseMsg.Properties["__MethodName"]); 
				Console.WriteLine("Datos recibidos:" + 
					(string)requestHeaders["Tamaño"] + "/" +
					(string)requestHeaders["TamañoComprimido"]);
				Console.WriteLine("Datos enviados:" + 
					System.Convert.ToString(responseHeaders["Tamaño"]) + "/" +
					System.Convert.ToString(responseHeaders["TamañoComprimido"])   );
				*///fin depuracion
			} 
			else 
			{
				Exception exp = new Exception(string.Format("{0}:IP nonallowed.", m_Provider.ToString()));
				responseMsg = new ReturnMessage(exp, (IMethodCallMessage)requestMsg);
			}
			return processing;
		}

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, 
			object state, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			//Como no se suele ejecutar no he podido ver las cabeceras ni el mensaje
			if (!stream.CanWrite)
			{
				try 
				{
					AbrirStream(ref stream);
				} 
				catch 
				{
					AbrirStream(ref stream,
						System.Convert.ToInt32( (string)msg.Properties["TamañoComprimido"]) );
				}
			}
			ProcessResponse(msg, headers, ref stream, state);

			try 
			{
				sinkStack.AsyncProcessResponse(msg, headers, stream);
			}
			catch (RemotingException e)
			{
				Console.Write(e.Message);				
			}
		}

		public IServerChannelSink NextChannelSink
		{
			get{ return m_NextIServerChannelSink;}
		}

		#endregion

		#region procesado del mensaje, compresion / descompresion
		private void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream, ref object state)
		{
			state = true;
			if ((string)headers["edonkeyCompress"] == "Yes")
			{
				//Descomprimir	y quitar la cabecera
				CompressionType c=CompressionType.Zip;
				int com=System.Convert.ToInt32(headers["CompressionType"]);
				if (Enum.GetName(typeof(CompressionType),com) != null)
				{
					c=(CompressionType)com;
					Descompresion descompresor=new Descompresion(stream,c);
					Stream descomprimido = descompresor.ToStream;
	
					if (descomprimido != null)
					{
						headers["CompressionType"] = null;
						headers["edonkeyCompress"] = null;
						//Si descomentamos no podemos hacer la depuracion
						//headers["TamañoComprimido"] = null;
						//headers["Tamaño"] = null;
						stream = descomprimido;
					} 
					else 
					{
						stream = null;
						Exception exp = new Exception(string.Format("{0}:Error, could not be decompressed.", m_Provider.ToString()));
						IMessage responseMsg = new ReturnMessage(exp, (IMethodCallMessage)message);
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf=
							new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
						bf.Serialize(stream,responseMsg);
					}
				}
				else
				{
					stream = null;
					Exception exp = new Exception(string.Format("{0}:Error, unknown method of compression.", m_Provider.ToString()));
					IMessage responseMsg = new ReturnMessage(exp, (IMethodCallMessage)message);
					System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf=
						new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					bf.Serialize(stream,responseMsg);
				}
			}
		}
		
		private void ProcessResponse(IMessage message, ITransportHeaders headers, ref Stream stream, object state)
		{
			if (state != null)
			{				
				if (headers!=null) 
				{	
					//Comprimir y marcar la cabecera.					
					Compresion compresor=new Compresion(stream, m_CompressionMethod  );
					Stream comprimido=compresor.ToStream;
					if (comprimido != null) 
					{	
						if (comprimido.Length < stream.Length)
						{
							headers["edonkeyCompress"] = "Yes";
							headers["TamañoComprimido"] = comprimido.Length;
							headers["Tamaño"] = stream.Length;
							headers["CompressionType"]=(int)compresor.CompressionProvider;
							stream = comprimido;
						}
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
			ms.Write(Data,0,tamano);
			stream=null;
			stream=ms;
		}
		private bool checkip(string IP)
		{
			string[] IPAllowed=(string[])CKernel.Preferences.GetProperty("AllowedIP");
			for (int i=0;i<IPAllowed.Length;i++)
			{
				if (IPAllowed[i]==IP) return true;
				if (IPAllowed[i]=="255.255.255.255") return true;
			}
			return false;
		}
		#endregion
	
	}
}