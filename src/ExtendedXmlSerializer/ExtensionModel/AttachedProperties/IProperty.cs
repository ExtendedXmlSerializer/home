using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	/// <summary>
	/// Represents an attached property.
	/// </summary>
	public interface IProperty : ISource<TypeInfo>, ISpecification<TypeInfo>, ITableSource<object, object>
	{
		/// <summary>
		/// The metadata of the property that this attached property represents.
		/// </summary>
		PropertyInfo Metadata { get; }
	}
}