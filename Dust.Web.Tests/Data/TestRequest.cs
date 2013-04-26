using System;
using System.Runtime.Serialization;

namespace Dust.Web.Tests.Data
{
	[DataContract]
	public class TestRequest : ISessionRequest
	{
		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public Guid SessionId { get; set; }

		[DataMember]
		public string Question { get; set; }

		[DataMember]
		public int Count { get; set; }
	}

	[DataContract]
	public class TestResponse : IResponse
	{
		[DataMember]
		public string Answer { get; set; }

		[DataMember]
		public Error Error { get; set; }
	}

	public interface ISessionRequest
	{
		string UserName { get; }
		
		Guid SessionId { get; }
	}
}