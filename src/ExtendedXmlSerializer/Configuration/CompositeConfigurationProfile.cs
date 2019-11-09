using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Configuration
{
	public class CompositeConfigurationProfile : CompositeAlteration<IConfigurationContainer>, IConfigurationProfile
	{
		public CompositeConfigurationProfile(params IAlteration<IConfigurationContainer>[] profiles)
			: this(profiles.AsEnumerable()) {}

		public CompositeConfigurationProfile(IEnumerable<IAlteration<IConfigurationContainer>> alterations) :
			base(alterations) {}
	}
}