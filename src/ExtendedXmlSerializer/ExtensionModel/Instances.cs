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
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class Instances : IEnumerable<object>
	{
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;
		readonly IActivation _activation;
		readonly IXmlFactory _xmlFactory;
		readonly IMemberConfiguration _memberConfiguration;
		readonly IMemberSerializers _serializers;
		readonly IElementOptionSelector _selector;
		readonly IContentOption _content;
		readonly object[] _additional;

		public Instances(IActivation activation, IMemberConfiguration configuration, IContentOption content,
		                 IXmlFactory xmlFactory, params object[] additional)
			: this(
				configuration.Policy.And<PropertyInfo>(configuration.Specification),
				configuration.Policy.And<FieldInfo>(configuration.Specification),
				activation, xmlFactory, configuration,
				new MemberSerializers(configuration.Runtime, configuration.Converters),
				ElementOptionSelector.Default, content, additional) {}

		public Instances(
			ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field,
			IActivation activation, IXmlFactory xmlFactory, IMemberConfiguration memberConfiguration,
			IMemberSerializers serializers, IElementOptionSelector selector, IContentOption content, params object[] additional)
		{
			_property = property;
			_field = field;
			_activation = activation;
			_xmlFactory = xmlFactory;
			_memberConfiguration = memberConfiguration;


			_serializers = serializers;
			_selector = selector;
			_content = content;
			_additional = additional;
		}

		public IEnumerator<object> GetEnumerator()
		{
			yield return _activation;
			yield return _property;
			yield return _field;

			yield return _memberConfiguration;
			yield return _memberConfiguration.Converters;
			yield return _memberConfiguration.Runtime;
			yield return _memberConfiguration.Specification;
			yield return _memberConfiguration.Aliases;
			yield return _memberConfiguration.Order;

			yield return _serializers;
			yield return _xmlFactory;

			yield return _content;
			yield return _selector;
			foreach (var o in _additional)
			{
				yield return o;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}