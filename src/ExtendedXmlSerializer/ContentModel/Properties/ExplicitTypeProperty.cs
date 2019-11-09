using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class ExplicitTypeProperty : Property<TypeInfo>
	{
		public static ExplicitTypeProperty Default { get; } = new ExplicitTypeProperty();

		ExplicitTypeProperty() : this(new FrameworkIdentity("type")) {}

		public ExplicitTypeProperty(IIdentity identity)
			: base(new TypedParsingReader(identity), new TypedFormattingWriter(identity), identity) {}
	}
}