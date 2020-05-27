using System;

namespace Voartec.Helpers
{
    public class Log
    {
        public int id { get; set; }
        public int userId { get; set; }
        public DateTime date { get; set; }
        public string hour { get; set; }
        public string resource { get; set; }
        public string action { get; set; }
        public string registerKey { get; set; }
        public string registerCopy { get; set; }

        public Log(int userId, String resource, String action, String registerKey, String registerCopy)
        {
            string mn = DateTime.Now.Minute.ToString();
            mn = mn.PadLeft(2, '0');

            string hr = DateTime.Now.Hour.ToString();
            hr = hr.PadLeft(2, '0');

            this.userId = userId;
            this.date = DateTime.Now;
            this.hour = hr + ":" + mn;
            this.resource = resource;
            this.action = action;
            this.registerKey = registerKey;
            this.registerCopy = registerCopy;
        }
    }
}
