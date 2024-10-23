using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel;

sealed class GeneratedListAwareExtension : ISerializerExtension
{
	public static GeneratedListAwareExtension Default { get; } = new();

	GeneratedListAwareExtension() {}

	public IServiceRepository Get(IServiceRepository parameter)
		=> parameter.Decorate<ITypePartResolver, TypePartResolver>();

	public void Execute(IServices parameter) {}

	sealed class TypePartResolver : ITypePartResolver
	{
		readonly ITypePartResolver        _previous;
		readonly ISpecification<TypeInfo> _specification;
		readonly IAlteration<Type>        _alter;

		public TypePartResolver(ITypePartResolver previous)
			: this(previous, IsGeneratedList.Default, GeneratedSubstitute.Default) {}

		public TypePartResolver(ITypePartResolver previous, ISpecification<TypeInfo> specification,
		                        IAlteration<Type> alter)
		{
			_previous      = previous;
			_specification = specification;
			_alter         = alter;
		}

		public TypeParts Get(TypeInfo parameter)
		{
			var type   = _specification.IsSatisfiedBy(parameter) ? _alter.Get(parameter) : parameter;
			var result = _previous.Get(type);
			return result;
		}
	}
}