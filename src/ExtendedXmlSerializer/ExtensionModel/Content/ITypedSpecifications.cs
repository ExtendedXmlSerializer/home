using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	/// <summary>
	/// A store of type specifications, keyed by type metdata.
	/// </summary>
	public interface ITypedSpecifications : IParameterizedSource<TypeInfo, ISpecification<object>> {}
}