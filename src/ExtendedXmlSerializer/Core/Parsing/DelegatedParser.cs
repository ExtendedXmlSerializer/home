using System;

namespace ExtendedXmlSerializer.Core.Parsing
{
	sealed class DelegatedParser<T> : IParser<T>
	{
		readonly Func<string, T> _source;

		public DelegatedParser(Func<string, T> source) => _source = source;

		public T Get(string parameter) => _source(parameter);
	}
}