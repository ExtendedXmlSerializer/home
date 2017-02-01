using System;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Names;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class XmlWriter : IWriter
	{
		readonly System.Xml.XmlWriter _writer;
		readonly INamespaces _namespaces;
		readonly IDisposable _finish;

		public XmlWriter(INamespaces namespaces, System.Xml.XmlWriter writer)
			: this(namespaces, writer, new DelegatedDisposable(writer.WriteEndElement)) {}

		public XmlWriter(INamespaces namespaces, System.Xml.XmlWriter writer, IDisposable finish)
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