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
    abstract class LegacyTemplateBase<T> : ITemplate where T : IInstance
    {
        public void Render(IEmitter emitter, IWriter writer, IElement element)
        {
            var content = (T) element.Content;

            using (writer.New(element))
            {
                if (EmitType(element))
                {
                    writer.Emit(new TypeProperty(element.Content.Type));
                }

                Render(emitter, writer, element, content);
            }
        }

        protected abstract void Render(IEmitter emitter, IWriter writer, IElement element, T content);

        protected virtual bool EmitType(IElement element)
        {
            var entity = element.Content;
            if (element is IRoot)
            {
                return !(entity is IEnumerableObject) && !(entity is IPrimitive);
            }

            if (element.DefinedType != entity.Type)
            {
                return (element as IMember)?.IsWritable ?? true;
            }

            return false;
        }

        public bool IsSatisfiedBy(IElement parameter) => parameter.Content is T;
    }
}