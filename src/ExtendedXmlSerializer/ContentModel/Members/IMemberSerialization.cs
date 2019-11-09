using System.Collections.Immutable;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	public interface IMemberSerialization
		: ISource<ImmutableArray<IMemberSerializer>>,
		  IParameterizedSource<string, IMemberSerializer>,
		  IParameterizedSource<object, ImmutableArray<IMemberSerializer>> {}
}