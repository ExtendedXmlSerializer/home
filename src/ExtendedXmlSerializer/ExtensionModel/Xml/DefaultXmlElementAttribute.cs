using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class DefaultXmlElementAttribute : IParameterizedSource<MemberInfo, XmlElementAttribute>
	{
		public static DefaultXmlElementAttribute Default { get; } = new DefaultXmlElementAttribute();

		DefaultXmlElementAttribute() {}

		public XmlElementAttribute Get(MemberInfo parameter)
		{
			var attributes = parameter.GetCustomAttributes<XmlElementAttribute>(false)
			                          .ToArray();
			var result = attributes.FirstOrDefault(x => x.DataType == null) ?? attributes.Only();
			return result;
		}
	}
}