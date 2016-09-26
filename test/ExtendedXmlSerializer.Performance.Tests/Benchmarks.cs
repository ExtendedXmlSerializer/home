using System.IO;
using System.Xml.Serialization;
using BenchmarkDotNet.Attributes;
using ExtendedXmlSerialization.Performance.Tests.Model;

namespace ExtendedXmlSerialization.Performance.Tests
{
    public class ExtendedXmlSerializerTest
    {
        private readonly TestClassOtherClass _obj = new TestClassOtherClass();
        private readonly string _xml;
        private readonly ExtendedXmlSerializer _serializer = new ExtendedXmlSerializer();

        public ExtendedXmlSerializerTest()
        {
            _obj.Init();
            _xml = _serializer.Serialize(_obj);
        }

        [Benchmark]
        public string SerializationClassWithPrimitive()
        {
            return _serializer.Serialize(_obj);
        }
        [Benchmark]
        public TestClassOtherClass DeserializationClassWithPrimitive()
        {
            return _serializer.Deserialize<TestClassOtherClass>(_xml);
        }
    }

    public class XmlSerializerTest
    {
        private readonly TestClassOtherClass _obj = new TestClassOtherClass();
        private readonly string _xml;
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(TestClassOtherClass));

        public XmlSerializerTest()
        {
            _obj.Init();
            using (StringWriter textWriter = new StringWriter())
            {
                _serializer.Serialize(textWriter, _obj);
                _xml = textWriter.ToString();
            }
        }

        [Benchmark]
        public string SerializationClassWithPrimitive()
        {
            using (StringWriter textWriter = new StringWriter())
            {
                _serializer.Serialize(textWriter, _obj);
                return textWriter.ToString();
            }
        }

        [Benchmark]
        public TestClassOtherClass DeserializationClassWithPrimitive()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestClassOtherClass));
            StringReader textReader = new StringReader(_xml);
            return (TestClassOtherClass)xmlSerializer.Deserialize(textReader);
        }
    }
}
