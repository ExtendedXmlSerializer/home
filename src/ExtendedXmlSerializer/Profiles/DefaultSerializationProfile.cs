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
using System.Linq;
using ExtendedXmlSerialization.Common;
using ExtendedXmlSerialization.Write;

namespace ExtendedXmlSerialization.Profiles
{
    public class SerializationProfile : SerializationProfileBase
    {
        private readonly IInstructionSpecification _specification;
        private readonly INamespaceLocator _locator;
        private readonly Func<IWritingContext> _context;
        private readonly object[] _services;

        public SerializationProfile(IInstructionSpecification specification, Uri identifier)
            : this(specification, () => new DefaultWritingContext(), identifier) {}

        public SerializationProfile(IInstructionSpecification specification, Func<IWritingContext> context, Uri identifier)
            : this(specification, new NamespaceLocator(identifier), context, identifier, TypeFormatter.Default, MemberValueAssignedExtension.Default) {}

        public SerializationProfile(IInstructionSpecification specification, INamespaceLocator locator, Func<IWritingContext> context, Uri identifier, params object[] services)
            : base(identifier)
        {
            _specification = specification;
            _locator = locator;
            _context = context;
            _services = services;
        }

        public override ISerialization New()
        {
            var host = new SerializationToolsFactoryHost();
            var services = new ServiceRepository(_services);
            var extensions = new OrderedSet<IExtension>(_services.OfType<IExtension>().Concat( new IExtension[] { new DefaultWritingExtensions(host) } ).ToArray());
            var plan = new WritePlanComposer(new PlanSelector(_specification)).Compose();
            var factory = new WritingFactory(host, _locator, services, _context, new CompositeExtension(extensions));
            var serializer = new Serializer(plan, factory);
            var result = new Serialization(host, serializer, services, extensions);
            return result;
        }
    }

    class DefaultSerializationProfile : SerializationProfile
    {
        public static DefaultSerializationProfile Default { get; } = new DefaultSerializationProfile();

        DefaultSerializationProfile()
            : base(
                InstructionSpecification.Default, DefaultNamespaceLocator.Default, () => new DefaultWritingContext(),
                null, DefaultTypeFormatter.Default, DefaultMemberValueAssignedExtension.Default) {}

        public override bool IsSatisfiedBy(Uri parameter) => true;
    }
}