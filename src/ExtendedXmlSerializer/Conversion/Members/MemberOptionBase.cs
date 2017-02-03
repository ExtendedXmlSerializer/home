using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface IMemberAdorner : IAlteration<IElement> {}

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

		public override IMember Get(MemberInformation parameter)
		{
			var element = new Element(_alias.Get(parameter.Metadata) ?? parameter.Metadata.Name, parameter.MemberType);
			var result = Create(element, parameter.Metadata);
			return result;
		}

		protected abstract IMember Create(IElement element, MemberInfo metadata);
	}
}