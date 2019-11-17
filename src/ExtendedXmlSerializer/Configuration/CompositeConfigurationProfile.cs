using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Used to define a configuration profile composed of multiple configurations.
	/// </summary>
	public class CompositeConfigurationProfile : CompositeAlteration<IConfigurationContainer>, IConfigurationProfile
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="profiles">Configuration profiles to apply to a configuration container.</param>
		public CompositeConfigurationProfile(params IAlteration<IConfigurationContainer>[] profiles) : base(profiles) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="profiles">Configuration profiles to apply to a configuration container.</param>
		public CompositeConfigurationProfile(IEnumerable<IAlteration<IConfigurationContainer>> profiles)
			: base(profiles) {}
	}
}