using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dust
{
	public class ServiceOperationProfileLoader : IServiceOperationProfileLoader
	{
		private readonly ITypeFactory typeFactory;
		private readonly IServiceAspectFactory serviceAspectFactory;

		public ServiceOperationProfileLoader(ITypeFactory typeFactory, IServiceAspectFactory serviceAspectFactory)
		{
			this.typeFactory = typeFactory;
			this.serviceAspectFactory = serviceAspectFactory;
		}

		public IEnumerable<IServiceOperationProfile> LoadServiceOperationProfiles(Assembly serviceOperationAssembly)
		{
			var serviceOperationInterfaceDefinition = typeof(IServiceOperation<,>);

			foreach (var assemblyType in serviceOperationAssembly.GetTypes())
			{
				var serviceOperationInterfaceType = assemblyType.GetGenericInterfaceType(serviceOperationInterfaceDefinition);
				
				if (serviceOperationInterfaceType == null)
				{
					continue;
				}

				// Get service operation request and response types

				var genericArgumentTypes = serviceOperationInterfaceType.GetGenericArguments();

				var requestType = genericArgumentTypes[0];
				var responseType = genericArgumentTypes[1];

				var serviceOperationNameAttributes = assemblyType.GetAttributes<ServiceOperationNameAttribute>().ToList();

				var requestNames = serviceOperationNameAttributes.Count > 0 ? serviceOperationNameAttributes.Select(x => x.RequestName).ToList() : new List<string> {requestType.Name};

				if (requestNames.Any(x => string.IsNullOrEmpty(x)))
				{
					var log = string.Format("The ServiceOperation '{0}' has a ServiceOperationNameAttribute with a null or empty RequestName field", assemblyType.Name);
					throw new NotSupportedException(log);
				}

				// Create generic service operation type

				var serviceOperationProxyType = typeof(ServiceOperationProxy<,,>).MakeGenericType(assemblyType, requestType, responseType);

				var serviceOperationProxy = (IServiceOperation) Activator.CreateInstance(serviceOperationProxyType, this.typeFactory, this.serviceAspectFactory);

				var serviceOperationProfile = new ServiceOperationProfile
            {
					RequestNames = requestNames,
            	RequestType = requestType,
            	ResponseType = responseType,
					ServiceOperation = serviceOperationProxy,
            };

				yield return serviceOperationProfile;
			}
		}

		public IEnumerable<IServiceOperationProfile> LoadServiceOperationProfiles(IEnumerable<Assembly> serviceOperationAssemblies)
		{
			foreach (var serviceOperationProfile in serviceOperationAssemblies.SelectMany(x => LoadServiceOperationProfiles(x)))
			{
				yield return serviceOperationProfile;
			}
		}
	}
}