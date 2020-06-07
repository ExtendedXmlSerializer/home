using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue398Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer(Extensions.Default.ToArray()).Type<SubjectBase>()
			                                                                         .Member(x => x.Message)
			                                                                         .Ignore()
			                                                                         .Create()
			                                                                         .ForTesting();

			serializer.Assert(new Subject(),
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue398Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues"" />");
		}

		sealed class Subject : SubjectBase
		{
			public override string Message
			{
				get => "Hello World!";
				set {}
			}
		}

		public abstract class SubjectBase
		{
			public abstract string Message { get; set; }
		}

		sealed class Extensions : IEnumerable<ISerializerExtension>
		{
			public static Extensions Default { get; } = new Extensions();

			Extensions() : this(DefaultExtensions.Default) {}

			readonly IEnumerable<ISerializerExtension> _previous;

			public Extensions(IEnumerable<ISerializerExtension> previous) => _previous = previous;

			public IEnumerator<ISerializerExtension> GetEnumerator()
				=> _previous.Select(extension => extension is IAllowedMembersExtension previous
					                                 ? new AllowedMembersExtension(previous)
					                                 : extension)
				            .GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		sealed class AllowedMembersExtension : IAllowedMembersExtension
		{
			readonly IAllowedMembersExtension _previous;
			readonly IMetadataSpecification   _specification;

			public AllowedMembersExtension(IAllowedMembersExtension previous)
				: this(previous, DefaultMetadataSpecification.Default) {}

			public AllowedMembersExtension(IAllowedMembersExtension previous, IMetadataSpecification specification)
			{
				_previous      = previous;
				_specification = specification;
			}

			public IServiceRepository Get(IServiceRepository parameter)
			{
				var comparer =
					new MemberComparer(new CompositeTypeComparer(TypeIdentityComparer.Default,
					                                             AssignableFromTypeComparer.Default));

				var policy = Whitelist.Any()
					             ? (ISpecification<MemberInfo>)new AllowedPolicy(comparer, Whitelist.ToArray())
					             : new ProhibitedPolicy(comparer, Blacklist.ToArray());

				return parameter.RegisterInstance(policy.And<PropertyInfo>(_specification))
				                .RegisterInstance(policy.And<FieldInfo>(_specification));
			}

			public void Execute(IServices parameter) {}

			public ICollection<MemberInfo> Blacklist => _previous.Blacklist;

			public ICollection<MemberInfo> Whitelist => _previous.Whitelist;
		}

		sealed class AssignableFromTypeComparer : ITypeComparer
		{
			public static AssignableFromTypeComparer Default { get; } = new AssignableFromTypeComparer();

			AssignableFromTypeComparer() {}

			public bool Equals(TypeInfo x, TypeInfo y) => x.IsAssignableFrom(y);

			public int GetHashCode(TypeInfo obj) => 0;
		}

		sealed class AllowedPolicy : ContainsSpecification<MemberInfo>
		{
			public AllowedPolicy(IEqualityComparer<MemberInfo> comparer, params MemberInfo[] avoid)
				: base(new HashSet<MemberInfo>(avoid, comparer)) {}
		}

		sealed class ProhibitedPolicy : DecoratedSpecification<MemberInfo>
		{
			public ProhibitedPolicy(IEqualityComparer<MemberInfo> comparer, params MemberInfo[] avoid)
				: this(new HashSet<MemberInfo>(avoid, comparer)) {}

			ProhibitedPolicy(ICollection<MemberInfo> avoid)
				: base(new ContainsSpecification<MemberInfo>(avoid).Inverse()) {}
		}

		class ContainsSpecification<T> : DelegatedSpecification<T>
		{
			public ContainsSpecification(ICollection<T> source) : base(source.Contains) {}

			public sealed override bool IsSatisfiedBy(T parameter) => base.IsSatisfiedBy(parameter);
		}

		sealed class MemberComparer : IEqualityComparer<MemberInfo>
		{
			readonly ITypeComparer _type;

			public MemberComparer(ITypeComparer type) => _type = type;

			public bool Equals(MemberInfo x, MemberInfo y)
				=> x.Name.Equals(y.Name) && _type.Equals(x.DeclaringType.GetTypeInfo(), y.DeclaringType.GetTypeInfo());

			public int GetHashCode(MemberInfo obj) => 0;
		}
	}
}