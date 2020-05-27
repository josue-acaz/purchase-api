using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Helpers
{
    public class PaginationResult
    {
        public string resultStatus { get; set; }
        public List<string> resultMessages { get; set; }
        public int totalPages { get; set; }
        public int totalRecords { get; set; }
        public List<object> data { get; set; }
        public object resume { get; set; }
    }
}