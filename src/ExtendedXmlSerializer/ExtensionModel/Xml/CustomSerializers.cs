using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CustomSerializers : Metadata<TypeInfo, ISerializer>, ICustomSerializers
	{
		readonly static CompositeTypeComparer Comparer = new CompositeTypeComparer(ImplementedTypeComparer.Default,
		                                                                           TypeIdentityComparer.Default,
		                                                                           InheritedTypeComparer.Default);

		public CustomSerializers() : this(Comparer) {}

		public CustomSerializers(IEqualityComparer<TypeInfo> comparer) : base(comparer) {}
	}
}