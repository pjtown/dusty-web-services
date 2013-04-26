using System;

namespace Dust.Web.Tests.Data
{
	public class TestServiceOperation : IServiceOperation<TestRequest, TestResponse>
	{
		public TestResponse Process(TestRequest request)
		{
			Console.WriteLine("Processed TestRequest : {0}", request.Question);

			return new TestResponse
			{
				Answer = "Dont know",
			};
		}
	}
}