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
		readonly IMemberAdorner _adorner;

		protected MemberOptionBase(ISpecification<MemberInformation> specification, IMemberAdorner adorner)
			: this(specification, MemberAliasProvider.Default, adorner) {}

		protected MemberOptionBase(ISpecification<MemberInformation> specification, IAliasProvider alias,
		                           IMemberAdorner adorner) : base(specification)
		{
			_alias = alias;
			_adorner = adorner;
		}

		public override IMember Get(MemberInformation parameter)
		{
			var element = new Element(_alias.Get(parameter.Metadata) ?? parameter.Metadata.Name, parameter.MemberType);
			var adorned = _adorner.Get(element);
			var result = Create(adorned, parameter.Metadata);
			return result;
		}

		protected abstract IMember Create(IElement element, MemberInfo metadata);
	}
}