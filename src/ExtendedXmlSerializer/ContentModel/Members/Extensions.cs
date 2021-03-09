using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	static class Extensions
	{
		public static T GetIfAssigned<T>(this IReader<T> @this, IFormatReader reader)
			=> reader.IsAssigned() ? @this.Get(reader) : default;

		public static object GetIfAssigned(this IReader @this, IFormatReader reader)
			=> reader.IsAssigned() ? @this.Get(reader) : null;

		public static bool Contains(this IFormatReader @this, IIdentity identity)
			=> Content.Contains.Default.IsSatisfiedBy((@this, identity));

		public static bool IsAssigned(this IFormatReader @this)
			=> !IdentityComparer.Default.Equals(@this, NullElementIdentity.Default) &&
			   !@this.IsSatisfiedBy(NullValueIdentity.Default);

		public static IConverter Get(this IMemberConverters @this, IMember descriptor)
			=> @this.Get(descriptor.Metadata) ?? @this.Get(descriptor.MemberType);

		public static ISpecification<object> GetInstance(this ISpecification<object> @this)
			=> @this is IInstanceValueSpecification instance ? instance.Instance : AlwaysSpecification<object>.Default;
	}
}