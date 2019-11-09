using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class DefaultValues<T> : DelegatedSource<object>
	{
		public static DefaultValues<T> Default { get; } = new DefaultValues<T>();

		DefaultValues() : base(() => default(T)) {}
	}
}