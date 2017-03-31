// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Coercion;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	sealed class Evaluation : DecoratedOption<TypeInfo, object>, IEvaluation
	{
		readonly ISource<Exception> _exception;

		public Evaluation(ICoercers coercers, string expression, object instance)
			: this(new Implementation(coercers, expression, instance)) {}

		public Evaluation(ICoercers coercers, string expression, Exception error)
			: this(new Implementation(coercers, expression, error)) {}

		Evaluation(Implementation implementation) : this(implementation, implementation) {}

		Evaluation(ISource<Exception> exception, IParameterizedSource<TypeInfo, object> implementation) : base(
			new DelegatedAssignedSpecification<TypeInfo, object>(implementation.Get),
			implementation)
		{
			_exception = exception;
		}


		sealed class Implementation : CacheBase<TypeInfo, Implementation.Result?>, IParameterizedSource<TypeInfo, object>,
		                              ISource<Exception>
		{
			readonly ICoercers _coercers;
			readonly string _expression;
			readonly object _instance;
			readonly Exception _error;

			public Implementation(ICoercers coercers, string expression, object instance)
				: this(coercers, expression, instance, null) {}

			public Implementation(ICoercers coercers, string expression, Exception error)
				: this(coercers, expression, null, error) {}

			Implementation(ICoercers coercers, string expression, object instance, Exception error = null)
			{
				_coercers = coercers;
				_expression = expression;
				_instance = instance;
				_error = error;
			}

			protected override Result? Create(TypeInfo parameter)
			{
				var instance = _error != null ? _expression : _instance;
				if (parameter.IsInstanceOfType(instance))
				{
					return new Result(instance);
				}

				var coercer = _coercers.Get(instance);
				var result =
					(coercer?.IsSatisfiedBy(parameter) ?? false)
						? new Result(coercer.Get(parameter))
						: parameter.IsInstanceOfType(_expression) ? new Result(_expression) : (Result?) null;
				return result;
			}

			object IParameterizedSource<TypeInfo, object>.Get(TypeInfo parameter) => Get(parameter)?.Instance;
			public Exception Get() => _error;

			public struct Result
			{
				public Result(object instance)
				{
					Instance = instance;
				}

				public object Instance { get; }
			}
		}

		public Exception Get() => _exception.Get();
	}

	/*struct ParameterCandidate
	{
		public ParameterCandidate(Evaluation evaluation, ICoercion coercer)
		{
			Evaluation = evaluation;
			Coercer = coercer;
		}

		public Evaluation Evaluation { get; }
		public ICoercion Coercer { get; }
	}*/
}