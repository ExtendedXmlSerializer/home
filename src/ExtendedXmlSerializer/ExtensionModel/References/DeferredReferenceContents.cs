using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceContents : IContents
	{
		readonly static IsCollectionTypeSpecification IsCollectionTypeSpecification =
			IsCollectionTypeSpecification.Default;

		readonly ISpecification<TypeInfo> _specification;
		readonly IRootReferences          _references;
		readonly IContents                _contents;

		[UsedImplicitly]
		public DeferredReferenceContents(IRootReferences references, IContents contents)
			: this(IsCollectionTypeSpecification, references, contents) {}

		public DeferredReferenceContents(ISpecification<TypeInfo> specification, IRootReferences references,
		                                 IContents contents)
		{
			_specification = specification;
			_references    = references;
			_contents      = contents;
		}

		public ContentModel.ISerializer Get(TypeInfo parameter)
		{
			var serializer = _contents.Get(parameter);
			var result = serializer is ReferenceSerializer && _specification.IsSatisfiedBy(parameter)
				             ? new DeferredReferenceContent(_references, serializer)
				             : serializer;
			return result;
		}
	}
}