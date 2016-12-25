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

using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    sealed class LegacyTemplatedEmitter : TemplatedEmitter
    {
        private readonly IContextMonitor _monitor;

        public LegacyTemplatedEmitter(IWriter writer, IContextMonitor monitor, IIdentities identities,
                                      IVersionLocator locator) : base(writer,
                                                                      LegacyPrimitiveTemplate.Default,
                                                                      new LegacyEnumerableObjectTemplate(
                                                                          identities, locator),
                                                                      new LegacyObjectTemplate(identities, locator),
                                                                      LegacyElementsTemplate.Default
        )
        {
            _monitor = monitor;
        }

        protected override void Render(IElement parameter, ITemplate template)
        {
            _monitor.Update(parameter);
            base.Render(parameter, template);
        }
    }
}