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
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue192Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().EnableXmlText()
														 .Create()
														 .ForTesting();

			var instance = new Subject {Item = 123};
			serializer.Cycle(instance)
					  .ShouldBeEquivalentTo(instance);

			var second = new SubjectWithMembers {Item = 123, Message = "Message", Time = DateTimeOffset.Now};
			serializer.Cycle(second)
			          .ShouldBeEquivalentTo(second);

			var group = new Group2{ Type = GroupType.Large };
			serializer.Cycle(group)
			          .ShouldBeEquivalentTo(group);

			var group3 = new Group3{ CreationTime = DateTime.Now};
			serializer.Cycle(group3)
			          .ShouldBeEquivalentTo(group3);
		}

		[Fact]
		void VerifyLists()
		{
			var serializer = new ConfigurationContainer().EnableXmlText()
			                                             .Create()
			                                             .ForTesting();

			var instance = new ClassWithMixedContent {Value = new List<object>{ 123, 345 }};
			serializer.Cycle(instance)
			          .ShouldBeEquivalentTo(instance);

			var second = new ClassWithMixedContent {Value = new List<object>{ 123, "hello world?", 345 }};
			var content = serializer.Cycle(second);
			content.ShouldBeEquivalentTo(second);
			content.Value.Should()
			       .HaveCount(3);
			content.Value.Should()
			       .Contain("hello world?");
		}

		public class Subject
		{
			[XmlText]
			public int Item { get; set; }
		}

		public class SubjectWithMembers
		{
			public string Message { get; set; }

			[XmlText]
			public int Item { get; set; }

			public DateTimeOffset Time { get; set; }
		}


		public class Group2
		{
			[XmlText(Type = typeof(GroupType))] public GroupType Type = GroupType.Small;
		}

		public enum GroupType
		{
			Small,
			Medium,
			Large
		}

		public class Group3
		{
			[XmlText(Type = typeof(DateTime))] public DateTime CreationTime = DateTime.Now;
		}

		public class ClassWithMixedContent
		{
			[XmlText(typeof(string))] public List<object> Value;
		}

		[GeneratedCode("xsd", "2.0.50727.42"), Serializable, DebuggerStepThrough, DesignerCategory("code"),
		 XmlRoot(Namespace = "", IsNullable = false)]
		public class LinkList
		{
			int      idField;
			string   nameField;
			LinkList nextField;
			string[] textField;

			public int id
			{
				get { return idField; }
				set { idField = value; }
			}

			public string name
			{
				get { return nameField; }
				set { nameField = value; }
			}

			public LinkList next
			{
				get { return nextField; }
				set { nextField = value; }
			}

			[XmlText]
			public string[] Text
			{
				get { return textField; }
				set { textField = value; }
			}
		}
	}
}