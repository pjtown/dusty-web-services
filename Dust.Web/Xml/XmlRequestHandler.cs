using System;
using System.Net;
using System.Runtime.Serialization;
using Dust.Serialization;

namespace Dust.Web.Xml
{
	public class XmlRequestHandler<TRequest, TResponse> : IRequestHandler
		where TRequest : new()
		where TResponse : IResponse, new()
	{
		private readonly IServiceOperation<TRequest, TResponse> serviceOperation;
		private readonly DataContractSerializer requestSerializer = new DataContractSerializer(typeof(TRequest));
		private readonly DataContractSerializer responseSerializer = new DataContractSerializer(typeof(TResponse));
		private readonly QueryStringDeserializer<TRequest> queryStringDeserializer = new QueryStringDeserializer<TRequest>();

		public XmlRequestHandler(IServiceOperation<TRequest, TResponse> serviceOperation)
		{
			this.serviceOperation = serviceOperation;
		}

		public void Process(HttpListenerContext httpListenerContext)
		{
			try
			{
				TRequest request;

				var httpMethod = httpListenerContext.Request.HttpMethod.ToUpper();
				
				if (httpMethod == "GET" || httpMethod == "OPTIONS")
				{
					request = this.queryStringDeserializer.Deserialize(httpListenerContext.Request.QueryString);
				}
				else
				{
					request = (TRequest) this.requestSerializer.ReadObject(httpListenerContext.Request.InputStream);
				}

				var response = this.serviceOperation.Process(request);

				using (var responseStream = httpListenerContext.Response.OutputStream)
				{
					this.responseSerializer.WriteObject(responseStream, response);
				}
			}
			catch (ServiceException ex)
			{
				this.WriteError(httpListenerContext, ex.ErrorCode, ex.Message, ex.StackTrace);
			}
			catch (Exception ex)
			{
				this.WriteError(httpListenerContext, ex.GetType().Name, ex.Message, ex.StackTrace);
			}
		}

		public void WriteError(HttpListenerContext httpListenerContext, string errorCode, string errorMessage, string stackTrace)
		{
			var response = new TResponse
      	{
      		Error = new Error
	        	{
	        		Code = errorCode,
	        		Message = errorMessage,
	        		StackTrace = stackTrace,
	        	}
      	};

			using (var responseStream = httpListenerContext.Response.OutputStream)
			{
				this.responseSerializer.WriteObject(responseStream, response);
			}
		}
	}
}