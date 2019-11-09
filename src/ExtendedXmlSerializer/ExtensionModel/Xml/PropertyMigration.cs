using System.Linq;
using System.Xml.Linq;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	class PropertyMigration : ICommand<XElement>
	{
		readonly string _from;
		readonly string _to;

		public PropertyMigration(string from, string to)
		{
			_from = from;
			_to   = to;
		}

		public void Execute(XElement parameter)
		{
			var element = parameter.Elements(XName.Get(_from, parameter.Name.NamespaceName))
			                       .Single();
			parameter.Add(new XElement(XName.Get(_to, parameter.Name.NamespaceName), element.Value));
			element.Remove();
		}
	}
}