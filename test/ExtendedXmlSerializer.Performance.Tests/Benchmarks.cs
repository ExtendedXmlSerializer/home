using System.IO;
using System.Xml.Serialization;
using BenchmarkDotNet.Attributes;
using ExtendedXmlSerialization.Performance.Tests.Model;
using ExtendedXmlSerialization.Performance.Tests.Model.ExtendedXmlSerialization;

namespace ExtendedXmlSerialization.Performance.Tests
{
    public class ExtendedXmlSerializerTest
    {
        private readonly TestClassOtherClass _obj = new TestClassOtherClass();
        private readonly string _xml;
        private readonly IExtendedXmlSerializer _serializer = new LegacyXmlSerializer();

        public ExtendedXmlSerializerTest()
        {
            _obj.Init();
            _xml = _serializer.Serialize(_obj);
        }

        [Benchmark]
        public string SerializationClassWithPrimitive() => _serializer.Serialize(_obj);

        [Benchmark]
        public TestClassOtherClass DeserializationClassWithPrimitive()
            => _serializer.Deserialize<TestClassOtherClass>(_xml);
    }


    public class ExtendedXmlSerializerV2Test
    {
        private readonly TestClassOtherClass _obj = new TestClassOtherClass();
        private readonly string _xml;
        private readonly IExtendedXmlSerializer _serializer = new ExtendedXmlSerializer();

        public ExtendedXmlSerializerV2Test()
        {
            _obj.Init();
            _xml = _serializer.Serialize(_obj);
        }

        [Benchmark]
        public string SerializationClassWithPrimitive() => _serializer.Serialize(_obj);

        [Benchmark]
        public TestClassOtherClass DeserializationClassWithPrimitive()
            => _serializer.Deserialize<TestClassOtherClass>(_xml);
    }

    public class XmlSerializerTest
    {
        private readonly TestClassOtherClass _obj = new TestClassOtherClass();
        private readonly string _xml;
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(TestClassOtherClass));

        public XmlSerializerTest()
        {
            _obj.Init();
            _xml = SerializationClassWithPrimitive();
        }

        [Benchmark]
        public string SerializationClassWithPrimitive()
        {
            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(stream, _obj);
                stream.Seek(0, SeekOrigin.Begin);
                var result = new StreamReader(stream).ReadToEnd();
                return result;
            }
        }

        [Benchmark]
        public TestClassOtherClass DeserializationClassWithPrimitive()
        {
            using (StringReader textReader = new StringReader(_xml))
            {
                return (TestClassOtherClass) _serializer.Deserialize(textReader);
            }
        }
    }
}