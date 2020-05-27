using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Models
{
    public class Message
    {
        public int msg_id { get; set; }
        public int msg_user_from { get; set; }
        public int msg_user_to { get; set; }
        public string msg_source_type { get; set; }
        public int msg_source_key { get; set; }
        public string msg_text { get; set; }
        public DateTime? msg_sent_date_hour { get; set; }
        public bool msg_sent;
        public bool msg_read { get; set; }
        public bool msg_important { get; set; }
        public bool msg_excluded { get; set; }

        /// <summary>
        /// Método construtor que inicializa todas as possíveis dependências do corpo da requisição
        /// </summary>
        public Message()
        {
            SetupReferences();
        }

        // Incializa as dependências da mensagem
        private void SetupReferences()
        {
            // A mensagem recebe a data e hora de envio
            this.msg_sent_date_hour = DateTime.Now;

            // Toda mensagem inicia como não importante
            this.msg_important = false;

            // Toda mensagem inicia como não lida
            this.msg_read = false;

            // Toda mensagem inicia como ativa
            this.msg_excluded = false;
        }

        // Seters e Geters
        public void SetId(int id) { this.msg_id = id; }
        public int GetId() { return this.msg_id; }
        public void SetSourceKey(int key) { this.msg_source_key = key; }
        public int GetSourceKey() { return this.msg_source_key; }
        public void SetSourceType(string type) { this.msg_source_type = type; }
        public string GetSourceType() { return this.msg_source_type; }
    }
}