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

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class VariableTypeMemberContents : IMemberContents
	{
		readonly IVariableTypeMemberSpecifications _specifications;
		readonly IProperty<TypeInfo> _type;
		readonly ISerializer _runtime;
		readonly IContents _contents;

		[UsedImplicitly]
		public VariableTypeMemberContents(ISerializer runtime, DeferredContents contents)
			: this(VariableTypeMemberSpecifications.Default, ExplicitTypeProperty.Default, runtime, contents) {}

		public VariableTypeMemberContents(IVariableTypeMemberSpecifications specifications, IProperty<TypeInfo> type,
		                                  ISerializer runtime, IContents contents)
		{
			_specifications = specifications;
			_type = type;
			_runtime = runtime;
			_contents = contents;
		}

		public ISerializer Get(IMember parameter)
		{
			var contents = _contents.Get(parameter.MemberType);
			var specification = _specifications.Get(parameter);
			var result = new Serializer(contents, new VariableTypedMemberWriter(specification, _runtime, contents, _type));
			return result;
		}
	}

	sealed class VariableTypeMemberContents<T> : IMemberContents<T>
	{
		readonly Func<bool> _specification;
		readonly IMemberContents<T> _contents;
		readonly IContentSerializer<T> _runtime;

		public VariableTypeMemberContents(IMemberContents<T> contents, IContentSerializer<T> runtime)
			: this(VariableTypeSpecification.Default.Fix(Support<T>.Key), contents, runtime) {}

		public VariableTypeMemberContents(Func<bool> specification, IMemberContents<T> contents, IContentSerializer<T> runtime)
		{
			_specification = specification;
			_contents = contents;
			_runtime = runtime;
		}

		public IContentSerializer<T> Get(IMember parameter)
		{
			var contents = _contents.Get(parameter);
			var result = _specification() ? new ContentSerializer<T>(contents, new VariableTypedMemberWriter<T>(_runtime, contents)) : contents;
			return result;
		}
	}

}