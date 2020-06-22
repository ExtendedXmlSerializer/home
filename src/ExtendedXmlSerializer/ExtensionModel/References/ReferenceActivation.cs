using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using JetBrains.Annotations;
using System;
using System.Reflection;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceActivation : IActivation
	{
		readonly Func<TypeInfo, IReader> _activation;
		readonly IEntities               _entities;
		readonly IReferenceMaps          _maps;
		
		[UsedImplicitly]
		public ReferenceActivation(IActivation activation, IEntities entities)
			: this(activation.Get, entities) {}

		public ReferenceActivation(Func<TypeInfo, IReader> activation, IEntities entities)
			: this(activation, entities, ReferenceMaps.Default) {}

		// ReSharper disable once TooManyDependencies
		public ReferenceActivation(Func<TypeInfo, IReader> activation, IEntities entities, IReferenceMaps maps)
		{
			_activation = activation;
			_entities   = entities;
			_maps       = maps;
		}

		public IReader Get(TypeInfo parameter) => new Activator(_activation(parameter), _entities, _maps);

		sealed class Activator : IReader
		{
			readonly IReader        _activator;
			readonly IEntities      _entities;
			readonly IReferenceMaps _maps;

			// ReSharper disable once TooManyDependencies
			public Activator(IReader activator, IEntities entities, IReferenceMaps maps)
			{
				_activator = activator;
				_entities  = entities;
				_maps      = maps;
			}

			static ReferenceIdentity? Identity(IFormatReader reader)
			{
				var identity = IdentityProperty.Default.Get(reader);
				var result   = identity.HasValue ? new ReferenceIdentity(identity.Value) : (ReferenceIdentity?)null;
				return result;
			}

			ReferenceIdentity? Entity(IFormatReader reader, object instance)
			{
				var typeInfo = instance.GetType()
				                       .GetTypeInfo();
				var entity = _entities.Get(typeInfo)
				                      ?.Get(reader);
				var result = entity != null
					             ? (ReferenceIdentity?)new ReferenceIdentity(typeInfo, entity)
					             : null;
				return result;
			}

			public object Get(IFormatReader parameter)
			{
				var element =  parameter.Get().To<XmlReader>().NodeType != XmlNodeType.Attribute || MemberProperty.Default.Get(parameter);

				var declared = element ? Identity(parameter) : null;
				var result   = _activator.Get(parameter);

				var identity = declared ?? (element && result != null ? Entity(parameter, result) : null);

				if (identity != null)
				{
					_maps.Get(parameter)
					     .Assign(identity.Value, result);
				}

				return result;
			}
		}
	}
}