using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References;

sealed class ProcessReference : ICommand<ReferenceSet>
{
	readonly ISpecification<TypeInfo> _allowed;
	readonly ITypeMembers             _members;
	readonly IMemberAccessors         _accessors;
	readonly IEnumeratorStore         _store;

	// ReSharper disable once TooManyDependencies
	public ProcessReference(IContainsCustomSerialization custom, ITypeMembers members, IMemberAccessors accessors,
	                        IEnumeratorStore store)
		: this(AssignedSpecification<TypeInfo>.Default.And(custom.Inverse()), members, accessors, store) {}

	// ReSharper disable once TooManyDependencies
	public ProcessReference(ISpecification<TypeInfo> allowed, ITypeMembers members, IMemberAccessors accessors,
	                        IEnumeratorStore store)
	{
		_allowed   = allowed;
		_members   = members;
		_accessors = accessors;
		_store     = store;
	}

	public void Execute(ReferenceSet parameter)
	{
		using var boundary = parameter.Get();
		var       next     = boundary.Subject;
		var       type     = next.GetType();
		if (_allowed.IsSatisfiedBy(type))
		{
			var members = _members.Get(type);
			for (var i = 0; i < members.Length; i++)
			{
				var value = _accessors.Get(members[i]).Get(next);
				parameter.Execute(value);
			}

			var iterator = _store.For(next);
			while (iterator?.MoveNext() ?? false)
			{
				parameter.Execute(iterator.Current);
			}
		}
	}
}