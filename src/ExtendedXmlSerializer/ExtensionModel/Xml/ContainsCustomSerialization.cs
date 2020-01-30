using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ContainsCustomSerialization : AnySpecification<TypeInfo>, IContainsCustomSerialization
	{
		public ContainsCustomSerialization(ICustomXmlSerializers xmlSerializers, ICustomSerializers types)
			: base(xmlSerializers, types) {}
	}
}