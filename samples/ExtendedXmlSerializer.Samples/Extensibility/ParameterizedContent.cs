using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using System;
using System.IO;
using System.Xml;
// ReSharper disable All

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class ParameterizedContent : ICommand<object>
	{
		public static ParameterizedContent Default { get; } = new ParameterizedContent();
		ParameterizedContent() {}

		public void Execute(object parameter)
		{
// Example
IExtendedXmlSerializer serializer = new ConfigurationContainer().EnableParameterizedContent()
                                                                .Create();
ParameterizedSubject subject = new ParameterizedSubject("Hello World!", 123, DateTime.Now);
string contents = serializer.Serialize(subject);
// ...
// EndExample

			string data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.ParameterizedContent.xml", data);
		}
	}

// Subject
public sealed class ParameterizedSubject
{
	public ParameterizedSubject(string message, int number, DateTime time)
	{
		Message = message;
		Number = number;
		Time = time;
	}

	public string Message { get; }
	public int Number { get; }
	public DateTime Time { get; }
}
// EndSubject
}
