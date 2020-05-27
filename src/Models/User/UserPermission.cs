using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voartec.Models
{
    public class UserPermission
    {

        public int per_id { get; set; }
        public int per_resource_id { get; set; }
        public int per_user_id { get; set; }
        public bool per_create { get; set; }
        public bool per_read { get; set; }
        public bool per_update { get; set; }
        public bool per_delete { get; set; }

        public string resource_key_word { get; set; }
        public string resource_name { get; set; }

        public UserPermission()
        {
            SetupReferences();
        }

        public void SetupReferences()
        {
            this.per_create = false;
            this.per_read = false;
            this.per_update = false;
            this.per_delete = false;
        }

    }
}
