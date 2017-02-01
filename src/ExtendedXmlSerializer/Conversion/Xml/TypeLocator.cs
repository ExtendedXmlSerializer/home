using System;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	class TypeLocator : ITypeLocator
	{
		readonly ITypes _types;

		public TypeLocator(ITypes types)
		{
			_types = types;
		}

		public TypeInfo Get(System.Xml.XmlReader parameter)
		{
			switch (parameter.MoveToContent())
			{
				case XmlNodeType.Element:
					var name = XName.Get(parameter.LocalName, parameter.NamespaceURI);
					var result = _types.Get(name);
					return result;
			}

			throw new InvalidOperationException($"Could not locate the type from the current Xml reader '{parameter}.'");
		}
	}
}