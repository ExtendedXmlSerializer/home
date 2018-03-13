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
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class Activation : IActivation
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly IClassification          _classification;
		readonly IActivators              _activators;

		public Activation(IClassification classification, IActivators activators)
			: this(ReflectionModel.VariableTypeSpecification.Default, classification, activators) {}

		public Activation(ISpecification<TypeInfo> specification, IClassification classification, IActivators activators)
		{
			_specification  = specification;
			_classification = classification;
			_activators     = activators;
		}

		public IReader Get(TypeInfo parameter)
		{
			var activate = new DelegatedActivator(_activators.Get(parameter.AsType())
			                                                 .Get);
			var result = _specification.IsSatisfiedBy(parameter)
				             ? (IReader)new RuntimeActivator(_activators, _classification, activate)
				             : activate;
			return result;
		}

		sealed class RuntimeActivator : IReader
		{
			readonly static ISpecification<TypeInfo> Specification =
				AssignedSpecification<object>.Default.And(IsCollectionTypeSpecification.Default.Inverse());

			readonly IActivators              _activators;
			readonly IClassification          _classification;
			readonly IReader                  _activator;
			readonly ISpecification<TypeInfo> _specification;

			public RuntimeActivator(IActivators activators, IClassification classification, IReader activator)
				: this(activators, classification, activator, Specification) {}

			public RuntimeActivator(IActivators activators, IClassification classification, IReader activator,
			                        ISpecification<TypeInfo> specification)
			{
				_activators     = activators;
				_classification = classification;
				_activator      = activator;
				_specification  = specification;
			}

			public object Get(IFormatReader parameter)
			{
				var classification = _classification.Get(parameter);
				var result = _specification.IsSatisfiedBy(classification)
					             ? _activators.Get(classification)
					                          .Get()
					             : _activator.Get(parameter);
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