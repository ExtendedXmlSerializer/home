using System.Collections.Generic;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IMembers : IEnumerable<IMemberContext>, IParameterizedSource<string, IMemberContext> {}
}