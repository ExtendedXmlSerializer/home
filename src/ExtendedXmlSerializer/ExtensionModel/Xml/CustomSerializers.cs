using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CustomSerializers : Metadata<TypeInfo, ContentModel.ISerializer>, ICustomSerializers
	{
		public CustomSerializers()
			: this(new CompositeTypeComparer(TypeIdentityComparer.Default, InheritedTypeComparer.Default)) {}

		public CustomSerializers(IEqualityComparer<TypeInfo> comparer) : base(comparer) {}
	}
}