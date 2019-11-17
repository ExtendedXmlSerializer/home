using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// Used to retrieve a member serialization component from a provided type.
	/// </summary>
	public interface IMemberSerializations : IParameterizedSource<TypeInfo, IMemberSerialization> {}
}