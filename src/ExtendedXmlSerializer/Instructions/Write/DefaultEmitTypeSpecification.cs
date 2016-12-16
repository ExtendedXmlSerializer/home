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

using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.Instructions.Write
{
    class DefaultEmitTypeSpecification : ISpecification<ISerialization>
    {
        public static DefaultEmitTypeSpecification Default { get; } = new DefaultEmitTypeSpecification();
        DefaultEmitTypeSpecification() {}

        public bool IsSatisfiedBy(ISerialization parameter)
        {
            /*var current = parameter.Current;
            var context = current.GetMemberScope();
            if (context != null)
            {
                var member = context.Instance;
                var result = current is IInstanceScope && current.Parent != null && member.IsWritable &&
                             current.Definition.Type != member.MemberType;
                return result;
            }*/

            /*var array = current.GetArrayContext();
            if (array != null)
            {
                var type = array.Instance.GetType();
                var elementType = ElementTypeLocator.Default.Locate(type);
                var result = type != elementType;
                return result;
            }

            var dictionary = current.GetDictionaryContext();
            if (dictionary != null)
            {
                var result = dictionary.Instance.GetType() != dictionary.Definition.GenericArguments[1];
                return result;
            }*/
            return false;
        }
    }
}