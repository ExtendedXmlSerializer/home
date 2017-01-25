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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface ISelectingConverter : IParameterizedSource<TypeInfo, IConverter>, IParameterizedSource<IContext, IConverter>, IConverter {}

	public abstract class SelectingConverterBase : ConverterBase
	{
		public override void Write(IWriteContext context, object instance) => Select(context).Write(context, instance);

		protected abstract IConverter Select(IContext context);

		public override object Read(IReadContext context) => Select(context).Read(context);
	}

	public class SelectingConverter : SelectingConverterBase, ISelectingConverter
	{
		readonly IParameterizedSource<TypeInfo, IConverter> _selector;

		public SelectingConverter(IParameterizedSource<ISelectingConverter, IEnumerable<IConverterOption>> options)
		{
			_selector = new Selector<TypeInfo, IConverter>(options.Get(this).ToArray());
		}

		public IConverter Get(TypeInfo parameter) => _selector.Get(parameter);

		protected override IConverter Select(IContext context) => Get(context);

		public IConverter Get(IContext parameter)
		{
			return _selector.Get(parameter.Element.Classification) ??
			       _selector.Get(parameter.Element.GetType().GetTypeInfo());
		}
	}
}