using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using System.Reflection;
using Xunit;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue246Tests
	{
		[Fact]
		void Verify()
		{
			new ConfigurationContainer().EnableImplicitTyping(typeof(TestData))
			                            .Type<TestData>()
			                            .Member(x => x.Name)
			                            .EmitWhen(x => !string.IsNullOrEmpty(x))
			                            .Create()
			                            .ForTesting()
			                            .Assert(new TestData {Name = ""},
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue246Tests-TestData />");

			new ConfigurationContainer().EnableImplicitTyping(typeof(TestData))
			                            .Type<string>()
			                            .EmitWhen(x => !string.IsNullOrEmpty(x))
			                            .Create()
			                            .ForTesting()
			                            .Assert(new TestData {Name = ""},
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue246Tests-TestData />");
		}

		[Fact]
		void VerifyDefaultValue()
		{
			new ConfigurationContainer().EnableImplicitTyping(typeof(Subject))
			                            .Emit(EmitBehaviors.Assigned)
			                            .Create()
			                            .ForTesting()
			                            .Assert(new Subject(),
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue246Tests-Subject />");
		}

		public class Subject
		{
			public Subject() : this(string.Empty) {}

			public Subject(string name) => Name = name;

			public string Name { [UsedImplicitly] get; set; }
		}

		[Fact]
		void VerifyClosedTagsOnEmpty()
		{
			new ConfigurationContainer().Extend(Extension.Default)
			                            .EnableImplicitTyping(typeof(TestData))
			                            .Create()
			                            .ForTesting()
			                            .Assert(new TestData {Name = ""},
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue246Tests-TestData><Name /></Issue246Tests-TestData>");
		}

		sealed class Extension : ISerializerExtension
		{
			public static Extension Default { get; } = new Extension();

			Extension() {}

			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.Decorate<IFormatWriters<XmlWriter>, Writers>();

			void ICommand<IServices>.Execute(IServices parameter) {}

			sealed class Writers : IFormatWriters<XmlWriter>
			{
				readonly IFormatWriters<XmlWriter> _writers;

				public Writers(IFormatWriters<XmlWriter> writers) => _writers = writers;

				public IFormatWriter Get(XmlWriter parameter) => new Writer(_writers.Get(parameter));
			}

			sealed class Writer : IFormatWriter
			{
				readonly IFormatWriter _writer;

				public Writer(IFormatWriter writer) => _writer = writer;

				public object Get() => _writer.Get();

				public IIdentity Get(string name, string identifier) => _writer.Get(name, identifier);

				public void Dispose()
				{
					_writer.Dispose();
				}

				public string Get(MemberInfo parameter) => _writer.Get(parameter);

				public void Start(IIdentity identity)
				{
					_writer.Start(identity);
				}

				public void EndCurrent()
				{
					_writer.EndCurrent();
				}

				public void Content(IIdentity property, string content)
				{
					_writer.Content(property, content);
				}

				public void Content(string content)
				{
					if (content != string.Empty)
					{
						_writer.Content(content);
					}
				}

				public void Verbatim(string content)
				{
					_writer.Verbatim(content);
				}
			}
		}

		public class TestData
		{
			public string Name { get; set; }
		}
	}
}