using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using NetFabric.Hyperlinq;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	// TODO

	sealed class ReferenceWalker : IParameterizedSource<object, Lease<object>>
	{
		readonly IReferencesPolicy _policy;
		readonly IterateReferences _references;

		public ReferenceWalker(IReferencesPolicy policy, IterateReferences references)
		{
			_policy     = policy;
			_references = references;
		}

		public Lease<object> Get(object parameter)
		{
			var scheduler = new ReferenceScheduler(_policy);
			scheduler.IsSatisfiedBy(parameter);
			var result = _references.Get(scheduler)
			                        .AsValueEnumerable()
			                        .ToArray(ArrayPool<object>.Shared);
			return result;
		}
	}

	sealed class ReferenceScheduler : ISpecification<object>, ISource<object>
	{
		readonly IReferencesPolicy _policy;
		readonly Stack<object>     _remaining;
		readonly HashSet<object>   _tracked;

		public ReferenceScheduler(IReferencesPolicy policy) :
			this(policy, new Stack<object>(), new HashSet<object>()) {}

		public ReferenceScheduler(IReferencesPolicy policy, Stack<object> remaining, HashSet<object> tracked)
		{
			_policy    = policy;
			_remaining = remaining;
			_tracked   = tracked;
		}

		public bool IsSatisfiedBy(object parameter)
		{
			if (parameter is not null)
			{
				var add = _tracked.Add(parameter);
				if (add)
				{
					_remaining.Push(parameter);
				}
				else
				{
					var info   = parameter.GetType();
					var result = info is { IsValueType: false } && _policy.IsSatisfiedBy(info);
					return result;
				}
			}

			return false;
		}

		public object Get() => _remaining.Pop();

		public int Count => _remaining.Count;
	}

	sealed class ReferenceMembers : IParameterizedSource<ReferenceScheduler, IEnumerable<object>>
	{
		readonly ISpecification<TypeInfo> _allowed;
		readonly ITypeMembers             _members;
		readonly IMemberAccessors         _accessors;
		readonly IEnumeratorStore         _store;

		// ReSharper disable once TooManyDependencies
		public ReferenceMembers(IContainsCustomSerialization custom, ITypeMembers members, IMemberAccessors accessors,
		                        IEnumeratorStore store)
			: this(AssignedSpecification<TypeInfo>.Default.And(custom.Inverse()), members, accessors, store) {}

		// ReSharper disable once TooManyDependencies
		public ReferenceMembers(ISpecification<TypeInfo> allowed, ITypeMembers members, IMemberAccessors accessors,
		                        IEnumeratorStore store)
		{
			_allowed   = allowed;
			_members   = members;
			_accessors = accessors;
			_store     = store;
		}

		public IEnumerable<object> Get(ReferenceScheduler parameter)
		{
			var next = parameter.Get();
			var type = next.GetType();
			if (_allowed.IsSatisfiedBy(type))
			{
				var members = _members.Get(type);
				for (var i = 0; i < members.Length; i++)
				{
					var value = _accessors.Get(members[i]).Get(next);
					if (parameter.IsSatisfiedBy(value))
					{
						yield return value;
					}
				}

				var iterator = _store.For(next);
				while (iterator?.MoveNext() ?? false)
				{
					if (parameter.IsSatisfiedBy(iterator.Current))
					{
						yield return iterator.Current;
					}
				}
			}
		}
	}

	sealed class IterateReferences : IParameterizedSource<ReferenceScheduler, IEnumerable<object>>
	{
		readonly ReferenceMembers _members;

		public IterateReferences(ReferenceMembers members) => _members = members;

		public IEnumerable<object> Get(ReferenceScheduler parameter)
		{
			while (parameter.Count > 0)
			{
				foreach (var member in _members.Get(parameter))
				{
					yield return member;
				}
			}
		}
	}
}