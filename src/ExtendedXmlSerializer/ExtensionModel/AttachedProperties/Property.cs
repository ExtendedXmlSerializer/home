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

using System;
using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public class Property<TType, TValue> : IProperty
	{
		readonly static ISpecification<TypeInfo> Accepts = IsAssignableSpecification<TType>.Default;
		readonly static TypeInfo TypeInfo = Support<TValue>.Key;

		readonly ISpecification<TypeInfo> _specification;
		readonly ITableSource<TType, TValue> _store;

		public Property(ITableSource<TType, TValue> store, Expression<Func<IProperty>> source) : this(Accepts, store, source) {}

		public Property(ISpecification<TypeInfo> specification, ITableSource<TType, TValue> store,
		                Expression<Func<IProperty>> source)

		{
			_specification = specification;
			_store = store;
			Metadata = source.GetMemberInfo().AsValid<PropertyInfo>();
		}

		public PropertyInfo Metadata { get; }

		bool ISpecification<object>.IsSatisfiedBy(object parameter) => parameter is TType && IsSatisfiedBy((TType) parameter);
		object IParameterizedSource<object, object>.Get(object parameter) => Get(parameter.AsValid<TType>());

		void IAssignable<object, object>.Assign(object key, object value)
			=> Assign(key.AsValid<TType>(), value.AsValid<TValue>());

		public bool IsSatisfiedBy(TType parameter) => _store.IsSatisfiedBy(parameter);
		public TValue Get(TType parameter) => _store.Get(parameter);
		public void Assign(TType key, TValue value) => _store.Assign(key, value);

		public TypeInfo Get() => TypeInfo;

		public bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);
		bool ITableSource<object, object>.Remove(object key) => Remove(key.AsValid<TType>());

		public bool Remove(TType key) => _store.Remove(key);
	}
}