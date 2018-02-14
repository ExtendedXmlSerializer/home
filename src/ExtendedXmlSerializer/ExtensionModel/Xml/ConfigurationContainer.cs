using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public interface ISerializers : IParameterizedSource<TypeInfo, IXmlSerializer<object>> {}

	/*public sealed class ConfigurationContainer : ConfigurationContainer
	{
		public ConfigurationContainer() : this(DefaultExtensions.Default.ToArray()) { }
		public ConfigurationContainer(params ISerializerExtension[] extensions) : base(extensions) { }
		public ConfigurationContainer(ICollection<ISerializerExtension> extensions, IActivator<ISerializers> activator) : base(extensions, activator) { }
	}*/
}
