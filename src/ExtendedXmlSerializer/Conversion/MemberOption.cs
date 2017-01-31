using ExtendedXmlSerialization.Conversion.Model;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	public class MemberOption : MemberOptionBase
	{
		readonly IGetterFactory _getter;
		readonly ISetterFactory _setter;
		readonly IContexts _contexts;

		public MemberOption(IContexts contexts) : this(contexts, GetterFactory.Default, SetterFactory.Default) {}

		public MemberOption(IContexts contexts, IGetterFactory getter, ISetterFactory setter)
			: base(new DelegatedSpecification<MemberInformation>(x => x.Assignable))
		{
			_getter = getter;
			_setter = setter;
			_contexts = contexts;
		}

		protected override IMemberContext Create(IMemberName name)
		{
			var metadata = name.Metadata;
			var getter = _getter.Get(metadata);
			var setter = _setter.Get(metadata);
			var result = new MemberContext(name, _contexts.Get(name.MemberType), setter, getter);
			return result;
		}
	}
}