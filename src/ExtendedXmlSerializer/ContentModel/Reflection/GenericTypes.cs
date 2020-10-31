using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class GenericTypes : StructureCache<IIdentity, ImmutableArray<TypeInfo>>, IGenericTypes
	{
		[UsedImplicitly]
		public GenericTypes(IActivatingTypeSpecification specification, ITypeFormatter formatter)
			: this(IsGenericTypeSpecification.Default.And(specification), formatter) {}

		GenericTypes(ISpecification<TypeInfo> specification, ITypeFormatter formatter)
			: base(
			       new TypeCandidates(specification, formatter,
			                          new AssemblyTypePartitions(specification, formatter.Get), TypeLoader.Default)
				       .Get) {}
	}
}