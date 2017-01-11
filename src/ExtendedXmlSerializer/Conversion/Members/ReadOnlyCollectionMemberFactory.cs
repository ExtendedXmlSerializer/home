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

using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;

namespace ExtendedXmlSerialization.Conversion.Members
{
    public class ReadOnlyCollectionMemberFactory : IMemberFactory
    {
        private readonly IEnumeratingReader _reader;
        private readonly IConverter _converter;
        private readonly IGetterFactory _getter;
        private readonly IAddDelegates _add;

        public ReadOnlyCollectionMemberFactory(IConverter converter, IEnumeratingReader reader)
            : this(converter, reader, GetterFactory.Default, AddDelegates.Default) {}

        public ReadOnlyCollectionMemberFactory(IConverter converter, IEnumeratingReader reader, IGetterFactory getter,
                                               IAddDelegates add)
        {
            _converter = converter;
            _reader = reader;
            _getter = getter;
            _add = add;
        }

        public IMemberConverter Get(IMemberElement parameter)
        {
            var add = _add.Get(parameter.MemberType);
            if (add != null)
            {
                var getter = _getter.Get(parameter.Metadata);
                var result = new ReadOnlyCollectionMemberConverter(_reader, new ElementWriter(parameter, _converter),
                                                                   parameter, getter, add);
                return result;
            }
            return null;
        }
    }
}