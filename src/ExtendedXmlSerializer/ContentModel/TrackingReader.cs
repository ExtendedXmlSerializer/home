using ExtendedXmlSerializer.ContentModel.Format;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class TrackingReader<T> : IReader<T>
	{
		readonly IReader<T> _reader;

		public TrackingReader(IReader<T> reader) => _reader = reader;

		public T Get(IFormatReader parameter)
		{
			var reader = parameter.Get().To<XmlReader>();
			var name   = reader.NodeType == XmlNodeType.Attribute ? parameter.Name : null;
			var result = _reader.Get(parameter);
			if (name != null)
			{
				reader.MoveToAttribute(name);
			}
			else
			{
				parameter.Set();
			}

			return result;
		}
	}
}