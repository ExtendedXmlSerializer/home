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
using System.Reflection;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.ProcessModel.Write;

namespace ExtendedXmlSerialization.Instructions.Write
{
    class EmitMemberAsTextInstruction : WriteInstructionBase
    {
        public static EmitMemberAsTextInstruction Default { get; } = new EmitMemberAsTextInstruction();
        EmitMemberAsTextInstruction() {}

        protected override void OnExecute(IWriting services)
        {
            var member = services.Current.Member;
            if (member != null)
            {
                var @namespace = !member.Value.Metadata.DeclaringType.IsInstanceOfType(services.Current.Instance)
                    ? services.Get(member.Value.Metadata.DeclaringType)
                    : null;
                services.Emit(new MemberProperty(@namespace, member.Value));
            }
            else
            {
                throw new InvalidOperationException(
                          $"An attempt was made to emit a member of '{services.Current.Instance.GetType()}' but it is not available.");
            }
        }
    }
}