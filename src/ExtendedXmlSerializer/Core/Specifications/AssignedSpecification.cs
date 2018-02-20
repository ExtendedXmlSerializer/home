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

using System;

namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class AssignedGuardSpecification<T> : GuardedSpecification<T, ArgumentNullException>
	{
		public static AssignedGuardSpecification<T> Default { get; } = new AssignedGuardSpecification<T>();
		AssignedGuardSpecification() : this(AssignedSpecification<T>.Default, new ArgumentNullException($"Argument of type {typeof(T)} was not assigned.")) {}

		public AssignedGuardSpecification(ISpecification<T> specification, ArgumentNullException exception) : base(specification, exception) {}
	}

	class GuardedSpecification<T, TException> : ISpecification<T> where TException : Exception
	{
		readonly ISpecification<T> _specification;
		readonly TException _exception;

		public GuardedSpecification(ISpecification<T> specification, TException exception)
		{
			_specification = specification;
			_exception = exception;
		}

		public bool IsSatisfiedBy(T parameter)
		{
			if (!_specification.IsSatisfiedBy(parameter))
			{
				throw _exception;
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