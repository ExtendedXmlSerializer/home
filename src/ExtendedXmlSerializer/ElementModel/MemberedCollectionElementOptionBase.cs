using System;
using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel
{
	public abstract class MemberedCollectionElementOptionBase : CollectionElementOptionBase
	{
		readonly IElementMembers _members;

		protected MemberedCollectionElementOptionBase(ISpecification<TypeInfo> specification, IElements elements,
		                                              INames names, IElementMembers members)
			: this(specification, elements, names, members, CollectionItemTypeLocator.Default) {}

		protected MemberedCollectionElementOptionBase(ISpecification<TypeInfo> specification, IElements elements, INames names,
		                                              IElementMembers members, ICollectionItemTypeLocator items)
			: base(specification, elements, names, items)
		{
			_members = members;
		}

		protected override IElement Create(string name, TypeInfo collectionType, Func<IElement> item) =>
			Create(name, collectionType, _members.Get(collectionType), item);

		protected abstract IElement Create(string name, TypeInfo collectionType, IMembers members, Func<IElement> item);
	}
}