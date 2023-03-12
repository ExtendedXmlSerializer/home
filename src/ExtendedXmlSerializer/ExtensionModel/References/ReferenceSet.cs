using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.References;

sealed record ReferenceSet : ReferenceResult, ISpecification<object>, IParameterizedSource<object, ReferenceBoundary>
{
	readonly IReferencesPolicy _policy;
	readonly Stack<object>     _depth;
	readonly HashSet<object>   _tracked;

	public ReferenceSet(IReferencesPolicy policy) : this(policy, new Stack<object>(), new HashSet<object>()) {}

	public ReferenceSet(IReferencesPolicy policy, Stack<object> depth, HashSet<object> tracked)
	{
		_policy  = policy;
		_depth   = depth;
		_tracked = tracked;
	}

	// ReSharper disable once CognitiveComplexity
	// ReSharper disable once ExcessiveIndentation
	public bool IsSatisfiedBy(object parameter)
	{
		if (parameter is not null)
		{
			var result = _tracked.Add(parameter);
			if (!result)
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
			return result;
		}

		return false;
	}

	public ReferenceBoundary Get(object parameter)
	{
		_depth.Push(parameter);
		return new(_depth);
	}
}