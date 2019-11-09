namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class Self<T> : IAlteration<T>
	{
		public static Self<T> Default { get; } = new Self<T>();

		Self() {}

		public T Get(T parameter) => parameter;
	}
}