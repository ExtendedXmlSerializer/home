using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using JetBrains.Annotations;
using System.Reflection;

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
			var typeInfo = instance.GetType()
			                       .GetTypeInfo();
			_serialization.Get(typeInfo)
			              .Write(writer, instance);
		}

		public object Get(IFormatReader reader) => _serialization.Get(_classification.Get(reader))
		                                                         .Get(reader);
	}
}