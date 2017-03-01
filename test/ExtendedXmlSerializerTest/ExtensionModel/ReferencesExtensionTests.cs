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
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.Test.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerialization.Test.ExtensionModel
{
	public class ReferencesExtensionTests
	{
		readonly static Guid Guid = new Guid("{6DBB618F-DBBD-4909-9644-A1D955F06249}");

		[Fact]
		public void SimpleIdentity()
		{
			var support = new SerializationSupport(new ExtendedXmlConfiguration().Extend(new ReferencesExtension()).Create());
			var instance = new Subject {Id = new Guid("{0E2DECA4-CC38-46BA-9C47-94B8070D7353}"), PropertyName = "Hello World!"};
			instance.Self = instance;
			var actual = support.Assert(instance,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:identity=""1"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><Id>0e2deca4-cc38-46ba-9c47-94b8070d7353</Id><Self exs:reference=""1"" /><PropertyName>Hello World!</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.Same(actual, actual.Self);
		}

		[Fact]
		public void EnabledWithoutConfiguration()
		{
			var support = new SerializationSupport(new ExtendedXmlConfiguration().Extend(new ReferencesExtension()).Create());
			var expected = new Subject
			               {
				               Id = Guid,
				               PropertyName = "Primary Root",
				               Self = new Subject {Id = Guid, PropertyName = "Another subject"}
			               };
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><Id>6dbb618f-dbbd-4909-9644-a1d955f06249</Id><Self><Id>6dbb618f-dbbd-4909-9644-a1d955f06249</Id><PropertyName>Another subject</PropertyName></Self><PropertyName>Primary Root</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.NotSame(actual, actual.Self);
			Assert.StrictEqual(expected.Id, actual.Id);
			Assert.StrictEqual(expected.Self.Id, actual.Self.Id);
			Assert.StrictEqual(expected.Id, expected.Self.Id);
		}

		[Fact]
		public void SimpleEntity()
		{
			var descriptor = new MemberDescriptor(typeof(Subject).GetTypeInfo().GetProperty(nameof(Subject.Id)));
			var entities = new Dictionary<TypeInfo, MemberInfo>
			               {
				               {descriptor.ReflectedType, descriptor.Metadata}
			               };

			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 {descriptor.Metadata, GuidConverter.Default}
			                 };
			var sut = new ReferencesExtension(new EntityMembers(entities));

			var memberConfiguration = new MemberConfiguration(converters);
			var support = new SerializationSupport(new ExtendedXmlConfiguration(memberConfiguration).Extend(sut).Create());
			var expected = new Subject
			               {
				               Id = Guid,
				               PropertyName = "Primary Root"
			               };
			expected.Self = expected;
			var actual = support.Assert(expected,
			                            @"<?xml version=""1.0"" encoding=""utf-8""?><ReferencesExtensionTests-Subject Id=""6dbb618f-dbbd-4909-9644-a1d955f06249"" xmlns=""clr-namespace:ExtendedXmlSerialization.Test.ExtensionModel;assembly=ExtendedXmlSerializerTest""><Self xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:entity=""6dbb618f-dbbd-4909-9644-a1d955f06249"" /><PropertyName>Primary Root</PropertyName></ReferencesExtensionTests-Subject>");
			Assert.NotNull(actual.Self);
			Assert.Same(actual, actual.Self);
		}

		class Subject
		{
			[UsedImplicitly]
			public Guid Id { get; set; }

			[UsedImplicitly]
			public Subject Self { get; set; }

			[UsedImplicitly]
			public string PropertyName { get; set; }
		}
	}
}