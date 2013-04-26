using System;
using Dust.Web.Tests.Data;

namespace Dust.Web.TestConsole.Data
{
	public class TestServiceOperation : IServiceOperation<TestRequest, TestResponse>
	{
		public TestResponse Process(TestRequest request)
		{
			Console.WriteLine("Processing TestRequest : {0} [{1}]", request.Question, request.Count);

			return new TestResponse
			       {
			       	Answer = "The answer is 42",
			       };
		}
	}

	[ServiceAspect("Timing")]
	[ServiceAspect("Session")]
	[ServiceOperationName("Testing")]
	public class UnknownTestServiceOperation : IServiceOperation<TestRequest, TestResponse>
	{
		public TestResponse Process(TestRequest request)
		{
			Console.WriteLine("Processing TestRequest : {0} [{1}]", request.Question, request.Count);

			return new TestResponse
			{
				Answer = "Dont know",
			};
		}
	}

	[ServiceOperationName("Testing2")]
	public class FailingTestServiceOperation : IServiceOperation<TestRequest, TestResponse>
	{
		public TestResponse Process(TestRequest request)
		{
			Console.WriteLine("Processing TestRequest : {0} [{1}]", request.Question, request.Count);

			throw new InvalidOperationException("This service operation ALWAYS fails!");
		}
	}
}