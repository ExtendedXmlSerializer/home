namespace ExtendedXmlSerializer.ContentModel.Format
{
	struct Writing<T>
	{
		public Writing(T writer, object instance)
		{
			Writer   = writer;
			Instance = instance;
		}

		public T Writer { get; }
		public object Instance { get; }
	}
}