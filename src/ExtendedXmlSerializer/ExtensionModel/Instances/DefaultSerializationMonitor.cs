using System;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class DefaultSerializationMonitor : ISerializationMonitor
	{
		public static DefaultSerializationMonitor Default { get; } = new DefaultSerializationMonitor();

		DefaultSerializationMonitor() {}

		public void OnSerializing(IFormatWriter writer, object instance) {}

		public void OnSerialized(IFormatWriter writer, object instance) {}

		public void OnActivating(IFormatReader reader, Type instanceType) {}

		public void OnActivated(object instance) {}

		public void OnDeserializing(IFormatReader reader, Type instanceType) {}

		public void OnDeserialized(IFormatReader reader, object instance) {}
	}

	sealed class SerializationMonitor<T> : ISerializationMonitor
	{
		readonly ISerializationMonitor<T> _monitor;

		public SerializationMonitor(ISerializationMonitor<T> monitor) => _monitor = monitor;

		public void OnSerializing(IFormatWriter writer, object instance)
		{
			_monitor.OnSerializing(writer, (T)instance);
		}

		public void OnSerialized(IFormatWriter writer, object instance)
		{
			_monitor.OnSerialized(writer, (T)instance);
		}

		public void OnActivating(IFormatReader reader, Type instanceType)
		{
			_monitor.OnActivating(reader, instanceType);
		}

		public void OnActivated(object instance)
		{
			_monitor.OnActivated((T)instance);
		}

		public void OnDeserializing(IFormatReader reader, Type instanceType)
		{
			_monitor.OnDeserializing(reader, instanceType);
		}

		public void OnDeserialized(IFormatReader reader, object instance)
		{
			_monitor.OnDeserialized(reader, (T)instance);
		}
	}
}