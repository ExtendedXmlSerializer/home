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
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Instructions.Write;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    /*public class RootScope : InstanceScope<object>
    {
        public RootScope(IInstanceServices services, object instance) : base(services, null, instance) {}

        protected override void Start(IInstanceServices services, Uri identifier)
        {
            base.Start(services, identifier);
            if (identifier != null)
            {
                services.Execute(Instance);
            }
        }
    }

    public class MembersScope : ScopeBase<IImmutableList<IMemberDefinition>>
    {
        private readonly IInstruction _type /*= DefaultEmitTypeForInstanceInstruction.Default#1#;

        public MembersScope(IScopeFactory factory, IContext parent, IImmutableList<IMemberDefinition> instance,
                            ITypeDefinition definition)
            : this(factory, parent, instance, definition, EmitTypeInstruction.Default) {}

        public MembersScope(IScopeFactory factory, IContext parent, IImmutableList<IMemberDefinition> instance,
                            ITypeDefinition definition,
                            IInstruction type) : base(factory, parent, instance, definition)
        {
            _type = type;
        }

        public override void Execute(IProcess parameter) {}
    }

    public interface IInstanceScope : IScope {}

    /*public class InstanceScope : InstanceScope<object>, IInstanceScope
    {
        public InstanceScope(IInstanceServices services, IElementInformation information, IScope parent, object instance)
            : base(services, information, parent, instance) {}

        public InstanceScope(IInstanceServices services, IElementInformation information, IInstruction body,
                             IScope parent, object instance) : base(services, information, body, parent, instance) {}

        public InstanceScope(IInstanceServices services, IElementInformation information, IInstruction body,
                             IScope parent, object instance, ITypeDefinition definition)
            : base(services, information, body, parent, instance, definition) {}
    }#1#

    public class InstanceScope<T> : ScopeBase<T>, IInstanceScope
    {
        private readonly IElementInformation _information;
        private readonly IInstruction _body;
        readonly private IInstanceServices _services;

        public InstanceScope(IInstanceServices services, IScope parent, T instance)
            : this(services, ElementInformation.Default, parent, instance) {}

        public InstanceScope(
            IInstanceServices services,
            IElementInformation information,
            IScope parent, T instance)
            : this(services, information, EmitInstanceBodyInstruction.Default, parent, instance) {}

        protected InstanceScope(
            IInstanceServices services, IElementInformation information,
            IInstruction body,
            IScope parent, T instance)
            : this(services, information, body, parent, instance, TypeDefinitionCache.GetDefinition(instance.GetType())) {}

        protected InstanceScope(
            IInstanceServices services, IElementInformation information,
            IInstruction body,
            IScope parent, T instance, ITypeDefinition definition)
            : base(parent, instance, definition)
        {
            _services = services;
            _information = information;
            _body = body;
        }

        public override void Execute(IProcess services)
        {
            var type = _information.GetType(this);
            var identifier = type != null ? _services.Locate(type) : null;
            Start(_services, identifier);
            services.Execute(_body);
        }

        protected virtual void Start(IInstanceServices services, Uri identifier)
            => services.Begin(_information.GetName(this), identifier);

        protected override void OnDispose(bool disposing) => _services.EndElement();
    }

    public interface IMemberScope : IScope<IMemberDefinition> {}

    public class MemberScope : InstanceScope<IMemberDefinition>, IMemberScope
    {
        public MemberScope(IInstanceServices services, IInstruction body,
                              IScope parent, IMemberDefinition instance)
            : base(services, MemberElementInformation.Instance, body, parent, instance, instance.MemberType) {}

        /*private readonly IInstruction _body;

        public MemberScope(IScopeFactory factory, IContext parent, MemberContext instance, ITypeDefinition definition,
                           IInstruction body)
            : base(factory, parent, instance, definition)
        {
            _body = body;
        }

        public override void Execute(IProcess services) => services.Execute(_body);#1#
    }

    public class EnumerableScope : InstanceScope<IEnumerable>
    {

        // public EnumerableScope(IScope parent, IEnumerable instance) : base(factory, writer, locator, parent, instance) {}

        /*public EnumerableScope(IWriter writer, INamespaceLocator locator, IContext parent, IEnumerable instance)
            : base(writer, locator, parent, instance) {}

        public EnumerableScope(IWriter writer, INamespaceLocator locator, IInstruction body, IContext parent,
                               IEnumerable instance, ITypeDefinition definition, Func<IContext, Type> type,
                               Func<IContext, string> name)
            : base(writer, locator, body, parent, instance, definition, type, name) {}

        public override void Execute(IProcess parameter) => parameter.Execute(_body);#1#
        public EnumerableScope(IInstanceServices services, IScope parent, IEnumerable instance) : base(services, parent, instance) {}
    }*/
}