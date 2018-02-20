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
var serializer = new ConfigurationContainer().Type<CustomStruct>().Register(CustomStructConverter.Default).Create();
var subject = new CustomStruct(123);
var contents = serializer.Serialize(subject);
// ...
// EndConverter

			var data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.Converters.xml", data);
		}
	}
}
