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

using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class MemberFormatExtension : ISerializerExtension
	{
		public MemberFormatExtension()
			: this(new Dictionary<MemberInfo, IAttributeSpecification>(), new HashSet<MemberInfo>()) {}

		public MemberFormatExtension(IDictionary<MemberInfo, IAttributeSpecification> specifications,
		                             ICollection<MemberInfo> registered)
		{
			Specifications = specifications;
			Registered = registered;
		}

		public IDictionary<MemberInfo, IAttributeSpecification> Specifications { get; }

		public ICollection<MemberInfo> Registered { get; }

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var specification = new MemberConverterSpecification(new ContainsSpecification<MemberInfo>(Registered)
				                                                     .Or(IsDefinedSpecification<XmlAttributeAttribute>.Default)
			);
			var specifications = new ContentModel.Members.AttributeSpecifications(Specifications);
			return parameter.RegisterInstance<IAttributeSpecifications>(specifications)
			                .RegisterInstance<IMemberConverterSpecification>(specification)
			                .Register<IMemberConverters, MemberConverters>();
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}