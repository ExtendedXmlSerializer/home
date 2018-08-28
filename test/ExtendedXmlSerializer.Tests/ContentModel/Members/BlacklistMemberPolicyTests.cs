// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using FluentAssertions;
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
			const string name = nameof(IDictionary<object, object>.Keys);
			var ignore = typeof(IDictionary<,>).GetRuntimeProperty(name);
			var sut = new BlacklistMemberPolicy(ignore);
			var instance = new Dictionary<string, int>();
			var type = instance.GetType();
			var candidate = type.GetRuntimeProperty(name);
			var allowed = sut.IsSatisfiedBy(candidate);
			Assert.False(allowed);

			foreach (var property in type.GetRuntimeProperties().Except(candidate.Yield()))
			{
				Assert.True(sut.IsSatisfiedBy(property));
			}
		}

		[Fact]
		public void Inherited()
		{
			const string name = nameof(IDictionary<object, object>.Keys);
			var ignore = typeof(IDictionary<,>).GetRuntimeProperty(name);
			var sut = new BlacklistMemberPolicy(ignore);
			var instance = new Dictionary();
			var type = instance.GetType();
			var candidate = type.GetRuntimeProperty(name);
			var allowed = sut.IsSatisfiedBy(candidate);
			Assert.False(allowed);
		}
		class Dictionary : Dictionary<string, string> {}

		[Fact]
		void Overrides()
		{
			var member = Get<Button, bool>(x => x.AutoSize);

			var list = new HashSet<MemberInfo>(MemberComparer.Default) { member };

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
			public string Name { get; set; }

			public DialogResult DialogResult { get; set; } = DialogResult.None;

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