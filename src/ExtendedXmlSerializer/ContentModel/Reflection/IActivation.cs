using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface IActivation : IParameterizedSource<TypeInfo, IReader> {}
}