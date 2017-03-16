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

namespace ExtendedXmlSerialization.Configuration
{
	class ExtendedXmlMemberConfiguration : IExtendedXmlMemberConfiguration
	{
		readonly MemberInfo _memberInfo;

		public ExtendedXmlMemberConfiguration(MemberInfo memberInfo)
		{
			_memberInfo = memberInfo;
		}

		/*public ExtendedXmlTypeConfiguration<T> TypeConfiguration { get; set; }
		public Expression<Func<T, TProperty>> PropertyExpression { get; set; }

		public bool IsObjectReference { get; set; }
		public bool IsAttribute { get; set; }
		public bool IsEncrypt { get; set; }
		public string ChangedName { get; set; }
		public int ChangedOrder { get; set; }

		public IExtendedXmlPropertyConfiguration<T, TOtherProperty> Property<TOtherProperty>(
			Expression<Func<T, TOtherProperty>> property)
		{
			return TypeConfiguration.Member(property);
		}

		public IExtendedXmlPropertyConfiguration<T, TProperty> EnableReferences()
		{
			IsObjectReference = true;
			TypeConfiguration.IsObjectReference = true;
			TypeConfiguration.GetObjectId = p => PropertyExpression.Compile()((T) p).ToString();
			return this;
		}

		public IExtendedXmlPropertyConfiguration<T, TProperty> AsAttribute()
		{
			IsAttribute = true;
			return this;
		}

		public IExtendedXmlPropertyConfiguration<T, TProperty> Encrypt()
		{
			IsEncrypt = true;
			return this;
		}

		public IExtendedXmlPropertyConfiguration<T, TProperty> Name(string name)
		{
			ChangedName = name;
			return this;
		}

		public IExtendedXmlPropertyConfiguration<T, TProperty> Order(int order)
		{
			ChangedOrder = order;
			return this;
		}*/

		public IExtendedXmlMemberConfiguration Name(string name)
		{
			throw new NotImplementedException();
		}

		public IExtendedXmlMemberConfiguration Order(int order)
		{
			throw new NotImplementedException();
		}
	}
}