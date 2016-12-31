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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using ExtendedXmlSerialization.Configuration.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
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
            var root = new Root(new FixedSource(6776).Get());
            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var emitter = new Emitter(writer);
            root.Execute(emitter);
            writer.Flush();

            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><int>6776</int>", builder.ToString());
        }

        [Fact]
        public void InstanceSerialization()
        {
            var instance = new InstanceClass {PropertyName = "Hello World!"};
            var root = new Root(new FixedSource(instance).Get());
            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var emitter = new Emitter(writer);
            root.Execute(emitter);
            writer.Flush();

            var actual = builder.ToString();
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><InstanceClass><PropertyName>Hello World!</PropertyName></InstanceClass>", actual);
        }

        class InstanceClass
        {
            public string PropertyName { get; set; }
        }

        public interface IContentSource
        {
            IContent Get();
        }

        public class FixedSource : IContentSource
        {
            private readonly object _source;
            private readonly Type _instanceType;

            public FixedSource(object source) : this(source, source.GetType()) {}

            public FixedSource(object source, Type instanceType)
            {
                _source = source;
                _instanceType = instanceType;
            }


            public IContent Get()
            {
                if (Primitives.Default.ContainsKey(_instanceType))
                {
                    return new Primitive(Primitives.Default[_instanceType], _source);
                }

                var members =
                    SelectMembers().Select(x => new Member(new FixedSource(x.GetValue(_source)), x));
                var instance = new Instance(TypeDefinitions.Default.Get(_instanceType).Name, members);
                return instance;
            }

            private ImmutableArray<IMemberDefinition> SelectMembers()
            {
                return TypeDefinitions.Default.Get(_instanceType)
                                      .Members;
            }
        }

        public interface IContext : ICommand<IEmitter> {}

        public interface IRoot : IContext {}

        class Root : IRoot
        {
            private readonly IContent _content;

            public Root(IContent content)
            {
                _content = content;
            }

            public void Execute(IEmitter parameter)
            {
                parameter.Start(this);
                _content.Execute(parameter);
                parameter.End(this);
            }

            public override string ToString()
            {
                return _content.ToString();
            }
        }

        public interface IEmitter
        {
            void Start(IContext context);
            void Emit(IContent content);
            void End(IContext context);
            // void Emit(IContext context, IContent content);
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

            public void Start(IContext context)
            {
                _writer.WriteStartElement(_serializer.Serialize(context));
            }

            public void Emit(IContent content) => _writer.WriteString(_serializer.Serialize(content));

            public void End(IContext context) => _writer.WriteEndElement();
        }

        public interface IContent : ICommand<IEmitter> {}

        public interface IInstance : IContent {}

        class Instance : IInstance
        {
            private readonly string _name;
            private readonly IEnumerable<IMember> _members;

            public Instance(string name, IEnumerable<IMember> members)
            {
                _name = name;
                _members = members;
            }

            public void Execute(IEmitter parameter)
            {
                foreach (var member in _members)
                {
                    member.Execute(parameter);
                }
            }

            public override string ToString() => _name;
        }

        public interface IMember : IContext
        {
            // bool IsWritable { get; }
        }

        class Member : IMember, ISerializable
        {
            private readonly IContentSource _source;
            private readonly IMemberDefinition _definition;

            public Member(IContentSource source, IMemberDefinition definition)
            {
                _source = source;
                _definition = definition;
            }

            public void Execute(IEmitter parameter)
            {
                parameter.Start(this);
                var content = _source.Get();
                content.Execute(parameter);
                parameter.End(this);
            }

            public string Get(IObjectSerializer parameter) => _definition.Name;
        }

        abstract class ContentBase : IContent
        {
            public abstract void Execute(IEmitter parameter);
        }

        public interface IPrimitive : IContent {}

        class Primitive : ContentBase, IPrimitive, ISerializable
        {
            private readonly string _name;
            private readonly object _instance;

            public Primitive(string name, object instance)
            {
                _name = name;
                _instance = instance;
            }

            public override void Execute(IEmitter parameter) => parameter.Emit(this);
            public string Get(IObjectSerializer parameter) => parameter.Serialize(_instance);

            public override string ToString() => _name;
        }


        /*
                [Fact]
                public void Basic()
                {
                    / * var selector = new ContentSelector(PrimitiveContentSelector.Default);
                    var emitter = new Emitter(selector);
        
                    var root = new Root(new FixedSource(6776));
                    emitter.Execute(root); * /
                    // var serialization = new Serialization(new RootBuilder(selector), new TemplatedEmitter());
                }
                       
                       
                class Emitter : IEmitter
                {
                    private readonly IContentSelector _selector;
                    private readonly IWriter _writer;
        
                    public Emitter(IContentSelector selector, IWriter writer)
                    {
                        _selector = selector;
                        _writer = writer;
                    }
        
                    public void Execute(IContext parameter)
                    {
                        var content = _selector.Get(parameter);
        
                        content.Execute(_writer);
        
                        /*var primitive = content as IPrimitive;
                        if (primitive != null)
                        {
                              
                        }* /
                    }
                }
        }*/
    }
}