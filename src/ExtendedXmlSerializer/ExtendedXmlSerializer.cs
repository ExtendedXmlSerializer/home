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

namespace ExtendedXmlSerialization
{
	/// <summary>
	/// Extended Xml Serializer
	/// </summary>
	public class ExtendedXmlSerializer : SelectingConverterBase, IExtendedXmlSerializer, IRootConverter
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
	}
}