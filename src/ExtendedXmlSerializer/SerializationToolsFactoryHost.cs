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
using ExtendedXmlSerialization.Extensibility;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization
{
    public class SerializationToolsFactoryHost : ServiceRepository, ISerializationToolsFactoryHost
    {
        private readonly Func<IWritingContext> _context;

        public SerializationToolsFactoryHost(IImmutableList<object> services)
            : this(() => new DefaultWritingContext(), services) {}

        public SerializationToolsFactoryHost(Func<IWritingContext> context,
                                             IImmutableList<object> services)
            : this(context, new OrderedSet<IExtensionDefinition>(services.OfType<IExtensionDefinition>()), services) {}

        public SerializationToolsFactoryHost(Func<IWritingContext> context, IList<IExtensionDefinition> extensions,
                                             IEnumerable<object> services) : base(services)
        {
            _context = context;
            Extensions = extensions;
        }

        ISerializationToolsFactory Factory { get; set; }

        public IWritingContext New() => _context();

        public void Assign(ISerializationToolsFactory factory) => Factory = factory;

        public IList<IExtensionDefinition> Extensions { get; }

        public IExtendedXmlSerializerConfig GetConfiguration(Type type) => Factory?.GetConfiguration(type);

        public IPropertyEncryption EncryptionAlgorithm => Factory?.EncryptionAlgorithm;

        /*public bool Starting(IServiceProvider services) => _extension.Starting(services);
        public void Completed(IServiceProvider services) => _extension.Completed(services);*/
    }

    public interface IExtensionRegistry
    {
        void RegisterSpecification(ProcessState state, IExtensionSpecification specification);
        void Register(ProcessState state, IExtension extension);
    }

    public interface IExtensionDefinition : IExtension
    {
        void Accept(IExtensionRegistry registry);
    }

    class ExtensionRegistry : IExtensionRegistry, IExtensions
    {
        readonly IDictionary<ProcessState, ICollection<IExtensionSpecification>> _specifications =
            new Dictionary<ProcessState, ICollection<IExtensionSpecification>>();

        readonly IDictionary<ProcessState, ICollection<IExtension>> _complete =
            new Dictionary<ProcessState, ICollection<IExtension>>();

        public void RegisterSpecification(ProcessState state, IExtensionSpecification specification)
        {
            if (!_specifications.ContainsKey(state))
            {
                _specifications[state] = new OrderedSet<IExtensionSpecification>();
            }
            _specifications[state].Add(specification);
        }

        public void Register(ProcessState state, IExtension extension)
        {
            if (!_complete.ContainsKey(state))
            {
                _complete[state] = new OrderedSet<IExtension>();
            }
            _complete[state].Add(extension);
        }

        ProcessState DetermineState(IServiceProvider services) => ((IWriting) services).Current.State;

        public bool IsSatisfiedBy(IServiceProvider services)
        {
            var state = DetermineState(services);
            if (_specifications.ContainsKey(state))
            {
                foreach (var specification in _specifications[state])
                {
                    if (!specification.IsSatisfiedBy(services))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Complete(IServiceProvider services)
        {
            var state = DetermineState(services);
            if (_complete.ContainsKey(state))
            {
                foreach (var extension in _complete[state])
                {
                    extension.Complete(services);
                }
            }
        }
    }
}