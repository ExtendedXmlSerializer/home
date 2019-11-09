using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class MigrationVersionIdentity : ConverterProperty<uint>
	{
		public static MigrationVersionIdentity Default { get; } = new MigrationVersionIdentity();

		MigrationVersionIdentity() : base(UnsignedIntegerConverter.Default, new FrameworkIdentity("version")) {}
	}
}