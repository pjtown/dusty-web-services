using System;
using Dust.Web.Tests.Data;

namespace Dust.Web.TestConsole.Aspects
{
	[ServiceAspectName("Session")]
	public class SessionAspect<TRequest, TResponse> : IServiceAspect<TRequest, TResponse>
		where TRequest : ISessionRequest
	{
		public TResponse Process(TRequest request, Func<TRequest, TResponse> serviceOperation)
		{
			Console.WriteLine("User {0} Session {1} validated", request.UserName, request.SessionId.ToString("N"));

			var response = serviceOperation(request);

			return response;
		}
	}
}