using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Xml;
// ReSharper disable All

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class ImplicitTypes : ICommand<object>
	{
		public static ImplicitTypes Default { get; } = new ImplicitTypes();
		ImplicitTypes() {}

		public void Execute(object parameter)
		{
// Example
IExtendedXmlSerializer serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(Subject))
											 .Create();
Subject subject = new Subject{ Message = "Hello World!  No namespaces, yay!" };
string contents = serializer.Serialize(subject);
// ...
// EndExample

			string data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.ImplicitTypes.xml", data);
		}
	}
}
