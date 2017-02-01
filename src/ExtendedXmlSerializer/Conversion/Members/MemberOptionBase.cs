using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public abstract class MemberOptionBase : OptionBase<MemberInformation, IMember>, IMemberOption
	{
		readonly IAliasProvider _alias;

		protected MemberOptionBase(ISpecification<MemberInformation> specification)
			: this(specification, MemberAliasProvider.Default) {}

		protected MemberOptionBase(ISpecification<MemberInformation> specification, IAliasProvider alias)
			: base(specification)
		{
			_alias = alias;
		}

		public override IMember Get(MemberInformation parameter) => Create(_alias.Get(parameter.Metadata) ?? parameter.Metadata.Name, parameter.MemberType, parameter.Metadata);

		protected abstract IMember Create(string displayName, TypeInfo classification, MemberInfo metadata);
	}
}