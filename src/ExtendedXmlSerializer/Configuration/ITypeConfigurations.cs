using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Represents a store of type configurations.  This is considered internal code and not to be used by external
	/// applications.
	/// </summary>
	public interface ITypeConfigurations
		: IParameterizedSource<TypeInfo, ITypeConfiguration>, IEnumerable<ITypeConfiguration> {}
}