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
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.Tests.Support;
using ExtendedXmlSerializer.Tests.TestObject;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel
{
	public class DeferredReferencesExtensionTests
	{
		[Fact]
		public void ComplexList()
		{
			var support =
				new SerializationSupport(
					new ExtendedConfiguration().Type<TestClassReference>()
					                           .EnableReferences(x => x.Id)
					                           .Configuration.Extend(DeferredReferencesExtension.Default));

			var instance = new TestClassReferenceWithList {Parent = new TestClassReference {Id = 1}};
			var other = new TestClassReference {Id = 2, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent};
			instance.All = new List<IReference>
			               {
				               new TestClassReference {Id = 3, ObjectA = instance.Parent, ReferenceToObjectA = instance.Parent},
				               new TestClassReference {Id = 4, ObjectA = other, ReferenceToObjectA = other},
				               other,
				               instance.Parent
			               };

			support.Assert(instance,
										@"<?xml version=""1.0"" encoding=""utf-8""?><TestClassReferenceWithList xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.TestObject;assembly=ExtendedXmlSerializer.Tests""><Parent xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" Id=""1"" /><All><Capacity>4</Capacity><TestClassReference Id=""3""><ObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /></TestClassReference><TestClassReference Id=""4""><ObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /><ReferenceToObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""2"" /></TestClassReference><TestClassReference Id=""2""><ObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /><ReferenceToObjectA xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:type=""TestClassReference"" exs:entity=""1"" /></TestClassReference><TestClassReference xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/v2"" exs:entity=""1"" /></All></TestClassReferenceWithList>");
			/*Assert.NotNull(actual.Parent);
			var list = actual.All.Cast<TestClassReference>().ToList();
			Assert.Same(actual.Parent, list[0].ObjectA);
			Assert.Same(actual.Parent, list[0].ReferenceToObjectA);
			Assert.Same(list[1].ObjectA, list[1].ReferenceToObjectA);
			Assert.Same(list[1].ObjectA.To<TestClassReference>().ObjectA,
						list[1].ObjectA.To<TestClassReference>().ReferenceToObjectA);
			Assert.Same(actual.Parent, list[1].ObjectA.To<TestClassReference>().ObjectA);
			Assert.Same(list[2], list[1].ObjectA);
			Assert.Same(actual.Parent, list[3]);*/
		}
	}
}