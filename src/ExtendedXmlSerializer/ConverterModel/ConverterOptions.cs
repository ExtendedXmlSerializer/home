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
using ExtendedXmlSerialization.ConverterModel.Collections;
using ExtendedXmlSerialization.ConverterModel.Converters;
using ExtendedXmlSerialization.ConverterModel.Members;

namespace ExtendedXmlSerialization.ConverterModel
{
	class ConverterOptions : IConverterOptions
	{
		readonly IConverters _converters;

		public ConverterOptions(IConverters converters)
		{
			_converters = converters;
		}

		public IEnumerator<IConverterOption> GetEnumerator()
		{
			yield return BooleanConverter.Default;
			yield return CharacterConverter.Default;
			yield return ByteConverter.Default;
			yield return UnsignedByteConverter.Default;
			yield return ShortConverter.Default;
			yield return UnsignedShortConverter.Default;
			yield return IntegerConverter.Default;
			yield return UnsignedIntegerConverter.Default;
			yield return LongConverter.Default;
			yield return UnsignedLongConverter.Default;
			yield return FloatConverter.Default;
			yield return DoubleConverter.Default;
			yield return DecimalConverter.Default;
			yield return EnumerationTypeConverter.Default;
			yield return DateTimeConverter.Default;
			yield return DateTimeOffsetConverter.Default;
			yield return StringConverter.Default;
			yield return GuidConverter.Default;
			yield return TimeSpanConverter.Default;

			// yield return new DictionaryContext();
			yield return new CollectionOption(_converters);

			yield return new MemberedConverterOption(new Members.Members(_converters));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}