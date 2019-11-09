using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class RegisteredContentSpecification : DecoratedSpecification<TypeInfo>
	{
		public RegisteredContentSpecification(ICustomSerializers serializers) :
			base(IsDefinedSpecification<ContentSerializerAttribute>.Default.Or(serializers)) {}
	}
}