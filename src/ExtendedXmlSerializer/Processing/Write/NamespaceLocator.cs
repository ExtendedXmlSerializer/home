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

/*namespace ExtendedXmlSerialization.Processing.Write
{
    public class NamespaceLocator : WeakCacheBase<Type, Uri>, INamespaceLocator
    {
        readonly private static Assembly Assembly = typeof(ExtendedXmlSerializer).GetTypeInfo().Assembly;
        private readonly ISpecification<Type> _primitiveSpecification;
        private readonly Uri _root, _primitive;
        readonly private Assembly _assembly;

        public NamespaceLocator(Uri root)
            : this(IsPrimitiveSpecification.Default, root, PrimitiveNamespace.Default.Identifier) {}

        public NamespaceLocator(ISpecification<Type> primitiveSpecification, Uri root,
                                Uri primitive)
        {
            _primitiveSpecification = primitiveSpecification;
            _root = root;
            _primitive = primitive;
            _assembly = Assembly;
        }

        public Uri Locate(object parameter) => (parameter as INamespace)?.Identifier ?? FromType(parameter);

        private Uri FromType(object parameter)
        {
            var type = parameter as Type ?? parameter.GetType();
            var result = Equals(type.GetTypeInfo().Assembly, _assembly) ? _root : base.Get(type);
            return result;
        }

        protected override Uri Callback(Type key)
        {
            var result =
                _primitiveSpecification.IsSatisfiedBy(key)
                    ? _primitive
                    : new Uri($"clr-namespace:{key.Namespace};assembly={key.GetTypeInfo().Assembly.GetName().Name}");
            return result;
        }
    }
}*/