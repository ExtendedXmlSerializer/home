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

using System.Collections.Generic;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.Conversion
{
	sealed class ConverterOptions : IParameterizedSource<IConverter, IEnumerable<IConverterOption>>
	{
		public static ConverterOptions Default { get; } = new ConverterOptions();
		ConverterOptions() : this(KnownConverters.Default) {}

		readonly IConverterOption _known;

		public ConverterOptions(IConverterOption known)
		{
			_known = known;
		}

		public IEnumerable<IConverterOption> Get(IConverter parameter)
		{
			yield return new ConverterOption<IDictionaryElement>(new DictionaryConverter(parameter));
			yield return new ConverterOption<IArrayElement>(new ArrayConverter(parameter));
			yield return new ConverterOption<ICollectionElement>(new EnumerableConverter(parameter));
			yield return new ConverterOption<IActivatedElement>(new InstanceConverter(parameter));
			var element = new ElementSelectingConverter(parameter);
			yield return new ReadOnlyCollectionMemberConverterOption(element);
			yield return new MemberConverterOption(element);
			yield return _known;
		}

		class InstanceConverter : Converter
		{
			public InstanceConverter(IConverter converter)
				: base(new InstanceBodyReader(converter), new InstanceBodyWriter(converter)) {}
		}
	}
}