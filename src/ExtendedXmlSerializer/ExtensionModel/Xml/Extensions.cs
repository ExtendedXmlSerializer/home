// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml.Classic;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public static class Extensions
	{
		public static IMemberConfiguration Attribute<T, TMember>(
			this MemberConfiguration<T, TMember> @this, Func<TMember, bool> when)
		{
			@this.Configuration.With<MemberFormatExtension>().Specifications[@this.Get()] =
				new AttributeSpecification(new DelegatedSpecification<TMember>(when).Adapt());
			return @this.Attribute();
		}

		public static IMemberConfiguration Attribute(this IMemberConfiguration @this)
		{
			@this.Configuration.With<MemberFormatExtension>().Registered.Add(@this.Get());
			return @this;
		}

		public static IMemberConfiguration Content(this IMemberConfiguration @this)
		{
			@this.Configuration.With<MemberFormatExtension>().Registered.Remove(@this.Get());
			return @this;
		}

		public static TypeConfiguration<T> CustomSerializer<T>(this TypeConfiguration<T> @this,
		                                                       Action<XmlWriter, T> serializer,
		                                                       Func<XElement, T> deserialize)
			=> @this.CustomSerializer(new ExtendedXmlCustomSerializer<T>(deserialize, serializer));

		public static TypeConfiguration<T> CustomSerializer<T>(this TypeConfiguration<T> @this,
		                                                       IExtendedXmlCustomSerializer<T> serializer)
		{
			@this.Configuration.With<CustomXmlExtension>().Assign(@this.Get(), new Adapter<T>(serializer));
			return @this;
		}

		public static TypeConfiguration<T> AddMigration<T>(this TypeConfiguration<T> @this,
		                                                   ICommand<XElement> migration)
			=> @this.AddMigration(migration.Execute);

		public static TypeConfiguration<T> AddMigration<T>(this TypeConfiguration<T> @this,
		                                                   Action<XElement> migration)
			=> @this.AddMigration(migration.Yield());

		public static TypeConfiguration<T> AddMigration<T>(this TypeConfiguration<T> @this,
		                                                   IEnumerable<Action<XElement>> migrations)
		{
			@this.Configuration.With<MigrationsExtension>().Add(@this.Get(), migrations.Fixed());
			return @this;
		}

		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this)
			=> @this.Extend(AutoMemberFormatExtension.Default);

		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this, int maxTextLength)
			=> @this.Extend(new AutoMemberFormatExtension(maxTextLength));

		public static IConfigurationContainer EnableClassicMode(this IConfigurationContainer @this)
			=> @this.Emit(EmitBehaviors.Classic).Extend(ClassicExtension.Default);

		public static IConfigurationContainer UseOptimizedNamespaces(this IConfigurationContainer @this)
			=> @this.Extend(OptimizedNamespaceExtension.Default);
	}
}