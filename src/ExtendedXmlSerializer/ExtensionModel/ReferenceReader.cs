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
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ReferenceReader : DecoratedReader
	{
		readonly static ReferenceProperty ReferenceProperty = ReferenceProperty.Default;
		readonly static ReferenceMaps ReferenceMaps = ReferenceMaps.Default;

		readonly IReferenceMaps _maps;
		readonly IEntities _entities;
		readonly TypeInfo _definition;

		public ReferenceReader(IReader reader, IEntities entities, TypeInfo definition)
			: this(reader, ReferenceMaps, entities, definition) {}

		public ReferenceReader(IReader reader, IReferenceMaps maps, IEntities entities, TypeInfo definition) : base(reader)
		{
			_maps = maps;
			_entities = entities;
			_definition = definition;
		}

		ReferenceIdentity? GetReference(IXmlReader parameter)
		{
			if (parameter.Contains(ReferenceProperty))
			{
				return new ReferenceIdentity(ReferenceProperty.Get(parameter));
			}

			var type = parameter.Classification ?? _definition;
			var entity = _entities.Get(type)?.Reference(parameter);
			if (entity != null)
			{
				return new ReferenceIdentity(type, entity);
			}
			return null;
		}

		public override object Get(IXmlReader parameter)
		{
			var identity = GetReference(parameter);
			if (identity != null)
			{
				var reference = _maps.Get(parameter).Get(identity.Value);
				return reference;
			}

			var result = base.Get(parameter);
			return result;
		}
	}
}