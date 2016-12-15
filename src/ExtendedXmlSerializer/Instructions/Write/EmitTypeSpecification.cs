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

using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.Instructions.Write
{
    class EmitTypeSpecification : ISpecification<ISerialization>
    {
        public static EmitTypeSpecification Default { get; } = new EmitTypeSpecification();
        EmitTypeSpecification() {}

        public bool IsSatisfiedBy(ISerialization parameter)
        {
            var current = parameter.Current;
            if (current is IInstanceScope && current.Parent != null)
            {
                var context = current.GetMemberScope();
                if (context != null)
                {
                    var member = context.Instance;
                    var result = member.IsWritable &&
                                 current.Definition.Type != member.MemberType;
                    return result;
                }

                var emit = current.GetArrayContext() == null && current.GetDictionaryContext() == null;
                return emit;
            }
            return false;
        }
    }
}