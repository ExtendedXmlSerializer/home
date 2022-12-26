using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	// TODO

	sealed class ReferenceWalker : IParameterizedSource<object, ReferenceResult>
	{
		readonly IReferencesPolicy _policy;
		readonly ProcessReference  _process;

		public ReferenceWalker(IReferencesPolicy policy, ProcessReference process)
		{
			_policy  = policy;
			_process = process;
		}

		public ReferenceResult Get(object parameter)
		{
			var result = new ReferenceSet(_policy);
			result.Execute(parameter);
			while (result.Any())
			{
				_process.Execute(result);
			}

			return result;
		}
	}

	readonly struct ReferenceBoundary : IDisposable
	{
		readonly Stack<object> _context;

		public ReferenceBoundary(Stack<object> context, object subject)
		{
			Subject  = subject;
			_context = context;
		}

		public object Subject { get; }

		public void Dispose()
		{
			_context.Push(ReferenceCompleted.Default);
		}
	}

	sealed class ReferenceCompleted
	{
		public static ReferenceCompleted Default { get; } = new();

		ReferenceCompleted() {}
	}

	record ReferenceResult(HashSet<object> Encountered, HashSet<object> Cyclical)
	{
		public ReferenceResult() : this(new HashSet<object>(), new HashSet<object>()) {}
	}

	sealed record ReferenceSet : ReferenceResult, ICommand<object>, ISource<ReferenceBoundary>
	{
		readonly IReferencesPolicy _policy;
		readonly Stack<object>     _remaining, _depth = new();
		readonly HashSet<object>   _tracked;

		public ReferenceSet(IReferencesPolicy policy)
			: this(policy, new Stack<object>(), new HashSet<object>()) {}

		public ReferenceSet(IReferencesPolicy policy, Stack<object> remaining, HashSet<object> tracked)
		{
			_policy    = policy;
			_remaining = remaining;
			_tracked   = tracked;
		}

		public void Execute(object parameter)
		{
			if (parameter is not null)
			{
				if (_tracked.Add(parameter))
				{
					_remaining.Push(parameter);
				}
				else
				{
					var info = parameter.GetType();
					if (!info.IsValueType && _policy.IsSatisfiedBy(info))
					{
						Encountered.Add(parameter);
						if (_depth.Contains(parameter))
						{
							Cyclical.Add(parameter);
						}
					}
				}
			}
		}

		public ReferenceBoundary Get()
		{
			var subject = _remaining.Pop();
			while (subject is ReferenceCompleted)
			{
				_depth.Pop();
				subject = Any() ? _remaining.Pop() : null;
			}

			if (subject is not null)
			{
				_depth.Push(subject);
			}
			return new(_depth, subject);
		}

		public bool Any() => _remaining.Count > 0;
	}

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
}