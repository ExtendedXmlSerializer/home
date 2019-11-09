using System.Collections.Generic;
using ExtendedXmlSerializer.ExtensionModel;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IConfigurationContainer : IContext, IEnumerable<ITypeConfiguration>
	{
		IConfigurationContainer Extend(ISerializerExtension extension);
	}
}