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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	public sealed class AllowedMemberValuesExtension : Collection<IAllowedMemberValues>, ISerializerExtension
	{
		readonly IAllowedValueSpecification _allowed;

		public AllowedMemberValuesExtension() : this(AllowAssignedValues.Default) {}

		public AllowedMemberValuesExtension(IAllowedValueSpecification allowed)
			: this(allowed, new Dictionary<MemberInfo, IAllowedValueSpecification>()) {}

		public AllowedMemberValuesExtension(IAllowedValueSpecification allowed,
		                                    IDictionary<MemberInfo, IAllowedValueSpecification> specifications,
		                                    params IAllowedMemberValues[] items) : base(items.ToList())
		{
			_allowed       = allowed;
			Specifications = specifications;
		}

		public IDictionary<MemberInfo, IAllowedValueSpecification> Specifications { get; }

		public IDictionary<MemberInfo, ISpecification<object>> Instances { get; }
			= new Dictionary<MemberInfo, ISpecification<object>>();

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Register(Register);

		IAllowedMemberValues Register(IServiceProvider arg)
		{
			IParameterizedSource<MemberInfo, IAllowedValueSpecification>
				seed = new MappedAllowedMemberValues(Specifications.ToDictionary(x => x.Key, x => (ISpecification<object>)x.Value)
				                                                   .Concat(Instances)
				                                                   .GroupBy(x => x.Key)
				                                                   .ToDictionary(x => x.Key, Create)),
				fallback = new FixedInstanceSource<MemberInfo, IAllowedValueSpecification>(_allowed);
			var source = this.Appending(fallback)
			                 .Aggregate(seed, (current, item) => current.Or(item));
			var result = new AllowedMemberValues(source);
			return result;
		}

		IAllowedValueSpecification Create(IGrouping<MemberInfo, KeyValuePair<MemberInfo, ISpecification<object>>> parameter)
		{
			var item = parameter.Select(x => x.Value)
			                    .ToArray();

			var only = item.Only();
			if (only != null)
			{
				return new InstanceAwareValueSpecification(only is IAllowedValueSpecification allow ? allow : _allowed,
				                                           Instances.ContainsKey(parameter.Key)
					                                           ? Instances[parameter.Key]
					                                           : AlwaysSpecification<object>.Default);
			}

			return new InstanceAwareValueSpecification(item.First(), item.Last());
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}