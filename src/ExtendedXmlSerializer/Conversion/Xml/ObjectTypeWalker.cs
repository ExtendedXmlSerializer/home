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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class ObjectTypeWalker : ObjectWalkerBase<object, IEnumerable<TypeInfo>>, ISource<IEnumerable<TypeInfo>>
	{
		readonly ConditionMonitor _framework = new ConditionMonitor();

		readonly IElements _elements;
		readonly static TypeInfo FrameworkType = ElementModel.Defaults.FrameworkType;

		public ObjectTypeWalker(IElements elements, object root) : base(root)
		{
			_elements = elements;
		}

		protected override IEnumerable<TypeInfo> Select(object input)
		{
			var parameter = input.GetType().GetTypeInfo();
			yield return parameter;
			
			var element = _elements.Get(parameter);
			var membered = element as IMemberedElement;
			if (membered != null)
			{
				foreach (var member in membered.Members)
				{
					if (Check(member, member.Get(input)))
					{
						yield return FrameworkType;
					}
				}
			}

			var collection = element as ICollectionElement;
			if (collection != null)
			{
				var item = collection as IDictionaryElement;
				if (item != null)
				{
					foreach (DictionaryEntry entry in (IDictionary) input)
					{
						Schedule(entry.Key);
						Schedule(entry.Value);
					}
				}
				else
				{
					foreach (var i in (IEnumerable) input)
					{
						Schedule(i);
					}
				}
			}
		}

		bool Check(IClassification classification, object instance)
		{
			if (!classification.Exact(instance))
			{
				Schedule(instance);
				var result = _framework.Apply();
				return  result;
			}
			return false;
		}

		public IEnumerable<TypeInfo> Get() => this.SelectMany(x => x).Distinct();
	}
}