namespace ExtendedXmlSerializer.Core.Sources
{
	public class FixedInstanceSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly TResult _instance;

		public FixedInstanceSource(TResult instance) => _instance = instance;

		public TResult Get(TParameter parameter) => _instance;
	}

	public class FixedInstanceSource<T> : ISource<T>
	{
		readonly T _instance;

		public FixedInstanceSource(T instance) => _instance = instance;

		public T Get() => _instance;
	}
}