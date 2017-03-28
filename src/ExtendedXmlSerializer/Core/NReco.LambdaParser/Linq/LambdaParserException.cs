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

using System;

namespace ExtendedXmlSerializer.Core.NReco.LambdaParser.Linq
{
	/// <summary>
	/// The exception that is thrown when lambda expression parse error occurs
	/// </summary>
	public class LambdaParserException : Exception
	{
		/// <summary>
		/// Lambda expression
		/// </summary>
		public string Expression { get; private set; }

		/// <summary>
		/// Parser position where syntax error occurs 
		/// </summary>
		public int Index { get; private set; }

		public LambdaParserException(string expr, int idx, string msg)
			: base(string.Format("{0} at {1}: {2}", msg, idx, expr))
		{
			Expression = expr;
			Index = idx;
		}
	}
}