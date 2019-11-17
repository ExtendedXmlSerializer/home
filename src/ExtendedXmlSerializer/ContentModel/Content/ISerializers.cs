using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	/// <summary>
	/// A root-level component that will locate and select a serializer when given a provided type.
	/// </summary>
	public interface ISerializers : IParameterizedSource<TypeInfo, ISerializer> {}
}