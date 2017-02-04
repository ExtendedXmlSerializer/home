using System;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class MemberOption : MemberOptionBase
	{
		readonly ISetterFactory _setter;

		public MemberOption(IConverters converters, IMemberElementProvider provider)
			: this(converters, provider, SetterFactory.Default) {}

		public MemberOption(IConverters converters, IMemberElementProvider provider, ISetterFactory setter)
			: base(new DelegatedSpecification<MemberInformation>(x => x.Assignable), converters, provider)
		{
			_setter = setter;
		}

		protected override IMember Create(IElement element, Func<object, object> getter, IConverter body, MemberInfo metadata)
			=> CreateMember(element, _setter.Get(metadata), getter, body);

		protected virtual IMember CreateMember(IElement element, Action<object, object> setter, Func<object, object> getter,
		                                       IConverter body) => new Member(element, setter, getter, body);
	}
}