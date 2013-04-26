using System;
using System.Collections.Generic;
using System.Linq;

namespace Dust
{
	public class ServiceOperationProxy<TServiceOperation, TRequest, TResponse> : IServiceOperation<TRequest, TResponse>
		where TServiceOperation : class, IServiceOperation<TRequest, TResponse>
	{
		private readonly Func<TRequest, TResponse> serviceOperation;

		public ServiceOperationProxy(ITypeFactory typeFactory, IServiceAspectFactory serviceAspectFactory)
		{
			var serviceOperationType = typeof(TServiceOperation);
			var serviceAspects = new List<IServiceAspect<TRequest, TResponse>>();

			var serviceAspectAttributes = serviceOperationType.GetAttributes<ServiceAspectAttribute>().ToList();

			foreach (var serviceAspectAttribute in serviceAspectAttributes)
			{
				var aspectName = serviceAspectAttribute.AspectName;

				var serviceAspect = serviceAspectFactory.CreateServiceAspect<TRequest, TResponse>(aspectName);

				if (serviceAspect != null)
				{
					serviceAspects.Add(serviceAspect);
				}
			}

			this.serviceOperation = request =>
			{
				var localServiceOperation = typeFactory.Resolve<TServiceOperation>();

				if (localServiceOperation == null)
				{
					var log = string.Format("The ServiceOperation '{0}' cannot be resolved", typeof(TServiceOperation).Name);
					throw new NotSupportedException(log);
				}
				
				var response = localServiceOperation.Process(request);

				return response;
			};

			foreach (var serviceAspect in serviceAspects)
			{
				var localServiceAspect = serviceAspect;
				var localServiceOperation = this.serviceOperation;

				this.serviceOperation = request =>
				{
					var response = localServiceAspect.Process(request, localServiceOperation);

					return response;
				};
			}
		}

		public TResponse Process(TRequest request)
		{
			return this.serviceOperation(request);
		}
	}
}