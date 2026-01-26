using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using SystemXmlReader = System.Xml.XmlReader;

namespace ExtendedXmlSerializer.ExtensionModel.Xml;

sealed class RegressableXmlReader : SystemXmlReader, IFormatReader, IRegressableReader, IXmlLineInfo
{
    readonly IFormatReaderContext  _context;
    readonly SystemXmlReader       _reader;
    readonly XDocument             _document;
    readonly string                _defaultNamespace;
    XObject                        _current;
    bool                           _disposed;
    bool                           _onEnd;


    public RegressableXmlReader(IFormatReaderContext context, SystemXmlReader reader)
        : this(context, reader, reader.LookupNamespace(string.Empty)) { }

    public RegressableXmlReader(IFormatReaderContext context, SystemXmlReader reader, string defaultNamespace)
    {
        _reader = reader;
        _context = context;
        _document = XDocument.Load(reader, LoadOptions.SetLineInfo);
        _defaultNamespace = defaultNamespace;
        _current = _document;
    }

    static XElement ElementFor(XObject node)
    {
        return node switch
        {
            XElement e => e,
            XAttribute a => a.Parent,
            XText t => t.Parent,
            XComment c => c.Parent,
            XProcessingInstruction p => p.Parent,
            XDocument d => d.Root,
            _ => null
        };
    }

    XAttribute FindAttribute(XElement element, string localName, string ns)
    {
        if (element == null) return null;
        var targetNs = ns == _defaultNamespace ? string.Empty : ns;
        return element.Attributes().FirstOrDefault(a =>
               a.Name.LocalName == localName &&
               a.Name.NamespaceName == targetNs);
    }

    static (XObject next, bool end) NextInDocumentOrder(XObject current, bool end)
    {
        if (current is XDocument doc) return (doc.Root, false);
        if (!end && current is XElement el)
        {
            var first = el.Nodes().FirstOrDefault();
            if (first != null) return (first, false);
            // go to XNode case
        }
        if (current is XNode node)
        {
            var next = NextSibling(node);
            if (next != null) return (next, false);
            var parent = node.Parent;
            if (parent == null)
                return (null, false);
            return (parent, true);
        }
        if (current is XAttribute attr)
        {
            var owner = attr.Parent;
            if (owner == null) return (null, true);
            var first = owner.Nodes().FirstOrDefault();
            if (first != null) return (first, false);
            // it's not a recusion owner is an XElement
            return NextInDocumentOrder(owner, false);
        }
        return (null, false);
    }

    static XNode NextSibling(XNode node)
        => node?.NodesAfterSelf().FirstOrDefault();

    public override int AttributeCount
    {
        get
        {
            if (!HasAttribute()) return 0;
            var el = ElementFor(_current);
            return el?.Attributes().Count() ?? 0;
        }
    }

    public override string BaseURI => _reader.BaseURI;

    public override int Depth
    {
        get
        {
            var depth = 0;
            if (_current is not XElement el)
            {
                el = ElementFor(_current);
                if (el == null) return 0;
                depth++;
            }
            for (var parent = el.Parent; parent != null; parent = parent.Parent)
            {
                if (parent is XElement) depth++;
            }
            return depth;
        }
    }

    public override bool EOF => _current == null;

    public override bool IsEmptyElement => _current is XElement el && !el.Nodes().Any();

    public override string LocalName
    {
        get
        {
            return _current switch
            {
                XElement e => e.Name.LocalName,
                XAttribute a => a.Name.LocalName,
                XProcessingInstruction p => p.Target,
                _ => string.Empty
            };
        }
    }

    string IIdentity.Name => LocalName;

    public override string NamespaceURI
    {
        get
        {
            return _current switch
            {
                XElement e => e.Name.NamespaceName,
                XAttribute a => a.Name.NamespaceName,
                _ => string.Empty
            };
        }
    }

    public override XmlNameTable NameTable => _reader.NameTable;

