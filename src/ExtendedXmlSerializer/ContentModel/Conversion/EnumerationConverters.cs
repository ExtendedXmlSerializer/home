using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class EnumerationConverters : ConditionalSource<TypeInfo, IConverter>, IConverters
	{
		public EnumerationConverters(IConverters converters)
			: base(IsAssignableSpecification<Enum>.Default, new Converters(converters.Get(typeof(int))), converters) {}

		sealed class Converters : IConverters
		{
			readonly IConverter _converter;

			public Converters(IConverter converter) => _converter = converter;

			public IConverter Get(TypeInfo parameter)
				=> parameter.GetCustomAttribute<FlagsAttribute>() != null
					   ? new Converter(parameter, _converter).Adapt()
					   : new EnumerationConverter(parameter.AsType());

			sealed class Converter : ConverterBase<Enum>
			{
				readonly Type       _type;
				readonly IConverter _converter;

				public Converter(Type type, IConverter converter)
				{
					_type      = type;
					_converter = converter;
				}

				public override Enum Parse(string data) => (Enum)Enum.Parse(_type, data);

				public override string Format(Enum instance) => _converter.Format((int)(object)instance);
			}
		}
	}
}