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
using System.Reflection;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Extensions;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using IAliases = ExtendedXmlSerialization.ContentModel.Members.IAliases;
using Selector = ExtendedXmlSerialization.ContentModel.Members.Selector;

namespace ExtendedXmlSerialization.Configuration
{
	class SerializationServices : ISerialization, IServiceProvider
	{
		readonly IServiceRepository _services;
		readonly ISerialization _serialization;

		public SerializationServices(
			ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field,
			IMemberEmitSpecifications specifications,
			ITypeSelector types, IXmlFactory xml,
			IMemberWriters writers, IContentOption known,
			IAliases aliases, IMemberOrder order, ICollection<ISerializerExtension> extensions)
		{
			var runtime = new RuntimeSerializer(this);
			var variable = new VariableTypeMemberOption(runtime);
			var memberContent = new RecursionGuardedMemberContent(new MemberContent(this));
			var profiles = new MemberProfiles(specifications, memberContent, writers, aliases, order);
			var serialization = new MemberSerialization(property, field, profiles.Get);

			var members = new Members(serialization, new Selector(variable));

			var content = new ContentOptions(this, serialization, members, variable, runtime);

			IServiceRepository seed = new ServiceRepository(
				property, field, specifications, types, xml, writers, content, aliases, order,
				runtime, variable, profiles, serialization, members, known, ElementOptionSelector.Default
			);

			_services = extensions.Alter(seed);
			_serialization = Serialization(extensions);
		}

		ISerialization Serialization(IEnumerable<ISerializerExtension> extensions)
		{
			var seed = new SerializationResult(_services,
			                                   _services.Get<ISerialization>() ??
			                                   new Serialization(Options().ToArray()));

			var result = extensions.Alter(seed).Get();
			return result;
		}

		IContainerOptions Options() =>
			_services.Get<IContainerOptions>() ??
			new ContainerOptions(
				_services.GetValid<IContentOption>(),
				_services.GetValid<IElementOptionSelector>(),
				_services.GetValid<IContentOptions>()
			);

		IContainer IParameterizedSource<TypeInfo, IContainer>.Get(TypeInfo parameter) => _serialization.Get(parameter);

		public object GetService(Type serviceType) => _services.GetService(serviceType);
	}
}