using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	interface ICoercion : ISpecificationSource<TypeInfo, object> {}
}