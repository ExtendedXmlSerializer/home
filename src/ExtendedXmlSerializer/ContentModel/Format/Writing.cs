namespace ExtendedXmlSerializer.ContentModel.Format
{
	readonly struct Writing
	{
		public Writing(System.Xml.XmlWriter writer, object instance)
		{
			Writer   = writer;
			Instance = instance;
		}

		public System.Xml.XmlWriter Writer { get; }
		public object Instance { get; }
	}
}