using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
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

			var group = new Group2 {Type = GroupType.Large};
			serializer.Cycle(group)
			          .ShouldBeEquivalentTo(group);

			var group3 = new Group3 {CreationTime = DateTime.Now};
			serializer.Cycle(group3)
			          .ShouldBeEquivalentTo(group3);
		}

		[Fact]
		void VerifyLists()
		{
			var serializer = new ConfigurationContainer().EnableXmlText()
			                                             .Create()
			                                             .ForTesting();

			var instance = new ClassWithMixedContent {Value = new List<object> {123, 345}};

			serializer.Assert(instance,
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue192Tests-ClassWithMixedContent xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><int xmlns=""https://extendedxmlserializer.github.io/system"">123</int><int xmlns=""https://extendedxmlserializer.github.io/system"">345</int></Issue192Tests-ClassWithMixedContent>");

			serializer.Cycle(instance)
			          .ShouldBeEquivalentTo(instance);

			var second  = new ClassWithMixedContent {Value = new List<object> {123, "hello world?", 345}};
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
			public string Message { [UsedImplicitly] get; set; }

			[XmlText]
			public int Item { get; set; }

			public DateTimeOffset Time { [UsedImplicitly] get; set; }
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
				get => idField;
				set => idField = value;
			}

			public string name
			{
				get => nameField;
				set => nameField = value;
			}

			public LinkList next
			{
				get => nextField;
				set => nextField = value;
			}

			[XmlText]
			public string[] Text
			{
				get => textField;
				set => textField = value;
			}
		}
	}
}