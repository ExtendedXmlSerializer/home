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
using System.Reflection;
using ExtendedXmlSerialization.ConverterModel.Elements;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Members
{
	public abstract class MemberOptionBase : OptionBase<MemberInformation, IMember>, IMemberOption
	{
		readonly IContainers _containers;
		readonly IAliasProvider<MemberInfo> _alias;
		readonly IGetterFactory _getter;

		protected MemberOptionBase(ISpecification<MemberInformation> specification, IContainers containers)
			: this(specification, containers, MemberAliasProvider.Default, GetterFactory.Default) {}

		protected MemberOptionBase(ISpecification<MemberInformation> specification, IContainers containers,
		                           IAliasProvider<MemberInfo> alias, IGetterFactory getter
		)
			: base(specification)
		{
			_containers = containers;
			_alias = alias;
			_getter = getter;
		}

		public override IMember Get(MemberInformation parameter)
		{
			var getter = _getter.Get(parameter.Metadata);
			var body = _containers.Content(parameter.MemberType);
			var result = Create(_alias.Get(parameter.Metadata) ?? parameter.Metadata.Name, parameter.MemberType, getter, body,
			                    parameter.Metadata);
			return result;
		}

		protected abstract IMember Create(string displayName, TypeInfo classification, Func<object, object> getter,
		                                  ISerializer body, MemberInfo metadata);
	}
}