using System.Runtime.Serialization;

namespace Dust
{
	public interface IResponse
	{
		Error Error { get; set; }
	}

	[DataContract(Namespace = "")]
	public class Error
	{
		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public string StackTrace { get; set; }
	}
}