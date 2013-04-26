using System.Linq;
using System.Reflection;
using Dust.Web.Tests.StructureMap;
using Dust.Web.Xml;
using NUnit.Framework;
using StructureMap;

namespace Dust.Web.Tests
{
	[TestFixture]
	public class Sandbox
	{
		[SetUp]
		public void SetUp()
		{
			ObjectFactory.Initialize(x =>
			{
				x.IgnoreStructureMapConfig = true;
			});
		}

		[Test]
		public void Test()
		{
			var typeFactory = new StructureMapServiceTypeFactory();

			var serviceAspectFactory = new ServiceAspectFactory(typeFactory);
			
			serviceAspectFactory.Register(Assembly.GetExecutingAssembly());

			var serviceOperationProfileLoader = new ServiceOperationProfileLoader(typeFactory, serviceAspectFactory);

			var xmlRequestRouter = new XmlRequestRouter(serviceOperationProfileLoader);

			xmlRequestRouter.Register(Assembly.GetExecutingAssembly());
		}
	}
}