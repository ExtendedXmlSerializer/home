using ExtendedXmlSerialization.Conversion.Model;
using ExtendedXmlSerialization.Conversion.Model.Names;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public class MemberNameProvider : IParameterizedSource<MemberInformation, IMemberName>
	{
		readonly IAliasProvider _alias;
		public static MemberNameProvider Default { get; } = new MemberNameProvider();
		MemberNameProvider() : this(MemberAliasProvider.Default) {}

		public MemberNameProvider(IAliasProvider alias)
		{
			_alias = alias;
		}

		public IMemberName Get(MemberInformation parameter)
			=>
				new MemberName(_alias.Get(parameter.Metadata) ?? parameter.Metadata.Name, parameter.Metadata, parameter.MemberType);
	}
}