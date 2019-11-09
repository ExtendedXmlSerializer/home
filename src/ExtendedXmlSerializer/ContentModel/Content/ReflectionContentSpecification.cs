using System;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class ReflectionContentSpecification : AnySpecification<TypeInfo>
	{
		public static ReflectionContentSpecification Default { get; } = new ReflectionContentSpecification();

		ReflectionContentSpecification() : base(new[] {typeof(TypeInfo), typeof(Type)}
		                                        .YieldMetadata()
		                                        .Select(IsAssignableSpecification.Defaults.Get)
		                                        .ToArray()) {}
	}
}