using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public interface IStartElementProvider : IParameterizedSource<TypeInfo, IEmitter> {}
	/*public interface INameProvider<out T> : IParameterizedSource<TypeInfo, T> where T : IElement { }*/
}