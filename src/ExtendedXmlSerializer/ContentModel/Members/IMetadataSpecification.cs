using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	public interface IMetadataSpecification : ISpecification<PropertyInfo>, ISpecification<FieldInfo> {}
}