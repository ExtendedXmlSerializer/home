using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Xml;

namespace ExtendedXmlSerializer.Samples.Introduction
{
	public sealed class Type : ICommand<object>
	{
		public static Type Default { get; } = new Type();
		Type() {}

		public void Execute(object parameter)
		{
// Type
IExtendedXmlSerializer serializer = new ConfigurationContainer().ConfigureType<Subject>()
											 .Name("ModifiedSubject")
											 .Create();
// EndType

			Subject subject = new Subject{ Count = 6776, Message = "Hello World!" };
			string contents = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Introduction.Type.xml", contents);
		}
	}
}