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

using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ClassicContentOptions : IContentOptions
	{
		readonly ISerialization _owner;
		readonly IMembers _members;
		readonly IMemberOption _variable;
		readonly ISerializer _runtime;
		readonly IActivation _activation;
		readonly IMemberSerialization _memberSerialization;

		public ClassicContentOptions(
			IActivation activation,
			ISerialization owner,
			IMemberSerialization memberSerialization,
			IMembers members,
			IMemberOption variable, ISerializer runtime)
		{
			_owner = owner;
			_memberSerialization = memberSerialization;
			_members = members;
			_variable = variable;
			_runtime = runtime;
			_activation = activation;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<IContentOption> GetEnumerator()
		{
			yield return new ArrayContentOption(_activation, _owner);
			var entries = new DictionaryEntries(_activation, _memberSerialization, _variable);
			yield return new ClassicDictionaryContentOption(_activation, _members, entries);
			yield return new ClassicCollectionContentOption(_activation, _owner);
			yield return new MemberedContentOption(_activation, _members);
			yield return new RuntimeContentOption(_runtime);
		}
	}
}