using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	public interface IMember : IIdentity
	{
		MemberInfo Metadata { get; }

		TypeInfo MemberType { get; }

		bool IsWritable { get; }

		int Order { get; }
	}
}