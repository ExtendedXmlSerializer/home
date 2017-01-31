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

/*namespace ExtendedXmlSerialization.Conversion
{
 class DictionaryContextOption : ContainerContextOptionBase<IName>
 {
	 readonly IContexts _contexts;
	 readonly IActivators _activators;
	 readonly IAddDelegates _add;

	 public DictionaryContextOption(IContexts contexts, IActivators activators, ICollectionItemTypeLocator locator,
		                              INames names, IAddDelegates add)
		 : this(contexts, new CollectionItemNameProvider(locator, names), activators, add) {}

	 public DictionaryContextOption(IContexts contexts, INameProvider<IName> names, IActivators activators,
		                              IAddDelegates add)
		 : base(IsCollectionTypeSpecification.Default, names)
	 {
		 _contexts = contexts;
		 _activators = activators;
		 _add = add;
	 }

	 protected override IElementContext Create(TypeInfo type, IName name)
	 {
		 throw new System.NotImplementedException();
		 // new EnumerableContext(new CollectionItemContext(_contexts, name), _activators, _add);
	 }
 }
}*/