using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// The default implementation of the metadata specifications, determining the policies used to allow a given field or
	/// property to be selected as a valid member.
	/// </summary>
	public sealed class DefaultMetadataSpecification : MetadataSpecification
	{
		/// <summary>
		/// Property specification instance that allows a property member if it's an instance (non-static) member, is public,
		/// and can be read.
		/// </summary>
		public static ISpecification<PropertyInfo> Property { get; } =
			new MemberSpecification<PropertyInfo>(PropertyMemberSpecification.Default);

		/// <summary>
		/// Field specification instance that allows a field member if it's an instance (non-static) member, public, or if it
		/// is decorated with either the <see cref="XmlAttributeAttribute"/> or <see cref="XmlElementAttribute"/> attributes.
		/// </summary>
		public static ISpecification<FieldInfo> Field { get; } =
			new MemberSpecification<FieldInfo>(new FieldMemberSpecification(
			                                                                IsDefinedSpecification<XmlElementAttribute>
				                                                                .Default.Or(
				                                                                            IsDefinedSpecification
					                                                                            <XmlAttributeAttribute
					                                                                            >
					                                                                            .Default)));

		/// <summary>
		/// The default instance.
		/// </summary>
		public static DefaultMetadataSpecification Default { get; } = new DefaultMetadataSpecification();

		DefaultMetadataSpecification() : base(Property, Field) {}
	}
}