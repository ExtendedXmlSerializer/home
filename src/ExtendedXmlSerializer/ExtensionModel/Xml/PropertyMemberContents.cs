using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content.Runtime;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class RuntimePropertyContentExtension : ISerializerExtension
	{
		

		public IServiceRepository Get(IServiceRepository parameter)
		{
			throw new System.NotImplementedException();
		}

		void ICommand<IServices>.Execute(IServices parameter) {}

		
	}

	interface IRuntimePropertySpecifications<T> : ISpecificationSource<MemberInfo, ISpecification<Writing<T>>> {}

	sealed class PropertyToken
	{
		public static PropertyToken Default { get; } = new PropertyToken();
		PropertyToken() {}
	}

	sealed class PropertyTokenSpecification : DecoratedSpecification<object>
	{
		public static PropertyTokenSpecification Default { get; } = new PropertyTokenSpecification();
		PropertyTokenSpecification() : base(new EqualitySpecification<object>(PropertyToken.Default)) {}
	}

	class RuntimePropertyPipelineComposer<T> : IRuntimePipelineComposer<T>
	{
		readonly ISpecification<object> _specification;
		readonly IRuntimePropertySpecifications<T> _specifications;
		readonly IRuntimeMemberWriters<T> _writers;

		public RuntimePropertyPipelineComposer(IRuntimePropertySpecifications<T> specifications, IRuntimeMemberWriters<T> writers)
			: this(PropertyTokenSpecification.Default, specifications, writers) {}

		public RuntimePropertyPipelineComposer(ISpecification<object> specification, IRuntimePropertySpecifications<T> specifications, IRuntimeMemberWriters<T> writers)
		{
			_specification = specification;
			_specifications = specifications;
			_writers = writers;
		}

		public IRuntimePipelinePart<T> Get(IRuntimePipelinePart<T> parameter)
		{
			var member = parameter.Get();
			if (_specifications.IsSatisfiedBy(member))
			{
				var specification = _specifications.Get(member.Metadata);
				var property = _writers.Get(member);
				var condition = new ConditionalContentWriter<T>(specification, property, parameter);
				var writer = new RuntimeContentWriter<T>(_specification, condition, member);
				var result = new RuntimePipelinePart<T>(writer, parameter);
				return result;
			}
			return parameter;
		}
	}

	interface IRuntimeMemberWriters<T> : IParameterizedSource<IMember, IContentWriter<T>> {}

	sealed class RuntimeMemberWriters<T> : LocatedMemberWriters<T, MemberWriters<object, object>>, IRuntimeMemberWriters<T>
	{
		public RuntimeMemberWriters(IServiceProvider provider) : base(provider) { }
	}

	sealed class MemberWriters<T, TMember> : DecoratedMemberWriters<T>
	{
		public MemberWriters(IMemberAccessors<T, TMember> accessors, PropertyMemberWriterContent<TMember> content)
			: base(new ContentModel.Members.MemberWriters<T,TMember>(accessors, content)) {}
	}


	sealed class RuntimePropertyOrderAlteration<T> : EnumerableAlterations<IMemberContentWriter<T>>, IMemberContentsAlteration<T>
	{
		public RuntimePropertyOrderAlteration(IPropertyContentSpecification specification, IMemberContentsAlteration<T> alteration)
			: base(new OrderByAlteration<IMemberContentWriter<T>, bool>(specification.To(MemberContentMetadataCoercer<T>.Default).ToDelegate()),
			       alteration)
		{ }
	}

	sealed class PropertyContentExtension : ISerializerExtension, IAssignable<MemberInfo, bool>
	{
		readonly ICollection<MemberInfo> _members;

		public PropertyContentExtension() : this(new HashSet<MemberInfo>(MemberComparer.Default)) {}

		public PropertyContentExtension(ICollection<MemberInfo> members) => _members = members;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateDefinition<IMemberWriterContent<object>, MemberWriterContent<object>>()
			            .DecorateDefinition<IMemberContentsAlteration<object>, PropertyOrderAlteration<object>>()
			            .RegisterInstance<IPropertyContentSpecification>(new PropertyContentSpecification(_members));

		void ICommand<IServices>.Execute(IServices parameter) {}

		public void Execute(KeyValuePair<MemberInfo, bool> parameter)
		{
			if (parameter.Value)
			{
				_members.Add(parameter.Key);
			}
			else
			{
				_members.Remove(parameter.Key);
			}
		}
	}

	sealed class PropertyOrderAlteration<T> : EnumerableAlterations<IMemberContentWriter<T>>, IMemberContentsAlteration<T>
	{
		public PropertyOrderAlteration(IPropertyContentSpecification specification, IMemberContentsAlteration<T> alteration)
			: base(new OrderByAlteration<IMemberContentWriter<T>, bool>(specification.To(MemberContentMetadataCoercer<T>.Default).ToDelegate()),
			       alteration) {}
	}

	sealed class MemberContentMetadataCoercer<T> : IParameterizedSource<IMemberContentWriter<T>, MemberInfo>
	{
		public static MemberContentMetadataCoercer<T> Default { get; } = new MemberContentMetadataCoercer<T>();
		MemberContentMetadataCoercer() {}

		public MemberInfo Get(IMemberContentWriter<T> parameter) => parameter.Get().Metadata;
	}



	interface IPropertyContentSpecification : ISpecification<MemberInfo> {}

	sealed class PropertyContentSpecification : ContainsSpecification<MemberInfo>, IPropertyContentSpecification
	{
		public PropertyContentSpecification(ICollection<MemberInfo> source) : base(source) {}
	}

	sealed class MemberWriterContent<T> : ConditionalSource<IMember, IContentWriter<T>>, IMemberWriterContent<T>
	{
		public MemberWriterContent(IPropertyContentSpecification specification, PropertyMemberWriterContent<T> source,
		                           IMemberWriterContent<T> fallback)
			: base(specification.To(MemberMetadataCoercer.Default), source, fallback) {}
	}

	sealed class PropertyMemberWriterContent<T> : IMemberWriterContent<T>
	{
		readonly IMemberConverters<T> _converters;

		public PropertyMemberWriterContent(IMemberConverters<T> converters) => _converters = converters;

		public IContentWriter<T> Get(IMember parameter) => new ConverterProperty<T>(_converters.Get(parameter.Metadata), parameter);
	}
}
