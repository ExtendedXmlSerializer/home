using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class PropertyContentExtension : ISerializerExtension
	{
		readonly ICollection<MemberInfo> _members;

		public PropertyContentExtension() : this(new HashSet<MemberInfo>(MemberComparer.Default)) {}

		public PropertyContentExtension(ICollection<MemberInfo> members)
			: this(members, new AddCommand<MemberInfo>(members), new RemoveCommand<MemberInfo>(members)) {}

		public PropertyContentExtension(ICollection<MemberInfo> members, ICommand<MemberInfo> add, ICommand<MemberInfo> remove)
		{
			Add = add;
			Remove = remove;
			_members = members;
		}

		public ICommand<MemberInfo> Add { get; }

		public ICommand<MemberInfo> Remove { get; }


		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.DecorateDefinition<IMemberWriterContent<object>, MemberWriterContent<object>>()
			            .DecorateDefinition<IMemberContentsAlteration<object>, PropertyOrderAlteration<object>>()
			            .RegisterInstance<IPropertyContentSpecification>(new PropertyContentSpecification(_members));

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	sealed class PropertyOrderAlteration<T> : EnumerableAlterations<IMemberContentWriter<T>>, IMemberContentsAlteration<T>
	{
		public PropertyOrderAlteration(IPropertyContentSpecification specification, IMemberContentsAlteration<T> alteration)
			: base(new OrderByAlteration<IMemberContentWriter<T>, bool>(specification.To(MemberMetadataCoercer.Default
			                                                                                                  .In(SourceCoercer<IMember>.Default))
			                                                                         .ToSpecificationDelegate()),
			       alteration) {}
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
		/*readonly IParameterizedSource<IMember, IConvert<T>> _converters;

		public PropertyMemberWriterContent(IRegisteredSerializers<T> converters) : this(converters.In(MemberMetadataCoercer.Default)) {}

		public PropertyMemberWriterContent(IParameterizedSource<IMember, IConvert<T>> converters) => _converters = converters;

		public IContentWriter<T> Get(IMember parameter) => new ConverterProperty<T>(_converters.Get(parameter), parameter);*/
		public IContentWriter<T> Get(IMember parameter)
		{
			return null;
		}
	}
}
