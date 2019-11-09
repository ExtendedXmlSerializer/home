using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class EntityIdentity : ConverterProperty<string>
	{
		public static EntityIdentity Default { get; } = new EntityIdentity();

		EntityIdentity() : base(StringConverter.Default, new FrameworkIdentity("entity")) {}
	}
}