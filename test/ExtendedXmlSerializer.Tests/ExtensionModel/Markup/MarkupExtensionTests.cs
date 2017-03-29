// MIT License
//
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Markup;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Markup
{
	public class MarkupExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(AutoMemberFormatExtension.Default).EnableMarkupExtensions());
			var subject = serializer.Deserialize<Subject>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" PropertyName=""{MarkupExtensionTests-Extension}"" />");
			subject.PropertyName.Should().Be(string.Concat(Extension.Message, Extension.None, " 6776"));
		}

		[Fact]
		public void VerifyWithProperty()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(AutoMemberFormatExtension.Default).EnableMarkupExtensions());
			var subject = serializer.Deserialize<Subject>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" PropertyName=""{MarkupExtensionTests-Extension Number=3 * 3}"" />");
			subject.PropertyName.Should().Be(string.Concat(Extension.Message, Extension.None, " 9"));
		}

		[Fact]
		public void VerifyWithParameter()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(AutoMemberFormatExtension.Default).EnableMarkupExtensions());
			var subject = serializer.Deserialize<Subject>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" PropertyName=""{MarkupExtensionTests-Extension 'Provided Message!'}"" />");
			subject.PropertyName.Should().Be(string.Concat(Extension.Message, "Provided Message!", " 6776"));
		}

		[Fact]
		public void VerifyWithParameterAndProperty()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(AutoMemberFormatExtension.Default).EnableMarkupExtensions());
			var subject = serializer.Deserialize<Subject>(@"<?xml version=""1.0"" encoding=""utf-8""?><MarkupExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Markup;assembly=ExtendedXmlSerializer.Tests"" PropertyName=""{MarkupExtensionTests-Extension 'Provided Message!', Number=3 * 4}"" />");
			subject.PropertyName.Should().Be(string.Concat(Extension.Message, "Provided Message!", " 12"));
		}

		sealed class Subject
		{
			public string PropertyName { get; [UsedImplicitly] set; }
		}

		sealed class Extension : IMarkupExtension
		{
			public const string Message = "Hello World from Markup Extension! Your message is: ", None = "N/A";

			readonly string _message;

			[UsedImplicitly]
			public Extension() : this(None) {}

			public Extension(string message)
			{
				_message = message;
			}

			public object ProvideValue(System.IServiceProvider serviceProvider) => string.Concat(Message, _message, $" {Number.ToString()}");

			public int Number { get; set; } = 6776;
		}
	}
}