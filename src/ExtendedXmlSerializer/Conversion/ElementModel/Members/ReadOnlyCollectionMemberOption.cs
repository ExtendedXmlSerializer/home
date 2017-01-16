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
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.ElementModel.Members
{
    public class ReadOnlyCollectionMemberOption : MemberOptionBase
    {
        public static ReadOnlyCollectionMemberOption Default { get; } = new ReadOnlyCollectionMemberOption();
        ReadOnlyCollectionMemberOption() : this(GetterFactory.Default, AddDelegates.Default) {}

        private readonly IGetterFactory _getter;
        private readonly IAddDelegates _add;

        public ReadOnlyCollectionMemberOption(IGetterFactory getter, IAddDelegates add) : base(Specification.Instance)
        {
            _getter = getter;
            _add = add;
        }

        protected override IMemberElement Create(MemberInformation parameter, IElementName name)
        {
            var add = _add.Get(parameter.MemberType);
            if (add != null)
            {
                var getter = _getter.Get(parameter.Metadata);
                var result = new ReadOnlyCollectionMemberElement(name, parameter.Metadata, parameter.MemberType, add,
                                                                 getter);
                return result;
            }
            return null;
        }

        sealed class Specification : ISpecification<MemberInformation>
        {
            public static Specification Instance { get; } = new Specification();
            Specification() : this(IsCollectionTypeSpecification.Default) {}
            private readonly ISpecification<TypeInfo> _specification;


            public Specification(ISpecification<TypeInfo> specification)
            {
                _specification = specification;
            }

            public bool IsSatisfiedBy(MemberInformation parameter) => _specification.IsSatisfiedBy(parameter.MemberType);
        }
    }
}