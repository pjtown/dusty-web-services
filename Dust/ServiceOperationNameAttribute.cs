using System;

namespace Dust
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class ServiceOperationNameAttribute : Attribute
	{
		public ServiceOperationNameAttribute(string requestName)
		{
			this.RequestName = requestName;
		}

		public string RequestName { get; set; }
	}
}