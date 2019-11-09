using System.Xml;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class Prefixes : IPrefixes
	{
		public static Prefixes Default { get; } = new Prefixes();

		Prefixes() : this(IsTypeSpecification<XmlTextWriter>.Default) {}

		readonly ISpecification<System.Xml.XmlWriter> _specification;

		public Prefixes(ISpecification<System.Xml.XmlWriter> specification)
			=> _specification = specification;

		public IPrefix Get(System.Xml.XmlWriter parameter)
		{
			var prefix = new DefaultPrefix(parameter);
			var result = _specification.IsSatisfiedBy(parameter) ? new Prefix(prefix) : (IPrefix)prefix;
			return result;
		}
	}
}