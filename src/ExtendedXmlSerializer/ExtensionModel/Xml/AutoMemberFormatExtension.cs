using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using System;
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
			            .Decorate<IAttributeSpecifications>(Decorate)
			            .Decorate<IConverters, Converters>()
			            .DecorateContentsWith<NullableStructureAwareContents>()
			            .Then();

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

			IConverter From(MemberDescriptor parameter) => _converters.Get(parameter.MemberType);
		}

		sealed class NullableStructureAwareContents : IContents
		{
			readonly IContents _previous;

			public NullableStructureAwareContents(IContents previous) => _previous = previous;

			public ContentModel.ISerializer Get(TypeInfo parameter)
			{
				var serializer = _previous.Get(parameter);
				var result = Nullable.GetUnderlyingType(parameter) != null
					             ? new ContentModel.Serializer(new Reader(serializer), serializer)
					             : serializer;
				return result;
			}
		}

		sealed class Reader : IReader
		{
			readonly IReader _previous;

			public Reader(IReader previous) => _previous = previous;

			public object Get(IFormatReader parameter)
			{
				try
				{
					return _previous.Get(parameter);
				}
				catch (FormatException)
				{
					return null;
				}
			}
		}

		sealed class Converters : IConverters
		{
			readonly IConverters _previous;

			public Converters(IConverters previous) => _previous = previous;

			public IConverter Get(TypeInfo parameter)
			{
				var underlying = Nullable.GetUnderlyingType(parameter);
				var decorate   = underlying != null;
				var converter  = (decorate ? _previous.Get(underlying) : null) ?? _previous.Get(parameter);
				var result     = decorate && converter != null ? new Converter(converter) : converter;
				return result;
			}
		}

		sealed class Converter : IConverter
		{
			readonly IConverter _converter;

			public Converter(IConverter converter) => _converter = converter;

			public bool IsSatisfiedBy(TypeInfo parameter) => _converter.IsSatisfiedBy(parameter);

			public object Parse(string data) => !string.IsNullOrEmpty(data) ? _converter.Parse(data) : null;

			public string Format(object instance) => _converter.Format(instance);

			public TypeInfo Get() => _converter.Get();
		}
	}
}