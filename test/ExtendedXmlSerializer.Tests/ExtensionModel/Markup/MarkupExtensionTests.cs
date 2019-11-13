using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Markup;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Markup
{
	public class MarkupExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var container = new ConfigurationContainer();
			container.EnableMarkupExtensions();
			var serializer = new SerializationSupport(container);
			var subject =
				serializer
					.Deserialize<Subject
					>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" PropertyName=""{MarkupExtensionTests-Extension}"" />");
			subject.PropertyName.Should()
			       .Be(string.Concat(Extension.Message, Extension.None, " 6776"));
		}

		[Fact]
		public void VerifyWithProperty()
		{
			var container = new ConfigurationContainer();
			container.EnableMarkupExtensions();
			var serializer = new SerializationSupport(container);
			var subject =
				serializer
					.Deserialize<Subject
					>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" PropertyName=""{MarkupExtensionTests-Extension Number=3 * 3}"" />");
			subject.PropertyName.Should()
			       .Be(string.Concat(Extension.Message, Extension.None, " 9"));
		}

		[Fact]
		public void VerifyWithParameter()
		{
			var container = new ConfigurationContainer();
			container.EnableMarkupExtensions();
			var serializer = new SerializationSupport(container);
			var subject =
				serializer
					.Deserialize<Subject
					>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" PropertyName=""{MarkupExtensionTests-Extension 'Provided Message!'}"" />");
			subject.PropertyName.Should()
			       .Be(string.Concat(Extension.Message, "Provided Message!", " 6776"));
		}

		[Fact]
		public void VerifyWithParameterAndProperty()
		{
			var container = new ConfigurationContainer();
			container.EnableMarkupExtensions();
			var serializer = new SerializationSupport(container);
			var subject =
				serializer
					.Deserialize<Subject
					>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" PropertyName=""{MarkupExtensionTests-Extension 'Provided Message!', Number=3 * 4}"" />");
			subject.PropertyName.Should()
			       .Be(string.Concat(Extension.Message, "Provided Message!", " 12"));
		}

		[Fact]
		public void VerifyType()
		{
			var container = new ConfigurationContainer();
			container.EnableMarkupExtensions();
			var serializer = new SerializationSupport(container);
			var subject =
				serializer
					.Deserialize<TypedSubject
					>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-TypedSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:sys=""https://extendedxmlserializer.github.io/system""  SuppliedType=""{exs:Type sys:int}"" AnotherProperty=""Hello World!"" />");
			subject.SuppliedType.Should()
			       .Be(typeof(int));
			subject.AnotherProperty.Should()
			       .Be("Hello World!");
		}

		[Fact]
		public void VerifyMarkupExtension()
		{
			var container = new ConfigurationContainer();
			container.EnableMarkupExtensions();
			var serializer = new SerializationSupport(container);
			var subject =
				serializer
					.Deserialize<TypedSubject
					>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-TypedSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:sys=""https://extendedxmlserializer.github.io/system""  AnotherProperty=""{MarkupExtensionTests-TypeName {exs:Type sys:dateTime}}"" />");
			subject.AnotherProperty.Should()
			       .Be(typeof(DateTime).AssemblyQualifiedName);
		}

		[Fact]
		public void VerifyStatic()
		{
			var container = new ConfigurationContainer();
			container.EnableMarkupExtensions();
			var serializer = new SerializationSupport(container);
			var subject =
				serializer
					.Deserialize<DatedSubject
					>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-DatedSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:sys=""https://extendedxmlserializer.github.io/system""  Date=""{exs:Static sys:dateTime.Now}"" DateNullable=""{exs:Static sys:dateTime.Now}"" />");
			subject.Date.Should()
			       .NotBe(default(DateTime))
			       .And.BeAfter(DateTime.Today);
			subject.DateNullable.Should()
			       .NotBeNull()
			       .And.BeAfter(DateTime.Today);
		}

		[Fact]
		public void VerifyNull()
		{
			new ConfigurationContainer().EnableMarkupExtensions()
			                            .Emit(EmitBehaviors.WhenModified)
			                            .ForTesting()
			                            .Deserialize<DefaultAssignedSubject
			                            >(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-DefaultAssignedSubject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:sys=""https://extendedxmlserializer.github.io/system""  AssignedMessage=""{exs:Null}"" />")
			                            .AssignedMessage
			                            .Should()
			                            .BeNull();
		}

		sealed class DefaultAssignedSubject
		{
			public string AssignedMessage { get; set; } = "Hello World!";
		}

		sealed class Subject
		{
			public string PropertyName { get; [UsedImplicitly] set; }
		}

		sealed class DatedSubject
		{
			public DateTime Date { get; [UsedImplicitly] set; }

			public DateTime? DateNullable { get; [UsedImplicitly] set; }
		}

		sealed class TypedSubject
		{
			public Type SuppliedType { get; [UsedImplicitly] set; }

			public string AnotherProperty { get; [UsedImplicitly] set; }
		}

		sealed class TypeNameExtension : IMarkupExtension
		{
			readonly Type _type;

			[UsedImplicitly]
			public TypeNameExtension(Type type) => _type = type;

			public object ProvideValue(IServiceProvider serviceProvider) => _type.AssemblyQualifiedName;
		}

		sealed class Extension : IMarkupExtension
		{
			public const string Message = "Hello World from Markup Extension! Your message is: ", None = "N/A";

			readonly string _message;

			[UsedImplicitly]
			public Extension() : this(None) {}

			public Extension(string message) => _message = message;

			public object ProvideValue(IServiceProvider serviceProvider)
				=> string.Concat(Message, _message, $" {Number.ToString()}");

			public int Number { get; set; } = 6776;
		}
	}
}