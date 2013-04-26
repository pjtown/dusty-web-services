using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Dust.Web.Tests.Data;
using Dust.Web.Tests.StructureMap;
using Dust.Web.Xml;
using StructureMap;

namespace Dust.Web.TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			ObjectFactory.Initialize(x =>
			{
				x.IgnoreStructureMapConfig = true;
			});

			var typeFactory = new StructureMapServiceTypeFactory();

			var serviceAspectFactory = new ServiceAspectFactory(typeFactory);
			serviceAspectFactory.Register(Assembly.GetExecutingAssembly());

			var serviceOperationProfileLoader = new ServiceOperationProfileLoader(typeFactory, serviceAspectFactory);

			var xmlRequestRouter = new XmlRequestRouter(serviceOperationProfileLoader);

			xmlRequestRouter.Register(Assembly.GetExecutingAssembly());

			using (var listener = new HttpRequestListener("http://localhost/Public/Xml/", xmlRequestRouter))
			{
				var choice = 0;
				var sessionId = Guid.NewGuid();
				var requestNames = new[] { "TestRequest", "Testing", "Testing2" };

				Console.WriteLine("Press space to send request");

				while (true)
				{
					var key = Console.ReadKey();

					if (key.Key == ConsoleKey.Spacebar)
					{
						var requestName = requestNames[choice++ % requestNames.Length]; 
						var testRequest = new TestRequest { UserName = "peter", SessionId = sessionId, Question = "Is there anybody out there?" };

						Console.WriteLine();
						Console.WriteLine("Request = {0}", testRequest.Question);

						var testResponse = SendRequest(requestName, testRequest);

						if (testResponse.Error == null)
						{
							Console.WriteLine("Response = {0}", testResponse.Answer);
						}
						else
						{
							Console.WriteLine("Error : {0} {1}", testResponse.Error.Code, testResponse.Error.Message);
							Console.WriteLine("Error StackTrace:");
							Console.WriteLine(testResponse.Error.StackTrace);
						}
					}
					else
					{
						break;
					}
				}
			}
		}

		private static TestResponse SendRequest(string requestName, TestRequest testRequest)
		{
			var requestSerializer = new DataContractSerializer(typeof(TestRequest));
			var responseSerializer = new DataContractSerializer(typeof(TestResponse));

			var request = WebRequest.Create("http://localhost/Public/Xml/" + requestName);

			request.Method = "POST";
			request.ContentType = "application/xml";

			using (var requestStream = request.GetRequestStream())
			{
				requestSerializer.WriteObject(requestStream, testRequest);
			}

			// Get the response.

			using (var response = request.GetResponse())
			using (var responseStream = response.GetResponseStream())
			{
				var testResponse = (TestResponse) responseSerializer.ReadObject(responseStream);
				return testResponse;
			}
		}
	}
}
