using System;
using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class ArgumentsTypeProperty : Property<ImmutableArray<Type>>
	{
		public static ArgumentsTypeProperty Default { get; } = new ArgumentsTypeProperty();

		ArgumentsTypeProperty() : this(new FrameworkIdentity("arguments")) {}

		public ArgumentsTypeProperty(IIdentity identity)
			: base(new ContextualReader<ImmutableArray<Type>>(TypeArguments.Default.Get, identity),
			       new ContextualWriter<ImmutableArray<Type>>(TypeArguments.Default.Get, identity), identity) {}
	}
}