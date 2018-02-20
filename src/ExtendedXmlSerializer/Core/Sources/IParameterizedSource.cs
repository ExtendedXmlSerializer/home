// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Sources
{
	public interface IParameterizedSource<in TParameter, out TResult>
	{
		TResult Get(TParameter parameter);
	}

	class ValidatedResult<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly ISpecification<TResult> _specification;
		readonly IParameterizedSource<TParameter, TResult> _source;
		readonly IParameterizedSource<TParameter, TResult> _fallback;

		public ValidatedResult(ISpecification<TResult> specification, IParameterizedSource<TParameter, TResult> source,
		                       IParameterizedSource<TParameter, TResult> fallback)
		{
			_specification = specification;
			_source = source;
			_fallback = fallback;
		}

		public TResult Get(TParameter parameter)
		{
			var candidate = _source.Get(parameter);
			var result = _specification.IsSatisfiedBy(candidate) ? candidate : _fallback.Get(parameter);
			return result;
		}
	}

	class GuardedFallback<TParameter, TResult> : ConditionalSource<TParameter, TResult>
	{
		public static GuardedFallback<TParameter, TResult> Default { get; } = new GuardedFallback<TParameter, TResult>();
		GuardedFallback() : this(Message<TParameter, TResult>.Default) {}

		public GuardedFallback(IMessage<TParameter> message)
			: base(new AssignedInstanceGuard<TParameter>(NeverSpecification<TParameter>.Default, message),
			       DefaultValueSource<TParameter, TResult>.Default) {}
	}

	sealed class DefaultValueSource<TParameter, TResult> : FixedInstanceSource<TParameter, TResult>
	{
		public static DefaultValueSource<TParameter, TResult> Default { get; } = new DefaultValueSource<TParameter, TResult>();
		DefaultValueSource() : base(default(TResult)) {}
	}

	/*sealed class AssignedInstanceGuard<T> : GuardedSpecification<T, InvalidOperationException>
	{
		public static IParameterizedSource<string, AssignedInstanceGuard<T>> Defaults { get; }
			= new ReferenceCache<string, AssignedInstanceGuard<T>>(x => new AssignedInstanceGuard<T>(x));

		public static AssignedInstanceGuard<T> Default { get; } = Defaults.Get($"Expected instance of type {typeof(T)} to be assigned.");

		public AssignedInstanceGuard(string message) : this(AssignedSpecification<T>.Default, message) {}
		public AssignedInstanceGuard(ISpecification<T> specification, string message) : this(specification, new InvalidOperationException(message)) {}
		public AssignedInstanceGuard(ISpecification<T> specification, InvalidOperationException exception) : base(specification, exception) {}
	}*/

	sealed class Message<TParameter, TResult> : DelegatedSource<TParameter, string>, IMessage<TParameter>
	{
		public static Message<TParameter, TResult> Default { get; } = new Message<TParameter, TResult>();
		Message() : this(x => $"Expected instance of type {typeof(TResult)} to be assigned, but an operation using an instance of {x.GetType()} did not produce this.") {}

		public Message(Func<TParameter, string> source) : base(source) {}
	}

	public interface IValueSource<in TParameter, out TResult> : IParameterizedSource<TParameter, TResult>, IEnumerable<TResult> {}

	public class TableValueSource<TParameter, TResult> : ValueSource<TParameter, TResult>, ISpecificationSource<TParameter, TResult>
	{
		readonly ISpecification<TParameter> _source;

		public TableValueSource() : this(new Dictionary<TParameter, TResult>()) {}

		public TableValueSource(IDictionary<TParameter, TResult> store)
			: this(new TableSource<TParameter, TResult>(store), new Values<TParameter, TResult>(store)) { }

		public TableValueSource(Func<TParameter, TResult> select)
			: this(@select, new ConcurrentDictionary<TParameter, TResult>()) {}

		public TableValueSource(Func<TParameter, TResult> select, ConcurrentDictionary<TParameter, TResult> store)
			: this(new Cache<TParameter, TResult>(select, store), new Values<TParameter, TResult>(store)) {}

		public TableValueSource(ISpecificationSource<TParameter, TResult> source, IEnumerable<TResult> items)
			: base(source, items) => _source = source;

		public bool IsSatisfiedBy(TParameter parameter) => _source.IsSatisfiedBy(parameter);
	}

	public class ValueSource<TParameter, TResult> : IValueSource<TParameter, TResult>
	{
		readonly IParameterizedSource<TParameter, TResult> _source;
		readonly IEnumerable<TResult> _items;

		public ValueSource(IParameterizedSource<TParameter, TResult> source, IEnumerable<TResult> items)
		{
			_source = source;
			_items = items;
		}

		public TResult Get(TParameter parameter) => _source.Get(parameter);

		public IEnumerator<TResult> GetEnumerator() => _items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}