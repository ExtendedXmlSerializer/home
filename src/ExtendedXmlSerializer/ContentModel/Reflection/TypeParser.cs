using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sources;
using Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeParser : CacheBase<string, TypeInfo>, IParser<TypeInfo>
	{
		readonly ITypePartReflector _reflector;
		readonly Parser<TypeParts>  _parser;

		public TypeParser(ITypePartReflector reflector) : this(reflector, TypePartsParser.Default) {}

		public TypeParser(ITypePartReflector reflector, Parser<TypeParts> parser)
		{
			_reflector = reflector;
			_parser    = parser;
		}

		protected override TypeInfo Create(string parameter) => _reflector.Get(_parser.Parse(parameter));
	}
}