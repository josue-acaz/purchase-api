using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Helpers
{
    public class ItemProgress
    {
        public int itp_id { get; set; }
        public string itp_description { get; set; }
        public DateTime? itp_date_hour { get; set; }
        public int itp_user_id { get; set; }
        public int itp_item_id { get; set; }
        public int itp_item_status_id { get; set; }
        public bool itp_active { get; set; }

        /// <summary>
        /// Método construtor que inicializa todas as possíveis dependências do corpo da requisição
        /// </summary>
        public ItemProgress()
        {
            SetupReferences();
        }

        // Incializa as dependências da mensagem
        private void SetupReferences()
        {
            // Recebe a data e hora do progresso atual
            this.itp_date_hour = DateTime.Now;
        }

        // Seters e Geters
        public void SetId(int id) { this.itp_id = id; }
        public int GetId() { return this.itp_id; }
    }
}