using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	struct Existing
	{
		public Existing(XmlReader reader, object instance)
		{
			Reader   = reader;
			Instance = instance;
		}

		public XmlReader Reader { get; }

		public object Instance { get; }
	}
}