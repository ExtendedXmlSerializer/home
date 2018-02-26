using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	class ConditionalMemberContents<T> : ConditionalSource<IMember, IContentSerializer<T>>
	{
		public ConditionalMemberContents(ISpecificationSource<MemberInfo, IContentSerializer<T>> source, IParameterizedSource<IMember, IContentSerializer<T>> fallback)
			: base(source.To(MemberMetadataCoercer.Default), source.In(MemberMetadataCoercer.Default)
			                                                       .Out(AssignedSpecification<IContentSerializer<T>>.Default, fallback), fallback) {}
	}
}
