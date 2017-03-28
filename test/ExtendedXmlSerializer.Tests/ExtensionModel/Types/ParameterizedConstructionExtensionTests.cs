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

using System;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Types
{
	public class ParameterizedConstructionExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(ParameterizedConstructionExtension.Default));
			var expected = new Subject("Hello World!");
			var actual = serializer.Assert(expected, @"<?xml version=""1.0"" encoding=""utf-8""?><ParameterizedConstructionExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Types;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message></ParameterizedConstructionExtensionTests-Subject>");
			Assert.Equal(expected.Message, actual.Message);
		}

		[Fact]
		public void MultipleParameters()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(ParameterizedConstructionExtension.Default));
			var expected = new SubjectWithMultipleParameters("Hello World!", 6776);
			var actual = serializer.Assert(expected, @"<?xml version=""1.0"" encoding=""utf-8""?><ParameterizedConstructionExtensionTests-SubjectWithMultipleParameters xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Types;assembly=ExtendedXmlSerializer.Tests""><Message>Hello World!</Message><Number>6776</Number></ParameterizedConstructionExtensionTests-SubjectWithMultipleParameters>");
			Assert.Equal(expected.Message, actual.Message);
			Assert.Equal(expected.Number, actual.Number);
		}

		[Fact]
		public void MultipleConstructors()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(ParameterizedConstructionExtension.Default));
			var expected = new SubjectWithMultipleConstructors(6776, new DateTime(1976, 6, 7));
			var actual = serializer.Assert(expected, @"<?xml version=""1.0"" encoding=""utf-8""?><ParameterizedConstructionExtensionTests-SubjectWithMultipleConstructors xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Types;assembly=ExtendedXmlSerializer.Tests""><Number>6776</Number><DateTime>1976-06-07T00:00:00</DateTime></ParameterizedConstructionExtensionTests-SubjectWithMultipleConstructors>");
			Assert.Equal(expected.Number, actual.Number);
			Assert.Equal(expected.DateTime, actual.DateTime);
			Assert.Equal(SubjectWithMultipleConstructors.Message, actual.Get());
		}

		[Fact]
		public void Invalid()
		{
			var serializer = new SerializationSupport(new ExtendedConfiguration().Extend(ParameterizedConstructionExtension.Default));
			var expected = new SubjectWithInvalidConstructor("Hello World!", 6776);
			Assert.Throws<InvalidOperationException>(() => serializer.Serialize(expected));
		}

		class Subject
		{
			public Subject(string message)
			{
				Message = message;
			}

			public string Message { get; }
		}

		class SubjectWithMultipleParameters
		{
			public SubjectWithMultipleParameters(string message, int number)
			{
				Message = message;
				Number = number;
			}

			public string Message { get; }
			public int Number { get; }
		}

		class SubjectWithInvalidConstructor
		{
			[UsedImplicitly] readonly string _message;

			public SubjectWithInvalidConstructor(string message, int number)
			{
				Number = number;
				_message = message;
			}

			public int Number { [UsedImplicitly] get; }
		}

		class SubjectWithMultipleConstructors : ISource<string>
		{
			public const string Message = "Hello World from Second Candidate Selector!";
			readonly string _message;

			public SubjectWithMultipleConstructors(int number, DateTime dateTime) : this(Message, number, dateTime) {}

			public SubjectWithMultipleConstructors(string message, int number, DateTime dateTime)
			{
				Number = number;
				DateTime = dateTime;
				_message = message;
			}

			public int Number { get; }
			public DateTime DateTime { get; }
			public string Get() => _message;
		}
	}
}