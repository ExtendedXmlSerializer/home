using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedMemberAccessors : IMemberAccessors
	{
		readonly IAllowedMemberValues _allow;
		readonly IMemberAccessors     _accessors;
		readonly IGetterFactory       _getter;

		public ParameterizedMemberAccessors(IAllowedMemberValues allow, IMemberAccessors accessors)
			: this(allow, accessors, GetterFactory.Default) {}

		public ParameterizedMemberAccessors(IAllowedMemberValues allow, IMemberAccessors accessors,
		                                    IGetterFactory getter)
		{
			_allow     = allow;
			_accessors = accessors;
			_getter    = getter;
		}

		public IMemberAccess Get(IMember parameter)
			=>
				parameter is ParameterizedMember
					? new MemberAccess(_allow.Get(parameter.Metadata), _getter.Get(parameter.Metadata),
					                   parameter.Metadata.Name)
					: Access(parameter);

		IMemberAccess Access(IMember parameter)
		{
			var access = _accessors.Get(parameter);
			var result = access != null ? new ActivatedMemberAccess(access, parameter.Name) : null;
			return result;
		}

		sealed class ActivatedMemberAccess : IMemberAccess
		{
			readonly IMemberAccess _member;
			readonly string        _name;

			public ActivatedMemberAccess(IMemberAccess member, string name)
			{
				_member = member;
				_name   = name;
			}

			public bool IsSatisfiedBy(object parameter) => _member.IsSatisfiedBy(parameter);

			public ISpecification<object> Instance => _member.Instance;

			public object Get(object instance) => (instance as IActivationContext)?.Get(_name) ?? _member.Get(instance);

			public void Assign(object instance, object value)
			{
				if (instance is IActivationContext context)
				{
					context.Assign(_name, value);
				}
				else
				{
					_member.Assign(instance, value);
				}
			}
		}

		sealed class MemberAccess : IMemberAccess
		{
			public ISpecification<object> Instance { get; }
			readonly ISpecification<object> _specification;
			readonly Func<object, object>   _get;
			readonly string                 _name;

			public MemberAccess(ISpecification<object> specification, Func<object, object> get, string name)
				: this(specification, specification.GetInstance(), get, name) {}

			// ReSharper disable once TooManyDependencies
			public MemberAccess(ISpecification<object> specification, ISpecification<object> instance,
			                    Func<object, object> get, string name)
			{
				Instance       = instance;
				_specification = specification;
				_get           = get;
				_name          = name;
			}

			public bool IsSatisfiedBy(object parameter) => _specification.IsSatisfiedBy(parameter);

			public object Get(object instance) => _get.Invoke(instance);

			public void Assign(object instance, object value) => (instance as IActivationContext)?.Assign(_name, value);
		}
	}
}