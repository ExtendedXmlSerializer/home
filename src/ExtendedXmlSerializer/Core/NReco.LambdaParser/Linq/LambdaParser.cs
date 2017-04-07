#region License
/*
 * NReco library (http://nreco.googlecode.com/)
 * Copyright 2014 Vitaliy Fedorchenko
 * Distributed under the LGPL licence
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable All

namespace ExtendedXmlSerializer.Core.NReco.LambdaParser.Linq {

	public class LambdaParser {

		static readonly char[] delimiters = new char[] {
			'(', ')', '[', ']', '?', ':', '.', ',', '=', '<', '>', '!', '&', '|', '*', '/', '%', '+','-', '{', '}'};
		static readonly char[] specialNameChars = new char[] {
			'_' };
		static readonly char charQuote = '"';
		static readonly string[] mulOps = new[] {"*", "/", "%" };
		static readonly string[] addOps = new[] { "+", "-" };
		static readonly string[] eqOps = new[] { "==", "!=", "<", ">", "<=", ">=" };

		static IDictionary<string, CompiledExpression> CachedExpressions = new Dictionary<string, CompiledExpression>();

		public bool UseCache { get; set; }

		public LambdaParser() {
			UseCache = true;
		}

		internal class ExtractParamsVisitor : ExpressionVisitor {
			internal List<ParameterExpression> ParamsList;
			public ExtractParamsVisitor() {
				ParamsList = new List<ParameterExpression>();
			}

			public override Expression Visit(Expression node) {
				if (node!=null && node.NodeType == ExpressionType.Parameter)
					ParamsList.Add( (ParameterExpression)node);
				return base.Visit(node);
			}
		}

		public static ParameterExpression[] GetExpressionParameters(Expression expr) {
			var paramsVisitor = new ExtractParamsVisitor();
			paramsVisitor.Visit(expr);
			return paramsVisitor.ParamsList.ToArray();
		}

		public object Eval(string expr, IDictionary<string, object> vars) {
			return Eval(expr, (varName) => {
				object val = null;
				vars.TryGetValue(varName, out val);
				return val;
			});
		}

		public object Eval(string expr, Func<string,object> getVarValue) {
			CompiledExpression compiledExpr;
			if (!UseCache || !CachedExpressions.TryGetValue(expr, out compiledExpr)) {
				var linqExpr = Parse(expr);
				compiledExpr = new CompiledExpression() {
					Parameters = GetExpressionParameters(linqExpr)
				};
				var lambdaExpr = Expression.Lambda(linqExpr, compiledExpr.Parameters);
				compiledExpr.Lambda = lambdaExpr.Compile();

				if (UseCache)
					lock (CachedExpressions) {
						CachedExpressions[expr] = compiledExpr;
					}
			}

			var valuesList = new List<object>();
			foreach (var paramExpr in compiledExpr.Parameters) {
				valuesList.Add( new LambdaParameterWrapper( getVarValue(paramExpr.Name)) );
			}

			var lambdaRes = compiledExpr.Lambda.DynamicInvoke(valuesList.ToArray());
			if (lambdaRes is LambdaParameterWrapper)
				lambdaRes = ((LambdaParameterWrapper)lambdaRes).Value;
			return lambdaRes;
		}


		public Expression Parse(string expr) {
			var parseResult = ParseConditional(expr, 0);
			var lastLexem = ReadLexem(expr, parseResult.End);
			if (lastLexem.Type != LexemType.Stop)
				throw new LambdaParserException(expr, parseResult.End, "Invalid expression");
			return parseResult.Expr;
		}

		protected Lexem ReadLexem(string s, int startIdx) {
			var lexem = new Lexem();
			lexem.Type = LexemType.Unknown;
			lexem.Expr = s;
			lexem.Start = startIdx;
			lexem.End = startIdx;
			while (lexem.End < s.Length) {
				if (Array.IndexOf(delimiters, s[lexem.End]) >= 0) {
					if (lexem.Type == LexemType.Unknown) {
						lexem.End++;
						lexem.Type = LexemType.Delimiter;
						return lexem;
					}
					if (lexem.Type != LexemType.StringConstant && (lexem.Type != LexemType.NumberConstant || s[lexem.End] != '.'))
						return lexem; // stop
				} else if (Char.IsSeparator(s[lexem.End])) {
					if (lexem.Type != LexemType.StringConstant && lexem.Type != LexemType.Unknown)
						return lexem; // stop
				} else if (Char.IsLetter(s[lexem.End])) {
					if (lexem.Type == LexemType.Unknown)
						lexem.Type = LexemType.Name;
				} else if (Char.IsDigit(s[lexem.End])) {
					if (lexem.Type == LexemType.Unknown)
						lexem.Type = LexemType.NumberConstant;
				} else if (Array.IndexOf(specialNameChars, s[lexem.End]) >= 0) {
					if (lexem.Type == LexemType.Unknown)
						lexem.Type = LexemType.Name;
					else if (lexem.Type!=LexemType.StringConstant)
						return lexem;
				} else if (s[lexem.End] == charQuote) {
					if (lexem.Type == LexemType.Unknown)
						lexem.Type = LexemType.StringConstant;
					else {
						if (lexem.Type == LexemType.StringConstant) {
							// check for "" combination
							if (((lexem.End + 1) >= s.Length || s[lexem.End + 1] != charQuote)) {
								lexem.End++;
								return lexem;
							} else
								if ((lexem.End + 1) < s.Length)
									lexem.End++; // skip next quote
						} else {
							return lexem;
						}
					}
				} else if (Char.IsControl(s[lexem.End]) && lexem.Type != LexemType.Unknown && lexem.Type != LexemType.StringConstant)
					return lexem;

				// goto next char
				lexem.End++;
			}

			if (lexem.Type == LexemType.Unknown) {
				lexem.Type = LexemType.Stop;
				return lexem;
			}
			if (lexem.Type == LexemType.StringConstant)
				throw new LambdaParserException(s, startIdx, "Unterminated string constant");
			return lexem;
		}


		ConstructorInfo LambdaParameterWrapperConstructor = typeof(LambdaParameterWrapper).GetConstructor(new[] { typeof(object) });

		protected ParseResult ParseConditional(string expr, int start) {
			var testExpr = ParseOr(expr, start);
			var ifLexem = ReadLexem(expr, testExpr.End);
			if (ifLexem.Type == LexemType.Delimiter && ifLexem.GetValue() == "?") {
				// read positive expr
				var positiveOp = ParseOr(expr, ifLexem.End);
				var positiveOpExpr = Expression.New(LambdaParameterWrapperConstructor, Expression.Convert(positiveOp.Expr,typeof(object)) );

				var elseLexem = ReadLexem(expr, positiveOp.End);
				if (elseLexem.Type == LexemType.Delimiter && elseLexem.GetValue() == ":") {
					var negativeOp = ParseOr(expr, elseLexem.End);
					var negativeOpExpr = Expression.New(LambdaParameterWrapperConstructor, Expression.Convert( negativeOp.Expr, typeof(object)));
					return new ParseResult() {
						End = negativeOp.End,
						Expr = Expression.Condition( Expression.IsTrue( testExpr.Expr ), positiveOpExpr, negativeOpExpr)
					};

				} else {
					throw new LambdaParserException(expr, positiveOp.End, "Expected ':'");
				}
			}
			return testExpr;
		}

		protected ParseResult ParseOr(string expr, int start) {
			var firstOp = ParseAnd(expr, start);
			do {
				var opLexem = ReadLexem(expr, firstOp.End);
				var isOr = false;
				if (opLexem.Type == LexemType.Name && opLexem.GetValue() == "or") {
					isOr = true;
				} else if (opLexem.Type == LexemType.Delimiter && opLexem.GetValue() == "|") {
					opLexem = ReadLexem(expr, opLexem.End);
					if (opLexem.Type == LexemType.Delimiter && opLexem.GetValue() == "|")
						isOr = true;
				}

				if (isOr) {
					var secondOp = ParseOr(expr, opLexem.End);
					firstOp = new ParseResult() {
						End = secondOp.End,
						Expr = Expression.OrElse(firstOp.Expr, secondOp.Expr)
					};
				} else
					break;
			} while (true);
			return firstOp;
		}

		protected ParseResult ParseAnd(string expr, int start) {
			var firstOp = ParseEq(expr, start);
			do {
				var opLexem = ReadLexem(expr, firstOp.End);
				var isAnd = false;
				if (opLexem.Type == LexemType.Name && opLexem.GetValue() == "and") {
					isAnd = true;
				} else if (opLexem.Type == LexemType.Delimiter && opLexem.GetValue() == "&") {
					opLexem = ReadLexem(expr, opLexem.End);
					if (opLexem.Type == LexemType.Delimiter && opLexem.GetValue() == "&")
						isAnd = true;
				}

				if (isAnd) {
					var secondOp = ParseAnd(expr, opLexem.End);
					firstOp = new ParseResult() {
						End = secondOp.End,
						Expr = Expression.AndAlso(firstOp.Expr, secondOp.Expr)
					};
				} else
					break;
			} while (true);
			return firstOp;
		}

		protected ParseResult ParseEq(string expr, int start) {
			var firstOp = ParseAdditive(expr, start);
			do {
				var opLexem = ReadLexem(expr, firstOp.End);
				if (opLexem.Type == LexemType.Delimiter) {
					var nextOpLexem = ReadLexem(expr, opLexem.End);
					if (nextOpLexem.Type == LexemType.Delimiter) {
						var opVal = opLexem.GetValue() + nextOpLexem.GetValue();
						if (eqOps.Contains(opVal)) {
							var secondOp = ParseAdditive(expr, nextOpLexem.End);

							switch (opVal) {
								case "==":
									firstOp = new ParseResult() {
										End = secondOp.End,
										Expr = Expression.Equal(firstOp.Expr, secondOp.Expr)
									};
									continue;
								case "!=":
									firstOp = new ParseResult() {
										End = secondOp.End,
										Expr = Expression.NotEqual(firstOp.Expr, secondOp.Expr)
									};
									continue;
								case ">=":
									firstOp = new ParseResult() {
										End = secondOp.End,
										Expr = Expression.GreaterThanOrEqual(firstOp.Expr, secondOp.Expr)
									};
									continue;
								case "<=":
									firstOp = new ParseResult() {
										End = secondOp.End,
										Expr = Expression.LessThanOrEqual(firstOp.Expr, secondOp.Expr)
									};
									continue;

							}
						}

					}

					if (opLexem.GetValue() == ">" || opLexem.GetValue() == "<") {
						var secondOp = ParseAdditive(expr, opLexem.End);
						switch (opLexem.GetValue()) {
							case ">":
								firstOp = new ParseResult() {
									End = secondOp.End,
									Expr = Expression.GreaterThan(firstOp.Expr, secondOp.Expr)
								};
								continue;
							case "<":
								firstOp = new ParseResult() {
									End = secondOp.End,
									Expr = Expression.LessThan(firstOp.Expr, secondOp.Expr)
								};
								continue;
						}
					}

				}
				break;
			} while (true);
			return firstOp;
		}


		protected ParseResult ParseAdditive(string expr, int start) {
			var firstOp = ParseMultiplicative(expr, start);
			do {
				var opLexem = ReadLexem(expr, firstOp.End);
				if (opLexem.Type == LexemType.Delimiter && addOps.Contains(opLexem.GetValue())) {
					var secondOp = ParseMultiplicative(expr, opLexem.End);
					var res = new ParseResult() { End = secondOp.End };
					switch (opLexem.GetValue()) {
						case "+":
							res.Expr = Expression.Add(firstOp.Expr, secondOp.Expr);
							break;
						case "-":
							res.Expr = Expression.Subtract(firstOp.Expr, secondOp.Expr);
							break;
					}
					firstOp = res;
					continue;
				}
				break;
			} while (true);
			return firstOp;
		}

		protected ParseResult ParseMultiplicative(string expr, int start) {
			var firstOp = ParseUnary(expr, start);
			do {
				var opLexem = ReadLexem(expr, firstOp.End);
				if (opLexem.Type == LexemType.Delimiter && mulOps.Contains(opLexem.GetValue())) {
					var secondOp = ParseUnary(expr, opLexem.End);
					var res = new ParseResult() { End = secondOp.End };
					switch (opLexem.GetValue()) {
						case "*":
							res.Expr = Expression.Multiply(firstOp.Expr, secondOp.Expr);
							break;
						case "/":
							res.Expr = Expression.Divide(firstOp.Expr, secondOp.Expr);
							break;
						case "%":
							res.Expr = Expression.Modulo(firstOp.Expr, secondOp.Expr);
							break;
					}
					firstOp = res;
					continue;
				}
				break;
			} while (true);
			return firstOp;
		}

		protected ParseResult ParseUnary(string expr, int start) {
			var opLexem = ReadLexem(expr, start);
			if (opLexem.Type == LexemType.Delimiter && opLexem.GetValue() == "-") {
				var operand = ParsePrimary(expr, opLexem.End);
				operand.Expr = Expression.Negate(operand.Expr);
				return operand;
			}
			return ParsePrimary(expr, start);
		}

		protected MethodInfo GetInvokeMethod() {
			return typeof(LambdaParameterWrapper).GetMethod("InvokeMethod",
				BindingFlags.Static | BindingFlags.Public);
		}
		protected MethodInfo GetInvokeDelegate() {
			return typeof(LambdaParameterWrapper).GetMethod("InvokeDelegate",
				BindingFlags.Static | BindingFlags.Public);
		}
		protected MethodInfo GetPropertyOrFieldMethod() {
			return typeof(LambdaParameterWrapper).GetMethod("InvokePropertyOrField",
				BindingFlags.Static | BindingFlags.Public);
		}
		protected MethodInfo GetIndexerMethod() {
			return typeof(LambdaParameterWrapper).GetMethod("InvokeIndexer",
				BindingFlags.Static | BindingFlags.Public);
		}
		protected MethodInfo GetCreateDictionaryMethod() {
			return typeof(LambdaParameterWrapper).GetMethod("CreateDictionary",
				BindingFlags.Static | BindingFlags.Public);
		}

		protected ParseResult ParsePrimary(string expr, int start) {
			var val = ParseValue(expr, start);
			do {
				var lexem = ReadLexem(expr, val.End);
				if (lexem.Type==LexemType.Delimiter) {
					if (lexem.GetValue() == ".") { // member or method
						var memberLexem = ReadLexem(expr, lexem.End);
						if (memberLexem.Type == LexemType.Name) {
							var openCallLexem = ReadLexem(expr, memberLexem.End);
							if (openCallLexem.Type == LexemType.Delimiter && openCallLexem.GetValue() == "(") {
								var methodParams = new List<Expression>();
								var paramsEnd = ReadCallArguments(expr, openCallLexem.End, ")", methodParams);
								var paramsExpr = Expression.NewArrayInit(typeof(object), methodParams);
								val = new ParseResult() {
									End = paramsEnd,
									Expr = Expression.Call(GetInvokeMethod(),
										val.Expr,
										Expression.Constant(memberLexem.GetValue()),
										paramsExpr)
								};
								continue;
							} else {
								// member
								val = new ParseResult() {
									End = memberLexem.End,
									Expr = Expression.Call(GetPropertyOrFieldMethod(), val.Expr, Expression.Constant(memberLexem.GetValue()))
								};
								continue;
							}
						}
					} else if (lexem.GetValue()=="[") {
						var indexerParams = new List<Expression>();
						var paramsEnd = ReadCallArguments(expr, lexem.End, "]", indexerParams);
						val = new ParseResult() {
							End = paramsEnd,
							Expr = Expression.Call(GetIndexerMethod(), val.Expr,
								Expression.NewArrayInit(typeof(object), indexerParams)
							)
						};
						continue;
					} else if (lexem.GetValue()=="(") {
						var methodParams = new List<Expression>();
						var paramsEnd = ReadCallArguments(expr, lexem.End, ")", methodParams);
						var paramsExpr = Expression.NewArrayInit(typeof(object), methodParams);
						val = new ParseResult() {
							End = paramsEnd,
							Expr = Expression.Call(GetInvokeDelegate(),
								val.Expr, paramsExpr)
						};
					}
				}
				break;
			} while (true);
			return val;
		}

		protected int ReadCallArguments(string expr, int start, string endLexem, List<Expression> args) {
			var end = start;
			do {
				var lexem = ReadLexem(expr, end);
				if (lexem.Type == LexemType.Delimiter) {
					if (lexem.GetValue() == endLexem) {
						return lexem.End;
					} else if (lexem.GetValue() == ",") {
						if (args.Count == 0) {
							throw new LambdaParserException(expr, lexem.Start, "Expected method call parameter");
						}
						end = lexem.End;
					}
				}
				// read parameter
				var paramExpr = ParseConditional(expr, end);
				args.Add(paramExpr.Expr);
				end = paramExpr.End;
			} while (true);
		}

		protected ParseResult ParseValue(string expr, int start) {
			var lexem = ReadLexem(expr, start);

			if (lexem.Type == LexemType.Delimiter && lexem.GetValue() == "(") {
				var groupRes = ParseConditional(expr, lexem.End);
				var endLexem = ReadLexem(expr, groupRes.End);
				if (endLexem.Type != LexemType.Delimiter || endLexem.GetValue() != ")")
					throw new LambdaParserException(expr, endLexem.Start, "Expected ')'");
				groupRes.End = endLexem.End;
				return groupRes;
			} else if (lexem.Type == LexemType.NumberConstant) {
				decimal numConst;
				if (!Decimal.TryParse(lexem.GetValue(), NumberStyles.Any, CultureInfo.InvariantCulture, out numConst)) {
					throw new Exception(String.Format("Invalid number: {0}", lexem.GetValue()));
				}
				return new ParseResult() {
					End = lexem.End,
					Expr = Expression.Constant(new LambdaParameterWrapper( numConst ) ) };
			} else if (lexem.Type == LexemType.StringConstant) {
				return new ParseResult() {
					End = lexem.End,
					Expr = Expression.Constant( new LambdaParameterWrapper( lexem.GetValue() ) ) };
			} else if (lexem.Type == LexemType.Name) {
				// check for predefined constants
				var val = lexem.GetValue();
				switch (val) {
					case "true":
						return new ParseResult() { End = lexem.End, Expr = Expression.Constant(new LambdaParameterWrapper(true) ) };
					case "false":
						return new ParseResult() { End = lexem.End, Expr = Expression.Constant(new LambdaParameterWrapper(false) ) };
					case "new":
						return ReadNewInstance(expr, lexem.End);
				}

				// todo

				return new ParseResult() { End = lexem.End, Expr = Expression.Parameter(typeof(LambdaParameterWrapper), val) };
			}
			throw new LambdaParserException(expr, start, "Expected value");
		}

		protected ParseResult ReadNewInstance(string expr, int start) {
			var nextLexem = ReadLexem(expr, start);
			if (nextLexem.Type == LexemType.Delimiter && nextLexem.GetValue() == "[") {
				nextLexem = ReadLexem(expr, nextLexem.End);
				if (!(nextLexem.Type == LexemType.Delimiter && nextLexem.GetValue() == "]"))
					throw new LambdaParserException(expr, nextLexem.Start, "Expected ']'");

				nextLexem = ReadLexem(expr, nextLexem.End);
				if (!(nextLexem.Type == LexemType.Delimiter && nextLexem.GetValue() == "{"))
					throw new LambdaParserException(expr, nextLexem.Start, "Expected '{'");

				var arrayArgs = new List<Expression>();
				var end = ReadCallArguments(expr, nextLexem.End, "}", arrayArgs);
				var newArrExpr = Expression.NewArrayInit(typeof(object), arrayArgs );
				return new ParseResult() {
					End = end,
					Expr = Expression.New(LambdaParameterWrapperConstructor, Expression.Convert(newArrExpr, typeof(object)))
				};
			}
			if (nextLexem.Type == LexemType.Name && nextLexem.GetValue().ToLower() == "dictionary") {
				nextLexem = ReadLexem(expr, nextLexem.End);
				if (!(nextLexem.Type == LexemType.Delimiter && nextLexem.GetValue() == "{"))
					throw new LambdaParserException(expr, nextLexem.Start, "Expected '{'");

				var dictionaryKeys = new List<Expression>();
				var dictionaryValues = new List<Expression>();
				do {
					nextLexem = ReadLexem(expr, nextLexem.End);
					if (!(nextLexem.Type == LexemType.Delimiter && nextLexem.GetValue() == "{"))
						throw new LambdaParserException(expr, nextLexem.Start, "Expected '{'");
					var entryArgs = new List<Expression>();
					var end = ReadCallArguments(expr, nextLexem.End, "}", entryArgs);
					if (entryArgs.Count!=2)
						throw new LambdaParserException(expr, nextLexem.Start, "Dictionary entry should have exactly 2 arguments");

					dictionaryKeys.Add( entryArgs[0] );
					dictionaryValues.Add( entryArgs[1] );

					nextLexem = ReadLexem(expr, end);
				} while (nextLexem.Type == LexemType.Delimiter && nextLexem.GetValue() == ",");

				if (!(nextLexem.Type == LexemType.Delimiter && nextLexem.GetValue() == "}"))
					throw new LambdaParserException(expr, nextLexem.Start, "Expected '}'");

				var newKeysArrExpr = Expression.NewArrayInit(typeof(object), dictionaryKeys );
				var newValuesArrExpr = Expression.NewArrayInit(typeof(object), dictionaryValues );

				return new ParseResult() {
					End = nextLexem.End,
					Expr = Expression.Call(GetCreateDictionaryMethod(),
						newKeysArrExpr, newValuesArrExpr)
				};
			}
			throw new LambdaParserException(expr, start, "Unknown new instance initializer");
		}

		protected enum LexemType {
			Unknown,
			Name,
			Delimiter,
			StringConstant,
			NumberConstant,
			Stop
		}

		protected struct Lexem {
			public LexemType Type;
			public int Start;
			public int End;
			public string Expr;

			string rawValue;

			public string GetValue() {
				if (rawValue==null) {
					rawValue = Expr.Substring(Start, End-Start).Trim();
					if (Type==LexemType.StringConstant) {
						rawValue = rawValue.Substring(1, rawValue.Length-2).Replace( "\"\"", "\"" );
					}
				}
				return rawValue;
			}
		}

		protected struct ParseResult {
			public Expression Expr;
			public int End;
		}

		public class CompiledExpression {
			public Delegate Lambda;
			public ParameterExpression[] Parameters;
		}


	}
}
