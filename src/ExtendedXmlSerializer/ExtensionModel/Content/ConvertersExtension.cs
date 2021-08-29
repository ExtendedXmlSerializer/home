using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	/// <summary>
	/// A default serializer extension that manages the registration of converters used by generated serializers.
	/// </summary>
	public sealed class ConvertersExtension : ISerializerExtension
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		public ConvertersExtension() : this(WellKnownConverters.Default.KeyedByType()) {}


		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExtendedXmlSerializer.ExtensionModel.Content.ConvertersExtension"/> class.
		/// </summary>
		/// <param name="converters">The converters.</param>
		public ConvertersExtension(ICollection<IConverter> converters) => Converters = converters;

		/// <summary>
		/// Current registry of converters.
		/// </summary>
		public ICollection<IConverter> Converters { get; }

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(Converters.ToArray())
			            .Register<IConverters, Converters>()
			            .Decorate<IConverters, EnumerationConverters>()
			            .Register<ISerializers, Serializers>()
			            .Register<ConverterContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}