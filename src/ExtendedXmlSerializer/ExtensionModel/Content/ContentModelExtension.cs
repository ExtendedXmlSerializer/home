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

using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using VariableTypeSpecification = ExtendedXmlSerializer.ReflectionModel.VariableTypeSpecification;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class ElementsExtension : ISerializerExtension
	{
		public static ElementsExtension Default { get; } = new ElementsExtension();
		ElementsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<Elements>()
			            .Register<IElements, Elements>()
			            .RegisterDefinition<IElements<object>, Elements<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	public sealed class VariableTypeElementsExtension : ISerializerExtension
	{
		public static VariableTypeElementsExtension Default { get; } = new VariableTypeElementsExtension();
		VariableTypeElementsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<VariableTypeElement>(VariableTypeSpecification.Default)
			            .DecorateDefinition<IElements<object>, VariableTypeElementsRegistration<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	public sealed class ArrayElementsExtension : ISerializerExtension
	{
		public static ArrayElementsExtension Default { get; } = new ArrayElementsExtension();
		ArrayElementsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<ArrayElement>(IsArraySpecification.Default)
			            .DecorateDefinition<IElements<object>, ArrayElementsRegistration<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
	public sealed class GenericElementsExtension : ISerializerExtension
	{
		public static GenericElementsExtension Default { get; } = new GenericElementsExtension();
		GenericElementsExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<GenericElement>(IsGenericTypeSpecification.Default)
			            .DecorateDefinition<IElements<object>, GenericElementsRegistration<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	public sealed class ContentModelExtension : ISerializerExtension
	{
		public static ContentModelExtension Default { get; } = new ContentModelExtension();
		ContentModelExtension() : this(WellKnownAssemblyIdentities.Default.Values) {}

		readonly ICollection<IIdentity> _namespaces;

		public ContentModelExtension(ICollection<IIdentity> namespaces) => _namespaces = namespaces;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.RegisterInstance<IAliases>(new Aliases(_namespaces))
		                                                                        .Register<IClassification, Classification>()
		                                                                        .Register<IIdentityStore, IdentityStore>();

		void ICommand<IServices>.Execute(IServices parameter)
		{
		}
	}
}