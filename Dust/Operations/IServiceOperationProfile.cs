using System;
using System.Collections.Generic;

namespace Dust
{
	public interface IServiceOperationProfile
	{
		List<string> RequestNames { get; set; }
		Type RequestType { get; }
		Type ResponseType { get; }
		IServiceOperation ServiceOperation { get; }
	}
}