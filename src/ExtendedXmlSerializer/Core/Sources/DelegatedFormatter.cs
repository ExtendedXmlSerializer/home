using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class DelegatedFormatter<T> : IFormatter<T>
	{
		readonly Func<T, string> _source;

		public DelegatedFormatter(Func<T, string> source) => _source = source;

		public string Get(T parameter) => _source(parameter);
	}
}