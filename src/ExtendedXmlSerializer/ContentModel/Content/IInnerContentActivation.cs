using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	interface IInnerContentActivation : IParameterizedSource<TypeInfo, IInnerContentActivator> {}
}