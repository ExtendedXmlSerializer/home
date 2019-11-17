using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class Write : IWrite
	{
		readonly ISerializers   _serializers;
		readonly IFormatWriters _writers;

		public Write(ISerializers serializers, IFormatWriters writers)
		{
			_serializers = serializers;
			_writers     = writers;
		}

		public void Execute(Writing parameter)
		{
			_serializers.Get(parameter.Instance.GetType())
			            .Write(_writers.Get(parameter.Writer), parameter.Instance);
		}
	}
}