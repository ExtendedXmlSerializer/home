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
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Extensions;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Configuration
{
	public class ExtendedXmlConfiguration : IExtendedXmlConfiguration, IInternalExtendedXmlConfiguration
	{
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;
		readonly IDictionary<MemberInfo, IConverter> _converters;
		readonly IMemberEmitSpecifications _emit;
		readonly IDictionary<MemberInfo, IRuntimeMemberSpecification> _runtime;
		readonly KeyedByTypeCollection<ISerializerExtension> _extensions = new KeyedByTypeCollection<ISerializerExtension>();

		public ExtendedXmlConfiguration()
			: this(new Dictionary<MemberInfo, IConverter>(),
			       new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			       new Dictionary<MemberInfo, IRuntimeMemberSpecification>()) {}

		public ExtendedXmlConfiguration(IDictionary<MemberInfo, IConverter> converters,
		                                IDictionary<MemberInfo, IMemberEmitSpecification> emit,
		                                IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
			: this(Defaults.Property, Defaults.Field, converters, emit, runtime) {}

		public ExtendedXmlConfiguration(
			ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field,
			IDictionary<MemberInfo, IConverter> converters,
			IDictionary<MemberInfo, IMemberEmitSpecification> emit,
			IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
			: this(property, field, converters, new MemberEmitSpecifications(emit), runtime) {}

		public ExtendedXmlConfiguration(
			ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field,
			IDictionary<MemberInfo, IConverter> converters,
			IMemberEmitSpecifications emit,
			IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
		{
			_property = property;
			_field = field;
			_converters = converters;
			_emit = emit;
			_runtime = runtime;
		}

		public bool AutoProperties { get; set; }
		public bool Namespaces { get; set; }

		public XmlReaderSettings ReaderSettings { get; set; } = new XmlReaderSettings
			{
				IgnoreWhitespace = true,
				IgnoreComments = true,
				IgnoreProcessingInstructions = true
			};

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

			var writers = new MemberWriters(new RuntimeMemberSpecifications(_runtime), new MemberConverters(_converters));
			var alteration = OptimizedConverterAlteration.Default;
			var converters = WellKnownConverters.Default;
			var content = new CompositeContentOption(
				converters
					.Select(alteration.ToContent)
					.Appending(new EnumerationContentOption(alteration))
					.ToArray()
			);

			var services = new SerializationServices(
				_property.And(policy), _field.And(policy), _emit, new XmlFactory(ReaderSettings), writers, content,
				MemberAliases.Default, MemberOrder.Default, _extensions
			);

			var result = services.Get<IExtendedXmlSerializer>() ??
			             new ExtendedXmlSerializer(services.GetValid<IXmlFactory>(), services);
			return result;
		}

		public IExtendedXmlConfiguration Extend(params ISerializerExtension[] extensions)
		{
			foreach (var extension in extensions)
			{
				_extensions.Add(extension);
			}
			return this;
		}
	}
}