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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class TypePartsEqualityComparer : IEqualityComparer<TypeParts>
	{
		readonly static IdentityComparer<TypeParts> Comparer = IdentityComparer<TypeParts>.Default;

		public static TypePartsEqualityComparer Default { get; } = new TypePartsEqualityComparer();

		TypePartsEqualityComparer()
		{
		}

		public bool Equals(TypeParts x, TypeParts y)
		{
			var argumentsX = x.GetArguments()
				.GetValueOrDefault(ImmutableArray<TypeParts>.Empty);
			var argumentsY = y.GetArguments()
				.GetValueOrDefault(ImmutableArray<TypeParts>.Empty);

			var arguments = argumentsX.SequenceEqual(argumentsY, this);
			var identity = Comparer.Equals(x, y);

			var dimensions = x.Dimensions.GetValueOrDefault(ImmutableArray<int>.Empty)
				.SequenceEqual(y.Dimensions.GetValueOrDefault(ImmutableArray<int>.Empty));

			var result = arguments && identity && dimensions;
			return result;
		}

		public int GetHashCode(TypeParts obj)
		{
			unchecked
			{
				return ((obj.Dimensions?.GetHashCode() ?? 0) * 489) ^
				       ((obj.GetArguments()
					         ?.GetHashCode() ?? 0) * 397) ^
				       Comparer.GetHashCode(obj);
			}
		}
	}
}