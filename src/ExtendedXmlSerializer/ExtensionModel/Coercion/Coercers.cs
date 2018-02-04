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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	sealed class Coercers : ICoercers
	{
		readonly IEnumerable<ICoercer> _coercers;

		public Coercers(IEnumerable<ICoercer> coercers) => _coercers = coercers;

		public ICoercion Get(object parameter)
		{
			var candidates = Yield(parameter)
				.ToImmutableArray();
			var result = candidates.Any() ? new Context(parameter, candidates) : null;
			return result;
		}

		IEnumerable<ICoercer> Yield(object parameter)
		{
			foreach (var candidate in _coercers)
			{
				if (candidate.IsSatisfiedBy(parameter))
				{
					yield return candidate;
				}
			}
		}

		sealed class Context : AnySpecification<TypeInfo>, ICoercion
		{
			readonly object _instance;
			readonly ImmutableArray<ICoercer> _candidates;

			public Context(object instance, ImmutableArray<ICoercer> candidates)
				: base(candidates.ToArray<ISpecification<TypeInfo>>())
			{
				_instance = instance;
				_candidates = candidates;
			}

			public object Get(TypeInfo parameter)
			{
				foreach (var candidate in _candidates)
				{
					if (candidate.IsSatisfiedBy(parameter))
					{
						return candidate.Get(new CoercerParameter(_instance, parameter));
					}
				}

				return null;
			}
		}
	}
}