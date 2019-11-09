using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	class ConditionalElement : ConditionalSource<TypeInfo, IWriter>, IElement
	{
		public ConditionalElement(ISpecification<TypeInfo> specification, IElement source, IElement fallback)
			: base(specification, source, fallback) {}
	}
}