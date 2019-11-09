using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface ITypeMemberSource : IParameterizedSource<TypeInfo, IEnumerable<IMember>> {}
}