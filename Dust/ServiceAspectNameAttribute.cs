using System;

namespace Dust
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ServiceAspectNameAttribute : Attribute
	{
		public ServiceAspectNameAttribute(string aspectName)
		{
			this.AspectName = aspectName;
		}

		public string AspectName { get; set; }
	}
}