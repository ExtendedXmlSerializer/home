using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel
{
	class WriterMarker : IWriter
	{
		void IWriter<object>.Write(IFormatWriter writer, object instance)
			=> throw new InvalidOperationException("This exists for static type-checking purposes only.");
	}

	/*class WriterAdapter<T> : WriterMarker, IWriter<T>, IGenericAware
	{
		readonly static TypeInfo TypeInfo = Support<T>.Key;

		readonly IWriter<T> _writer;

		public WriterAdapter(IWriter<T> writer) => _writer = writer;

		public void Write(IFormatWriter writer, T instance) => _writer.Write(writer, instance);

		public TypeInfo Get() => TypeInfo;
	}*/

	class SerializerAdapter<T> : SerializerMarker, ISerializer<T>, IGenericAware
	{
		readonly static TypeInfo TypeInfo = Support<T>.Key;

		readonly IReader<T> _reader;
		readonly IWriter<T> _writer;

		public SerializerAdapter(IReader<T> reader, IWriter<T> writer)
		{
			_reader = reader;
			_writer = writer;
		}

		public T Get(IFormatReader parameter) => _reader.Get(parameter);

		public void Write(IFormatWriter writer, T instance) => _writer.Write(writer, instance);

		public TypeInfo Get() => TypeInfo;
	}

	class SerializerMarker : ISerializer
	{
		object IParameterizedSource<IFormatReader, object>.Get(IFormatReader parameter)
			=> throw new InvalidOperationException("This exists for static type-checking purposes only.");

		void IWriter<object>.Write(IFormatWriter writer, object instance)
			=> throw new InvalidOperationException("This exists for static type-checking purposes only.");
	}
}