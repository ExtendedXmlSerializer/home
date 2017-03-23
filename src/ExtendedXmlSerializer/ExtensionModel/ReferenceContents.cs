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
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ReferenceContents : DecoratedSource<TypeInfo, ISerializer>, IContents
	{
		readonly static IsReferenceSpecification Specification = IsReferenceSpecification.Default;

		readonly ISpecification<TypeInfo> _specification;
		readonly IEncounterStore _encounters;
		readonly IEntities _entities;

		public ReferenceContents(IEncounterStore encounters, IEntities entities, IContents option)
			: this(Specification, encounters, entities, option) {}

		public ReferenceContents(ISpecification<TypeInfo> specification, IEncounterStore encounters,
		                         IEntities entities, IContents option) : base(option)
		{
			_encounters = encounters;
			_entities = entities;
			_specification = specification;
		}

		public override ISerializer Get(TypeInfo parameter)
		{
			var serializer = base.Get(parameter);
			var result = serializer as RuntimeSerializer ??
			             (_specification.IsSatisfiedBy(parameter)
				             ? new ReferenceSerializer(_encounters, new References(serializer, _entities, parameter), serializer)
				             : serializer);
			return result;
		}
	}
}