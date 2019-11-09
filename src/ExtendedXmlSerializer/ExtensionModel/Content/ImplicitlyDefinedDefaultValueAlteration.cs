using System;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ImplicitlyDefinedDefaultValueAlteration : IAlteration<IConverter>
	{
		public static ImplicitlyDefinedDefaultValueAlteration Default { get; } =
			new ImplicitlyDefinedDefaultValueAlteration();

		ImplicitlyDefinedDefaultValueAlteration() {}

		public IConverter Get(IConverter parameter)
		{
			var @default = TypeDefaults.Default.Get(parameter.Get());
			var parser   = new Parser(parameter.Parse, @default);
			var result   = new Converter<object>(parameter, parser.Get, parameter.Format);
			return result;
		}

		sealed class Parser : IParser<object>
		{
			readonly Func<string, object> _parser;
			readonly object               _defaultValue;

			public Parser(Func<string, object> parser, object defaultValue)
			{
				_parser       = parser;
				_defaultValue = defaultValue;
			}

			public object Get(string parameter)
			{
				try
				{
					return _parser(parameter);
				}
				catch (FormatException)
				{
					return _defaultValue;
				}
			}
		}
	}
}