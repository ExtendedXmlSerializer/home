using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Samples.MigrationMap
{
    public class TestClassMigrationMap : AbstractMigrationMap<TestClass>
    {
        private static readonly Dictionary<int, Action<XElement>> MigrationMap = new Dictionary<int, Action<XElement>>
            {
                {0, MigrationV0 },
                {1, MigrationV1 }
            };

        public override int Version
        {
            get { return 2; }
        }

        public override Dictionary<int, Action<XElement>> Migrations
        {
            get { return MigrationMap; }
        }

        public static void MigrationV0(XElement node)
        {
            var typeElement = node.Elements().FirstOrDefault(x => x.Name == "Type");
            // Add new node
            node.Add(new XElement("Name", typeElement.Value));
            // Remove old node
            typeElement.Remove();
        }

        public static void MigrationV1(XElement node)
        {
            // Add new node
            node.Add(new XElement("Value", "Calculated"));
        }
    }
}
