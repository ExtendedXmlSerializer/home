using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue218Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new MyListImpl<string>("Hello World!") {"One", "Two"};
			new ConfigurationContainer().EnableParameterizedContent()
			                            .Create()
			                            .ForTesting()
			                            .Cycle(instance)
			                            .Should().BeEquivalentTo(instance);
		}

		[Fact]
		void VerifyParent()
		{
			var instance = new SerializedObject();
			new ConfigurationContainer().EnableParameterizedContent()
			                            .EnableReferences()
			                            .UseOptimizedNamespaces()
			                            .Create()
			                            .ForTesting()
			                            .Assert(instance,
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue218Tests-SerializedObject xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:identity=""1"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues""><MyListImpl><Owner exs:type=""Issue218Tests-SerializedObject"" exs:reference=""1"" /><Capacity>4</Capacity><sys:string>Test</sys:string><sys:string>One</sys:string><sys:string>Two</sys:string></MyListImpl></Issue218Tests-SerializedObject>");
		}

		[Fact]
		void VerifySerializeWithParent()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContent()
			                                             .EnableReferences()
			                                             .UseOptimizedNamespaces()
			                                             .Create()
			                                             .ForTesting();

			var p = new Parent();
			p.Childs.Add("test");

			Action action = () => serializer.Serialize(p);
			action.Should().Throw<InvalidOperationException>()
			      .WithMessage(
			                   "The serializer for type \'ExtendedXmlSerializer.Tests.ReportedIssues.Issue218Tests+ChildList\' could not be found.  Please ensure that the type is a valid type can be activated. Parameterized Content is enabled on the container.  By default, the type must satisfy the following rules if a public parameterless constructor is not found:\r\n\r\n- Each member must not already be marked as an explicit contract\r\n- Must be a public fields / property.\r\n- Any public fields (spit) must be readonly\r\n- Any public properties must have a get but not a set (on the public API, at least)\r\n- There must be exactly one interesting constructor, with parameters that are a case-insensitive match for each field/property in some order (i.e. there must be an obvious 1:1 mapping between members and constructor parameter names)\r\n\r\nMore information can be found here: https://github.com/ExtendedXmlSerializer/home/issues/222");
		}

		[Fact]
		void VerifyPublicFieldsWorkAsExpected()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContent()
			                                             .EnableReferences()
			                                             .UseOptimizedNamespaces()
			                                             .Create()
			                                             .ForTesting();

			var p = new ValidParent();
			p.Childs.Add("test");

			serializer.Serialize(p);
		}

		[Fact]
		void DefaultReadonly()
		{
			var collection = new Collection<object> {123, "hello world!"};
			var instance   = new DefaultReadonlyParent(collection);
			new ConfigurationContainer().Create()
			                            .Cycle(instance)
			                            .Should().BeEquivalentTo(instance);
		}

		sealed class DefaultReadonlyParent
		{
			[UsedImplicitly]
			public DefaultReadonlyParent() : this(new Collection<object>()) {}

			public DefaultReadonlyParent(ICollection<object> objects) => Objects = objects;

			public ICollection<object> Objects { [UsedImplicitly] get; }
		}

		public class Parent
		{
			public Parent() => Childs = new ChildList(this);

			// ReSharper disable once CollectionNeverQueried.Global
			public ChildList Childs { get; }
		}

		public class ValidParent
		{
			public ValidParent() => Childs = new ValidChildList(this);

			public ValidChildList Childs { get; }
		}

		public class ValidChildList : List<string>
		{
			public readonly object owner;

			public ValidChildList(object owner) => this.owner = owner;
		}

		public class ChildList : List<string>
		{
			[UsedImplicitly] readonly object _owner;

			public ChildList(object owner) => _owner = owner;
		}

		public class MyListImpl<T> : List<T>
		{
			public MyListImpl(object owner) => Owner = owner;

			public object Owner { [UsedImplicitly] get; }
		}

		public class SerializedObject
		{
			public SerializedObject() => MyListImpl = new MyListImpl<string>(this) {"Test", "One", "Two"};

			// ReSharper disable once MemberHidesStaticFromOuterClass
			public MyListImpl<string> MyListImpl { [UsedImplicitly] get; }
		}
	}
}