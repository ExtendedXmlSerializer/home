using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.ElementModel.Names;

namespace ExtendedXmlSerialization.ElementModel
{
	public abstract class MemberedCollectionElementBase : CollectionElementBase, IMemberedElement
	{
		protected MemberedCollectionElementBase(IName name, INamedElement element, IMembers members)
			: this(name, new CollectionItem(element), members) {}

		protected MemberedCollectionElementBase(IName name, ICollectionItem element, IMembers members)
			: base(name, element)
		{
			Members = members;
		}

		public IMembers Members { get; }
	}
}