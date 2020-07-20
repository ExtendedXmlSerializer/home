using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue416Tests_Extended
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer(Extensions.Default.ToArray())
			                 .Type<BaseSubject>()
			                 .Member(x => x.IgnoreThisEverywhere)
			                 .Ignore() // <-- This doesn't work
			                 //.Type<ISubject>().Member(x => x.IgnoreThisEverywhere).Ignore()   // <-- This works
			                 .Type<SubjectA>()
			                 .Member(x => x.IgnoreThisForSubset)
			                 .Ignore()
			                 .Create()
			                 .ForTesting();


			var subjectA = new SubjectA
			{
				IgnoreThisEverywhere = "Should NOT see this"
			};
			var subjectB = new SubjectB {
				IgnoreThisForSubset  = "Should see this",
				IgnoreThisEverywhere = "Should NOT see this"
			};
			var parent = new Parent
			{
				IgnoreThisForSubset = "Should see this",
				Commands = new List<ISubject>{
					subjectA,
					subjectB
				}
			};

			var cycled = serializer.Cycle(parent);
			cycled.IgnoreThisEverywhere.Should().BeNull();
			cycled.Commands[0].IgnoreThisEverywhere.Should().BeNull();
			cycled.Commands[1].IgnoreThisEverywhere.Should().BeNull();

		}

		public interface ISubject
		{
			string IgnoreThisForSubset { get; set; }
			string IgnoreThisEverywhere { get; set; }
		}

		public abstract class BaseSubject : ISubject
		{
			public string IgnoreThisForSubset { get; set; }
			public string IgnoreThisEverywhere { get; set; }
		}

		public class Parent : BaseSubject
		{
			public List<ISubject> Commands { get; set; } = new List<ISubject>();
		}

		public class SubjectA : BaseSubject {}

		public class SubjectB : BaseSubject {}

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
				=> x.Name.Equals(y.Name) && _type.Equals(x.ReflectedType.GetTypeInfo(), y.ReflectedType.GetTypeInfo());

			public int GetHashCode(MemberInfo obj) => 0;
		}
	}
}