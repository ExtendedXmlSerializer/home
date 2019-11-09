using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class TypeArguments
		: IParameterizedSource<IFormatReader, ImmutableArray<Type>>,
		  IFormattedContent<ImmutableArray<Type>>
	{
		public static TypeArguments Default { get; } = new TypeArguments();

		TypeArguments() : this(TypePartsList.Default, TypePartsFormatter.Default.Get) {}

		readonly Parser<ImmutableArray<TypeParts>> _list;
		readonly Func<TypeParts, string>           _formatter;

		public TypeArguments(Parser<ImmutableArray<TypeParts>> list, Func<TypeParts, string> formatter)
		{
			_list      = list;
			_formatter = formatter;
		}

		public ImmutableArray<Type> Get(IFormatReader parameter) => _list.Parse(parameter.Content())
		                                                                 .Select(_formatter)
		                                                                 .Select(parameter.Get)
		                                                                 .Cast<TypeInfo>()
		                                                                 .Select(x => x.AsType())
		                                                                 .ToImmutableArray();

		public string Get(IFormatWriter writer, ImmutableArray<Type> instance)
			=> string.Join(",", Pack(writer, instance));

		static string[] Pack(IFormatWriter writer, ImmutableArray<Type> arguments)
		{
			var length = arguments.Length;
			var result = new string[length];
			for (var i = 0; i < length; i++)
			{
				result[i] = writer.Get(arguments[i]
					                       .GetTypeInfo());
			}

			return result;
		}
	}
}