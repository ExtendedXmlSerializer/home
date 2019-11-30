using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using System.Xml;
// ReSharper disable UnusedVariable

namespace ExtendedXmlSerializer.Samples.Introduction
{
	public sealed class Settings : ICommand<object>
	{
		public void Execute(object parameter)
		{
// Write
Subject subject = new Subject{ Count = 6776, Message = "Hello World!" };
IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
string contents = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
// ...
// EndWrite

// Read
Subject instance = serializer.Deserialize<Subject>(new XmlReaderSettings{IgnoreWhitespace = false}, contents);
// ...
// EndRead
		}
	}
}
