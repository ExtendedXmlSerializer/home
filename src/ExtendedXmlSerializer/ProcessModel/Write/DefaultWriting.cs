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
using System.Collections.Immutable;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Sources;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    /*public class AlteringWriting : IWriting
    {
        private readonly IAlteration<WriteContext> _alteration;

        public AlteringWriting(IAlteration<WriteContext> alteration)
        {
            _alteration = alteration;
        }

        // protected override IDisposable New(WriteContext context) => base.New(_alteration.Get(context));
        public void Dispose() {}

        public object GetService(Type serviceType)
        {
            return null;
        }

        public IWriteContext Current
        {
            get { return null; }
        }

        public IDisposable New(object instance, IElementInformation information)
        {
            return null;
        }

        public IDisposable New(IImmutableList<MemberContext> members)
        {
            return null;
        }

        public IDisposable New(MemberContext member)
        {
            return null;
        }

        public void Emit(object instance) {}
        public void Emit(IProperty property) {}

        public Uri Locate(object parameter)
        {
            return null;
        }
    }*/

    public class Serialization : CompositeServiceProvider, ISerialization
    {
        private readonly IWriter _writer;
        private readonly INamespaceLocator _locator;
        private readonly IDisposable _undo;
        private readonly IObjectSerializer _serializer;
        private readonly INamespaceEmitter _emitter;

        public Serialization(IWriter writer, INamespaceLocator locator, IObjectSerializer serializer,
                       INamespaceEmitter emitter, params object[] services) : base(services)
        {
            _writer = writer;
            _locator = locator;
            _serializer = serializer;
            _emitter = emitter;
            _undo = new DelegatedDisposable(Undo);
        }

        public IWriteContext Current { get; private set; }

        protected virtual IDisposable New(IWriteContext context)
        {
            Current = context;
            // _chain.Push(Current = context);
            return _undo;
        }

        void Undo()
        {
            switch (Current?.State)
            {
                case ProcessState.Instance:
                    _writer.EndElement();
                    break;
            }
            Current = Current?.Parent;
            // Current = _chain.Pop();
        }

        /*public IDisposable Start(object root)
        {
            if (_chain.Any())
            {
                throw new InvalidOperationException(
                          "A request to start a new writing context was made, but it has already started.");
            }
            return
                New(new WriteContext(ProcessState.Root, root, null, TypeDefinitionCache.GetDefinition(root.GetType()),
                                     null, null));
        }*/

        /*public IDisposable New(object instance)
        {
            var previous = _chain.Peek();
            var result =
                New(new WriteContext(ProcessState.Instance, previous.Root, instance,
                                     TypeDefinitionCache.GetDefinition(instance.GetType()), null, null));
            return result;
        }*/

        public IDisposable New(object instance, IElementInformation information)
        {
            var definition = TypeDefinitionCache.GetDefinition(instance.GetType());
            var root = Current == null;
            var context = root
                ? new WriteContext(instance, definition)
                : new WriteContext(Current, instance, definition);

            var current = Current ?? context;
            var type = information.GetType(current);
            var identifier = type != null ? _locator.Locate(type) : null;
            _writer.Begin(information.GetName(current), identifier);

            if (root && identifier != null)
            {
                _emitter.Execute(instance);
            }

            var result = New(context);
            return result;
        }

        public IDisposable New(IImmutableList<MemberContext> members)
        {
            var result = New(new WriteContext(Current, members));
            return result;
        }

        public IDisposable New(MemberContext member)
        {
            var context = new WriteContext(Current, member);
            var result = New(context);
            return result;
        }

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

        /*public IEnumerable<WriteContext> Hierarchy
        {
            get
            {
                foreach (var context in _chain)
                {
                    yield return context;
                }
            }
        }*/

        /*public void Dispose() {}
        public object GetService(Type serviceType)
        {
            return null;
        }*/
        public Uri Locate(object parameter) => _locator.Locate(parameter);

        public void Execute(IInstruction parameter) => parameter.Execute(this);
    }
}