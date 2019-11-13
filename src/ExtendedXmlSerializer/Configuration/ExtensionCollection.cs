using System.Linq;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class ExtensionCollection : KeyedByTypeCollection<ISerializerExtension>, IExtensionCollection
	{
		public ExtensionCollection(params ISerializerExtension[] extensions) : base(extensions) {}

		public bool Contains<T>() where T : ISerializerExtension => Contains(Support<T>.Key);

		public T Find<T>() where T : ISerializerExtension => this.OfType<T>()
		                                                         .FirstOrDefault();
	}
}