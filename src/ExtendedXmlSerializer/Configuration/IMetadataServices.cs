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
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IMetadataConfiguration<out T> : IConfigurationElement, ISource<IMetadata<T>> where T : MemberInfo {}


	public abstract class MetadataConfiguration<TMetadata, TMember> : ConfigurationElement, ISource<TMetadata>, IMetadataConfiguration<TMember>
		where TMetadata : class, IMetadata<TMember>
		where TMember : MemberInfo
	{
		readonly TMetadata _metadata;

		protected MetadataConfiguration(IReflection reflection, IExtensions extensions, TMetadata metadata)
			: base(reflection, extensions) => _metadata = metadata;

		IMetadata<TMember> ISource<IMetadata<TMember>>.Get() => Get();

		public TMetadata Get() => _metadata;
	}

	// ReSharper disable once UnusedTypeParameter
	public interface IType<T> : ITypeConfiguration {}

	public interface ITypeConfiguration : IMetadataConfiguration<TypeInfo>, IValueSource<MemberInfo, IMember> {}

	public interface IMemberConfiguration : IMetadataConfiguration<MemberInfo> {}

	public interface ITypeMetadata : IMetadata<TypeInfo> {}

	public interface IMetadata<out T> : ISource<T> where T : MemberInfo {}

	sealed class MemberInstances : IParameterizedSource<MemberInfo, IMember>
	{
		readonly ITypeMetadata _type;

		public MemberInstances(ITypeMetadata type) => _type = type;

		public IMember Get(MemberInfo parameter) => new Member(_type, parameter);
	}



	sealed class TypeMetadata : FixedInstanceSource<TypeInfo>, ITypeMetadata
	{
		public TypeMetadata(TypeInfo instance) : base(instance) {}
	}

	public interface IMember : IMetadata<MemberInfo>
	{
		ITypeMetadata Type { get; }
	}

	sealed class Member : FixedInstanceSource<MemberInfo>, IMember
	{
		public Member(ITypeMetadata type, MemberInfo instance) : base(instance) => Type = type;

		public ITypeMetadata Type { get; }
	}

	public interface IReflection : IValueSource<TypeInfo, ITypeMetadata> {}

	sealed class Reflection : TableValueSource<TypeInfo, ITypeMetadata>, IReflection
	{
		public Reflection() : base(Coercer.Default.Get) {}

		sealed class Coercer : IParameterizedSource<TypeInfo, ITypeMetadata>
		{
			public static Coercer Default { get; } = new Coercer();
			Coercer() {}

			public ITypeMetadata Get(TypeInfo parameter) => new TypeMetadata(parameter);
		}
	}
}