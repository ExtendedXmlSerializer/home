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

			var serializer = new ConfigurationContainer().Create();
			var instance   = new TestClass();
			var stream     = new MemoryStream();

			var contents = serializer.Serialize(new XmlWriterSettings { /* ... */ }, stream, instance);

// EndSerialization

			Console.WriteLine(contents);

			Program.PrintHeader("Deserialization with Settings Override");
// Deserialization

			var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
			var deserialized = serializer.Deserialize<TestClass>(new XmlReaderSettings{ /* ... */ }, contentStream);

// EndDeserialization
			Console.WriteLine($"Object id = {deserialized.Id}");
		}
	}
}