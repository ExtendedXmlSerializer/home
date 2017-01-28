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
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.TypeModel;
using Emitter = ExtendedXmlSerialization.Conversion.Write.Emitter;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IConverterOptions : IParameterizedSource<IRootConverter, IConverterSelector> {}

	public class ConverterOptions : IConverterOptions
	{
		readonly IAddDelegates _add;
		readonly ImmutableArray<ITypeConverter> _known;

		protected ConverterOptions(IAddDelegates add, params ITypeConverter[] known) : this(add, known.ToImmutableArray()) {}

		public ConverterOptions(IAddDelegates add, ImmutableArray<ITypeConverter> known)
		{
			_add = add;
			_known = known;
		}

		public IConverterSelector Get(IRootConverter parameter)
			=> new ConverterSelector(_known.Select(x => new ConverterOption(x)).Concat(Options(parameter)).ToArray());

		IEnumerable<IConverterOption> Options(IRootConverter parameter)
		{
			var element = new SelectingConverter(parameter);
			var emitter = new Emitter(element);
			var converter = new Converter(element, emitter);
			yield return new ConverterOption<IRoot>(converter);
			yield return
				new ConverterOption<IReadOnlyCollectionMember>(new Converter(new EnumeratingReader(converter), emitter));
			yield return new ConverterOption<IMember>(converter);
			

			var activators = new Activators();
			yield return new ConverterOption<IDictionaryElement>(new DictionaryConverter(activators, parameter));
			yield return new ConverterOption<IArrayElement>(new ArrayConverter(parameter));
			yield return new ConverterOption<ICollectionElement>(new EnumerableConverter(parameter, activators, _add));
			yield return new ConverterOption<IActivatedElement>(new ActivatedInstanceConverter(activators, parameter));
		}
	}

	
}