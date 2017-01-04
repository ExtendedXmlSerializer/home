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

namespace ExtendedXmlSerialization.Test
{
    public class ContextsTests
    {
        /* [Fact]
         public void PrimitiveWrite()
         {
             var stream = new MemoryStream();
             var serializer = new Serializer(new ConditionalCompositeWriter(IntegerConverter.Default));
             serializer.Serialize(stream, 6776);
             stream.Seek(0, SeekOrigin.Begin);
             var actual = new StreamReader(stream).ReadToEnd();
             Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?><int>6776</int>", actual);
         }
 
         [Fact]
         public void PrimitiveRead()
         {
             const string data = @"<?xml version=""1.0"" encoding=""utf-8""?><int>6776</int>";
             var deserializer =
                 new Deserializer(
                     new ConditionalCompositeReader(new HintedRootTypeProvider(typeof(int), TypeProvider.Default),
                                                    IntegerConverter.Default));
             var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
             var actual = deserializer.Deserialize(stream);
             Assert.Equal(6776, actual);
         }
 
         [Fact]
         public void InstanceWrite()
         {
             var instance = new InstanceClass {PropertyName = "Hello World!"};
             var stream = new MemoryStream();
             var serializer = new Serializer(new ConditionalCompositeWriter(IntegerConverter.Default));
             serializer.Serialize(stream, instance);
             stream.Seek(0, SeekOrigin.Begin);
             var actual = new StreamReader(stream).ReadToEnd();
             Assert.Equal(
                 @"<?xml version=""1.0"" encoding=""utf-8""?><InstanceClass><PropertyName>Hello World!</PropertyName></InstanceClass>",
                 actual);
         }
 
         class InstanceClass
         {
             public string PropertyName { get; set; }
         }*/

        /*public class Primitives : Dictionary<Type, string>
        {
            public static Primitives Default { get; } = new Primitives();
            Primitives() : base(new Dictionary<Type, string>
                                {
                                    {typeof(bool), "boolean"},
                                    {typeof(char), "char"},
                                    {typeof(sbyte), "byte"},
                                    {typeof(byte), "unsignedByte"},
                                    {typeof(short), "short"},
                                    {typeof(ushort), "unsignedShort"},
                                    {typeof(int), "int"},
                                    {typeof(uint), "unsignedInt"},
                                    {typeof(long), "long"},
                                    {typeof(ulong), "unsignedLong"},
                                    {typeof(float), "float"},
                                    {typeof(double), "double"},
                                    {typeof(decimal), "decimal"},
                                    {typeof(DateTime), "dateTime"},
                                    {typeof(DateTimeOffset), "dateTimeOffset"},
                                    {typeof(string), "string"},
                                    {typeof(Guid), "guid"},
                                    {typeof(TimeSpan), "TimeSpan"},
                                }) {}
        }*/
    }
}