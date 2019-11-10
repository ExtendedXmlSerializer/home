using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IConfigurationContainer EnableAllConstructors(this IConfigurationContainer @this)
			=> @this.Extend(AllConstructorsExtension.Default);

		public static IEnumerable<Type> WithArrayTypes(this IEnumerable<Type> @this)
			=> @this.ToArray()
			        .Alter(x => x.Concat(x.Select(y => y.MakeArrayType()))
			                     .ToArray());
	}
}
