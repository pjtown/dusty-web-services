using System;

namespace Dust
{
	public interface IServiceAspect<TRequest, TResponse>
	{
		TResponse Process(TRequest request, Func<TRequest, TResponse> serviceOperation);
	}
}