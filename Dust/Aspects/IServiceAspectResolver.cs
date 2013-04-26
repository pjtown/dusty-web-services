namespace Dust
{
	public interface IServiceAspectResolver<TRequest, TResponse>
	{
		IServiceAspect<TRequest, TResponse> Resolve();
	}
}