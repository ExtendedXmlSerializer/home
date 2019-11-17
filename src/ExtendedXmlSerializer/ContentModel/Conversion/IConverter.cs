using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	/// <summary>
	/// Root-level component that is used to convert the configured type to and from string text.  This is a generalized version of <see cref="IConverter{T}"/>
	/// </summary>
	public interface IConverter : IConverter<object>, ISource<TypeInfo> {}

	/// <summary>
	/// Root-level component that is used to convert the configured type to and from string text.  This component builds on
	/// the <see cref="IConvert{T}"/> class to provide a specification to let the serializer know it can (or cannot) handle
	/// a provided type during serialization selection.
	/// </summary>
	/// <typeparam name="T">The instance type to convert.</typeparam>
	public interface IConverter<T> : ISpecification<TypeInfo>, IConvert<T> {}
}