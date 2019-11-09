using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public interface IProperty : ISource<TypeInfo>, ISpecification<TypeInfo>, ITableSource<object, object>
	{
		PropertyInfo Metadata { get; }
	}
}