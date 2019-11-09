using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class CollectionItemAwareWriter : IWriter
	{
		readonly IWriter   _writer;
		readonly IIdentity _null;

		public CollectionItemAwareWriter(IWriter writer) : this(writer, NullElementIdentity.Default) {}

		public CollectionItemAwareWriter(IWriter writer, IIdentity @null)
		{
			_writer = writer;
			_null   = @null;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			if (instance == null)
			{
				writer.Start(_null);
				writer.EndCurrent();
			}
			else
			{
				_writer.Write(writer, instance);
			}
		}
	}
}