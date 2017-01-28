using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.ElementModel.Names;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel.Options
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

		protected override IElement Create(IName name, INamedElement item) => Create(name, _members.Get(name.Classification), item);

		protected abstract IElement Create(IName name, ElementModel.Members.IMembers members, INamedElement item);
	}
}