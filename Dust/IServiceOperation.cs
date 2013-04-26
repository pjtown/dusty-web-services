namespace Dust
{
	public interface IServiceOperation
	{
	}

	public interface IServiceOperation<TRequest, TResponse> : IServiceOperation
	{
		TResponse Process(TRequest request);
	}
}