using System;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class MemberOption : MemberOptionBase
	{
		readonly ISpecification<TypeInfo> _specification;

		readonly ISetterFactory _setter;

		public MemberOption(IConverters converters)
			: this(FixedTypeSpecification.Default, converters, MemberAliasProvider.Default, SetterFactory.Default) {}

		public MemberOption(ISpecification<TypeInfo> specification, IConverters converters, IAliasProvider alias,
		                    ISetterFactory setter)
			: base(new DelegatedSpecification<MemberInformation>(x => x.Assignable), converters, alias)
		{
			_specification = specification;
			_setter = setter;
		}

		protected override IMember Create(string displayName, TypeInfo classification, Func<object, object> getter,
		                                  IConverter body, MemberInfo metadata)
			=> CreateMember(displayName, classification, _setter.Get(metadata), getter, body);

		protected virtual IMember CreateMember(string displayName, TypeInfo classification, Action<object, object> setter,
		                                       Func<object, object> getter, IConverter body)
		{
			var start = _specification.IsSatisfiedBy(classification)
				? new StartMember(displayName)
				: new StartVariableTypedMember(displayName, classification.AsType());
			var result = new Member(displayName, start, setter, getter, body);
			return result;
		}
	}
}