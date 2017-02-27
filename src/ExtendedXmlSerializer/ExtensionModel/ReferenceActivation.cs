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
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ReferenceActivation : IActivation
	{
		readonly IActivation _activation;
		readonly IReferenceIdentities _identities;
		readonly IEntities _entities;

		public ReferenceActivation(IActivation activation, IEntities entities)
			: this(activation, entities, ReferenceIdentities.Default) {}

		public ReferenceActivation(IActivation activation, IEntities entities, IReferenceIdentities identities)
		{
			_activation = activation;
			_identities = identities;
			_entities = entities;
		}

		public IReader Get(TypeInfo parameter) => new Activator(_activation.Get(parameter), _entities, _identities);

		class Activator : IReader
		{
			readonly IReader _reader;
			readonly IEntities _entities;
			readonly IReferenceIdentities _identities;

			public Activator(IReader reader, IEntities entities, IReferenceIdentities identities)
			{
				_reader = reader;
				_entities = entities;
				_identities = identities;
			}

			static ReferenceIdentity? Identity(IXmlReader reader) =>
				reader.Contains(IdentityProperty.Default)
					? (ReferenceIdentity?) new ReferenceIdentity(Defaults.Reference, IdentityProperty.Default.Get(reader))
					: null;

			ReferenceIdentity? Entity(object instance)
			{
				var typeInfo = instance.GetType().GetTypeInfo();
				var entity = _entities.Get(typeInfo);
				var result = entity != null
					? (ReferenceIdentity?) new ReferenceIdentity(Defaults.Reference, entity.Get(instance))
					: null;
				return result;
			}


			public object Get(IXmlReader parameter)
			{
				var declared = Identity(parameter);
				var result = _reader.Get(parameter);

				var identity = declared ?? Entity(result);

				if (identity != null)
				{
					_identities.Get(parameter).Add(identity.Value, result);
				}
				return result;
			}
		}
	}
}