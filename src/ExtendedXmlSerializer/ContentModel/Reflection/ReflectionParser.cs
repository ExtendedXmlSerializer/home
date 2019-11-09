using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class ReflectionParser : CacheBase<string, MemberInfo>, IParser<MemberInfo>
	{
		readonly IParser<TypeInfo>   _types;
		readonly ITypePartReflector  _parts;
		readonly Parser<MemberParts> _parser;

		public ReflectionParser(IParser<TypeInfo> types, ITypePartReflector parts)
			: this(types, parts, MemberPartsParser.Default) {}

		public ReflectionParser(IParser<TypeInfo> types, ITypePartReflector parts, Parser<MemberParts> parser)
		{
			_types  = types;
			_parts  = parts;
			_parser = parser;
		}

		protected override MemberInfo Create(string parameter)
		{
			var parse = _parser.TryParse(parameter);
			if (parse.WasSuccessful)
			{
				var parts = parse.Value;
				var type  = _parts.Get(parts.Type);
				var name  = parts.MemberName;
				var result = type.GetMember(name)
				                 .Only() ??
				             type.GetProperty(name) ??
				             type.GetField(name) ??
				             type.GetMethod(name) ??
				             (MemberInfo)type.GetEvent(name);
				return result;
			}

			return _types.Get(parameter);
		}
	}
}