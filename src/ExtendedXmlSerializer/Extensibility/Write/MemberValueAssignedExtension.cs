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

using System;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;

namespace ExtendedXmlSerialization.Extensibility.Write
{
    class MemberValueAssignedExtension : WritingExtensionBase
    {
        public static MemberValueAssignedExtension Default { get; } = new MemberValueAssignedExtension();
        protected MemberValueAssignedExtension() : this(DefaultValues.Default.Get) {}

        private readonly Func<Type, object> _values;

        public MemberValueAssignedExtension(Func<Type, object> values)
        {
            _values = values;
        }

        public override bool IsSatisfiedBy(IWriting services) => services.Current.Member != null && FromMember(services.Current.Member.Value);

        protected virtual bool FromMember(MemberContext member)
        {
            var defaultValue = _values(member.MemberType);
            var result = !Equals(member.Value, defaultValue);
            return result;
        }

        public override void Accept(IExtensionRegistry registry) => registry.Register(ProcessState.Member, this);
    }
}