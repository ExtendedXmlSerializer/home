using System;
using ExtendedXmlSerialization.Conversion.Model.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IEmitter
	{
		IDisposable Emit(IName name);

		void Write(string text);
	}
}