using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	class ConditionalMemberContents<T> : ConditionalSource<IMember, IContentSerializer<T>>
	{
		public ConditionalMemberContents(ISpecificationSource<MemberInfo, IContentSerializer<T>> source, IParameterizedSource<IMember, IContentSerializer<T>> fallback)
			: this(source, source, fallback) {}

		public ConditionalMemberContents(ISpecification<MemberInfo> specification, IParameterizedSource<MemberInfo, IContentSerializer<T>> source, IParameterizedSource<IMember, IContentSerializer<T>> fallback)
			: this(new SpecificationSource<IMember, IContentSerializer<T>>(specification.To(MemberMetadataCoercer.Default), source.In(MemberMetadataCoercer.Default)), fallback) {}

		public ConditionalMemberContents(ISpecificationSource<IMember, IContentSerializer<T>> source, IParameterizedSource<IMember, IContentSerializer<T>> fallback) : this(source, AssignedSpecification<IContentSerializer<T>>.Default, fallback) {}
		public ConditionalMemberContents(ISpecificationSource<IMember, IContentSerializer<T>> source, ISpecification<IContentSerializer<T>> result, IParameterizedSource<IMember, IContentSerializer<T>> fallback) : this(source, result, source, fallback) {}

		public ConditionalMemberContents(ISpecification<IMember> specification, ISpecification<IContentSerializer<T>> result,
		                                 IParameterizedSource<IMember, IContentSerializer<T>> source,
		                                 IParameterizedSource<IMember, IContentSerializer<T>> fallback) : base(specification,
		                                                                                                       result, source,
		                                                                                                       fallback) {}
	}
}
