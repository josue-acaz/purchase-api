using System;
using System.Collections;
using System.Collections.Generic;
using Voartec.Services;

namespace Voartec.Models
{
    public class RequestItem
    {
        public int itm_id { get; set; }
        public int itm_request_id { get; set; }
        public int itm_status_id { get; set; }
        public string itm_pn { get; set; }
        public int itm_quantity { get; set; }
        public int itm_approved_quantity { get; set; }
        public string itm_description { get; set; }
        public string itm_application { get; set; }
        public string itm_priority { get; set; }
        public DateTime itm_deadline { get; set; }
        public bool itm_active { get; set; }
        public bool itm_excluded { get; set; }

        // Acessar o serviço de status do item
        private ItemStatusService service = new ItemStatusService();

        /// <summary>Método construtor para inicialização de dependências</summary>
        public RequestItem()
        {
            SetupReferences();
        }

        // Incializa as dependências da mensagem
        private void SetupReferences()
        {
            // Todo item da requisição começa com o status <ED> EM DIGITAÇÃO
            this.itm_status_id = service.GetStatusIdByName("ED");
            // Todo item da requisição começa como ativo
            this.itm_active = true;
            this.itm_excluded = false;
        }

        // Seters and Geters
        public void SetId(int id) { this.itm_id = id; }
        public int GetId() { return this.itm_id; }
        public void SetRequestId(int id) { this.itm_request_id = id; }
        public int GetRequestId() { return this.itm_request_id; }
        public void SetApplication(string application) { this.itm_application = application; }
        public string GetApplication() { return this.itm_application; }
        public void SetPriority(string priority) { this.itm_priority = priority; }
        public string GetPriority() { return this.itm_priority; }
    }
}