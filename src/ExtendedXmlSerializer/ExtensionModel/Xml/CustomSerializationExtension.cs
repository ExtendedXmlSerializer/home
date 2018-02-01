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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CustomSerializationExtension : ISerializerExtension
	{
		public CustomSerializationExtension() : this(new CustomXmlSerializers(), new CustomSerializers(),
		                                             new MemberCustomSerializers()) {}

		public CustomSerializationExtension(ICustomXmlSerializers xmlSerializers, ICustomSerializers types,
		                                    ICustomMemberSerializers members)
		{
			XmlSerializers = xmlSerializers;
			Types = types;
			Members = members;
		}

		public ICustomXmlSerializers XmlSerializers { get; }

		public ICustomSerializers Types { get; }

		public ICustomMemberSerializers Members { get; }

		public IServiceRepository Get(IServiceRepository parameter) => Extensions()
		                                                               .Aggregate(parameter,
		                                                                          (repository, serializer) =>
			                                                                          serializer.Get(repository))
		                                                               .RegisterInstance(XmlSerializers)
		                                                               .RegisterInstance(Types)
		                                                               .RegisterInstance(Members)
		                                                               .Register<RegisteredMemberContents>()
		                                                               .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter)
		{
			foreach (var extension in Extensions())
			{
				extension.Execute(parameter);
			}
		}

		IEnumerable<ISerializerExtension> Extensions() => new ISerializerExtension[] {XmlSerializers, Types, Members};

		sealed class Contents : IContents
		{
			readonly ICustomXmlSerializers _custom;
			readonly IContents _contents;

			public Contents(ICustomXmlSerializers custom, IContents contents)
			{
				_custom = custom;
				_contents = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var custom = _custom.Get(parameter);
				var result = custom != null ? new Serializer(custom) : _contents.Get(parameter);
				return result;
			}

			sealed class Serializer : ISerializer
			{
				readonly IExtendedXmlCustomSerializer _custom;

				public Serializer(IExtendedXmlCustomSerializer custom) => _custom = custom;

				public object Get(IFormatReader parameter)
				{
					var reader = parameter.Get()
					                      .AsValid<System.Xml.XmlReader>();
					var subtree = reader.ReadSubtree();
					var element = XElement.Load(subtree);
					var result = _custom.Deserialize(element);
					return result;
				}

				public void Write(IFormatWriter writer, object instance)
					=> _custom.Serializer(writer.Get()
					                            .AsValid<System.Xml.XmlWriter>(), instance);
			}
		}
	}
}