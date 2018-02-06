using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content {
	sealed class ConditionalElement : ConditionalSource<TypeInfo, IWriter>, IElements
	{
		public ConditionalElement(ISpecification<TypeInfo> specification, IElements source, IElements fallback)
			: base(specification, source, fallback) {}
	}
}