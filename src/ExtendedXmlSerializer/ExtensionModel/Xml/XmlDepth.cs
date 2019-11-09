using System.Xml;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlDepth : IParameterizedSource<System.Xml.XmlReader, int?>
	{
		public static XmlDepth Default { get; } = new XmlDepth();

		XmlDepth() {}

		public int? Get(System.Xml.XmlReader parameter)
		{
			if (parameter.HasAttributes && parameter.NodeType == XmlNodeType.Attribute)
			{
				parameter.MoveToContent();
			}

			var result = !parameter.IsEmptyElement ? parameter.Depth + 1 : (int?)null;
			return result;
		}
	}
}