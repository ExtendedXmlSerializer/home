// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.Linq;
using ExtendedXmlSerialization.AspCore.Samples.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ExtendedXmlSerialization.AspCore.Samples.Controllers
{
    [Route("api/[controller]")]
    public class TestClassController : Controller
    {
        private static readonly List<TestClass> Data = new List<TestClass>()
        {
            new TestClass {Guid = Guid.NewGuid(), Name = "FromDb", DateTime = new DateTime(2010, 01, 01), Priority = TestClassPriority.Lowest}
        };
        // GET: api/TestClass
        [HttpGet]
        public IEnumerable<TestClass> Get()
        {
            return Data;
        }

        // GET: api/TestClass/{name}
        [HttpGet("{name}")]
        public TestClass Get(string name)
        {
            return Data.FirstOrDefault(p => p.Name == name);
        }

        // POST: api/TestClass
        [HttpPost]
        public void Post([FromBody] TestClass value)
        {
            Data.Add(value);
        }
    }
}
