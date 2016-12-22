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
    class LegacyEmitter : IEmitter
    {
        private readonly IWriter _writer;
        private readonly IContextMonitor _monitor;
        private readonly IIdentityLocator _locator;

        readonly ISet<object> _scanned = new HashSet<object>();

        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public LegacyEmitter(IWriter writer, IContextMonitor monitor, IIdentityLocator locator)
        {
            _writer = writer;
            _monitor = monitor;
            _locator = locator;
        }

        public void Execute(IContext parameter)
        {
            _monitor.Update(parameter);

            var entity = parameter.Entity;
            var primitive = entity as IPrimitive;
            if (primitive != null)
            {
                using (_writer.New(parameter))
                {
                    ApplyType(parameter);
                    _writer.Emit(primitive.Value);
                }
                return;
            }

            var @object = entity as IObject;
            if (@object != null)
            {
                var instance = @object.Instance;
                using (_writer.New(parameter))
                {
                    var identity = _scanned.Contains(instance) ? parameter is IItem : _generator.For(instance).FirstEncounter;
                    var id = _locator.Get(instance);
                    ApplyType(parameter, id != null && identity);

                    
                    if (id != null)
                    {
                        var property = identity ? (IProperty) new IdentityProperty(id) : new ObjectReferenceProperty(id);
                        _writer.Emit(property);
                        if (!identity)
                        {
                            return;
                        }
                    }

                    foreach (var o in @object.Members)
                    {
                        Execute(o);
                    }

                    var enumerable = @object as IEnumerableObject;
                    if (enumerable != null)
                    {
                        foreach (var item in enumerable.Instance)
                        {
                            if (!_generator.Contains(item))
                            {
                                _scanned.Add(item);
                            }
                        }

                        foreach (var item in enumerable.Items)
                        {
                            Execute(item);
                        }

                        foreach (var item in enumerable.Instance)
                        {
                            _scanned.Remove(item);
                        }
                    }
                }
                return;
            }

            var composite = entity as CompositeEntity;
            if (composite != null)
            {
                using (_writer.New(parameter))
                {
                    foreach (var context in composite)
                    {
                        Execute(context);
                    }
                }
                return;
            }
        }

        private static bool ShouldApply(IContext context, bool? identity)
        {
            var entity = context.Entity;
            if (context is IRoot)
            {
                return !(entity is IEnumerableObject) && !(entity is IPrimitive);
            }

            var member = context as IMember;
            if (member != null)
            {
                return identity.GetValueOrDefault() || (member.Definition.IsWritable && entity.Type != member.Definition.Type);
            }

            var item = context as ITypeAwareContext;
            if (item != null)
            {
                return identity.GetValueOrDefault() || item.ReferencedType != entity.Type;
            }

            return false;
        }

        private void ApplyType(IContext context, bool? tag = null)
        {
            if (ShouldApply(context, tag))
            {
                _writer.Emit(new TypeProperty(context.Entity.Type));
            }
        }
    }

    class DefaultEmitter : IEmitter
    {
        private readonly IWriter _writer;

        public DefaultEmitter(IWriter writer)
        {
            _writer = writer;
        }

        public void Execute(IContext parameter)
        {
            var primitive = parameter.Entity as IPrimitive;
            if (primitive != null)
            {
                using (_writer.New(parameter))
                {
                    ApplyType(parameter);
                    _writer.Emit(primitive.Value);
                }
                return;
            }

            var instance = parameter.Entity as IObject;
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

            var composite = parameter.Entity as CompositeEntity;
            if (composite != null)
            {
                using (_writer.New(parameter))
                {
                    foreach (var context in composite)
                    {
                        Execute(context);
                    }
                }
                return;
            }
        }

        private static bool ShouldApply(IContext context)
        {
            var entity = context.Entity;
            var enumerable = entity is IEnumerableObject;
            if (context is IRoot)
            {
                return !enumerable && !(entity is IPrimitive);
            }

            var apply = entity is IObject;
            if (apply && !enumerable)
            {
                return true;
            }

            var member = context as IMember;
            if (member != null)
            {
                return member.Definition.IsWritable && entity.Type != member.Definition.Type;
            }

            return false;
        }

        private void ApplyType(IContext context)
        {
            if (ShouldApply(context))
            {
                _writer.Emit(new TypeProperty(context.Entity.Type));
            }
        }
    }
}