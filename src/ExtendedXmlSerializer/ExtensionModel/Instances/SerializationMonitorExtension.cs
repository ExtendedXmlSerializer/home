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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class SerializationMonitorExtension : ISerializerExtension
	{
		readonly ITypedTable<ISerializationMonitor> _registrations;

		public SerializationMonitorExtension(ISerializationMonitor @default) : this(new Monitors(@default)) {}

		public SerializationMonitorExtension(ITypedTable<ISerializationMonitor> registrations)
			=> _registrations = registrations;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IActivation>(Register)
		                                                                        .Decorate<IActivators>(Register)
		                                                                        .Decorate<IContents>(Register);

		IContents Register(IServiceProvider _, IContents previous) => new Contents(_registrations, previous);

		IActivators Register(IServiceProvider _, IActivators previous) => new Activators(_registrations, previous);

		IActivation Register(System.IServiceProvider _, IActivation previous)
			=> new Activation(_registrations, previous);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly ITypedTable<ISerializationMonitor> _monitors;
			readonly IContents                          _contents;

			public Contents(ITypedTable<ISerializationMonitor> monitors, IContents contents)
			{
				_monitors = monitors;
				_contents = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var previous = _contents.Get(parameter);
				var result = _monitors.IsSatisfiedBy(parameter)
					             ? Create(parameter, previous)
					             : previous;
				return result;
			}

			Serializer Create(TypeInfo parameter, ISerializer previous)
			{
				var monitor = _monitors.Get(parameter);
				var result = new Serializer(new Reader(monitor, previous), new Writer(monitor, previous));
				return result;
			}
		}

		sealed class Monitors : ITypedTable<ISerializationMonitor>
		{
			readonly ITypedTable<ISerializationMonitor> _table;
			readonly ISerializationMonitor              _default;

			public Monitors(ISerializationMonitor @default) : this(new TypedTable<ISerializationMonitor>(), @default) {}

			public Monitors(ITypedTable<ISerializationMonitor> table, ISerializationMonitor @default)
			{
				_table   = table;
				_default = @default;
			}

			public bool IsSatisfiedBy(TypeInfo parameter) => true;

			public ISerializationMonitor Get(TypeInfo parameter) => _table.Get(parameter) ?? _default;

			public void Assign(TypeInfo key, ISerializationMonitor value)
			{
				_table.Assign(key, value);
			}

			public bool Remove(TypeInfo key) => _table.Remove(key);
		}

		sealed class Activation : IActivation
		{
			readonly ITypedTable<ISerializationMonitor> _table;
			readonly IActivation                        _activation;

			public Activation(ITypedTable<ISerializationMonitor> table, IActivation activation)
			{
				_table      = table;
				_activation = activation;
			}

			public IReader Get(TypeInfo parameter)
				=> new ActivatedReader(parameter, _table.Get(parameter), _activation.Get(parameter));
		}

		sealed class Activators : IActivators
		{
			readonly ITypedTable<ISerializationMonitor> _table;
			readonly IActivators                        _activators;

			public Activators(ITypedTable<ISerializationMonitor> table, IActivators activators)
			{
				_table      = table;
				_activators = activators;
			}

			public IActivator Get(Type parameter)
				=> new Activator(_activators.Get(parameter), _table.Get(parameter.GetTypeInfo()));

			sealed class Activator : IActivator
			{
				readonly IActivator            _activator;
				readonly ISerializationMonitor _monitor;

				public Activator(IActivator activator, ISerializationMonitor monitor)
				{
					_activator = activator;
					_monitor   = monitor;
				}

				public object Get()
				{
					var result = _activator.Get();
					_monitor.OnActivated(result);
					return result;
				}
			}
		}

		sealed class Writer : IWriter
		{
			readonly ISerializationMonitor _monitor;
			readonly IWriter               _writer;

			public Writer(ISerializationMonitor monitor, IWriter writer)
			{
				_monitor = monitor;
				_writer  = writer;
			}

			public void Write(IFormatWriter writer, object instance)
			{
				_monitor.OnSerializing(writer, instance);
				_writer.Write(writer, instance);
				_monitor.OnSerialized(writer, instance);
			}
		}

		sealed class Reader : IReader
		{
			readonly ISerializationMonitor _monitor;
			readonly IReader               _reader;

			public Reader(ISerializationMonitor monitor, IReader reader)
			{
				_monitor = monitor;
				_reader  = reader;
			}

			public object Get(IFormatReader parameter)
			{
				var result = _reader.Get(parameter);
				_monitor.OnDeserialized(parameter, result);
				return result;
			}
		}

		sealed class ActivatedReader : IReader
		{
			readonly TypeInfo              _type;
			readonly ISerializationMonitor _monitor;
			readonly IReader               _reader;

			public ActivatedReader(TypeInfo type, ISerializationMonitor monitor, IReader reader)
			{
				_type    = type;
				_monitor = monitor;
				_reader  = reader;
			}

			public object Get(IFormatReader parameter)
			{
				_monitor.OnActivating(parameter, _type);
				var result = _reader.Get(parameter);
				return result;
			}
		}
	}
}