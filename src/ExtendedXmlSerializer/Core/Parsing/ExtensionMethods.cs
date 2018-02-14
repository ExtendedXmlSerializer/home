using ExtendedXmlSerializer.Core.Sprache;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.Core.Parsing
{
	static class ExtensionMethods
    {
		public static TypeInfo GetType(this IParser<MemberInfo> @this, string parameter)
			=> @this.Get(parameter).To<TypeInfo>();


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

	    public static Parser<T> ToParser<T>(this IParsing<T> @this) => @this.Get;

	    public static T ParseAsOptional<T>(this Parser<T> @this, string data)
		    => @this.XOptional()
		            .Invoke(Inputs.Default.Get(data))
		            .Value.GetOrDefault();

	    public static T Get<T>(this IParsing<T> @this, string parameter) => @this.Get(Inputs.Default.Get(parameter))
	                                                                             .Value;

	    public static Func<IInput, IResult<T>> ToDelegate<T>(this Parser<T> @this)
		    => new Func<IInput, IResult<T>>(@this);

	    public static Parser<T> Get<T>(this Func<IInput, IResult<T>> @this) => new Parser<T>(@this);

	    public static T TryOrDefault<T>(this Parser<T> @this, string data)
	    {
		    var parse = @this.TryParse(data);
		    var result = parse.WasSuccessful ? parse.Value : default(T);
		    return result;
	    }
    }
}
