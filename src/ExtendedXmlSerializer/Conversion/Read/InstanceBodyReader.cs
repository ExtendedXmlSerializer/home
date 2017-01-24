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

using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
	class InstanceBodyReader : DecoratedReader
	{
		readonly IActivators _activators;

		public InstanceBodyReader(IActivators activators, IReader reader) : base(reader)
		{
			_activators = activators;
		}

		public override object Read(IReadContext context)
		{
			var result = Activate(context);
			foreach (var child in context)
			{
				var value = base.Read(child);
				var member = child.Container;
				member.Assign(result, value);
			}
			return result;
		}

		protected virtual object Activate(IReadContext context)
			=> _activators.Activate<object>(context.Element.Classification.AsType());
	}
}