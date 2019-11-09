using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	class ConverterProperty<T> : IProperty<T>
	{
		readonly IProperty<T> _property;

		public ConverterProperty(IConvert<T> converter, IIdentity identity)
			: this(new DelegatedProperty<T>(converter.Parse, converter.Format, identity)) {}

		public ConverterProperty(IProperty<T> property) => _property = property;

		public T Get(IFormatReader parameter) => _property.Get(parameter);

		public void Write(IFormatWriter writer, T instance) => _property.Write(writer, instance);

		public string Identifier => _property.Identifier;

		public string Name => _property.Name;
	}
}