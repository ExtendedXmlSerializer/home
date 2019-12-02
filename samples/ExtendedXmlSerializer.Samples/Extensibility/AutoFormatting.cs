using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using System;
using System.IO;
using System.Xml;
// ReSharper disable UnusedVariable

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class AutoFormatting : ICommand<object>
	{
		public static AutoFormatting Default { get; } = new AutoFormatting();
		AutoFormatting() {}

		public void Execute(object parameter)
		{
// Example

IExtendedXmlSerializer serializer = new ConfigurationContainer().UseAutoFormatting()
											 .Create();
SubjectWithThreeProperties subject = new SubjectWithThreeProperties{ Message = "Hello World!", Number = 123, Time = DateTime.Now };
string contents = serializer.Serialize(subject);
// ...
// EndExample

			string @default = new ConfigurationContainer().Create()
													   .Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.AutoFormatting.Default.xml", @default);

			string data = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Extensibility.AutoFormatting.Enabled.xml", data);
		}
	}

	public class SubjectWithThreeProperties
	{
		public int Number { get; set; }

		public string Message { get; set; }

		public DateTime Time { get; set; }
	}
}
