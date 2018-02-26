using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ContentSerializerAttribute : TypeContainerAttribute
	{
		public ContentSerializerAttribute(Type serializerType) : base(serializerType) {}
	}


	sealed class DeclaredMetadataContentExtension : ISerializerExtension, ICommand<IEnumerable<MemberInfo>>
	{
		readonly ICollection<Type> _types;

		[UsedImplicitly]
		public DeclaredMetadataContentExtension() : this(new List<Type>()) {}

		public DeclaredMetadataContentExtension(ICollection<Type> types) => _types = types;

		public IServiceRepository Get(IServiceRepository parameter)
			=> _types.Where(new ContainsSpecification<Type>(parameter.AvailableServices.Fixed()).Inverse()
			                                                                                    .IsSatisfiedBy)
			         .Select(x => new Services.Registration(x))
			         .Alter(parameter).RegisterDefinition<IMetadataContents<object>, MetadataContents<object>>()
			         .RegisterDefinition<IContents<object>, DeclaredMetadataContents<object>>()
			         .RegisterDefinition<IMemberContents<object>, DeclaredMemberMetadataContents<object>>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		public void Execute(IEnumerable<MemberInfo> parameter)
		{
			_types.Clear();
			new DeclaredMetadataTypes<ContentSerializerAttribute>(parameter).ForEach(_types.Add);
		}
	}

	sealed class DeclaredMetadataTypes<T> : ItemsBase<Type> where T : Attribute, ISource<Type>
	{
		readonly IEnumerable<MemberInfo> _members;
		readonly Func<MemberInfo, bool> _specification;

		public DeclaredMetadataTypes(IEnumerable<MemberInfo> members) : this(members, IsDefinedSpecification<T>.Default.IsSatisfiedBy) {}

		public DeclaredMetadataTypes(IEnumerable<MemberInfo> members, Func<MemberInfo, bool> specification)
		{
			_members = members;
			_specification = specification;
		}

		public override IEnumerator<Type> GetEnumerator() => _members.Where(_specification)
		                                                             .Select(x => x.GetCustomAttribute<T>()
		                                                                           .Get())
		                                                             .GetEnumerator();
	}

	interface IMetadataContents<T> : IParameterizedSource<MemberInfo, IContentSerializer<T>> { }

	sealed class MetadataContents<T> : IMetadataContents<T>
	{
		readonly IActivators<IContentSerializer<T>> _activators;
		readonly IServices _services;

		public MetadataContents(IActivators<IContentSerializer<T>> activators, IServices services)
		{
			_activators = activators;
			_services = services;
		}

		public IContentSerializer<T> Get(MemberInfo parameter)
		{
			var type = parameter.GetCustomAttribute<ContentSerializerAttribute>().Get();
			var result = _services.AvailableServices.Contains(type)
							 ? _services.Get<IContentSerializer<T>>(type)
							 : _activators.Get(type)
										  .Get();
			return result;
		}
	}

	sealed class DeclaredMetadataContents<T> : ConditionalSource<IContentSerializer<T>>, IContents<T>
	{
		public DeclaredMetadataContents(IMetadataContents<T> source, IContents<T> contents)
			: base(IsDefinedSpecification<ContentSerializerAttribute>.Default.Fix(Support<T>.Key),
			       new ConditionalInstance<IContentSerializer<T>>(AssignedSpecification<IContentSerializer<T>>.Default,
			                                                      source.Fix(Support<T>.Key),
			                                                      contents),
			       contents)
		{ }
	}

	sealed class DeclaredMemberMetadataContents<T> : ConditionalSource<IMember, IContentSerializer<T>>, IMemberContents<T>
	{
		public DeclaredMemberMetadataContents(IMetadataContents<T> source, IMemberContents<T> contents)
			: base(IsDefinedSpecification<ContentSerializerAttribute>.Default.To(MemberMetadataCoercer.Default),
				   source.In(MemberMetadataCoercer.Default)
				         .Out(AssignedSpecification<IContentSerializer<T>>.Default, contents),
			       contents) { }
	}

}