using System;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class ContentWriters : IContentWriters
	{
		public static ContentWriters Default { get; } = new ContentWriters();

		ContentWriters() {}

		public IWriter Get(Func<object, string> parameter) => new ContentWriter(parameter);
	}
}