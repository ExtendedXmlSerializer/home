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
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.NewConfiguration;

namespace ExtendedXmlSerialization.Legacy
{
    sealed class LegacyConverterOptions : IParameterizedSource<IConverter, IEnumerable<IConverterOption>>
    {
        private readonly ExtendedXmlSerializerConfig _config;
        
        public LegacyConverterOptions(ExtendedXmlSerializerConfig config)
        {
            _config = config;
            
        }

        public IEnumerable<IConverterOption> Get(IConverter parameter)
        {
            var element = new ElementSelectingConverter(parameter);
            yield return new ReadOnlyCollectionMemberConverterOption(element);
            yield return new LegacyMemberOption(_config, new MemberConverterOption(element));
            yield return KnownConverters.Default;
            yield return new ConverterOption<IDictionaryElement>(new DictionaryConverter(parameter));
            yield return new ConverterOption<IArrayElement>(new LegacyArrayTypeConverter(_config, parameter));
            yield return new ConverterOption<ICollectionElement>(new LegacyEnumerableTypeConverter(_config, parameter));
            yield return new ConverterOption<IActivatedElement>(new LegacyInstanceTypeConverter(_config, parameter));
        }

        sealed class MemberConverterOption : ConverterOption<IMemberElement>
        {
            public MemberConverterOption(IConverter converter) : base(
                new Converter(converter,
                              new ValidatingAssignedWriter(new Emitter(new LegacyMemberTypeEmittingWriter(converter))))) {}
        }
    }
}