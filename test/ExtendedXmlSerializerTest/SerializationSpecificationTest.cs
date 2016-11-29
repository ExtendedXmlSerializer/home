// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class SerializationSpecificationTest : BaseTest
    {
        readonly private static IExtendedXmlSerializerConfig 
            Stream = new ExtendedXmlSerializerConfig<object>( type => type == typeof(MemoryStream) ),
            Null = new ExtendedXmlSerializerConfig<object>( type => type == null ),
            Eight = new ExtendedXmlSerializerConfig<object>( type => type.Name.Length == 8 );
            
        [Fact]
        public void VerifySpecification()
        {
            var sut = new SimpleSerializationToolsFactory
                      {
                          Configurations = new List<IExtendedXmlSerializerConfig> {Null, Stream, Eight}
                      };

            Assert.Equal(Stream, sut.GetConfiguration(typeof(MemoryStream)));
            Assert.Equal(Null, sut.GetConfiguration(null));
            Assert.Equal(Eight, sut.GetConfiguration(typeof(BaseTest)));
        }
    }
}