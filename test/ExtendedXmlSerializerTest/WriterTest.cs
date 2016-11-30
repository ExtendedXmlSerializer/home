using System.Collections.Generic;
using System.Diagnostics;
using ExtendedXmlSerialization.Common;
using ExtendedXmlSerialization.Test.TestObject;
using ExtendedXmlSerialization.Test.TestObjectConfigs;
using ExtendedXmlSerialization.Write;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class WriterTest : BaseTest
    {
        public WriterTest()
        {
            Serializer.SerializationToolsFactory = new SimpleSerializationToolsFactory()
            {
                Configurations = new List<IExtendedXmlSerializerConfig> { new TestClassReferenceConfig(), new InterfaceReferenceConfig() }
            };
        }

        [Fact]
        public void VerifyBasic()
        {

            var subject = new TestClassWithMap();
            subject.Init();
            // var subject = new Dictionary<string, string> { {"Hello", "World!"} };

/*TestClassReference subject = new TestClassReference();
                    subject.Id = 1;
                    subject.CyclicReference = subject;*/
                    /*subject.ObjectA = new TestClassReference {Id = 2};
                    subject.ReferenceToObjectA = subject.ObjectA;
                    subject.Lists = new List<TestClassReference>
                    {
                        new TestClassReference {Id = 3},
                        new TestClassReference {Id = 4}
                    };*/

            var extensions = new DefaultWriteExtensions(Serializer.SerializationToolsFactory);
            var data = new Write.Serializer(/*extensions*/).Serialize(subject);
            Debugger.Break();

        }
    }
}
