using System.Collections;
using ExtendedXmlSerialization.Conversion.Model.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IYielder : IName
	{
		string Value();

		IEnumerator Members();

		IEnumerator Items();
	}
}