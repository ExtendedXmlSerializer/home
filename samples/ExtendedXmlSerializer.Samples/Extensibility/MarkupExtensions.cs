using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Markup;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	sealed class MarkupExtensions : ICommand<object>
	{
		public static MarkupExtensions Default { get; } = new MarkupExtensions();
		MarkupExtensions() {}

		public void Execute(object parameter)
		{
			// Example
			var contents =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
				  <Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Samples.Extensibility;assembly=ExtendedXmlSerializer.Samples""
					Message=""{Extension 'PRETTY COOL HUH!!!'}"" />";
			var serializer = new ConfigurationContainer().EnableMarkupExtensions()
			                                             .Create();
			var subject = serializer.Deserialize<Subject>(contents);
			Console.WriteLine(subject.Message); // "Hello World from Markup Extension! Your message is: PRETTY COOL HUH!!!"
			// EndExample
		}
	}

	// Extension
	sealed class Extension : IMarkupExtension
	{
		const string Message = "Hello World from Markup Extension! Your message is: ", None = "N/A";

		readonly string _message;

		public Extension() : this(None) {}

		public Extension(string message)
		{
			_message = message;
		}

		public object ProvideValue(IServiceProvider serviceProvider) => string.Concat(Message, _message);
	}
	// EndExtension
}
