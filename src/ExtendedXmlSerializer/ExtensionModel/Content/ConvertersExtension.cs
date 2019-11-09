using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class ConvertersExtension : ISerializerExtension
	{
		public ConvertersExtension() : this(WellKnownConverters.Default.KeyedByType()) {}

		public ConvertersExtension(ICollection<IConverter> converters) => Converters = converters;

		public ICollection<IConverter> Converters { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(Converters.ToArray()
			                                        .Hide())
			            .Register<IConverters, Converters>()
			            .Decorate<IConverters, EnumerationConverters>()
			            .Register<ISerializers, Serializers>()
			            .Register<ConverterContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}