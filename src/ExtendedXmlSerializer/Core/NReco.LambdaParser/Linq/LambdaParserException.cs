using System;
// ReSharper disable All

namespace ExtendedXmlSerializer.Core.NReco.LambdaParser.Linq
{
	/// <summary>
	/// The exception that is thrown when lambda expression parse error occurs
	/// </summary>
	sealed class LambdaParserException : Exception
	{
		/// <summary>
		/// Lambda expression
		/// </summary>
		public string Expression { get; }

		/// <summary>
		/// Parser position where syntax error occurs
		/// </summary>
		public int Index { get; }

		public LambdaParserException(string expr, int idx, string msg)
			: base(string.Format("{0} at {1}: {2}", msg, idx, expr))
		{
			Expression = expr;
			Index      = idx;
		}
	}
}