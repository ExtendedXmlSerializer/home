using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CustomSerializers : Metadata<TypeInfo, ContentModel.ISerializer>, ICustomSerializers
	{
		public CustomSerializers() : this(ExtensionModel.Defaults.SpecificTypeComparer) {}

		public CustomSerializers(IEqualityComparer<TypeInfo> comparer) : base(comparer) {}
	}
}