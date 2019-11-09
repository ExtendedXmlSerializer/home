using System.Collections.Generic;
using ExtendedXmlSerializer.ExtensionModel;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IExtensionCollection : ICollection<ISerializerExtension>
	{
		bool Contains<T>() where T : ISerializerExtension;

		T Find<T>() where T : ISerializerExtension;
	}
}