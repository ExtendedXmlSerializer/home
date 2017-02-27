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
	class Services : IEnumerable<object>
	{
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;
		readonly IActivation _activation;
		readonly IXmlFactory _xmlFactory;
		readonly IMemberConfiguration _memberConfiguration;
		readonly IMemberWriters _writers;
		readonly IElementOptionSelector _selector;
		readonly IContentOption _content;

		public Services(IActivation activation, IMemberConfiguration configuration, IContentOption content,
		                IXmlFactory xmlFactory)
			: this(
				configuration.Policy.And<PropertyInfo>(configuration.Specification),
				configuration.Policy.And<FieldInfo>(configuration.Specification),
				activation, xmlFactory, configuration,
				new MemberWriters(new RuntimeMemberSpecifications(configuration.Runtime),
				                  new MemberConverters(configuration.Converters)
				), ElementOptionSelector.Default, content) {}

		public Services(
			ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field,
			IActivation activation, IXmlFactory xmlFactory, IMemberConfiguration memberConfiguration,
			IMemberWriters writers, IElementOptionSelector selector, IContentOption content)
		{
			_property = property;
			_field = field;
			_activation = activation;
			_xmlFactory = xmlFactory;
			_memberConfiguration = memberConfiguration;


			_writers = writers;
			_selector = selector;
			_content = content;
		}

		public IEnumerator<object> GetEnumerator()
		{
			yield return _activation;
			yield return _property;
			yield return _field;

			yield return _memberConfiguration;
			yield return _memberConfiguration.EmitSpecifications;
			yield return _memberConfiguration.Aliases;
			yield return _memberConfiguration.Order;

			yield return _writers;
			yield return _xmlFactory;

			yield return _content;
			yield return _selector;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}