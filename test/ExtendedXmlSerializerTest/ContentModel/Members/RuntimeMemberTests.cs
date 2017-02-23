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

using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Test.Support;
using Xunit;

namespace ExtendedXmlSerialization.Test.ContentModel.Members
{
	public class RuntimeMemberTests
	{
		[Fact]
		public void Verify()
		{
			const string target = "I am Attribute, Hear me roar! #rawr!";

			var memberInfo = typeof(SimpleSubject).GetRuntimeProperty(nameof(SimpleSubject.Message));
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 {memberInfo, StringConverter.Default}
			                 };

			var specifications = new Dictionary<MemberInfo, IRuntimeMemberSpecification>
			                     {
				                     {
					                     memberInfo,
					                     new RuntimeMemberSpecification(new DelegatedSpecification<string>(x => x == target).Adapt())
				                     }
			                     };

			var configuration = new ExtendedXmlConfiguration(converters,
			                                                 new Dictionary<MemberInfo, IMemberEmitSpecification>(),
			                                                 specifications);

			var support = new SerializationSupport(configuration.Create());
			var expected = new SimpleSubject {Message = "Hello World!", Number = 6776};
			var content = support.Assert(expected,
			                             @"<?xml version=""1.0"" encoding=""utf-8""?><RuntimeMemberTests-SimpleSubject xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ContentModel.Members;assembly=ExtendedXmlSerializerTest""><Message>Hello World!</Message><Number>6776</Number></RuntimeMemberTests-SimpleSubject>");
			Assert.Equal(expected.Message, content.Message);
			Assert.Equal(expected.Number, content.Number);

			expected.Message = target;
			var attributes = support.Assert(expected,
			                             @"<?xml version=""1.0"" encoding=""utf-8""?><RuntimeMemberTests-SimpleSubject Message=""I am Attribute, Hear me roar! #rawr!"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ContentModel.Members;assembly=ExtendedXmlSerializerTest""><Number>6776</Number></RuntimeMemberTests-SimpleSubject>");
			Assert.Equal(expected.Message, attributes.Message);
			Assert.Equal(expected.Number, attributes.Number);

		}

		class SimpleSubject
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}
	}
}