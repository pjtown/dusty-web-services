using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dust
{
	public class ServiceAspectFactory : IServiceAspectFactory
	{
		private readonly ITypeFactory typeFactory;
		private readonly Dictionary<string, Type> serviceAspectDefinitionTypesByAspectName = new Dictionary<string, Type>();

		public ServiceAspectFactory(ITypeFactory typeFactory)
		{
			this.typeFactory = typeFactory;
		}

		public void Register(Assembly serviceAspectAssembly)
		{
			foreach (var assemblyType in serviceAspectAssembly.GetTypes())
			{
				var interfaceType = assemblyType.GetGenericInterfaceType(typeof(IServiceAspect<,>));

				if (interfaceType == null)
				{
					continue;
				}

				if (!assemblyType.IsGenericTypeDefinition)
				{
					var log = string.Format("The ServiceAspect '{0}' must have generic TRequest and TResponse parameters", assemblyType.Name);
					throw new NotSupportedException(log);
				}

				if (assemblyType.GetGenericArguments().Length != 2)
				{
					var log = string.Format("The ServiceAspect '{0}' must have generic TRequest and TResponse parameters only", assemblyType.Name);
					throw new NotSupportedException(log);
				}

				var serviceAspectNameAttribute = assemblyType.GetAttribute<ServiceAspectNameAttribute>();

				if (serviceAspectNameAttribute == null)
				{
					var log = string.Format("The ServiceAspect '{0}' must define a ServiceAspectNameAttribute", assemblyType.Name);
					throw new NotSupportedException(log);
				}

				var aspectName = serviceAspectNameAttribute.AspectName;

				if (this.serviceAspectDefinitionTypesByAspectName.ContainsKey(aspectName))
				{
					var log = string.Format("The ServiceAspect '{0}' has an AspectName '{1}' that has already been loaded", assemblyType.Name, aspectName);
					throw new NotSupportedException(log);
				}

				this.serviceAspectDefinitionTypesByAspectName[aspectName] = assemblyType;
			}
		}

		public void Register(IEnumerable<Assembly> serviceAspectAssemblies)
		{
			foreach (var serviceAspectAssembly in serviceAspectAssemblies)
			{
				this.Register(serviceAspectAssembly);
			}
		}

		public IServiceAspect<TRequest, TResponse> CreateServiceAspect<TRequest, TResponse>(string aspectName)
		{
			Type serviceAspectDefinitionType;

			if (!this.serviceAspectDefinitionTypesByAspectName.TryGetValue(aspectName, out serviceAspectDefinitionType))
			{
				var log = string.Format("The AspectName '{0}' does not correspond to a loaded ServiceAspect", aspectName);
				throw new NotSupportedException(log);
			}

			var requestType = typeof(TRequest);
			var responseType = typeof(TResponse);

			var serviceAspectType = serviceAspectDefinitionType.MakeGenericType(requestType, responseType);
			var serviceAspectResolverType = typeof(ServiceAspectResolver<,,>).MakeGenericType(serviceAspectType, requestType, responseType);
			var serviceAspectResolver = (IServiceAspectResolver<TRequest, TResponse>) Activator.CreateInstance(serviceAspectResolverType, this.typeFactory);

			var serviceAspect = serviceAspectResolver.Resolve();

			return serviceAspect;
		}
	}
}