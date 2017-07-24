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
using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.Core
{
	/// <summary>
	/// Attribution: https://msdn.microsoft.com/en-us/library/system.runtime.serialization.objectmanager(v=vs.110).aspx
	/// </summary>
	sealed class ObjectIdGenerator
	{
		readonly static uint[] Sizes =
		{
			5, 11, 29, 47, 97, 197, 397, 797, 1597, 3203, 6421, 12853, 25717, 51437, 102877, 205759, 411527, 823117,
			1646237, 3292489, 6584983
		};

		uint _currentCount, _currentSize;
		uint[] _ids;
		object[] _objs;

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" /> class.</summary>
		public ObjectIdGenerator()
		{
			_currentCount = 1;
			_currentSize = Sizes[0];
			_ids = new uint[_currentSize * 4];
			_objs = new object[_currentSize * 4];
		}

		uint FindIndex(object obj, out bool found)
		{
			var hashCode = RuntimeHelpers.GetHashCode(obj);
			var num1 = (int) (1 + (hashCode & int.MaxValue) % (_currentSize - 2));
			while (true)
			{
				var num2 = (hashCode & int.MaxValue) % _currentSize * 4;
				for (var index = num2; index < num2 + 4; ++index)
				{
					var o = _objs[index];
					if (o == null)
					{
						found = false;
						return (uint) index;
					}
					if (o == obj)
					{
						found = true;
						return (uint) index;
					}
				}
				hashCode += num1;
			}
		}

		/// <summary>Returns the ID for the specified object, generating a new ID if the specified object has not already been identified by the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" />.</summary>
		/// <returns>The object's identity context, which can be used for serialization. FirstEncounter is set to true if this is the first time the object has been identified; otherwise, it is set to false.</returns>
		/// <param name="obj">The object you want an ID for. </param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="obj" /> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" /> has been asked to keep track of too many objects. </exception>
		public uint For(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException(nameof(obj), "ArgumentNull_Obj");
			bool found;
			var index = FindIndex(obj, out found);
			var result = found ? _ids[index] : Create(obj, index);
			return result;
		}

		uint Create(object obj, uint index)
		{
			uint id;
			_objs[index] = obj;
			var ids = _ids;
			var count = _currentCount;
			_currentCount = count + 1;
			ids[index] = count;
			id = _ids[index];
			if (_currentCount > _currentSize * 4 / 2)
				Rehash();
			return id;
		}

		public bool Contains(object obj)
		{
			bool result;
			FindIndex(obj, out result);
			return result;
		}

		void Rehash()
		{
			var index1 = 0;
			var currentSize = _currentSize;
			while (index1 < Sizes.Length && Sizes[index1] <= currentSize)
				++index1;
			if (index1 == Sizes.Length)
				throw new InvalidOperationException("Serialization_TooManyElements");
			_currentSize = Sizes[index1];
			var numArray = new uint[_currentSize * 4];
			var objArray = new object[_currentSize * 4];
			var ids = _ids;
			var objs = _objs;
			_ids = numArray;
			_objs = objArray;
			for (var index2 = 0; index2 < objs.Length; ++index2)
			{
				if (objs[index2] != null)
				{
					var element = FindIndex(objs[index2], out _);
					_objs[element] = objs[index2];
					_ids[element] = ids[index2];
				}
			}
		}
	}
}