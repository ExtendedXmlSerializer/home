using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.References;

sealed class ReferenceView : IReferenceView
{
	readonly ReferenceWalker _walker;

	// ReSharper disable once TooManyDependencies
	public ReferenceView(IContainsCustomSerialization custom, IReferencesPolicy policy, ITypeMembers members,
	                     IEnumeratorStore enumerators, IMemberAccessors accessors)
		: this(new ReferenceWalker(policy, new ProcessReference(custom, members, accessors, enumerators))) {}

	// ReSharper disable once TooManyDependencies
	ReferenceView(ReferenceWalker walker) => _walker = walker;

	public ReferenceResult Get(object parameter) => _walker.Get(parameter);
}