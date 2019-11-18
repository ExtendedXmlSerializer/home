using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	/// <summary>
	/// Convenience component that encapsulates a query for the <see cref="IMember.IsWritable"/> property.
	/// </summary>
	public sealed class IsWritable : DelegatedSpecification<IMember>
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static IsWritable Default { get; } = new IsWritable();

		IsWritable() : base(x => x.IsWritable) {}
	}
}