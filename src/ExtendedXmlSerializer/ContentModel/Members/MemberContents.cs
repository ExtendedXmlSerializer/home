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
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class RegisteredMemberContents : IMemberContents, ISpecification<IMember>
	{
		readonly IMemberTable<ISerializer> _serializers;

		public RegisteredMemberContents(IMemberTable<ISerializer> serializers) => _serializers = serializers;

		public bool IsSatisfiedBy(IMember parameter) => _serializers.IsSatisfiedBy(parameter.Metadata);

		public ISerializer Get(IMember parameter) => _serializers.Get(parameter.Metadata);
	}

	sealed class MemberContents : Selector<IMember, ISerializer>, IMemberContents
	{
		readonly static DelegatedAssignedSpecification<IMember, IVariableTypeSpecification> Specification = new DelegatedAssignedSpecification<IMember, IVariableTypeSpecification>(VariableTypeMemberSpecifications.Default.Get);

		public MemberContents(RegisteredMemberContents registered, VariableTypeMemberContents variable, DefaultMemberContents contents)
			: base(
			       new Option(registered, registered),
			       new Option(Specification, variable),
			       new Option(contents)
			) {}

		sealed class Option : DecoratedOption<IMember, ISerializer>
		{
			public Option(IMemberContents source) : this(x => true, source) {}

			public Option(Func<IMember, bool> specification, IMemberContents source)
				: base(new DelegatedSpecification<IMember>(specification), source) {}

			public Option(ISpecification<IMember> specification, IParameterizedSource<IMember, ISerializer> source)
				: base(specification, source) {}
		}
	}
}