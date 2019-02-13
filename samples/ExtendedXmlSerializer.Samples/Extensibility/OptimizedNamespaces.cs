using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Collections.Generic;
using System.IO;
using System.Xml;
// ReSharper disable UnusedVariable

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class OptimizedNamespaces : ICommand<object>
	{
		public static OptimizedNamespaces Default { get; } = new OptimizedNamespaces();
		OptimizedNamespaces() {}

		public void Execute(object parameter)
		{
// Example

IExtendedXmlSerializer serializer = new ConfigurationContainer().UseOptimizedNamespaces()
											 .Create();
List<object> subject = new List<object>
			    {
				    new Subject {Message = "First"},
				    new Subject {Message = "Second"},
				    new Subject {Message = "Third"}
			    };
string contents = serializer.Serialize(subject);
// ...
// EndExample

			string @default = new ConfigurationContainer().Create()
													   .Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.OptimizedNamepsaces.Default.xml", @default);

			string data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.OptimizedNamepsaces.Optimized.xml", data);
		}
	}

	public sealed class Subject
	{
		public string Message { get; set; }
	}

}
