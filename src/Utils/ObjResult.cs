using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Helpers
{
    public class ObjResult
    {
        public string resultStatus { get; set; }
        public List<string> resultMessages { get; set; }
        public Object data { get; set; }
        public Object resume { get; set; }
        public Object resume2 { get; set; }

        public void Success()
        {
            this.resultStatus = "success";
        }

        public void Error()
        {
            this.resultStatus = "error";
        }

        public void SetData(Object obj)
        {
            this.data = obj;
        }
    }
}