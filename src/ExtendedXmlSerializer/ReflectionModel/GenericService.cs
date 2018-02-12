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

using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IGenericService<out T> : IParameterizedSource<ImmutableArray<TypeInfo>, T> {}

	class GenericService<T> : IGenericService<T>
	{
		readonly IGeneric<T> _generic;

		public GenericService(IServiceProvider provider, Type definition) :
			this(new GenericLocation<T>(provider, definition)) {}

		public GenericService(IGeneric<T> generic) => _generic = generic;

		public T Get(ImmutableArray<TypeInfo> parameter) => _generic.Get(parameter)
		                                                            .Invoke();

		public T Get(TypeInfo parameter) => Get(ImmutableArray.Create(parameter));
	}

	class GenericSource<T> : IGenericService<T>
	{
		readonly IGenericService<ISource<T>> _source;

		public GenericSource(IServiceProvider provider, Type definition) : this(new GenericService<ISource<T>>(provider, definition)) {}

		public GenericSource(IGenericService<ISource<T>> source) => _source = source;

		public T Get(ImmutableArray<TypeInfo> parameter) => _source.Get(parameter).Get();

		public T Get(TypeInfo parameter) => Get(ImmutableArray.Create(parameter));
	}

	interface IGenericSource<in TParameter, out TResult> : IGenericService<IParameterizedSource<TParameter, TResult>> {}

	interface IGenericDecoration<TParameter, TResult> : IGenericService<IDecoration<TParameter, TResult>> { }

	interface IInstanceTypes<in T> : IParameterizedSource<T, IEnumerable<TypeInfo>> {}

	class GenericSource<TParameter, TResult, TDefinition> : GenericSource<TParameter, TResult>
	{
		public GenericSource(IServiceProvider provider, IInstanceTypes<TParameter> types)
			: base(provider, typeof(TDefinition).GetGenericTypeDefinition(), types) {}
	}

	class GenericSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly IInstanceTypes<TParameter> _types;
		readonly IGenericService<IParameterizedSource<TParameter, TResult>> _source;

		public GenericSource(IServiceProvider provider, Type definition, IInstanceTypes<TParameter> types)
			: this(new GenericService<IParameterizedSource<TParameter, TResult>>(provider, definition), types) {}

		public GenericSource(IGenericService<IParameterizedSource<TParameter, TResult>> source, IInstanceTypes<TParameter> types)
		{
			_types = types;
			_source = source;
		}

		public TResult Get(TParameter parameter) => _source.Get(_types.Get(parameter).ToImmutableArray()).Get(parameter);
	}

	class GenericDecoration<TParameter, TResult, TDefinition> : GenericSource<Decoration<TParameter, TResult>, TResult, TDefinition>, IDecoration<TParameter, TResult>
	{
		public GenericDecoration(IServiceProvider provider, IInstanceTypes<TParameter> types) : this(provider, new TypesDecoration<TParameter, TResult>(types)) {}
		public GenericDecoration(IServiceProvider provider, IInstanceTypes<Decoration<TParameter, TResult>> types) : base(provider, types) {}
	}

	class GenericDecoration<TParameter, TResult> : GenericSource<Decoration<TParameter, TResult>, TResult>, IDecoration<TParameter, TResult>
	{
		public GenericDecoration(IServiceProvider provider, Type definition, IInstanceTypes<Decoration<TParameter, TResult>> types)
			: base(provider, definition, types) {}
	}

	class TypesDecoration<TParameter, TResult> : IInstanceTypes<Decoration<TParameter, TResult>>
	{
		readonly IInstanceTypes<TParameter> _types;

		public TypesDecoration(IInstanceTypes<TParameter> types) => _types = types;

		public IEnumerable<TypeInfo> Get(Decoration<TParameter, TResult> parameter) => _types.Get(parameter.Parameter);
	}
}