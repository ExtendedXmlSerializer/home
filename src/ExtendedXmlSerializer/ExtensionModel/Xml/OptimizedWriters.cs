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
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class OptimizedWriters : IFormatWriters<System.Xml.XmlWriter>
	{
		readonly ISpecification<object> _specification;
		readonly IFormatWriters<System.Xml.XmlWriter> _factory;
		readonly IObjectIdentifiers _identifiers;
		readonly IRootInstances _instances;

		public OptimizedWriters(IFormatWriters<System.Xml.XmlWriter> factory, IObjectIdentifiers identifiers, IRootInstances instances)
			: this(new FirstInvocationByParameterSpecification<object>(), factory, identifiers, instances) {}

		public OptimizedWriters(ISpecification<object> specification, IFormatWriters<System.Xml.XmlWriter> factory, IObjectIdentifiers identifiers, IRootInstances instances)
		{
			_specification = specification;
			_factory = factory;
			_identifiers = identifiers;
			_instances = instances;
		}

		public IFormatWriter Get(System.Xml.XmlWriter parameter)
			=> new OptimizedNamespaceXmlWriter(_factory.Get(parameter),
			                                   new IdentifierCommand(_identifiers,
			                                                         _specification.Build(parameter),
			                                                         _instances.Build(parameter)));
	}
}