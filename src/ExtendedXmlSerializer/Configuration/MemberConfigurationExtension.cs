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
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.TypeModel;
using RuntimeMemberSpecifications = ExtendedXmlSerialization.ContentModel.Members.RuntimeMemberSpecifications;

namespace ExtendedXmlSerialization.Configuration
{
	sealed class MemberConfigurationExtension : ISerializerExtension
	{
		public MemberConfigurationExtension()
			: this(
				DefaultMetadataSpecification.Default, new Dictionary<MemberInfo, IConverter>(),
				new Dictionary<MemberInfo, IMemberEmitSpecification>(),
				new Dictionary<MemberInfo, IRuntimeMemberSpecification>()) {}

		public MemberConfigurationExtension(IDictionary<MemberInfo, IConverter> converters)
			: this(
				DefaultMetadataSpecification.Default, converters, new Dictionary<MemberInfo, IMemberEmitSpecification>(),
				new Dictionary<MemberInfo, IRuntimeMemberSpecification>()) {}

		public MemberConfigurationExtension(IDictionary<MemberInfo, IConverter> converters,
		                                    IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
			: this(
				DefaultMetadataSpecification.Default, converters, new Dictionary<MemberInfo, IMemberEmitSpecification>(), runtime) {}

		public MemberConfigurationExtension(IMetadataSpecification specification,
		                                    IDictionary<MemberInfo, IConverter> converters,
		                                    IDictionary<MemberInfo, IMemberEmitSpecification> emit,
		                                    IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
			: this(specification, new MemberConverters(converters), new MappedMemberEmitSpecifications(emit),
			       new RuntimeMemberSpecifications(runtime), new Dictionary<MemberInfo, string>(),
			       new Dictionary<MemberInfo, int>(), Defaults.MemberPolicy) {}

		public MemberConfigurationExtension(
			IMetadataSpecification specification,
			IMemberConverters converters,
			IMemberEmitSpecifications specifications,
			IRuntimeMemberSpecifications runtime, IDictionary<MemberInfo, string> names, IDictionary<MemberInfo, int> order,
			IMemberPolicy policy)
		{
			Specification = specification;
			Converters = converters;
			EmitSpecifications = specifications;
			Runtime = runtime;
			Order = order;
			Names = names;
			Policy = policy;
		}

		public IDictionary<MemberInfo, string> Names { get; }
		public IDictionary<MemberInfo, int> Order { get; }

		public IMetadataSpecification Specification { get; }
		public IMemberConverters Converters { get; }
		public IMemberEmitSpecifications EmitSpecifications { get; }
		public IRuntimeMemberSpecifications Runtime { get; }

		public IMemberPolicy Policy { get; }

		public IServiceRepository Get(IServiceRepository parameter)
		{
			return parameter.Register<IMetadataSpecification, MetadataSpecification>()
			                .RegisterInstance(Policy.And<PropertyInfo>(Specification))
			                .RegisterInstance(Policy.And<FieldInfo>(Specification))
			                .RegisterInstance(Converters)
			                .RegisterInstance(Runtime)
			                .RegisterInstance<INames>(new MemberNames(new MemberTable<string>(Names).Or(DeclaredNames.Default)))
			                .RegisterInstance<IMemberOrder>(new MemberOrder(Order, DefaultMemberOrder.Default))
			                .RegisterInstance<IMemberEmitSpecification>(AssignedEmitMemberSpecification.Default)
			                .RegisterInstance(EmitSpecifications)
			                .Decorate<IMemberEmitSpecifications>(
				                (provider, defaults) =>
					                new MemberEmitSpecifications(defaults, provider.Get<IMemberEmitSpecification>()));
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}