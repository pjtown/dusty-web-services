using System;

namespace Dust
{
	public class ServiceException : Exception
	{
		public string ErrorCode { get; set; }
	}
}