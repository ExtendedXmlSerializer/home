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

using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class Activation<T> : IActivation<T>
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IClassification _classification;
		readonly IActivators<T> _activators;

		public Activation(IClassification classification, IActivators<T> activators)
			: this(ReflectionModel.VariableTypeSpecification.Default, classification, activators) { }

		public Activation(ISpecification<TypeInfo> specification, IClassification classification, IActivators<T> activators)
		{
			_specification = specification;
			_classification = classification;
			_activators = activators;
		}

		public IContentReader<T> Get()
		{
			var activate = new DelegatedActivator(_activators.Get(typeof(T))
															 .Get);
			var result = _specification.IsSatisfiedBy(Support<T>.Key)
							 ? (IContentReader<T>)new RuntimeActivator(_activators, _classification, activate)
							 : activate;
			return result;
		}

		sealed class RuntimeActivator : IContentReader<T>
		{
			readonly IActivators<T> _activators;
			readonly IClassification _classification;
			readonly IContentReader<T> _activator;

			public RuntimeActivator(IActivators<T> activators, IClassification classification, IContentReader<T> activator)
			{
				_activators = activators;
				_classification = classification;
				_activator = activator;
			}

			public T Get(IFormatReader parameter)
			{
				if (parameter.IsSatisfiedBy(ExplicitTypeProperty.Default))
				{
					var classification = _classification.Get(parameter);
					if (classification != null)
					{
						return _activators.Get(classification.AsType())
										  .Get();
					}
				}
				var result = _activator.Get(parameter);
				return result;
			}
		}

		sealed class DelegatedActivator : IContentReader<T>
		{
			readonly Func<T> _activate;

			public DelegatedActivator(Func<T> activate) => _activate = activate;

			public T Get(IFormatReader _) => _activate();
		}
	}


	sealed class Activation : IActivation
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IClassification _classification;
		readonly IActivators _activators;

		public Activation(IClassification classification, IActivators activators)
			: this(ReflectionModel.VariableTypeSpecification.Default, classification, activators) {}

		public Activation(ISpecification<TypeInfo> specification, IClassification classification, IActivators activators)
		{
			_specification = specification;
			_classification = classification;
			_activators = activators;
		}

		public IReader Get(TypeInfo parameter)
		{
			var activate = new DelegatedActivator(_activators.Get(parameter.AsType())
			                                                 .Get);
			var result = _specification.IsSatisfiedBy(parameter)
				             ? (IReader) new RuntimeActivator(_activators, _classification, activate)
				             : activate;
			return result;
		}

		sealed class RuntimeActivator : IReader
		{
			readonly IActivators _activators;
			readonly IClassification _classification;
			readonly IReader _activator;

			public RuntimeActivator(IActivators activators, IClassification classification, IReader activator)
			{
				_activators = activators;
				_classification = classification;
				_activator = activator;
			}

			public object Get(IFormatReader parameter)
			{
				if (parameter.IsSatisfiedBy(ExplicitTypeProperty.Default))
				{
					var classification = _classification.Get(parameter);
					if (classification != null)
					{
						return _activators.Get(classification.AsType())
						                  .Get();
					}
				}
				var result = _activator.Get(parameter);
				return result;
			}
		}

		sealed class DelegatedActivator : IReader
		{
			readonly Func<object> _activate;

			public DelegatedActivator(Func<object> activate) => _activate = activate;

			public object Get(IFormatReader parameter) => _activate();
		}
	}
}