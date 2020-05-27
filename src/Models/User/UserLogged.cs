using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voartec.Models
{
    public class UserLogged
    {
        public int id { get; set; }
        public int entity { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string sector { get; set; }
        public string function { get; set; }
        public string phone { get; set; }
        public string image { get; set; }
        public string entity_name { get; set; }
        public string date { get; set; }
        public string hour { get; set; }
        public string hour_format { get; set; }
        public string token { get; set; }

        public List<UserConfig> configs { get; set; }

        public UserLogged()
        {
            SetupReferences();
        }

        public void SetupReferences()
        {
            this.date = DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00");
            this.hour = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00");
        }

    }
}
