using System;
using System.IO;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Instances {
	sealed class RootInstanceAwareWriters<T> : IWriters<T>
	{
		readonly IWriters<T>                _writers;
		readonly ITableSource<Stream, Type> _types;
		readonly ITableSource<Stream, T>    _instances;

		public RootInstanceAwareWriters(IWriters<T> writers) : this(writers, RootInstanceTypes.Default, RootInstances<T>.Default) {}

		public RootInstanceAwareWriters(IWriters<T> writers, ITableSource<Stream, Type> types, ITableSource<Stream, T> instances)
		{
			_writers   = writers;
			_types     = types;
			_instances = instances;
		}

		public IWriter<T> Get() => new RootContextWriter<T>(_writers.Get(), _types, _instances);
	}
}