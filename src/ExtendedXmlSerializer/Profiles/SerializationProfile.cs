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
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.Instructions.Write;
using ExtendedXmlSerialization.Plans;
using ExtendedXmlSerialization.Plans.Write;
using ExtendedXmlSerialization.ProcessModel.Write;

namespace ExtendedXmlSerialization.Profiles
{
    public class SerializationProfile : SerializationProfileBase
    {
        readonly private static DefaultWritingExtensions DefaultWritingExtensions =
            new DefaultWritingExtensions(EmitTypeForInstanceInstruction.Default);

        private readonly IPlan _plan;
        private readonly INamespaces _namespaces;
        private readonly INamespaceLocator _locator;
        private readonly IImmutableList<object> _services;

        public SerializationProfile(IInstructionSpecification specification, Uri identifier)
            : this(
                new PlanMaker(new Plans.Write.Plans(specification)).Make(),
                new NamespaceLocator(identifier),
                new RootNamespace(identifier)) {}

        SerializationProfile(IPlan plan, INamespaceLocator locator,
                             INamespace root)
            : this(plan, new Namespaces(locator, root, PrimitiveNamespace.Default), locator, root, MemberValueAssignedExtension.Default, DefaultWritingExtensions) {}

        public SerializationProfile(IPlan plan,
                                    INamespaces namespaces,
                                    INamespaceLocator locator, INamespace root, params object[] services)
            : base(root)
        {
            _plan = plan;
            _namespaces = namespaces;
            _locator = locator;
            _services = services.ToImmutableList();
        }

        public override ISerialization New()
        {
            var host = CreateHost(_services);
            var factory = new WritingFactory(host, _locator, _namespaces);
            var serializer = new Serializer(_plan, factory);
            var result = new Serialization(host, serializer);
            return result;
        }

        protected virtual ISerializationToolsFactoryHost CreateHost(IImmutableList<object> services) =>
            new SerializationToolsFactoryHost(services);
    }
}