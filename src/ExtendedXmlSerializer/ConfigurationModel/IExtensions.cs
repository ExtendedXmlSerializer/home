using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;

namespace ExtendedXmlSerializer.ConfigurationModel
{
	public interface IExtensions : ICollection<ISerializerExtension>
	{
		T Find<T>() where T : ISerializerExtension;
	}

	sealed class ExtensionCollection : KeyedByTypeCollection<ISerializerExtension>, IExtensions
	{
		public ExtensionCollection() : this(DefaultExtensions.Default.ToArray()) { }

		public ExtensionCollection(params ISerializerExtension[] extensions) : base(extensions) { }

		public new T Find<T>() where T : ISerializerExtension => base.Find<T>();
	}

}