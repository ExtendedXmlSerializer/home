using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer
{
	sealed class Write<T> : IWrite<T>
	{
		readonly ISerializers      _serializers;
		readonly IFormatWriters<T> _writers;

		public Write(ISerializers serializers, IFormatWriters<T> writers)
		{
			_serializers = serializers;
			_writers     = writers;
		}

		public void Execute(Writing<T> parameter)
		{
			_serializers.Get(parameter.Instance.GetType()
			                          .GetTypeInfo())
			            .Write(_writers.Get(parameter.Writer), parameter.Instance);
		}
	}
}