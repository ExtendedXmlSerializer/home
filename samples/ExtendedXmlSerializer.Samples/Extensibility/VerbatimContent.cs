using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Xml;
// ReSharper disable UnusedVariable

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class VerbatimContent : ICommand<object>
	{
		public static VerbatimContent Default { get; } = new VerbatimContent();
		VerbatimContent() {}

		public void Execute(object parameter)
		{
// Example
IExtendedXmlSerializer serializer = new ConfigurationContainer().Type<Subject>()
                                                                .Member(x => x.Message)
                                                                .Verbatim()
                                                                .Create();
Subject subject = new Subject {Message = @"<{""Ilegal characters and such""}>"};
string contents = serializer.Serialize(subject);
// ...
// EndExample

			string @default = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.VerbatimContent.xml", @default);
		}
	}

// Subject
public sealed class VerbatimSubject
{
	[Verbatim]
	public string Message { get; set; }
}
// EndSubject
}