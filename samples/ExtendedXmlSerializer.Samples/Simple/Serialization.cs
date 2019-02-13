using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
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

			IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
			TestClass instance   = new TestClass();
			MemoryStream stream     = new MemoryStream();
			using (XmlWriter writer = XmlWriter.Create(stream))
			{
				serializer.Serialize(writer, instance);
				writer.Flush();
			}

			stream.Seek(0, SeekOrigin.Begin);
			string contents = new StreamReader(stream).ReadToEnd();

// EndSerialization

			Console.WriteLine(contents);

			Program.PrintHeader("Classic Stream-based Deserialization");
// Deserialization

			TestClass deserialized;
			MemoryStream contentStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
			using (XmlReader reader = XmlReader.Create(contentStream))
			{
				deserialized = (TestClass)serializer.Deserialize(reader);
			}

// EndDeserialization
			Console.WriteLine($"Object id = {deserialized.Id}");
		}
	}
}