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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IMetadataConfiguration<out T, TMetadata> : IConfigurationElement, ISource<T>
		where T : IMetadata<TMetadata>
		where TMetadata : MemberInfo {}

	public abstract class MetadataConfiguration<T, TMetadata> : ConfigurationElement, IMetadataConfiguration<T, TMetadata>
		where T : IMetadata<TMetadata> where TMetadata : MemberInfo
	{
		readonly T _metadata;

		protected MetadataConfiguration(ITypes types, IExtensions extensions, T metadata) : base(types, extensions) => _metadata = metadata;

		public T Get() => _metadata;
	}

	// ReSharper disable once UnusedTypeParameter
	public interface IType<T> : ITypeConfiguration {}

	public interface ITypeConfiguration : IMetadataConfiguration<IType, TypeInfo> {}

	public interface IMemberConfiguration : IMetadataConfiguration<IMember, MemberInfo> {}

	public interface IMetadata<out T> : ISource<T> where T : MemberInfo {}

	sealed class MemberInstances : IParameterizedSource<MemberInfo, IMember>
	{
		readonly ITypeMetadata _type;

		public MemberInstances(ITypeMetadata type) => _type = type;

		public IMember Get(MemberInfo parameter) => new Member(_type, parameter);
	}


	public interface ITypeMetadata : IMetadata<TypeInfo> {}

	sealed class TypeMetadata : FixedInstanceSource<TypeInfo>, ITypeMetadata
	{
		public TypeMetadata(TypeInfo instance) : base(instance) {}
	}

	public interface IType : ITypeMetadata, IValueSource<MemberInfo, IMember> {}

	sealed class TypeDefinition : IType
	{
		readonly IValueSource<MemberInfo, IMember> _members;
		readonly ITypeMetadata _type;

		public TypeDefinition(IValueSource<MemberInfo, IMember> members, ITypeMetadata type)
		{
			_members = members;
			_type = type;
		}

		public IMember Get(MemberInfo parameter) => _members.Get(parameter);

		public IEnumerator<IMember> GetEnumerator() => _members.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public TypeInfo Get() => _type.Get();
	}

	public interface IMember : IMetadata<MemberInfo>
	{
		ITypeMetadata Type { get; }
	}

	sealed class TypeDefinitions : ReferenceCacheBase<ITypeMetadata, IType>
	{
		public static TypeDefinitions Default { get; } = new TypeDefinitions();
		TypeDefinitions() {}

		protected override IType Create(ITypeMetadata parameter)
			=> new TypeDefinition(new TableValueSource<MemberInfo, IMember>(new MemberInstances(parameter).Get), parameter);
	}

	sealed class Member : FixedInstanceSource<MemberInfo>, IMember
	{
		public Member(ITypeMetadata type, MemberInfo instance) : base(instance) => Type = type;

		public ITypeMetadata Type { get; }
	}

	public interface ITypes : IValueSource<TypeInfo, IType> {}

	sealed class Types : TableValueSource<TypeInfo, IType>, ITypes
	{
		public Types() : base(TypeDefinitions.Default.In(Coercer.Default).Get) {}

		sealed class Coercer : IParameterizedSource<TypeInfo, ITypeMetadata>
		{
			public static Coercer Default { get; } = new Coercer();
			Coercer() {}

			public ITypeMetadata Get(TypeInfo parameter) => new TypeMetadata(parameter);
		}
	}

	/*public interface IMetadataServices : IValueSource<TypeInfo, IConfigurationElement>,
	                                     IParameterizedSource<IConfigurationElement, IMembers> {}*/
}