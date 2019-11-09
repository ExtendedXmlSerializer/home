using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IMemberConfigurations
		: IParameterizedSource<MemberInfo, IMemberConfiguration>,
		  IEnumerable<IMemberConfiguration> {}
}