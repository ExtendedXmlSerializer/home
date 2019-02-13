using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Xml;
// ReSharper disable UnusedVariable

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	public sealed class Converters : ICommand<object>
	{
		public static Converters Default { get; } = new Converters();
		Converters() {}

		public void Execute(object parameter)
		{
// Converter
IExtendedXmlSerializer serializer = new ConfigurationContainer().Register(CustomStructConverter.Default).Create();
CustomStruct subject = new CustomStruct(123);
string contents = serializer.Serialize(subject);
// ...
// EndConverter

			string data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.Converters.xml", data);
		}
	}
}
