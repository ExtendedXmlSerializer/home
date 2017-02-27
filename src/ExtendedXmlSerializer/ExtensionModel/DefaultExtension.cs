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
using System.Linq;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using LightInject;
using ContainerOptions = ExtendedXmlSerialization.ContentModel.Content.ContainerOptions;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class DefaultExtension : ISerializerExtension, IParameterizedSource<IServiceProvider, ISerialization>
	{
		readonly ISerialization _serialization;
		readonly IMemberContent _content;
		readonly ImmutableArray<object> _services;

		public DefaultExtension(ISerialization serialization, params object[] services)
			: this(
				serialization, new RecursionGuardedMemberContent(new MemberContent(serialization)), services.ToImmutableArray()) {}

		public DefaultExtension(ISerialization serialization, IMemberContent content, ImmutableArray<object> services)
		{
			_serialization = serialization;
			_content = content;
			_services = services;
		}

		public IServiceRegistry Get(IServiceRegistry parameter) =>
			_services.Aggregate(parameter, (registry, o) => registry.RegisterInstanceByConvention(o))
			         .RegisterInstance(_serialization)
			         .RegisterInstance(_content)
			         .Register<ISerializer, RuntimeSerializer>()
			         .Register<IMemberOption, VariableTypeMemberOption>()
			         .Register<IMemberOption, VariableTypeMemberOption>()
			         .Register<MemberProfiles>()
			         .Register(factory => factory.GetInstance<MemberProfiles>().ToDelegate())
			         .Register<IMemberSerialization, MemberSerialization>()
			         .Register<ISelector, ContentModel.Members.Selector>()
			         .Register<IMembers, Members>()
			         .Register<IContentOptions, ContentOptions>()
			         .Register<IContainerOptions, ContainerOptions>().Register<IExtendedXmlSerializer, ExtendedXmlSerializer>();

		public ISerialization Get(IServiceProvider parameter)
			=> new Serialization(parameter.Get<IContainerOptions>().ToArray());
	}
}