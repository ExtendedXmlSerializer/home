using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
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
			List<Subject> legacy = new List<Subject>
			              {
				              new Subject {Message = "First"},
				              new Subject {Message = "Second"},
				              new Subject {Message = "Third"}
			              };

#pragma warning disable 618
			File.WriteAllText(@"bin\Upgrade.Example.v1.xml", new ExtendedXmlSerialization.ExtendedXmlSerializer().Serialize(legacy));

// Example

ExtendedXmlSerialization.ExtendedXmlSerializer legacySerializer = new ExtendedXmlSerialization.ExtendedXmlSerializer();
string content = File.ReadAllText(@"bin\Upgrade.Example.v1.xml"); // Path to your legacy xml file.
List<Subject> subject = legacySerializer.Deserialize<List<Subject>>(content);

// Upgrade:
IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
string contents = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
File.WriteAllText(@"bin\Upgrade.Example.v2.xml", contents);
// ...
// EndExample
#pragma warning restore 618
		}
	}
}
