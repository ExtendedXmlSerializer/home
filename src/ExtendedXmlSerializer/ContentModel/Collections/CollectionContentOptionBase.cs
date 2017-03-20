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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Collections
{
	abstract class CollectionContentOptionBase : ContentOptionBase
	{
		readonly static CollectionItemTypeLocator Locator = CollectionItemTypeLocator.Default;

		readonly ISerializers _serializers;
		readonly ICollectionItemTypeLocator _locator;

		protected CollectionContentOptionBase(IActivatingTypeSpecification specification, ISerializers serializers)
			: this(specification.And(IsCollectionTypeSpecification.Default), serializers) {}

		protected CollectionContentOptionBase(ISpecification<TypeInfo> specification, ISerializers serializers)
			: this(specification, serializers, Locator) {}

		protected CollectionContentOptionBase(ISpecification<TypeInfo> specification, ISerializers serializers,
		                                      ICollectionItemTypeLocator locator) : base(specification)
		{
			_serializers = serializers;
			_locator = locator;
		}

		public sealed override ISerializer Get(TypeInfo parameter)
			=> Create(_serializers.Get(_locator.Get(parameter)), parameter);

		protected abstract ISerializer Create(ISerializer item, TypeInfo classification);
	}
}