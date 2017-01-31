using System.Collections;
using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IReader : IName
	{
		string Value();

		IEnumerator Members();

		IEnumerator Items();
	}
}