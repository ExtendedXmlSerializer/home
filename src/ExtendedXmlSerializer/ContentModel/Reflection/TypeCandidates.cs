// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeCandidates : ITypeCandidates
	{
		readonly ITypeCandidates _identities, _reflection;

		public TypeCandidates(ISpecification<TypeInfo> specification, ITypeFormatter formatter,
		                      params ITypePartitions[] partitions)
			: this(new IdentityPartitionedTypeCandidates(specification, formatter), new PartitionedTypeCandidates(partitions)) {}

		public TypeCandidates(ITypeCandidates identities, ITypeCandidates reflection)
		{
			_identities = identities;
			_reflection = reflection;
		}

		public ImmutableArray<TypeInfo> Get(IIdentity parameter)
		{
			var immutableArray = _identities.GetAny(parameter);
			var typeInfos = _reflection.Get(parameter);
			return immutableArray ?? typeInfos;
		}
	}
}