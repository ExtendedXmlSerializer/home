using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class Read : IRead
	{
		readonly ISerializers      _serializers;
		readonly IFormatReaders _readers;
		readonly IClassification   _classification;

		public Read(ISerializers serializers, IFormatReaders readers, IClassification classification)
		{
			_serializers    = serializers;
			_readers        = readers;
			_classification = classification;
		}

		public object Get(System.Xml.XmlReader parameter)
		{
			using (var content = _readers.Get(parameter))
			{
				var classification = _classification.GetClassification(content);
				var result = _serializers.Get(classification)
				                         .Get(content);
				return result;
			}
		}
	}
}