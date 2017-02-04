using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IReader : IDisplayAware
	{
		TypeInfo Classification();

		string Value();

		IEnumerator Members();

		IEnumerator Items();
	}
}