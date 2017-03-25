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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ActivationContext : TableSource<string, object>, IActivationContext
	{
		readonly ConstructorInfo _constructor;
		readonly ImmutableArray<IMember> _members;
		readonly IMemberAccessors _accessors;

		public ActivationContext(IMemberAccessors accessors, ConstructorInfo constructor, ImmutableArray<IMember> members)
			: this(accessors, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), constructor, members) {}

		public ActivationContext(IMemberAccessors accessors, IDictionary<string, object> source,
		                         ConstructorInfo constructor, ImmutableArray<IMember> members) : base(source)
		{
			_constructor = constructor;
			_members = members;
			_accessors = accessors;
		}

		public object Get()
		{
			var names = _constructor.GetParameters()
			                        .Select(x => x.Name)
			                        .ToArray();
			var parameters = names.Select(Get).ToArray();
			var result = _constructor.Invoke(parameters);
			Apply(result);
			return result;
		}

		void Apply(object result)
		{
			foreach (var member in _members)
			{
				if (IsSatisfiedBy(member.Name))
				{
					var access = _accessors.Get(member);
					access.Assign(result, Get(member.Name));
				}
			}
		}
	}
}