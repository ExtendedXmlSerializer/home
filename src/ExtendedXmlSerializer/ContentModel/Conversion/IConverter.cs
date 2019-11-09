using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	public interface IConverter : IConverter<object>, ISource<TypeInfo> {}

	public interface IConverter<T> : ISpecification<TypeInfo>, IConvert<T> {}
}