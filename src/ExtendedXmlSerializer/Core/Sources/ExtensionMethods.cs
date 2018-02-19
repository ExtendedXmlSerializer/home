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

using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtendedXmlSerializer.Core.Sources
{
	public static class Assume<T>
	{
		public static IParameterizedSource<T, TResult> Default<TResult>(TResult @this)
			=> @this.ToSource(A<T>.Default);
	}

	public static class ExtensionMethods
	{
		public static ISpecificationSource<TParameter, TResult>
			Cache<TParameter, TResult>(this ISpecificationSource<TParameter, TResult> @this)
			where TParameter : class where TResult : class
			=> new SpecificationSource<TParameter, TResult>(@this, @this
			                                                       .ToDelegate()
			                                                       .Cache()
			                                                       .ToSource());

		public static IParameterizedSource<TParameter, TResult>
			Cache<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this)
			where TParameter : class where TResult : class => @this.ToDelegate()
			                                                       .Cache()
			                                                       .ToSource();

		public static Func<TParameter, TResult>
			Cache<TParameter, TResult>(this Func<TParameter, TResult> @this)
			where TParameter : class where TResult : class
			=> CachingAlteration<TParameter, TResult>.Default.Get(@this);

		public static ISpecificationSource<TParameter, TResult>
			ReferenceCache<TParameter, TResult>(this ISpecificationSource<TParameter, TResult> @this)
			where TParameter : class where TResult : class
			=> new SpecificationSource<TParameter, TResult>(@this, @this
			                                                       .ToDelegate()
			                                                       .ReferenceCache()
			                                                       .ToSource());

		public static IParameterizedSource<TParameter, TResult>
			ReferenceCache<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this)
			where TParameter : class where TResult : class => @this.ToDelegate()
			                                                       .ReferenceCache()
			                                                       .ToSource();

		public static Func<TParameter, TResult>
			ReferenceCache<TParameter, TResult>(this Func<TParameter, TResult> @this)
			where TParameter : class where TResult : class
			=> ReferenceCachingAlteration<TParameter, TResult>.Default.Get(@this);

		public static ISpecificationSource<TParameter, TResult>
			StructureCache<TParameter, TResult>(this ISpecificationSource<TParameter, TResult> @this)
			where TParameter : class where TResult : struct
			=> new SpecificationSource<TParameter, TResult>(@this, @this.ToDelegate().StructureCache().ToSource());

		public static IParameterizedSource<TParameter, TResult>
			StructureCache<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this)
			where TParameter : class where TResult : struct => @this.ToDelegate()
			                                                       .StructureCache()
			                                                       .ToSource();

		public static Func<TParameter, TResult>
			StructureCache<TParameter, TResult>(this Func<TParameter, TResult> @this)
			where TParameter : class where TResult : struct
			=> StructureCachingAlteration<TParameter, TResult>.Default.Get(@this);

		public static IParameterizedSource<TParameter, TResult> ToSource<TParameter, TResult>(this TResult @this, A<TParameter> _)
			=> new FixedInstanceSource<TParameter, TResult>(@this);

		public static IParameterizedSource<TParameter, TResult>
			ToSource<TParameter, TResult>(this Func<TParameter, TResult> @this)
			=> @this.Target as IParameterizedSource<TParameter, TResult> ?? new DelegatedSource<TParameter, TResult>(@this);

		public static IAlteration<object> Adapt<T>(this IAlteration<T> @this) => new AlterationAdapter<T>(@this);

		public static T Get<T>(this IParameterizedSource<Stream, T> @this, string parameter)
			=> @this.Get(new MemoryStream(Encoding.UTF8.GetBytes(parameter)));

		public static T Get<T>(this IParameterizedSource<TypeInfo, T> @this, Type parameter)
			=> @this.Get(parameter.GetTypeInfo());

		public static T Get<T>(this IParameterizedSource<Type, T> @this, TypeInfo parameter)
			=> @this.Get(parameter.AsType());


		public static IParameterizedSource<Decoration<TFrom, TResult>, TResult> In<TFrom, TTo, TResult>(
			this IParameterizedSource<Decoration<TTo, TResult>, TResult> @this, A<TFrom> _) => In(@this, DecorationParameterCoercer<TFrom, TTo, TResult>.Default);

		public static IParameterizedSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this IParameterizedSource<TTo, TResult> @this, A<TFrom> _) => In(@this, CastCoercer<TFrom, TTo>.Default);

		public static ISpecificationSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this ISpecificationSource<TTo, TResult> @this, IParameterizedSource<TFrom, TTo> coercer)
			=> new SpecificationSource<TFrom, TResult>(@this.To(coercer.ToDelegate()), @this.ToDelegate().In(coercer.ToDelegate()));

		public static IParameterizedSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this IParameterizedSource<TTo, TResult> @this, IParameterizedSource<TFrom, TTo> coercer)
			=> @this.ToDelegate().In(coercer.ToDelegate());

		public static IParameterizedSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this IParameterizedSource<TTo, TResult> @this, Func<TFrom, TTo> coercer) => @this.ToDelegate().In(coercer);

		public static IParameterizedSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this Func<TTo, TResult> @this, Func<TFrom, TTo> coercer)
			=> new CoercedParameter<TFrom, TTo, TResult>(@this, coercer);

		/*public static ISpecificationSource<TParameter, TTo> To<TParameter, TResult, TTo>(
			this ISpecificationSource<TParameter, TResult> @this, A<TTo> _) => @this.To(CastCoercer<TResult, TTo>.Default);

		public static ISpecificationSource<TParameter, TTo> To<TParameter, TResult, TTo>(
			this ISpecificationSource<TParameter, TResult> @this, IParameterizedSource<TResult, TTo> coercer)
			=> new SpecificationSource<TParameter, TTo>(@this, To((IParameterizedSource<TParameter, TResult>)@this, coercer));*/

		public static IParameterizedSource<TParameter, TTo> Out<TParameter, TResult, TTo>(
			this IParameterizedSource<TParameter, TResult> @this, A<TTo> _) => @this.Out(CastCoercer<TResult, TTo>.Default);

		public static IParameterizedSource<TParameter, TTo> Out<TParameter, TResult, TTo>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<TResult, TTo> coercer)
			=> new CoercedResult<TParameter, TResult, TTo>(@this, coercer);

		public static ImmutableArray<TItem>? GetAny<T, TItem>(this IParameterizedSource<T, ImmutableArray<TItem>> @this,
		                                                      T parameter)
		{
			var items = @this.Get(parameter);
			var result = items.Any() ? items : (ImmutableArray<TItem>?) null;
			return result;
		}

		public static IParameterizedSource<TParameter, TResult> If<TParameter, TResult, TAttribute>(
			this IParameterizedSource<TParameter, TResult> @this, A<TAttribute> _) where TAttribute : Attribute
			=> @this.If(IsDefinedSpecification<TAttribute>.Default.To(InstanceMetadataCoercer<TParameter>.Default));

		public static IParameterizedSource<TParameter, TResult> If<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification)
			=> new ConditionalSource<TParameter, TResult>(specification, @this,
			                                              new FixedInstanceSource<TParameter, TResult>(default(TResult)));

		public static IParameterizedSource<TParameter, TResult> If<TParameter, TResult, TAttribute>(
			this TResult @this, A<TAttribute> _) where TAttribute : Attribute
			=> If(@this, IsDefinedSpecification<TAttribute>.Default.To(InstanceMetadataCoercer<TParameter>.Default));

		public static IParameterizedSource<TParameter, TResult> If<TParameter, TResult>(
			this TResult @this, ISpecification<TParameter> specification)
			=> new ConditionalInstanceSource<TParameter, TResult>(specification, @this, default(TResult));

		public static IParameterizedSource<TParameter, TResult> Into<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<Decoration<TParameter, TResult>, TResult> other)
			=> new Decorator<TParameter, TResult>(other, @this);

		public static IParameterizedSource<TParameter, TResult> Unless<TParameter, TResult, TOther>(
			this IParameterizedSource<TParameter, TResult> @this, A<TOther> _) where TOther : ISource<TResult>
			=> @this.Unless(IsTypeSpecification<TParameter, TOther>.Default, SourceCoercer<TResult>.Default.In(A<TParameter>.Default));

		public static IParameterizedSource<TParameter, TResult> Unless<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecificationSource<TParameter, TResult> source)
			=> Unless(@this, source, source);

		public static IParameterizedSource<TSpecification, TInstance> Unless<TSpecification, TInstance>(
			this TInstance @this, ISpecification<TSpecification> specification, TInstance other)
			=> new FixedInstanceSource<TSpecification, TInstance>(@this).Unless(specification, other);

		public static IParameterizedSource<TParameter, TResult> Unless<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			TResult other)
			=> @this.Unless(specification, new FixedInstanceSource<TParameter, TResult>(other));

		public static IParameterizedSource<TParameter, TResult> Unless<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			IParameterizedSource<TParameter, TResult> other)
			=> Unless(@this, specification, AlwaysSpecification<TResult>.Default, other);

		public static IParameterizedSource<TParameter, TResult> Unless<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			ISpecification<TResult> result, IParameterizedSource<TParameter, TResult> other)
			=> new ConditionalSource<TParameter, TResult>(specification, result, other, @this);

		public static IParameterizedSource<TParameter, TResult> Or<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<TParameter, TResult> next)
			where TResult : class => new LinkedDecoratedSource<TParameter, TResult>(@this, next);

		public static T Alter<T>(this IEnumerable<IAlteration<T>> @this, T seed)
			=> @this.Aggregate(seed, (current, alteration) => alteration.Get(current));

		public static TResult Alter<T, TResult>(this IEnumerable<T> @this, Func<T, TResult> alter) => @this.Select(alter).Last();

		public static Func<TResult> Build<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                       TParameter parameter)
			=> @this.ToDelegate()
			        .Build(parameter);

		public static Func<TResult> Build<TParameter, TResult>(this Func<TParameter, TResult> @this,
		                                                       TParameter parameter)
			=> @this.Fix(parameter)
			        .Singleton()
			        .Get;


		public static ISource<TResult> Fix<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> @this.ToDelegate()
			        .Fix(parameter);

		public static Func<TResult> ToDelegate<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> @this.ToDelegate()
			        .Fix(parameter)
			        .ToDelegate();

		public static ISource<TResult> Fix<TParameter, TResult>(this Func<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> new FixedParameterSource<TParameter, TResult>(@this, parameter);

		public static ISource<T> Singleton<T>(this ISource<T> @this) => new SingletonSource<T>(@this.Get);

		public static Func<TParameter, TResult> ToDelegate<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this) => @this.Get;

		public static Func<T> ToDelegate<T>(this ISource<T> @this) => @this.Get;
	}
}