    public override XmlNodeType NodeType
    {
        get
        {
            if (_onEnd) return XmlNodeType.EndElement;
            if (_current == _document) return XmlNodeType.None;
            return _current.NodeType;
        }
    }

    public override string Prefix
    {
        get
        {
            if (_current is XElement e)
            {
                var prefix = e.GetPrefixOfNamespace(e.Name.Namespace);
                return prefix ?? string.Empty;
            }
            if (_current is XAttribute a)
            {
                var owner = a.Parent;
                var prefix = owner?.GetPrefixOfNamespace(a.Name.Namespace);
                return prefix ?? string.Empty;
            }
            return string.Empty;
        }
    }

    public override ReadState ReadState => _disposed ? ReadState.Closed : ReadState.Interactive;

    public override string Value
    {
        get
        {
            return _current switch
            {
                XAttribute a => a.Value,
                XText t => t.Value,
                XComment c => c.Value,
                XProcessingInstruction p => p.Data,
                _ => string.Empty
            };
        }
    }

    string IIdentity.Identifier => NamespaceURI;

    public bool IsRegressable => HasLineInfo();

    public int LineNumber
    {
        get
        {
            if (_current is IXmlLineInfo info && info.HasLineInfo())
                return info.LineNumber;
            return 0;
        }
    }

    public int LinePosition
    {
        get
        {
            if (_current is IXmlLineInfo info && info.HasLineInfo())
                return info.LinePosition;
            return 0;
        }
    }

    public string Content()
    {
        switch (NodeType)
        {
            case XmlNodeType.Attribute:
                return Value;
            default:
                if (IsEmptyElement && CanReadValueChunk)
                {
                    MoveToContent();
                    return string.Empty;
                }

                var isNull = IsSatisfiedBy(NullValueIdentity.Default);

                if (!isNull)
                {
                    Read();
                }

                var result = isNull ? null : Value;

                if (NodeType == XmlNodeType.CDATA)
                {
                    return CharacterData(result);
                }
                else if (!string.IsNullOrEmpty(result))
                {
                    Read();
                    MoveToContent();
                }


                return result;
        }

        string CharacterData(string result)
        {
            Read();
            MoveToContent();

            if (NodeType == XmlNodeType.CDATA)
            {
                var builder = new StringBuilder(result);
                while (NodeType == XmlNodeType.CDATA)
                {
                    builder.Append(Value);
                    Read();
                    MoveToContent();
                }

                return builder.ToString();
            }

            return result;
        }
    }

    object Core.Sources.ISource<object>.Get() => this;

    public IIdentity Get(string name, string identifier) => _context.Get(name, identifier);

    public MemberInfo Get(string parameter) => _context.Get(parameter);

    public override string GetAttribute(int i)
    {
        if (!HasAttribute()) return default;
        var el = ElementFor(_current);
        var attr = el?.Attributes().ElementAtOrDefault(i);
        return attr?.Value;
    }

    public override string GetAttribute(string name)
    {
        if (!HasAttribute()) return default;
        var attr = FindAttribute(ElementFor(_current), name, _defaultNamespace);
        return attr?.Value;
    }

    public override string GetAttribute(string name, string namespaceURI)
    {
        if (!HasAttribute()) return default;
        var attr = FindAttribute(ElementFor(_current), name, namespaceURI);
        return attr?.Value;
    }

    public ReaderPosition GetPosition()
    {
        if (_current is not IXmlLineInfo info || !info.HasLineInfo())
            throw new InvalidExpressionException("No ligne info available.");
        else if (!_onEnd)
            return new ReaderPosition(info.LinePosition, info.LineNumber);
        else
            return new ReaderPosition(-info.LinePosition, info.LineNumber);
    }

    public bool IsSatisfiedBy(IIdentity parameter)
    {
        var el = ElementFor(_current);
        if (el == null) return false;
        var ns = parameter.Identifier == _defaultNamespace ? string.Empty : parameter.Identifier;
        return el.Attributes().Any(a => a.Name.LocalName == parameter.Name && a.Name.NamespaceName == ns);
    }

