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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	public sealed class MemberOrderingExtension : ISerializerExtension, IAssignable<MemberInfo, int>, IParameterizedSource<MemberInfo, int?>
	{
		readonly IDictionary<MemberInfo, int> _order;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;

		public MemberOrderingExtension(IParameterizedSource<MemberInfo, int> defaultMemberOrder)
			: this(new Dictionary<MemberInfo, int>(MemberComparer.Default), defaultMemberOrder) {}

		public MemberOrderingExtension(IDictionary<MemberInfo, int> order,
		                                 IParameterizedSource<MemberInfo, int> defaultMemberOrder)
		{
			_order = order;
			_defaultMemberOrder = defaultMemberOrder;
		}



		public IServiceRepository Get(IServiceRepository parameter) =>
			parameter
				.RegisterInstance<IMemberOrder>(new MemberOrder(_order, _defaultMemberOrder));

		void ICommand<IServices>.Execute(IServices parameter) {}
		public void Execute(KeyValuePair<MemberInfo, int> parameter)
		{
			_order[parameter.Key] = parameter.Value;
		}

		public int? Get(MemberInfo parameter) => _order.GetStructure(parameter);
	}
}