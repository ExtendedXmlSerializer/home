using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// A specialized configuration component that is intended to alter the way the serializer emits content.
	/// </summary>
	public interface IEmitBehavior : IAlteration<IConfigurationContainer> {}
}