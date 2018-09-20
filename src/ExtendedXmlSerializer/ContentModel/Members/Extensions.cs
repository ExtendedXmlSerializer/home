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

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	static class Extensions
	{
		public static T GetIfAssigned<T>(this IReader<T> @this, IFormatReader reader)
			=> reader.IsAssigned() ? @this.Get(reader) : default(T);

		public static object GetIfAssigned(this IReader @this, IFormatReader reader)
			=> reader.IsAssigned() ? @this.Get(reader) : null;

		public static bool IsAssigned(this IFormatReader @this)
			=> !IdentityComparer.Default.Equals(@this, NullElementIdentity.Default) &&
			   !@this.IsSatisfiedBy(NullValueIdentity.Default);

		public static IConverter Get(this IMemberConverters @this, IMember descriptor)
			=> @this.Get(descriptor.Metadata) ?? @this.Get(descriptor.MemberType);

		public static ISpecification<object> GetInstance(this ISpecification<object> @this)
			=> @this is IInstanceValueSpecification instance ? instance.Instance : AlwaysSpecification<object>.Default;
	}
}