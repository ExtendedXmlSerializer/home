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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	sealed class AttachedMember : IMember, IProperty
	{
		readonly static Delimiter Separator = DefaultClrDelimiters.Default.Separator;

		readonly IIdentity _type;
		readonly IMember _member;
		readonly IProperty _property;
		readonly string _separator;

		public AttachedMember(IIdentity type, IMember member, IProperty property)
			: this(type, member, property, property.Get(), Separator) {}

		public AttachedMember(IIdentity type, IMember member, IProperty property, TypeInfo memberType, string separator)
		{
			_type = type;
			_member = member;
			_property = property;
			MemberType = memberType;
			_separator = separator;
		}

		public string Identifier => _type.Identifier;

		public string Name => $"{_type.Name}{_separator}{_member.Name}";

		public MemberInfo Metadata => _property.Metadata;

		public TypeInfo MemberType { get; }

		public bool IsWritable => _member.IsWritable;

		public int Order => _member.Order;
		public TypeInfo Get() => _property.Get();

		public bool IsSatisfiedBy(TypeInfo parameter) => _property.IsSatisfiedBy(parameter);
		public bool IsSatisfiedBy(object parameter) => _property.IsSatisfiedBy((TypeInfo) parameter);
		public object Get(object parameter) => _property.Get(parameter);
		public void Assign(object key, object value) => _property.Assign(key, value);

		PropertyInfo IProperty.Metadata => _property.Metadata;
	}
}