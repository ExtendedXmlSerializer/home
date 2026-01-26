using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.References;

sealed class ReferenceViewUsingHeap : IReferenceView
{
    readonly ReferenceWalker _walker;

    // ReSharper disable once TooManyDependencies
    public ReferenceViewUsingHeap(IContainsCustomSerialization custom, IReferencesPolicy policy, ITypeMembers members,
                         IEnumeratorStore enumerators, IMemberAccessors accessors)
        : this(new ReferenceWalker(policy, new ProcessReferenceUsingHeap(custom, members, accessors, enumerators))) { }

    // ReSharper disable once TooManyDependencies
    ReferenceViewUsingHeap(ReferenceWalker walker) => _walker = walker;

    public ReferenceResult Get(object parameter) => _walker.Get(parameter);
}