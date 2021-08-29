using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class EnumerationConverters : ConditionalSource<TypeInfo, IConverter>, IConverters
	{
		public EnumerationConverters(IConverters previous) : this(previous, Converters.Default) {}

		public EnumerationConverters(IConverters previous, IConverters converters)
			: base(IsAssignableSpecification<Enum>.Default, converters, previous) {}

		sealed class Converters : IConverters
		{
			public static Converters Default { get; } = new Converters();

			Converters() {}

			public IConverter Get(TypeInfo parameter) => new EnumerationConverter(parameter.AsType());
		}
	}
}