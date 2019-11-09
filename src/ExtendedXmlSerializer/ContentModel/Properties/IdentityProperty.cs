using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class IdentityProperty : ConverterProperty<uint?>
	{
		public static IdentityProperty Default { get; } = new IdentityProperty();

		IdentityProperty() : base(UnsignedIntegerConverter.Default.Structured<uint>(),
		                          new FrameworkIdentity("identity")) {}
	}
}