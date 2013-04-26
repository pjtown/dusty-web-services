using System.Net;

namespace Dust.Web
{
	public interface IRequestHandler
	{
		void Process(HttpListenerContext httpListenerContext);
	}
}