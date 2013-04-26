using System;

namespace Dust
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ServiceAspectAttribute : Attribute
	{
		public ServiceAspectAttribute(string aspectName)
		{
			this.AspectName = aspectName;
		}

		public string AspectName { get; set; }
	}
}