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
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization.Extensibility.Write
{
/*
    public class ObjectReferencesExtension : WritingExtensionBase
    {
        private readonly WeakCache<ISerialization, Context> _contexts = new WeakCache<ISerialization, Context>(_ => new Context());
        private readonly IInstruction _instruction;

        public ObjectReferencesExtension(IInstruction instruction)
        {
            _instruction = instruction;
        }

        public override void Accept(IExtensionRegistry registry)
        {
            registry.RegisterSpecification(ProcessState.Members, this);
            registry.RegisterSpecification(ProcessState.Instance, this);
            registry.Register(ProcessState.Instance, this);
        }

        public override bool IsSatisfiedBy(ISerialization services)
        {
            switch (services.Current.State)
            {
                case ProcessState.Instance:
                    var set = _contexts.Get(services).Elements;
                    foreach (var item in Arrays.Default.AsArray(services.Current.Instance))
                    {
                        if (!set.Contains(item))
                        {
                            set.Add(item);
                        }
                    }
                    break;
                case ProcessState.Members:
                    var instance = services.Current.Instance;
                    var configuration =
                        services.GetValid<ISerializationToolsFactory>().GetConfiguration(instance.GetType());
                    if (configuration?.IsObjectReference ?? false)
                    {
                        var context = _contexts.Get(services);
                        var elements = context.Elements;
                        var references = context.References;
                        var objectId = configuration.GetObjectId(instance);
                        var contains = references.Contains(instance);
                        var reference = contains || (services.Current.GetArrayContext() == null && elements.Contains(instance));
                        var @namespace = services.Locate(this);
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
                            _instruction.Execute(services);
                            services.Emit(property);
                        }
                        return result;
                    }
                    break;
            }

            return true;
        }

        public override void Completed(ISerialization services)
        {
            var instance = services.Current.Instance;
            if (Arrays.Default.Is(instance))
            {
                _contexts.Get(services).Elements.Clear();
            }
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
*/
}