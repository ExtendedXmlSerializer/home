using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlWriter : Dictionary<string, string>, IFormatWriter
	{
		readonly static string    Xmlns            = XNamespace.Xmlns.NamespaceName;
		readonly static Delimiter DefaultSeparator = DefaultClrDelimiters.Default.Separator;

		readonly IAliases                   _aliases;
		readonly IIdentifierFormatter       _formatter;
		readonly ITypePartResolver          _parts;
		readonly IPrefix                    _prefixes;
		readonly Func<TypeParts, TypeParts> _selector;
		readonly Delimiter                  _separator;
		readonly IIdentityStore             _store;

		readonly System.Xml.XmlWriter _writer;

		public XmlWriter(IAliases aliases, IIdentifierFormatter formatter, IIdentityStore store,
		                 ITypePartResolver parts, IPrefix prefixes, System.Xml.XmlWriter writer)
			: this(aliases, formatter, store, parts, prefixes, writer, DefaultSeparator) {}

		public XmlWriter(IAliases aliases, IIdentifierFormatter formatter, IIdentityStore store,
		                 ITypePartResolver parts, IPrefix prefixes,
		                 System.Xml.XmlWriter writer, Delimiter separator)
		{
			_aliases   = aliases;
			_formatter = formatter;
			_store     = store;
			_parts     = parts;
			_prefixes  = prefixes;
			_writer    = writer;
			_separator = separator;
			_selector  = Get;
		}

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

		public void Content(string content)
		{
			if (content == null)
			{
				_writer.WriteAttributeString("xsi", NullValueIdentity.Default.Name,
				                             NullValueIdentity.Default.Identifier, "true");
			}
			else
			{
				_writer.WriteString(content);
			}
		}

		public void Verbatim(string content)
		{
			_writer.WriteCData(content);
		}

		public void Content(IIdentity property, string content)
		{
			var identifier = property.Identifier.NullIfEmpty();
			var prefix     = identifier != null ? Prefix(identifier) : null;
			var name       = property.Name;

			if (!string.IsNullOrEmpty(prefix))
			{
				_writer.WriteAttributeString(prefix, name, identifier, content);
			}
			else
			{
				_writer.WriteAttributeString(name, content);
			}
		}

		public string Get(MemberInfo parameter)
			=> parameter is TypeInfo info
				   ? GetType(info)
				   : string.Concat(GetType(parameter.ReflectedType.GetTypeInfo()), _separator, parameter.Name);

		public IIdentity Get(string name, string identifier) => _store.Get(name, Prefix(identifier));

		public void Dispose()
		{
			_writer.Dispose();
			Clear();
		}

		string Prefix(string parameter) => Lookup(parameter) ?? CreatePrefix(parameter);

		string Lookup(string parameter)
		{
			var lookup = _prefixes.Get(parameter);
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

		string GetType(TypeInfo parameter)
		{
			var typeParts = _parts.Get(parameter);
			var qualified = Get(typeParts);
			var result    = TypePartsFormatter.Default.Get(qualified);
			return result;
		}

		TypeParts Get(TypeParts parameter)
		{
			var arguments = parameter.GetArguments();
			var result = new TypeParts(parameter.Name, Prefix(parameter.Identifier),
			                           arguments.HasValue
				                           ? arguments.Value.Select(_selector)
				                                      .ToImmutableArray
				                           : (Func<ImmutableArray<TypeParts>>)null, parameter.Dimensions);
			return result;
		}
	}
}