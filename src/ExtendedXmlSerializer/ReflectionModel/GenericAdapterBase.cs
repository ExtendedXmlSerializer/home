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
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	abstract class GenericAdapterBase<T> : DecoratedSource<ImmutableArray<TypeInfo>, T>
	{
		protected GenericAdapterBase(Type definition, IParameterizedSource<TypeInfo, T> source)
			: base(
			       new SelectCoercer<TypeInfo, Type>(TypeCoercer.Default.ToDelegate())
				       .To(new GenericTypeAlteration(definition))
				       .To(TypeMetadataCoercer.Default)
				       .To(source)
			) {}
	}

	sealed class TypeMetadataCoercer : IParameterizedSource<Type, TypeInfo>
	{
		public static TypeMetadataCoercer Default { get; } = new TypeMetadataCoercer();
		TypeMetadataCoercer() { }

		public TypeInfo Get(Type parameter) => parameter.GetTypeInfo();
	}

	sealed class TypeCoercer : IParameterizedSource<TypeInfo, Type>
	{
		public static TypeCoercer Default { get; } = new TypeCoercer();
		TypeCoercer() { }

		public Type Get(TypeInfo parameter) => parameter.AsType();
	}


	sealed class GenericTypeAlteration : IParameterizedSource<ImmutableArray<Type>, Type>
	{
		readonly Type _definition;

		public GenericTypeAlteration(Type definition) => _definition = definition;

		public Type Get(ImmutableArray<Type> parameter) => _definition.MakeGenericType(parameter.ToArray());
	}

	sealed class SelectCoercer<TFrom, TTo> : IParameterizedSource<ImmutableArray<TFrom>, ImmutableArray<TTo>>
	{
		readonly Func<TFrom, TTo> _select;

		public SelectCoercer(Func<TFrom, TTo> select) => _select = select;

		public ImmutableArray<TTo> Get(ImmutableArray<TFrom> parameter) => parameter.Select(_select).ToImmutableArray();
	}
}