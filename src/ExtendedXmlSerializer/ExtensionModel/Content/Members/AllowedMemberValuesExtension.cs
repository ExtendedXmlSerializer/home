using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	/// <summary>
	/// Default serializer extension that configures when to allow, emit, and read values of members.
	/// </summary>
	public sealed class AllowedMemberValuesExtension : Collection<IAllowedMemberValues>, ISerializerExtension
	{
		readonly static AllowAssignedValues AllowAssignedValues = AllowAssignedValues.Default;

		readonly IAllowedValueSpecification _allowed;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExtendedXmlSerializer.ExtensionModel.Content.Members.AllowedMemberValuesExtension"/> class.
		/// </summary>
		public AllowedMemberValuesExtension() : this(AllowAssignedValues) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExtendedXmlSerializer.ExtensionModel.Content.Members.AllowedMemberValuesExtension"/> class.
		/// </summary>
		/// <param name="allowed">The allowed.</param>
		public AllowedMemberValuesExtension(IAllowedValueSpecification allowed)
			: this(allowed, new Dictionary<MemberInfo, IAllowedValueSpecification>()) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExtendedXmlSerializer.ExtensionModel.Content.Members.AllowedMemberValuesExtension"/> class.
		/// </summary>
		/// <param name="allowed">The allowed.</param>
		/// <param name="specifications">The specifications.</param>
		/// <param name="items">The items.</param>
		public AllowedMemberValuesExtension(IAllowedValueSpecification allowed,
		                                    IDictionary<MemberInfo, IAllowedValueSpecification> specifications,
		                                    params IAllowedMemberValues[] items) : base(items.ToList())
		{
			_allowed       = allowed;
			Specifications = specifications;
		}

		/// <summary>
		/// Registry of allowed value specifications, keyed by member metadata.
		/// </summary>
		public IDictionary<MemberInfo, IAllowedValueSpecification> Specifications { get; }

		/// <summary>
		/// Registry of instance value specifications, keyed by member metadata.
		/// </summary>
		public IDictionary<MemberInfo, ISpecification<object>> Instances { get; }
			= new Dictionary<MemberInfo, ISpecification<object>>();

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter) => parameter.Register(Register);

		IAllowedMemberValues Register(IServiceProvider arg)
		{
			IParameterizedSource<MemberInfo, IAllowedValueSpecification>
				seed = new MappedAllowedMemberValues(Specifications
				                                     .ToDictionary(x => x.Key, x => (ISpecification<object>)x.Value)
				                                     .Concat(Instances)
				                                     .GroupBy(x => x.Key)
				                                     .ToDictionary(x => x.Key, Create, Defaults.MemberComparer)),
				fallback = _allowed == AllowAssignedValues
					           ? Source.Default
					           : new FixedInstanceSource<MemberInfo, IAllowedValueSpecification>(_allowed);
			var source = this.Appending(fallback)
			                 .Aggregate(seed, (current, item) => current.Or(item));
			var result = new AllowedMemberValues(source);
			return result;
		}

		sealed class Source : IParameterizedSource<MemberInfo, IAllowedValueSpecification>
		{
			public static IParameterizedSource<MemberInfo, IAllowedValueSpecification> Default { get; } = new Source();

			Source() : this(AllowAssignedValues.Default,
			                new Generic<ISpecification<object>, ISpecification<object>>(typeof(Specification<>)),
			                CollectionItemTypeLocator.Default) {}

			readonly IAllowedValueSpecification                               _allowed;
			readonly IGeneric<ISpecification<object>, ISpecification<object>> _generic;
			readonly IParameterizedSource<TypeInfo, TypeInfo>                 _type;

			public Source(IAllowedValueSpecification allowed,
			              IGeneric<ISpecification<object>, ISpecification<object>> generic,
			              IParameterizedSource<TypeInfo, TypeInfo> type)
			{
				_allowed = allowed;
				_generic = generic;
				_type    = type;
			}

			public IAllowedValueSpecification Get(MemberInfo parameter) => From(parameter);

			IAllowedValueSpecification From(MemberDescriptor descriptor)
			{
				var result = IsCollectionTypeSpecification.Default.IsSatisfiedBy(descriptor.MemberType)
					             ? new AllowedValueSpecification(_generic
					                                             .Get(_type.Get(descriptor.MemberType))
					                                             .Invoke(_allowed))
					             : _allowed;
				return result;
			}
		}

		sealed class Specification<T> : ISpecification<object>
		{
			readonly ISpecification<object> _previous;

			public Specification(ISpecification<object> previous) => _previous = previous;

			public bool IsSatisfiedBy(object parameter)
				=> parameter is ICollection<T> generic
					   ? generic.Any()
					   : parameter is ICollection collection
						   ? collection.Count > 0
						   : _previous.IsSatisfiedBy(parameter);
		}

		IAllowedValueSpecification Create(
			IGrouping<MemberInfo, KeyValuePair<MemberInfo, ISpecification<object>>> parameter)
		{
			var item = parameter.Select(x => x.Value)
			                    .ToArray();

			var only = item.Only();
			if (only != null)
			{
				return new InstanceAwareValueSpecification(only is IAllowedValueSpecification allow ? allow : _allowed,
				                                           Instances.ContainsKey(parameter.Key)
					                                           ? Instances[parameter.Key]
					                                           : AlwaysSpecification<object>.Default);
			}

			return new InstanceAwareValueSpecification(item.First(), item.Last());
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}