using System;
using System.IO;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Instances
{
	sealed class RootContextWriter<T> : IWriter<T>
	{
		readonly IWriter<T> _writer;
		readonly ITableSource<Stream, Type> _types;
		readonly ITableSource<Stream, T> _instances;

		public RootContextWriter(IWriter<T> writer, ITableSource<Stream, Type> types, ITableSource<Stream, T> instances)
		{
			_writer = writer;
			_types = types;
			_instances = instances;
		}

		public void Execute(Input<T> parameter)
		{
			_types.Assign(parameter.Stream, parameter.Instance.GetType());
			_instances.Assign(parameter.Stream, parameter.Instance);
			_writer.Execute(parameter);
			_instances.Remove(parameter.Stream);
			_types.Remove(parameter.Stream);
		}
	}
}
