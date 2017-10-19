using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Xml;
// ReSharper disable UnusedVariable

namespace ExtendedXmlSerializer.Samples.Introduction
{
	public sealed class Settings : ICommand<object>
	{
		public void Execute(object parameter)
		{
// Write
var subject = new Subject{ Count = 6776, Message = "Hello World!" };
var serializer = new ConfigurationContainer().Create();
var contents = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
// ...
// EndWrite

// Read
var instance = serializer.Deserialize<Subject>(new XmlReaderSettings{IgnoreWhitespace = false}, contents);
// ...
// EndRead
		}
	}
}
