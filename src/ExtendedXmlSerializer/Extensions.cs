// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using System.IO;
using System.Reflection;
using System.Text;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer
{
	static class Extensions
	{
		public static ISerializer<T> Get<T>(this ISerializers @this) => @this.Get(Support<T>.Key)
		                                                                     .AsValid<ISerializer<T>>();

		public static string Serialize<T>(this ISerializers @this, T instance)
		{
			using (var stream = DefaultActivators.Default.New<MemoryStream>())
			{
				@this.Get<T>()
				     .Execute(new Input<T>(stream, instance));

				stream.Seek(0, SeekOrigin.Begin);
				var result = new StreamReader(stream).ReadToEnd();
				return result;
			}
		}

		public static object Deserialize(this ISerializers @this, TypeInfo type, string data) => @this.Get(type)
		                                                                                              .Deserialize(data);

		public static T Deserialize<T>(this ISerializer<T> @this, string data)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				var result = @this.Get(stream);
				return result;
			}
		}

		public static T Deserialize<T>(this ISerializers @this, string data)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				var result = @this.Get<T>()
				                  .Get(stream);
				return result;
			}
		}

		public static void Serialize(this ISerializers @this, XmlWriter writer, object instance)
		{
			@this.Get(instance.GetType()
			                  .GetTypeInfo())
			     .AsValid<IXmlSerializer<object>>()
			     .Execute(new XmlInput<object>(writer, instance));
		}
	}
}