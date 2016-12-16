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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;

namespace ExtendedXmlSerialization.Extensibility.Write
{
    public static class Extensions
    {
       /* /// <summary>
        /// TODO: Should put this in an instruction.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="this"></param>
        public static void ExecuteWithExtensions(this IInstruction @this, IWriting services) => ExecuteWithExtensions(@this, services, services);
        public static void ExecuteWithExtensions(this IInstruction @this, IExtensions extensions, IServiceProvider services)
        {
            if (extensions.IsSatisfiedBy(services))
            {
                @this.Execute(services);
            }
            extensions.Complete(services);
        }*/

        /*public static IContext GetArrayContext(this IContext @this)
        {
            var parent = @this.Parent?.Parent;
            var result = parent is EnumerableScope ? parent : null;
            return result;
        }

        public static IContext GetDictionaryContext(this IContext @this)
        {
            var parent = @this.Parent?.Parent;
            var instance = parent?.Instance;
            var result = instance is IDictionary
                ? parent
                : null;
            return result;
        }

        public static IMemberScope GetMemberScope(this IContext @this)
        {
            var result = @this as IMemberScope ?? @this.Parent as IMemberScope;
            return result;
        }*/
    }
}