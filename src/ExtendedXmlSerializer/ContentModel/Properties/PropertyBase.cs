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

using System.Xml.Linq;
using ExtendedXmlSerialization.ContentModel.Xml;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	abstract class PropertyBase<T> : IProperty<T>
	{
		protected PropertyBase(string displayName) : this(XName.Get(displayName, Defaults.Namespace)) {}

		protected PropertyBase(XName name)
		{
			Name = name;
		}

		public XName Name { get; }

		public virtual T Get(IXmlReader parameter) => Parse(parameter, parameter[Name]);
		protected abstract T Parse(IXmlReader parameter, string data);

		public virtual void Write(IXmlWriter writer, T instance)
		{
			var format = Format(writer, instance);
			writer.Attribute(Name, format);
		}

		protected abstract string Format(IXmlWriter writer, T instance);
	}
}