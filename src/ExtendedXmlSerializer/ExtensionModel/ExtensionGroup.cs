using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ExtensionGroup : Group<ISerializerExtension>
	{
		public ExtensionGroup(GroupName name, ICollection<ISerializerExtension> container) : this(name, container,
		                                                                                          Support<ISerializerExtension
		                                                                                          >.Empty) {}

		public ExtensionGroup(GroupName name, ICollection<ISerializerExtension> container,
		                      params ISerializerExtension[] items)
			: base(name, new ExtensionCollection(container, items)) {}
	}
}