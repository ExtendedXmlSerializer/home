using ExtendedXmlSerializer.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ExtendedXmlSerializer.Samples.Dictianary
{
	public static class DictianarySamples
	{
		public static void Run()
		{
			Program.PrintHeader("Serialization");

			IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
// InitDictionary

TestClass obj = new TestClass
{
	Dictionary = new Dictionary<int, string>
	{
		{1, "First"},
		{2, "Second"},
		{3, "Other"},
	}
};
// EndInitDictionary
			string xml = serializer.Serialize(new XmlWriterSettings {Indent = true}, obj);
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
