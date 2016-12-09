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
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Services.Services;

namespace ExtendedXmlSerialization.Extensibility.Write
{
    public abstract class WritingExtensionBase : ExtensionBase<IWriting>, IWritingExtension
    {
        public override bool Starting(IWriting services)
        {
            var current = services.Current;

            if (current.Member != null)
            {
                switch (current.State)
                {
                    case WriteState.MemberValue:
                        return StartingMemberValue(services, current.Instance, current.Member.GetValueOrDefault());
                    default:
                        return StartingMember(services, current.Instance, current.Member.GetValueOrDefault());
                }
            }

            if (current.Members != null)
            {
                return StartingMembers(services, current.Instance, current.Members);
            }

            var result = current.Instance != null
                ? StartingInstance(services, current.Instance)
                : Initializing(services);
            return result;
        }

        bool IExtension.Starting(IServiceProvider services) => Starting(services.AsValid<IWriting>());

        protected virtual bool Initializing(IWriting writing) => true;
        protected virtual bool StartingInstance(IWriting writing, object instance) => true;

        protected virtual bool StartingMembers(IWriting writing, object instance, IImmutableList<MemberInfo> members)
            => true;

        protected virtual bool StartingMember(IWriting writing, object instance, MemberContext context) => true;
        protected virtual bool StartingMemberValue(IWriting writing, object instance, MemberContext context) => true;

        void IExtension.Finished(IServiceProvider services) => Finished(services.AsValid<IWriting>());

        protected virtual void FinishedInstance(IWriting writing, object instance) {}
        protected virtual void FinishedMembers(IWriting writing, object instance, IImmutableList<MemberInfo> members) {}
        protected virtual void FinishedMember(IWriting writing, object instance, MemberContext member) {}
        protected virtual void FinishedMemberValue(IWriting writing, object instance, MemberContext member) {}
        protected virtual void Completed(IWriting writing) {}

        public override void Finished(IWriting services)
        {
            var current = services.Current;
            if (current.Member != null)
            {
                switch (current.State)
                {
                    case WriteState.MemberValue:
                        FinishedMemberValue(services, current.Instance, current.Member.GetValueOrDefault());
                        break;
                    default:
                        FinishedMember(services, current.Instance, current.Member.GetValueOrDefault());
                        break;
                }
            }
            else if (current.Members != null)
            {
                FinishedMembers(services, current.Instance, current.Members);
            }
            else if (current.Instance != null)
            {
                FinishedInstance(services, current.Instance);
            }
            else
            {
                Completed(services);
            }
        }
    }
}