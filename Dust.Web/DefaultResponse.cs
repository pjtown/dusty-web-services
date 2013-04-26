using System.Runtime.Serialization;

namespace Dust.Web
{
	[DataContract]
	public class DefaultResponse : IResponse
	{
		[DataMember]
		public Error Error { get; set; }
	}
}