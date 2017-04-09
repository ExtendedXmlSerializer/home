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
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class Entity : IEntity
	{
		readonly static EntityIdentity EntityIdentity = EntityIdentity.Default;

		readonly IConverter _converter;
		readonly IMemberSerializer _member;

		public Entity(IConverter converter, IMemberSerializer member)
		{
			_converter = converter;
			_member = member;
		}

		public string Get(object parameter) => _converter.Format(_member.Access.Get(parameter));

		public object Get(IReader parameter)
		{
			var contains = parameter.IsSatisfiedBy(_member.Profile);
			if (contains)
			{
				var result = _member.Get(parameter);
				parameter.Set();
				return result;
			}
			return null;
		}

		public object Reference(IReader parameter)
			=> parameter.IsSatisfiedBy(EntityIdentity) ? _converter.Parse(parameter.Content()) : null;
	}
}