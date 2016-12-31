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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml;
using ExtendedXmlSerialization.Configuration.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Model;
using ExtendedXmlSerialization.Processing;
using ExtendedXmlSerialization.Processing.Write;
using Xunit;

namespace ExtendedXmlSerialization.Test
{
    public class ContextsTests
    {
        [Fact]
        public void PrimitiveSerialization()
        {
            var root = new Root(new ProvidedContentFactory(6776).Get());
            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var emitter = new Emitter(writer);
            root.Execute(emitter);
            writer.Flush();

            var actual = builder.ToString();
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><int>6776</int>", actual);
        }

        [Fact]
        public void InstanceSerialization()
        {
            var instance = new InstanceClass {PropertyName = "Hello World!"};
            var root = new Root(new ProvidedContentFactory(instance).Get());
            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var emitter = new Emitter(writer);
            root.Execute(emitter);
            writer.Flush();

            var actual = builder.ToString();
            Assert.Equal(
                @"<?xml version=""1.0"" encoding=""utf-16""?><InstanceClass><PropertyName>Hello World!</PropertyName></InstanceClass>",
                actual);
        }

        class InstanceClass
        {
            public string PropertyName { get; set; }
        }

        public interface IContentFactory
        {
            IContext Get();
        }

        public class ProvidedContentFactory : IContentFactory
        {
            private readonly object _source;
            private readonly Type _instanceType;

            public ProvidedContentFactory(object source) : this(source, source.GetType()) {}

            public ProvidedContentFactory(object source, Type instanceType)
            {
                _source = source;
                _instanceType = instanceType;
            }

            public IContext Get()
            {
                if (Primitives.Default.ContainsKey(_instanceType))
                {
                    return new Primitive(Primitives.Default[_instanceType], _source);
                }

                var members =
                    SelectMembers().Select(x => new Member(x.Name, new ProvidedContentFactory(x.GetValue(_source)).Get()));
                var instance = new Instance(TypeDefinitions.Default.Get(_instanceType).Name, members);
                return instance;
            }

            private ImmutableArray<IMemberDefinition> SelectMembers()
            {
                return TypeDefinitions.Default.Get(_instanceType)
                                      .Members;
            }
        }

        public interface IRoot : IContext {}

        class Root : DecoratedContext, IRoot
        {
            public Root(IContext context) : base(context) {}
        }

        public interface IEmitter
        {
            void Start(IContext context);
            void Emit(IContext context);
            void End(IContext context);
        }

        class Emitter : IEmitter
        {
            private readonly XmlWriter _writer;
            private readonly IObjectSerializer _serializer;

            public Emitter(XmlWriter writer) : this(writer, ObjectSerializer.Default) {}

            public Emitter(XmlWriter writer, IObjectSerializer serializer)
            {
                _writer = writer;
                _serializer = serializer;
            }

            public void Start(IContext context) => _writer.WriteStartElement(_serializer.Serialize(context));

            public void Emit(IContext context) => _writer.WriteString(_serializer.Serialize(context));

            public void End(IContext context) => _writer.WriteEndElement();
        }

        public interface IContext : ICommand<IEmitter>
        {
            string Name { get; }
        }

        public interface IInstance : IContext {}

        class Instance : ContextBase, IInstance
        {
            private readonly IEnumerable<IMember> _members;

            public Instance(string name, IEnumerable<IMember> members) : base(name)
            {
                _members = members;
            }

            public override void Execute(IEmitter parameter)
            {
                foreach (var member in _members)
                {
                    member.Execute(parameter);
                }
            }
        }

        public interface IMember : IContext
        {
            // bool IsWritable { get; }
        }

        class Member : DecoratedContext, IMember
        {
            public Member(string name, IContext context) : base(name, context) {}
        }

        public class DecoratedContext : ContextBase
        {
            private readonly IContext _context;

            public DecoratedContext(IContext context) : this(context.Name, context) {}

            public DecoratedContext(string name, IContext context) : base(name)
            {
                _context = context;
            }

            public override void Execute(IEmitter parameter)
            {
                parameter.Start(this);
                _context.Execute(parameter);
                parameter.End(this);
            }
        }

        public abstract class ContextBase : IContext, ISerializable
        {
            protected ContextBase(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public abstract void Execute(IEmitter parameter);

            public virtual string Get(IObjectSerializer parameter) => Name;
        }

        public interface IPrimitive : IContext {}

        class Primitive : ContextBase, IPrimitive
        {
            private readonly object _instance;

            public Primitive(string name, object instance) : base(name)
            {
                _instance = instance;
            }

            public override void Execute(IEmitter parameter) => parameter.Emit(this);
            public override string Get(IObjectSerializer parameter) => parameter.Serialize(_instance);
        }
    }
}