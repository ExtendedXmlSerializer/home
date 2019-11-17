using ExtendedXmlSerializer.Core.Sources;
using System;
using XmlReader = System.Xml.XmlReader;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class InstanceReader : IInstanceReader
	{
		readonly IExtendedXmlSerializer _serializer;
		readonly IInstances             _instances;

		public InstanceReader(IExtendedXmlSerializer serializer) : this(serializer, Instances.Default) {}

		public InstanceReader(IExtendedXmlSerializer serializer, IInstances instances)
		{
			_serializer = serializer;
			_instances  = instances;
		}

		public object Get(Existing parameter)
		{
			using (new Context(_instances, parameter.Reader))
			{
				_instances.Assign(parameter.Reader, parameter.Instance);

				var result = _serializer.Deserialize(parameter.Reader);
				return result;
			}
		}

		struct Context : IDisposable
		{
			readonly ITableSource<XmlReader, object> _table;
			readonly XmlReader                       _key;

			public Context(ITableSource<XmlReader, object> table, XmlReader key)
			{
				_table = table;
				_key   = key;
			}

			public void Dispose()
			{
				_table.Remove(_key);
			}
		}
	}
}