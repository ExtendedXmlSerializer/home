using System.IO;
using System.Xml.Serialization;
using BenchmarkDotNet.Attributes;
using ExtendedXmlSerialization.Performance.Tests.Model;
using ExtendedXmlSerialization.Profiles;

namespace ExtendedXmlSerialization.Performance.Tests
{
    public class ExtendedXmlSerializerTest
    {
        private readonly TestClassOtherClass _obj = new TestClassOtherClass();
        private readonly string _xml;
        private readonly IExtendedXmlSerializer _serializer = new ExtendedXmlSerializer();

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


    public class ExtendedXmlSerializerV2Test
    {
        private readonly TestClassOtherClass _obj = new TestClassOtherClass();
        private readonly string _xml;
        private readonly IExtendedXmlSerializer _serializer = ExtendedSerialization.Default.Get(SerializerFuturesProfile.Uri);

        public ExtendedXmlSerializerV2Test()
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
                return (TestClassOtherClass)_serializer.Deserialize(textReader);
            }
        }
    }
}
