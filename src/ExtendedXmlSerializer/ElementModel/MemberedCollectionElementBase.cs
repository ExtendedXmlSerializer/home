using System.Reflection;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.ElementModel
{
	public abstract class MemberedCollectionElementBase : CollectionElementBase, IMemberedElement
	{
		protected MemberedCollectionElementBase(string displayName, TypeInfo classification, IElement element, IMembers members)
			: this(displayName, classification, new CollectionItem(element), members) {}

		protected MemberedCollectionElementBase(string displayName, TypeInfo classification, IContainerElement element, IMembers members)
			: base(displayName, classification, element)
		{
			Members = members;
		}

		public IMembers Members { get; }
	}
}