using StructureMap;

namespace Dust.Web.Tests.StructureMap
{
	public class StructureMapServiceTypeFactory : ITypeFactory
	{
		public T Resolve<T>()
		{
			return ObjectFactory.GetInstance<T>();
		}

		public T Resolve<T>(string named)
		{
			return ObjectFactory.GetNamedInstance<T>(named);
		}
	}
}