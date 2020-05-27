using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Helpers
{
    public class ParamSql
    {
        public string name { get; set; }
        public string value { get; set; }

        public ParamSql(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}