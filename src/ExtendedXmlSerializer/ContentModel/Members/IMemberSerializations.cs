using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	public interface IMemberSerializations : IParameterizedSource<TypeInfo, IMemberSerialization> {}
}