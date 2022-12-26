using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.References;

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