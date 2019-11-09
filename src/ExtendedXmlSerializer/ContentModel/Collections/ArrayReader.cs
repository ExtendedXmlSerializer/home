using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using System;
using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class ArrayReader : IReader
	{
		readonly IClassification    _classification;
		readonly TypeInfo           _declaredType;
		readonly IReader<ArrayList> _reader;

		// ReSharper disable once TooManyDependencies
		public ArrayReader(IInnerContentServices services, IClassification classification, TypeInfo elementType,
		                   IReader item)
			: this(classification,
			       elementType,
			       services.CreateContents<ArrayList>(new CollectionInnerContentHandler(item, services))) {}

		ArrayReader(IClassification classification, TypeInfo elementType, IReader<ArrayList> reader)
		{
			_classification = classification;
			_declaredType   = elementType;
			_reader         = reader;
		}

		public object Get(IFormatReader parameter)
		{
			var elementType = _classification.GetClassification(parameter, _declaredType)
			                                 .GetElementType();
			var result = _reader.Get(parameter)
			                    .ToArray(elementType ??
			                             throw new InvalidOperationException("Element type not found."));
			return result;
		}
	}
}