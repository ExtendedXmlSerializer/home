using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class RootContext : IRootContext
	{
		readonly IExtensionCollection _extensions;
		readonly IServicesFactory     _factory;

		public RootContext(IExtensionCollection extensions) : this(new TypeConfigurations(extensions), extensions) {}

		public RootContext(ITypeConfigurations types, IExtensionCollection extensions) : this(types, extensions,
		                                                                                      ServicesFactory
			                                                                                      .Default) {}

		public RootContext(ITypeConfigurations types, IExtensionCollection extensions, IServicesFactory factory)
		{
			Types       = types;
			_extensions = extensions;
			_factory    = factory;

			_extensions.Add(new RootContextExtension(this));
		}

		public IRootContext Root => this;
		public IContext Parent => null;

		public ITypeConfigurations Types { get; }

		public IExtendedXmlSerializer Create()
		{
			using (var services = _factory.Get(_extensions))
			{
				var result = services.Get<IExtendedXmlSerializer>();
				return result;
			}
		}

		IEnumerator<ISerializerExtension> IEnumerable<ISerializerExtension>.GetEnumerator()
			=> _extensions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _extensions.GetEnumerator();

		void ICollection<ISerializerExtension>.Add(ISerializerExtension item) => _extensions.Add(item);

		void ICollection<ISerializerExtension>.Clear() => _extensions.Clear();

		bool ICollection<ISerializerExtension>.Contains(ISerializerExtension item) => _extensions.Contains(item);

		void ICollection<ISerializerExtension>.CopyTo(ISerializerExtension[] array, int arrayIndex) =>
			_extensions.CopyTo(array, arrayIndex);

		bool ICollection<ISerializerExtension>.Remove(ISerializerExtension item) => _extensions.Remove(item);

		int ICollection<ISerializerExtension>.Count => _extensions.Count;
		bool ICollection<ISerializerExtension>.IsReadOnly => _extensions.IsReadOnly;

		bool IExtensionCollection.Contains<T>() => _extensions.Contains<T>();

		T IExtensionCollection.Find<T>() => _extensions.Find<T>();
	}
}