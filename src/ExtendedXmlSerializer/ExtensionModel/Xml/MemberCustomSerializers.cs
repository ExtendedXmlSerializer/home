using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class MemberCustomSerializers : Metadata<MemberInfo, ContentModel.ISerializer>, ICustomMemberSerializers
	{
		public MemberCustomSerializers() : base(new MemberComparer(ExtensionModel.Defaults.SpecificTypeComparer)) {}
	}
}