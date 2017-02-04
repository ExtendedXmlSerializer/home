using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface ITypeMembers : IParameterizedSource<TypeInfo, IEnumerable<IMember>> {}
}