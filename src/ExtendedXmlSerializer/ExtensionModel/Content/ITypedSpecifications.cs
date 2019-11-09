using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public interface ITypedSpecifications : IParameterizedSource<TypeInfo, ISpecification<object>> {}
}