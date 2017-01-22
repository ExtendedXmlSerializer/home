using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ElementModel
{
	public interface IDictionaryItemFactory : IParameterizedSource<TypeInfo, IElement> {}
}