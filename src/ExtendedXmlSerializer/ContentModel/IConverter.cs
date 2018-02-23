using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ContentModel
{
	public interface IConverter<T>
	{
		T Parse(string parameter);

		string Format(T parameter);
	}

	class Converter<T> : DelegatedSource<T, string>, IConverter<T>
	{
		readonly Func<string, T> _parse;

		public Converter(Func<string, T> parse, Func<T, string> format) : base(format) => _parse = parse;

		public T Parse(string parameter) => _parse(parameter);

		public string Format(T parameter) => Get(parameter);
	}
}
