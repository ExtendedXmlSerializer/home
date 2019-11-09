using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;

namespace ExtendedXmlSerializer
{
	sealed class Read<T> : IRead<T>
	{
		readonly ISerializers      _serializers;
		readonly IFormatReaders<T> _readers;
		readonly IClassification   _classification;

		public Read(ISerializers serializers, IFormatReaders<T> readers, IClassification classification)
		{
			_serializers    = serializers;
			_readers        = readers;
			_classification = classification;
		}

		public object Get(T parameter)
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