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

		protected override IMemberConverter Create(IMemberName name)
		{
			var metadata = name.Metadata;
			var getter = _getter.Get(metadata);
			var setter = _setter.Get(metadata);
			var result = new MemberConverter(name, _converters.Get(name.MemberType), setter, getter);
			return result;
		}
	}
}