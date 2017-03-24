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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class TypeModelExtension : ISerializerExtension
	{
		public static TypeModelExtension Default { get; } = new TypeModelExtension();
		TypeModelExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<ISortOrder, SortOrder>()
			            .Register<IActivation, Activation>()
			            .Register<IActivators, Activators>()
			            .Register<IActivatingTypeSpecification, ActivatingTypeSpecification>()
			            .Register<IConstructorLocator, ConstructorLocator>()
			            .Register<IEnumeratorStore, EnumeratorStore>()
			            .RegisterInstance<ICollectionAssignment>(CollectionAssignment.Default)
			            .RegisterInstance<IDictionaryEnumerators>(DictionaryEnumerators.Default)
			            .RegisterInstance<IEnumerators>(Enumerators.Default)
			            .RegisterInstance<IConstructors>(Constructors.Default)
			            .RegisterInstance<IFields>(Fields.Default)
			            .RegisterInstance<IProperties>(Properties.Default)
			            .RegisterInstance<IValidConstructorSpecification>(ValidConstructorSpecification.Default);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}