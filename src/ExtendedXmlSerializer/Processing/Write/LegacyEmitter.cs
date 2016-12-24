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
    sealed class LegacyEmitter : IEmitter
    {
        private readonly IWriter _writer;
        private readonly IContextMonitor _monitor;
        private readonly IIdentityLocator _locator;
        private readonly IVersionLocator _version;

        readonly ISet<object> _scanned = new HashSet<object>();

        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public LegacyEmitter(IWriter writer, IContextMonitor monitor, IIdentityLocator locator, IVersionLocator version)
        {
            _writer = writer;
            _monitor = monitor;
            _locator = locator;
            _version = version;
        }

        public void Execute(IElement parameter)
        {
            _monitor.Update(parameter);

            var entity = parameter.Content;
            var primitive = entity as IPrimitive;
            if (primitive != null)
            {
                using (_writer.New(parameter))
                {
                    ApplyType(parameter);
                    _writer.Emit(primitive.Instance);
                }
                return;
            }

            var @object = entity as IObject;
            if (@object != null)
            {
                using (_writer.New(parameter))
                {
                    var instance = @object.Instance;
                    var identity = _scanned.Contains(instance)
                        ? parameter is IItem
                        : _generator.For(instance).FirstEncounter;
                    var id = _locator.Get(instance);
                    ApplyType(parameter, id != null && identity);

                    var version = _version.Get(parameter.Content.Type);
                    if (version != null && version.Value > 0)
                    {
                        _writer.Emit(new VersionProperty(version.Value));
                    }

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

            var composite = entity as Elements;
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

        private static bool ShouldApply(IElement element, bool? identity)
        {
            var entity = element.Content;
            if (element is IRoot)
            {
                return !(entity is IEnumerableObject) && !(entity is IPrimitive);
            }

            var member = element as IMember;
            if (member != null)
            {
                return identity.GetValueOrDefault() ||
                       (member.IsWritable && entity.Type != member.DefinedType);
            }

            return identity.GetValueOrDefault() || element.DefinedType != entity.Type;
        }

        private void ApplyType(IElement element, bool? tag = null)
        {
            if (ShouldApply(element, tag))
            {
                _writer.Emit(new TypeProperty(element.Content.Type));
            }
        }
    }
}