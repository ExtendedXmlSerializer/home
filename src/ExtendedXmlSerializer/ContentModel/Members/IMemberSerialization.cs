using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// Used to contain serializers for a particular member.  Used during runtime purposes by providing the value of a
	/// member, as well as a general store to retrieve a member serializer based on member name.
	/// </summary>
	public interface IMemberSerialization
		: ISource<ImmutableArray<IMemberSerializer>>,
		  IParameterizedSource<string, IMemberSerializer>,
		  IParameterizedSource<object, ImmutableArray<IMemberSerializer>> {}
}