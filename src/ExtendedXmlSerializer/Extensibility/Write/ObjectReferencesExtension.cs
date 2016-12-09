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
using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Plans.Write;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Services.Write;
using WriteState = ExtendedXmlSerialization.Services.Write.WriteState;

namespace ExtendedXmlSerialization.Extensibility.Write
{
    public class ObjectReferencesExtension : WritingExtensionBase
    {
        private readonly WeakCache<IWriting, Context> _contexts = new WeakCache<IWriting, Context>(_ => new Context());
        private readonly ISerializationToolsFactory _factory;
        private readonly IInstruction _instruction;

        public ObjectReferencesExtension(ISerializationToolsFactory factory, IInstruction instruction)
        {
            _factory = factory;
            _instruction = instruction;
        }

        public override bool Starting(IWriting services)
        {
            switch (services.Current.State)
            {
                case WriteState.Instance:
                    var set = _contexts.Get(services).Elements;
                    foreach (var item in Arrays.Default.AsArray(services.Current.Instance))
                    {
                        if (!set.Contains(item))
                        {
                            set.Add(item);
                        }
                    }
                    break;
                case WriteState.Members:
                    var instance = services.Current.Instance;
                    var configuration = _factory.GetConfiguration(instance.GetType());
                    if (configuration?.IsObjectReference ?? false)
                    {
                        var context = _contexts.Get(services);
                        var elements = context.Elements;
                        var references = context.References;
                        var objectId = configuration.GetObjectId(instance);
                        var contains = references.Contains(instance);
                        var reference = contains || (services.GetArrayContext() == null && elements.Contains(instance));
                        var @namespace = services.Get(this);
                        var property = reference
                            ? (IProperty) new ObjectReferenceProperty(@namespace, objectId)
                            : new ObjectIdProperty(@namespace, objectId);
                        var result = !reference;
                        if (result)
                        {
                            services.Attach(property);
                            references.Add(instance);
                        }
                        else
                        {
                            // TODO: Find a more elegant way to handle this:
                            if (DefaultEmitTypeSpecification.Default.IsSatisfiedBy(services))
                            {
                                _instruction.Execute(services);
                            }
                            services.Emit(property);
                        }
                        return result;
                    }
                    break;
            }
            return true;
        }

        public override void Finished(IWriting services)
        {
            switch (services.Current.State)
            {
                case WriteState.Instance:
                    var instance = services.Current.Instance;
                    if (Arrays.Default.Is(instance))
                    {
                        _contexts.Get(services).Elements.Clear();
                    }
                   break;
            }
            base.Finished(services);
        }

        class Context
        {
            public Context() : this(new HashSet<object>(), new HashSet<object>()) {}

            public Context(ISet<object> references, ISet<object> elements)
            {
                References = references;
                Elements = elements;
            }

            public ISet<object> References { get; }
            public ISet<object> Elements { get; }
        }
    }
}