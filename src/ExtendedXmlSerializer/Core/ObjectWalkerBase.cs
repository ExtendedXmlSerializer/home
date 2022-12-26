using System;
using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core
{
	/// <summary>
	/// Attribution: https://msdn.microsoft.com/en-us/library/system.runtime.serialization.objectmanager(v=vs.110).aspx
	/// </summary>
	abstract class ObjectWalkerBase<TInput, TResult> : IEnumerable<TResult>, IEnumerator<TResult> where TInput : class
	{
		readonly Stack<TInput>   _remaining = new();
		readonly HashSet<object> _tracked;

		protected ObjectWalkerBase(TInput root) : this(root, new HashSet<object>()) {}

		protected ObjectWalkerBase(TInput root, HashSet<object> tracked)
		{
			_tracked = tracked;
			Schedule(root);
		}

		public IEnumerator<TResult> GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Reset()
		{
			throw new NotSupportedException("Resetting the enumerator is not supported.");
		}

		public TResult Current { get; private set; }

		object IEnumerator.Current => Current;

		protected bool Schedule(TInput candidate)
		{
			if (candidate is not null)
			{
				var result = First(candidate);
				if (result)
				{
					_remaining.Push(candidate);
					return true;
				}
			}
			return false;
		}

		protected bool First(object candidate) => _tracked.Add(candidate);

		protected abstract TResult Select(TInput input);

		// Advance to the next item in the enumeration.
		public bool MoveNext()
		{
			// If there are no more items to enumerate, return false.
			var result = _remaining.Count != 0;
			if (result)
			{
				var input = _remaining.Pop();
				Current = Select(input);
			}

			return result;
		}

		public void Dispose() {}
	}
}