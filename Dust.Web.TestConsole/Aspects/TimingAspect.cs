using System;
using System.Diagnostics;

namespace Dust.Web.TestConsole.Aspects
{
	[ServiceAspectName("Timing")]
	public class TimingAspect<TRequest, TResponse> : IServiceAspect<TRequest, TResponse>
	{
		public TResponse Process(TRequest request, Func<TRequest, TResponse> serviceOperation)
		{
			var stopwatch = new Stopwatch();

			Console.WriteLine("Operation timer started");

			stopwatch.Start();

			var response = serviceOperation(request);

			stopwatch.Stop();

			Console.WriteLine("Operation timer stopped: {0}ms", stopwatch.ElapsedMilliseconds);

			return response;
		}
	}
}