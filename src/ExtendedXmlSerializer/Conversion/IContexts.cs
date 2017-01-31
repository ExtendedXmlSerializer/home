using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IContexts : IParameterizedSource<TypeInfo, IElementContext> {}
}