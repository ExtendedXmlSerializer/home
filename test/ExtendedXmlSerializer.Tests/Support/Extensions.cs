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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;

namespace ExtendedXmlSerializer.Tests.Support
{
	static class Extensions
	{
		public static T Cycle<T>(this IExtendedXmlSerializer @this, T instance)
		{
			var serialize  = @this.Serialize(instance);
			var result     = @this.Deserialize<T>(serialize);
			result.Should().BeEquivalentTo(instance);
			return result;
		}

		public static T Cycle<T>(this ISerializers @this, T instance)
		{
			var serializer = @this.Get<T>();
			var serialize = @this.Serialize(instance);
			var result = serializer.Deserialize(serialize);
			result.Should().BeEquivalentTo(instance);
			return result;
		}

		public static SerializationSupport ForTesting(this IConfigurationElement @this) => new SerializationSupport(@this);
		public static SerializationSupport ForTesting(this IExtendedXmlSerializer @this) => new SerializationSupport(@this);

		public static SerializersSupport ToSupport(this IConfigurationRoot @this) => new SerializersSupport(@this);
		public static SerializersSupport ToSupport(this ISerializers @this) => new SerializersSupport(@this);

		public static SerializersSupport ToSupport(this IConfigurationElement @this)
			=> new SerializersSupport(@this.Create<ISerializers>());
	}
}