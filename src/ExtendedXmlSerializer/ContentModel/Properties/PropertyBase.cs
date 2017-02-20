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

using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Namespacing;

namespace ExtendedXmlSerialization.ContentModel.Properties
{
	abstract class PropertyBase<T> : Identity, IProperty<T>
	{
		protected PropertyBase(string name, string identifier) : base(name, identifier) {}

		public virtual T Get(IXmlReader parameter) => Parse(parameter, Value(parameter));

		protected virtual string Value(IXmlReader parameter) => parameter.Value();

		protected abstract T Parse(IXmlReader parameter, string data);

		public virtual void Write(IXmlWriter writer, T instance)
		{
			var ns = !string.IsNullOrEmpty(Identifier) ? new Namespace(writer.Get(Identifier), Identifier) : (Namespace?) null;
			var format = Format(writer, instance);
			var attribute = new Attribute(Name, format, ns);
			writer.Attribute(attribute);
		}

		protected abstract string Format(IXmlWriter writer, T instance);
	}
}