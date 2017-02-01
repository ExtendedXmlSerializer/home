using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public interface IElementProvider : IParameterizedSource<TypeInfo, IElement> {}
	/*public interface INameProvider<out T> : IParameterizedSource<TypeInfo, T> where T : IElement { }*/
}