using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class MemberOption : MemberOptionBase
	{
		readonly IGetterFactory _getter;
		readonly ISetterFactory _setter;
		readonly IConverters _converters;

		public MemberOption(IConverters converters) : this(converters, GetterFactory.Default, SetterFactory.Default) {}

		public MemberOption(IConverters converters, IGetterFactory getter, ISetterFactory setter)
			: base(new DelegatedSpecification<MemberInformation>(x => x.Assignable))
		{
			_getter = getter;
			_setter = setter;
			_converters = converters;
		}

		protected override IMember Create(string displayName, TypeInfo classification, MemberInfo metadata)
		{
			var getter = _getter.Get(metadata);
			var setter = _setter.Get(metadata);
			var result = new Member(displayName, classification, setter, getter, _converters.Get(classification));
			return result;
		}
	}
}