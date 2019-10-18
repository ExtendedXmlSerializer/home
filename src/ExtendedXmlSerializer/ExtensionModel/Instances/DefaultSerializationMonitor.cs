// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.ContentModel.Format;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class DefaultSerializationMonitor : ISerializationMonitor
	{
		public static DefaultSerializationMonitor Default { get; } = new DefaultSerializationMonitor();

		DefaultSerializationMonitor() {}

		public void OnSerializing(IFormatWriter writer, object instance) {}

		public void OnSerialized(IFormatWriter writer, object instance) {}

		public void OnActivating(IFormatReader reader, TypeInfo activating) {}

		public void OnActivated(object instance) {}

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

		public void OnActivating(IFormatReader reader, TypeInfo activating)
		{
			_monitor.OnActivating(reader, activating);
		}

		public void OnActivated(object instance)
		{
			_monitor.OnActivated((T)instance);
		}

		public void OnDeserialized(IFormatReader reader, object instance)
		{
			_monitor.OnDeserialized(reader, (T)instance);
		}
	}
}