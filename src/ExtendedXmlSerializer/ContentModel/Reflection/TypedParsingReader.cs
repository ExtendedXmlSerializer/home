using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	class TypedParsingReader : IReader<TypeInfo>
	{
		readonly IReader<MemberInfo> _reader;

		public TypedParsingReader(IIdentity identity) : this(new MemberParsedReader(identity)) {}

		public TypedParsingReader(IReader<MemberInfo> reader) => _reader = reader;

		public TypeInfo Get(IFormatReader parameter) => _reader.Get(parameter)
		                                                       ?.AsValid<TypeInfo>();
	}
}