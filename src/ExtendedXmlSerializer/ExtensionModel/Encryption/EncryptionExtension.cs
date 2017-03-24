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

using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Converters;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Encryption
{
	sealed class EncryptionExtension : ISerializerExtension, IAlteration<IConverter>
	{
		readonly static Encryption Encryption = Encryption.Default;

		readonly IEncryption _encryption;

		[UsedImplicitly]
		public EncryptionExtension() : this(Encryption) {}

		public EncryptionExtension(IEncryption encryption) : this(encryption, new HashSet<MemberInfo>()) {}

		public EncryptionExtension(ICollection<MemberInfo> registered) : this(Encryption, registered) {}

		public EncryptionExtension(IEncryption encryption, ICollection<MemberInfo> registered)
		{
			_encryption = encryption;
			Registered = registered;
		}

		public ICollection<MemberInfo> Registered { get; }

		public IServiceRepository Get(IServiceRepository parameter) =>
			parameter.Decorate<IMemberConverterSpecification>(Register)
			         .Decorate<IMemberConverters>(Register);

		IMemberConverterSpecification Register(IServiceProvider services, IMemberConverterSpecification specification)
			=> new MemberConverterSpecification(new ContainsSpecification<MemberInfo>(Registered), specification);

		IMemberConverters Register(IServiceProvider services, IMemberConverters converters)
			=> new Converters(this, Registered, converters);

		void ICommand<IServices>.Execute(IServices parameter) {}

		public IConverter Get(IConverter parameter) => new EncryptedConverter(_encryption, parameter);

		sealed class Converters : IMemberConverters
		{
			readonly IAlteration<IConverter> _alteration;
			readonly ICollection<MemberInfo> _registered;
			readonly IMemberConverters _converters;

			public Converters(IAlteration<IConverter> alteration, ICollection<MemberInfo> registered,
			                  IMemberConverters converters)
			{
				_alteration = alteration;
				_registered = registered;
				_converters = converters;
			}

			public IConverter Get(MemberInfo parameter)
			{
				var converter = _converters.Get(parameter);
				var result = _registered.Contains(parameter) ? _alteration.Get(converter) : converter;
				return result;
			}
		}
	}
}