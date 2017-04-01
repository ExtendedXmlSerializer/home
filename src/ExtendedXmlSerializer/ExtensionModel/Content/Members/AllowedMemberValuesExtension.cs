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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class AllowedMemberValuesExtension : Collection<IAllowedMemberValues>, ISerializerExtension
	{
		readonly static AllowAssignedValues AllowAssignedValues = AllowAssignedValues.Default;

		readonly IAllowedValueSpecification _allowed;

		public AllowedMemberValuesExtension() : this(AllowAssignedValues) {}

		public AllowedMemberValuesExtension(IAllowedValueSpecification allowed)
			: this(allowed, new Dictionary<MemberInfo, IAllowedValueSpecification>()) {}

		public AllowedMemberValuesExtension(IAllowedValueSpecification allowed,
		                                    IDictionary<MemberInfo, IAllowedValueSpecification> specifications,
		                                    params IAllowedMemberValues[] items) : base(items.ToList())
		{
			_allowed = allowed;
			Specifications = specifications;
		}

		public IDictionary<MemberInfo, IAllowedValueSpecification> Specifications { get; }

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Register(Register);

		IAllowedMemberValues Register(IServiceProvider arg)
		{
			IParameterizedSource<MemberInfo, IAllowedValueSpecification>
				seed = new MappedAllowedMemberValues(Specifications),
				fallback = new FixedInstanceSource<MemberInfo, IAllowedValueSpecification>(_allowed);
			var source = this.Appending(fallback).Aggregate(seed, (current, item) => current.Or(item));
			var result = new AllowedMemberValues(source);
			return result;
		}


		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}