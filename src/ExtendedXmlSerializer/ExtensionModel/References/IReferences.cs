using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	/// <summary>
	/// Specialized selector used to access references.
	/// </summary>
	[Obsolete("This interface has been deprecated and will be removed in a future version")]
	public interface IReferences : IParameterizedSource<object, ImmutableArray<object>> {}

	// TODO

	interface IMultipleReferencesAllowed
	{
		bool Allowed { get; }
	}

	sealed class MultipleReferencesAllowed : FixedInstanceSource<bool>, IMultipleReferencesAllowed
	{
		public MultipleReferencesAllowed(bool instance) : base(instance) {}

		public bool Allowed => Get();
	}

	interface IReferenceView : IParameterizedSource<object, ReferenceResult> {}

	sealed class ReferenceView : IReferenceView
	{
		readonly ReferenceWalker _walker;

		// ReSharper disable once TooManyDependencies
		public ReferenceView(IContainsCustomSerialization custom, IReferencesPolicy policy, ITypeMembers members,
		                  IEnumeratorStore enumerators, IMemberAccessors accessors)
			: this(new ReferenceWalker(policy, new ProcessReference(custom, members, accessors, enumerators))) {}

		// ReSharper disable once TooManyDependencies
		ReferenceView(ReferenceWalker walker) => _walker = walker;

		public ReferenceResult Get(object parameter)
		{
			var result = _walker.Get(parameter);
			return result;
		}
	}
}