using System.Collections.Generic;
using System.Reflection;

namespace Dust
{
	public interface IServiceOperationProfileLoader
	{
		IEnumerable<IServiceOperationProfile> LoadServiceOperationProfiles(Assembly serviceOperationAssembly);
		IEnumerable<IServiceOperationProfile> LoadServiceOperationProfiles(IEnumerable<Assembly> serviceOperationAssemblies);
	}
}