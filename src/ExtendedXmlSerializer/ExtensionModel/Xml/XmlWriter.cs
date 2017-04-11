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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlWriter : Dictionary<string, string>, IFormatWriter
	{
		readonly static string Xmlns = XNamespace.Xmlns.NamespaceName;
		readonly static Delimiter DefaultSeparator = DefaultClrDelimiters.Default.Separator;

		readonly IAliases _aliases;
		readonly IIdentifierFormatter _formatter;
		readonly IIdentityStore _store;
		readonly ITypePartResolver _parts;

		readonly System.Xml.XmlWriter _writer;
		readonly Delimiter _separator;
		readonly Func<TypeParts, TypeParts> _selector;

		public XmlWriter(IAliases aliases, IIdentifierFormatter formatter, IIdentityStore store, ITypePartResolver parts,
		                 Writing<System.Xml.XmlWriter> parameter)
			: this(aliases, formatter, store, parts, parameter.Writer, parameter.Instance, DefaultSeparator) {}

		public XmlWriter(IAliases aliases, IIdentifierFormatter formatter, IIdentityStore store, ITypePartResolver parts,
		                 System.Xml.XmlWriter writer, object instance,
		                 Delimiter separator)
		{
			_aliases = aliases;
			_formatter = formatter;
			_store = store;
			_parts = parts;
			_writer = writer;
			_separator = separator;
			Instance = instance;
			_selector = Get;
		}

		public object Instance { get; }
		public object Get() => _writer;

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

		string Prefix(string parameter) => Lookup(parameter) ?? CreatePrefix(parameter);

		string Lookup(string parameter)
		{
			var lookup = _writer.LookupPrefix(parameter ?? string.Empty);
			if (parameter != null && lookup == string.Empty && !ContainsKey(string.Empty))
			{
				Add(string.Empty, parameter);
			}

			var result = parameter == this.Get(string.Empty) ? string.Empty : lookup;
			return result;
		}

		string CreatePrefix(string identifier)
		{
			if (!ContainsKey(identifier))
			{
				var result = _aliases.Get(identifier) ?? Create(identifier);
				_writer.WriteAttributeString(result, Xmlns, identifier);
				return result;
			}
			return null;
		}

		string Create(string parameter)
		{
			var result = _formatter.Get(Count + 1);
			Add(result, parameter);
			return result;
		}

		public string Get(MemberInfo parameter)
			=> parameter is TypeInfo
				? GetType((TypeInfo) parameter)
				: string.Concat(GetType(parameter.GetReflectedType()), _separator, parameter.Name);

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

		public IIdentity Get(string name, string identifier) => _store.Get(name, Prefix(identifier));

		public void Dispose()
		{
			_writer.Dispose();
			Clear();
		}
	}
}