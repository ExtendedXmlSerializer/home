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
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

// ReSharper disable All

namespace ExtendedXmlSerializer.Core.NReco.LambdaParser.Linq
{
	/// <summary>
	/// This is wrapper that makes runtime types conversions real.
	/// </summary>
	internal sealed class LambdaParameterWrapper : IComparable, IEquatable<LambdaParameterWrapper>
	{
		public bool Equals(LambdaParameterWrapper other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(_Value, other._Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is LambdaParameterWrapper && Equals((LambdaParameterWrapper) obj);
		}

		public override int GetHashCode()
		{
			return _Value?.GetHashCode() ?? 0;
		}

		object _Value;

		public object Value
		{
			get { return _Value; }
		}

		public LambdaParameterWrapper(object val)
		{
			if (val is LambdaParameterWrapper)
				_Value = ((LambdaParameterWrapper) val).Value; // prevent nested wrappers
			else if (val is object[])
			{
				var objArr = (object[]) val;
				for (var i = 0; i < objArr.Length; i++)
					if (objArr[i] is LambdaParameterWrapper)
						objArr[i] = ((LambdaParameterWrapper) objArr[i]).Value;
				_Value = val;
			}
			else
			{
				_Value = val;
			}
		}

		public int CompareTo(object obj)
		{
			var objResolved = obj is LambdaParameterWrapper ? ((LambdaParameterWrapper) obj).Value : obj;
			return ValueComparer.Instance.Compare(Value, objResolved);
		}

		public bool IsTrue
		{
			get
			{
				if (_Value == null)
					return false;
				return _Value is bool ? (bool) _Value : Convert.ToBoolean(_Value);
			}
		}

		public static LambdaParameterWrapper CreateDictionary(object[] keys, object[] values)
		{
			if (keys.Length != values.Length)
				throw new ArgumentException();
			var d = new Dictionary<object, object>();
			for (var i = 0; i < keys.Length; i++)
			{
				var k = keys[i];
				var v = values[i];
				// unwrap
				if (k is LambdaParameterWrapper)
					k = ((LambdaParameterWrapper) k).Value;
				if (v is LambdaParameterWrapper)
					v = ((LambdaParameterWrapper) v).Value;
				d[k] = v;
			}
			return new LambdaParameterWrapper(d);
		}

		public static LambdaParameterWrapper InvokeMethod(object obj, string methodName, object[] args)
		{
			if (obj is LambdaParameterWrapper)
				obj = ((LambdaParameterWrapper) obj).Value;

			if (obj == null)
				throw new NullReferenceException(String.Format("Method {0} target is null", methodName));

			var argsResolved = new object[args.Length];
			for (var i = 0; i < args.Length; i++)
				argsResolved[i] = args[i] is LambdaParameterWrapper ? ((LambdaParameterWrapper) args[i]).Value : args[i];

			var invoke = new InvokeMethod(obj, methodName);
			var res = invoke.Invoke(argsResolved);
			return new LambdaParameterWrapper(res);
		}

		public static LambdaParameterWrapper InvokeDelegate(object obj, object[] args)
		{
			if (obj is LambdaParameterWrapper)
				obj = ((LambdaParameterWrapper) obj).Value;
			if (obj == null)
				throw new NullReferenceException("Delegate is null");
			if (!(obj is Delegate))
				throw new NullReferenceException(String.Format("{0} is not a delegate", obj.GetType()));
			var deleg = (Delegate) obj;

			var delegParams =
#if NET40
				 deleg.Method.GetParameters();
				#else
				deleg.GetMethodInfo().GetParameters();
#endif
			if (delegParams.Length != args.Length)
				throw new TargetParameterCountException(
					String.Format("Target delegate expects {0} parameters", delegParams.Length));

			var resolvedArgs = new object[args.Length];
			for (var i = 0; i < resolvedArgs.Length; i++)
			{
				resolvedArgs[i] = Convert.ChangeType(
					args[i] is LambdaParameterWrapper ? ((LambdaParameterWrapper) args[i]).Value : args[i],
					delegParams[i].ParameterType,
					CultureInfo.InvariantCulture
				);
			}
			return new LambdaParameterWrapper(deleg.DynamicInvoke(resolvedArgs));
		}

		public static LambdaParameterWrapper InvokePropertyOrField(object obj, string propertyName)
		{
			if (obj == null)
				throw new NullReferenceException(String.Format("Property or field {0} target is null", propertyName));
			if (obj is LambdaParameterWrapper)
				obj = ((LambdaParameterWrapper) obj).Value;

#if NET40
			var prop = obj.GetType().GetProperty(propertyName);
			#else
			var prop = obj.GetType().GetRuntimeProperty(propertyName);
#endif

			if (prop != null)
			{
				var propVal = prop.GetValue(obj, null);
				return new LambdaParameterWrapper(propVal);
			}
#if NET40
			var fld = obj.GetType().GetField(propertyName);
			#else
			var fld = obj.GetType().GetRuntimeField(propertyName);
#endif
			if (fld != null)
			{
				var fldVal = fld.GetValue(obj);
				return new LambdaParameterWrapper(fldVal);
			}
			throw new MissingMemberException(obj.GetType().ToString() + "." + propertyName);
		}

		public static LambdaParameterWrapper InvokeIndexer(object obj, object[] args)
		{
			if (obj == null)
				throw new NullReferenceException(String.Format("Indexer target is null"));
			if (obj is LambdaParameterWrapper)
				obj = ((LambdaParameterWrapper) obj).Value;

			var argsResolved = new object[args.Length];
			for (var i = 0; i < args.Length; i++)
				argsResolved[i] = args[i] is LambdaParameterWrapper ? ((LambdaParameterWrapper) args[i]).Value : args[i];

			if (obj is Array)
			{
				var objArr = (Array) obj;
				if (objArr.Rank != args.Length)
				{
					throw new RankException(String.Format("Array rank ({0}) doesn't match number of indicies ({1})",
					                                      objArr.Rank, args.Length));
				}
				var indicies = new int[argsResolved.Length];
				for (var i = 0; i < argsResolved.Length; i++)
					indicies[i] = Convert.ToInt32(argsResolved[i]);

				var res = objArr.GetValue(indicies);
				return new LambdaParameterWrapper(res);
			}
			else
			{
				// indexer method
				var invoke = new InvokeMethod(obj, "get_Item");
				var res = invoke.Invoke(argsResolved);
				return new LambdaParameterWrapper(res);
			}
		}

		public static LambdaParameterWrapper operator +(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			if (c1.Value is string || c2.Value is string)
			{
				return new LambdaParameterWrapper(Convert.ToString(c1.Value) + Convert.ToString(c2.Value));
			}
			else
			{
				var c1decimal = Convert.ToDecimal(c1.Value, CultureInfo.InvariantCulture);
				var c2decimal = Convert.ToDecimal(c2.Value, CultureInfo.InvariantCulture);
				return new LambdaParameterWrapper(c1decimal + c2decimal);
			}
		}

		public static LambdaParameterWrapper operator -(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var c1decimal = Convert.ToDecimal(c1.Value, CultureInfo.InvariantCulture);
			var c2decimal = Convert.ToDecimal(c2.Value, CultureInfo.InvariantCulture);
			return new LambdaParameterWrapper(c1decimal - c2decimal);
		}

		public static LambdaParameterWrapper operator -(LambdaParameterWrapper c1)
		{
			var c1decimal = Convert.ToDecimal(c1.Value, CultureInfo.InvariantCulture);
			return new LambdaParameterWrapper(-c1decimal);
		}

		public static LambdaParameterWrapper operator *(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var c1decimal = Convert.ToDecimal(c1.Value, CultureInfo.InvariantCulture);
			var c2decimal = Convert.ToDecimal(c2.Value, CultureInfo.InvariantCulture);
			return new LambdaParameterWrapper(c1decimal * c2decimal);
		}

		public static LambdaParameterWrapper operator /(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var c1decimal = Convert.ToDecimal(c1.Value, CultureInfo.InvariantCulture);
			var c2decimal = Convert.ToDecimal(c2.Value, CultureInfo.InvariantCulture);
			return new LambdaParameterWrapper(c1decimal / c2decimal);
		}

		public static LambdaParameterWrapper operator %(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var c1decimal = Convert.ToDecimal(c1.Value, CultureInfo.InvariantCulture);
			var c2decimal = Convert.ToDecimal(c2.Value, CultureInfo.InvariantCulture);
			return new LambdaParameterWrapper(c1decimal % c2decimal);
		}

		public static bool operator ==(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var compareRes = ValueComparer.Instance.Compare(c1.Value, c2.Value);
			return compareRes == 0;
		}

		public static bool operator ==(LambdaParameterWrapper c1, bool c2)
		{
			var c1bool = Convert.ToBoolean(c1.Value, CultureInfo.InvariantCulture);
			return c1bool == c2;
		}

		public static bool operator ==(bool c1, LambdaParameterWrapper c2)
		{
			var c2bool = Convert.ToBoolean(c2.Value, CultureInfo.InvariantCulture);
			return c1 == c2bool;
		}

		public static bool operator !=(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var compareRes = ValueComparer.Instance.Compare(c1.Value, c2.Value);
			return compareRes != 0;
		}

		public static bool operator !=(LambdaParameterWrapper c1, bool c2)
		{
			var c1bool = Convert.ToBoolean(c1.Value, CultureInfo.InvariantCulture);
			return c1bool != c2;
		}

		public static bool operator !=(bool c1, LambdaParameterWrapper c2)
		{
			var c2bool = Convert.ToBoolean(c2.Value, CultureInfo.InvariantCulture);
			return c1 != c2bool;
		}

		public static bool operator >(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var compareRes = ValueComparer.Instance.Compare(c1.Value, c2.Value);
			return compareRes > 0;
		}

		public static bool operator <(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var compareRes = ValueComparer.Instance.Compare(c1.Value, c2.Value);
			return compareRes < 0;
		}

		public static bool operator >=(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var compareRes = ValueComparer.Instance.Compare(c1.Value, c2.Value);
			return compareRes >= 0;
		}

		public static bool operator <=(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			var compareRes = ValueComparer.Instance.Compare(c1.Value, c2.Value);
			return compareRes <= 0;
		}

		public static LambdaParameterWrapper operator !(LambdaParameterWrapper c1)
		{
			var c1bool = Convert.ToBoolean(c1.Value, CultureInfo.InvariantCulture);
			return new LambdaParameterWrapper(!c1bool);
		}

		public static LambdaParameterWrapper operator &(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			return new LambdaParameterWrapper(c1.IsTrue && c2.IsTrue);
		}

		public static LambdaParameterWrapper operator &(LambdaParameterWrapper c1, bool c2)
		{
			return new LambdaParameterWrapper(c1.IsTrue && c2);
		}

		public static bool operator &(bool c1, LambdaParameterWrapper c2)
		{
			return c1 && c2.IsTrue;
		}

		public static bool operator true(LambdaParameterWrapper x)
		{
			return x.IsTrue;
		}

		public static LambdaParameterWrapper operator |(LambdaParameterWrapper c1, LambdaParameterWrapper c2)
		{
			return new LambdaParameterWrapper(c1.IsTrue || Convert.ToBoolean(c2.Value, CultureInfo.InvariantCulture));
		}

		public static LambdaParameterWrapper operator |(LambdaParameterWrapper c1, bool c2)
		{
			return new LambdaParameterWrapper(c1.IsTrue || c2);
		}

		public static bool operator |(bool c1, LambdaParameterWrapper c2)
		{
			return c1 || c2.IsTrue;
		}

		public static bool operator false(LambdaParameterWrapper x)
		{
			return !x.IsTrue;
		}
	}
}