using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class MemberCustomSerializers : Metadata<MemberInfo, ISerializer>, ICustomMemberSerializers
	{
		public MemberCustomSerializers() : base(MemberComparer.Default) {}
	}
}