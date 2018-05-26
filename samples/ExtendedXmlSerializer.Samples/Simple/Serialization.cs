using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ExtendedXmlSerializer.Samples.Simple
{
	sealed class Serialization : ICommand<object>
	{
		public static Serialization Default { get; } = new Serialization();

		Serialization() {}

		public void Execute(object parameter)
		{
			Program.PrintHeader("Classic Stream-based Serialization");
// Serialization

			var serializer = new ConfigurationContainer().Create();
			var instance   = new TestClass();
			var stream     = new MemoryStream();
			using (var writer = XmlWriter.Create(stream))
			{
				serializer.Serialize(writer, instance);
				writer.Flush();
			}

			stream.Seek(0, SeekOrigin.Begin);
			var contents = new StreamReader(stream).ReadToEnd();

// EndSerialization

			Console.WriteLine(contents);

			Program.PrintHeader("Classic Stream-based Deserialization");
// Deserialization

			TestClass deserialized;
			var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
			using (var reader = XmlReader.Create(contentStream))
			{
				deserialized = (TestClass)serializer.Deserialize(reader);
			}

// EndDeserialization
			Console.WriteLine($"Object id = {deserialized.Id}");
		}
	}
}