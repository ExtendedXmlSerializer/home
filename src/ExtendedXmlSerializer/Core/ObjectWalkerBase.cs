using System;
using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core
{
	/// <summary>
	/// Attribution: https://msdn.microsoft.com/en-us/library/system.runtime.serialization.objectmanager(v=vs.110).aspx
	/// </summary>
	abstract class ObjectWalkerBase<TInput, TResult> : IEnumerable<TResult>, IEnumerator<TResult>
	{
		readonly static TInput DefaultValue = default;

		readonly Stack<TInput> _remaining = new Stack<TInput>();
		readonly Conditions    _conditions;

		protected ObjectWalkerBase(TInput root) : this(root, new Conditions()) {}

		protected ObjectWalkerBase(TInput root, Conditions conditions)
		{
			_conditions = conditions;
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
			if (!Equals(candidate, DefaultValue))
			{
				// Ask the ObjectIDManager if this object has been examined before.
				// If this object has been examined before, do not look at it again just return.
				var result = First(candidate);
				if (result)
				{
					OnSchedule(candidate);
				}

				return result;
			}

			return false;
		}

		bool First(object candidate) => _conditions.Get(candidate)
		                                           .Apply();

		protected abstract TResult Select(TInput input);

		void OnSchedule(TInput candidate) => _remaining.Push(candidate);

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

		public virtual void Dispose() {}
	}
}