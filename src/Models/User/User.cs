using System;
using System.Collections;
using System.Collections.Generic;
using Voartec.Helpers;
using Voartec.Validators;

namespace Voartec.Models
{
    public class User
    {
        public int use_id { get; set; }
        public int use_ent_id { get; set; } // use_entity
        public string use_name { get; set; }
        public string use_code { get; set; }
        public string use_email { get; set; }
        public string use_sector { get; set; }
        public string use_function { get; set; }
        public string use_phone { get; set; }
        public string use_password { get; set; }
        public string use_image { get; set; }
        public bool use_active { get; set; }
        public bool use_excluded { get; set; }

        public User()
        {
            SetupReferences();
        }

        // Incializa as dependências do usuário
        private void SetupReferences()
        {
            this.use_image = "Default.png";
            this.use_active = true;
            this.use_excluded = false;
        }

        // Seters e Geters
        public void SetId(int id) { this.use_id = id; }
        public int GetId() { return this.use_id; }

        // Faz a validação dos campos do usuário
        public List<string> Validate(dynamic obj)
        {
            List<string> list_erros = new List<string>();
            Fluent fluent = new Fluent();
            DateHour dateHelper = new DateHour();

            if (fluent.IsNull(obj.use_id)) list_erros.Add("O campo " + this.DisplayName("use_id") + " deve ser informado.");
            if (fluent.IsEmpty(obj.use_id)) list_erros.Add("O campo " + this.DisplayName("use_id") + " deve ser informado.");
            if (fluent.IsNull(obj.use_entity)) list_erros.Add("O campo " + this.DisplayName("use_entity") + " deve ser informado.");
            if (fluent.IsEmpty(obj.use_entity)) list_erros.Add("O campo " + this.DisplayName("use_entity") + " deve ser informado.");
            if (fluent.IsNull(obj.use_name)) list_erros.Add("O campo " + this.DisplayName("use_name") + " deve ser informado.");
            if (fluent.IsEmpty(obj.use_name)) list_erros.Add("O campo " + this.DisplayName("use_name") + " deve ser informado.");
            if (fluent.IsNull(obj.use_code)) list_erros.Add("O campo " + this.DisplayName("use_code") + " deve ser informado.");
            if (fluent.IsNull(obj.use_email)) list_erros.Add("O campo " + this.DisplayName("use_email") + " deve ser informado.");
            if (fluent.IsNull(obj.use_sector)) list_erros.Add("O campo " + this.DisplayName("use_sector") + " deve ser informado.");
            if (fluent.IsNull(obj.use_function)) list_erros.Add("O campo " + this.DisplayName("use_function") + " deve ser informado.");
            if (fluent.IsNull(obj.use_phone)) list_erros.Add("O campo " + this.DisplayName("use_phone") + " deve ser informado.");
            if (fluent.IsNull(obj.use_password)) list_erros.Add("O campo " + this.DisplayName("use_password") + " deve ser informado.");
            if (fluent.IsNull(obj.use_image)) list_erros.Add("O campo " + this.DisplayName("use_image") + " deve ser informado.");
            if (fluent.IsNull(obj.use_active)) list_erros.Add("O campo " + this.DisplayName("use_active") + " deve ser informado.");
            if (fluent.IsEmpty(obj.use_active)) list_erros.Add("O campo " + this.DisplayName("use_active") + " deve ser informado.");
            if (fluent.IsNull(obj.use_excluded)) list_erros.Add("O campo " + this.DisplayName("use_excluded") + " deve ser informado.");
            if (fluent.IsEmpty(obj.use_excluded)) list_erros.Add("O campo " + this.DisplayName("use_excluded") + " deve ser informado.");

            if (fluent.HasMaxLen(obj.use_name, 40) == false) list_erros.Add("O campo " + this.DisplayName("use_name") + "  deve ter, no máximo, 40 caracteres.");
            if (fluent.HasMaxLen(obj.use_code, 30) == false) list_erros.Add("O campo " + this.DisplayName("use_code") + "  deve ter, no máximo, 30 caracteres.");
            if (fluent.HasMaxLen(obj.use_email, 100) == false) list_erros.Add("O campo " + this.DisplayName("use_email") + "  deve ter, no máximo, 100 caracteres.");
            if (fluent.HasMaxLen(obj.use_sector, 50) == false) list_erros.Add("O campo " + this.DisplayName("use_sector") + "  deve ter, no máximo, 50 caracteres.");
            if (fluent.HasMaxLen(obj.use_function, 50) == false) list_erros.Add("O campo " + this.DisplayName("use_function") + "  deve ter, no máximo, 50 caracteres.");
            if (fluent.HasMaxLen(obj.use_phone, 50) == false) list_erros.Add("O campo " + this.DisplayName("use_phone") + "  deve ter, no máximo, 50 caracteres.");
            if (fluent.HasMaxLen(obj.use_password, 40) == false) list_erros.Add("O campo " + this.DisplayName("use_password") + "  deve ter, no máximo, 40 caracteres.");
            if (fluent.HasMaxLen(obj.use_image, 2147483647) == false) list_erros.Add("O campo " + this.DisplayName("use_image") + "  deve ter, no máximo, 2147483647 caracteres.");
            return list_erros;
        }

        // Mostra o nome de acordo com o registro
        public string DisplayName(string name)
        {
            string result = "";
            if (name == "use_id") result = "Id";
            if (name == "use_entity") result = "Entidade";
            if (name == "use_name") result = "Nome";
            if (name == "use_code") result = "Codigo";
            if (name == "use_email") result = "Email";
            if (name == "use_sector") result = "Setor";
            if (name == "use_function") result = "Função";
            if (name == "use_phone") result = "Telefone";
            if (name == "use_password") result = "Senha";
            if (name == "use_image") result = "Imagem";
            if (name == "use_active") result = "Ativo";
            if (name == "use_excluded") result = "Excluido";
            return result;
        }
    }
}