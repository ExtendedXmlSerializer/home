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
using System.Xml.Linq;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Model;

namespace ExtendedXmlSerialization.Processing.Read
{
    /*class InstanceWriter : IWriter
    {
        public IDisposable New(IElement element)
        {
            return null;
        }


        public void Emit(IElement element) {}
        public void Emit(object instance) {}
        public void Dispose() {}
    }*/

    /*public class RootBuilder : IRootBuilder
    {
        private readonly IEntitySelector _builder;

        public RootBuilder(IEntitySelector builder)
        {
            _builder = builder;
        }

        public IElement Get(object parameter)
        {
            /*var descriptor = new InstanceDescriptor(parameter);
            var body = _builder.Get(descriptor);
            var result = new Root(body, descriptor.Name);#1#
            return null;
        }
    }*/

    public interface IInstanceBuilder : IParameterizedSource<XElement, object> {}
    class InstanceBuilder : IInstanceBuilder
    {
        private readonly IInstanceSelector _selector;
        private readonly ITypeDefinitionLocator _locator;

        public InstanceBuilder(IInstanceSelector selector, ITypeDefinitionLocator locator)
        {
            _selector = selector;
            _locator = locator;
        }

        public object Get(XElement parameter)
        {
            var definition = _locator.Get(parameter);

            return null;
        }
    }

    public interface IInstanceSelector : IParameterizedSource<Descriptor, object> {}

    public interface ITypeDefinitionLocator : IParameterizedSource<XElement, ITypeDefinition> {}
    class TypeDefinitionLocator : ITypeDefinitionLocator
    {
        public static TypeDefinitionLocator Default { get; } = new TypeDefinitionLocator();

        private readonly ITypeParser _parser;
        private readonly ITypeDefinitions _definitions;
        private readonly Type _context;

        public TypeDefinitionLocator(Type context = null) : this(Types.Default, TypeDefinitions.Default, context) {}

        public TypeDefinitionLocator(ITypeParser parser, ITypeDefinitions definitions, Type context = null)
        {
            _parser = parser;
            _definitions = definitions;
            _context = context;
        }

        public ITypeDefinition Get(XElement parameter)
        {
            var value = parameter.Attribute(ExtendedXmlSerializer.Type)?.Value;
            var type = value != null ? _parser.Get(value) : _context;
            var result = type != null ? _definitions.Get(type) : null;
            if (result == null)
            {
                throw new SerializationException($"Could not find TypeDefinition from provided value: {value}");
            }
            return result;
        }
    }

    public struct Descriptor
    {
        public Descriptor(ITypeDefinition definition) : this(definition, definition.Activate()) {}

        public Descriptor(ITypeDefinition definition, object instance)
        {
            Definition = definition;
            Instance = instance;
        }

        public ITypeDefinition Definition { get; }
        public object Instance { get; }
    }


    /*class Emitter : IProcessor
    {
        public void Execute(IContext parameter) {}
    }*/
}