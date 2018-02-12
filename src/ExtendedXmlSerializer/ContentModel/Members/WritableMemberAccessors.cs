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

using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	class WritableMemberAccessors : IMemberAccessors
	{
		readonly IAllowedMemberValues _emit;
		readonly IGetterFactory _getter;
		readonly ISetterFactory _setter;

		[UsedImplicitly]
		public WritableMemberAccessors(IAllowedMemberValues emit) : this(emit, SetterFactory.Default) {}

		public WritableMemberAccessors(IAllowedMemberValues emit, ISetterFactory setter)
			: this(emit, GetterFactory.Default, setter) {}

		public WritableMemberAccessors(IAllowedMemberValues emit, IGetterFactory getter, ISetterFactory setter)
		{
			_emit = emit;
			_getter = getter;
			_setter = setter;
		}

		public IMemberAccess Get(IMember parameter)
			=> new MemberAccess(_emit.Get(parameter.Metadata), _getter.Get(parameter.Metadata), _setter.Get(parameter.Metadata));
	}

	class MemberAccessors<T, TMember> : IMemberAccessors<T, TMember>
	{
		readonly IAllowedMemberValues<TMember> _emit;
		readonly IGetterFactory<T, TMember> _getter;
		readonly ISetterFactory<T, TMember> _setter;

		[UsedImplicitly]
		public MemberAccessors(IAllowedMemberValues<TMember> emit) : this(emit, SetterFactory<T, TMember>.Default) { }

		public MemberAccessors(IAllowedMemberValues<TMember> emit, ISetterFactory<T, TMember> setter)
			: this(emit, GetterFactory<T, TMember>.Default, setter) { }

		public MemberAccessors(IAllowedMemberValues<TMember> emit, IGetterFactory<T, TMember> getter, ISetterFactory<T, TMember> setter)
		{
			_emit = emit;
			_getter = getter;
			_setter = setter;
		}

		public IMemberAccess<T, TMember> Get(IMember parameter)
			=> new MemberAccess<T, TMember>(_emit.Get(parameter.Metadata), _getter.Get(parameter.Metadata), _setter.Get(parameter.Metadata));
	}

}