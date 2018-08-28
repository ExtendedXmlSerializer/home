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

using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class PrefixRegistryExtension : ISerializerExtension
	{
		readonly IDictionary<Type, string> _registry;

		public PrefixRegistryExtension(IDictionary<Type, string> registry) => _registry = registry;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IFormatWriters<System.Xml.XmlWriter>, FormatWriters>()
			            .RegisterInstance<IPrefixRegistry>(new PrefixRegistry(_registry));

		public void Execute(IServices parameter) {}

		interface IPrefixRegistry : IParameterizedSource<Type, string> {}

		sealed class PrefixRegistry : TableSource<Type, string>, IPrefixRegistry
		{
			public PrefixRegistry(IDictionary<Type, string> store) : base(store) {}
		}

		sealed class FormatWriters : IFormatWriters<System.Xml.XmlWriter>
		{
			readonly IPrefixRegistry                      _registry;
			readonly ITypes                               _types;
			readonly IFormatWriters<System.Xml.XmlWriter> _writers;

			public FormatWriters(IPrefixRegistry registry, ITypes types, IFormatWriters<System.Xml.XmlWriter> writers)
			{
				_registry = registry;
				_types    = types;
				_writers  = writers;
			}

			public IFormatWriter Get(System.Xml.XmlWriter parameter)
				=> new Writer(_registry, _types, _writers.Get(parameter), parameter);

			sealed class Writer : IFormatWriter
			{
				readonly IPrefixRegistry      _registry;
				readonly ITypes               _types;
				readonly IFormatWriter        _writer;
				readonly System.Xml.XmlWriter _native;

				public Writer(IPrefixRegistry registry, ITypes types, IFormatWriter writer, System.Xml.XmlWriter native)
				{
					_registry = registry;
					_types    = types;
					_writer   = writer;
					_native   = native;
				}

				public object Get() => _writer.Get();

				public IIdentity Get(string name, string identifier) => _writer.Get(name, identifier);

				public void Dispose()
				{
					_writer.Dispose();
				}

				public string Get(MemberInfo parameter) => _writer.Get(parameter);

				public void Start(IIdentity identity)
				{
					if (!Write(identity))
					{
						_writer.Start(identity);
					}
				}

				bool Write(IIdentity identity)
				{
					var prefix = _registry.Get(_types.Get(identity));
					if (prefix != null)
					{
						var identifier = identity.Identifier.NullIfEmpty();
						if (identifier != null)
						{
							_native.WriteStartElement(prefix, identity.Name, identifier);
							return true;
						}
					}

					return false;
				}

				public void EndCurrent()
				{
					_writer.EndCurrent();
				}

				public void Content(IIdentity property, string content)
				{
					_writer.Content(property, content);
				}

				public void Content(string content)
				{
					_writer.Content(content);
				}

				public void Verbatim(string content)
				{
					_writer.Verbatim(content);
				}
			}
		}
	}
}