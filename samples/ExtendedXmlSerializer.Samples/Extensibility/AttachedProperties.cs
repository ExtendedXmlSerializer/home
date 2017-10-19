using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.AttachedProperties;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Xml;
// ReSharper disable All

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class AttachedProperties : ICommand<object>
	{
		public static AttachedProperties Default { get; } = new AttachedProperties();
		AttachedProperties() {}

		public void Execute(object parameter)
		{
// Example
var serializer = new ConfigurationContainer().EnableAttachedProperties(NameProperty.Default,
																		NumberProperty.Default)
											 .Create();
var subject = new Subject {Message = "Hello World!"};
subject.Set(NameProperty.Default, "Hello World from Attached Properties!");
subject.Set(NumberProperty.Default, 123);

var contents = serializer.Serialize(subject);
// ...
// EndExample

			var data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.AttachedProperties.xml", data);
		}
	}

	// Properties
	sealed class NameProperty : ReferenceProperty<Subject, string>
	{
		public const string DefaultMessage = "The Name Has Not Been Set";

		public static NameProperty Default { get; } = new NameProperty();
		NameProperty() : base(() => Default, x => DefaultMessage) {}
	}

	sealed class NumberProperty : StructureProperty<Subject, int>
	{
		public const int DefaultValue = 123;

		public static NumberProperty Default { get; } = new NumberProperty();
		NumberProperty() : base(() => Default, x => DefaultValue) {}
	}
	// EndProperties
}
