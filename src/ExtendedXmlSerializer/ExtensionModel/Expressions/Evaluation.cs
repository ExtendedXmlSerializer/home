using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Coercion;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	sealed class Evaluation : SpecificationSource<TypeInfo, object>, IEvaluation
	{
		readonly ISource<Exception> _exception;

		public Evaluation(ICoercers coercers, string expression, object instance)
			: this(new Implementation(coercers, expression, instance)) {}

		public Evaluation(ICoercers coercers, string expression, Exception error)
			: this(new Implementation(coercers, expression, error)) {}

		Evaluation(Implementation implementation) : this(implementation, implementation) {}

		Evaluation(ISource<Exception> exception, IParameterizedSource<TypeInfo, object> implementation)
			: base(implementation) => _exception = exception;

		sealed class Implementation
			: CacheBase<TypeInfo, Implementation.Result?>,
			  IParameterizedSource<TypeInfo, object>,
			  ISource<Exception>
		{
			readonly ICoercers _coercers;
			readonly string    _expression;
			readonly object    _instance;
			readonly Exception _error;

			public Implementation(ICoercers coercers, string expression, object instance)
				: this(coercers, expression, instance, null) {}

			public Implementation(ICoercers coercers, string expression, Exception error)
				: this(coercers, expression, null, error) {}

			// ReSharper disable once TooManyDependencies
			Implementation(ICoercers coercers, string expression, object instance, Exception error = null)
			{
				_coercers   = coercers;
				_expression = expression;
				_instance   = instance;
				_error      = error;
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
					coercer?.IsSatisfiedBy(parameter) ?? false
						? new Result(coercer.Get(parameter))
						: parameter.IsInstanceOfType(_expression)
							? new Result(_expression)
							: (Result?)null;
				return result;
			}

			object IParameterizedSource<TypeInfo, object>.Get(TypeInfo parameter) => Get(parameter)
				?.Instance;

			public Exception Get() => _error;

			public readonly struct Result
			{
				public Result(object instance) => Instance = instance;

				public object Instance { get; }
			}
		}

		public Exception Get() => _exception.Get();
	}
}