using System.Xml;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class Instances : ReferenceCache<XmlReader, object>, IInstances
	{
		public static Instances Default { get; } = new Instances();

		Instances() : base(_ => null) {}
	}
}