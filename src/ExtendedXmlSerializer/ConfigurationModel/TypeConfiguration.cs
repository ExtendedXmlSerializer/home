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

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ConfigurationModel
{
	public sealed class TypeConfiguration<T> : ContextBase, ITypeConfiguration
	{
		readonly IProperty<string> _name;
		readonly IMemberSource _members;

		public TypeConfiguration(IRootContext root, IProperty<string> name)
			: this(root, name, new MemberConfigurations<T>(new TypeConfigurationContext(root, Support<T>.Key))) {}

		public TypeConfiguration(IRootContext context, IProperty<string> name, IMemberSource members)
			: base(context, context)
		{
			_name = name;
			_members = members;
		}

		public ITypeConfiguration Name(string name)
		{
			_name.Assign(name);
			return this;
		}

		public IMemberConfiguration Member(MemberInfo member) => _members.Get(member);

		public IEnumerator<IMemberConfiguration> GetEnumerator() => _members.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public TypeInfo Get() => Support<T>.Key;
		public IMemberConfiguration Get(MemberInfo parameter) => _members.Get(parameter);
	}
}