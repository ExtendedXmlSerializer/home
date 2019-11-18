using ExtendedXmlSerializer.ExtensionModel;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Root-level component that is used to create serializers.  The configuration container contains all the applied
	/// configurations which are then applied when the serializer is created, creating the fully configured serializer.
	/// </summary>
	public interface IConfigurationContainer : IContext, IEnumerable<ITypeConfiguration>
	{
		/// <summary>
		/// Used to extend this container with an extension.
		/// </summary>
		/// <param name="extension">The extension to add to this configuration container.</param>
		/// <returns>The configured configuration container with the extension applied.</returns>
		IConfigurationContainer Extend(ISerializerExtension extension);
	}
}