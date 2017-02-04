using System;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public abstract class MemberOptionBase : OptionBase<MemberInformation, IMember>, IMemberOption
	{
		readonly IConverters _converters;
		readonly IAliasProvider _alias;
		readonly IGetterFactory _getter;

		protected MemberOptionBase(ISpecification<MemberInformation> specification, IConverters converters,
		                           IAliasProvider alias)
			: this(specification, converters, alias, GetterFactory.Default) {}

		protected MemberOptionBase(ISpecification<MemberInformation> specification, IConverters converters,
		                           IAliasProvider alias, IGetterFactory getter
		)
			: base(specification)
		{
			_converters = converters;
			_alias = alias;
			_getter = getter;
		}

		public override IMember Get(MemberInformation parameter)
		{
			var getter = _getter.Get(parameter.Metadata);
			var body = _converters.Get(parameter.MemberType);
			var result = Create(_alias.Get(parameter.Metadata) ?? parameter.Metadata.Name, parameter.MemberType, getter, body,
			                    parameter.Metadata);
			return result;
		}

		protected abstract IMember Create(string displayName, TypeInfo classification, Func<object, object> getter,
		                                  IConverter body, MemberInfo metadata);
	}
}