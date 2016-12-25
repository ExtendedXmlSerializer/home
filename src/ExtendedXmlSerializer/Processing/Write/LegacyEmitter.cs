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
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    public interface ITemplate : ISpecification<IElement>
    {
        void Render(IEmitter emitter, IWriter writer, IElement element);
    }

    sealed class PrimitiveTemplate : TemplateBase<IPrimitive>
    {
        public static PrimitiveTemplate Default { get; } = new PrimitiveTemplate();
        PrimitiveTemplate() {}

        protected override void Render(IEmitter emitter, IWriter writer, IElement element, IPrimitive content)
            => writer.Emit(content.Instance);
    }

    class EnumerableObjectTemplate : ObjectTemplateBase<IEnumerableObject>
    {
        private readonly IIdentities _identities;

        public EnumerableObjectTemplate(IIdentities identities, IVersionLocator version)
            : base(identities, version)
        {
            _identities = identities;
        }

        protected override void Render(IEmitter emitter, IEnumerableObject content)
        {
            base.Render(emitter, content);

            foreach (var item in content.Instance)
            {
                _identities.Track(item);
            }

            foreach (var item in content.Items)
            {
                emitter.Execute(item);
            }

            foreach (var item in content.Instance)
            {
                _identities.Release(item);
            }
        }
    }

    public interface IIdentities : IParameterizedSource<IElement, IProperty>
    {
        void Track(object instance);
        void Release(object instance);
    }

    class Properties : WeakCacheBase<IElement, IProperty>, IIdentities
    {
        private readonly IIdentityLocator _locator;
        readonly ISet<object> _watching = new HashSet<object>();

        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public Properties(IIdentityLocator locator)
        {
            _locator = locator;
        }

        protected override IProperty Callback(IElement key)
        {
            var instance = key.Content.Instance;
            var id = _locator.Get(instance);
            if (id != null)
            {
                var identity = _watching.Contains(instance) ? key is IItem : _generator.For(instance).FirstEncounter;
                var result = identity ? (IProperty) new IdentityProperty(id) : new ObjectReferenceProperty(id);
                return result;
            }
            return null;
        }

        public void Track(object instance)
        {
            if (!_generator.Contains(instance))
            {
                _watching.Add(instance);
            }
        }

        public void Release(object instance) => _watching.Remove(instance);
    }

    class ObjectTemplate : ObjectTemplateBase<IObject>
    {
        public ObjectTemplate(IIdentities identities, IVersionLocator version) : base(identities, version) {}
    }

    abstract class ObjectTemplateBase<T> : TemplateBase<T> where T : IObject
    {
        private readonly IIdentities _identities;
        private readonly IVersionLocator _version;

        protected ObjectTemplateBase(IIdentities identities, IVersionLocator version)
        {
            _identities = identities;
            _version = version;
        }

        protected override bool EmitType(IElement element) =>
            _identities.Get(element) is IdentityProperty || base.EmitType(element);

        protected override void Render(IEmitter emitter, IWriter writer, IElement element, T content)
        {
            var version = _version.Get(content.Type);
            if (version > 0)
            {
                writer.Emit(new VersionProperty(version.Value));
            }

            var property = _identities.Get(element);
            if (property != null)
            {
                writer.Emit(property);
                if (property is ObjectReferenceProperty)
                {
                    return;
                }
            }

            Render(emitter, content);
        }

        protected virtual void Render(IEmitter emitter, T content)
        {
            foreach (var o in content.Members)
            {
                emitter.Execute(o);
            }
        }
    }

    abstract class TemplateBase<T> : ITemplate where T : IInstance
    {
        public void Render(IEmitter emitter, IWriter writer, IElement element)
        {
            var content = (T) element.Content;

            using (writer.New(element))
            {
                if (EmitType(element))
                {
                    writer.Emit(new TypeProperty(element.Content.Type));
                }

                Render(emitter, writer, element, content);
            }
        }

        protected abstract void Render(IEmitter emitter, IWriter writer, IElement element, T content);

        protected virtual bool EmitType(IElement element)
        {
            var entity = element.Content;
            if (element is IRoot)
            {
                return !(entity is IEnumerableObject) && !(entity is IPrimitive);
            }

            if (element.DefinedType != entity.Type)
            {
                return (element as IMember)?.IsWritable ?? true;
            }

            return false;
        }

        public bool IsSatisfiedBy(IElement parameter) => parameter.Content is T;
    }

    class ElementsTemplate : TemplateBase<Elements>
    {
        public static ElementsTemplate Default { get; } = new ElementsTemplate();
        ElementsTemplate() {}

        protected override bool EmitType(IElement element) => false;

        protected override void Render(IEmitter emitter, IWriter writer, IElement element, Elements content)
        {
            foreach (var context in content)
            {
                emitter.Execute(context);
            }
        }
    }

    sealed class LegacyTemplatedEmitter : TemplatedEmitter
    {
        private readonly IContextMonitor _monitor;

        public LegacyTemplatedEmitter(IWriter writer, IContextMonitor monitor, IIdentities identities, IVersionLocator locator) : base(writer,
            PrimitiveTemplate.Default,
            new EnumerableObjectTemplate(identities, locator), 
            new ObjectTemplate(identities, locator),
            ElementsTemplate.Default
        )
        {
            _monitor = monitor;
        }

        protected override void Render(IElement parameter, ITemplate template)
        {
            _monitor.Update(parameter);
            base.Render(parameter, template);
        }
    }

    public class TemplatedEmitter : IEmitter
    {
        private readonly IWriter _writer;
        private readonly ITemplate[] _templates;

        public TemplatedEmitter(IWriter writer, params ITemplate[] templates)
        {
            _writer = writer;
            _templates = templates;
        }

        public void Execute(IElement parameter)
        {
            foreach (var template in _templates)
            {
                if (template.IsSatisfiedBy(parameter))
                {
                    Render(parameter, template);
                    return;
                }
            }
            throw new SerializationException($"Could not find a template for element '{parameter}' with a defined type of '{parameter.DefinedType}'.");
        }

        protected virtual void Render(IElement parameter, ITemplate template) => template.Render(this, _writer, parameter);
    }
}