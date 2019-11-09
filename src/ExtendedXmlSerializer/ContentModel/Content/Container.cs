namespace ExtendedXmlSerializer.ContentModel.Content
{
	class Container<T> : SerializerAdapter<T>
	{
		public Container(IWriter<T> element, ISerializer<T> content) :
			base(content, new Enclosure<T>(element, content)) {}
	}

	sealed class Container : Container<object>
	{
		public Container(IWriter element, ISerializer content) : base(element, content) {}
	}
}