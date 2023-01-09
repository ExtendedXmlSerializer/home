using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References;

sealed class ProcessReference : ICommand<ProcessReferenceInput>
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

	public void Execute(ProcessReferenceInput parameter)
	{
		var (results, current) = parameter;
		results.IsSatisfiedBy(current);
		Process(results, current);
	}
	
	void Process(ReferenceSet results, object current)
	{
		using var boundary = results.Get(current);
		var       type     = current.GetType();
		if (_allowed.IsSatisfiedBy(type))
		{
			var members = _members.Get(type);
			for (var i = 0; i < members.Length; i++)
			{
				var value = _accessors.Get(members[i]).Get(current);
				if (results.IsSatisfiedBy(value))
				{
					Process(results, value);
				}
			}

			var iterator = _store.For(current);
			while (iterator?.MoveNext() ?? false)
			{
				var o = iterator.Current;
				if (results.IsSatisfiedBy(o))
				{
					Process(results, o);
				}
			}
		}
	}
}