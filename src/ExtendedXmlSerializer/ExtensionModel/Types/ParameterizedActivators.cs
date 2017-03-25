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
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ParameterizedActivators : IActivators
	{
		readonly IActivators _activators;
		readonly IParameterizedConstructors _constructors;
		readonly IMemberAccessors _accessors;
		readonly IConstructorMembers _members;

		public ParameterizedActivators(IActivators activators, IParameterizedConstructors constructors,
		                               IConstructorMembers members, IMemberAccessors accessors)
		{
			_activators = activators;
			_constructors = constructors;
			_members = members;
			_accessors = accessors;
		}

		public Func<object> Get(Type parameter)
		{
			var typeInfo = parameter.GetTypeInfo();
			var constructor = _constructors.Get(parameter);
			var members = constructor != null ? _members.Get(constructor) : null;
			var result = members != null
				? new Activator(_accessors, constructor, members.GetValueOrDefault()).ToDelegate()
				: _activators.Get(typeInfo);
			return result;
		}

		sealed class Activator : ISource<object>
		{
			readonly IMemberAccessors _accessors;
			readonly ConstructorInfo _constructor;
			readonly ImmutableArray<IMember> _members;

			public Activator(IMemberAccessors accessors, ConstructorInfo constructor, ImmutableArray<IMember> members)
			{
				_accessors = accessors;
				_constructor = constructor;
				_members = members;
			}

			public object Get() => new ActivationContext(_accessors, _constructor, _members);
		}
	}
}