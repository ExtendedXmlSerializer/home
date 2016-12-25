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
    abstract class LegacyObjectTemplateBase<T> : LegacyTemplateBase<T> where T : IObject
    {
        private readonly IIdentities _identities;
        private readonly IVersionLocator _version;

        protected LegacyObjectTemplateBase(IIdentities identities, IVersionLocator version)
        {
            _identities = identities;
            _version = version;
        }

        protected override bool EmitType(IElement element) =>
            _identities.Get(element) is IdentityProperty || base.EmitType(element);

        protected override void Render(IEmitter emitter, IWriter writer, IElement element, T content)
        {
            var version = _version.Get(content.Type);
            if (version > 0)
            {
                writer.Emit(new VersionProperty(version.Value));
            }

            var property = _identities.Get(element);
            if (property != null)
            {
                writer.Emit(property);
                if (property is ObjectReferenceProperty)
                {
                    return;
                }
            }

            Render(emitter, content);
        }

        protected virtual void Render(IEmitter emitter, T content)
        {
            foreach (var o in content.Members)
            {
                emitter.Execute(o);
            }
        }
    }
}