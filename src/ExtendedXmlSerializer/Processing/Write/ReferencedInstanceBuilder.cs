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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    public class ReferencedInstanceBuilder : IInstanceBuilder
    {
        private readonly IInstanceBuilder _builder;
        readonly private IDictionary<object, IReference> _references = new Dictionary<object, IReference>();

        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public ReferencedInstanceBuilder(IInstanceBuilder builder)
        {
            _builder = builder;
        }

        public IInstance Get(Descriptor parameter)
        {
            var instance = parameter.Instance;
            var context = _generator.For(instance);
            if (context.FirstEncounter)
            {
                var result = _builder.Get(parameter);
                var o = result as IObject;
                if (o != null)
                {
                    _references.Add(instance, new Reference(o));
                }
                return result;
            }

            if (_references.ContainsKey(instance))
            {
                return _references[instance];
            }

            // TODO: Make optional?
            throw new SerializationException(
                      $"Recursion detected while building entity '{instance}' of type '{parameter.DeclaredType}'.");
        }
    }
}