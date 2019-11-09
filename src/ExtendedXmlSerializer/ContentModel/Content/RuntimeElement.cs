using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class RuntimeElement : DecoratedSource<TypeInfo, IWriter>, IElement
	{
		public RuntimeElement(Element element, GenericElement generic, ArrayElement array)
			: base(element.Let(IsGenericTypeSpecification.Default, generic)
			              .Let(IsArraySpecification.Default, array)) {}
	}
}