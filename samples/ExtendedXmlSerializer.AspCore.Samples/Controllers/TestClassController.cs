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
