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
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Xml;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extended Xml Serializer
	/// </summary>
	[UsedImplicitly]
	public class ExtendedXmlSerializer : IExtendedXmlSerializer
	{
		readonly IXmlFactory _factory;
		readonly ISerializers _serialization;

		public ExtendedXmlSerializer(IXmlFactory factory, ISerializers serialization)
		{
			_factory = factory;
			_serialization = serialization;
		}

		public void Serialize(Stream stream, object instance)
		{
			using (var writer = _factory.Create(stream, instance))
			{
				_serialization.Get(instance.GetType().GetTypeInfo()).Write(writer, instance);
			}
		}

		public object Deserialize(Stream stream)
		{
			using (var reader = _factory.Create(stream))
			{
				return _serialization.Get(reader.GetClassification()).Get(reader);
			}
		}
	}
}