using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Models
{
    public class Request
    {
        public int req_id { get; set; }
        public int req_user_id { get; set; }
        public DateTime? req_sent_date_hour { get; set; }
        public string req_description { get; set; }
        public string req_application { get; set; }
        public string req_priority { get; set; }
        public DateTime req_deadline { get; set; }
        /// <summary>Status possíveis: | Em digitação (E), Ativa (A), Cancelada (C), Aprovada (AP), Aprovada Parcialmente (PAP), Não Aprovada (NAP) |</sumary>
        public string req_status { get; set; }
        public bool req_active { get; set; }
        public bool req_excluded { get; set; }

        /// <summary>
        /// Método construtor que inicializa todas as possíveis dependências do corpo da requisição
        /// </summary>
        public Request()
        {
            SetupReferences();
        }

        // Incializa as dependências da requisição
        private void SetupReferences()
        {
            // Recebe a data e hora de envio atuais.
            this.req_sent_date_hour = DateTime.Now;
            // Uma nova requisição sempre vai iniciar como ativa.
            this.req_active = true;
            this.req_excluded = false;
            this.req_status = "E";
        }

        // Seters e Geters
        public void SetId(int id) { this.req_id = id; }
        public int GetId() { return this.req_id; }
        public void SetApplication(string application) { this.req_application = application; }
        public string GetApplication() { return this.req_application; }
        public void SetPriority(string priority) { this.req_priority = priority; }
        public string GetPriority() { return this.req_priority; }
    }

}
