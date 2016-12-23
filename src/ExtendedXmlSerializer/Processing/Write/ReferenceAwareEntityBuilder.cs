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
        readonly private IDictionary<object, IReference> _references = new Dictionary<object, IReference>();
        readonly ISet<object> _scanned = new HashSet<object>();

        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public ReferenceAwareEntityBuilder(IEntityBuilder builder, IIdentityLocator locator)
        {
            _builder = builder;
            _locator = locator;
        }

        public IEntity Get(ContextDescriptor parameter)
        {
            // Scan ahead:
            var instance = parameter.Instance;
            var scanned = Arrays.Default.Is(instance) ? Arrays.Default.AsArray(instance) : null;
            if (scanned != null)
            {
                foreach (var item in scanned)
                {
                    var identity = _locator.Get(item);
                    if (identity != null)
                    {
                        _references.Add(item, new Reference(identity, item.GetType()));
                        _scanned.Add(item);
                    }
                }
            }

            try
            {
                var entity = For(parameter);
                if (entity != null)
                {
                    return entity;
                }

                if (_references.ContainsKey(instance))
                {
                    return _references[instance];
                }
            }
            finally
            {
                if (scanned != null)
                {
                    foreach (var item in scanned)
                    {
                        _scanned.Remove(item);
                    }
                }
            }

            // TODO: Make optional?
            throw new SerializationException(
                      $"Recursion detected while building entity '{instance}' of type '{parameter.DeclaredType}'.");
        }

        private IEntity For(ContextDescriptor descriptor)
        {
            var key = descriptor.Instance;
            var context = _generator.For(key);
            if (context.FirstEncounter)
            {
                var result = _builder.Get(descriptor);
                var o = result as IObject;
                if (o != null)
                {
                    var identity = _locator.Get(context);
                    if (identity != null)
                    {
                        var unique = new UniqueObject(identity, o);
                        _references.Add(key, new Reference(unique));
                        return unique;
                    }
                }
                return result;
            }
            return null;
        }
    }
}