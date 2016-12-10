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

namespace ExtendedXmlSerialization.Plans.Write
{
    class DefaultEmitTypeSpecification : ISpecification<IWritingContext>
    {
        public static DefaultEmitTypeSpecification Default { get; } = new DefaultEmitTypeSpecification();
        DefaultEmitTypeSpecification() {}

        public bool IsSatisfiedBy(IWritingContext parameter)
        {
            var context = parameter.GetMemberContext().GetValueOrDefault();
            switch (context.State)
            {
                case ProcessState.MemberValue:
                    var member = context.Member.GetValueOrDefault();
                    var result = member.IsWritable && member.Value.GetType() != member.MemberType;
                    return result;
            }
            var array = parameter.GetArrayContext();
            if (array != null)
            {
                var elementType = ElementTypeLocator.Default.Locate(array.Value.Instance.GetType());
                var result = parameter.Current.Instance.GetType() != elementType;
                return result;
            }

            var dictionary = parameter.GetDictionaryContext();
            if (dictionary != null)
            {
                var type = TypeDefinitionCache.GetDefinition(dictionary.Value.Instance.GetType()).GenericArguments[1];
                var result = parameter.Current.Instance.GetType() != type;
                return result;
            }
            return false;
        }
    }
}