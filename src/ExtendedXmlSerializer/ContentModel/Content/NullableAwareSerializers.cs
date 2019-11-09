using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class NullableAwareSerializers : DelegatedSource<TypeInfo, ISerializer>, ISerializers
	{
		public NullableAwareSerializers(ISerializers serializers) : this(AccountForNullableAlteration.Default,
		                                                                 serializers) {}

		NullableAwareSerializers(IAlteration<TypeInfo> type, ISerializers serializers) : base(serializers.In(type)
		                                                                                                 .Get) {}
	}
}