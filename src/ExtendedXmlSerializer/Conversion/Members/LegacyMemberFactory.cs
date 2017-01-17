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

using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.NewConfiguration;

namespace ExtendedXmlSerialization.Conversion.Members
{
    class LegacyMemberFactory : IMemberFactory
    {
        private readonly ExtendedXmlSerializerConfig _config;
        private readonly IMemberFactory _factory;

        public LegacyMemberFactory(ExtendedXmlSerializerConfig config, IMemberFactory factory)
        {
            _config = config;
            _factory = factory;
        }

        public IMember Create(MemberInfo metadata, Typed memberType, bool assignable)
        {
            var result = _factory.Create(metadata, memberType, assignable);
            var assignableMember = result as IAssignableMember;
            if (assignableMember != null)
            {
                var configuration = _config.GetTypeConfig(metadata.DeclaringType);

                var propertyConfig = configuration?.GetPropertyConfig(metadata.Name);
                if (propertyConfig != null && propertyConfig.IsEncrypt)
                {
                    var algorithm = _config.EncryptionAlgorithm;
                    if (algorithm != null)
                    {
                        return new EncryptedMember(algorithm, assignableMember);
                        // value = algorithm.Decrypt(value);
                    }
                }
            }
            return result;
        }
    }
}