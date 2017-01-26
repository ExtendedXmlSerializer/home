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
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	sealed class ConverterOptions : IParameterizedSource<IRootConverter, IEnumerable<IConverterOption>>
	{
		public ConverterOptions(IAddDelegates add) : this(add, KnownConverters.Default) {}

		readonly IAddDelegates _add;
		readonly IConverterOption _known;

		public ConverterOptions(IAddDelegates add, IConverterOption known)
		{
			_add = add;
			_known = known;
		}

		public IEnumerable<IConverterOption> Get(IRootConverter parameter)
		{
			yield return _known;

			var emitter = new Emitter(parameter);
			yield return
				new ConverterOption<IReadOnlyCollectionMemberElement>(new Converter(new EnumeratingReader(parameter), emitter));
			yield return new ConverterOption<IMemberElement>(new Converter(parameter, emitter));

			var activators = new Activators();

			var converter = new SelectingConverter(parameter);
			yield return new ConverterOption<IDictionaryElement>(new DictionaryConverter(activators, converter));
			yield return new ConverterOption<IArrayElement>(new ArrayConverter(converter));
			yield return new ConverterOption<ICollectionElement>(new EnumerableConverter(converter, activators, _add));
			yield return new ConverterOption<IActivatedElement>(new InstanceConverter(activators, converter));
		}

		class InstanceConverter : Converter
		{
			public InstanceConverter(IActivators activators, IConverter converter)
				: base(new InstanceBodyReader(activators, converter), new InstanceBodyWriter(converter)) {}
		}
	}
}