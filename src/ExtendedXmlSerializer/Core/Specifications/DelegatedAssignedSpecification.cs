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
	class DelegatedAssignedSpecification<TParameter, TResult> : ISpecification<TParameter>
	{
		readonly static Func<TResult, bool> Specification = AssignedSpecification<TResult>.Default.IsSatisfiedBy;

		readonly Func<TParameter, TResult> _source;
		readonly Func<TResult, bool> _specification;

		public DelegatedAssignedSpecification(Func<TParameter, TResult> source) : this(source, Specification) {}

		DelegatedAssignedSpecification(Func<TParameter, TResult> source, Func<TResult, bool> specification)
		{
			_source = source;
			_specification = specification;
		}

		public bool IsSatisfiedBy(TParameter parameter) => _specification(_source(parameter));
	}
}