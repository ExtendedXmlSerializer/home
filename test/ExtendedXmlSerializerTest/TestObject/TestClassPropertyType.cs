using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization.Test.TestObject
{
    public class TestClassPropertyType
    {
        private string _onlyGetProperty;

        public string NormalProp { get; set; }
        public string OnlyGetProp => _onlyGetProperty;
        public static string StaticProp { get; set; }
        public virtual string VirtualProp { get; set; }

        public string NormalField;
        public readonly string ReadonlyField = "6";
        public const string ConstField = "7";
        public static string StaticField;


        public void Init()
        {
            NormalProp = "1";
            _onlyGetProperty = "2";
            StaticProp = "3";
            VirtualProp = "4";
            NormalField = "5";
            StaticField = "8";
        }
    }
}
