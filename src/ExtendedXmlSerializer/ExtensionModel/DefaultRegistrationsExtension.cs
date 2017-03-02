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

using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core.Sources;
using LightInject;
using ContainerOptions = ExtendedXmlSerialization.ContentModel.Content.ContainerOptions;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class DefaultRegistrationsExtension : ISerializerExtension
	{
		readonly ImmutableArray<object> _services;
		readonly IMemberEmitSpecifications _specifications;

		public DefaultRegistrationsExtension(params object[] services) : this(services.ToImmutableArray()) {}

		public DefaultRegistrationsExtension(ImmutableArray<object> services)
			: this(services, services.OfType<MemberConfiguration>().Single().EmitSpecifications) {}

		public DefaultRegistrationsExtension(ImmutableArray<object> services, IMemberEmitSpecifications specifications)
		{
			_services = services;
			_specifications = specifications;
		}

		public IServices Get(IServices parameter) =>
			_services.Aggregate(parameter, (registry, o) => registry.RegisterInstanceByConvention(o))
			         .Register<IMemberEmitSpecifications, MemberEmitSpecifications>()
			         .RegisterInstance(_specifications.GetType(), _specifications)
			         .RegisterInstance(DefaultMemberEmitSpecifications.Default)
			         .Register<ISerializer, RuntimeSerializer>()
			         .Register<IMemberOption, VariableTypeMemberOption>()
			         .Register<MemberProfiles>()
			         .Register(factory => factory.GetInstance<MemberProfiles>().ToDelegate())
			         .Register<IMemberSerialization, MemberSerialization>()
			         .Register<ISelector, ContentModel.Members.Selector>()
			         .Register<IMembers, Members>()
			         .Register<IContentOptions, ContentOptions>()
			         .Register<IContainerOptions, ContainerOptions>()
			         .Register(factory => factory.GetInstance<IContainerOptions>().ToArray())
			         .Register<IStaticReferenceSpecification, ContainsStaticReferenceSpecification>()
			         .Register<ContainsStaticReferenceSpecification>()
			         .Register<IRootReferences, RootReferences>()
			         .Decorate<IXmlFactory>(
				         (factory, xmlFactory) =>
					         new ReferentialAwareXmlFactory(factory.GetInstance<IStaticReferenceSpecification>(),
					                                        factory.GetInstance<IRootReferences>(), xmlFactory)
			         )
			         .Register<IExtendedXmlSerializer, ExtendedXmlSerializer>();
	}
}