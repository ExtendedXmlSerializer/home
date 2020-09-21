using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel
{
	[UsedImplicitly]
	sealed class RuntimeSerializer : ISerializer
	{
		readonly IContents       _serialization;
		readonly IClassification _classification;

		public RuntimeSerializer(IRuntimeSerialization serialization, IClassification classification)
		{
			_serialization  = serialization;
			_classification = classification;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			_serialization.Get(instance.GetType()).Write(writer, instance);
		}

		public object Get(IFormatReader reader) => _serialization.Get(_classification.Get(reader)).Get(reader);
	}
}