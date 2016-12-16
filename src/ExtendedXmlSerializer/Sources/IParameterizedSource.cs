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
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.Sources
{
    public interface IParameterizedSource<in TParameter, out TResult>
    {
        TResult Get(TParameter parameter);
    }

    public class ConditionalParameterizedSource<TParameter, TResult> : ConditionalParameterizedSourceBase<TParameter, TResult>
    {
        private readonly Func<TParameter, TResult> _source;

        public ConditionalParameterizedSource(ISpecification<TParameter> specification, Func<TParameter, TResult> source) : base(specification)
        {
            _source = source;
        }
        protected override TResult GetResult(TParameter parameter) => _source(parameter);
    }

    public abstract class ConditionalParameterizedSourceBase<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
    {
        private readonly ISpecification<TParameter> _specification;

        protected ConditionalParameterizedSourceBase(ISpecification<TParameter> specification)
        {
            _specification = specification;
        }

        public TResult Get(TParameter parameter)
        {
            return _specification.IsSatisfiedBy(parameter) ? GetResult(parameter) : default(TResult);
        }

        protected abstract TResult GetResult(TParameter parameter);
    }

    public class FirstAssignedSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
    {
        private readonly ImmutableArray<IParameterizedSource<TParameter, TResult>> _sources;
        public FirstAssignedSource(params IParameterizedSource<TParameter, TResult>[] sources )
        {
            _sources = sources.ToImmutableArray();
        }

        public TResult Get(TParameter parameter)
        {
            foreach (var source in _sources)
            {
                var result = source.Get(parameter);
                if (!Equals(result, default(TResult)))
                {
                    return result;
                }
            }
            return default(TResult);
        }
    }
}