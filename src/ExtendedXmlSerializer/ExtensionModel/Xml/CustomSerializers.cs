using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CustomSerializers : Metadata<TypeInfo, ContentModel.ISerializer>, ICustomSerializers
	{
		readonly static CompositeTypeComparer Comparer = new CompositeTypeComparer(ImplementedTypeComparer.Default,
		                                                                           TypeIdentityComparer.Default,
		                                                                           InheritedTypeComparer.Default);

		public CustomSerializers() : this(Comparer) {}

		public CustomSerializers(IEqualityComparer<TypeInfo> comparer) : base(comparer) {}
	}
}