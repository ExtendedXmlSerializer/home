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

using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.Converters.Primitives;

namespace ExtendedXmlSerialization.Converters.Legacy
{
    class LegacyPrimitives : IEnumerable<ITypeConverter>
    {
        public static LegacyPrimitives Default { get; } = new LegacyPrimitives();
        LegacyPrimitives() {}

        public IEnumerator<ITypeConverter> GetEnumerator()
        {
            yield return LegacyBooleanTypeConverter.Default;
            yield return LegacyCharacterTypeConverter.Default;
            yield return ByteTypeConverter.Default;
            yield return UnsignedByteTypeConverter.Default;
            yield return ShortTypeConverter.Default;
            yield return UnsignedShortTypeConverter.Default;
            yield return IntegerTypeConverter.Default;
            yield return UnsignedIntegerTypeConverter.Default;
            yield return LongTypeConverter.Default;
            yield return UnsignedLongTypeConverter.Default;
            yield return FloatTypeConverter.Default;
            yield return DoubleTypeConverter.Default;
            yield return DecimalTypeConverter.Default;
            yield return EnumerationTypeConverter.Default;
            yield return DateTimeTypeConverter.Default;
            yield return DateTimeOffsetTypeConverter.Default;
            yield return StringTypeConverter.Default;
            yield return GuidTypeConverter.Default;
            yield return TimeSpanTypeConverter.Default;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}