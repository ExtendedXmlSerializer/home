using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.IO;
using System.Xml;
// ReSharper disable All

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class Tuples : ICommand<object>
	{
		public static Tuples Default { get; } = new Tuples();
		Tuples() {}

		public void Execute(object parameter)
		{
// Example
var serializer = new ConfigurationContainer().EnableParameterizedContent()
                                             .Type<Tuple<string>>()
                                             .Member(x => x.Item1)
                                             .Name("Message")
                                             .Create();
var subject = Tuple.Create("Hello World!");
var contents = serializer.Serialize(subject);
// ...
// EndExample

			var data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.Tuples.xml", data);
		}
	}
}
