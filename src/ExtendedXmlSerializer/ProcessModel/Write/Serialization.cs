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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Sources;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public interface IEmitter : ICommand<object> {}

    public class Emitter : IEmitter
    {
        private readonly IWriter _writer;
        private readonly IObjectSerializer _serializer;
        private readonly INamespaceLocator _locator;

        public Emitter(IWriter writer, IObjectSerializer serializer, INamespaceLocator locator)
        {
            _writer = writer;
            _serializer = serializer;
            _locator = locator;
        }
        public void Execute(object parameter) {}

public void Emit(object instance) => _writer.Emit(_serializer.Serialize(instance));

        public void Emit(IProperty property)
        {
            var type = property.Value as Type;
            if (property.Identifier != null && type != null)
            {
                var identifier = _locator.Locate(type)?.ToString();
                if (identifier != null)
                {
                    var name = TypeDefinitionCache.GetDefinition(type).Name;
                    _writer.Emit(property.Name, property.Identifier, name, identifier);
                    return;
                }
            }
            var serialized = _serializer.Serialize(property.Value);
            _writer.Emit(property.Name, serialized, property.Identifier, property.Prefix);
        }

        public void Emit(Type type) {}
    }

    public class Serialization : CompositeServiceProvider, ISerialization, IContextMonitor, IScopeFactory, IEmitter
    {
        private readonly IContextMonitor _contextMonitor;
        private readonly IScopeFactory _factory;
        private readonly IEmitter _emitter;

        /*public Serialization(IWriter writer, INamespaceEmitter emitter, params object[] services)
            : this(writer, DefaultNamespaceLocator.Default, ObjectSerializer.Default, emitter, new SerializationContext(), services)
        {}*/

        public Serialization(
            IContextMonitor contextMonitor, 
            IScopeFactory factory, 
            IEmitter emitter,
            params object[] services)
            : base(services)
        {
            _contextMonitor = contextMonitor;
            _factory = factory;
            _emitter = emitter;
        }

        public void Execute(IInstruction parameter) => parameter.Execute(this);
       
        void ICommand<object>.Execute(object parameter) => _emitter.Execute(parameter);

        IScope IScopeFactory.Create(object instance) => _factory.Create(instance);

        IContext IWriteContextAware.Current => _contextMonitor.Current;
        IEnumerator<IContext> IEnumerable<IContext>.GetEnumerator() => _contextMonitor.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _contextMonitor.GetEnumerator();
        void IContextMonitor.MakeCurrent(IContext context) => _contextMonitor.MakeCurrent(context);
        void IContextMonitor.Undo() => _contextMonitor.Undo();
    }
}