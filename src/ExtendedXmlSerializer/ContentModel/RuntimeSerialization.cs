using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class RuntimeSerialization : IRuntimeSerialization
	{
		readonly IContents                             _contents;
		readonly IRuntimeSerializationExceptionMessage _message;

		public RuntimeSerialization(IContents contents, IRuntimeSerializationExceptionMessage message)
		{
			_contents = contents;
			_message  = message;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var serializer = _contents.Get(parameter);
			if (serializer is RuntimeSerializer)
			{
				throw new
					InvalidOperationException($"The serializer for type '{parameter}' could not be found.  Please ensure that the type is a valid type can be activated. {_message.Get(parameter)}");
			}

			return serializer;
		}
	}
}