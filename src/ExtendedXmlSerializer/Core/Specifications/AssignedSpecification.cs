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

using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class AssignedArgumentGuard<T> : GuardedSpecification<T, ArgumentNullException>
	{
		public static AssignedArgumentGuard<T> Default { get; } = new AssignedArgumentGuard<T>();
		AssignedArgumentGuard() : this(AssignedSpecification<T>.Default, new ArgumentNullException($"Argument of type {typeof(T)} was not assigned.")) {}

		public AssignedArgumentGuard(ISpecification<T> specification, ArgumentNullException exception)
			: base(specification, exception.Accept) {}
	}

	sealed class AssignedInstanceGuard<T> : GuardedSpecification<T, InvalidOperationException>
	{
		/*public static IParameterizedSource<IExceptionMessage<T>, AssignedInstanceGuard<T>> Defaults { get; }
		= new ReferenceCache<IExceptionMessage<T>, AssignedInstanceGuard<T>>(x => new AssignedInstanceGuard<T>(x));*/

		/*public AssignedInstanceGuard(IExceptionMessage<T> message) : this(AssignedSpecification<T>.Default, message) {}*/

		public AssignedInstanceGuard(ISpecification<T> specification, IMessage<T> message)
			: this(specification, Exceptions.From(x => new InvalidOperationException(x)).In(message).Get) {}

		public AssignedInstanceGuard(ISpecification<T> specification, Func<T, InvalidOperationException> exception) : base(specification, exception) {}
	}

	public static class Exceptions
	{
		public static Exceptions<T> From<T>(Func<string, T> create) where T : Exception => new Exceptions<T>(create);
	}

	public sealed class Exceptions<T> : DelegatedSource<string, T> where T : Exception
	{
		public Exceptions(Func<string, T> source) : base(source) {}
	}

	public interface IMessage<in T> : IParameterizedSource<T, string>{}

	class GuardedSpecification<T, TException> : ISpecification<T> where TException : Exception
	{
		readonly ISpecification<T> _specification;
		readonly Func<T, TException> _exception;

		public GuardedSpecification(ISpecification<T> specification, Func<T, TException> exception)
		{
			_specification = specification;
			_exception = exception;
		}

		public bool IsSatisfiedBy(T parameter)
		{
			if (!_specification.IsSatisfiedBy(parameter))
			{
				throw _exception(parameter);
			}
			return true;
		}
	}

	sealed class AssignedSpecification<T> : ISpecification<T>
	{
		public static AssignedSpecification<T> Default { get; } = new AssignedSpecification<T>();
		AssignedSpecification() {}

		public bool IsSatisfiedBy(T parameter) => parameter != null;
	}
}