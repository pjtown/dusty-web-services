using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Dust.Web
{
	public class HttpRequestListener : IDisposable
	{
		private readonly static Regex ServiceOperationUrlRegex = new Regex(@".*/([^/]+)/([^/?]+)(.*)?$", RegexOptions.Compiled);

		private readonly IRequestRouter xmlRequestRouter;
		private HttpListener httpListener;

		public HttpRequestListener(string uriPrefix, IRequestRouter xmlRequestRouter)
		{
			this.xmlRequestRouter = xmlRequestRouter;

			this.httpListener = new HttpListener();
			this.httpListener.Prefixes.Add(uriPrefix);

			this.httpListener.Start();

			this.httpListener.BeginGetContext(this.GetContextCallback, null);
		}

		private void GetContextCallback(IAsyncResult asyncResult)
		{
			if (this.httpListener == null)
			{
				return;
			}

			var httpListenerContext = this.httpListener.EndGetContext(asyncResult);
			
			this.httpListener.BeginGetContext(this.GetContextCallback, null);

			var urlMatch = ServiceOperationUrlRegex.Match(httpListenerContext.Request.RawUrl);

			if (!urlMatch.Success)
			{
				// Default error to XML response format
				this.xmlRequestRouter.WriteError(httpListenerContext, "InvalidUrlFormat", "Url should be formatted '/format/operation[?querystring]'", new StackTrace().ToString());
			}

			var requestFormat = urlMatch.Groups[1].Value.ToLower();
			var requestName = urlMatch.Groups[2].Value;

			if (requestFormat == "xml")
			{
				this.xmlRequestRouter.Process(httpListenerContext, requestName);
			}
			else
			{
				// Default error to XML response format
				this.xmlRequestRouter.WriteError(httpListenerContext, "InvalidRequestFormat", "Unknown request format " + requestFormat, new StackTrace().ToString());
			}
		}

		public void Dispose()
		{
			this.httpListener.Stop();
			this.httpListener.Close();
			this.httpListener = null;
		}
	}
}