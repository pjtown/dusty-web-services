using System.Net;

namespace Dust.Web
{
	public interface IRequestRouter
	{
		string RequestFormat { get; }

		void Process(HttpListenerContext httpListenerContext, string serviceOperationName);

		void WriteError(HttpListenerContext httpListenerContext, string errorCode, string errorMessage, string stackTrace);
	}
}