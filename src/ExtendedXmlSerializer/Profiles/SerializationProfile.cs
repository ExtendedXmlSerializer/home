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
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Instructions.Write;
using ExtendedXmlSerialization.Plans;
using ExtendedXmlSerialization.Plans.Write;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Services.Write;

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
            : this(specification, () => new DefaultWritingContext(), identifier, MemberValueAssignedExtension.Default) {}

        public SerializationProfile(IInstructionSpecification specification, Func<IWritingContext> context,
                                    Uri identifier, params object[] services)
            : this(
                new PlanMaker(new Plans.Write.Plans(specification)).Make(), context, EmitTypeForInstanceInstruction.Default,
                new NamespaceLocator(identifier),
                new RootNamespace(identifier), services) {}

        SerializationProfile(IPlan plan, Func<IWritingContext> context, IInstruction emitType, INamespaceLocator locator,
                             INamespace root, params object[] services)
            : this(
                plan, context, emitType, new Namespaces(locator, root, PrimitiveNamespace.Default), locator, root,
                services) {}

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
            var items =
                _services.OfType<IExtension>()
                         .Concat(new IExtension[] {new DefaultWritingExtensions(host, _emitType)})
                         .ToArray(); // Should probably move out to a factory.
            var extensions = new OrderedSet<IExtension>(items);
            var factory = new WritingFactory(_locator, _namespaces, host, services, _context,
                                             new CompositeExtension(extensions));
            var serializer = new Serializer(_plan, factory);
            var result = new Serialization(host, serializer, services, extensions);
            return result;
        }
    }
}