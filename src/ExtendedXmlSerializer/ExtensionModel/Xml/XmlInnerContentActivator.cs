using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlInnerContentActivator : IInnerContentActivator
	{
		readonly IReader               _activator;
		readonly IXmlContentsActivator _contents;

		public XmlInnerContentActivator(IReader activator, IXmlContentsActivator contents)
		{
			_activator = activator;
			_contents  = contents;
		}

		public IInnerContent Get(IFormatReader parameter)
			=> parameter.IsAssigned()
				   ? _contents.Create(parameter, _activator.Get(parameter),
				                      Content(parameter.Get()
				                                       .To<System.Xml.XmlReader>()))
				   : null;

		static XmlContent Content(System.Xml.XmlReader reader)
		{
			var attributes = reader.HasAttributes ? new XmlAttributes(reader) : (XmlAttributes?)null;
			var depth      = XmlDepth.Default.Get(reader);
			var content    = depth.HasValue ? new XmlElements(reader, depth.Value) : (XmlElements?)null;
			var result     = new XmlContent(attributes, content);
			return result;
		}
	}
}