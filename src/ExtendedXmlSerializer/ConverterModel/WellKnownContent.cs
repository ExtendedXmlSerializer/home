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

using System.Reflection;
using ExtendedXmlSerialization.ConverterModel.Converters;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ConverterModel
{
	class WellKnownContent : Selector<TypeInfo, IConverter>, IContentOption
	{
		readonly ISpecification<TypeInfo> _specification;
		public static WellKnownContent Default { get; } = new WellKnownContent();

		WellKnownContent() : this(
			BooleanConverter.Default.ToContent(),
			CharacterConverter.Default.ToContent(),
			ByteConverter.Default.ToContent(),
			UnsignedByteConverter.Default.ToContent(),
			ShortConverter.Default.ToContent(),
			UnsignedShortConverter.Default.ToContent(),
			IntegerConverter.Default.ToContent(),
			UnsignedIntegerConverter.Default.ToContent(),
			LongConverter.Default.ToContent(),
			UnsignedLongConverter.Default.ToContent(),
			FloatConverter.Default.ToContent(),
			DoubleConverter.Default.ToContent(),
			DecimalConverter.Default.ToContent(),
			EnumerationContentOption.Default,
			DateTimeConverter.Default.ToContent(),
			DateTimeOffsetConverter.Default.ToContent(),
			StringConverter.Default.ToContent(),
			GuidConverter.Default.ToContent(),
			TimeSpanConverter.Default.ToContent()
		) {}

		public WellKnownContent(params IContentOption[] options)
			: this(new AnySpecification<TypeInfo>(options), options) {}

		public WellKnownContent(ISpecification<TypeInfo> specification, params IContentOption[] options)
			: base(options)
		{
			_specification = specification;
		}

		public bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);
	}
}