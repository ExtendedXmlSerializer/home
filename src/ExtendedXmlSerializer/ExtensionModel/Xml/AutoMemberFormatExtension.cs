using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class AutoMemberFormatExtension : ISerializerExtension
	{
		public static AutoMemberFormatExtension Default { get; } = new AutoMemberFormatExtension();

		AutoMemberFormatExtension() : this(128) {}

		readonly IAttributeSpecification _text;

		public AutoMemberFormatExtension(int maxTextLength)
			: this(new AttributeSpecification(new TextSpecification(maxTextLength).Adapt())) {}

		public AutoMemberFormatExtension(IAttributeSpecification text) => _text = text;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IMemberConverters, MemberConverters>()
			            .Decorate<IAttributeSpecifications>(Decorate);

		IAttributeSpecifications Decorate(IServiceProvider provider, IAttributeSpecifications defaults)
			=> new AttributeSpecifications(provider.Get<ConverterSpecification>(), _text, defaults);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class TextSpecification : ISpecification<string>
		{
			readonly int _maxTextLength;

			public TextSpecification(int maxTextLength) => _maxTextLength = maxTextLength;

			public bool IsSatisfiedBy(string parameter) => parameter.Length < _maxTextLength;
		}

		sealed class MemberConverters : IMemberConverters
		{
			readonly IMemberConverters _members;
			readonly IConverters       _converters;

			public MemberConverters(IMemberConverters members, IConverters converters)
			{
				_members    = members;
				_converters = converters;
			}

			public IConverter Get(MemberInfo parameter) => _members.Get(parameter) ?? From(parameter);

			IConverter From(MemberDescriptor parameter) => _converters.Get(parameter.MemberType.AccountForNullable());
		}
	}
}