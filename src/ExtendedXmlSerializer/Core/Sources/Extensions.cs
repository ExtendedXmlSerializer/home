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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ExtendedXmlSerializer.Core.Sprache;

namespace ExtendedXmlSerializer.Core.Sources
{
	public static class Extensions
	{
		public static Parser<IOption<T>> XOptional<T>(this Parser<T> parser)
		{
			if (parser == null) throw new ArgumentNullException(nameof(parser));
			return i =>
			       {
				       var result = parser(i);
				       if (result.WasSuccessful)
					       return Result.Success(new Some<T>(result.Value), result.Remainder);

				       if (result.Remainder.Equals(i))
					       return Result.Success(new None<T>(), i);

				       return Result.Failure<IOption<T>>(result.Remainder, result.Message, result.Expectations);
			       };
		}

		public static Parser<Tuple<T1, T2>> SelectMany<T1, T2>(this Parser<T1> parser, Parser<T2> instance)
			=> parser.SelectMany(instance.Accept, Tuple.Create);

		public static Parser<T> Get<T>(this IParsing<T> @this) => @this.Get;

		public static T ParseAsOptional<T>(this Parser<T> @this, string data)
			=> @this.XOptional().Invoke(Inputs.Default.Get(data)).Value.GetOrDefault();

		public static T Get<T>(this IParsing<T> @this, string parameter) => @this.Get(Inputs.Default.Get(parameter)).Value;

		public static Func<IInput, IResult<T>> ToDelegate<T>(this Parser<T> @this)
			=> new Func<IInput, IResult<T>>(@this);

		public static Parser<T> ToParser<T>(this Func<IInput, IResult<T>> @this) => new Parser<T>(@this);

		public static T TryOrDefault<T>(this Parser<T> @this, string data)
		{
			var parse = @this.TryParse(data);
			var result = parse.WasSuccessful ? parse.Value : default(T);
			return result;
		}

		public static T Get<T>(this IParameterizedSource<Stream, T> @this, string parameter)
			=> @this.Get(new MemoryStream(Encoding.UTF8.GetBytes(parameter)));

		public static T Get<T>(this IParameterizedSource<TypeInfo, T> @this, Type parameter)
			=> @this.Get(parameter.GetTypeInfo());

		public static T Get<T>(this IParameterizedSource<Type, T> @this, TypeInfo parameter)
			=> @this.Get(parameter.AsType());

		public static IParameterizedSource<TParameter, TResult> Or<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<TParameter, TResult> next)
			where TResult : class => new LinkedDecoratedSource<TParameter, TResult>(@this, next);

		public static T Alter<T>(this IEnumerable<IAlteration<T>> @this, T seed)
			=> @this.Aggregate(seed, (current, alteration) => alteration.Get(current));

		public static Func<TResult> Build<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                       TParameter parameter)
			=> @this.Fix(parameter).Singleton().Get;

		public static ISource<TResult> Fix<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> new FixedSource<TParameter, TResult>(@this, parameter);

		public static ISource<T> Singleton<T>(this ISource<T> @this) => new SingletonSource<T>(@this.Get);

		public static Func<TParameter, TResult> ToDelegate<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this) => @this.Get;

		public static Func<T> ToDelegate<T>(this ISource<T> @this) => @this.Get;
	}
}