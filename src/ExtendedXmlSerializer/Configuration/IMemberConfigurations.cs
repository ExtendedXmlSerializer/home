using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Represents a store of member configurations.  Considered internal framework code and not to be used by external applications.
	/// </summary>
	public interface IMemberConfigurations : IParameterizedSource<MemberInfo, IMemberConfiguration>, IEnumerable<IMemberConfiguration> {}
}