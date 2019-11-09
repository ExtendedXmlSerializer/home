using System;
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	public static class Extensions
	{
		/*public static IConfigurationContainer EnableSingletons(this IConfigurationContainer @this)
			=> @this.Extend(SingletonActivationExtension.Default);*/

		public static IConfigurationContainer EnableAllConstructors(this IConfigurationContainer @this)
			=> @this.Extend(AllConstructorsExtension.Default);

		public static IEnumerable<Type> WithArrayTypes(this IEnumerable<Type> @this)
			=> @this.ToArray()
			        .Alter(x => x.Concat(x.Select(y => y.MakeArrayType()))
			                     .ToArray());
	}
}