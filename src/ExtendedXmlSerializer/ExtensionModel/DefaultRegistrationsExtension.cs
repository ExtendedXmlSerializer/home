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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using RuntimeSerializer = ExtendedXmlSerialization.ContentModel.RuntimeSerializer;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class DefaultRegistrationsExtension : ISerializerExtension
	{
		readonly ImmutableArray<object> _services;

		public DefaultRegistrationsExtension(params object[] services) : this(services.ToImmutableArray()) {}

		public DefaultRegistrationsExtension(ImmutableArray<object> services)
		{
			_services = services;
		}

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var fallback = new ServiceProvider(_services);
			var configuration = fallback.Get<MemberConfiguration>();

			return _services.Aggregate(parameter, (registry, o) => registry.RegisterInstanceByConvention(o))
			                .RegisterInstance<IElements>(ContentModel.Content.Elements.Default)
			                .RegisterInstance<IMemberEmitSpecification>(AssignedEmitMemberSpecification.Default)
			                .RegisterInstance<IActivation>(Activation.Default)
			                .RegisterFallback(fallback.IsSatisfiedBy, fallback.GetService)
			                .Register<ISerializer, RuntimeSerializer>()
			                .Register<IXmlFactory, XmlFactory>()
			                .Register<ArrayContentOption>()
			                .Register<IDictionaryEntries, DictionaryEntries>()
			                .Register<DictionaryContentOption>()
			                .Register<CollectionContentOption>()
			                .Register<MemberedContentOption>()
			                .Register<RuntimeContentOption>()
			                .Register<IEnumerable<IContentOption>, ContentOptions>()
			                .Register(provider => provider.GetAllInstances<IContentOption>().ToArray())
			                .Register<IContents, Contents>()
			                .RegisterInstance(configuration.EmitSpecifications)
			                .Decorate<IMemberEmitSpecifications>(
				                (provider, defaults) =>
					                new MemberEmitSpecifications(defaults, provider.Get<IMemberEmitSpecification>()))
			                .RegisterInstance(configuration.Runtime)
			                .RegisterConstructorDependency<IMemberOption>(
				                (provider, info) => provider.Create<VariableTypeMemberOption>())
			                .Register<IMemberSerializers, MemberSerializers>()
			                .Register<IMemberSerialization, MemberSerialization>()
			                .Register<ISelector, Selector>()
			                .Register<IMembers, Members>()
			                .Register<ContainsStaticReferenceSpecification>()
			                .Register<IStaticReferenceSpecification, ContainsStaticReferenceSpecification>()
			                .Register<IRootReferences, RootReferences>()
			                .Decorate<ISerialization>((factory, context) => new ReferentialAwareSerialization(
				                                          factory.Get<IStaticReferenceSpecification>(),
				                                          factory.Get<IRootReferences>(),
				                                          context
			                                          )
			                )
			                .Register<IExtendedXmlSerializer, ExtendedXmlSerializer>();
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}