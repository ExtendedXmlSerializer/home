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
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class ActivationAwarenessExtension : ISerializerExtension
	{
		readonly ITypedTable<IDeserializationAware> _registrations;

		public ActivationAwarenessExtension(IDeserializationAware @default) : this(new Table(@default)) {}

		public ActivationAwarenessExtension(ITypedTable<IDeserializationAware> registrations)
			=> _registrations = registrations;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IActivation>(Register)
		                                                                        .Decorate<IActivators>(Register);

		IActivators Register(IServiceProvider _, IActivators previous) => new Activators(_registrations, previous);

		IActivation Register(System.IServiceProvider _, IActivation previous)
			=> new Activation(_registrations, previous);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Table : ITypedTable<IDeserializationAware>
		{
			readonly ITypedTable<IDeserializationAware> _table;
			readonly IDeserializationAware              _default;

			public Table(IDeserializationAware @default) : this(new TypedTable<IDeserializationAware>(), @default) {}

			public Table(ITypedTable<IDeserializationAware> table, IDeserializationAware @default)
			{
				_table   = table;
				_default = @default;
			}

			public bool IsSatisfiedBy(TypeInfo parameter) => true;

			public IDeserializationAware Get(TypeInfo parameter) => _table.Get(parameter) ?? _default;

			public void Assign(TypeInfo key, IDeserializationAware value)
			{
				_table.Assign(key, value);
			}

			public bool Remove(TypeInfo key) => _table.Remove(key);
		}

		sealed class Activation : IActivation
		{
			readonly ITypedTable<IDeserializationAware> _table;
			readonly IActivation                        _activation;

			public Activation(ITypedTable<IDeserializationAware> table, IActivation activation)
			{
				_table      = table;
				_activation = activation;
			}

			public IReader Get(TypeInfo parameter)
				=> new Reader(parameter, _table.Get(parameter), _activation.Get(parameter));
		}

		sealed class Activators : IActivators
		{
			readonly ITypedTable<IDeserializationAware> _table;
			readonly IActivators                        _activators;

			public Activators(ITypedTable<IDeserializationAware> table, IActivators activators)
			{
				_table      = table;
				_activators = activators;
			}

			public IActivator Get(Type parameter)
				=> new Activator(_activators.Get(parameter), _table.Get(parameter.GetTypeInfo()));

			sealed class Activator : IActivator
			{
				readonly IActivator            _activator;
				readonly IDeserializationAware _aware;

				public Activator(IActivator activator, IDeserializationAware aware)
				{
					_activator = activator;
					_aware     = aware;
				}

				public object Get()
				{
					var result = _activator.Get();
					_aware.OnActivated(result);
					return result;
				}
			}
		}

		sealed class Reader : IReader
		{
			readonly TypeInfo              _type;
			readonly IDeserializationAware _aware;
			readonly IReader               _reader;

			public Reader(TypeInfo type, IDeserializationAware aware, IReader reader)
			{
				_type   = type;
				_aware  = aware;
				_reader = reader;
			}

			public object Get(IFormatReader parameter)
			{
				_aware.OnActivating(_type, parameter);
				var result = _reader.Get(parameter);
				_aware.OnDeserialized(result, parameter);
				return result;
			}
		}
	}
}