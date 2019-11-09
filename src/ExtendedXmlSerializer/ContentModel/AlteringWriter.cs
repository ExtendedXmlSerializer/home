using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel
{
	class AlteringWriter : IWriter
	{
		readonly IAlteration<object> _alteration;
		readonly IWriter             _writer;

		public AlteringWriter(IAlteration<object> alteration, IWriter writer)
		{
			_alteration = alteration;
			_writer     = writer;
		}

		public void Write(IFormatWriter writer, object instance) => _writer.Write(writer, _alteration.Get(instance));
	}
}