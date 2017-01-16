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

using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.ElementModel.Members
{
    public class MemberOption : MemberOptionBase
    {
        public static MemberOption Default { get; } = new MemberOption();
        MemberOption() : this(GetterFactory.Default, SetterFactory.Default) {}

        private readonly IGetterFactory _getter;
        private readonly ISetterFactory _setter;

        public MemberOption(IGetterFactory getter, ISetterFactory setter)
            : base(new DelegatedSpecification<MemberInformation>(x => x.Assignable))
        {
            _getter = getter;
            _setter = setter;
        }

        protected override IMemberElement Create(MemberInformation parameter, IElementName name)
        {
            var getter = _getter.Get(parameter.Metadata);
            var setter = _setter.Get(parameter.Metadata);
            var result = new MemberElement(name, parameter.Metadata, parameter.MemberType, setter, getter);
            return result;
        }
    }
}