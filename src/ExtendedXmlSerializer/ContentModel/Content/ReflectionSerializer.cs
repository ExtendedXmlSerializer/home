using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class ReflectionSerializer : ISerializer<MemberInfo>
	{
		public static ReflectionSerializer Default { get; } = new ReflectionSerializer();

		ReflectionSerializer() {}

		public MemberInfo Get(IFormatReader parameter) => parameter.Get(parameter.Content());

		public void Write(IFormatWriter writer, MemberInfo instance) => writer.Content(writer.Get(instance));
	}
}