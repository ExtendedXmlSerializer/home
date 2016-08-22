// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Collections.Concurrent;
using System.Reflection;

#if NETSTANDARD1_6
using Microsoft.Extensions.DependencyModel;
#endif

namespace ExtendedXmlSerialization.Cache
{
    internal static class TypeDefinitionCache
    {
        private static readonly ConcurrentDictionary<Type, TypeDefinition> TypeDefinitions = new ConcurrentDictionary<Type, TypeDefinition>();
        private static readonly ConcurrentDictionary<string, Type> TypeCache = new ConcurrentDictionary<string, Type>();

		public static TypeDefinition GetDefinition(Type type)
		{
			if (TypeDefinitions.ContainsKey(type))
			{
				return TypeDefinitions[type];
			}

			TypeDefinition result = new TypeDefinition(type);
			TypeDefinitions.TryAdd(type, result);
			return result;
		}

        public static Type GetType(string naneType)
        {
            if (TypeCache.ContainsKey(naneType))
            {
                return TypeCache[naneType];
            }

            var result = GetTypeFromName(naneType);
            TypeCache.TryAdd(naneType, result);
            return result;
        }

        private static Type GetTypeFromName(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
                return type;
#if NETSTANDARD1_6
            // TODO In .Net Core 1.1 will be new API or reuse an existing one (AppDomain.GetAssemblies)
            // https://github.com/dotnet/corefx/issues/8806
            // https://github.com/dotnet/corefx/issues/8910
            foreach (RuntimeLibrary runtimeLibrary in DependencyContext.Default.RuntimeLibraries)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(runtimeLibrary.Name));
                
                    type = assembly.GetType(typeName);
                    if (type != null)
                        return type;
                }
                catch
                {  
                    continue;
                }
            }
#else
            foreach (Assembly c in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = c.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
#endif

            throw new Exception("Unknown type "+ typeName);
        }

    }
}
