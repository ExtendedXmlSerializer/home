using System;
using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IWriter
	{
		IDisposable Emit(IElement element);

		void Write(string text);
	}
}