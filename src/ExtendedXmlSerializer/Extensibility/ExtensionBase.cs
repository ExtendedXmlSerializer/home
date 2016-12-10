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

namespace ExtendedXmlSerialization.Extensibility
{
    public abstract class ExtensionBase<T> : IExtensionSpecification, IExtensionDefinition where T : IServiceProvider
    {
        public abstract void Accept(IExtensionRegistry registry);

        public bool IsSatisfiedBy(IServiceProvider parameter) => !(parameter is T) || IsSatisfiedBy((T) parameter);
        public virtual bool IsSatisfiedBy(T services) => true;


        void IExtension.Executing(IServiceProvider services)
        {
            if (services is T)
            {
                Executing((T) services);
            }
        }

        void IExtension.Executed(IServiceProvider services)
        {
            if (services is T)
            {
                Executed((T) services);
            }
        }

        void IExtension.Complete(IServiceProvider services)
        {
            if (services is T)
            {
                Completed((T) services);
            }
        }

        public virtual void Executing(T services) {}
        public virtual void Executed(T services) {}
        public virtual void Completed(T services) {}
    }
}