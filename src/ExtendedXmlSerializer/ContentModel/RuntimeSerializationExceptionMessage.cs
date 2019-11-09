using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class RuntimeSerializationExceptionMessage
		: DelegatedSource<TypeInfo, string>, IRuntimeSerializationExceptionMessage
	{
		public static IRuntimeSerializationExceptionMessage Default { get; }
			= new RuntimeSerializationExceptionMessage();

		RuntimeSerializationExceptionMessage() :
			base(_ => "The default behavior requires an empty public constructor on the (non-abstract) class to activate.") {}
	}
}