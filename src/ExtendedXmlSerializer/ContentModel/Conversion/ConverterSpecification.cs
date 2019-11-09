using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class ConverterSpecification : DelegatedAssignedSpecification<TypeInfo, IConverter>
	{
		public ConverterSpecification(IConverters converters) : base(converters.Get) {}
	}
}