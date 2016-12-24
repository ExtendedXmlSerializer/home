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
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    class SimpleEmitter : IEmitter
    {
        private readonly IWriter _writer;

        public SimpleEmitter(IWriter writer)
        {
            _writer = writer;
        }

        public void Execute(IElement parameter)
        {
            var primitive = parameter.Instance as IPrimitive;
            if (primitive != null)
            {
                using (_writer.New(parameter))
                {
                    ApplyType(parameter);
                    _writer.Emit(primitive.Value);
                }
                return;
            }

            var instance = parameter.Instance as IObject;
            if (instance != null)
            {
                using (_writer.New(parameter))
                {
                    ApplyType(parameter);

                    foreach (var o in instance.Members)
                    {
                        Execute(o);
                    }

                    var enumerable = instance as IEnumerableObject;
                    if (enumerable != null)
                    {
                        foreach (var item in enumerable.Items)
                        {
                            Execute(item);
                        }
                    }
                }
                return;
            }

            var composite = parameter.Instance as IEnumerable<IElement>;
            if (composite != null)
            {
                using (_writer.New(parameter))
                {
                    foreach (var context in composite)
                    {
                        Execute(context);
                    }
                }
            }
        }

        private static bool ShouldApply(IElement element)
        {
            var entity = element.Instance;
            var enumerable = entity is IEnumerableObject;
            if (element is IRoot)
            {
                return !enumerable && !(entity is IPrimitive);
            }

            var apply = entity is IObject;
            if (apply && !enumerable)
            {
                return true;
            }

            var member = element as IMember;
            if (member != null)
            {
                return member.IsWritable && entity.Type != member.Type;
            }

            return false;
        }

        private void ApplyType(IElement element)
        {
            if (ShouldApply(element))
            {
                _writer.Emit(new TypeProperty(element.Instance.Type));
            }
        }
    }
}