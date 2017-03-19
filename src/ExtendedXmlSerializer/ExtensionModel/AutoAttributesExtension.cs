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
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ExtensionModel
{
	sealed class AutoAttributesExtension : ISerializerExtension
	{
		public static AutoAttributesExtension Default { get; } = new AutoAttributesExtension();
		AutoAttributesExtension() {}

		readonly IRuntimeMemberSpecification _text;

		public AutoAttributesExtension(int maxTextLength = 128)
			: this(new RuntimeMemberSpecification(new TextSpecification(maxTextLength).Adapt())) {}

		public AutoAttributesExtension(IRuntimeMemberSpecification text)
		{
			_text = text;
		}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<Converters>()
			            .Decorate<IMemberConverters>(
				            (provider, converters) => new MemberConverters(converters, provider.Get<Converters>()))
			            .Decorate<IRuntimeMemberSpecifications>(Decorate);

		IRuntimeMemberSpecifications Decorate(IServiceProvider provider, IRuntimeMemberSpecifications defaults)
			=> new RuntimeMemberSpecifications(_text, defaults, provider.Get<Converters>());

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class TextSpecification : ISpecification<string>
		{
			readonly int _maxTextLength;

			public TextSpecification(int maxTextLength)
			{
				_maxTextLength = maxTextLength;
			}

			public bool IsSatisfiedBy(string parameter) => parameter.Length < _maxTextLength;
		}

		sealed class MemberConverters : IMemberConverters
		{
			readonly IMemberConverters _members;
			readonly IConverters _converters;

			public MemberConverters(IMemberConverters members, IConverters converters)
			{
				_members = members;
				_converters = converters;
			}

			public IConverter Get(MemberInfo parameter) => _members.Get(parameter) ?? From(parameter);

			IConverter From(MemberDescriptor parameter) => _converters.Get(parameter.MemberType);
		}
	}
}