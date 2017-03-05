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
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

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
			var specifications = fallback.Get<MemberConfiguration>().EmitSpecifications;
			return _services.Aggregate(parameter, (registry, o) => registry.RegisterInstanceByConvention(o))
			                .RegisterFallback(fallback.IsSatisfiedBy, fallback.GetService)
			                .Register(fallback.Get<IEnumerable<IConverter>>)
			                .Register(fallback.Get<IEnumerable<IConverters>>)
			                .Register(fallback.Get<IAlteration<IConverter>>)
			                .Register<IAlteration<IConverters>, ConvertersAlteration>()
			                .Decorate<IEnumerable<IConverter>>(
				                (provider, converters) => converters.Select(provider.Get<IAlteration<IConverter>>().Get))
			                .Decorate<IEnumerable<IConverters>>(
				                (provider, converters) => converters.Select(provider.Get<IAlteration<IConverters>>().Get))
			                .Register<EnumerationConverters>()
			                .Register<ISerializer, RuntimeSerializer>()
			                .Register<IXmlFactory, XmlFactory>()
			                .Register<ArrayContentOption>()
			                .Register<IDictionaryEntries, DictionaryEntries>()
			                .Register<DictionaryContentOption>()
			                .Register<CollectionContentOption>()
			                .Register<MemberedContentOption>()
			                .Register<RuntimeContentOption>()
			                .Register<IEnumerable<IContentOption>, ContentOptions>()
			                .Register(
				                provider => provider.GetAllInstances<IConverter>()
				                                    .Select(x => new FixedContentOption(x, x.ToSerializer()))
				                                    .Concat(provider.GetAllInstances<IConverters>()
				                                                    .Select<IConverters, IContentOption>(
					                                                    x => new DelegatedContentOption(x, x.Get)))
				                                    .Concat(provider.GetAllInstances<IContentOption>())
				                                    .ToArray())
			                .Register<IContents, Contents>()
			                .Register<IMemberEmitSpecifications, MemberEmitSpecifications>()
			                .RegisterInstance(specifications.GetType(), specifications)
			                .Register<IMemberOption, VariableTypeMemberOption>()
			                .Register<MemberProfiles>()
			                .Register(factory => factory.Get<MemberProfiles>().ToDelegate())
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