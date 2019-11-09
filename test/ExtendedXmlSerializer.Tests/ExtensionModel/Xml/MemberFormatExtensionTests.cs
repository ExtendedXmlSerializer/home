using System;
using System.Xml.Serialization;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public class MemberFormatExtensionTests
	{
		[Fact]
		public void VerifyDeclared()
		{
			var instance = new Subject {PropertyName = "Testing"};
			var actual = new SerializationSupport().Assert(instance,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><MemberFormatExtensionTests-Subject PropertyName=""Testing"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests"" />");
			Assert.Equal(instance.PropertyName, actual.PropertyName);
		}

		[Fact]
		public void VerifyDifferentName()
		{
			var expected = new SubjectWithName {PropertyName = "Testing"};
			var actual = new SerializationSupport().Assert(expected,
			                                               @"<?xml version=""1.0"" encoding=""utf-8""?><MemberFormatExtensionTests-SubjectWithName AnotherDifferentName=""Testing"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ExtensionModel.Xml;assembly=ExtendedXmlSerializer.Tests"" />");
			Assert.Equal(expected.PropertyName, actual.PropertyName);
		}

		[Fact]
		public void VerifyThrow() =>
			Assert.Throws<InvalidOperationException>(
			                                         ()
				                                         => new SerializationSupport()
					                                         .Serialize(new SubjectWithNoKnownConverter
						                                                    {PropertyName = new Subject()})
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