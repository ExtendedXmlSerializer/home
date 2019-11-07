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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using System;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public static class Extensions
	{
		public static IConfigurationContainer AllowExistingInstances(this IConfigurationContainer @this)
		{
			@this.Root.With<ExistingInstanceExtension>();
			return @this;
		}

		public static IConfigurationContainer EnableThreadProtection(this IConfigurationContainer @this)
			=> @this.Extend(ThreadProtectionExtension.Default);

		public static IConfigurationContainer EnableMemberExceptionHandling(this IConfigurationContainer @this)
			=> @this.Extend(MemberExceptionHandlingExtension.Default);

		[Obsolete(
			"This method is being deprecated.  Please use ConfigurationContainer.WithUnknownContent.Call instead.")]
		public static IConfigurationContainer EnableUnknownContentHandling(this IConfigurationContainer @this,
		                                                                   Action<IFormatReader> onMissing)
			=> @this.WithUnknownContent().Call(onMissing);

		/// <summary>
		/// https://github.com/WojciechNagorski/ExtendedXmlSerializer/issues/271#issuecomment-550976753
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static UnknownContentContext WithUnknownContent(this IConfigurationContainer @this)
			=> new UnknownContentContext(@this);

		public static T EnableRootInstances<T>(this T @this) where T : IRootContext
		{
			@this.Root.With<RootInstanceExtension>();
			return @this;
		}
	}
}