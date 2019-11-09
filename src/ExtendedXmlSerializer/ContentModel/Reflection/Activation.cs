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

		public Activation(ISpecification<TypeInfo> specification, IClassification classification,
		                  IActivators activators)
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

			// ReSharper disable once TooManyDependencies
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
				var isSatisfiedBy  = _specification.IsSatisfiedBy(classification);
				var result = isSatisfiedBy
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