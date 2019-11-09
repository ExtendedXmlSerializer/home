using Sprache;
using System;

namespace ExtendedXmlSerializer.Core.Parsing
{
	static class Extensions
	{
		public static Parser<Tuple<T1, T2>> SelectMany<T1, T2>(this Parser<T1> parser, Parser<T2> instance)
			=> parser.SelectMany(instance.Accept, Tuple.Create);

		public static Parser<T> ToParser<T>(this IParsing<T> @this) => @this.Get;

		public static T ParseAsOptional<T>(this Parser<T> @this, string data)
			=> @this.XOptional()
			        .Invoke(Inputs.Default.Get(data))
			        .Value.GetOrDefault();

		public static Func<T> Build<T>(this IOption<T> @this) => @this.IsDefined ? new Func<T>(@this.Get) : null;

		public static T? GetAssigned<T>(this IOption<T> @this) where T : struct
			=> @this.IsDefined ? @this.Get() : (T?)null;

		public static T Get<T>(this IParsing<T> @this, string parameter) => @this.Get(Inputs.Default.Get(parameter))
		                                                                         .Value;

		public static Func<IInput, IResult<T>> ToDelegate<T>(this Parser<T> @this)
			=> new Func<IInput, IResult<T>>(@this);

		public static Parser<T> Get<T>(this Func<IInput, IResult<T>> @this) => new Parser<T>(@this);

		public static T TryOrDefault<T>(this Parser<T> @this, string data)
		{
			var parse  = @this.TryParse(data);
			var result = parse.WasSuccessful ? parse.Value : default;
			return result;
		}
	}
}