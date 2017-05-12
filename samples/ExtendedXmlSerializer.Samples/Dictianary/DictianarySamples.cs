using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ExtendedXmlSerialization.Samples.Encrypt;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Encryption;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerialization.Samples.Dictianary
{
    public static class DictianarySamples
    {
        public static void Run()
        {
            Program.PrintHeader("Serialization");

            var serializer = new ConfigurationContainer().Create();
// InitDictionary
            var obj = new TestClass
            {
                Dictionary = new Dictionary<int, string>
                {
                    {1, "First"},
                    {2, "Second"},
                    {3, "Other"},
                }
            };
// EndInitDictionary
            var xml = serializer.Serialize(new XmlWriterSettings {Indent = true}, obj);
            File.WriteAllText("bin\\DictianarySamples.xml", xml);
            Console.WriteLine(xml);

            Program.PrintHeader("Deserialization");
            serializer.Deserialize<TestClass>(xml);

            serializer = new ConfigurationContainer().UseOptimizedNamespaces().Create();
            xml = serializer.Serialize(new XmlWriterSettings() { Indent = true }, obj);
            File.WriteAllText("bin\\DictianarySamplesUseOptimizedNamespaces.xml", xml);
        }
    }
}
