using System;
using System.Xml;
using ExtendedXmlSerialization.Conversion.Model.Names;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion
{
	public class XmlEmitter : IEmitter
	{
		readonly XmlWriter _writer;
		readonly INamespaces _namespaces;
		readonly IDisposable _finish;

		public XmlEmitter(INamespaces namespaces, XmlWriter writer)
			: this(namespaces, writer, new DelegatedDisposable(writer.WriteEndElement)) {}

		public XmlEmitter(INamespaces namespaces, XmlWriter writer, IDisposable finish)
		{
			_writer = writer;
			_namespaces = namespaces;
			_finish = finish;
		}

		public IDisposable Emit(IName name)
		{
			if (name is IMemberName)
			{
				_writer.WriteStartElement(name.DisplayName);

				/*var type = instance.GetType().GetTypeInfo();
				if (!context.Container.Exact(type))
				{
					context.Write(TypeProperty.Default, type);
				}*/
			}
			else
			{
				_writer.WriteStartElement(name.DisplayName, _namespaces.Get(name.Classification).Namespace.NamespaceName);
			}
			return _finish;
		}

		public void Write(string text) => _writer.WriteString(text);
	}
}