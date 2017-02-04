using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public interface IEmitters : ISelector<TypeInfo, IEmitter> {}
}