using System.Collections;
using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IReader : IElement
	{
		string Value();

		IEnumerator Members();

		IEnumerator Items();
	}
}