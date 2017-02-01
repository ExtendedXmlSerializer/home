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

using System.IO;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core.Sources;
using INames = ExtendedXmlSerialization.Conversion.Names.INames;
using Names = ExtendedXmlSerialization.Conversion.Names.Names;
using XmlReader = ExtendedXmlSerialization.Conversion.Xml.XmlReader;
using XmlWriter = ExtendedXmlSerialization.Conversion.Xml.XmlWriter;

namespace ExtendedXmlSerialization
{
	/// <summary>
	/// Extended Xml Serializer
	/// </summary>
	/*public class ExtendedXmlSerializer : SelectingConverterBase, IExtendedXmlSerializer, IRootConverter
	{
		readonly IXmlContextFactory _factory;
		readonly IConverterSelector _selector;

		public ExtendedXmlSerializer(IXmlContextFactory factory, IConverterOptions options)
		{
			_factory = factory;
			_selector = options.Get(this);
		}

		public void Serialize(Stream stream, object instance)
		{
			using (var writer = XmlWriter.Create(stream))
			{
				Write(_factory.Create(writer, instance), instance);
			}
		}

		public object Deserialize(Stream stream) => Read(_factory.Create(stream));

		protected override IConverter Select(IContext context)
			=> _selector.Get(context.Container.GetType().GetTypeInfo()) ?? _selector.Get(context);

		IConverter IParameterizedSource<IContext, IConverter>.Get(IContext parameter) => _selector.Get(parameter);

		IConverter IParameterizedSource<TypeInfo, IConverter>.Get(TypeInfo parameter) => _selector.Get(parameter);
	}*/
	public class ExtendedXmlSerializer : CacheBase<TypeInfo, IConverter>, IExtendedXmlSerializer
	{
		readonly static XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
		                                                      {
			                                                      IgnoreWhitespace = true,
			                                                      IgnoreComments = true,
			                                                      IgnoreProcessingInstructions = true
		                                                      };

		readonly INames _names;
		readonly IConverters _converters;
		readonly INamespaces _namespaces;
		readonly ITypeLocator _type;
		readonly XmlReaderSettings _settings;

		public ExtendedXmlSerializer() : this(new Names(), new Namespaces()) {}

		public ExtendedXmlSerializer(INames names, INamespaces namespaces)
			: this(
				names, new Converters(names), namespaces, new TypeLocator(new Types(namespaces, new TypeContexts())),
				XmlReaderSettings) {}

		public ExtendedXmlSerializer(INames names, IConverters converters, INamespaces namespaces, ITypeLocator type,
		                             XmlReaderSettings settings)
		{
			_names = names;
			_converters = converters;
			_namespaces = namespaces;
			_type = type;
			_settings = settings;
		}

		public void Serialize(Stream stream, object instance)
		{
			using (var writer = System.Xml.XmlWriter.Create(stream))
			{
				var emitter = new XmlWriter(_namespaces, writer);
				var context = Get(instance.GetType().GetTypeInfo());
				context.Emit(emitter, instance);
			}
		}

		public object Deserialize(Stream stream)
		{
			using (var reader = System.Xml.XmlReader.Create(stream, _settings))
			{
				var yielder = new XmlReader(_type, reader);
				var context = Get(yielder.Classification);
				var result = context.Get(yielder);
				return result;
			}
		}

		protected override IConverter Create(TypeInfo parameter)
			=> new RootConverter(_names.Get(parameter), _converters.Get(parameter));
	}
}