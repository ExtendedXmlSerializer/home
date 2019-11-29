using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Reflection;
using Xunit;
// ReSharper disable UnusedMember.Local

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue282Tests_Extension
	{
		[Fact]
		void Verify()
		{
			var container = new ConfigurationContainer().EnableImplicitTyping(typeof(Subject))
			                                            .Extend(Extension.Default)
			                                            .Create()
			                                            .ForTesting();

			var instance = new Subject {Number = 10};

			container.Assert(instance,
			                 @"<?xml version=""1.0"" encoding=""utf-8""?><Issue282Tests_Extension-Subject><Number>52</Number></Issue282Tests_Extension-Subject>")
			         .Number.Should()
			         .Be(94);
		}

		sealed class Subject
		{
			public int Number { get; set; }
		}

		sealed class Extension : ISerializerExtension
		{
			public static Extension Default { get; } = new Extension();

			Extension() {}

			public IServiceRepository Get(IServiceRepository parameter) => parameter.DecorateContentsWith<Contents>()
			                                                                        .Then();

			void ICommand<IServices>.Execute(IServices parameter) {}

			sealed class Contents : IContents
			{
				readonly IContents        _previous;
				readonly ISerializer<int> _number;

				public Contents(IContents previous)
					: this(previous, new AnswerToEverythingSerializer(previous.Get(typeof(int)).For<int>())) {}

				public Contents(IContents previous, ISerializer<int> number)
				{
					_previous = previous;
					_number   = number;
				}

				public ISerializer Get(TypeInfo parameter)
					=> parameter == typeof(int) ? _number.Adapt() : _previous.Get(parameter);
			}

			sealed class AnswerToEverythingSerializer : ISerializer<int>
			{
				readonly ISerializer<int> _previous;

				public AnswerToEverythingSerializer(ISerializer<int> previous) => _previous = previous;

				public int Get(IFormatReader parameter) => _previous.Get(parameter) + 42;

				public void Write(IFormatWriter writer, int instance)
				{
					_previous.Write(writer, instance + 42);
				}
			}
		}
	}
}