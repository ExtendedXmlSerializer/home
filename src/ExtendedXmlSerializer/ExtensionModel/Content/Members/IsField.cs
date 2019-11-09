using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class IsField : DelegatedSpecification<IMember>
	{
		public static IsField Default { get; } = new IsField();

		IsField() : base(x => x.Metadata is FieldInfo) {}
	}
}