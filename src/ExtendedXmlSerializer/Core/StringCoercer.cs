using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core
{
	sealed class StringCoercer<T> : DelegatedSource<T, string>
	{
		public static StringCoercer<T> Default { get; } = new StringCoercer<T>();
		StringCoercer() : base(x => x.ToString()) {}
	}
}
