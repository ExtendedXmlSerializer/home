using System.Collections.Generic;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface IMembers : IEnumerable<IMemberConverter>, IParameterizedSource<string, IMemberConverter> {}
}