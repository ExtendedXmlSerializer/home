namespace ExtendedXmlSerializer.ContentModel.Content
{
	class Container<T> : SerializerAdapter<T>
	{
		public Container(IWriter<T> element, ISerializer<T> content, IEnclosures enclosures)
			: base(content, enclosures.Get(element, content)) {}
	}

	sealed class Container : Container<object>
	{
		public Container(IWriter element, ISerializer content, IEnclosures enclosures) 
			: base(element, content, enclosures) {}
	}
}