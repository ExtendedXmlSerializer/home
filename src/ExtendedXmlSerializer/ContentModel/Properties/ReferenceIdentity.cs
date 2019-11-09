using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class ReferenceIdentity : ConverterProperty<uint?>
	{
		public static ReferenceIdentity Default { get; } = new ReferenceIdentity();

		ReferenceIdentity() : base(UnsignedIntegerConverter.Default.Structured<uint>(),
		                           new FrameworkIdentity("reference")) {}
	}
}