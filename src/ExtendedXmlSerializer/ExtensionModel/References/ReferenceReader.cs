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

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceReader : DecoratedContentReader
	{
		readonly static ContentModel.Properties.ReferenceIdentity ReferenceIdentity =
			ContentModel.Properties.ReferenceIdentity.Default;

		readonly IReferenceMaps _maps;
		readonly IEntities _entities;
		readonly TypeInfo _definition;
		readonly IClassification _classification;

		public ReferenceReader(IContentReader reader, IReferenceMaps maps, IEntities entities, TypeInfo definition,
		                       IClassification classification) : base(reader)
		{
			_maps = maps;
			_entities = entities;
			_definition = definition;
			_classification = classification;
		}

		ReferenceIdentity? GetReference(IContentAdapter parameter)
		{
			var identity = ReferenceIdentity.Get(parameter);
			if (identity.HasValue)
			{
				return new ReferenceIdentity(identity.Value);
			}

			var type = _classification.GetClassification(parameter, _definition);
			var entity = _entities.Get(type)?.Reference(parameter);
			if (entity != null)
			{
				return new ReferenceIdentity(type, entity);
			}
			return null;
		}

		public override object Get(IContentAdapter parameter)
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