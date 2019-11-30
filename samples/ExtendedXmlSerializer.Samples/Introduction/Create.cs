using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using System.IO;
using System.Xml;

namespace ExtendedXmlSerializer.Samples.Introduction
{
	public sealed class Create : ICommand<object>
	{
		public static Create Default { get; } = new Create();
		Create() {}

		public void Execute(object parameter)
		{
// Create
IExtendedXmlSerializer serializer = new ConfigurationContainer()
											// Configure...
											.Create();
// EndCreate

			Subject subject = new Subject{ Count = 6776, Message = "Hello World!" };
			string contents = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Introduction.xml", contents);
		}
	}
}