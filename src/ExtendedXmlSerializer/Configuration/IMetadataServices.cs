// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IMetadataConfiguration : IConfigurationElement, ISource<IMetadata> {}


	public abstract class MetadataConfiguration : ConfigurationElement, IMetadataConfiguration
	{
		readonly IMetadata _metadata;

		protected MetadataConfiguration(IExtensions extensions, IMetadata metadata) : base(extensions) => _metadata = metadata;

		IMetadata ISource<IMetadata>.Get() => _metadata;
	}

	public abstract class MetadataConfiguration<TMetadata> : MetadataConfiguration, ISource<TMetadata>
		where TMetadata : class, IMetadata
	{
		readonly TMetadata _metadata;

		protected MetadataConfiguration(IExtensions extensions, TMetadata metadata) : base(extensions, metadata)
			=> _metadata = metadata;

		public TMetadata Get() => _metadata;
	}

	// ReSharper disable once UnusedTypeParameter
	public interface IType<T> : ITypeConfiguration {}

	public interface ITypeConfiguration : IMetadataConfiguration, IValueSource<MemberInfo, IMetadata> {}

	public interface IMemberConfiguration : IMetadataConfiguration {}

	public interface IMetadata : ISource<MemberInfo> {}

	sealed class MemberInstances : IParameterizedSource<MemberInfo, Member>
	{
		readonly IMetadata _type;

		public MemberInstances(IMetadata type) => _type = type;

		public Member Get(MemberInfo parameter) => new Member(_type, parameter);
	}

	public class TypeMetadata<T> : TypeMetadata
	{
		public TypeMetadata() : base(Support<T>.Key) {}
	}

	public class TypeMetadata : FixedInstanceSource<TypeInfo>, IMetadata
	{
		public TypeMetadata(TypeInfo instance) : base(instance) {}

		MemberInfo ISource<MemberInfo>.Get() => Get();
	}

	/*public interface IMember : IMetadata
	{
		IMetadata Type { get; }
	}*/

	public sealed class Member : FixedInstanceSource<MemberInfo>, IMetadata
	{
		public Member(IMetadata type, MemberInfo instance) : base(instance) => Type = type;

		public IMetadata Type { get; }
	}
}