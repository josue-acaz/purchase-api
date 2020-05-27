using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Models
{
    public class ItemStatus
    {
        /// <summary>Status possíveis</summary>
        /// <status>ED</status> --> 1. Em digitação
        /// <status>AAT</status> --> 2. Aguardando Aprovação Técnica
        /// <status>ATN</status> --> 3. Aprovação Técnica Negada
        /// <status>ATC</status> --> 4. Aprovação Técnica Concedida
        /// <status>EC</status> --> 5. Em Cotação
        /// <status>AAC</status> --> 6. Aguardando Aprovação de Compra
        /// <status>ACN</status> --> 7. Aprovação de Compra Negada
        /// <status>CA</status> --> 8. Compra Aprovada
        /// <status>ET</status> --> 9. Em Trânsito
        /// <status>ENTRG</status> --> 10. Entregue
        /// <status>CANCL</status> --> 11. Cancelada
        public int its_id { get; set; }
        public string its_name { get; set; }
        public bool its_excluded { get; set; }

        /// <summary>Método construtor para inicialização de dependências</summary>
        public ItemStatus()
        {
            SetupReferences();
        }

        // Incializa as dependências da mensagem
        private void SetupReferences()
        {
            // Todo status de um determinado item inicia como ativo
            this.its_excluded = false;
        }

        // Seters and Geters
        public void SetId(int id) { this.its_id = id; }
        public int GetId() { return this.its_id; }
        public void SetStatusName(string status_name) { this.its_name = status_name; }
        public string GetStatusName() { return this.its_name; }
    }
}