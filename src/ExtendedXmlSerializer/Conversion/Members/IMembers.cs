using System.Collections.Generic;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface IMembers : IEnumerable<IMember>, IParameterizedSource<string, IMember> {}
}