    public override string LookupNamespace(string prefix)
    {
        var el = ElementFor(_current) ?? _document.Root;
        if (el == null) return null;
        if (string.IsNullOrEmpty(prefix))
        {
            var defaultNs = el.GetDefaultNamespace();
            if (defaultNs != null && !string.IsNullOrEmpty(defaultNs.NamespaceName))
                return defaultNs.NamespaceName;
            return _defaultNamespace;
        }
        var ns = el.GetNamespaceOfPrefix(prefix);
        if (ns != null) return ns.NamespaceName;
        return null;
    }

    public override bool MoveToAttribute(string name)
    {
        if (!HasAttribute()) return false;
        var attr = FindAttribute(ElementFor(_current), name, _defaultNamespace);
        if (attr == null) return false;
        _current = attr;
        return true;
    }

    public override bool MoveToAttribute(string name, string ns)
    {
        if (!HasAttribute()) return false;
        var attr = FindAttribute(ElementFor(_current), name, ns);
        if (attr == null) return false;
        _current = attr;
        return true;
    }

    public override bool MoveToElement()
    {
        if (_current is XAttribute a && a.Parent != null)
        {
            _current = a.Parent;
            return true;
        }

        if (_current is XNode n && n.Parent is XElement p)
        {
            _current = p;
            return true;
        }

        return false;
    }

    public override bool MoveToFirstAttribute()
    {
        if (!HasAttribute()) return false;
        var el = ElementFor(_current);
        var first = el?.Attributes().FirstOrDefault();
        if (first == null) return false;
        _current = first;
        return true;
    }

    public override bool MoveToNextAttribute()
    {
        if (!HasAttribute()) return false;
        if (_current is not XAttribute a) return MoveToFirstAttribute();
        var el = a.Parent;
        if (el == null) return false;
        var list = el.Attributes().ToList();
        var idx = list.IndexOf(a);
        if (idx < 0 || idx + 1 >= list.Count) return false;
        _current = list[idx + 1];
        return true;
    }

    public override bool Read()
    {
        (XObject next, bool end) = NextInDocumentOrder(_current, _onEnd);
        _current = next;
        _onEnd = end;
        return _current != null;
    }

    public override bool ReadAttributeValue()
    {
        // attributes in LINQ to XML are atomic (no separate attribute value nodes)
        return false;
    }

    public override void ResolveEntity()
    {
        // No entity resolving required for XDocument-backed reader
    }

    void IFormatReader.Set() => MoveToContent();

    public bool SetPosition(ReaderPosition position)
    {
        if (position == null) return false;

        bool onEnd = position.LinePosition < 0;
        if (onEnd) position.LinePosition = -position.LinePosition;

        foreach (var node in _document.Root.DescendantNodesAndSelf())
        {
            if (node is IXmlLineInfo eli && eli.HasLineInfo() 
                && eli.LineNumber == position.LineNumber
                && eli.LinePosition == position.LinePosition)
            {
                _current = node;
                _onEnd = onEnd;
                return true;
            }

            if (node is XElement el)
            {
                foreach (var a in el.Attributes())
                {
                    if (a is IXmlLineInfo ai && ai.HasLineInfo()
                        && ai.LineNumber == position.LineNumber
                        && ai.LinePosition == position.LinePosition)
                    {
                        _current = a;
                        _onEnd = onEnd;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override string ToString() => $"{base.ToString()}: {IdentityFormatter.Default.Get(this)}";

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && !_disposed)
        {
            _context.Dispose();
            _reader.Dispose();
            _disposed = true;
        }
    }

    public bool HasLineInfo()
    {
        return _current is IXmlLineInfo info && info.HasLineInfo();
    }

    private bool HasAttribute()
    {
        return !_onEnd && _current is not XText and not XComment and not XProcessingInstruction;
    }                                                  
}