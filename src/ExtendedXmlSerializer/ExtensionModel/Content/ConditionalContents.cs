using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	class ConditionalContents : ConditionalSource<TypeInfo, ContentModel.ISerializer>, IContents
	{
		public ConditionalContents(ISpecification<TypeInfo> specification, IContents source, IContents fallback)
			: base(specification, source, fallback) {}
	}
}