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

namespace ExtendedXmlSerialization.Configuration
{
	public interface IMemberConfiguration
	{
		IMetadataSpecification Specification { get; }
		IDictionary<MemberInfo, IMemberEmitSpecification> EmitSpecifications { get; }

		IDictionary<MemberInfo, IConverter> Converters { get; }
		IDictionary<MemberInfo, IRuntimeMemberSpecification> Runtime { get; }

		IMemberPolicy Policy { get; }

		IAliases Aliases { get; }
		IMemberOrder Order { get; }
	}


	public class MemberConfiguration : IMemberConfiguration
	{
		public MemberConfiguration()
			: this(
				MetadataSpecification.Default, new Dictionary<MemberInfo, IConverter>(),
				new Dictionary<MemberInfo, IMemberEmitSpecification>(), 
				new Dictionary<MemberInfo, IRuntimeMemberSpecification>()) {}

		public MemberConfiguration(IDictionary<MemberInfo, IConverter> converters)
			: this(
				MetadataSpecification.Default, converters, new Dictionary<MemberInfo, IMemberEmitSpecification>(),
				new Dictionary<MemberInfo, IRuntimeMemberSpecification>()) {}

		public MemberConfiguration(IDictionary<MemberInfo, IConverter> converters, IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
			: this(
				MetadataSpecification.Default, converters, new Dictionary<MemberInfo, IMemberEmitSpecification>(), runtime) {}

		public MemberConfiguration(IMetadataSpecification specification, IDictionary<MemberInfo, IConverter> converters,
		                           IDictionary<MemberInfo, IMemberEmitSpecification> emit, IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime)
			: this(specification, converters, emit, runtime, MemberOrder.Default, MemberAliases.Default, Defaults.MemberPolicy) {}

		public MemberConfiguration(
			IMetadataSpecification specification,
			IDictionary<MemberInfo, IConverter> converters,
			IDictionary<MemberInfo, IMemberEmitSpecification> specifications,
			IDictionary<MemberInfo, IRuntimeMemberSpecification> runtime, IMemberOrder order, IAliases aliases, IMemberPolicy policy)
		{
			Specification = specification;
			Converters = converters;
			EmitSpecifications = specifications;
			Runtime = runtime;
			Order = order;
			Aliases = aliases;
			Policy = policy;
		}

		public IMetadataSpecification Specification { get; }
		public IDictionary<MemberInfo, IConverter> Converters { get; }
		public IDictionary<MemberInfo, IMemberEmitSpecification> EmitSpecifications { get; }
		public IDictionary<MemberInfo, IRuntimeMemberSpecification> Runtime { get; }
		public IMemberOrder Order { get; }
		public IAliases Aliases { get; }
		public IMemberPolicy Policy { get; }
	}
}