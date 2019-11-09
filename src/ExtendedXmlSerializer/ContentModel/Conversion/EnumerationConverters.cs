using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class EnumerationConverters : ConditionalSource<TypeInfo, IConverter>, IConverters
	{
		public EnumerationConverters(IConverters converters) : base(IsAssignableSpecification<Enum>.Default,
		                                                            Converters.Default, converters) {}

		sealed class Converters : IConverters
		{
			public static Converters Default { get; } = new Converters();

			Converters() {}

			public IConverter Get(TypeInfo parameter) => new EnumerationConverter(parameter.AsType());
		}
	}
}