using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ExtendedXmlSerializer.Samples.Simple
{
	sealed class Overrides : ICommand<object>
	{
		public static Overrides Default { get; } = new Overrides();

		Overrides() {}

		public void Execute(object parameter)
		{
			Program.PrintHeader("Serialization with Settings Override");
// Serialization

			IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
			TestClass              instance   = new TestClass();
			MemoryStream           stream     = new MemoryStream();

			string contents = serializer.Serialize(new XmlWriterSettings { /* ... */ }, stream, instance);

// EndSerialization

			Console.WriteLine(contents);

			Program.PrintHeader("Deserialization with Settings Override");
// Deserialization

			MemoryStream contentStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
			TestClass deserialized = serializer.Deserialize<TestClass>(new XmlReaderSettings{ /* ... */ }, contentStream);

// EndDeserialization
			Console.WriteLine($"Object id = {deserialized.Id}");
		}
	}
}