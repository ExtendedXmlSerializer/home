using System;
using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IWriter
	{
		IDisposable Emit(IName name);

		void Write(string text);
	}
}