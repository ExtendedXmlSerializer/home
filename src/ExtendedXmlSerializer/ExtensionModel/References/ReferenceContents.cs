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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceContents : IContents
	{
		readonly static IsReferenceSpecification Specification = IsReferenceSpecification.Default;

		readonly ISpecification<TypeInfo> _specification;
		readonly ISpecification<TypeInfo> _convertible;
		readonly IReferenceEncounters _identifiers;
		readonly IReferenceMaps _maps;
		readonly IEntities _entities;
		readonly IContents _option;
		readonly IClassification _classification;

		[UsedImplicitly]
		public ReferenceContents(IReferenceEncounters identifiers, IReferenceMaps maps, IEntities entities, IContents option,
		                         IClassification classification, IConverters converters)
			: this(Specification, converters.IfAssigned(), identifiers, maps, entities, option, classification) {}

		public ReferenceContents(ISpecification<TypeInfo> specification, ISpecification<TypeInfo> convertible,
		                         IReferenceEncounters identifiers, IReferenceMaps maps, IEntities entities,
		                         IContents option, IClassification classification)

		{
			_identifiers = identifiers;
			_maps = maps;
			_entities = entities;
			_option = option;
			_classification = classification;
			_specification = specification;
			_convertible = convertible;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var serializer = _option.Get(parameter);
			var result = serializer as RuntimeSerializer ??
			             (_specification.IsSatisfiedBy(parameter) ? Serializer(parameter, serializer) : serializer);
			return result;
		}

		ReferenceSerializer Serializer(TypeInfo parameter, ISerializer serializer)
		{
			var reader = _convertible.IsSatisfiedBy(parameter)
				             ? new ReferenceActivation(serializer.Accept, _entities).Get(parameter)
				             : serializer;
			var referenceReader = new ReferenceReader(reader, _maps, _entities, parameter, _classification);
			var result = new ReferenceSerializer(_identifiers, referenceReader, serializer);
			return result;
		}
	}
}