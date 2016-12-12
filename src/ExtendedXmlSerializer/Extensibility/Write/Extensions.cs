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
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.ProcessModel.Write;

namespace ExtendedXmlSerialization.Extensibility.Write
{
    public static class Extensions
    {
        /// <summary>
        /// TODO: Should put this in an instruction.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="instruction"></param>
        public static void ApplyExtensions(this IWriting @this, IInstruction instruction) => ApplyExtensions(@this, @this, instruction);
        public static void ApplyExtensions(this IExtensions @this, IServiceProvider services, IInstruction instruction)
        {
            if (@this.IsSatisfiedBy(services))
            {
                instruction.Execute(services);
            }
            @this.Complete(services);
        }

        public static WriteContext? Parent(this IWritingContext @this, int level = 1)
            => @this.Hierarchy.ElementAtOrDefault(level);

        public static WriteContext? GetArrayContext(this IWritingContext @this)
        {
            var parent = @this.Parent((int) @this.Current.State);
            var instance = parent?.Instance;
            var result = instance != null && Arrays.Default.Is(instance) ? parent : null;
            return result;
        }

        public static WriteContext? GetDictionaryContext(this IWritingContext @this)
        {
            var parent = @this.Parent((int) @this.Current.State + 1);

            var instance = parent?.Instance;
            var result = instance != null && TypeDefinitionCache.GetDefinition(instance.GetType()).IsDictionary
                ? parent
                : null;
            return result;
        }

        public static WriteContext? GetMemberContext(this IWritingContext @this)
        {
            if (@this.Current.Member != null)
            {
                return @this.Current;
            }
            var parent = @this.Parent((int) @this.Current.State);
            var result = parent?.Member != null ? parent : null;
            return result;
        }
    }
}