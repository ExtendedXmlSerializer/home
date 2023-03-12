using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceAwareSerializers : ISerializers
	{
		readonly ISpecification<object>        _conditions;
		readonly IStaticReferenceSpecification _specification;
		readonly IReferenceView                _references;
		readonly ISerializers                  _serializers;
		readonly IMultipleReferencesAllowed    _allowed;

		// ReSharper disable once TooManyDependencies
		public ReferenceAwareSerializers(IStaticReferenceSpecification specification, IReferenceView references,
		                                 ISerializers serializers, IMultipleReferencesAllowed allowed)
			: this(new InstanceConditionalSpecification(), specification, references, serializers, allowed) {}

		// ReSharper disable once TooManyDependencies
		public ReferenceAwareSerializers(ISpecification<object> conditions, IStaticReferenceSpecification specification,
		                                 IReferenceView references, ISerializers serializers,
		                                 IMultipleReferencesAllowed allowed)
		{
			_conditions    = conditions;
			_specification = specification;
			_references    = references;
			_serializers   = serializers;
			_allowed       = allowed;
		}

		public ContentModel.ISerializer Get(TypeInfo parameter)
		{
			var serializer = _serializers.Get(parameter);
			var result = _specification.IsSatisfiedBy(parameter)
				             ? new Serializer(_conditions, _references, _allowed, serializer)
				             : serializer;
			return result;
		}

		sealed class Serializer : ContentModel.ISerializer
		{
			readonly ISpecification<object>     _conditions;
			readonly IReferenceView             _references;
			readonly ContentModel.ISerializer   _container;
			readonly IMultipleReferencesAllowed _allowed;

			// ReSharper disable once TooManyDependencies
			public Serializer(ISpecification<object> conditions, IReferenceView references,
			                  IMultipleReferencesAllowed allowed, ContentModel.ISerializer container)
			{
				_conditions = conditions;
				_references = references;
				_container  = container;
				_allowed    = allowed;
			}

			public object Get(IFormatReader parameter) => _container.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				if (_conditions.IsSatisfiedBy(writer.Get()))
				{
					var references = _references.Get(instance);
					if (references.Cyclical.Any())
					{
						var type = instance.GetType();
						var line = Environment.NewLine;
						var message =
							$"{line}{line}Here is a list of found references:{line}{string.Join(line, references.Cyclical.Select(x => $"- {x}"))}";

						throw new CircularReferencesDetectedException(
						                                              $"The provided instance of type '{type}' contains circular references within its graph. Serializing this instance would result in a recursive, endless loop. To properly serialize this instance, please create a serializer that has referential support enabled by extending it with the ReferencesExtension using the EnableReferences extension method on the ConfigurationContainer.{message}",
						                                              _container);
					}

					if (references.Encountered.Any() && !_allowed.Allowed)
					{
						var type = instance.GetType();
						var line = Environment.NewLine;
						var message =
							$"{line}{line}Here is a list of found references:{line}{string.Join(line, references.Encountered.Select(x => $"- {x}"))}";

						throw new MultipleReferencesDetectedException(
						                                              $"The provided instance of type '{type}' contains the same reference multiple times in its graph. While this is technically allowed, it is recommended to instead enable referential support by calling EnableReferences on the ConfigurationContainer. Doing so will ensure that multiple references found in the graph are emitted only once in the serialized document. Additionally, if you do want to allow multiple instances emitted as-is, make use of the `AllowMultipleReferences` method on the ConfigurationContainer.{message}",
						                                              _container);
					}
				}

				_container.Write(writer, instance);
			}
		}
	}
}