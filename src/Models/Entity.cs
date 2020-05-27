using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Models
{
    public class Entity
    {
        public int ent_id { get; set; }
        public string ent_name { get; set; }
        public string ent_official_name { get; set; }
        public string ent_address { get; set; }
        public string ent_address_number { get; set; }
        public string ent_complement { get; set; }
        public string ent_neighborhood { get; set; }
        public string ent_city { get; set; }
        public string ent_uf { get; set; }
        public string ent_cep { get; set; }
        public string ent_cnpj { get; set; }
        public string ent_ie { get; set; }
        public bool ent_active { get; set; }
        public bool ent_excluded { get; set; }

        public Entity()
        {
            SetupReferences();
        }

        private void SetupReferences()
        {
            this.ent_active = true;
            this.ent_excluded = false;
        }
    }
}