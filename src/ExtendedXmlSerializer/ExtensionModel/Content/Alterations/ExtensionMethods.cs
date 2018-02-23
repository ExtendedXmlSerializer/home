using System;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	public static class ExtensionMethods
	{
		public static IType<T> AlterInstance<T>(this IType<T> @this, Func<T, T> alter)
			=> @this.AlterInstance(new DelegatedAlteration<T>(alter));

		public static THost AlterInstance<THost, T>(this THost @this, IAlteration<T> alteration)
			where THost : class, IMetadataConfiguration
			=> @this.Set(WriteInstancePipelineProperty<T>.Default,
			             @this.Get(WriteInstancePipelineProperty<T>.Default).Adding(alteration));

		public static THost AlterContent<THost>(this THost @this, Func<string, string> alter)
			where THost : class, IMetadataConfiguration => AlterContent(@this, new DelegatedAlteration<string>(alter));

		public static THost AlterContent<THost>(this THost @this, IAlteration<string> alteration)
			where THost : class, IMetadataConfiguration
			=> @this.Set(WriteContentPipelineProperty.Default,
			             @this.Get(WriteContentPipelineProperty.Default).Adding(alteration));
	}
}