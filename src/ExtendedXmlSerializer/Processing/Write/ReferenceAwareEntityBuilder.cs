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
    public class ReferenceAwareEntityBuilder : IEntityBuilder
    {
        private readonly IEntityBuilder _builder;
        private readonly IIdentityLocator _locator;
        readonly private IDictionary<long, IReference> _references = new Dictionary<long, IReference>();
        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        // public ReferenceAwareEntityBuilder(IEntityBuilder builder) : this(builder, DefaultIdentityLocator.Default) {}

        public ReferenceAwareEntityBuilder(IEntityBuilder builder, IIdentityLocator locator)
        {
            _builder = builder;
            _locator = locator;
        }

        public IEntity Get(ContextDescriptor parameter)
        {
            bool first;
            var id = _generator.GetId(parameter.Instance, out first);
            if (first)
            {
                var instance = _builder.Get(parameter);
                var @object = instance as IObject;
                if (@object != null)
                {
                    var identity = _locator.Get(parameter);
                    if (identity != null)
                    {
                        var item = new UniqueObject(identity, @object);
                        _references.Add(id, new Reference(item));
                        return item;
                    }
                }
                return instance;
            }

            if (_references.ContainsKey(id))
            {
                return _references[id];
            }

            // TODO: Make optional?
            throw new SerializationException(
                      $"Recursion detected while building entity '{parameter.Instance}' of type '{parameter.DeclaredType}'.");
        }
    }
}