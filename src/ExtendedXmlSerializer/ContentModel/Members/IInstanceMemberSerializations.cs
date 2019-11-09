using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IInstanceMemberSerializations : IParameterizedSource<TypeInfo, IInstanceMemberSerialization> {}
}