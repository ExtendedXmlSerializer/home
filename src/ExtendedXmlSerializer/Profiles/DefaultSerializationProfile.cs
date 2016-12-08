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
using System.Linq;
using ExtendedXmlSerialization.Common;
using ExtendedXmlSerialization.Write;

namespace ExtendedXmlSerialization.Profiles
{
    public class SerializationProfile : SerializationProfileBase
    {
        private readonly IPlan _plan;
        private readonly INamespaces _namespaces;
        private readonly INamespaceLocator _locator;
        private readonly Func<IWritingContext> _context;
        private readonly IInstruction _emitType;
        private readonly object[] _services;

        public SerializationProfile(IInstructionSpecification specification, Uri identifier)
            : this(specification, () => new DefaultWritingContext(), identifier) {}

        public SerializationProfile(IInstructionSpecification specification, Func<IWritingContext> context, Uri identifier)
            : this(specification, EmitTypeForInstanceInstruction.Default, context, identifier) {}

        public SerializationProfile(IInstructionSpecification specification, IInstruction emitType, Func<IWritingContext> context, Uri identifier)
            : this(specification, emitType, context, new RootNamespace(identifier)) {}

        public SerializationProfile(IInstructionSpecification specification, IInstruction emitType, Func<IWritingContext> context, INamespace root)
            : this(
                new PlanMaker(new PlanSelector(specification, new EnumerableInstructionFactory(emitType), emitType)),
                new NamespaceLocator(root), context, root, emitType,
                MemberValueAssignedExtension.Default) {}

        public SerializationProfile(IPlanMaker maker, INamespaceLocator locator, Func<IWritingContext> context,
                                    INamespace root, IInstruction emitType, params object[] services)
            : this(maker.Make(), context, emitType, new Namespaces(locator, root), locator, root, services)
        {}

        public SerializationProfile(IPlan plan, Func<IWritingContext> context,
                                    IInstruction emitType,
                                    INamespaces namespaces,
                                    INamespaceLocator locator, INamespace root, params object[] services)
            : base(root)
        {
            _plan = plan;
            _namespaces = namespaces;
            _locator = locator;
            _context = context;
            _emitType = emitType;
            _services = services;
        }

        public override ISerialization New()
        {
            var host = new SerializationToolsFactoryHost();
            var services = new ServiceRepository(_services);
            var items = _services.OfType<IExtension>().Concat( new IExtension[] { new DefaultWritingExtensions(host, _emitType) } ).ToArray(); // Should probably move out to a factory.
            var extensions = new OrderedSet<IExtension>(items);
            var factory = new WritingFactory(host, _namespaces,  _locator, services, _context, new CompositeExtension(extensions));
            var serializer = new Serializer(_plan, factory);
            var result = new Serialization(host, serializer, services, extensions);
            return result;
        }
    }

    class DefaultSerializationProfile : SerializationProfile
    {
        readonly private static IPlanMaker Maker =
            new PlanMaker(new DefaultPlanSelector(DefaultInstructionSpecification.Default,
                                                  DefaultEnumerableInstructionFactory.Default));

        public new static DefaultSerializationProfile Default { get; } = new DefaultSerializationProfile();
        DefaultSerializationProfile()
            : base(Maker.Make(), () => new DefaultWritingContext(), EmitTypeInstruction.Default, DefaultNamespaces.Default,
                   DefaultNamespaceLocator.Default, null, DefaultTypeFormatter.Default,
                   DefaultMemberValueAssignedExtension.Default) {}

        public override bool IsSatisfiedBy(Uri parameter) => true;
    }
}