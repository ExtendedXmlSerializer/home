using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class DefaultMetadataSpecification : MetadataSpecification
	{
		public static ISpecification<PropertyInfo> Property { get; } =
			new MemberSpecification<PropertyInfo>(PropertyMemberSpecification.Default);

		public static ISpecification<FieldInfo> Field { get; } =
			new MemberSpecification<FieldInfo>(new FieldMemberSpecification(
			                                                                IsDefinedSpecification<XmlElementAttribute>
				                                                                .Default.Or(
				                                                                            IsDefinedSpecification
					                                                                            <XmlAttributeAttribute
					                                                                            >
					                                                                            .Default)));

		public static DefaultMetadataSpecification Default { get; } = new DefaultMetadataSpecification();

		DefaultMetadataSpecification() : base(Property, Field) {}
	}
}