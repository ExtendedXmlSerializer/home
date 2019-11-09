using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Members
{
	public class BlacklistMemberPolicyTests
	{
		[Fact]
		public void Blacklist()
		{
			const string name      = nameof(IDictionary<object, object>.Keys);
			var          ignore    = typeof(IDictionary<,>).GetRuntimeProperty(name);
			var          sut       = new BlacklistMemberPolicy(ignore);
			var          instance  = new Dictionary<string, int>();
			var          type      = instance.GetType();
			var          candidate = type.GetRuntimeProperty(name);
			var          allowed   = sut.IsSatisfiedBy(candidate);
			Assert.False(allowed);

			foreach (var property in type.GetRuntimeProperties()
			                             .Except(candidate.Yield()))
			{
				Assert.True(sut.IsSatisfiedBy(property));
			}
		}

		[Fact]
		public void Inherited()
		{
			const string name      = nameof(IDictionary<object, object>.Keys);
			var          ignore    = typeof(IDictionary<,>).GetRuntimeProperty(name);
			var          sut       = new BlacklistMemberPolicy(ignore);
			var          instance  = new Dictionary();
			var          type      = instance.GetType();
			var          candidate = type.GetRuntimeProperty(name);
			var          allowed   = sut.IsSatisfiedBy(candidate);
			Assert.False(allowed);
		}

		class Dictionary : Dictionary<string, string> {}

		[Fact]
		void Overrides()
		{
			var member = Get<Button, bool>(x => x.AutoSize);

			var list = new HashSet<MemberInfo>(MemberComparer.Default) {member};

			list.Contains(typeof(Button).GetTypeInfo()
			                            .GetRuntimeProperty(nameof(Control.AutoSize)))
			    .Should()
			    .BeTrue();

			list.Contains(typeof(ButtonBase).GetTypeInfo()
			                                .GetRuntimeProperty(nameof(Control.AutoSize)))
			    .Should()
			    .BeTrue();

			list.Contains(typeof(Control).GetTypeInfo()
			                             .GetRuntimeProperty(nameof(Control.AutoSize)))
			    .Should()
			    .BeFalse();
		}

		static MemberInfo Get<T, TMember>(Expression<Func<T, TMember>> member) => member.GetMemberInfo();

		class ButtonBase : Control
		{
			// ReSharper disable once RedundantOverriddenMember
			public override bool AutoSize
			{
				get => base.AutoSize;
				set => base.AutoSize = value;
			}
		}

		class Control
		{
			public virtual bool AutoSize { get; set; }
		}

		class Button : ButtonBase
		{
			[UsedImplicitly]
			public string Name { get; set; }

			[UsedImplicitly]
			public DialogResult DialogResult { get; set; } = DialogResult.None;

			[UsedImplicitly]
			public Cursor Cursor { get; set; }
		}

		sealed class Cursor
		{
			Cursor() {}
		}

		public enum DialogResult
		{
			None,
			OK,
			Cancel,
			Abort,
			Retry,
			Ignore,
			Yes,
			No,
		}
	}
}