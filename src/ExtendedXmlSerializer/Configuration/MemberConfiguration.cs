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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using System.Reflection;

// ReSharper disable UnusedTypeParameter

namespace ExtendedXmlSerializer.Configuration
{
	class MemberConfiguration<T, TMember> : ContextBase, IMemberConfiguration<T, TMember>, IInternalMemberConfiguration
	{
		readonly IProperty<string> _name;
		readonly IProperty<int> _order;
		readonly MemberInfo _member;

		public MemberConfiguration(ITypeConfiguration parent, MemberInfo member)
			: this(parent, parent.Root.With<MemberPropertiesExtension>(), member) {}

		MemberConfiguration(
			ITypeConfiguration parent,
			MemberPropertiesExtension extension,
			MemberInfo member) : this(parent,
			                          new MemberProperty<string>(extension.Names, member),
			                          new MemberProperty<int>(extension.Order, member),
			                          member) {}

		public MemberConfiguration(ITypeConfiguration parent, IProperty<string> name, IProperty<int> order,
		                           MemberInfo member) : base(parent)
		{
			_name = name;
			_order = order;
			_member = member;
		}

		IMemberConfiguration IInternalMemberConfiguration.Name(string name)
		{
			_name.Assign(name);
			return this;
		}

		IMemberConfiguration IInternalMemberConfiguration.Order(int order)
		{
			_order.Assign(order);
			return this;
		}

		MemberInfo ISource<MemberInfo>.Get() => _member;
		public IMemberConfigurations Get() => Parent.AsValid<ITypeConfiguration>().AsInternal().Get();
	}
}