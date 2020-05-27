using System;
using System.Collections;
using System.Collections.Generic;
using Voartec.Config;

namespace Voartec.Helpers
{
    public class ErrorHandler
    {
        public string DealError(Exception ex)
        {
            var c = Builder.GetConfiguration();

            // --> enviar erro por email, gravar em aquivo txt

            if (c.GetSection("environment").Value == "dev")
            {
                return ex.Message + "StackTraceString: " + ex.StackTrace;
                
            }
            else
            {
                return ex.Message;
            }
        }

        public string DealErrorList(List<Exception> errors)
        {
            var c = Builder.GetConfiguration();
            string line = "";

            foreach (Exception e in errors)
            {
                line += e.Message + "  StackTraceString: " + e.StackTrace + "\r\n";
            }


            //enviar erro por email, gravar em aquivo txt
            if (c.GetSection("environment").Value == "dev")
            {
                return line;
            }
            else
            {
                return "An internal error has occurred :(";
            }
        }

        public string DealErrorModelState(dynamic errors)
        {
            List<Exception> messages = new List<Exception>();
            foreach (Exception e in errors)
            {
                messages.Add(e);
            }
            return this.DealErrorList(messages);
        }

    }
}