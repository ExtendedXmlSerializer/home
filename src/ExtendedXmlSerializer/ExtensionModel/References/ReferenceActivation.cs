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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Properties;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceActivation : IActivation
	{
		readonly IActivation _activation;
		readonly IEntities _entities;
		readonly IReferenceMaps _maps;

		[UsedImplicitly]
		public ReferenceActivation(IActivation activation, IEntities entities)
			: this(activation, entities, ReferenceMaps.Default) {}

		public ReferenceActivation(IActivation activation, IEntities entities, IReferenceMaps maps)
		{
			_activation = activation;
			_entities = entities;
			_maps = maps;
		}

		public IContentReader Get(TypeInfo parameter) => new Activator(_activation.Get(parameter), _entities, _maps);

		sealed class Activator : IContentReader
		{
			readonly IContentReader _activator;
			readonly IEntities _entities;
			readonly IReferenceMaps _maps;

			public Activator(IContentReader activator, IEntities entities, IReferenceMaps maps)
			{
				_activator = activator;
				_entities = entities;
				_maps = maps;
			}

			static ReferenceIdentity? Identity(IReader reader)
			{
				var identity = IdentityIdentity.Default.Get(reader);
				var result = identity.HasValue ? new ReferenceIdentity(identity.Value) : (ReferenceIdentity?) null;
				return result;
			}

			ReferenceIdentity? Entity(IReader reader, object instance)
			{
				var typeInfo = instance.GetType().GetTypeInfo();
				var entity = _entities.Get(typeInfo)?.Get(reader);
				var result = entity != null
					? (ReferenceIdentity?) new ReferenceIdentity(typeInfo, entity)
					: null;
				return result;
			}


			public object Get(IReader parameter)
			{
				var declared = Identity(parameter);
				var result = _activator.Get(parameter);

				var identity = declared ?? Entity(parameter, result);

				if (identity != null)
				{
					_maps.Get(parameter).Assign(identity.Value, result);
				}
				return result;
			}
		}
	}
}