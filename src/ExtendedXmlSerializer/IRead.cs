using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer
{
	interface IRead<in T> : IParameterizedSource<T, object> {}
}