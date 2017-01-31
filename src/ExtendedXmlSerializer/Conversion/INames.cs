using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface INames : ISelector<TypeInfo, IName> {}
}