﻿// MIT License
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
using System.Xml.Serialization;
using ExtendedXmlSerialization.Test.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerialization.Test.Configuration
{
	public class AttributesExtensionTests
	{
		[Fact]
		public void VerifyDeclared()
		{
			var instance = new Subject {PropertyName = "Testing"};
			var actual = new SerializationSupport().Assert(instance,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><AttributesExtensionTests-Subject PropertyName=""Testing"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.Configuration;assembly=ExtendedXmlSerializerTest"" />");
			Assert.Equal(instance.PropertyName, actual.PropertyName);
		}

		[Fact]
		public void VerifyDifferentName()
		{
			var expected = new SubjectWithName {PropertyName = "Testing"};
			var actual = new SerializationSupport().Assert(expected,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><AttributesExtensionTests-SubjectWithName AnotherDifferentName=""Testing"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.Configuration;assembly=ExtendedXmlSerializerTest"" />");
			Assert.Equal(expected.PropertyName, actual.PropertyName);
		}

		[Fact]
		public void VerifyThrow() =>
			Assert.Throws<InvalidOperationException>(
				() => new SerializationSupport().Serialize(new SubjectWithNoKnownConverter {PropertyName = new Subject()})
			);

		class Subject
		{
			[XmlAttribute]
			public string PropertyName { get; set; }
		}

		class SubjectWithName
		{
			[XmlAttribute("AnotherDifferentName")]
			public string PropertyName { get; set; }
		}

		class SubjectWithNoKnownConverter
		{
			[XmlAttribute("AnotherDifferentName")]
			public Subject PropertyName { [UsedImplicitly] get; set; }
		}
	}
}