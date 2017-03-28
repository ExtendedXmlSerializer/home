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

namespace ExtendedXmlSerializer.Core.NReco.LambdaParser
{
	/// <summary>
	/// Invoke object's method that is most compatible with provided arguments
	/// </summary>
	internal class InvokeMethod
	{
		public object TargetObject { get; set; }

		public string MethodName { get; set; }

		public InvokeMethod(object o, string methodName)
		{
			TargetObject = o;
			MethodName = methodName;
		}

		protected MethodInfo FindMethod(Type[] argTypes)
		{
			if (TargetObject is Type)
			{
				// static method
#if NET40
				return ((Type)TargetObject).GetMethod(MethodName, BindingFlags.Static | BindingFlags.Public);
				#else
				return ((Type) TargetObject).GetRuntimeMethod(MethodName, argTypes);
#endif
			}
#if NET40
			return TargetObject.GetType().GetMethod(MethodName, argTypes);
			#else
			return TargetObject.GetType().GetRuntimeMethod(MethodName, argTypes);
#endif
		}

		protected IEnumerable<MethodInfo> GetAllMethods()
		{
			if (TargetObject is Type)
			{
#if NET40
				return ((Type)TargetObject).GetMethods(BindingFlags.Static | BindingFlags.Public);
				#else
				return ((Type) TargetObject).GetRuntimeMethods();
#endif
			}
#if NET40
			return TargetObject.GetType().GetMethods();
			#else
			return TargetObject.GetType().GetRuntimeMethods();
#endif
		}

		public object Invoke(object[] args)
		{
			var argTypes = new Type[args.Length];
			for (var i = 0; i < argTypes.Length; i++)
				argTypes[i] = args[i] != null ? args[i].GetType() : typeof(object);

			// strict matching first
			var targetMethodInfo = FindMethod(argTypes);
			// fuzzy matching
			if (targetMethodInfo == null)
			{
				var methods = GetAllMethods();

				foreach (var m in methods)
					if (m.Name == MethodName &&
					    m.GetParameters().Length == args.Length &&
					    CheckParamsCompatibility(m.GetParameters(), argTypes, args))
					{
						targetMethodInfo = m;
						break;
					}
			}
			if (targetMethodInfo == null)
			{
				var argTypeNames = new string[argTypes.Length];
				for (var i = 0; i < argTypeNames.Length; i++)
					argTypeNames[i] = argTypes[i].Name;
				var argTypeNamesStr = String.Join(",", argTypeNames);
				throw new MissingMemberException(
					(TargetObject is Type ? (Type) TargetObject : TargetObject.GetType()).FullName + "." + MethodName);
			}
			var argValues = PrepareActualValues(targetMethodInfo.GetParameters(), args);
			object res = null;
			try
			{
				res = targetMethodInfo.Invoke(TargetObject is Type ? null : TargetObject, argValues);
			}
			catch (TargetInvocationException tiEx)
			{
				if (tiEx.InnerException != null)
					throw new Exception(tiEx.InnerException.Message, tiEx.InnerException);
				else
				{
					throw;
				}
			}
			return res;
		}

		private bool IsInstanceOfType(Type t, object val)
		{
#if NET40 
			return t.IsInstanceOfType(val);
			#else
			return val != null && t.GetTypeInfo().IsAssignableFrom(val.GetType().GetTypeInfo());
#endif
		}

		protected bool CheckParamsCompatibility(ParameterInfo[] paramsInfo, Type[] types, object[] values)
		{
			for (var i = 0; i < paramsInfo.Length; i++)
			{
				var paramType = paramsInfo[i].ParameterType;
				var val = values[i];
				if (IsInstanceOfType(paramType, val))
					continue;
				// null and reference types
				if (val == null &&
#if NET40
					!paramType.IsValueType
					#else
				    !paramType.GetTypeInfo().IsValueType
#endif
				)
					continue;
				// possible autocast between generic/non-generic common types
				try
				{
					Convert.ChangeType(val, paramType, CultureInfo.InvariantCulture);
					continue;
				}
				catch {}
				//if (ConvertManager.CanChangeType(types[i],paramType))
				//	continue;
				// incompatible parameter
				return false;
			}
			return true;
		}


		protected object[] PrepareActualValues(ParameterInfo[] paramsInfo, object[] values)
		{
			var res = new object[paramsInfo.Length];
			for (var i = 0; i < paramsInfo.Length; i++)
			{
				if (values[i] == null || IsInstanceOfType(paramsInfo[i].ParameterType, values[i]))
				{
					res[i] = values[i];
					continue;
				}
				try
				{
					res[i] = Convert.ChangeType(values[i], paramsInfo[i].ParameterType, CultureInfo.InvariantCulture);
					continue;
				}
				catch
				{
					throw new InvalidCastException(
						String.Format("Invoke method '{0}': cannot convert argument #{1} from {2} to {3}",
						              MethodName, i, values[i].GetType(), paramsInfo[i].ParameterType));
				}
			}
			return res;
		}
	}
}