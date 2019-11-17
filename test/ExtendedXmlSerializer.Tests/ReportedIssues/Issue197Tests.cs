using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue197Tests
	{
		[Fact]
		void Verify()
		{
			var btn = new Button {Name = "Testing?", DialogResult = DialogResult.Abort};
			var serializer = new ConfigurationContainer().Extend(FallbackSerializationExtension.Default)
			                                             .Create()
			                                             .ForTesting();

			var button = serializer.Cycle(btn);
			button.Name.Should()
			      .Be(btn.Name);
			button.DialogResult.Should().BeEquivalentTo(btn.DialogResult);
		}

		[Fact]
		void VerifyIgnore()
		{
			new ConfigurationContainer().Extend(FallbackSerializationExtension.Default)
			                            .Type<Button>()
			                            .Member(m => m.AutoSize)
			                            .Ignore()
			                            .Create()
			                            .ForTesting()
			                            .Assert(new Button(),
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue197Tests-Button xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><DialogResult>None</DialogResult></Issue197Tests-Button>");
			new ConfigurationContainer().Extend(FallbackSerializationExtension.Default)
			                            .Type<Button>()
			                            .Create()
			                            .ForTesting()
			                            .Assert(new Button(),
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue197Tests-Button xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><AutoSize>false</AutoSize><DialogResult>None</DialogResult></Issue197Tests-Button>");
		}

		[Fact]
		void VerifyProperties()
		{
			var buttonEx2 = new ButtonEx2 {MyProperty = 1.0};
			var serializer = new ConfigurationContainer().Extend(FallbackSerializationExtension.Default)
			                                             .Type<ButtonExBase2>()
			                                             .Member(x => x.MyProperty)
			                                             .Ignore()
			                                             .Create()
			                                             .ForTesting();
			serializer.Cycle(buttonEx2)
			          .Should()
			          .BeEquivalentTo(buttonEx2);
		}

		[Fact]
		void Properties2()
		{
			var prop1 = new PropertyBase {RefID                      = 1};
			var prop2 = new PropertyInherit {RefID                   = 2};
			var hand1 = new PropertyBaseHandle {HandleToIProperty    = prop1};
			var hand2 = new PropertyInheritHandle {HandleToIProperty = prop2};

			var obj = new TestIProperty();
			obj.MyProperties.Add(prop1);
			obj.MyProperties.Add(prop2);
			obj.MyHandles.Add(hand1);
			obj.MyHandles.Add(hand2);

			new ConfigurationContainer().Extend(FallbackSerializationExtension.Default)
			                            .Type<IProperty>()
			                            .EnableReferences(p => p.RefID)
			                            .Type<IPropertyHandleBase>()
			                            .Member(x => x.HandleToIProperty)
			                            .Ignore()
			                            .Create()
			                            .ForTesting()
			                            .Cycle(obj)
			                            .Should()
			                            .BeEquivalentTo(obj);
		}

		//Interface implementations
		public interface IProperty
		{
			int MyProperty { get; set; }
			int RefID { get; set; }
		}

		public class PropertyBase : IProperty
		{
			public virtual int MyProperty { get; set; } = 1;
			public int RefID { get; set; }
		}

		public class PropertyInherit : PropertyBase
		{
			public override int MyProperty { get; set; } = 2;
		}

//Class-handles to interface
		public class IPropertyHandleBase
		{
			public IProperty HandleToIProperty { get; set; }
		}

		public class PropertyBaseHandle : IPropertyHandleBase
		{
			public new PropertyBase HandleToIProperty
			{
				get => base.HandleToIProperty as PropertyBase;
				set => base.HandleToIProperty = value;
			}
		}

		public class PropertyInheritHandle : PropertyBaseHandle
		{
			public new PropertyInherit HandleToIProperty
			{
				get => base.HandleToIProperty as PropertyInherit;
				set => base.HandleToIProperty = value;
			}
		}

//Object to serialize/deserialize
		public class TestIProperty
		{
			// ReSharper disable once CollectionNeverQueried.Global
			public List<IProperty> MyProperties { get; set; } = new List<IProperty>();

			// ReSharper disable once CollectionNeverQueried.Global
			public List<IPropertyHandleBase> MyHandles { get; set; } = new List<IPropertyHandleBase>();
		}

		public class ButtonExBase2
		{
			public int MyProperty { get; [UsedImplicitly] set; }
		}

		public class ButtonEx2 : ButtonExBase2
		{
			public new double MyProperty { [UsedImplicitly] get; set; }
		}

		[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
		public sealed class ButtonExPropertyAttribute : Attribute {}

		class ButtonExBase : Button
		{
			[ButtonExProperty, UsedImplicitly]
			public int FirstPropButtonExBase { get; set; }

			[UsedImplicitly]
			public int SecondPropButtonExBase { get; set; }
		}

		[UsedImplicitly]
		class ButtonEx : ButtonExBase
		{
			[ButtonExProperty, UsedImplicitly]
			public int PropButtonEx { get; set; }
		}

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
			public string Name { get; set; }

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