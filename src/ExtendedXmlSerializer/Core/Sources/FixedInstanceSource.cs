namespace ExtendedXmlSerializer.Core.Sources
{
	/// <exclude />
	public class FixedInstanceSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly TResult _instance;

		/// <exclude />
		public FixedInstanceSource(TResult instance) => _instance = instance;

		/// <inheritdoc />
		public TResult Get(TParameter parameter) => _instance;
	}

	/// <exclude />
	public class FixedInstanceSource<T> : ISource<T>
	{
		readonly T _instance;

		/// <exclude />
		public FixedInstanceSource(T instance) => _instance = instance;

		/// <inheritdoc />
		public T Get() => _instance;
	}
}