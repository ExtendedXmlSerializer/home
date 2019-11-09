using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	public sealed class IsWritable : DelegatedSpecification<IMember>
	{
		public static IsWritable Default { get; } = new IsWritable();

		IsWritable() : base(x => x.IsWritable) {}
	}
}