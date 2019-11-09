using System;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class ContentReaders : IContentReaders
	{
		public static ContentReaders Default { get; } = new ContentReaders();

		ContentReaders() {}

		public IReader Get(Func<string, object> parameter) => new ContentReader(parameter);
	}
}