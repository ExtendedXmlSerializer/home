using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	/// <summary>
	/// Root-level component that selects a serializer for handling inner-content when provided a type.
	/// </summary>
	public interface IContents : IParameterizedSource<TypeInfo, ISerializer> {}
}