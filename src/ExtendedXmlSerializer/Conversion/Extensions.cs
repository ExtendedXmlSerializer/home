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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	public static class Extensions
	{
		readonly static TypeInfo TypeObject = typeof(object).GetTypeInfo();

		public static void Emit(this IWriter @this, IWriteContext context, IContainerElement container, object instance)
			=> Emit(@this, context, container, instance, instance.GetType().GetTypeInfo());

		public static void Emit(this IWriter @this, IWriteContext context, IContainerElement container, object instance,
		                        TypeInfo instanceType)
		{
			var child = context.New(container, instanceType);
			using (child.Emit())
			{
				@this.Write(child, instance);
			}
		}

		public static void New(this IWriter @this, IWriteContext context, IContainerElement container, object instance)
			=>
				@this.Write(context.New(container, instance?.GetType().GetTypeInfo() ?? container.Classification),
				            instance);

		public static ImmutableArray<string> ToStringArray(this string target, params char[] delimiters) =>
			target.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToImmutableArray();

		public static TypeInfo ToTypeInfo(this MemberInfo @this)
			=> @this as TypeInfo ?? @this.DeclaringType.GetTypeInfo();

		public static T Annotated<T>(this XElement @this, T item)
		{
			@this.AddAnnotation(item);
			return item;
		}

		public static object AnnotationAll(this XElement @this, Type type)
			=> @this.Annotation(type) ?? @this.Parent?.AnnotationAll(type);

		public static TypeInfo AccountForNullable(this TypeInfo @this)
			=> Nullable.GetUnderlyingType(@this.AsType())?.GetTypeInfo() ?? @this;


		public static T Activate<T>(this IActivators @this, Type type) => (T) @this.Get(type).Invoke();

		public static IElement Load(this IElements @this, IContainerElement container, TypeInfo instanceType)
			=> Equals(instanceType, container.Classification) ? container.Element : @this.Get(instanceType);

		public static TypeInfo GetDeclaredType(this IContainerElement target, IClassification classification)
		{
			if (Equals(classification.Classification, TypeObject))
			{
				var member = classification as IMemberElement;
				if (member != null)
				{
					var property = target.GetType().GetRuntimeProperty(member.Metadata.Name);
					if (property != null)
					{
						var instance = GetterFactory.Default.Get(property).Invoke(target);
						var result = (instance as IClassification)?.Classification ?? instance as TypeInfo;
						return result;
					}
				}
			}

			return classification.Classification;
		}
	}
}