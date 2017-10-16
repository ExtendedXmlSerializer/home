using System.IO;
using System.Xml;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.Samples.Introduction {
	public sealed class Create : ICommand<object>
	{
		public static Create Default { get; } = new Create();
		Create() {}

		public void Execute(object parameter)
		{
			// Create
			var serializer = new ConfigurationContainer()
														// Configure...
														.Create();
			// EndCreate

			var subject = new Subject{ Count = 6776, Message = "Hello World!" };
			var contents = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Introduction.xml", contents);
		}
	}
}