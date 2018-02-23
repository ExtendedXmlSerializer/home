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

using ExtendedXmlSerializer.ExtensionModel.Services;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class AutoMemberFormatExtension : ISerializerExtension
	{
		public static AutoMemberFormatExtension Default { get; } = new AutoMemberFormatExtension();
		AutoMemberFormatExtension() /*: this(128)*/ {}

		/*readonly IAttributeSpecification _text;

		public AutoMemberFormatExtension(int maxTextLength)
			: this(new AttributeSpecification(new TextSpecification(maxTextLength).Adapt())) {}

		public AutoMemberFormatExtension(IAttributeSpecification text) => _text = text;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IMemberConverters, MemberConverters>()
			            .Decorate<IAttributeSpecifications>(Decorate);

		IAttributeSpecifications Decorate(IServiceProvider provider, IAttributeSpecifications defaults)
			=> new AttributeSpecifications(provider.Get<ConverterSpecification>(), _text, defaults);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class TextSpecification : ISpecification<string>
		{
			readonly int _maxTextLength;

			public TextSpecification(int maxTextLength) => _maxTextLength = maxTextLength;

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

			public IConverter Get(MemberInfo parameter)
			{
				var converter = _members.Get(parameter);
				return converter ?? From(parameter);
			}

			IConverter From(MemberDescriptor parameter) => _converters.Get(parameter.MemberType.AccountForNullable());
		}*/
		public IServiceRepository Get(IServiceRepository parameter)
		{
			return null;
		}

		public void Execute(IServices parameter) {}
	}
}