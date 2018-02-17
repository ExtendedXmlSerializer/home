// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Members
{
	public class AllowedAssignedInstanceValuesTests
	{
		[Fact]
		public void EmitValuesBasedOnInstanceDefaults()
		{
			var instance = new SubjectWithDefaultValue();
			new SerializationSupport().Assert(instance,
			                                  @"<?xml version=""1.0"" encoding=""utf-8""?><AllowedAssignedInstanceValuesTests-SubjectWithDefaultValue xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ContentModel.Members;assembly=ExtendedXmlSerializer.Tests""><SomeValue>This is a Default Value!</SomeValue></AllowedAssignedInstanceValuesTests-SubjectWithDefaultValue>");

			var configuration = new ConfigurationContainer().Emit(EmitBehaviors.Assigned);

			var support = new SerializationSupport(configuration);
			support.Assert(instance,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><AllowedAssignedInstanceValuesTests-SubjectWithDefaultValue xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ContentModel.Members;assembly=ExtendedXmlSerializer.Tests"" />");
		}

		class SubjectWithDefaultValue
		{
			public SubjectWithDefaultValue() : this("This is a Default Value!") {}

			public SubjectWithDefaultValue(string someValue)
			{
				SomeValue = someValue;
			}

			[UsedImplicitly]
			public string SomeValue { get; set; }
		}
	}
}