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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Linq;
using ExtendedXmlSerialization.Test.TestObject;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
/*
    public class FuturesTest
    {
        [Fact]
        public void VerifyAutoAttributes()
        {
            var serializer = ExtendedSerialization.Default.Get(SerializerFuturesProfile.Uri);
            var instance = new TestClassPrimitiveTypes().Init();
            var data = serializer.Serialize(instance);
            Assert.Equal(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestClassPrimitiveTypes xmlns=""clr-namespace:ExtendedXmlSerialization.Test.TestObject;assembly=ExtendedXmlSerializerTest"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures"" PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropDecimalMinValue=""-79228162514264337593543950335"" PropDecimalMaxValue=""79228162514264337593543950335"" PropFloat=""7.4432"" PropFloatNaN=""NaN"" PropFloatPositiveInfinity=""INF"" PropFloatNegativeInfinity=""-INF"" PropFloatMinValue=""-3.40282347E+38"" PropFloatMaxValue=""3.40282347E+38"" PropDouble=""3.4234"" PropDoubleNaN=""NaN"" PropDoublePositiveInfinity=""INF"" PropDoubleNegativeInfinity=""-INF"" PropDoubleMinValue=""-1.7976931348623157E+308"" PropDoubleMaxValue=""1.7976931348623157E+308"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""103"" PropString=""TestString"" />",
                data
            );
        }

        [Fact]
        public void VerifyAutoAttributesWithLongContent()
        {
            var serializer = ExtendedSerialization.Default.Get(SerializerFuturesProfile.Uri);
            var instance = new TestClassPrimitiveTypes().Init();
            instance.PropString =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed urna sapien, pulvinar et consequat sit amet, fermentum in volutpat. This sentence should break the property out into content.";
            var data = serializer.Serialize(instance);
            Assert.Equal(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestClassPrimitiveTypes xmlns=""clr-namespace:ExtendedXmlSerialization.Test.TestObject;assembly=ExtendedXmlSerializerTest"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures"" PropInt=""-1"" PropuInt=""2234"" PropDecimal=""3.346"" PropDecimalMinValue=""-79228162514264337593543950335"" PropDecimalMaxValue=""79228162514264337593543950335"" PropFloat=""7.4432"" PropFloatNaN=""NaN"" PropFloatPositiveInfinity=""INF"" PropFloatNegativeInfinity=""-INF"" PropFloatMinValue=""-3.40282347E+38"" PropFloatMaxValue=""3.40282347E+38"" PropDouble=""3.4234"" PropDoubleNaN=""NaN"" PropDoublePositiveInfinity=""INF"" PropDoubleNegativeInfinity=""-INF"" PropDoubleMinValue=""-1.7976931348623157E+308"" PropDoubleMaxValue=""1.7976931348623157E+308"" PropLong=""234234142"" PropUlong=""2345352534"" PropShort=""23"" PropUshort=""2344"" PropDateTime=""2014-01-23T00:00:00"" PropByte=""23"" PropSbyte=""33"" PropChar=""103"">
  <PropString>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed urna sapien, pulvinar et consequat sit amet, fermentum in volutpat. This sentence should break the property out into content.</PropString>
</TestClassPrimitiveTypes>",
                data
            );
        }

        [Fact]
        public void BasicDictionary()
        {
            var instance = new Dictionary<int, string>
                           {
                               {1, "First"},
                               {2, "Second"},
                               {3, "Other"}
                           };
            var serializer = ExtendedSerialization.Default.Get(SerializerFuturesProfile.Uri);
            var data = serializer.Serialize(instance);
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfInt32String xmlns=""clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures"">
  <exs:Item>
    <exs:Key>1</exs:Key>
    <exs:Value>First</exs:Value>
  </exs:Item>
  <exs:Item>
    <exs:Key>2</exs:Key>
    <exs:Value>Second</exs:Value>
  </exs:Item>
  <exs:Item>
    <exs:Key>3</exs:Key>
    <exs:Value>Other</exs:Value>
  </exs:Item>
</ArrayOfInt32String>", data);
        }

        [Fact]
        public void VerifyComplexPropertyFromExternalAssembly()
        {
            var serializer = ExtendedSerialization.Default.Get(SerializerFuturesProfile.Uri);
            var instance = new SubjectWithPropertyFromExternalAssembly {};
            var data = serializer.Serialize(instance);
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?>
<SubjectWithPropertyFromExternalAssembly xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures"" xmlns:ns1=""clr-namespace:System.Xml.Linq;assembly=System.Xml.XDocument"" xmlns:ns2=""clr-namespace:System.Collections;assembly=System.Private.CoreLib"" xmlns:ns3=""clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib"" xmlns:sys=""https://github.com/wojtpl2/ExtendedXmlSerializer/primitives"">
  <List exs:type=""ns3:ArrayOfString"">
    <sys:string>Hello World!</sys:string>
  </List>
  <Comparer exs:type=""ns1:XNodeDocumentOrderComparer"" />
</SubjectWithPropertyFromExternalAssembly>", data);
        }

        public class SubjectWithPropertyFromExternalAssembly
        {
            public ICollection<string> List { get; set; } = new List<string> {"Hello World!"};

            public IComparer Comparer { get; set; } = new XNodeDocumentOrderComparer();
        }

        [Fact]
        public void WriterExtensionForAttachedProperties()
        {
            var serializer = ExtendedSerialization.Default.Get(SerializerFuturesProfile.Uri);
            // serializer.Extensions.Add(AttachedPropertyExtension.Default);

            var subject = new BasicSubject {BasicProperty = "This is a basic property"};
            var data = serializer.Serialize(subject);
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?>
<BasicSubject xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures"" BasicProperty=""This is a basic property"" test:hello=""Hello World!  This is an attached property."" xmlns:test=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures/testing"" />",
                         data);
        }

        class BasicSubject
        {
            public string BasicProperty { get; set; }
        }

        class AttachedPropertyExtension : WritingExtensionBase
        {
            public static AttachedPropertyExtension Default { get; } = new AttachedPropertyExtension();
            AttachedPropertyExtension() {}

            public override bool IsSatisfiedBy(IWriting services)
            {
                services.Attach(new AttachedProperty("Hello World!  This is an attached property."));
                return base.IsSatisfiedBy(services);
            }

            public override void Accept(IExtensionRegistry registry) => registry.RegisterSpecification(ProcessState.Instance, this);
        }

        class AttachedProperty : PropertyBase
        {
            readonly private static Namespace Namespace = new Namespace("test",
                                                                        new Uri(
                                                                            "https://github.com/wojtpl2/ExtendedXmlSerializer/futures/testing"));

            public AttachedProperty(string value) : base(Namespace, "hello", value) {}
        }

        [Fact]
        public void DemonstrateExtensionSkippingElements()
        {
            var subject = new List<int> {1, 2, 3, 10, 11, 12, 13, 14};
            var serializer = ExtendedSerialization.Default.Get(SerializerFuturesProfile.Uri);
            // serializer.Extensions.Add(SkipUnluckyNumberExtension.Default);

            var data = serializer.Serialize(subject);
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfInt32 xmlns=""clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures"" xmlns:sys=""https://github.com/wojtpl2/ExtendedXmlSerializer/primitives"">
  <sys:int>1</sys:int>
  <sys:int>2</sys:int>
  <sys:int>3</sys:int>
  <sys:int>10</sys:int>
  <sys:int>11</sys:int>
  <sys:int>12</sys:int>
  <sys:int>14</sys:int>
</ArrayOfInt32>", data);
        }

        class SkipUnluckyNumberExtension : WritingExtensionBase
        {
            public static SkipUnluckyNumberExtension Default { get; } = new SkipUnluckyNumberExtension();
            SkipUnluckyNumberExtension() {}

            public override bool IsSatisfiedBy(IWriting services)
            {
                var instance = services.Current.Instance;
                if (instance is int)
                {
                    var integer = (int) instance;
                    var result = integer != 13;
                    return result;
                }
                return true;
            }

            public override void Accept(IExtensionRegistry registry) => registry.RegisterSpecification(ProcessState.Instance, this);
        }

        /*[Fact]
        public void DemonstrateProfileThatModifiesValues()
        {
            var subject = new List<int> {11, 12, 13, 14, 15};
            var serializer = new LuckyProfile().New();
            var data = serializer.Serialize(subject);
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfInt32 xmlns=""clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures/lucky"" xmlns:sys=""https://github.com/wojtpl2/ExtendedXmlSerializer/primitives"">
  <sys:int>11</sys:int>
  <sys:int>12</sys:int>
  <sys:int>7</sys:int>
  <sys:int>14</sys:int>
  <sys:int>15</sys:int>
</ArrayOfInt32>", data);
        }#1#

        /*class LuckyProfile : SerializationProfile
        {
            public LuckyProfile()
                : base(
                    AutoAttributeSpecification.Default,
                    new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/futures/lucky")) {}

            protected override ISerializationToolsFactoryHost CreateHost(IImmutableList<object> services) 
                => new SerializationToolsFactoryHost(/*() => new AlteringWriting(ContextAlteration.Default),#2# services);
        }#1#

        /*class ContextAlteration : IAlteration<WriteContext>
        {
            public static ContextAlteration Default { get; } = new ContextAlteration();
            ContextAlteration() {}

            public WriteContext Get(WriteContext parameter)
                =>
                parameter.Instance?.Equals(13) ?? false
                    ? new WriteContext(parameter.Parent, parameter.State, parameter.Root, 7, parameter.Definition, parameter.Members, parameter.Member)
                    : parameter;
        }#1#

        [Fact]
        public void CustomWritePlanForListsWithInheritance()
        {
            var instance = new TestList {new SomeObject {Name = "One"}, new SomeObject {Name = "Two"}};
            instance.PropertyName = "HELLO WORLD!!";
            var serializer = ExtendedSerialization.Default.Get(SerializerFuturesProfile.Uri);
            var data = serializer.Serialize(instance);
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?>
<TestList xmlns=""clr-namespace:ExtendedXmlSerialization.Test;assembly=ExtendedXmlSerializerTest"" xmlns:exs=""https://github.com/wojtpl2/ExtendedXmlSerializer/futures"" Capacity=""4"" PropertyName=""HELLO WORLD!!"">
  <SomeObject Name=""One"" />
  <SomeObject Name=""Two"" />
</TestList>", data);
        }

        public class TestList : List<ISomeObject>
        {
            public string PropertyName { get; set; }
        }

        public interface ISomeObject
        {
            string Name { get; set; }
        }

        public class SomeObject : ISomeObject
        {
            public string Name { get; set; }
        }
    }
*/
}