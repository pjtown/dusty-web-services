using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;

namespace Dust.Web.Xml
{
	public class XmlRequestRouter : IRequestRouter
	{
		public const string Format = "xml";

		private readonly DataContractSerializer responseSerializer = new DataContractSerializer(typeof(DefaultResponse));
		private readonly Dictionary<string, IRequestHandler> requestProcessorsByName = new Dictionary<string, IRequestHandler>();
		private readonly IServiceOperationProfileLoader serviceOperationProfileLoader;

		public XmlRequestRouter(IServiceOperationProfileLoader serviceOperationProfileLoader)
		{
			this.serviceOperationProfileLoader = serviceOperationProfileLoader;
		}

		public string RequestFormat
		{
			get { return Format; }
		}

		public void Register(Assembly serviceOperationAssembly)
		{
			foreach (var serviceOperationProfiles in this.serviceOperationProfileLoader.LoadServiceOperationProfiles(serviceOperationAssembly))
			{
				var requestProcessorType = typeof(XmlRequestHandler<,>).MakeGenericType(serviceOperationProfiles.RequestType, serviceOperationProfiles.ResponseType);

				var requestProcessor = (IRequestHandler) Activator.CreateInstance(requestProcessorType, serviceOperationProfiles.ServiceOperation);

				foreach (var requestName in serviceOperationProfiles.RequestNames)
				{
					if (this.requestProcessorsByName.ContainsKey(requestName.ToLower()))
					{
						var log = string.Format("Cannot have duplicate XML service operation registered for name {0}", requestName);
						throw new Exception(log);
					}

					this.requestProcessorsByName.Add(requestName.ToLower(), requestProcessor);
				}
			}
		}

		public void Register(IEnumerable<Assembly> serviceOperationAssemblies)
		{
			foreach (var serviceOperationAssembly in serviceOperationAssemblies)
			{
				this.Register(serviceOperationAssembly);
			}
		}

		public void Process(HttpListenerContext httpListenerContext, string serviceOperationName)
		{
			IRequestHandler requestHandler;

			if (this.requestProcessorsByName.TryGetValue(serviceOperationName.ToLower(), out requestHandler))
			{
				requestHandler.Process(httpListenerContext);
			}
			else
			{
				this.WriteError(httpListenerContext, "UnknownRequest", "Unknown XML service operation name " + serviceOperationName, new StackTrace().ToString());
			}
		}

		public void WriteError(HttpListenerContext httpListenerContext, string errorCode, string errorMessage, string stackTrace)
		{
			var response = new DefaultResponse
      	{
      		Error = new Error
	        	{
	        		Code = errorCode,
	        		Message = errorMessage,
	        		StackTrace = stackTrace,
	        	}
      	};

			using (var outputStream = httpListenerContext.Response.OutputStream)
			{
				this.responseSerializer.WriteObject(outputStream, response);
			}
		}
	}
}