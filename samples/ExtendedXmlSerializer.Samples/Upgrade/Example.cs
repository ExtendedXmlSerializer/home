using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Subject = ExtendedXmlSerializer.Samples.Introduction.Subject;

namespace ExtendedXmlSerializer.Samples.Upgrade
{
	sealed class Example : ICommand<object>
	{
		public static Example Default { get; } = new Example();
		Example() {}

		public void Execute(object parameter)
		{
			var legacy = new List<Subject>
			              {
				              new Subject {Message = "First"},
				              new Subject {Message = "Second"},
				              new Subject {Message = "Third"}
			              };

#pragma warning disable 618
			File.WriteAllText(@"bin\Upgrade.Example.v1.xml", new ExtendedXmlSerialization.ExtendedXmlSerializer().Serialize(legacy));

// Example

var legacySerializer = new ExtendedXmlSerialization.ExtendedXmlSerializer();
var content = File.ReadAllText(@"bin\Upgrade.Example.v1.xml"); // Path to your legacy xml file.
var subject = legacySerializer.Deserialize<List<Subject>>(content);

// Upgrade:
var serializer = new ConfigurationContainer().Create();
var contents = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
File.WriteAllText(@"bin\Upgrade.Example.v2.xml", contents);
// ...
// EndExample
#pragma warning restore 618
		}
	}
}
