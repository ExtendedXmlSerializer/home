using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Configuration
{
	public interface ITypeConfigurations
		: IParameterizedSource<TypeInfo, ITypeConfiguration>, IEnumerable<ITypeConfiguration> {}
}