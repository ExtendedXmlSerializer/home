using System.Diagnostics;
using System.IO;
using System.Xml;
using ExtendedXmlSerialization.Test.TestObject;
using ExtendedXmlSerialization.Write;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class WriterTest
    {
        [Fact]
        public void VerifyBasic()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Writer(XmlWriter.Create(stream)))
                {
                    /*var subject = new TestClassPrimitiveTypes();
                    subject.Init();*/
	                var subject = new TestClassWithMap();
					subject.Init();
                    new Writing(writer).Write(subject);
                }
                stream.Seek(0, SeekOrigin.Begin);

                var data = new StreamReader(stream).ReadToEnd();

                Debugger.Break();
            }
        }
    }
}
