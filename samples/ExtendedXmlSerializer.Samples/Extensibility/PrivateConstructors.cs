using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Xml;
// ReSharper disable All

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class PrivateConstructors : ICommand<object>
	{
		public static PrivateConstructors Default { get; } = new PrivateConstructors();
		PrivateConstructors() {}

		public void Execute(object parameter)
		{
// Example
IExtendedXmlSerializer serializer = new ConfigurationContainer().EnableAllConstructors()
                                                                .Create();
SubjectByFactory subject = SubjectByFactory.Create("Hello World!");
string contents = serializer.Serialize(subject);
// ...
// EndExample

			string data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.PrivateConstructors.xml", data);
		}
	}

// Subject
public sealed class SubjectByFactory
{
	public static SubjectByFactory Create(string message) => new SubjectByFactory(message);

	SubjectByFactory() : this(null) {} // Used by serializer.

	SubjectByFactory(string message) => Message = message;

	public string Message { get; set; }
}
// EndSubject
}
