using System;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface IMemberElementProvider : IParameterizedSource<MemberInformation, IElement> {}

	public abstract class MemberOptionBase : OptionBase<MemberInformation, IMember>, IMemberOption
	{
		readonly IConverters _converters;
		readonly IMemberElementProvider _provider;
		readonly IGetterFactory _getter;
		
		protected MemberOptionBase(ISpecification<MemberInformation> specification, IConverters converters,
		                           IMemberElementProvider provider)
			: this(specification, converters, provider, GetterFactory.Default) {}

		protected MemberOptionBase(ISpecification<MemberInformation> specification, IConverters converters,
		                           IMemberElementProvider provider,
		                           IGetterFactory getter
		)
			: base(specification)
		{
			_converters = converters;
			_provider = provider;
			_getter = getter;
		}

		public override IMember Get(MemberInformation parameter)
		{
			var getter = _getter.Get(parameter.Metadata);
			var element = _provider.Get(parameter);
			var body = _converters.Get(element.Classification);
			var result = Create(element, getter, body, parameter.Metadata);
			return result;
		}

		protected abstract IMember Create(IElement element, Func<object, object> getter, IConverter body, MemberInfo metadata);
	}
}