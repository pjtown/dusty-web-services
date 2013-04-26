using System;
using System.Collections.Generic;

namespace Dust
{
	public class ServiceOperationProfile : IServiceOperationProfile
	{
		public List<string> RequestNames { get; set; }
		public Type RequestType { get; set; }
		public Type ResponseType { get; set; }
		public IServiceOperation ServiceOperation { get; set; }
	}
}