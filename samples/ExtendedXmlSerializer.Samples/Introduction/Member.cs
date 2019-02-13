using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Xml;

namespace ExtendedXmlSerializer.Samples.Introduction
{
	public sealed class Member : ICommand<object>
	{
		public static Member Default { get; } = new Member();
		Member() {}

		public void Execute(object parameter)
		{
// Member
IExtendedXmlSerializer serializer = new ConfigurationContainer().ConfigureType<Subject>()
                                                                .Member(x => x.Message)
                                                                .Name("Text")
                                                                .Create();
// EndMember

			Subject subject = new Subject{ Count = 6776, Message = "Hello World!" };
			string contents = serializer.Serialize(new XmlWriterSettings {Indent = true}, subject);
			File.WriteAllText(@"bin\Introduction.Member.xml", contents);
		}
	}
}