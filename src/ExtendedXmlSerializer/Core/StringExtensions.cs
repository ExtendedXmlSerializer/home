using System;

namespace ExtendedXmlSerializer.Core
{
	// ATTRIBUTION: https://github.com/mattmc3/dotmore
	public static class StringExtensions
	{
		/// <summary>
		/// Provides a more natural way to call String.Format() on a string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="args">An object array that contains zero or more objects to format</param>
		public static string FormatWith(this string s, params object[] args)
		{
			if (s == null) return null;
			return String.Format(s, args);
		}

		/// <summary>
		/// Provides a more natural way to call String.Format() on a string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="provider">An object that supplies the culture specific formatting</param>
		/// <param name="args">An object array that contains zero or more objects to format</param>
		public static string FormatWith(this string s, IFormatProvider provider, params object[] args)
		{
			if (s == null) return null;
			return String.Format(provider, s, args);
		}
	}

}
