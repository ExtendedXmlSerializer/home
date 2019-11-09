using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class NullableContents : DelegatedSource<TypeInfo, ISerializer>, IContents
	{
		[UsedImplicitly]
		public NullableContents(ConverterContents converters) : base(converters.In(Alteration.Default)
		                                                                       .Get) {}

		sealed class Alteration : IAlteration<TypeInfo>
		{
			public static Alteration Default { get; } = new Alteration();

			Alteration() {}

			public TypeInfo Get(TypeInfo parameter) => parameter.AccountForNullable();
		}
	}
}