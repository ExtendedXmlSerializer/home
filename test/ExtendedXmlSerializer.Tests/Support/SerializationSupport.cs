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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.Tests.Support
{
	public interface ISerializersSupport : ISerializers
	{
		T Assert<T>(T instance, string expected);
	}


	sealed class SerializersSupport : ISerializersSupport
	{
		readonly ISerializers _serializer;

		public SerializersSupport() : this(new ConfigurationRoot()) { }

		public SerializersSupport(IConfigurationRoot configuration)
			: this(ServiceActivator<ISerializers>.Default.Get(configuration.ToImmutableArray())) { }

		public SerializersSupport(ISerializers serializer) => _serializer = serializer;

		public T Assert<T>(T instance, string expected)
		{
			var data = _serializer.Serialize(instance);
			data?.Replace("\r\n", string.Empty)
			    .Replace("\n", string.Empty)
			    .Should()
			    .Be(expected?.Replace("\r\n", string.Empty)
			                .Replace("\n", string.Empty));
			var result = _serializer.Deserialize<T>(data);
			return result;
		}


		// Used for a simple way to emit instances as text in tests:
		public void WriteLine<T>(T instance) => throw new InvalidOperationException(_serializer.Serialize(instance));
		public ISerializer<object> Get(TypeInfo parameter) => _serializer.Get(parameter);
	}


	sealed class SerializationSupport : ISerializationSupport
	{
		readonly IExtendedXmlSerializer _serializer;

		public SerializationSupport() : this(new ConfigurationContainer()) {}

		public SerializationSupport(IConfigurationElement configuration) : this(configuration.Create()) {}

		public SerializationSupport(IExtendedXmlSerializer serializer)
		{
			_serializer = serializer;
		}

		public T Assert<T>(T instance, string expected)
		{
			var data = _serializer.Serialize(instance);
			data?.Replace("\r\n", string.Empty)
			    .Replace("\n", string.Empty)
			    .Should()
			    .Be(expected?.Replace("\r\n", string.Empty)
			                .Replace("\n", string.Empty));
			var result = _serializer.Deserialize<T>(data);
			return result;
		}

		public void Serialize(System.Xml.XmlWriter writer, object instance) => _serializer.Serialize(writer, instance);

		public object Deserialize(System.Xml.XmlReader stream) => _serializer.Deserialize(stream);

		// Used for a simple way to emit instances as text in tests:
		public void WriteLine(object instance) => throw new InvalidOperationException(_serializer.Serialize(instance));
	}
}