// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;

namespace ExtendedXmlSerialization.Configuration
{
	public class ExtendedXmlConfiguration : IExtendedXmlConfiguration, IInternalExtendedXmlConfiguration
	{
		readonly IExtendedXmlSerializerFactory _factory;
		readonly IDictionary<MemberInfo, IConverter> _converters;
		readonly IMemberEmitSpecifications _emit;
		readonly IDictionary<MemberInfo, IRuntimeMemberSpecification> _runtime;

		public ExtendedXmlConfiguration()
			: this(ExtendedXmlSerializerFactory.Default) {}

		public ExtendedXmlConfiguration(IExtendedXmlSerializerFactory factory)
			: this(
				factory,
				new Dictionary<MemberInfo, IConverter>(),
				new Dictionary<MemberInfo, IMemberEmitSpecification>(),
				new Dictionary<MemberInfo, IRuntimeMemberSpecification>()) {}

		public ExtendedXmlConfiguration(IExtendedXmlSerializerFactory factory,
		                                IDictionary<MemberInfo, IConverter> converters,
		                                IDictionary<MemberInfo, IMemberEmitSpecification> emit,
		                                IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
			: this(factory, converters, new MemberEmitSpecifications(emit), runtime) {}

		public ExtendedXmlConfiguration(IExtendedXmlSerializerFactory factory,
		                                IDictionary<MemberInfo, IConverter> converters,
		                                IMemberEmitSpecifications emit,
		                                IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
		{
			_factory = factory;
			_converters = converters;
			_emit = emit;
			_runtime = runtime;
		}

		public bool AutoProperties { get; set; }
		public bool Namespaces { get; set; }
		public XmlReaderSettings ReaderSettings { get; set; }
		public XmlWriterSettings WriterSettings { get; set; }
		public IPropertyEncryption EncryptionAlgorithm { get; set; }

		IExtendedXmlTypeConfiguration IInternalExtendedXmlConfiguration.GetTypeConfiguration(Type type)
		{
			return _cache.ContainsKey(type) ? _cache[type] : null;
		}

		readonly Dictionary<Type, IExtendedXmlTypeConfiguration> _cache =
			new Dictionary<Type, IExtendedXmlTypeConfiguration>();

		public IExtendedXmlTypeConfiguration<T> ConfigureType<T>()
		{
			var configType = new ExtendedXmlTypeConfiguration<T>();

			_cache.Add(typeof(T), configType);
			return configType;
		}

		public IExtendedXmlConfiguration UseAutoProperties()
		{
			AutoProperties = true;
			return this;
		}

		public IExtendedXmlConfiguration UseNamespaces()
		{
			Namespaces = true;
			return this;
		}

		public IExtendedXmlConfiguration UseEncryptionAlgorithm(IPropertyEncryption propertyEncryption)
		{
			EncryptionAlgorithm = propertyEncryption;
			return this;
		}

		public IExtendedXmlConfiguration WithSettings(XmlReaderSettings readerSettings)
		{
			ReaderSettings = readerSettings;
			return this;
		}

		public IExtendedXmlConfiguration WithSettings(XmlWriterSettings writerSettings)
		{
			WriterSettings = writerSettings;
			return this;
		}

		public IExtendedXmlConfiguration WithSettings(XmlReaderSettings readerSettings, XmlWriterSettings writerSettings)
		{
			ReaderSettings = readerSettings;
			WriterSettings = writerSettings;
			return this;
		}

		public IExtendedXmlSerializer Create()
		{
			var policy = Defaults.MemberPolicy;

			var serializers = new MemberSerializers(new RuntimeMemberSpecifications(_runtime), new MemberConverters(_converters));
			var configuration = new SerializationConfiguration(_emit, serializers, TypeSelector.Default, MemberAliases.Default,
			                                                   policy);
			var result = _factory.Get(configuration);
			return result;
		}
	}
}