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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerializer.ContentModel.Conversion.Formatting;
using ExtendedXmlSerializer.ContentModel.Conversion.Parsing;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Xml.Namespacing;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class XmlWriter : Prefixer, IXmlWriter
	{
		readonly static string Xmlns = XNamespace.Xmlns.NamespaceName;
		readonly static PrefixTable PrefixTable = PrefixTable.Default;

		readonly IPrefixTable _prefixes;
		readonly ITypePartsSource _parts;
		readonly IFormatter<MemberInfo> _formatter;

		readonly System.Xml.XmlWriter _writer;
		readonly XmlNamespaceManager _manager;
		readonly Func<TypeParts, TypeParts> _selector;

		public XmlWriter(ITypePartsSource parts, IFormatter<MemberInfo> formatter, System.Xml.XmlWriter writer,
		                 XmlNamespaceManager manager, object root)
			: this(PrefixTable, parts, formatter, writer, manager, root) {}

		public XmlWriter(IPrefixTable prefixes, ITypePartsSource parts, IFormatter<MemberInfo> formatter,
		                 System.Xml.XmlWriter writer, XmlNamespaceManager manager, object root)
		{
			Root = root;
			_prefixes = prefixes;
			_parts = parts;
			_formatter = formatter;
			_writer = writer;
			_manager = manager;
			_selector = Get;

			_manager.PushScope();
		}

		public object Root { get; }

		public void Start(IIdentity identity)
		{
			var identifier = identity.Identifier.NullIfEmpty();
			if (identifier != null)
			{
				_writer.WriteStartElement(identity.Name, identifier);
			}
			else
			{
				_writer.WriteStartElement(identity.Name);
			}
		}

		public void EndCurrent() => _writer.WriteEndElement();

		public System.Xml.XmlWriter Get() => _writer;

		public void Content(string content) => _writer.WriteString(content);

		public void Content(IIdentity property, string content)
		{
			var identifier = property.Identifier.NullIfEmpty();
			var prefix = identifier != null ? Prefix(identifier) : null;
			var name = property.Name;

			if (!string.IsNullOrEmpty(prefix))
			{
				_writer.WriteAttributeString(prefix, name, identifier, content);
			}
			else
			{
				_writer.WriteAttributeString(name, content);
			}
		}

		string IParameterizedSource<string, string>.Get(string parameter) => Prefix(parameter);

		string Prefix(string parameter) => Lookup(parameter) ?? Create(parameter);

		string Lookup(string parameter)
		{
			var lookup = _writer.LookupPrefix(parameter ?? string.Empty);
			if (parameter != null && lookup == string.Empty && !_manager.HasNamespace(parameter))
			{
				_manager.AddNamespace(string.Empty, parameter);
			}

			var result = parameter == _manager.DefaultNamespace ? string.Empty : lookup;
			return result;
		}

		new string Create(string identifier)
		{
			if (!_manager.HasNamespace(identifier))
			{
				var result = _prefixes.Get(identifier) ?? Get(identifier);
				_writer.WriteAttributeString(result, Xmlns, identifier);
				return result;
			}
			return null;
		}

		public string Get(MemberInfo parameter)
			=> parameter is TypeInfo ? GetType((TypeInfo) parameter) : _formatter.Get(parameter);

		string GetType(TypeInfo parameter)
		{
			var typeParts = _parts.Get(parameter);
			var qualified = Get(typeParts);
			var result = TypePartsFormatter.Default.Get(qualified);
			return result;
		}

		TypeParts Get(TypeParts parameter)
		{
			var arguments = parameter.GetArguments();
			var result = new TypeParts(parameter.Name, Prefix(parameter.Identifier),
			                           arguments.HasValue
				                           ? arguments.Value.Select(_selector).ToImmutableArray
				                           : (Func<ImmutableArray<TypeParts>>) null);
			return result;
		}

		public void Dispose()
		{
			_writer.Dispose();
			_manager.PopScope();
		}
	}
}