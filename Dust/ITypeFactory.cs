namespace Dust
{
	public interface ITypeFactory
	{
		T Resolve<T>();
		T Resolve<T>(string named);
	}
}