using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// A specific implementation marker that is used to determine whether or not to select and process a type's member.
	/// </summary>
	public interface IMetadataSpecification : ISpecification<PropertyInfo>, ISpecification<FieldInfo> {}
}