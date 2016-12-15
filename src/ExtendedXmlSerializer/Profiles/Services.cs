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
using ExtendedXmlSerialization.Extensibility;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Instructions.Write;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.Profiles
{
/*
    class DefaultServices : Services
    {
        public new static DefaultServices Default { get; } = new DefaultServices();
        DefaultServices() : base(DefaultEmitTypeSpecification.Default, DefaultMemberValueAssignedExtension.Default) {}
    }

    class Services : IEnumerable<object>
    {
        public static Services Default { get; } = new Services();
        Services() : this(EmitTypeSpecification.Default, MemberValueAssignedExtension.Default) {}

        private readonly ISpecification<ISerialization> _specification;
        private readonly IInstruction _instruction;
        private readonly IExtension _extension;

        public Services(ISpecification<ISerialization> specification, IExtension extension) : this(specification, EmitTypeInstruction.Default, extension) {}

        public Services(ISpecification<ISerialization> specification, IInstruction instruction, IExtension extension)
        {
            _specification = specification;
            _instruction = instruction;
            _extension = extension;
        }

        public IEnumerator<object> GetEnumerator()
        {
            yield return _extension;
            yield return new ObjectReferencesExtension(new ConditionalInstruction<ISerialization>(_specification, _instruction));
            yield return VersionExtension.Default;
            yield return new CustomSerializationExtension(_instruction);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
*/
}