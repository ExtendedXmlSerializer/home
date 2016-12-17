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
using System.IO;
using ExtendedXmlSerialization.NodeModel.Write;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public interface ISerializer
    {
        void Serialize(Stream stream, object instance);
    }

    /*class Selector : FirstConditionalCommand<IObjectNode>
    {
        public Selector(IWriter writer) : base(
            new RootEmitter(writer)
        ) {}
    }*/

    /*class SelectorFactory : IParameterizedSource<IWriter, ICommand<IObjectNode>>
    {
        public static SelectorFactory Default { get; } = new SelectorFactory();
        SelectorFactory() {}

        public ICommand<IObjectNode> Get(IWriter parameter)
        {
            var collection = new List<IConditionalCommand<IObjectNode>>(8);
            var result = new FirstConditionalCommand<IObjectNode>(collection);

            collection.Add(new PrimitiveEmitter(parameter));
            collection.Add(new PropertyEmitter(parameter));
            collection.Add(new ContentEmitter(parameter, result));
            collection.Add(new MembersEmitter(result));
            collection.Add(new InstanceEmitter(parameter, result));
            collection.Add(new RootEmitter(result));
            return result;
        }
    }*/

    /*class Selector : INodeEmitter
    {
        readonly private INodeEmitter _primitive, _property, _content, _members, _instance, _root;

        public Selector(IWriter writer)
        {
            _primitive = new PrimitiveEmitter(writer);
            _property = new PropertyEmitter(writer);
            _content = new ContentEmitter(writer, this);
            _members = new MembersEmitter(this);
            _instance = new InstanceEmitter(writer, this);
            _root = new RootEmitter(this);
        }

        INodeEmitter Select(IObjectNode parameter)
        {
            if (parameter is IPrimitive)
            {
                return _primitive;
            }
            if (parameter is IProperty)
            {
                return _property;
            }
            if (parameter is IContent)
            {
                return _content;
            }
            if (parameter is IMembers)
            {
                return _members;
            }
            if (parameter is IInstance)
            {
                return _instance;
            }
            if (parameter is IRoot)
            {
                return _root;
            }
            throw new InvalidOperationException($"Could not find emmiter for {parameter.GetType()}");
        }

        public void Execute(IObjectNode parameter) => Select(parameter).Execute(parameter);
    }

    class PrimitiveEmitter : NodeEmitterBase<IPrimitive>
    {
        private readonly IWriter _writer;

        public PrimitiveEmitter(IWriter writer)
        {
            _writer = writer;
        }

        protected override void Execute(IPrimitive parameter) => _writer.Emit(parameter);
    }

    class PropertyEmitter : NodeEmitterBase<IProperty>
    {
        private readonly IWriter _writer;

        public PropertyEmitter(IWriter writer)
        {
            _writer = writer;
        }

        protected override void Execute(IProperty parameter) => _writer.Emit(parameter.Instance);
    }

    class ContentEmitter : NodeEmitterBase<IContent>
    {
        private readonly IWriter _writer;
        private readonly ICommand<IObjectNode> _emit;

        public ContentEmitter(IWriter writer, ICommand<IObjectNode> emit)
        {
            _writer = writer;
            _emit = emit;
        }

        protected override void Execute(IContent parameter)
        {
            using (_writer.Begin(parameter))
            {
                _emit.Execute(parameter.Instance);
            }
        }
    }

    class MembersEmitter : NodeEmitterBase<IMembers>
    {
        private readonly ICommand<IObjectNode> _emit;

        public MembersEmitter(ICommand<IObjectNode> emit)
        {
            _emit = emit;
        }

        protected override void Execute(IMembers parameter)
        {
            foreach (var member in parameter)
            {
                _emit.Execute(member);
            }
        }
    }


    class RootEmitter : NodeEmitterBase<IRoot>
    {
        private readonly ICommand<IObjectNode> _emit;

        public RootEmitter(ICommand<IObjectNode> emit)
        {
            _emit = emit;
        }

        protected override void Execute(IRoot parameter) => _emit.Execute(parameter.Instance);
    }

    class InstanceEmitter : NodeEmitterBase<IInstance>
    {
        private readonly IWriter _writer;
        private readonly ICommand<IObjectNode> _emit;

        public InstanceEmitter(IWriter writer, ICommand<IObjectNode> emit)
        {
            _writer = writer;
            _emit = emit;
        }

        protected override void Execute(IInstance parameter)
        {
            using (_writer.Begin(parameter))
            {
                _emit.Execute(parameter.Members);
            }
        }
    }

    abstract class NodeEmitterBase<T> : ConditionalCommandBase<IObjectNode>, INodeEmitter where T : IObjectNode
    {
        public override void Execute(IObjectNode parameter) => Execute(parameter.AsValid<T>());
        protected abstract void Execute(T parameter);

        public override bool IsSatisfiedBy(IObjectNode parameter) => parameter is T;
    }*/

    public interface IEmitter : ICommand<IObjectNode> {}

    class DefaultEmitter : IEmitter
    {
        private readonly IWriter _writer;
        public DefaultEmitter(IWriter writer)
        {
            _writer = writer;
        }

        public void Execute(IObjectNode parameter)
        {
            var reference = parameter as IReference;
            if (reference != null)
            {
                return;
            }

            var instance = parameter as IInstance;
            if (instance != null)
            {
                using (_writer.Begin(instance))
                {
                    if (instance is Instance)
                    {
                        _writer.Emit(new TypeProperty(instance.Instance.GetType()));
                    }
                    
                    foreach (var member in instance.Members)
                    {
                        var primitive = member as IPrimitive;
                        if (primitive != null)
                        {
                            using (_writer.Begin(primitive))
                            {
                                _writer.Emit(primitive.Instance);
                            }
                        }
                        else
                        {
                            Execute(member);
                        }
                    }
                }
            }
            else
            {
                var decorated = parameter as IObjectNode<IObjectNode>;
                if (decorated != null)
                {
                    Execute(decorated.Instance);
                }
            }
        }
    }
}