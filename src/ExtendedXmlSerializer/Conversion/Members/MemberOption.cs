using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class MemberOption : MemberOptionBase
	{
		readonly IGetterFactory _getter;
		readonly ISetterFactory _setter;
		readonly IConverters _converters;

		public MemberOption(IConverters converters, IMemberAdorner adorner) : this(converters, adorner, GetterFactory.Default, SetterFactory.Default) {}

		public MemberOption(IConverters converters, IMemberAdorner adorner, IGetterFactory getter, ISetterFactory setter)
			: base(new DelegatedSpecification<MemberInformation>(x => x.Assignable), adorner)
		{
			_getter = getter;
			_setter = setter;
			_converters = converters;
		}

		protected override IMember Create(IElement element, MemberInfo metadata)
		{
			var getter = _getter.Get(metadata);
			var setter = _setter.Get(metadata);
			var body = _converters.Get(element.Classification);
			var result = new Member(element, setter, getter, body);
			return result;
		}
	}
}