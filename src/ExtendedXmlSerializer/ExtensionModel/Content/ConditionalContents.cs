using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	class ConditionalContents : ConditionalSource<TypeInfo, ISerializer>, IContents
	{
		public ConditionalContents(ISpecification<TypeInfo> specification, IContents source, IContents fallback)
			: base(specification, source, fallback) {}
	}
}