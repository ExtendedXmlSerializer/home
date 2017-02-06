using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization.ConverterModel
{
	public static class Extensions
	{
		public static IContentOption ToContent<T>(this IConverter<T> @this ) => new ContentOption<T>(@this);
	}
}
