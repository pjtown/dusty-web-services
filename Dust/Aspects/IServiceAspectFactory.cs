using System.Collections.Generic;
using System.Reflection;

namespace Dust
{
	public interface IServiceAspectFactory
	{
		void Register(Assembly serviceAspectAssembly);
		void Register(IEnumerable<Assembly> serviceAspectAssemblies);

		IServiceAspect<TRequest, TResponse> CreateServiceAspect<TRequest, TResponse>(string aspectName);
	}
}