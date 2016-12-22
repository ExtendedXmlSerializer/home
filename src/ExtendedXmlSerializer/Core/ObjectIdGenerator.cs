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
using System.Runtime.Serialization;
using ExtendedXmlSerialization.Processing.Write;

namespace ExtendedXmlSerialization.Core
{
    /// <summary>
    /// Attribution: https://msdn.microsoft.com/en-us/library/system.runtime.serialization.objectmanager(v=vs.110).aspx
    /// </summary>
    public class ObjectIdGenerator
    {
        private static readonly int[] Sizes = new int[21]
                                              {
                                                  5, 11, 29, 47, 97, 197, 397, 797, 1597, 3203, 6421, 12853, 25717,
                                                  51437,
                                                  102877, 205759, 411527, 823117, 1646237, 3292489, 6584983
                                              };


        int _currentCount, _currentSize;
        long[] _ids;
        object[] _objs;

        /// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" /> class.</summary>
        public ObjectIdGenerator()
        {
            _currentCount = 1;
            _currentSize = Sizes[0];
            _ids = new long[_currentSize*4];
            _objs = new object[_currentSize*4];
        }

        private int FindIndex(object obj, out bool found)
        {
            int hashCode = RuntimeHelpers.GetHashCode(obj);
            int num1 = 1 + (hashCode & int.MaxValue)%(_currentSize - 2);
            while (true)
            {
                int num2 = (hashCode & int.MaxValue)%_currentSize*4;
                for (int index = num2; index < num2 + 4; ++index)
                {
                    var o = _objs[index];
                    if (o == null)
                    {
                        found = false;
                        return index;
                    }
                    if (o == obj)
                    {
                        found = true;
                        return index;
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
        public virtual IdentityGenerationContext For(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj", ("ArgumentNull_Obj"));
            bool found;
            int index = FindIndex(obj, out found);
            long id;
            if (!found)
            {
                _objs[index] = obj;
                long[] ids = _ids;
                int currentCount = _currentCount;
                _currentCount = currentCount + 1;
                long num = (long) currentCount;
                ids[index] = num;
                id = _ids[index];
                if (_currentCount > _currentSize*4/2)
                    Rehash();
            }
            else
                id = _ids[index];
            var result = new IdentityGenerationContext(id, obj, !found);
            return result;
        }

        /*/// <summary>Determines whether an object has already been assigned an ID.</summary>
        /// <returns>The object ID of <paramref name="obj" /> if previously known to the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" />; otherwise, null.</returns>
        /// <param name="obj">The object you are asking for. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="obj" /> parameter is null. </exception>
        public virtual long? Locate(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj", ("ArgumentNull_Obj"));
            bool found;
            var index = FindIndex(obj, out found);
            var result = found ? (long?) _ids[index] : null;
            return result;
        }*/

        public virtual bool Contains(object obj)
        {
            bool result;
            FindIndex(obj, out result);
            return result;
        }

        private void Rehash()
        {
            int index1 = 0;
            int currentSize = _currentSize;
            while (index1 < Sizes.Length && Sizes[index1] <= currentSize)
                ++index1;
            if (index1 == Sizes.Length)
                throw new SerializationException("Serialization_TooManyElements");
            _currentSize = Sizes[index1];
            long[] numArray = new long[_currentSize*4];
            object[] objArray = new object[_currentSize*4];
            long[] ids = _ids;
            object[] objs = _objs;
            _ids = numArray;
            _objs = objArray;
            for (int index2 = 0; index2 < objs.Length; ++index2)
            {
                if (objs[index2] != null)
                {
                    bool found;
                    int element = FindIndex(objs[index2], out found);
                    _objs[element] = objs[index2];
                    _ids[element] = ids[index2];
                }
            }
        }
    }
}