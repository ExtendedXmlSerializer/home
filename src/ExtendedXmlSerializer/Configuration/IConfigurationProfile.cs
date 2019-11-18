using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Used as a mechanism to encapsulate a set of pre-determined configurations to apply to a provided container.
	/// </summary>
	public interface IConfigurationProfile : IAlteration<IConfigurationContainer> {}
}