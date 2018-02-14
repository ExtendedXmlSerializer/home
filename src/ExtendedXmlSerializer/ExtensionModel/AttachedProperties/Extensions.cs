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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public static class Extensions
	{
		public static IConfigurationElement EnableAttachedProperties(this IConfigurationElement @this,
		                                                               params IProperty[] properties)
			=> @this.EnableAttachedProperties(new HashSet<IProperty>(properties));

		public static IConfigurationElement EnableAttachedProperties(this IConfigurationElement @this,
		                                                               ICollection<IProperty> properties)
			=> EnableAttachedProperties(@this, properties, new HashSet<Type>());

		public static IConfigurationElement EnableAttachedProperties(this IConfigurationElement @this,
		                                                               ICollection<IProperty> properties,
		                                                               ICollection<Type> types)
			=> @this.Executed(new AttachedPropertiesExtension(new Registrations<IProperty>(properties, types)))
			        .Return(@this);

		public static TValue Get<TType, TValue>(this TType @this, Property<TType, TValue> property) => property.Get(@this);

		public static void Set<TType, TValue>(this TType @this, Property<TType, TValue> property, TValue value)
			=> property.Assign(@this, value);

		public static MemberConfiguration<TType, TValue> AttachedProperty<TType, TValue>(
			this IConfigurationElement @this,
			Expression<Func<Property<TType, TValue>>> property)
		{
			var instance = property.Compile()
			                       .Invoke();
			@this.Extend<AttachedPropertiesExtension>()
			     .Registrations.Instances.Add(instance);
			var subject = property.GetMemberInfo()
			                      .AsValid<PropertyInfo>();


			var type = @this.GetTypeConfiguration(subject.DeclaringType);
			var result = type.Member(subject).AsValid<MemberConfiguration<TType, TValue>>();
			return result;
		}

		public static IConfigurationElement AttachedProperty<TType, TValue>(
			this IConfigurationElement @this,
				Expression<Func<Property<TType, TValue>>> property,
				Action<MemberConfiguration<TType, TValue>> configure)
		{
			configure(@this.AttachedProperty(property));
			return @this;
		}
	}
}