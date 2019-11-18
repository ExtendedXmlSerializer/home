using ExtendedXmlSerializer.ExtensionModel;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// A specialized collection that manages a set of serializer extensions.
	/// </summary>
	public interface IExtensionCollection : ICollection<ISerializerExtension>
	{
		/// <summary>
		/// Used to determine if a particular extension type is contained within the collection.
		/// </summary>
		/// <typeparam name="T">The extension type.</typeparam>
		/// <returns>`true` if the collection contains the type, otherwise `false`.</returns>
		bool Contains<T>() where T : ISerializerExtension;

		/// <summary>
		/// Finds an extension of the provided type in the collection.
		/// </summary>
		/// <typeparam name="T">The requested type.</typeparam>
		/// <returns>The located extension, if found. `null` otherwise.</returns>
		T Find<T>() where T : ISerializerExtension;
	}
}