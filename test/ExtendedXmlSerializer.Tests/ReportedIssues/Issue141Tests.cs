using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
    public class Issue141Tests
    {
        [Fact]
        public void ShouldPreserveNullStringValueIfDefaultIsNotNull()
        {
            var config = new ConfigurationContainer();
            config.Emit(EmitBehaviors.Assigned); //no matter what we put here
            var serializer = config.Create();

            string xml = serializer.Serialize(new ClassWithDefautString() { Name = null, SubNode = null });
            var deserialized = serializer.Deserialize<ClassWithDefautString>(xml);

            deserialized.Name.Should().BeNull();    //fail
        }

        [Fact]
        public void ShouldPreserveNullObjectValueIfDefaultIsNotNull()
        {
            var config = new ConfigurationContainer();
            config.Emit(EmitBehaviors.Assigned); //no matter what we put here
            var serializer = config.Create();

            string xml = serializer.Serialize(new ClassWithDefautString() { Name = null, SubNode = null });
            var deserialized = serializer.Deserialize<ClassWithDefautString>(xml);

            deserialized.SubNode.Should().BeNull();    //fail
        }

        [Fact]
        public void ShouldPreserveNullAttributeValueIfDefaultIsNotNull()
        {
            var config = new ConfigurationContainer();
            config.Emit(EmitBehaviors.Assigned); //no matter what we put here
            config.ConfigureType<ClassWithDefautString>().Member(x => x.Attribute).Attribute();
            var serializer = config.Create();

            string xml = serializer.Serialize(new ClassWithDefautString() { Name = null, SubNode = null });
            var deserialized = serializer.Deserialize<ClassWithDefautString>(xml);

            deserialized.Attribute.Should().BeNull();    //fail
        }

        [Fact]
        public void ShouldPreserveNullObjectValueIfEmitWhenReturnsTrue()
        {
            var config = new ConfigurationContainer();
            config.Emit(EmitBehaviors.Assigned); //no matter what we put here
            config.ConfigureType<ClassWithDefautString>().Member(x => x.SubNode).EmitWhen(x => true);
            var serializer = config.Create();

            string xml = serializer.Serialize(new ClassWithDefautString() { Name = null, SubNode = null });
            var deserialized = serializer.Deserialize<ClassWithDefautString>(xml);

            deserialized.Attribute.Should().BeNull();    //fail
        }

        public class ClassWithDefautString
        {
            public string Name { get; set; } = "Unnamed";
            public string Attribute { get; set; } = "Unset";
            public SubClassWithDefautString SubNode { get; set; } = new SubClassWithDefautString();
        }

        public class SubClassWithDefautString
        {
            public string Name { get; set; } = "UnnamedSubclass";
        }
    }
}
