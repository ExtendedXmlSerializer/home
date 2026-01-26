using System.IO;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
    public class RegressableXmlReaderTests
    {
        sealed class DummyIdentity : IIdentity
        {
            public DummyIdentity(string name, string identifier)
            {
                Name = name;
                Identifier = identifier;
            }

            public string Identifier { get; }
            public string Name { get; }
        }

        sealed class DummyFormatReaderContext : IFormatReaderContext
        {
            public MemberInfo Get(string parameter) => null;
            public IIdentity Get(string name, string identifier) => new DummyIdentity(name, identifier);
            public void Dispose() { }
        }

        private const string uri = "http://default";
        private static readonly string[] rootAttributes = new string[] { uri, "urn:ns", "value" };
        private static readonly string[] childsAttributes = new string[] { "some" };
        private static readonly (string, string)[] childValues =
        {
            ("child1", "text1"),
            ("child2", "text2")
        };
        private const string xml = @"<?xml version=""1.0""?>
<root xmlns=""http://default"" xmlns:ns=""urn:ns"" attr=""value"">
    <child1 value=""some"">text1</child1>
    <child2 value=""some"">text2</child2>
</root>";

        private static void MoveToEnd(System.Xml.XmlReader reader)
        {
            while (reader.Read() && reader.NodeType != XmlNodeType.EndElement || reader.Depth != 0) ;
            reader.NodeType.Should().Be(XmlNodeType.EndElement);
            reader.Depth.Should().Be(0);
            reader.Name.Should().Be("root");
        }

        [Fact]
        public void SavePositionMovePosition()
        {
            var ctx = new DummyFormatReaderContext();
            using (var stringReader = new StringReader(xml))
            using (var xr = System.Xml.XmlReader.Create(stringReader, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true }))
            using (var reader = new RegressableXmlReader(ctx, xr, xr.LookupNamespace(string.Empty)))
            {
                // move to root
                reader.Read().Should().BeTrue();
                reader.MoveToContent().Should().Be(XmlNodeType.Element);
                reader.Name.Should().Be("root");
                reader.IsRegressable.Should().BeTrue();

                // save position
                var position = reader.GetPosition();
                position.Should().NotBeNull();

                // move to root end 
                MoveToEnd(reader);

                // move to root head
                reader.SetPosition(position).Should().BeTrue();
                reader.NodeType.Should().Be(XmlNodeType.Element);
                reader.Name.Should().Be("root");
                reader.IsRegressable.Should().BeTrue();

                // move to attribute "attr" 
                reader.MoveToAttribute(2);
                reader.Name.Should().Be("attr");
                reader.Value.Should().Be(rootAttributes[2]);

                // save position
                position = reader.GetPosition();
                position.Should().NotBeNull();

                // move to root end 
                MoveToEnd(reader);

                // move to attribute "attr" 
                reader.SetPosition(position).Should().BeTrue();
                reader.Name.Should().Be("attr");
                reader.Value.Should().Be(rootAttributes[2]);

                // move to child1
                reader.MoveToContent().Should().Be(XmlNodeType.Element);
                reader.Read().Should().BeTrue();
                reader.MoveToContent().Should().Be(XmlNodeType.Element);
                reader.Name.Should().Be("child1");

                // save position
                position = reader.GetPosition();
                position.Should().NotBeNull();

                // move to root end 
                MoveToEnd(reader);

                // move to child1
                reader.SetPosition(position).Should().BeTrue();
                reader.Name.Should().Be("child1");

                // move to text1
                while (reader.NodeType != XmlNodeType.Text)
                    reader.Read().Should().BeTrue();
                reader.Value.Should().Be("text1");

                // save position
                position = reader.GetPosition();
                position.Should().NotBeNull();

                // move to root end 
                MoveToEnd(reader);

                // move to text1
                reader.SetPosition(position).Should().BeTrue();
                reader.NodeType.Should().Be(XmlNodeType.Text);
                reader.Value.Should().Be("text1");
            }
        }

        [Fact]
        public void TraverseDocumentHandleContent()
        {
            var ctx = new DummyFormatReaderContext();
            using (var stringReader = new StringReader(xml))
            using (var xr = System.Xml.XmlReader.Create(stringReader, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true }))
            using (var reader = new RegressableXmlReader(ctx, xr, xr.LookupNamespace(string.Empty)))
            // using (var reader = System.Xml.XmlReader.Create(stringReader))
            {
                // initial state = Document
                reader.NodeType.Should().Be(XmlNodeType.None);

                // move to root element
                reader.Read().Should().BeTrue();
                reader.MoveToContent().Should().Be(XmlNodeType.Element);
                reader.NodeType.Should().Be(XmlNodeType.Element);
                reader.NodeType.Should().Be(XmlNodeType.Element);
                reader.LocalName.Should().Be("root");

                // move to root attribute
                reader.MoveToFirstAttribute().Should().BeTrue();

                // back to element
                reader.MoveToElement().Should().BeTrue();
                reader.NodeType.Should().Be(XmlNodeType.Element);
                reader.Read().Should().BeTrue();

                // read childs elements and their content
                foreach (var (name, content) in childValues)
                {
                    reader.MoveToContent().Should().Be(XmlNodeType.Element);

                    reader.LocalName.Should().Be(name);
                    reader.Depth.Should().Be(1);

                    var actual = reader.ReadElementContentAsString();
                    actual.Should().Be(content);
                }

                // namespace lookups
                reader.LookupNamespace("ns").Should().Be("urn:ns");
                reader.LookupNamespace(string.Empty).Should().Be(uri);
            }
        }

        [Fact]
        public void TraverseDocumentHandleAttributes()
        {
            var ctx = new DummyFormatReaderContext();
            using (var stringReader = new StringReader(xml))
            using (var xr = System.Xml.XmlReader.Create(stringReader, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true }))
            using (var reader = new RegressableXmlReader(ctx, xr, xr.LookupNamespace(string.Empty)))
            // using (var reader = System.Xml.XmlReader.Create(stringReader))
            {
                // move to root element
                reader.Read().Should().BeTrue();
                reader.MoveToContent().Should().Be(XmlNodeType.Element);
                reader.NodeType.Should().Be(XmlNodeType.Element);
                reader.LocalName.Should().Be("root");
                reader.NamespaceURI.Should().Be(uri);

                // attributes access
                CheckAttributes(rootAttributes);

                // namespace lookups
                reader.LookupNamespace("ns").Should().Be("urn:ns");
                reader.LookupNamespace(string.Empty).Should().Be(uri);

                // go to child1
                reader.Read().Should().BeTrue(); 
                reader.MoveToContent().Should().Be(XmlNodeType.Element);
                CheckAttributes(childsAttributes);

                // go to root end element
                MoveToEnd(reader);

                void CheckAttributes(string[] attributes)
                {
                    reader.AttributeCount.Should().Be(attributes.Length);
                    foreach (var attribute in attributes)
                    {
                        reader.MoveToNextAttribute().Should().BeTrue();
                        reader.NodeType.Should().Be(XmlNodeType.Attribute);
                        reader.Value.Should().Be(attribute);
                        reader.Content().Should().Be(attribute);
                        reader.AttributeCount.Should().Be(attributes.Length);
                    }
                }
            }
        }

        [Fact]
        public void TraverDocumentDeeps()
        {
            var ctx = new DummyFormatReaderContext();
            using (var stringReader = new StringReader(xml))
            using (var xr = System.Xml.XmlReader.Create(stringReader, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true }))
            using (var reader = new RegressableXmlReader(ctx, xr, xr.LookupNamespace(string.Empty)))
            // using (var reader = System.Xml.XmlReader.Create(stringReader))
            {
                // initial state = Document
                reader.NodeType.Should().Be(XmlNodeType.None);

                // move to root element
                reader.Read().Should().BeTrue();
                reader.MoveToContent().Should().Be(XmlNodeType.Element);
                reader.NodeType.Should().Be(XmlNodeType.Element);
                reader.NodeType.Should().Be(XmlNodeType.Element);
                reader.LocalName.Should().Be("root");

                // move to root attribute
                reader.MoveToFirstAttribute().Should().BeTrue();

                for (int i = 0; i < childValues.Length; i++)
                {
                    // move to element 
                    while (reader.NodeType != XmlNodeType.Element)
                    {
                        reader.Depth.Should().Be(1);
                        reader.Read().Should().BeTrue();
                    }
                    
                    // move to text
                    while (reader.NodeType != XmlNodeType.Text)
                    {
                        reader.Depth.Should().Be(1);
                        reader.Read().Should().BeTrue();
                    }

                    // move to end element
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Depth.Should().Be(2);
                        reader.Read().Should().BeTrue();
                    }

                    reader.Depth.Should().Be(1);
                }

                // namespace lookups
                reader.LookupNamespace("ns").Should().Be("urn:ns");
                reader.LookupNamespace(string.Empty).Should().Be(uri);
            }
        }

    }
}
