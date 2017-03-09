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
using System.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ExtensionModel;

namespace ExtendedXmlSerialization.Configuration
{
	public class ExtendedXmlConfiguration : IExtendedXmlConfiguration, IInternalExtendedXmlConfiguration
	{
		readonly IMemberConfiguration _memberConfiguration;

		readonly KeyedByTypeCollection<ISerializerExtension> _extensions = new KeyedByTypeCollection<ISerializerExtension>();

		public ExtendedXmlConfiguration() : this(new MemberConfiguration()) {}

		public ExtendedXmlConfiguration(IMemberConfiguration memberConfiguration)
		{
			_memberConfiguration = memberConfiguration;
		}

		public bool AutoProperties { get; set; }
		public bool Namespaces { get; set; }

		public XmlReaderSettings ReaderSettings { get; set; } = new XmlReaderSettings
		                                                        {
			                                                        IgnoreWhitespace = true,
			                                                        IgnoreComments = true,
			                                                        IgnoreProcessingInstructions = true
		                                                        };

		public XmlWriterSettings WriterSettings { get; set; } = new XmlWriterSettings();
		public IEncryption EncryptionAlgorithm { get; set; }

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

		/*public IExtendedXmlConfiguration UseAutoProperties()
		{
			AutoProperties = true;
			return this;
		}

		public IExtendedXmlConfiguration UseNamespaces()
		{
			Namespaces = true;
			return this;
		}

		public IExtendedXmlConfiguration UseEncryptionAlgorithm(IEncryption propertyEncryption)
		{
			EncryptionAlgorithm = propertyEncryption;
			return this;
		}*/

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
			var instances = new Instances(_memberConfiguration, ReaderSettings, WriterSettings).ToArray();

			using (var services = new ConfiguredServices(instances).Get(_extensions))
			{
				var result = services.Get<IExtendedXmlSerializer>();
				return result;
			}
		}

		public void Extend(ISerializerExtension extension) => _extensions.Add(extension);
	}
}