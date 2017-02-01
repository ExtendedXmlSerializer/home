using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Names
{
	public interface INameProvider : INameProvider<IName> {}
	public interface INameProvider<out T> : IParameterizedSource<TypeInfo, T> where T : IName { }
}