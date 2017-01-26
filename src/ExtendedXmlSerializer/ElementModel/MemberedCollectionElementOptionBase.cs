using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel
{
	public abstract class MemberedCollectionElementOptionBase : CollectionElementOptionBase
	{
		readonly IElementMembers _members;

		protected MemberedCollectionElementOptionBase(ISpecification<TypeInfo> specification, IElements elements, INames names,
		                                              IElementMembers members, ICollectionItemTypeLocator items)
			: base(specification, elements, names, items)
		{
			_members = members;
		}

		protected override IElement Create(string displayName, TypeInfo collectionType, IElement item) =>
			Create(displayName, collectionType, _members.Get(collectionType), item);

		protected abstract IElement Create(string displayName, TypeInfo collectionType, IMembers members, IElement item);
	}
}