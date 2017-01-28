using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.ElementModel.Names;

namespace ExtendedXmlSerialization.ElementModel
{
	public abstract class MemberedCollectionBase : CollectionElementBase, IMemberedElement
	{
		protected MemberedCollectionBase(IName name, INamedElement element, IMembers members)
			: this(name, new CollectionItem(element), members) {}

		protected MemberedCollectionBase(IName name, ICollectionItem element, IMembers members)
			: base(name, element)
		{
			Members = members;
		}

		public IMembers Members { get; }
	}
}