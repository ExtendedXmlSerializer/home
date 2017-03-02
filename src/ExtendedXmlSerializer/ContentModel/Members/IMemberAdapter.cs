using System.Reflection;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	public interface IMemberAdapter : IIdentity
	{
		MemberInfo Metadata { get; }

		TypeInfo MemberType { get; }

		bool IsWritable { get; }

		object Get(object instance);

		void Assign(object instance, object value);
	}
}