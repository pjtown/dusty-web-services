using System;

namespace Dust
{
	public class ServiceAspectResolver<TServiceAspect, TRequest, TResponse> : IServiceAspectResolver<TRequest, TResponse>
		where TServiceAspect : class, IServiceAspect<TRequest, TResponse>
	{
		private readonly ITypeFactory typeFactory;

		public ServiceAspectResolver(ITypeFactory typeFactory)
		{
			this.typeFactory = typeFactory;
		}

		public IServiceAspect<TRequest, TResponse> Resolve()
		{
			var serviceAspect = this.typeFactory.Resolve<TServiceAspect>();

			if (serviceAspect == null)
			{
				var log = string.Format("The ServiceAspect '{0}' cannot be resolved", typeof(TServiceAspect).Name);
				throw new NotSupportedException(log);
			}

			return serviceAspect;
		}
	}
}