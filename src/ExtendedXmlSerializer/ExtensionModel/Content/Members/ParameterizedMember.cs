using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedMember : IMember
	{
		readonly IMember _member;

		public ParameterizedMember(IMember member) => _member = member;

		public string Identifier => _member.Identifier;

		public string Name => _member.Name;

		public MemberInfo Metadata => _member.Metadata;

		public TypeInfo MemberType => _member.MemberType;

		public bool IsWritable => _member.IsWritable;

		public int Order => _member.Order;
	}
}